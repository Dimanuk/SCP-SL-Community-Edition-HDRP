namespace InventorySystem.Items.Firearms
{
    public class DisruptorAnimatorEvents : global::InventorySystem.Items.Firearms.FirearmAnimatorEventsBase
    {
        private void MarkAsEquipped()
        {
            Firearm firearm = TargetFirearm;
            if (firearm == null) return;
            if ((!firearm.IsLocalPlayer || !IsServerController) && firearm.EquipperModule is global::InventorySystem.Items.Firearms.Modules.EventBasedEquipper eventBasedEquipper)
            {
                eventBasedEquipper.Equip();
            }
        }

        private void SetMagazine(int state)
        {
            SetFlag(global::InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted, state);
        }

        private void SetCocked(int state)
        {
            SetFlag(global::InventorySystem.Items.Firearms.FirearmStatusFlags.Cocked, state);
        }

        private void SetChambered(int state)
        {
            SetFlag(global::InventorySystem.Items.Firearms.FirearmStatusFlags.Chambered, state);
        }

        private void SetFlag(global::InventorySystem.Items.Firearms.FirearmStatusFlags flag, int code)
        {
            if (IsServerController)
            {
                global::InventorySystem.Items.Firearms.FirearmStatusFlags flags = base.TargetFirearm.Status.Flags;
                flags = ((code != 0) ? (flags | flag) : ((global::InventorySystem.Items.Firearms.FirearmStatusFlags)((uint)flags & (uint)(byte)(~(int)flag))));
                base.TargetFirearm.Status = new global::InventorySystem.Items.Firearms.FirearmStatus(base.TargetFirearm.Status.Ammo, flags, base.TargetFirearm.Status.Attachments);
            }
        }
    }
}
