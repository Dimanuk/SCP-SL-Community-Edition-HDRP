using System;
using InventorySystem.Items.Firearms;
using Mirror;
using PlayerRoles.Spectating;
using RelativePositioning;
using UnityEngine;
using Utils.Networking;

namespace PlayerRoles.PlayableScps.Scp939.Ripples
{
    public class FirearmRippleTrigger : RippleTriggerBase
    {
        private Scp939FocusAbility _focus;
        private RelativePosition _syncRipplePos;
        private RoleTypeId _syncRoleColor;

        public override void SpawnObject()
        {
            base.SpawnObject();
            FirearmExtensions.ServerSoundPlayed = (Action<Firearm, byte, float>)
                Delegate.Combine(FirearmExtensions.ServerSoundPlayed, 
                new Action<Firearm, byte, float>(OnServerSoundPlayed));
        }

        public override void ResetObject()
        {
            base.ResetObject();
            FirearmExtensions.ServerSoundPlayed = (Action<Firearm, byte, float>)
                Delegate.Remove(FirearmExtensions.ServerSoundPlayed, 
                new Action<Firearm, byte, float>(OnServerSoundPlayed));
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _focus);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            RelativePositionSerialization.WriteRelativePosition(writer, _syncRipplePos);
            if (_focus.State >= 1f)
            {
                NetworkWriterExtensions.WriteSByte(writer, (sbyte)_syncRoleColor);
            }
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            Vector3 position = RelativePositionSerialization.ReadRelativePosition(reader).Position;
            Color color = DecodeColor(reader);
            base.Player.Play(position, color);
        }

        private Color DecodeColor(NetworkReader reader)
        {
            if (reader.Position >= reader.Capacity)
                return Color.red;

            sbyte roleId = NetworkReaderExtensions.ReadSByte(reader);
            if (!PlayerRoleLoader.TryGetRoleTemplate<HumanRole>((RoleTypeId)roleId, out HumanRole result))
                return Color.red;

            return result.RoleColor;
        }
        private void OnServerSoundPlayed(Firearm firearm, byte audioId, float dis)
        {
            if (!NetworkServer.active)
                return;

            if (!(firearm.Owner.roleManager.CurrentRole is HumanRole humanRole))
                return;

            if ((humanRole.FpcModule.Position - base.ScpRole.FpcModule.Position).sqrMagnitude > dis * dis)
                return;

            _syncRipplePos = new RelativePosition(humanRole.FpcModule.Position);
            _syncRoleColor = humanRole.RoleTypeId;

            ServerSendRpc((ReferenceHub x) => x == base.Owner || SpectatorNetworking.IsSpectatedBy(base.Owner, x));
        }

        private bool OnServerSoundPlayedCondition(ReferenceHub x)
        {
            if (x == base.Owner)
                return true;
            return SpectatorNetworking.IsSpectatedBy(base.Owner, x);
        }
    }
}
