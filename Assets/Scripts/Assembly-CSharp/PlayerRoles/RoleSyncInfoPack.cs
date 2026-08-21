namespace PlayerRoles
{
    public struct RoleSyncInfoPack : global::Mirror.NetworkMessage
    {
        private readonly ReferenceHub _receiverHub;

        public RoleSyncInfoPack(ReferenceHub receiver)
        {
            _receiverHub = receiver;
        }

        public RoleSyncInfoPack(global::Mirror.NetworkReader reader)
        {
            _receiverHub = null;
            int num = global::Mirror.NetworkReaderExtensions.ReadUShort(reader);
            for (int i = 0; i < num; i++)
            {
                reader.ReadRoleSyncInfo();
            }
        }

        public void WritePlayers(global::Mirror.NetworkWriter writer)
        {
            global::Mirror.NetworkWriterExtensions.WriteUShort(writer, (ushort)ReferenceHub.AllHubs.Count);
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                global::PlayerRoles.RoleTypeId roleTypeId = ((allHub.roleManager.CurrentRole is global::PlayerRoles.IObfuscatedRole obfuscatedRole) ? obfuscatedRole.GetRoleForUser(_receiverHub) : allHub.roleManager.CurrentRole.RoleTypeId);
                new global::PlayerRoles.RoleSyncInfo(allHub, roleTypeId, _receiverHub).Write(writer);
                allHub.roleManager.PreviouslySentRole[_receiverHub.netId] = roleTypeId;
            }
        }
    }
}
