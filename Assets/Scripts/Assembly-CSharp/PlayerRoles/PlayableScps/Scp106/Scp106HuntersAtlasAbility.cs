using GameObjectPools;
using MapGeneration;
using Mirror;
using PlayerRoles.Spectating;
using RelativePositioning;
using UnityEngine;
using Interactables.Interobjects.DoorUtils;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106HuntersAtlasAbility : Scp106VigorAbilityBase, IPoolResettable
    {
        public const float CostPerMeter = 0.019f;

        private const ActionName SelectKey = ActionName.Shoot;
        private const int SyncAccuracy = 50;
        private const float SubmergeTime = 2.5f;
        private const float HeightOffset = 0.2f;
        private const float NormalMultiplier = 1.1f;
        private const float GroundDetectorHeight = 2f;
        private const float DissolvePercent = 0.5f;
        private const float MaxRetakeRange = 15f;
        private const float HeightTolerance = 400f;
        private const float DoorHeightTolerance = 50f;
        private const int OverlapSphereMaxDetections = 8;
        private const float MinVigor = 0.25f;

        private Vector3 _syncPos;
        private RoomIdentifier _syncRoom;
        private bool _submerged;
        private bool _dissolveAnim;
        private float _lastDissolveAmount;
        private float _estimatedCost;

        private static readonly Collider[] DetectionsNonAlloc = new Collider[8];
        private static readonly float DebugDuration = 0f;
        private static bool _maskSet;
        private static int _mask;

        private static int DetectionMask
        {
            get
            {
                if (!_maskSet)
                {
                    _mask = LayerMask.GetMask("Default", "Locker");
                    _maskSet = true;
                }
                return _mask;
            }
        }

        protected override ActionName TargetKey => ActionName.Inventory;

        public override bool IsSubmerged => _submerged;

        protected override bool KeyPressable => base.Owner.isLocalPlayer;

        private void SetSubmerged(bool val)
        {
            if (_submerged != val)
            {
                _submerged = val;
                if (val)
                {
                    _dissolveAnim = true;
                    base.ScpRole.Sinkhole.TargetDuration = 2.5f;
                }
                if (NetworkServer.active)
                {
                    ServerSendRpc(toAll: true);
                }
            }
        }

        private void UpdateAny()
        {
            float normalizedState = base.ScpRole.Sinkhole.NormalizedState;

            if (_dissolveAnim && !_submerged && normalizedState < DissolvePercent)
            {
                _dissolveAnim = false;
            }

            if (base.Owner.isLocalPlayer || SpectatorNetworking.IsLocallySpectated(base.Owner))
            {
                _lastDissolveAmount = _dissolveAnim
                    ? Mathf.InverseLerp(DissolvePercent, 1f, normalizedState)
                    : 0f;

                Scp106Hud.SetDissolveAnimation(_lastDissolveAmount);
            }
        }

        private void UpdateClientside()
        {
            Scp106Minimap singleton = Scp106Minimap.Singleton;
            if (singleton == null)
                return;

            if (base.ScpRole.Sinkhole.NormalizedState != 0f || !IsKeyHeld || !base.ScpRole.FpcModule.IsGrounded)
            {
                singleton.IsVisible = false;
                return;
            }

            if (!base.ScpRole.Sinkhole.Cooldown.IsReady)
            {
                singleton.IsVisible = false;
                Scp106Hud.PlayFlash(vigor: false);
                return;
            }

            if (base.Vigor.VigorAmount < MinVigor)
            {
                singleton.IsVisible = false;
                Scp106Hud.PlayFlash(vigor: true);
                return;
            }

            singleton.IsVisible = true;

            if (Scp106MinimapElement.AnyHighlighted && Input.GetKey(NewInput.GetKey(SelectKey)))
            {
                _syncPos = singleton.LastWorldPos;
                _syncRoom = Scp106MinimapElement.LastHighlighted.Room;
                ClientSendCmd();
            }
        }

        private void UpdateServerside()
        {
            if (_submerged && !(base.ScpRole.Sinkhole.NormalizedState < 1f))
            {
                Vector3 safePosition = GetSafePosition();
                float distance = (safePosition - base.ScpRole.FpcModule.Position).MagnitudeIgnoreY();
                base.Vigor.VigorAmount -= Mathf.Min(_estimatedCost, distance * CostPerMeter);
                base.ScpRole.FpcModule.ServerOverridePosition(safePosition, Vector3.zero);
                SetSubmerged(val: false);
            }
        }

        private Vector3 GetSafePosition()
        {
            Vector3 result = base.ScpRole.FpcModule.Position;
            float bestSqr = float.MaxValue;

            foreach (DoorVariant door in DoorVariant.AllDoors)
            {
                if (!(door is IScp106PassableDoor passable) || !passable.IsScp106Passable || !door.Rooms.Contains(_syncRoom))
                    continue;

                Vector3 doorWorldPos = door.transform.position;
                if (Mathf.Abs(doorWorldPos.y - _syncPos.y) > DoorHeightTolerance)
                    continue;

                Vector3 candidate = ClosestDoorPosition(doorWorldPos);
                float sqrDist = (candidate - _syncPos).SqrMagnitudeIgnoreY();

                if (DebugDuration > 0f)
                    door.name = $"Debug door, disSqr={sqrDist}";

                if (sqrDist <= bestSqr)
                {
                    bestSqr = sqrDist;
                    result = candidate;
                }
            }

            return result;
        }

        private Vector3 ClosestDoorPosition(Vector3 doorPos)
        {
            Vector3 toTarget = _syncPos - doorPos;
            Vector3 dir = new Vector3(toTarget.x, 0f, toTarget.z);
            float remaining = dir.magnitude;

            if (remaining > 0f)
                dir /= remaining;

            float radius = base.ScpRole.FpcModule.CharController.radius;
            float height = base.ScpRole.FpcModule.CharController.height;
            Vector3 origin = doorPos + Vector3.up * (HeightOffset + radius);

            Color debugColor = DebugDuration > 0f
                ? Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.4f, 0.8f)
                : Color.clear;

            do
            {
                if (TrySphereCast(debugColor, origin, dir, radius, height, remaining, out Vector3 pos))
                    return pos;

                remaining = Mathf.Min(MaxRetakeRange, remaining - radius);
            }
            while (!(remaining < radius));

            return doorPos + Vector3.up * height;
        }

        private bool TrySphereCast(Color debugColor, Vector3 origin, Vector3 dir,
            float radius, float height, float maxDis, out Vector3 pos)
        {
            Debug.DrawRay(origin, dir, debugColor, DebugDuration);

            if (Physics.SphereCast(origin, radius, dir, out RaycastHit hitInfo, maxDis + radius, DetectionMask))
            {
                hitInfo.point += NormalMultiplier * radius * hitInfo.normal;
            }
            else
            {
                hitInfo.point = origin + dir * maxDis;
            }

            pos = hitInfo.point;

            if (DebugDuration > 0f)
                DebugHitPoint(hitInfo.point, debugColor);

            if (!Physics.Raycast(pos + Vector3.up * HeightOffset, Vector3.down,
                out RaycastHit groundHit, GroundDetectorHeight, DetectionMask))
            {
                return false;
            }

            int count = Physics.OverlapSphereNonAlloc(groundHit.point, radius * 2f, DetectionsNonAlloc);
            for (int i = 0; i < count; i++)
            {
                if (DetectionsNonAlloc[i].TryGetComponent<TeslaGate>(out _))
                    return false;
            }

            pos = groundHit.point + Vector3.up * (HeightOffset + radius);

            if (Physics.CheckCapsule(pos,
                pos + Vector3.up * (height - radius - HeightOffset),
                radius, DetectionMask))
            {
                return false;
            }

            pos = groundHit.point + Vector3.up * height;
            return true;
        }

        private void DebugHitPoint(Vector3 point, Color debugColor)
        {
            Vector3[] dirs = new Vector3[6]
            {
                Vector3.up,
                Vector3.down,
                Vector3.left,
                Vector3.right,
                Vector3.forward,
                Vector3.back
            };

            foreach (Vector3 d in dirs)
            {
                Debug.DrawLine(point, point + d * 0.1f, debugColor, DebugDuration);
            }
        }

        protected override void Update()
        {
            base.Update();
            UpdateAny();

            if (base.Owner.isLocalPlayer)
                UpdateClientside();

            if (NetworkServer.active)
                UpdateServerside();
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);

            Vector3 roomPos = _syncRoom.transform.position;
            RelativePositionSerialization.WriteRelativePosition(writer, new RelativePosition(roomPos));

            Vector3Int quantized = Vector3Int.RoundToInt((_syncPos - roomPos) * SyncAccuracy);
            NetworkWriterExtensions.WriteShort(writer, (short)quantized.x);
            NetworkWriterExtensions.WriteShort(writer, (short)quantized.z);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (base.ScpRole.Sinkhole.NormalizedState > 0f || !base.ScpRole.Sinkhole.Cooldown.IsReady)
                return;

            Vector3 roomPos = RelativePositionSerialization.ReadRelativePosition(reader).Position;
            _syncRoom = RoomIdUtils.RoomAtPosition(roomPos);

            Vector3 offset = new Vector3(
                NetworkReaderExtensions.ReadShort(reader),
                0f,
                NetworkReaderExtensions.ReadShort(reader));

            _syncPos = roomPos + offset / SyncAccuracy;

            if (_syncRoom == null)
                return;

            Vector3 currentPos = base.ScpRole.FpcModule.Position;
            if (Mathf.Abs(currentPos.y - _syncPos.y) > HeightTolerance)
                return;

            float cost = (currentPos - _syncPos).MagnitudeIgnoreY() * CostPerMeter;
            if (cost > base.Vigor.VigorAmount)
                return;

            _estimatedCost = cost;
            SetSubmerged(val: true);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            NetworkWriterExtensions.WriteBool(writer, IsSubmerged);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            if (!NetworkServer.active)
            {
                SetSubmerged(NetworkReaderExtensions.ReadBool(reader));
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _submerged = false;
            _dissolveAnim = false;

            if (_lastDissolveAmount > 0f)
            {
                _lastDissolveAmount = 0f;
                Scp106Hud.SetDissolveAnimation(0f);
            }
        }
    }
}