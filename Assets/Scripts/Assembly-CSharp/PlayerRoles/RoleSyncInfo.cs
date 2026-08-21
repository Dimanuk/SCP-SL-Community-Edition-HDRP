namespace PlayerRoles
{
    public struct RoleSyncInfo : global::Mirror.NetworkMessage
    {
        private readonly uint _targetNetId;

        private readonly uint _receiverNetId;

        private readonly global::PlayerRoles.PlayerRoleBase _role;

        private readonly global::PlayerRoles.RoleTypeId _targetRole;

        public RoleSyncInfo(ReferenceHub target, global::PlayerRoles.RoleTypeId role, ReferenceHub receiver)
        {
            _targetNetId = target.netId;
            _targetRole = role;
            _receiverNetId = receiver.netId;
            _role = target.roleManager.CurrentRole;
        }

        public RoleSyncInfo(global::Mirror.NetworkReader reader)
        {
            _receiverNetId = 0u;
            _targetNetId = global::Mirror.NetworkReaderExtensions.ReadUInt(reader);
            if (!ReferenceHub.TryGetHubNetID(_targetNetId, out var hub))
            {
                global::PlayerRoles.PlayerRolesNetUtils.QueuedRoles[_targetNetId] = reader;
                _role = null;
                _targetRole = global::PlayerRoles.RoleTypeId.None;
                return;
            }
            _targetRole = reader.ReadRoleType();
            if (!global::Mirror.NetworkServer.active)
            {
                hub.roleManager.InitializeNewRole(_targetRole, global::PlayerRoles.RoleChangeReason.None, global::PlayerRoles.RoleSpawnFlags.All, reader);
            }
            _role = hub.roleManager.CurrentRole;
        }

        public void Write(global::Mirror.NetworkWriter writer)
        {
            global::Mirror.NetworkWriterExtensions.WriteUInt(writer, _targetNetId);
            writer.WriteRoleType(_targetRole);
            if (_role is global::PlayerRoles.SpawnData.IPublicSpawnDataWriter publicSpawnDataWriter)
            {
                publicSpawnDataWriter.WritePublicSpawnData(writer);
            }
            if (_receiverNetId == _targetNetId && _role is global::PlayerRoles.SpawnData.IPrivateSpawnDataWriter privateSpawnDataWriter)
            {
                privateSpawnDataWriter.WritePrivateSpawnData(writer);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} (TargetNetId = '{1}' Role = '{2}')", "RoleSyncInfo", _targetNetId, _role);
        }
    }
}
