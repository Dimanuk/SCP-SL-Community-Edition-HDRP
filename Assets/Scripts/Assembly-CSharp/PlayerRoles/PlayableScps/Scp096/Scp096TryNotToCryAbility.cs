using System;
using System.Diagnostics;
using CursorManagement;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Subroutines;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp096
{
    public class Scp096TryNotToCryAbility : ScpKeySubroutine<Scp096Role>, ICursorOverride
    {
        [SerializeField] private float _clientDotTolerance;
        [SerializeField] private float _serverDotTolerance;
        [SerializeField] private float _clientDisTolerance;
        [SerializeField] private float _serverDisTolerance;
        [SerializeField] private float _maxVerticalAngle;
        [SerializeField] private float _maxDistance;
        [SerializeField] private float _minWidth;
        [SerializeField] private float _sideOffset;
        [SerializeField] private float _groundLevelMaxDiff;

        private RelativePosition _syncPoint;
        private Quaternion _syncRot;
        private bool _cancelled;
        private readonly Stopwatch _freezeSw = new Stopwatch();

        private const float AbsFreezeDuration = 0.1f;
        private const float RadiusTolerance   = 0.9f;

        private static readonly Quaternion[] RotationAngles = new Quaternion[2]
        {
            Quaternion.Euler(Vector3.up   * 90f),
            Quaternion.Euler(Vector3.down * 90f)
        };

        private static readonly ActionName[] CancelKeys = new ActionName[5]
        {
            ActionName.MoveBackward,
            ActionName.MoveForward,
            ActionName.MoveLeft,
            ActionName.MoveRight,
            ActionName.Jump
        };

        private static readonly CachedLayerMask Mask = new CachedLayerMask("Door", "Glass", "Default");

        private static readonly float[] Heights = new float[3] { 0f, -0.4f, -0.9f };

        private static readonly Vector3[] Offsets = new Vector3[RotationAngles.Length + 1];

        private static readonly Vector3[] GroundPoints = new Vector3[4];

        public CursorOverrideMode CursorOverride => CursorOverrideMode.NoOverride;

        public bool LockMovement
        {
            get
            {
                if (!base.Owner.isLocalPlayer || _cancelled)
                    return false;

                if (IsActive)
                    return true;

                if (!_freezeSw.IsRunning)
                    return false;

                double limit = NetworkTime.rtt + AbsFreezeDuration;
                return _freezeSw.Elapsed.TotalSeconds < limit;
            }
        }

        protected override ActionName TargetKey => ActionName.Zoom;

        private bool IsActive
        {
            get => base.ScpRole.IsAbilityState(Scp096AbilityState.TryingNotToCry);
            set
            {
                if (!NetworkServer.active)
                    throw new InvalidOperationException($"Cannot set {this}.IsActive as client.");

                if (IsActive == value)
                    return;

                if (value)
                {
                    base.Role.TryGetOwner(out var hub);
                    base.ScpRole.StateController.SetAbilityState(Scp096AbilityState.TryingNotToCry);
                }
                else if (IsActive)
                {
                    base.Role.TryGetOwner(out var hub2);
                    base.ScpRole.ResetAbilityState();
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!IsActive)
                return;

            if (base.Owner.isLocalPlayer)
                UpdateClient();

            if (NetworkServer.active && !ServerValidate())
                IsActive = false;
        }

        protected override void OnKeyDown()
        {
            base.OnKeyDown();

            if (IsActive || !ValidatePoint())
                return;

            _cancelled = false;
            _freezeSw.Restart();
            ClientSendCmd();
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);

            if (_cancelled)
                return;

            RelativePosition msg = new RelativePosition(base.ScpRole.FpcModule.Position);
            if (WaypointBase.TryGetWaypoint(msg.WaypointId, out var wp))
            {
                RelativePositionSerialization.WriteRelativePosition(writer, msg);
                writer.WriteQuaternion(wp.GetRelativeRotation(base.Owner.PlayerCameraReference.rotation));
            }
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (reader.Position >= reader.Capacity)
            {
                IsActive = false;
                return;
            }

            _syncPoint = RelativePositionSerialization.ReadRelativePosition(reader);
            _syncRot   = NetworkReaderExtensions.ReadQuaternion(reader);
            IsActive   = ServerValidate();
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            CursorManager.Register(this);
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _freezeSw.Reset();
            CursorManager.Unregister(this);
        }

        private void UpdateClient()
        {
            if (_cancelled)
                return;

            for (int i = 0; i < CancelKeys.Length; i++)
            {
                if (Input.GetKeyDown(NewInput.GetKey(CancelKeys[i])))
                {
                    _cancelled = true;
                    ClientSendCmd();
                    break;
                }
            }
        }

        private bool ServerValidate()
        {
            if (base.ScpRole.StateController.RageState != Scp096RageState.Docile)
                return false;

            if (!WaypointBase.TryGetWaypoint(_syncPoint.WaypointId, out var wp))
                return false;

            Vector3 worldPos = wp.GetWorldspacePosition(_syncPoint.Relative);
            Quaternion worldRot = wp.GetWorldspaceRotation(_syncRot);

            using (new FpcBacktracker(base.Owner, worldPos, worldRot))
            {
                return ValidatePoint();
            }
        }

        private bool ValidatePoint() => ValidateGround() && ValidateWall();

        private bool ValidateGround()
        {
            if (!base.ScpRole.FpcModule.IsGrounded)
                return false;

            Transform tr = base.Owner.transform;
            float height = base.ScpRole.FpcModule.CharController.height;
            float radius = base.ScpRole.FpcModule.CharController.radius * RadiusTolerance;

            GroundPoints[0] = tr.position + tr.forward * radius;
            GroundPoints[1] = tr.position + tr.right   * radius;
            GroundPoints[2] = tr.position - tr.forward * radius;
            GroundPoints[3] = tr.position - tr.right   * radius;

            float minDist = float.MaxValue;
            float maxDist = 0f;

            for (int i = 0; i < GroundPoints.Length; i++)
            {
                if (!Physics.Raycast(GroundPoints[i], Vector3.down, out RaycastHit hit, height, Mask))
                    return false;

                float d = hit.distance;
                if (d < minDist) minDist = d;
                if (d > maxDist) maxDist = d;

                if (maxDist - minDist > _groundLevelMaxDiff)
                    return false;
            }

            return true;
        }

        private bool ValidateWall()
        {
            Vector3 position = base.Owner.PlayerCameraReference.position;
            Vector3 forward  = base.Owner.PlayerCameraReference.forward;

            if (base.Owner.isLocalPlayer &&
                Mathf.Abs(base.ScpRole.FpcModule.MouseLook.CurrentVertical) > _maxVerticalAngle)
                return false;

            forward.y = 0f;
            float mag = forward.magnitude;
            if (mag == 0f)
                return false;

            forward /= mag;

            if (!Physics.Raycast(position, forward, out RaycastHit hit, _maxDistance, Mask))
                return false;

            float dotTol = base.Owner.isLocalPlayer ? _clientDotTolerance : _serverDotTolerance;
            Vector3 normal = hit.normal;

            if (Vector3.Dot(forward, normal) > dotTol)
                return false;

            Vector3 core = position + normal * _minWidth;
            Vector3 start = core + Vector3.down * _sideOffset;
            Vector3 end   = core + Vector3.up   * (Heights[Heights.Length - 1] + _sideOffset);

            if (Physics.CheckCapsule(start, end, _sideOffset, Mask))
                return false;

            float disTol = base.Owner.isLocalPlayer ? _clientDisTolerance : _serverDisTolerance;
            float minDist = float.MaxValue;
            float maxDist = 0f;

            for (int i = 0; i < RotationAngles.Length; i++)
                Offsets[i] = RotationAngles[i] * normal;

            for (int j = 0; j < Offsets.Length; j++)
            {
                for (int k = 0; k < Heights.Length; k++)
                {
                    Vector3 origin = Offsets[j] * _sideOffset + normal + position + Vector3.up * Heights[k];

                    if (!Physics.Raycast(origin, -normal, out RaycastHit hit2, _maxDistance, Mask))
                        return false;

                    if (Vector3.Dot(forward, normal) > dotTol)
                        return false;

                    float d = hit2.distance;
                    if (d < minDist) minDist = d;
                    if (d > maxDist) maxDist = d;

                    if (maxDist - minDist > disTol)
                        return false;
                }
            }

            return true;
        }
    }
}
