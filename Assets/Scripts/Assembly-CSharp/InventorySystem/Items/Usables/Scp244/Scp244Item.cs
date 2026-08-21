
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;

namespace InventorySystem.Items.Usables.Scp244
{
	public class Scp244Item : UsableItem, ICustomSearchCompletorItem
	{
		private bool _primed;

		private const float DropHeightOffset = 0.72f;

        public override void ServerOnUsingCompleted()
        {
            _primed = true;
            base.OwnerInventory.ServerDropItem(base.ItemSerial);
        }

        public override void OnUsingCancelled()
		{
			base.OnUsingCancelled();
			_primed = false;
		}

        public override global::InventorySystem.Items.Pickups.ItemPickupBase ServerDropItem()
        {
            PickupSyncInfo psi = new()
            {
                ItemId = ItemTypeId,
                Serial = base.ItemSerial,
                Weight = Weight
            };
            global::InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = base.OwnerInventory.ServerCreatePickup(this, psi, spawn: false);
            base.OwnerInventory.ServerRemoveItem(psi.Serial, itemPickupBase);
            itemPickupBase.PreviousOwner = new global::Footprinting.Footprint(base.Owner);
            itemPickupBase.transform.position = base.Owner.transform.position - global::UnityEngine.Vector3.up * 0.72f;
            itemPickupBase.transform.rotation = base.Owner.transform.rotation;
            itemPickupBase.RefreshPositionAndRotation();
            if (itemPickupBase is global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup scp244DeployablePickup)
            {
                scp244DeployablePickup.State = (_primed ? global::InventorySystem.Items.Usables.Scp244.Scp244State.Active : global::InventorySystem.Items.Usables.Scp244.Scp244State.Idle);
            }
            global::Mirror.NetworkServer.Spawn(itemPickupBase.gameObject);
            return itemPickupBase;
        }

        public global::InventorySystem.Searching.SearchCompletor GetCustomSearchCompletor(ReferenceHub hub, global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::InventorySystem.Items.ItemBase ib, double disSqrt)
        {
            return new global::InventorySystem.Searching.Scp244SearchCompletor(hub, ipb, ib, disSqrt);
        }
    }
}
