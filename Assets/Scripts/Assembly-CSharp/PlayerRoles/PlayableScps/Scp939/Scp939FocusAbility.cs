using System;
using System.Diagnostics;
using CameraShaking;
using CustomPlayerEffects;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939
{
    public class Scp939FocusAbility : ScpStandardSubroutine<Scp939Role>, IShakeEffect
    {
        [SerializeField]
        private float _transitionSpeed;

        [SerializeField]
        private AudioSource _focusInSource;

        [SerializeField]
        private AnimationCurve _cameraHeightOffset;

        [SerializeField]
        private AnimationCurve _cameraForwardOffset;

        [SerializeField]
        private float _cameraFov;

        private SoundtrackMute _muteEffect;
        private Scp939FocusKeySync _keySync;
        private Scp939ClawAbility _clawAbility;
        private Scp939LungeAbility _lungeAbility;

        private Transform _ownerTransform;

        public float _state;
        public bool _targetState;
        private bool _hitAnimationPlaying;

        private float _offsetMultiplier;
        private float _relativeFreezeRot;
        private byte _relativeWaypoint;

        private readonly AbilityCooldown _serverSendCooldown = new AbilityCooldown();
        private readonly Stopwatch _frozenSw = new Stopwatch();

        private const int RotationSpeed = 480;         
        private const float RotationRestartState = 0.9f;  
        private const float OffsetLerpSpeed = 15f;       
        private const float CamMinRadius = 0.16f;         
        private const float SourceDecaySpeed = 10f;    
        private const int AngleSyncAccuracy = 64;        
        private const float ResendCooldown = 1.5f;   

        private static readonly CachedLayerMask VisibilityMask;

        public event Action OnStateChanged;

        public float State
        {
            get => _state;
            set
            {
                value = Mathf.Clamp01(value);
                if (value != _state)
                {
                    _state = value;
                    OnStateChanged?.Invoke();
                }
            }
        }
        public bool TargetState
        {
            get
            {
                if (_targetState)
                    return true;
                return _lungeAbility.State == Scp939LungeState.Triggered;
            }
            private set
            {
                if (_targetState == value)
                    return;

                if (value && State == 0f)
                {
                    _relativeWaypoint = CurrentWaypointId;
                    _relativeFreezeRot = WaypointBase.GetRelativeRotation(_relativeWaypoint, _ownerTransform.rotation).eulerAngles.y;
                    _frozenSw.Restart();
                }

                _targetState = value;
                _serverSendCooldown.Clear();
            }
        }

        public float FrozenTime => (float)_frozenSw.Elapsed.TotalSeconds;

        public float FrozenRotation
        {
            get
            {
                Quaternion worldRot = WaypointBase.GetWorldRotation(
                    _relativeWaypoint,
                    Quaternion.Euler(Vector3.up * _relativeFreezeRot));
                return worldRot.eulerAngles.y;
            }
        }

        public float AngularDeviation
        {
            get
            {
                if (State <= 0f)
                    return 0f;
                return Mathf.DeltaAngle(FrozenRotation, _ownerTransform.eulerAngles.y);
            }
        }

        private byte CurrentWaypointId => new RelativePosition(base.ScpRole.FpcModule.Position).WaypointId;

        private bool IsAvailable => base.ScpRole.FpcModule.IsGrounded;

        private void Update()
        {
            State += Time.deltaTime * (TargetState ? _transitionSpeed : (0f - _transitionSpeed));

            if (!_targetState && _lungeAbility.State != Scp939LungeState.Triggered && _focusInSource != null)
            {
                _focusInSource.volume -= Time.deltaTime * SourceDecaySpeed;
            }

            if (!NetworkServer.active)
                return;

            TargetState = IsAvailable
                          && _keySync.FocusKeyHeld
                          && _clawAbility.Cooldown.IsReady
                          && (_targetState || State == 0f);

            _muteEffect.IsEnabled = TargetState;

            UpdateRelativeRotation();

            if (_serverSendCooldown.IsReady)
            {
                ServerSendRpc(toAll: true);
                _serverSendCooldown.Trigger(ResendCooldown);
            }
        }

        private void UpdateRelativeRotation()
        {
            if (!TargetState)
                return;

            byte currentWaypointId = CurrentWaypointId;
            if (currentWaypointId == _relativeWaypoint)
                return;

            _relativeFreezeRot = WaypointBase.GetRelativeRotation(
                currentWaypointId,
                Quaternion.Euler(Vector3.up * FrozenRotation)).eulerAngles.y;

            _relativeWaypoint = currentWaypointId;
            _serverSendCooldown.Clear();
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _keySync);
            GetSubroutine(out _clawAbility);
            GetSubroutine(out _lungeAbility);

            _lungeAbility.OnStateChanged += OnLungeStateChanged;
        }

        private void OnLungeStateChanged(Scp939LungeState newState)
        {
            _hitAnimationPlaying = newState == Scp939LungeState.LandHit;

            if (newState != Scp939LungeState.None)
            {
                if (_targetState)
                {
                    TargetState = false;
                }
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);

            int encodedAngle = Mathf.RoundToInt(_relativeFreezeRot * AngleSyncAccuracy) + 1;

            if (!_targetState)
                encodedAngle = -encodedAngle;

            NetworkWriterExtensions.WriteShort(writer, (short)encodedAngle);
            writer.WriteByte(_relativeWaypoint);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            short encodedAngle = NetworkReaderExtensions.ReadShort(reader);
            _targetState = encodedAngle > 0;

            _relativeWaypoint = reader.ReadByte();

            _relativeFreezeRot = (Mathf.Abs(encodedAngle) - 1f) / AngleSyncAccuracy;

            if (!_targetState)
            {
                if (_focusInSource != null)
                {
                    if (!_focusInSource.isPlaying)
                        _focusInSource.Play();
                    else
                        _focusInSource.timeSamples = 0;

                    _focusInSource.volume = 1f;
                }
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            _ownerTransform = base.Owner.transform;

            _offsetMultiplier = 1f;

            CameraShakeController.AddEffect(this);

            if (NetworkServer.active)
            {
                _muteEffect = base.Owner.playerEffectsController.GetEffect<SoundtrackMute>();
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _state = 0f;
            _targetState = false;
        }

        public bool GetEffect(ReferenceHub ply, out Quaternion targetShakeRotation, out Vector3 positionOffset,
            out float fovPercent, out float verticalJump, out float horizontalJump)
        {
            targetShakeRotation = Quaternion.identity;
            positionOffset = Vector3.zero;
            fovPercent = 1f;
            verticalJump = 0f;
            horizontalJump = 0f;

            if (base.ScpRole == null)
                return false;

            bool isLocalOrSpectated = base.Owner.isLocalPlayer
                                      || SpectatorNetworking.IsLocallySpectated(base.Owner);

            if (!isLocalOrSpectated || _state == 0f)
                return true;

            bool fullOffset = base.Owner.isLocalPlayer || _lungeAbility.State != Scp939LungeState.Triggered;

            _offsetMultiplier = Mathf.Lerp(_offsetMultiplier, fullOffset ? 1f : 0f, Time.deltaTime * OffsetLerpSpeed);

            float heightOffset = _cameraHeightOffset.Evaluate(_state) * _offsetMultiplier;
            float forwardOffset = _cameraForwardOffset.Evaluate(_state) * _offsetMultiplier;

            Vector3 forward = _ownerTransform.forward;
            Vector3 castOrigin = ply.PlayerCameraReference.position + Vector3.up * heightOffset;

            if (Physics.SphereCast(castOrigin, CamMinRadius, forward, out RaycastHit hit,
                forwardOffset + CamMinRadius, VisibilityMask))
            {
                forwardOffset = Mathf.Max(0f, hit.distance - CamMinRadius);
            }

            positionOffset = Vector3.up * heightOffset + forward * forwardOffset;

            fovPercent = Mathf.SmoothStep(fovPercent, _cameraFov + 1f, _state);

            if (_hitAnimationPlaying && base.Owner.isLocalPlayer)
            {
                verticalJump = -Time.deltaTime * RotationSpeed;
                if (_state < 0.9f)
                    _hitAnimationPlaying = false;
            }

            return true;
        }

        public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
        {
            bool result = GetEffect(ply, out Quaternion rot, out Vector3 pos, out float fov, out float vJump, out float hJump);
            shakeValues = new ShakeEffectValues(rot, null, pos, fov, vJump, hJump);
            return result;
        }

        static Scp939FocusAbility()
        {
            string[] layers = new string[]
            {
                "Default",
                "Glass",
                "CCTV",
                "Door",
                "Locker"
            };

            VisibilityMask = new CachedLayerMask(layers);
        }
    }
}
