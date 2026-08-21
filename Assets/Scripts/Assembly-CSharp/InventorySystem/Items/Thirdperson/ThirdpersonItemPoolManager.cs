
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace InventorySystem.Items.Thirdperson
{
	public static class ThirdpersonItemPoolManager
	{
		private static bool _poolsInitiated;

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientStarted += SetupPools;
        }

        private static void SetupPools()
        {
            if (_poolsInitiated)
            {
                return;
            }
            foreach (global::System.Collections.Generic.KeyValuePair<ItemType, global::InventorySystem.Items.ItemBase> availableItem in global::InventorySystem.InventoryItemLoader.AvailableItems)
            {
                global::GameObjectPools.PoolObject thirdpersonModel = availableItem.Value.ThirdpersonModel;
                if (thirdpersonModel != null)
                {
                    global::GameObjectPools.PoolManager.Singleton.TryAddPool(thirdpersonModel);
                }
            }
            _poolsInitiated = true;
        }

        public static bool TryGet(global::PlayerRoles.FirstPersonControl.Thirdperson.HumanCharacterModel model, global::InventorySystem.Items.ItemIdentifier item, out global::InventorySystem.Items.Thirdperson.ThirdpersonItemBase result, bool restrictPoolingToItem = false)
        {
            if (restrictPoolingToItem)
            {
                if (!global::InventorySystem.InventoryItemLoader.TryGetItem<global::InventorySystem.Items.ItemBase>(item.TypeId, out var result2) || result2.ThirdpersonModel == null)
                {
                    result = null;
                    return false;
                }
                global::GameObjectPools.PoolManager.Singleton.TryAddPool(result2.ThirdpersonModel);
            }
            else
            {
                SetupPools();
            }
            result = null;
            if (model.ItemSpawnpoint == null)
            {
                return false;
            }
            if (!global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(item.TypeId, out var value))
            {
                return false;
            }
            if (value.ThirdpersonModel == null)
            {
                return false;
            }
            if (!global::GameObjectPools.PoolManager.Singleton.TryGetPoolObject(value.ThirdpersonModel.gameObject, out var poolObject))
            {
                return false;
            }
            result = poolObject as global::InventorySystem.Items.Thirdperson.ThirdpersonItemBase;
            result.Initialize(model, item);
            result.SetupPoolObject();
            return true;
        }
    }
}
