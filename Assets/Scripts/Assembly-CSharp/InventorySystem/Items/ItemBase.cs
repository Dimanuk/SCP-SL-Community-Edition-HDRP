using System.Runtime.CompilerServices;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Thirdperson;
using UnityEngine;

namespace InventorySystem.Items
{
    public abstract class ItemBase : MonoBehaviour
    {
        public ItemType ItemTypeId;

        public ItemCategory Category;

        public ItemTierFlags TierFlags;

        public ThirdpersonItemBase ThirdpersonModel;

        public ItemViewmodelBase ViewModel;

        public Texture Icon;

        public ItemThrowSettings ThrowSettings;

        public ItemPickupBase PickupDropModel;

        public virtual ItemDescriptionType DescriptionType { get; protected set; }

        public ReferenceHub Owner { get; internal set; }
        public ushort ItemSerial { get; internal set; }

        public bool IsEquipped { get; internal set; }

        internal Inventory OwnerInventory => Owner.inventory;

        public virtual bool AllowHolster => true;

        public abstract float Weight { get; }

        internal bool IsLocalPlayer
        {
            get
            {
                return Owner != null && Owner.isLocalPlayer;
            }
        }

        public virtual void OnEquipped()
        {
        }

        public virtual void EquipUpdate()
        {
        }

        public virtual void AlwaysUpdate()
        {
        }

        public virtual void OnHolstered()
        {
        }

        public virtual void OnAdded(ItemPickupBase pickup)
        {
        }

        public virtual void OnRemoved(ItemPickupBase pickup)
        {
        }

        internal virtual void OnTemplateReloaded(bool wasEverLoaded)
        {
        }

        public virtual global::InventorySystem.Items.Pickups.ItemPickupBase ServerDropItem()
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Method ServerDropItem can only be executed on the server.");
            }
            if (PickupDropModel == null)
            {
                global::UnityEngine.Debug.LogError("No pickup drop model set. Could not drop the item.");
                return null;
            }
            global::InventorySystem.Items.Pickups.PickupSyncInfo psi = new(ItemTypeId, Owner.transform.position, global::UnityEngine.Quaternion.identity, Weight, ItemSerial);
            global::InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = OwnerInventory.ServerCreatePickup(this, psi);
            OwnerInventory.ServerRemoveItem(psi.Serial, itemPickupBase);
            itemPickupBase.PreviousOwner = new global::Footprinting.Footprint(Owner);
            return itemPickupBase;
        }
    }
}
