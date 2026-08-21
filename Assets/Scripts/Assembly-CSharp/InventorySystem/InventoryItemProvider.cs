namespace InventorySystem
{
	public static class InventoryItemProvider
	{
        public static global::System.Action<ReferenceHub, global::InventorySystem.Items.ItemBase> OnItemProvided;

        private static readonly global::System.Collections.Generic.Dictionary<ReferenceHub, global::System.Collections.Generic.List<global::InventorySystem.Items.Pickups.ItemPickupBase>> PreviousInventoryPickups = new global::System.Collections.Generic.Dictionary<ReferenceHub, global::System.Collections.Generic.List<global::InventorySystem.Items.Pickups.ItemPickupBase>>();

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::PlayerRoles.PlayerRoleManager.OnRoleChanged += RoleChanged;
        }

        public static void ServerGrantLoadout(ReferenceHub target, global::PlayerRoles.RoleTypeId roleTypeId, bool resetInventory = true)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Method ServerGrantLoadout can only be executed on the server.");
            }
            global::InventorySystem.Inventory inventory = target.inventory;
            if (resetInventory)
            {
                while (inventory.UserInventory.Items.Count > 0)
                {
                    inventory.ServerRemoveItem(global::System.Linq.Enumerable.ElementAt(inventory.UserInventory.Items, 0).Key, null);
                }
                inventory.UserInventory.ReserveAmmo.Clear();
                inventory.SendAmmoNextFrame = true;
            }
            if (!global::InventorySystem.Configs.StartingInventories.DefinedInventories.TryGetValue(roleTypeId, out var value))
            {
                return;
            }
            foreach (global::System.Collections.Generic.KeyValuePair<ItemType, ushort> item in value.Ammo)
            {
                inventory.ServerAddAmmo(item.Key, item.Value);
            }
            for (int i = 0; i < value.Items.Length; i++)
            {
                global::InventorySystem.Items.ItemBase arg = inventory.ServerAddItem(value.Items[i], 0);
                OnItemProvided?.Invoke(target, arg);
            }
        }

        private static void SpawnPreviousInventoryPickups(ReferenceHub hub)
        {
            if (!PreviousInventoryPickups.TryGetValue(hub, out var value))
            {
                return;
            }
            hub.transform.position = hub.transform.position;
            foreach (global::InventorySystem.Items.Pickups.ItemPickupBase item in value)
            {
                global::InventorySystem.Inventory inventory = hub.inventory;
                global::InventorySystem.Items.Pickups.PickupSyncInfo info = item.Info;
                if (!info.Locked && global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(info.ItemId, out var value2))
                {
                    if (inventory.UserInventory.Items.Count < 8 && value2.Category != ItemCategory.Armor)
                    {
                        inventory.ServerAddItem(info.ItemId, info.Serial, item);
                        global::InventorySystem.Items.Armor.BodyArmorUtils.RemoveEverythingExceedingLimits(inventory, global::InventorySystem.Items.Armor.BodyArmorUtils.TryGetBodyArmor(inventory, out var bodyArmor) ? bodyArmor : null);
                        item.DestroySelf();
                    }
                    else
                    {
                        item.transform.position = hub.transform.position;
                        item.RefreshPositionAndRotation();
                    }
                }
            }
            PreviousInventoryPickups.Remove(hub);
        }

        private static void RoleChanged(ReferenceHub ply, global::PlayerRoles.PlayerRoleBase prevRole, global::PlayerRoles.PlayerRoleBase newRole)
        {
            if (!global::Mirror.NetworkServer.active || !newRole.ServerSpawnFlags.HasFlag(global::PlayerRoles.RoleSpawnFlags.AssignInventory))
            {
                return;
            }
            global::InventorySystem.Inventory inventory = ply.inventory;
            bool flag = newRole.ServerSpawnReason == global::PlayerRoles.RoleChangeReason.Escaped;
            if (flag)
            {
                global::System.Collections.Generic.List<global::InventorySystem.Items.Pickups.ItemPickupBase> list = new global::System.Collections.Generic.List<global::InventorySystem.Items.Pickups.ItemPickupBase>();
                if (global::InventorySystem.Items.Armor.BodyArmorUtils.TryGetBodyArmor(inventory, out var bodyArmor))
                {
                    bodyArmor.DontRemoveExcessOnDrop = true;
                }
                while (inventory.UserInventory.Items.Count > 0)
                {
                    list.Add(inventory.ServerDropItem(global::System.Linq.Enumerable.ElementAt(inventory.UserInventory.Items, 0).Key));
                }
                PreviousInventoryPickups[ply] = list;
            }
            ServerGrantLoadout(ply, newRole.RoleTypeId, !flag);
            SpawnPreviousInventoryPickups(ply);
        }
    }
}
