namespace InventorySystem.Items.Firearms
{
    public class RevolverAnimatorEvents : global::InventorySystem.Items.Firearms.FirearmAnimatorEventsBase
    {
        private void ChangeMagStatus(int insert)
        {
            if (IsServerController && base.TargetFirearm.AmmoManagerModule is global::InventorySystem.Items.Firearms.Modules.ClipLoadedInternalMagAmmoManager clipLoadedInternalMagAmmoManager)
            {
                global::InventorySystem.Items.Firearms.FirearmStatusFlags flags = base.TargetFirearm.Status.Flags;
                ushort curAmmo = base.TargetFirearm.OwnerInventory.GetCurAmmo(base.TargetFirearm.AmmoType);
                if (insert == 1)
                {
                    flags |= global::InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted;
                    byte b = (byte)global::UnityEngine.Mathf.Min(curAmmo, clipLoadedInternalMagAmmoManager.MaxAmmo - base.TargetFirearm.Status.Ammo);
                    ModifyUserAmmo(-b);
                    base.TargetFirearm.Status = new global::InventorySystem.Items.Firearms.FirearmStatus((byte)(base.TargetFirearm.Status.Ammo + b), flags, base.TargetFirearm.Status.Attachments);
                }
                else
                {
                    flags &= ~global::InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted;
                    ModifyUserAmmo(base.TargetFirearm.Status.Ammo);
                    base.TargetFirearm.Status = new global::InventorySystem.Items.Firearms.FirearmStatus(0, flags, base.TargetFirearm.Status.Attachments);
                }
            }
        }
    }
}
