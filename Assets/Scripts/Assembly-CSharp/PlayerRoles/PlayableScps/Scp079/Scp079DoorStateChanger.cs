using System;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079DoorStateChanger : Scp079DoorAbility
    {

        [Serializable]
        private struct DoorCost
        {
            public KeycardPermissions Perm;
            public int Cost;
        }

        [SerializeField]
        private int _defaultCost;

        [SerializeField]
        private DoorCost[] _doorCostsheet;

        private static string _openText;
        private static string _closeText;

        public override ActionName ActivationKey => ActionName.Shoot;

        public override string AbilityName
        {
            get
            {
                string text = LastDoor != null && LastDoor.TargetState ? _closeText : _openText;
                int cost = GetCostForDoor(TargetAction, LastDoor);
                return string.Format(text, cost);
            }
        }

        protected override DoorAction TargetAction
        {
            get
            {
                if (LastDoor == null)
                    return DoorAction.Opened; // fallback
                
                if (!LastDoor.TargetState)
                    return DoorAction.Opened;
                
                return DoorAction.Closed;
            }
        }

        public static event Action<Scp079Role, DoorVariant> OnServerDoorToggled;

        protected override void Start()
        {
            base.Start();

            _openText = Translations.Get(Scp079HudTranslation.OpenDoor);
            _closeText = Translations.Get(Scp079HudTranslation.CloseDoor);
        }

        protected override int GetCostForDoor(DoorAction action, DoorVariant door)
        {
            if (door == null)
                return _defaultCost;

            KeycardPermissions requiredPermissions = door.RequiredPermissions.RequiredPermissions;
            int cost = _defaultCost;

            for (int i = 0; i < _doorCostsheet.Length; i++)
            {
                DoorCost doorCost = _doorCostsheet[i];
                
                if (DoorPermissionUtils.HasFlagFast(requiredPermissions, doorCost.Perm))
                {
                    cost = Mathf.Max(cost, doorCost.Cost);
                }
            }

            return cost;
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            NetworkWriterExtensions.WriteUInt(writer, LastDoor.netId);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            uint netId = NetworkReaderExtensions.ReadUInt(reader);

            if (!NetworkServer.spawned.TryGetValue(netId, out NetworkIdentity identity))
                return;

            if (!identity.TryGetComponent(out DoorVariant door))
                return;

            LastDoor = door;

            if (!IsReady)
                return;

            if (!Role.TryGetOwner(out ReferenceHub hub))
                return;

            if (LostSignalHandler.Lost)
                return;

            bool targetState = LastDoor.TargetState;

            LastDoor.ServerInteract(hub, 0);

            if (targetState != LastDoor.TargetState)
            {
                RewardManager.MarkRooms(LastDoor.Rooms);
                
                OnServerDoorToggled?.Invoke(ScpRole, LastDoor);
                
                AuxManager.CurrentAux -= GetCostForDoor(TargetAction, LastDoor);
                
                ServerSendRpc(toAll: false);
            }
        }
    }
}
