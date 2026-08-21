using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Ammo
{
    public class AmmoItem : ItemBase, IItemNametag, ICustomSearchCompletorItem
    {
        public int UnitPrice;

        [SerializeField]
        private string _caliber;
        public override float Weight => 0.25f + ((PickupDropModel is global::InventorySystem.Items.Firearms.Ammo.AmmoPickup ammoPickup)
            ? ((float)ammoPickup.SavedAmmo * 0.01f)  
            : 0f);

        public string Name => _caliber;

        public override ItemDescriptionType DescriptionType => ItemDescriptionType.None;

        public global::InventorySystem.Searching.SearchCompletor GetCustomSearchCompletor(ReferenceHub hub, global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::InventorySystem.Items.ItemBase ib, double disSqrt)
        {
            return new global::InventorySystem.Searching.AmmoSearchCompletor(hub, ipb, ib, disSqrt);
        }

        public override void OnAdded(global::InventorySystem.Items.Pickups.ItemPickupBase pickup)
        {
            if (global::Mirror.NetworkServer.active)
            {
                if (PickupDropModel is global::InventorySystem.Items.Firearms.Ammo.AmmoPickup ammoPickup)
                {
                    base.OwnerInventory.ServerAddAmmo(ItemTypeId, ammoPickup.SavedAmmo);
                }
                base.OwnerInventory.ServerRemoveItem(base.ItemSerial, null);
            }
        }
    }
}