namespace Scp914.Processors
{
    public class Scp244ItemProcessor : global::Scp914.Processors.StandardItemProcessor
    {
        public override global::InventorySystem.Items.Pickups.ItemPickupBase OnPickupUpgraded(global::Scp914.Scp914KnobSetting setting, global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::UnityEngine.Vector3 newPosition)
        {
            if (!(ipb is global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup scp244DeployablePickup) || scp244DeployablePickup.ModelDestroyed)
            {
                return null;
            }
            return base.OnPickupUpgraded(setting, ipb, newPosition);
        }

        protected override void HandleOldPickup(global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::UnityEngine.Vector3 newPos)
        {
            if (ipb is global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup scp244DeployablePickup && scp244DeployablePickup.State == global::InventorySystem.Items.Usables.Scp244.Scp244State.Active)
            {
                scp244DeployablePickup.State = global::InventorySystem.Items.Usables.Scp244.Scp244State.PickedUp;
            }
            else
            {
                base.HandleOldPickup(ipb, newPos);
            }
        }

        protected override void HandleNone(global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::UnityEngine.Vector3 newPos)
        {
            if (ipb is global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup scp244DeployablePickup)
            {
                if (!scp244DeployablePickup.ModelDestroyed)
                {
                    scp244DeployablePickup.State = global::InventorySystem.Items.Usables.Scp244.Scp244State.Destroyed;
                }
            }
            else
            {
                base.HandleNone(ipb, newPos);
            }
        }
    }
}
