using System;
using System.Diagnostics;
using CustomPlayerEffects;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.Spectating;
using UnityEngine;
using AudioPooling;
using PlayerRoles.PlayableScps.Scp939;

namespace PlayerRoles.PlayableScps.Scp939
{
    public class Scp939Model : AnimatedCharacterModel
    {
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private AnimationCurve _focusOverrideAnim;

        [SerializeField]
        private AnimationCurve _tiltOverTime;

        [SerializeField]
        private AnimationCurve _focusParamsCorrectionCurve;

        [SerializeField]
        private AudioClip[] _damagedVariants;

        [SerializeField]
        private AudioClip _cloudPlaceSound;

        [SerializeField]
        private float _tiltLerp;

        [SerializeField]
        private float _fadeSpeed;

        [SerializeField]
        private Vector2 _footstepPitchRand;

        [SerializeField]
        private float _amnesiaVisibleRange;

        private Scp939Role _scp939;
        private Scp939ClawAbility _clawAbility;
        private Scp939FocusAbility _focusAbility;
        private Scp939LungeAbility _lungeAbility;
        private Scp939AmnesticCloudAbility _amnesticAbility;

        private Transform _trModel;
        private Transform _trHub;

        private bool _prevFocus;
        private bool _isLunging;
        private float _curTilt;

        private readonly Stopwatch _lungeStopwatch = new Stopwatch();
        private readonly Stopwatch _fadeoutStopwatch = Stopwatch.StartNew();

        private const int FocusOverrideLayer = 4;
        private const int FocusHeadDirLayer = 6;
        private const float FocusHeadFadeTime = 0.4f;
        private const float FocusRotateRate = 3f;
        private const float LungeRotateSpeed = 7.5f;
        private const float DamagedSoundRange = 19f;
        private const float CloudSoundRange = 8f;
        private const float HiddenHeight = -3000f;
        private const float FullVisCooldown = 30f;

        private static readonly int GroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int ClawHash = Animator.StringToHash("Claw");
        private static readonly int FocusStateHash = Animator.StringToHash("Focus");
        private static readonly int FocusHeadDirHash = Animator.StringToHash("FocusDirection");
        private static readonly int LungeStateHash = Animator.StringToHash("LungeState");
        private static readonly int LungeTriggerHash = Animator.StringToHash("LungeTrigger");
        private static readonly int DamagedVariantHash = Animator.StringToHash("DamagedVariant");
        private static readonly int DamagedTriggerHash = Animator.StringToHash("DamagedTrigger");
        private static readonly int AmnesticChargingHash = Animator.StringToHash("AmnesticCharging");
        private static readonly int AmnesticTriggerHash = Animator.StringToHash("AmnesticCreated");

        private bool Visible
        {
            get
            {
                if (!SpectatorTargetTracker.TryGetTrackedPlayer(out var hub) && !ReferenceHub.TryGetLocalHub(out hub))
                    return true;

                if (!(hub.roleManager.CurrentRole is HumanRole humanRole))
                    return true;

                Vector3 position = humanRole.FpcModule.Position;
                if (Vector3.Distance(position, _scp939.FpcModule.Position) < _amnesiaVisibleRange)
                    return true;

                if (!hub.playerEffectsController.TryGetEffect<AmnesiaVision>(out var playerEffect))
                    return true;

                if (playerEffect.IsEnabled)
                    return false;

                if (playerEffect.LastActive < FullVisCooldown)
                    return true;

                foreach (Scp939AmnesticCloudInstance activeInstance in Scp939AmnesticCloudInstance.ActiveInstances)
                {
                    if (activeInstance.IsInArea(activeInstance.SourcePosition, position))
                        return false;
                }

                return true;
            }
        }

        // IL: cmp CurrentMovementState, 3 (Sprinting)
        protected override bool FootstepPlayable
        {
            get
            {
                if (base.FootstepPlayable)
                    return base.FpcModule.CurrentMovementState == PlayerMovementState.Sprinting;
                return false;
            }
        }

        protected override int WalkLayer => 5;

        private void PlayClawAttack(AttackResult attackRes)
        {
            if (attackRes != AttackResult.None)
                _animator.SetTrigger(ClawHash);
        }

        private void ProcessLungeState(Scp939LungeState newState)
        {
            switch (newState)
            {
                case Scp939LungeState.Triggered:
                    _lungeStopwatch.Restart();
                    break;
                case Scp939LungeState.None:
                    _isLunging = false;
                    return;
            }

            _isLunging = true;
            _animator.SetInteger(LungeStateHash, (int)newState);
            _animator.SetTrigger(LungeTriggerHash);
        }

        private void OnSpectatorTargetChanged()
        {
            ForceFade(1f);
        }

        private void UpdateFade()
        {
            ForceFade(_fadeSpeed * Time.deltaTime);
        }

