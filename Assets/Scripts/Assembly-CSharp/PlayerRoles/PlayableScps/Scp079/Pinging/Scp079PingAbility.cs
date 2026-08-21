using System;
using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.Spectating;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Pinging
{
    public class Scp079PingAbility : Scp079KeyAbilityBase
    {
        [SerializeField]
        private int _cost;

        [SerializeField]
        private float _instantCooldown;

        [SerializeField]
        private float _groupCooldown;

        [SerializeField]
        private int _groupSize;

        [SerializeField]
        private Scp079PingInstance _prefab;

        [SerializeField]
        private Sprite[] _icons;

        private string _abilityName;
        private string _cooldownMsg;
        private RateLimiter _rateLimiter;

        private byte _syncProcessorIndex;
        private RelativePosition _syncPos;
        private Vector3 _syncNormal;

        private const float RaycastMaxDis = 130f;

        private static readonly IPingProcessor[] PingProcessors = new IPingProcessor[]
        {
            new GeneratorPingProcessor(),
            new ProjectilePingProcessor(),
            new MicroHidPingProcesssor(),
            new HumanPingProcessor(),
            new ElevatorPingProcessor(),
            new DoorPingProcessor(),
            new DefaultPingProcessor()
        };

        public override ActionName ActivationKey => ActionName.Scp079PingLocation;

        public override bool IsReady => base.AuxManager.CurrentAux >= _cost && _rateLimiter.AllReady;

        public override bool IsVisible => !Scp079CursorManager.LockCameras;

        public override string AbilityName => string.Format(_abilityName, _cost);

        public override string FailMessage
        {
            get
            {
                if (base.AuxManager.CurrentAux < _cost)
                    return GetNoAuxMessage(_cost);

                if (!_rateLimiter.RateReady)
                    return _cooldownMsg;

                return null;
            }
        }

        private void SpawnIndicator(int processorIndex, RelativePosition pos, Vector3 normal)
        {
            Scp079PingInstance instance = Instantiate(_prefab);
            
            int iconId = PingProcessors[processorIndex].IconId;
            instance.IconSprite = _icons[iconId];

            Transform tr = instance.transform;
            tr.SetPositionAndRotation(pos.Position, Quaternion.FromToRotation(Vector3.up, normal));

            if (WaypointBase.TryGetWaypoint(pos.WaypointId, out WaypointBase waypoint))
            {
                tr.SetParent(waypoint.transform);
            }
        }

        private void WriteSyncVars(NetworkWriter writer)
        {
            writer.WriteByte(_syncProcessorIndex);
            RelativePositionSerialization.WriteRelativePosition(writer, _syncPos);
            NetworkWriterExtensions.WriteVector3(writer, _syncNormal);
        }

        private bool ServerCheckReceiver(ReferenceHub hub, Vector3 point, int processorIndex)
        {
            PlayerRoleBase currentRole = hub.roleManager.CurrentRole;

            if (currentRole is FpcStandardScp fpcScp)
            {
                float range = PingProcessors[processorIndex].Range;
                float rangeSqr = range * range;
                Vector3 scpPos = fpcScp.FpcModule.Position;

                RoomIdentifier room = RoomIdUtils.RoomAtPositionRaycasts(point);
                if (room == null)
                    return false;

                Vector3 gridScale = RoomIdentifier.GridScale;
                foreach (Vector3Int coord in room.OccupiedCoords)
                {
                    Bounds roomBounds = new Bounds(Vector3.Scale(coord, gridScale), gridScale);
                    if (roomBounds.SqrDistance(scpPos) <= rangeSqr)
                        return true;
                }

                return false;
            }

            if (hub.IsSCP())
                return true;

            return currentRole is SpectatorRole;
        }

        protected override void Start()
        {
            base.Start();
            _rateLimiter = new RateLimiter(_instantCooldown, _groupSize, _groupCooldown);
            _abilityName = Translations.Get(Scp079HudTranslation.PingLocation);
            _cooldownMsg = Translations.Get(Scp079HudTranslation.PingRateLimited);
        }

        protected override void Trigger()
        {
            Camera cam = Scp079Hud.RaycastCamera;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            for (int i = 0; i < PingProcessors.Length; i++)
            {
                IPingProcessor processor = PingProcessors[i];
                LayerMask mask = processor.Mask;

                if (!Physics.Raycast(ray, out RaycastHit hit, RaycastMaxDis, mask))
                    continue;

                if (!processor.TryProcess(hit))
                    continue;

                if (!NetworkServer.active)
                {
                    _rateLimiter.RegisterInput();
                }

                _syncProcessorIndex = (byte)i;
                _syncPos = new RelativePosition(hit.point);
                _syncNormal = hit.normal;

                ClientSendCmd();
                return;
            }
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            WriteSyncVars(writer);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (!IsReady || !base.Role.TryGetOwner(out _) || base.LostSignalHandler.Lost)
                return;

            _syncProcessorIndex = reader.ReadByte();

            if (_syncProcessorIndex >= PingProcessors.Length)
                return;

            _syncPos = RelativePositionSerialization.ReadRelativePosition(reader);
            _syncNormal = NetworkReaderExtensions.ReadVector3(reader);

            ServerSendRpc(x => ServerCheckReceiver(x, _syncPos.Position, _syncProcessorIndex));

            base.AuxManager.CurrentAux -= _cost;
            _rateLimiter.RegisterInput();
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            WriteSyncVars(writer);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            byte processorIndex = reader.ReadByte();
            RelativePosition pos = RelativePositionSerialization.ReadRelativePosition(reader);
            Vector3 normal = NetworkReaderExtensions.ReadVector3(reader);

            SpawnIndicator(processorIndex, pos, normal);
        }
    }
}
