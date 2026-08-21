using System;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;

namespace InventorySystem
{
	public static class InventoryExtensions
	{
		public static event Action<ReferenceHub, ItemBase, ItemPickupBase> OnItemAdded;

		public static event Action<ReferenceHub, ItemBase, ItemPickupBase> OnItemRemoved;

        public static ItemType GetSelectedItemType(this global::InventorySystem.Inventory inv)
        {
            if (inv.CurItem.SerialNumber <= 0)
            {
                return ItemType.None;
            }
            return inv.CurItem.TypeId;
        }

        public static bool TryGetHubHoldingSerial(ushort serial, out ReferenceHub hub)
        {
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.inventory.CurItem.SerialNumber == serial)
                {
                    hub = allHub;
                    return true;
                }
            }
            hub = null;
            return false;
        }

        public static bool ServerTryGetItemWithSerial(ushort serial, out global::InventorySystem.Items.ItemBase ib)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Method ServerTryGetItemWithSerial can only be executed on the server.");
            }
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.inventory.UserInventory.Items.TryGetValue(serial, out ib))
                {
                    return true;
                }
            }
            ib = null;
            return false;
        }

        public static global::InventorySystem.Items.ItemBase ServerAddItem(this global::InventorySystem.Inventory inv, ItemType type, ushort itemSerial = 0, global::InventorySystem.Items.Pickups.ItemPickupBase pickup = null)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Method ServerAddItem can only be executed on the server.");
            }
            if (inv.UserInventory.Items.Count >= 8 && global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(type, out var value) && value.Category != ItemCategory.Ammo)
            {
                return null;
            }
            if (itemSerial == 0)
            {
                itemSerial = global::InventorySystem.Items.ItemSerialGenerator.GenerateNext();
            }
            global::InventorySystem.Items.ItemBase itemBase = inv.CreateItemInstance(new global::InventorySystem.Items.ItemIdentifier(type, itemSerial), inv.isLocalPlayer);
            if (itemBase == null)
            {
                return null;
            }
            inv.UserInventory.Items[itemSerial] = itemBase;
            itemBase.OnAdded(pickup);
            global::InventorySystem.InventoryExtensions.OnItemAdded?.Invoke(inv._hub, itemBase, pickup);
            if (inv.isLocalPlayer && itemBase is global::InventorySystem.Items.IAcquisitionConfirmationTrigger acquisitionConfirmationTrigger)
            {
                acquisitionConfirmationTrigger.ServerConfirmAcqusition();
                acquisitionConfirmationTrigger.AcquisitionAlreadyReceived = true;
            }
            inv.SendItemsNextFrame = true;
            return itemBase;
        }

        public static void ServerRemoveItem(this global::InventorySystem.Inventory inv, ushort itemSerial, global::InventorySystem.Items.Pickups.ItemPickupBase ipb)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Method ServerRemoveItem can only be executed on the server.");
            }
            if (inv.DestroyItemInstance(itemSerial, ipb, out var foundItem))
            {
                if (itemSerial == inv.CurItem.SerialNumber)
                {
                    inv.CurItem = global::InventorySystem.Items.ItemIdentifier.None;
                }
                inv.UserInventory.Items.Remove(itemSerial);
                inv.SendItemsNextFrame = true;
                global::InventorySystem.InventoryExtensions.OnItemRemoved?.Invoke(inv._hub, foundItem, ipb);
            }
        }

        public static global::InventorySystem.Items.Pickups.ItemPickupBase ServerDropItem(this global::InventorySystem.Inventory inv, ushort itemSerial)
        {
            if (!inv.UserInventory.Items.TryGetValue(itemSerial, out var value))
            {
                return null;
            }
            return value.ServerDropItem();
        }

        public static void ServerDropEverything(this global::InventorySystem.Inventory inv)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Method ServerDropEverything can only be executed on the server.");
            }
            global::System.Collections.Generic.HashSet<ItemType> hashSet = global::NorthwoodLib.Pools.HashSetPool<ItemType>.Shared.Rent();
            foreach (global::System.Collections.Generic.KeyValuePair<ItemType, ushort> item in inv.UserInventory.ReserveAmmo)
            {
                if (item.Value > 0)
                {
                    hashSet.Add(item.Key);
                }
            }
            foreach (ItemType item2 in hashSet)
            {
                inv.ServerDropAmmo(item2, ushort.MaxValue);
            }
            while (inv.UserInventory.Items.Count > 0)
            {
                inv.ServerDropItem(global::System.Linq.Enumerable.ElementAt(inv.UserInventory.Items, 0).Key);
            }
            global::NorthwoodLib.Pools.HashSetPool<ItemType>.Shared.Return(hashSet);
        }

        public static bool ServerDropAmmo(this global::InventorySystem.Inventory inv, ItemType ammoType, ushort amount, bool checkMinimals = false)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Method ServerDropAmmo can only be executed on the server.");
            }
            if (inv.UserInventory.ReserveAmmo.TryGetValue(ammoType, out var value) && global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(ammoType, out var value2))
            {
                if (value2.PickupDropModel == null)
                {
                    global::UnityEngine.Debug.LogError("No pickup drop model set. Could not drop the ammo.");
                    return false;
                }
                if (checkMinimals && value2.PickupDropModel is global::InventorySystem.Items.Firearms.Ammo.AmmoPickup ammoPickup)
                {
                    int num = global::UnityEngine.Mathf.FloorToInt((float)(int)ammoPickup.SavedAmmo / 2f);
                    if (amount < num && value > num)
                    {
                        amount = (ushort)num;
                    }
                }
                int num2 = global::UnityEngine.Mathf.Min(amount, value);
                inv.UserInventory.ReserveAmmo[ammoType] = (ushort)(value - num2);
                inv.SendAmmoNextFrame = true;
                while (num2 > 0)
                {
                    // SavedAmmo must be set BEFORE the pickup is spawned: the spawn payload is
                    // serialized inside NetworkServer.Spawn, and in host mode the local client
                    // deserializes it back onto the very same object a frame later, overwriting
                    // anything written after the spawn call.
                    global::InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = ServerCreatePickup(psi: new global::InventorySystem.Items.Pickups.PickupSyncInfo(ammoType, inv.transform.position, global::UnityEngine.Quaternion.identity, value2.Weight, 0), inv: inv, item: value2, spawn: false);
                    if (itemPickupBase is global::InventorySystem.Items.Firearms.Ammo.AmmoPickup ammoPickup2)
                    {
                        ammoPickup2.SavedAmmo = (ushort)global::UnityEngine.Mathf.Min(ammoPickup2.MaxAmmo, num2);
                        num2 -= ammoPickup2.SavedAmmo;
                    }
                    else
                    {
                        num2--;
                    }
                    global::Mirror.NetworkServer.Spawn(itemPickupBase.gameObject);
                }
                return amount <= value;
            }
            return false;
        }

        public static global::InventorySystem.Items.Pickups.ItemPickupBase ServerCreatePickup(this global::InventorySystem.Inventory inv, global::InventorySystem.Items.ItemBase item, global::InventorySystem.Items.Pickups.PickupSyncInfo psi, bool spawn = true)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Method ServerCreatePickup can only be executed on the server.");
            }
            global::InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = global::UnityEngine.Object.Instantiate(item.PickupDropModel, inv.transform.position, ReferenceHub.GetHub(inv.gameObject).PlayerCameraReference.rotation * item.PickupDropModel.transform.rotation);
            itemPickupBase.Info = psi;
            if (spawn)
            {
                global::Mirror.NetworkServer.Spawn(itemPickupBase.gameObject);
            }
            itemPickupBase.InfoReceived(default(global::InventorySystem.Items.Pickups.PickupSyncInfo), psi);
            return itemPickupBase;
        }

        public static ushort GetCurAmmo(this global::InventorySystem.Inventory inv, ItemType ammoType)
        {
            if (!inv.UserInventory.ReserveAmmo.TryGetValue(ammoType, out var value))
            {
                return 0;
            }
            return value;
        }

        public static void ServerSetAmmo(this global::InventorySystem.Inventory inv, ItemType ammoType, int amount)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Method ServerSetAmmo can only be executed on the server.");
            }
            amount = global::UnityEngine.Mathf.Clamp(amount, 0, 65535);
            inv.UserInventory.ReserveAmmo[ammoType] = (ushort)amount;
            inv.SendAmmoNextFrame = true;
        }

        public static void ServerAddAmmo(this global::InventorySystem.Inventory inv, ItemType ammoType, int amountToAdd)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Method ServerAddAmmo can only be executed on the server.");
            }
            inv.ServerSetAmmo(ammoType, inv.GetCurAmmo(ammoType) + amountToAdd);
        }
    }
}
