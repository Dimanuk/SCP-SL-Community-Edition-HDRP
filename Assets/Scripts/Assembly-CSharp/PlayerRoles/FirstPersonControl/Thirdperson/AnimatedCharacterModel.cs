using System;
using System.Diagnostics;
using AudioPooling;
using CustomPlayerEffects;
using GameObjectPools;
using PlayerRoles.Spectating;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl.Thirdperson
{
    public class AnimatedCharacterModel : CharacterModel, IPoolResettable, IPoolSpawnable
    {
        private enum FootstepLoudness
        {
            Civilian = 8,
            FoundationForces = 12,
            Chaos = 30,
            Scp = 35
        }

        public static Action<AnimatedCharacterModel, float> OnFootstepPlayed;

        private static readonly int HashForward = Animator.StringToHash("Forward");
        private static readonly int HashStrafe = Animator.StringToHash("Strafe");
        private static readonly int HashSpeed = Animator.StringToHash("Speed");

        private readonly Stopwatch _lastTouchdownSw = Stopwatch.StartNew();

        private int _lastFootstep;
        private bool _forceUpdate;

        private const float SilentVelocityMultiplier = 0.7f;
        private const float SprintingLoudnessMultiplier = 2f;
        private const float MinimalFootstepSoundCooldown = 0.2f;
        private const float SpawnGroundedSuppression = 0.3f;

        [Header("Animation settings")]
        [SerializeField]
        private float _firstpersonDampTime;

        [SerializeField]
        private float _thirdpersonDampTime;

        [SerializeField]
        private AnimationCurve _walkVelocityScale;

        [Header("Footsteps")]
        [SerializeField]
        private AudioClip[] _footstepClips;

        [Range(0f, 1f)]
        [SerializeField]
        private float[] _footstepTimes;

        [SerializeField]
        private FootstepLoudness _footstepLoudness;

        public ModelSharedSettings SharedSettings;

        public AudioClip RandomFootstep => _footstepClips.RandomItem();

        public Vector3 HeadBobPosition { get; protected set; }

        public bool IsTracked
        {
            get
            {
                if (Pooled)
                    return false;

                if (OwnerHub.isLocalPlayer)
                    return true;

                return OwnerHub.IsLocallySpectated();
            }
        }

        internal Animator Animator { get; private set; }

        internal AnimatorOverrideController AnimatorOverride { get; private set; }

        protected FirstPersonMovementModule FpcModule { get; private set; }

        protected PlayerRoleBase Role { get; private set; }

        protected virtual float FootstepLoudnessDistance
        {
            get
            {
                float loudness = (float)_footstepLoudness;

                if (FpcModule.CurrentMovementState == PlayerMovementState.Sprinting)
                    loudness *= SprintingLoudnessMultiplier;

                return loudness;
            }
        }

        protected virtual bool FootstepPlayable
        {
            get
            {
                if (!FpcModule.IsGrounded || !FpcModule.Motor.MovementDetected)
                    return false;

                float sneakingVelocity = FpcModule.VelocityForState(PlayerMovementState.Sneaking, applyCrouch: false);

                if (FpcModule.MaxMovementSpeed <= sneakingVelocity)
                    return false;

                float threshold = sneakingVelocity * SilentVelocityMultiplier;

                return FpcModule.Motor.Velocity.SqrMagnitudeIgnoreY() >= threshold * threshold;
            }
        }

        public float WalkCycle
        {
            get
            {
                float normalizedTime = Animator.GetCurrentAnimatorStateInfo(WalkLayer).normalizedTime;
                return float.IsNaN(normalizedTime) ? 0f : normalizedTime - (int)normalizedTime;
            }
        }

        protected virtual int WalkLayer => 0;

        protected virtual bool LandingFootstepPlayable => true;

        public event Action<AudioSource> OnFootstepAudioSpawned;

        protected override void Awake()
        {
            base.Awake();
            Animator = SetupAnimator();
        }

        protected virtual void Update()
        {
            if (Pooled)
                return;

            float dampTime = OwnerHub.isLocalPlayer ? _firstpersonDampTime : _thirdpersonDampTime;

            Vector2 movementDirection;
            float normalizedVelocity;

            if (!FpcModule.IsGrounded)
            {
                movementDirection = Vector2.zero;
                normalizedVelocity = 0f;
            }
            else
            {
                Vector3 localVelocity = CachedTransform.InverseTransformDirection(FpcModule.Motor.Velocity);
                Vector2 flatVelocity = new Vector2(localVelocity.x, localVelocity.z);
                float magnitude = flatVelocity.magnitude;

                movementDirection = magnitude <= float.Epsilon ? Vector2.zero : flatVelocity / magnitude;
                float walkSpeed = FpcModule.WalkSpeed;
                normalizedVelocity = walkSpeed <= 0f ? 1f : magnitude / walkSpeed;
            }

            UpdateHeadBob(Animator.GetCurrentAnimatorStateInfo(WalkLayer).normalizedTime);
            UpdateFootsteps(Animator.GetCurrentAnimatorStateInfo(WalkLayer).normalizedTime);
            UpdateAnimatorParameters(movementDirection, normalizedVelocity, dampTime);
        }

        protected virtual Animator SetupAnimator()
        {
            Animator animator = GetComponent<Animator>();
            AnimatorOverride = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = AnimatorOverride;
            return animator;
        }

        protected virtual AudioSource PlayFootstepAudioClip(AudioClip clip, float distance, float volume)
        {
            return AudioSourcePoolManager.PlaySound(clip, transform, distance, volume, FalloffType.Footstep);
        }

        private void UpdateHeadBob(float time)
        {
            if (!IsTracked)
                return;

            float strafe = Animator.GetFloat(HashStrafe);
            float forward = Animator.GetFloat(HashForward);
            Vector2 animDirection = new Vector2(strafe, forward);

            HeadBobPosition = SharedSettings.GetHeadBob(time, animDirection);
        }

        private void UpdateFootsteps(float time)
        {
            time -= (int)time; 

            int count = _footstepTimes.Length;
            if (count == 0)
                return;

            if (_lastFootstep < count)
            {
                if (time >= _footstepTimes[_lastFootstep])
                {
                    _lastFootstep++;
                    if (FootstepPlayable)
                        PlayFootstep();
                }
            }
            else if (time < _footstepTimes[0])
            {
                _lastFootstep = 0;
            }
        }

        private void OnGrounded()
        {
            if (OwnerHub.roleManager.CurrentRole.ActiveTime < SpawnGroundedSuppression)
                return;

            if (LandingFootstepPlayable)
            {
                PlayFootstep();
                _lastTouchdownSw.Restart();

                if (IsTracked)
                    SharedSettings.PlayLandingAnimation();
            }
        }

        private void PlayFootstep()
        {
            float distance = FootstepLoudnessDistance;
            float volumeMultiplier = 1f;
            bool playDefault = true;

            var effects = OwnerHub.playerEffectsController.AllEffects;
            for (int i = 0; i < effects.Length; i++)
            {
                if (effects[i].IsEnabled && effects[i] is IFootstepEffect footstepEffect)
                {
                    float result = footstepEffect.ProcessFootstepOverrides(distance);
                    if (result >= 0f)
                        playDefault = false;

                    volumeMultiplier = Mathf.Min(volumeMultiplier, result);
                }
            }

            if (playDefault || volumeMultiplier >= 0f)
            {

                PlayFootstepAudioClip(RandomFootstep, distance, volumeMultiplier);
                OnFootstepPlayed?.Invoke(this, distance);
            }
        }

        public virtual void UpdateAnimatorParameters(Vector2 movementDirection, float normalizedVelocity, float dampTime)
        {
            float speedValue = _walkVelocityScale.Evaluate(normalizedVelocity);
            movementDirection *= normalizedVelocity;

            if (_forceUpdate)
            {
                Animator.SetFloat(HashForward, movementDirection.y);
                Animator.SetFloat(HashStrafe, movementDirection.x);
                Animator.SetFloat(HashSpeed, speedValue);
            }
            else
            {
                Animator.SetFloat(HashForward, movementDirection.y, dampTime, Time.deltaTime);
                Animator.SetFloat(HashStrafe, movementDirection.x, dampTime, Time.deltaTime);
                Animator.SetFloat(HashSpeed, speedValue, dampTime, Time.deltaTime);
            }
        }

        public virtual void ForceUpdate()
        {
            _forceUpdate = true;
            Update();
            _forceUpdate = false;
        }

        public override void ResetObject()
        {
            base.ResetObject();

            if (FpcModule != null)
            {
                FpcModule.OnGrounded = (Action)Delegate.Remove(FpcModule.OnGrounded, new Action(OnGrounded));
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();

            Role = OwnerHub.roleManager.CurrentRole;
            FpcModule = (Role as IFpcRole)?.FpcModule;

            if (FpcModule != null)
            {
                FpcModule.OnGrounded = (Action)Delegate.Combine(FpcModule.OnGrounded, new Action(OnGrounded));
            }

            Animator.Rebind();

            foreach (var hitbox in Hitboxes)
            {
                HitboxIdentity.Instances.Add(hitbox);
                hitbox.SetColliders(!OwnerHub.isLocalPlayer);
            }
        }

        public override void OnTreadmillInitialized()
        {
            base.OnTreadmillInitialized();
            Animator = SetupAnimator();
        }
    }
}