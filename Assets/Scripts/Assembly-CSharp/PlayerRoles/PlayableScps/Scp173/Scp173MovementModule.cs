using System.Diagnostics;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp173
{
    public class Scp173MovementModule : FirstPersonMovementModule
    {
        private float _normalSpeed;
        private float _fastSpeed;
        private float _observerSpeed;
        private float _jumpSpeed;

        private Scp173Role _role;
        private Scp173BreakneckSpeedsAbility _breakneckSpeeds;
        private Scp173ObserversTracker _observersTracker;

        private static int _snapMask;

        private readonly Stopwatch _lookStopwatch = Stopwatch.StartNew();

        private const float ObserverSpeedMultiplier = 2f;
        private const float ServerStopTime = 0.4f;
        private const int GlassLayerMask = 16384;
        private const float GlassRaycastDis = 0.3f;
        private const float RaycastFloorHeight = 3.6f;
        private const float RaycastCeilHeight = 7.2f;
        private const float RaycastPilotRadius = 0.025f;
        private const float RaycastFloorDot = 0.15f;
        private const float RaycastCcRadiusMultiplier = 1.2f;
        private const float RaycastStabilityRadiusRatio = 0.5f;
        private const float RaycastStabilityDistance = 0.5f;

        private float MovementSpeed
        {
            set
            {
                SneakSpeed = value;
                WalkSpeed = value;
                SprintSpeed = value;
                JumpSpeed = (_normalSpeed > value) ? 0f : _jumpSpeed;
            }
        }

        private float TargetSpeed
        {
            get
            {
                if (_observersTracker != null && _observersTracker.IsObserved)
                    return 0f;
                if (_breakneckSpeeds != null && _breakneckSpeeds.IsActive)
                    return _fastSpeed;
                return _normalSpeed;
            }
        }

        private float ServerSpeed
        {
            get
            {
                float targetSpeed = TargetSpeed;
                if (targetSpeed > 0f)
                {
                    _lookStopwatch.Restart();
                    return targetSpeed;
                }
                if (_lookStopwatch.Elapsed.TotalSeconds < ServerStopTime)
                    return _normalSpeed;
                return 0f;
            }
        }

        private static int TpMask
        {
            get
            {
                if (_snapMask != 0)
                    return _snapMask;

                int playerLayer = LayerMask.NameToLayer("Player");
                for (int i = 0; i < 32; i++)
                {
                    if (!Physics.GetIgnoreLayerCollision(playerLayer, i))
                    {
                        _snapMask |= 1 << i;
                    }
                }
                return _snapMask;
            }
        }

        private void Awake()
        {
            _fastSpeed = SprintSpeed;
            _normalSpeed = WalkSpeed;
            _jumpSpeed = JumpSpeed;
            _observerSpeed = SprintSpeed * ObserverSpeedMultiplier;

            _role = GetComponent<Scp173Role>();

            if (_role != null && _role.SubroutineModule != null)
            {
                _role.SubroutineModule.TryGetSubroutine(out _breakneckSpeeds);
                _role.SubroutineModule.TryGetSubroutine(out _observersTracker);
            }
        }

        protected override void UpdateMovement()
        {
            if (_role != null && _role.IsLocalPlayer)
            {
                MovementSpeed = TargetSpeed;
            }
            else if (NetworkServer.active)
            {
                MovementSpeed = ServerSpeed;
            }
            else
            {
                MovementSpeed = _observerSpeed;
            }

            base.UpdateMovement();
            UpdateGlassBreaking();
        }

        private void UpdateGlassBreaking()
        {
            if (!NetworkServer.active)
                return;

            if (_breakneckSpeeds == null || !_breakneckSpeeds.IsActive)
                return;

            if (Motor == null || CharController == null)
                return;

            Vector3 moveDirection = Motor.MoveDirection;
            float radius = CharController.radius;
            float maxDistance = radius + GlassRaycastDis;

            if (!Physics.Raycast(Position, moveDirection, out RaycastHit hitInfo, maxDistance, GlassLayerMask))
                return;

            if (!hitInfo.collider.TryGetComponent(out BreakableWindow window))
                return;

            if (_role == null)
                return;

            window.Damage(window.health, _role.DamageHandler, Vector3.zero);
        }

        public bool TryGetTeleportPos(float maxDis, out Vector3 pos, out float usedDistance)
        {
            pos = Vector3.zero;
            usedDistance = 0f;

            if (CharacterControllerSettings == null)
                return false;

            if (Hub == null || Hub.PlayerCameraReference == null)
                return false;

            float radius = CharacterControllerSettings.Radius;
            float num = radius * RaycastCcRadiusMultiplier;

            Vector3 position = Hub.PlayerCameraReference.position;
            Vector3 forward = Hub.PlayerCameraReference.forward;

            RaycastHit hitInfo;
            if (!Physics.SphereCast(position, RaycastPilotRadius, forward, out hitInfo, maxDis, TpMask))
            {
                hitInfo.point = position + forward * maxDis;
                hitInfo.normal = Vector3.up;
                hitInfo.distance = maxDis;
            }

            usedDistance = hitInfo.distance;
            pos = hitInfo.point + hitInfo.normal * num;

            if (Physics.CheckSphere(pos, radius, TpMask))
                return false;

            RaycastHit hitInfo2;
            if (!Physics.SphereCast(pos, radius, Vector3.down, out hitInfo2, RaycastFloorHeight, TpMask))
                return false;

            if (!Physics.SphereCast(new Ray(pos, Vector3.down), radius * RaycastStabilityRadiusRatio, hitInfo2.distance + RaycastStabilityDistance, TpMask))
                return false;

            if (Vector3.Dot(Vector3.up, hitInfo2.normal) < RaycastFloorDot)
                return false;

            RaycastHit hitInfo3;
            if (!Physics.SphereCast(pos, radius, Vector3.up, out hitInfo3, RaycastCeilHeight, TpMask))
            {
                hitInfo3.point = pos + Vector3.up * RaycastCeilHeight;
            }

            if (Mathf.Abs(hitInfo2.point.y - hitInfo3.point.y) < CharacterControllerSettings.Height)
                return false;

            if (hitInfo2.collider != null && hitInfo2.collider.TryGetComponent(out CheckpointKiller _))
                return false;

            pos = hitInfo2.point + (hitInfo2.normal + Vector3.down) * radius;
            return true;
        }

        public void ServerTeleportTo(Vector3 pos)
        {
            if (!NetworkServer.active)
                return;

            Position = pos;

            if (CharController != null)
                CharController.SimpleMove(Vector3.zero);

            Transform transform = base.transform;
            if (transform != null)
            {
                ServerOverridePosition(transform.position, Vector3.zero);
            }
        }
    }
}