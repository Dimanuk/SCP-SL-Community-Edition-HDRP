
using InventorySystem.Items.Firearms.Modules;

namespace InventorySystem.Items.Firearms
{
	public class ShotgunAnimatorEvents : FirearmAnimatorEventsBase
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

        private void InsertShell()
        {
            if (IsServerController && global::Mirror.NetworkServer.active && base.TargetFirearm.Status.Ammo < base.TargetFirearm.AmmoManagerModule.MaxAmmo && base.TargetFirearm.OwnerInventory.GetCurAmmo(base.TargetFirearm.AmmoType) != 0)
            {
                base.TargetFirearm.Status = new global::InventorySystem.Items.Firearms.FirearmStatus((byte)(base.TargetFirearm.Status.Ammo + 1), base.TargetFirearm.Status.Flags, base.TargetFirearm.Status.Attachments);
                base.TargetFirearm.OwnerInventory.ServerAddAmmo(base.TargetFirearm.AmmoType, -1);
            }
        }

        private void UnloadShells()
        {
            if (GetPumpAction(out var pa) && base.TargetFirearm.Status.Ammo > 0)
            {
                pa.Pump(sendToClients: true);
            }
        }

        private void Pump(int isReload)
        {
            if (GetPumpAction(out var pa))
            {
                pa.Pump(isReload == 1);
            }
        }

        private bool GetPumpAction(out global::InventorySystem.Items.Firearms.Modules.PumpAction pa)
        {
            pa = base.TargetFirearm.ActionModule as global::InventorySystem.Items.Firearms.Modules.PumpAction;
            if (!global::Mirror.NetworkServer.active || IsServerController)
            {
                return pa != null;
            }
            return false;
        }
    }
}
