using System.Collections.Generic;
using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem
{
	public static class InventoryItemLoader
	{
        private static global::System.Collections.Generic.Dictionary<ItemType, global::InventorySystem.Items.ItemBase> _loadedItems = new global::System.Collections.Generic.Dictionary<ItemType, global::InventorySystem.Items.ItemBase>();

        private static bool _loaded;

        private const string ItemsDirectoryName = "Defined Items";

        public static global::System.Collections.Generic.Dictionary<ItemType, global::InventorySystem.Items.ItemBase> AvailableItems
        {
            get
            {
                if (!_loaded)
                {
                    ForceReload();
                }
                return _loadedItems;
            }
        }

        public static bool TryGetItem<T>(ItemType itemType, out T result) where T : global::InventorySystem.Items.ItemBase
        {
            if (!AvailableItems.TryGetValue(itemType, out var value) || !(value is T val))
            {
                result = null;
                return false;
            }
            result = val;
            return true;
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientStarted += RegisterPrefabs;
        }

        private static void RegisterPrefabs()
        {
            global::System.Collections.Generic.HashSet<global::UnityEngine.GameObject> hashSet = global::NorthwoodLib.Pools.HashSetPool<global::UnityEngine.GameObject>.Shared.Rent();
            foreach (global::InventorySystem.Items.ItemBase value in AvailableItems.Values)
            {
                // A single item with a missing PickupDropModel must not abort the whole
                // loop with an NRE — that would leave every later item unregistered and
                // cause "Failed to spawn server object" for their pickups on clients.
                if (value == null || value.PickupDropModel == null)
                {
                    global::UnityEngine.Debug.LogError("Item '" + (value != null ? value.name : "<null>") + "' has no PickupDropModel; skipping pickup registration.");
                    continue;
                }
                if (hashSet.Add(value.PickupDropModel.gameObject))
                {
                    global::Mirror.NetworkClient.RegisterPrefab(value.PickupDropModel.gameObject);
                }
            }
            global::UnityEngine.Debug.Log("Successfully registered " + hashSet.Count + " pickups for " + AvailableItems.Count + " items.");
            global::NorthwoodLib.Pools.HashSetPool<global::UnityEngine.GameObject>.Shared.Return(hashSet);
        }


        public static void ForceReload()
        {
            try
            {
                _loadedItems = new global::System.Collections.Generic.Dictionary<ItemType, global::InventorySystem.Items.ItemBase>();
                global::InventorySystem.Items.ItemBase[] array = global::UnityEngine.Resources.LoadAll<global::InventorySystem.Items.ItemBase>(ItemsDirectoryName);
                global::System.Array.Sort(array, delegate (global::InventorySystem.Items.ItemBase x, global::InventorySystem.Items.ItemBase y)
                {
                    int itemTypeId = (int)x.ItemTypeId;
                    return itemTypeId.CompareTo((int)y.ItemTypeId);
                });
                global::InventorySystem.Items.ItemBase[] array2 = array;
                foreach (global::InventorySystem.Items.ItemBase itemBase in array2)
                {
                    if (itemBase.ItemTypeId == ItemType.None)
                    {
                        throw new global::System.InvalidOperationException(string.Format("Failed to load item '{0}' - {1} cannot be {2}.", itemBase.name, "ItemType", ItemType.None));
                    }
                    _loadedItems[itemBase.ItemTypeId] = itemBase;
                    itemBase.OnTemplateReloaded(_loaded);
                }
                _loaded = true;
            }
            catch (global::System.Exception ex)
            {
                global::UnityEngine.Debug.LogError("Error while loading items from the resources folder: " + ex.Message);
                _loaded = false;
            }
        }
    }
}
