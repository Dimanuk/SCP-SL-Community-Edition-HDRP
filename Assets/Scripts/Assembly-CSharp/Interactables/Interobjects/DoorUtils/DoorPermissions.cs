namespace Interactables.Interobjects.DoorUtils
{
    [global::System.Serializable]
    public class DoorPermissions
    {
        public global::Interactables.Interobjects.DoorUtils.KeycardPermissions RequiredPermissions;

        public bool RequireAll;

        public bool Bypass2176;

        public bool CheckPermissions(global::InventorySystem.Items.ItemBase item, ReferenceHub ply)
        {
            if (RequiredPermissions == global::Interactables.Interobjects.DoorUtils.KeycardPermissions.None)
            {
                return true;
            }
            if (ply != null)
            {
                if (ply.serverRoles.BypassMode)
                {
                    return true;
                }
                if (item == null)
                {
                    if (global::PlayerRoles.PlayerRolesUtils.IsSCP(ply))
                    {
                        return RequiredPermissions.HasFlagFast(global::Interactables.Interobjects.DoorUtils.KeycardPermissions.ScpOverride);
                    }
                    return false;
                }
            }
            if (item is global::InventorySystem.Items.Keycards.KeycardItem keycardItem)
            {
                if (!RequireAll)
                {
                    return (keycardItem.Permissions & RequiredPermissions) != 0;
                }
                return (keycardItem.Permissions & RequiredPermissions) == RequiredPermissions;
            }
            return false;
        }
    }
}