        private void ForceFade(float delta)
        {
            Fade += Visible ? delta : -delta;

            if (!(Fade > 0f) && !base.Role.IsLocalPlayer && !NetworkServer.active)
            {
                _fadeoutStopwatch.Restart();
                base.FpcModule.Position = Vector3.up * HiddenHeight;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _trModel = base.transform;
        }

        protected override void Update()
        {
            if (base.FpcModule == null || base.Animator == null
                || _focusAbility == null || _amnesticAbility == null)
                return;

            base.Update();
            base.Animator.SetBool(GroundedHash, base.FpcModule.IsGrounded);
            base.Animator.SetBool(AmnesticChargingHash, _amnesticAbility.TargetState);

            float state = _focusAbility.State;
            base.Animator.SetFloat(FocusStateHash, state);
            base.Animator.SetLayerWeight(FocusHeadDirLayer, state);

            if (_isLunging)
                base.Animator.SetLayerWeight(FocusOverrideLayer, 1f);
            else
                base.Animator.SetLayerWeight(FocusOverrideLayer, _focusOverrideAnim.Evaluate(state));
        }

        public void PlayDamagedEffect(int rand)
        {
            rand %= _damagedVariants.Length;
            _animator.SetFloat(DamagedVariantHash, rand);
            _animator.SetTrigger(DamagedTriggerHash);

            AudioSourcePoolManager.PlaySound(
                _damagedVariants[rand],
                base.transform,
                DamagedSoundRange,
                1f,
                FalloffType.Exponential,
                AudioMixerChannelType.NoDucking);
        }

        public void PlayCloudRelease()
        {
            _animator.SetTrigger(AmnesticTriggerHash);
            AudioSourcePoolManager.PlaySound(_cloudPlaceSound, base.transform, CloudSoundRange);
        }

        public override void UpdateAnimatorParameters(Vector2 movementDirection, float normalizedVelocity, float dampTime)
        {
            if (_focusAbility.State > 0f)
            {
                float b = _focusParamsCorrectionCurve.Evaluate(_focusAbility.State);
                float f = Vector3.Dot(movementDirection.normalized, Vector2.up);
                normalizedVelocity *= Mathf.Lerp(1f, b, Mathf.Abs(f));
            }

            base.UpdateAnimatorParameters(movementDirection, normalizedVelocity, dampTime);
        }

        private void LateUpdate()
        {
            if (_lungeAbility == null || _focusAbility == null || _trModel == null)
                return;

            float t = Time.deltaTime * _tiltLerp;

            if (_lungeAbility.State == Scp939LungeState.Triggered)
            {
                double totalSeconds = _lungeStopwatch.Elapsed.TotalSeconds;
                float b = _tiltOverTime.Evaluate((float)totalSeconds);
                _curTilt = Mathf.Lerp(_curTilt, b, t);
            }
            else
            {
                _curTilt = Mathf.Lerp(_curTilt, 0f, t);
            }

            if (_focusAbility.State == 0f)
            {
                if (_prevFocus)
                {
                    _trModel.localRotation = Quaternion.identity;
                    _prevFocus = false;
                }
                return;
            }

            if (!_prevFocus)
            {
                _prevFocus = true;
                return;
            }

            float t2;
            if (_isLunging)
            {
                double totalSeconds2 = _lungeStopwatch.Elapsed.TotalSeconds;
                t2 = 1f - (float)totalSeconds2 * LungeRotateSpeed;
            }
            else
            {
                t2 = _focusAbility.State * FocusRotateRate;
            }

            Quaternion b2 = Quaternion.Euler(0f, _focusAbility.FrozenRotation, 0f);
            _trModel.rotation = Quaternion.Slerp(_trHub.rotation, b2, t2);
            _trModel.Rotate(Vector3.right, _curTilt, Space.Self);

            float value = Mathf.DeltaAngle(_trHub.eulerAngles.y, _trModel.eulerAngles.y);
            base.Animator.SetFloat(FocusHeadDirHash, value, FocusHeadFadeTime, Time.deltaTime);
        }

        protected override Animator SetupAnimator()
        {
            return _animator;
        }

        protected override AudioSource PlayFootstepAudioClip(AudioClip clip, float dis, float vol)
        {
            AudioSource audioSource = base.PlayFootstepAudioClip(clip, dis, vol);
            audioSource.pitch = UnityEngine.Random.Range(_footstepPitchRand.x, _footstepPitchRand.y);
            return audioSource;
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            _trHub = base.OwnerHub.transform;
            _scp939 = base.OwnerHub.roleManager.CurrentRole as Scp939Role;

            _scp939.SubroutineModule.TryGetSubroutine(out _clawAbility);
            _scp939.SubroutineModule.TryGetSubroutine(out _focusAbility);
            _scp939.SubroutineModule.TryGetSubroutine(out _lungeAbility);
            _scp939.SubroutineModule.TryGetSubroutine(out _amnesticAbility);

            _clawAbility.OnAttacked += PlayClawAttack;
            _lungeAbility.OnStateChanged += ProcessLungeState;
            FirstPersonMovementModule.OnPositionUpdated += UpdateFade;

            SpectatorTargetTracker.OnTargetChanged = (Action)Delegate.Combine(
                SpectatorTargetTracker.OnTargetChanged,
                new Action(OnSpectatorTargetChanged));
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _clawAbility.OnAttacked -= PlayClawAttack;
            _lungeAbility.OnStateChanged -= ProcessLungeState;
            _curTilt = 0f;
            _prevFocus = false;
            FirstPersonMovementModule.OnPositionUpdated -= UpdateFade;

            SpectatorTargetTracker.OnTargetChanged = (Action)Delegate.Remove(
                SpectatorTargetTracker.OnTargetChanged,
                new Action(OnSpectatorTargetChanged));
        }

        private void OnDestroy()
        {
            // Static event subscriptions survive direct destruction (scene reload skips
            // ResetObject); a dead handler throwing also blocks every other subscriber.
            FirstPersonMovementModule.OnPositionUpdated -= UpdateFade;

            SpectatorTargetTracker.OnTargetChanged = (Action)Delegate.Remove(
                SpectatorTargetTracker.OnTargetChanged,
                new Action(OnSpectatorTargetChanged));
        }
    }
}
