using System.Collections.Generic;
using System.Diagnostics;
using Interactables.Interobjects;
using Mirror;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp096
{
    public class Scp096MovementModule : FirstPersonMovementModule
    {
        [SerializeField]
        private float _jumpSpeedRage;

        private const float SlowedSpeed = 2.55f;
        private const float NormalSpeed = 3.64f;
        private const float RageSpeed = 8f;
        private const float ChargeSpeed = 18.5f;
        private const float PryGateDuration = 2f;
        private const float AngleAdjustSpeed = 120f;

        private Scp096StateController _stateController;
        private float _gateLookAngle;
        private float _normalJumpSpeed;

        private readonly Stopwatch _gatePrySw = Stopwatch.StartNew();
        private readonly AnimationCurve _gatePryX = new AnimationCurve(TemplateKeyframes);
        private readonly AnimationCurve _gatePryZ = new AnimationCurve(TemplateKeyframes);
        private readonly List<Transform> _pryablePoints = new List<Transform>();

        private static readonly Keyframe[] TemplateKeyframes = new Keyframe[3]
        {
            new Keyframe(0f, 1f, 0f, 0f, 0f, 0.3f),
            new Keyframe(0.35f, 0.2f, 0f, 0f, 0.5f, 0.4f),
            new Keyframe(1f, 0f, 0f, 0f, 0.2f, 0f)
        };

        protected override FpcMotor NewMotor => new Scp096Motor(Hub, Role as Scp096Role);

        private float MovementSpeed
        {
            set
            {
                SneakSpeed = value;
                WalkSpeed = value;
                SprintSpeed = value;
            }
        }

        public override bool LockMovement
        {
            get
            {
                if (!Role.IsLocalPlayer)
                    return false;

                Scp096AbilityState abilityState = _stateController.AbilityState;
                if (abilityState - 3 <= Scp096AbilityState.TryingNotToCry)
                    return true;

                return base.LockMovement;
            }
        }

        private void Awake()
        {
            _normalJumpSpeed = JumpSpeed;
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            _stateController = (Role as Scp096Role).StateController;
        }

        protected override void UpdateMovement()
        {
            UpdateSpeedAndOverrides();
            base.UpdateMovement();
        }

        private void UpdateSpeedAndOverrides()
        {
            switch (_stateController.AbilityState)
            {
                case Scp096AbilityState.Charging:
                    MovementSpeed = ChargeSpeed;
                    (Motor as Scp096Motor).SetOverride(transform.forward);
                    return;
                case Scp096AbilityState.PryingGate:
                    MovementSpeed = NormalSpeed;
                    UpdateGatePrying();
                    return;
            }

            switch (_stateController.RageState)
            {
                case Scp096RageState.Distressed:
                case Scp096RageState.Calming:
                    MovementSpeed = SlowedSpeed;
                    JumpSpeed = _normalJumpSpeed;
                    break;
                case Scp096RageState.Enraged:
                    MovementSpeed = RageSpeed;
                    JumpSpeed = _jumpSpeedRage;
                    break;
                default:
                    MovementSpeed = NormalSpeed;
                    JumpSpeed = _normalJumpSpeed;
                    break;
            }
        }

        private void UpdateGatePrying()
        {
            float elapsed = (float)_gatePrySw.Elapsed.TotalSeconds;

            if (elapsed > PryGateDuration)
            {
                if (NetworkServer.active)
                {
                    _stateController.SetAbilityState(Scp096AbilityState.None);
                }
                return;
            }

            if (!Role.IsLocalPlayer)
                return;

            elapsed /= PryGateDuration;
            Position = new Vector3(_gatePryX.Evaluate(elapsed), Position.y, _gatePryZ.Evaluate(elapsed));
            MouseLook.CurrentHorizontal = Mathf.MoveTowardsAngle(MouseLook.CurrentHorizontal, _gateLookAngle, Time.deltaTime * AngleAdjustSpeed);
            MouseLook.CurrentVertical = Mathf.MoveTowardsAngle(MouseLook.CurrentVertical, 0f, Time.deltaTime * AngleAdjustSpeed);
        }

        private void SetGatePryCurves(int index, Vector3 pos)
        {
            Keyframe[] keysX = _gatePryX.keys;
            keysX[index].value = pos.x;
            _gatePryX.keys = keysX;

            Keyframe[] keysZ = _gatePryZ.keys;
            keysZ[index].value = pos.z;
            _gatePryZ.keys = keysZ;
        }

        public void SetTargetGate(PryableDoor door)
        {
            if (door.PryPositions.Length == 0)
                return;

            _gatePrySw.Restart();

            if (Role.IsLocalPlayer)
            {
                SetGatePryCurves(0, Position);

                _pryablePoints.Clear();
                _pryablePoints.AddRange(door.PryPositions);
                _pryablePoints.Sort(CompareByDistanceToPlayer);

                Transform nearest = _pryablePoints[0];
                Transform farthest = _pryablePoints[_pryablePoints.Count - 1];
                SetGatePryCurves(1, nearest.position);
                SetGatePryCurves(2, farthest.position);

                Vector3 direction = (farthest.position - nearest.position).normalized;
                _gateLookAngle = Vector3.Angle(direction, Vector3.forward) * Mathf.Sign(Vector3.Dot(direction, Vector3.right));
            }

            if (NetworkServer.active)
            {
                _stateController.SetAbilityState(Scp096AbilityState.PryingGate);
            }
        }

        private int CompareByDistanceToPlayer(Transform x, Transform y)
        {
            float distX = (Position - x.position).sqrMagnitude;
            float distY = (Position - y.position).sqrMagnitude;
            return distX.CompareTo(distY);
        }
    }
}
