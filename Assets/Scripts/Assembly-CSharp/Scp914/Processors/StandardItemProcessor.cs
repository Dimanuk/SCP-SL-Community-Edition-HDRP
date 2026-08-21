namespace Scp914.Processors
{
    public class StandardItemProcessor : global::Scp914.Processors.Scp914ItemProcessor
    {
        [global::UnityEngine.SerializeField]
        private ItemType[] _roughOutputs;

        [global::UnityEngine.SerializeField]
        private ItemType[] _coarseOutputs;

        [global::UnityEngine.SerializeField]
        private ItemType[] _oneToOneOutputs;

        [global::UnityEngine.SerializeField]
        private ItemType[] _fineOutputs;

        [global::UnityEngine.SerializeField]
        private ItemType[] _veryFineOutputs;

        [global::UnityEngine.SerializeField]
        private bool _fireUpgradeTrigger;

        public override global::InventorySystem.Items.ItemBase OnInventoryItemUpgraded(global::Scp914.Scp914KnobSetting setting, ReferenceHub hub, ushort serial)
        {
            if (!hub.inventory.UserInventory.Items.TryGetValue(serial, out var value))
            {
                return null;
            }
            ItemType itemType = RandomOutput(setting, value.ItemTypeId);
            if (itemType == value.ItemTypeId)
            {
                if (_fireUpgradeTrigger && value is global::InventorySystem.Items.IUpgradeTrigger upgradeTrigger)
                {
                    upgradeTrigger.ServerOnUpgraded(setting);
                }
                return null;
            }
            global::InventorySystem.InventoryExtensions.ServerRemoveItem(hub.inventory, serial, null);
            if (global::InventorySystem.InventoryItemLoader.AvailableItems.ContainsKey(itemType))
            {
                return global::InventorySystem.InventoryExtensions.ServerAddItem(hub.inventory, itemType, 0);
            }
            return null;
        }

        public override global::InventorySystem.Items.Pickups.ItemPickupBase OnPickupUpgraded(global::Scp914.Scp914KnobSetting setting, global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::UnityEngine.Vector3 newPosition)
        {
            ItemType itemType = RandomOutput(setting, ipb.Info.ItemId);
            if (itemType == ItemType.None)
            {
                HandleNone(ipb, newPosition);
                return null;
            }
            if (itemType == ipb.Info.ItemId || !global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(itemType, out var value))
            {
                ipb.transform.position = newPosition;
                if (_fireUpgradeTrigger && ipb is global::InventorySystem.Items.IUpgradeTrigger upgradeTrigger)
                {
                    upgradeTrigger.ServerOnUpgraded(setting);
                }
                return ipb;
            }
            global::InventorySystem.Items.Pickups.PickupSyncInfo psi = new()
            {
                ItemId = itemType,
                Serial = global::InventorySystem.Items.ItemSerialGenerator.GenerateNext(),
                Weight = value.Weight
            };
            global::InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = global::InventorySystem.InventoryExtensions.ServerCreatePickup(ReferenceHub.LocalHub.inventory, value, psi);
            itemPickupBase.transform.position = newPosition;
            itemPickupBase.transform.rotation = ipb.transform.rotation;
            itemPickupBase.RefreshPositionAndRotation();
            HandleOldPickup(ipb, newPosition);
            return itemPickupBase;
        }

        protected virtual void HandleNone(global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::UnityEngine.Vector3 newPosition)
        {
            ipb.DestroySelf();
        }

        protected virtual void HandleOldPickup(global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::UnityEngine.Vector3 newPosition)
        {
            ipb.DestroySelf();
        }

        private ItemType RandomOutput(global::Scp914.Scp914KnobSetting setting, ItemType id)
        {
            ItemType[] array;
            switch (setting)
            {
                case global::Scp914.Scp914KnobSetting.Rough:
                    array = _roughOutputs;
                    break;
                case global::Scp914.Scp914KnobSetting.Coarse:
                    array = _coarseOutputs;
                    break;
                case global::Scp914.Scp914KnobSetting.OneToOne:
                    array = _oneToOneOutputs;
                    break;
                case global::Scp914.Scp914KnobSetting.Fine:
                    array = _fineOutputs;
                    break;
                case global::Scp914.Scp914KnobSetting.VeryFine:
                    array = _veryFineOutputs;
                    break;
                default:
                    return id;
            }
            return array[global::UnityEngine.Random.Range(0, array.Length)];
        }
    }
}
