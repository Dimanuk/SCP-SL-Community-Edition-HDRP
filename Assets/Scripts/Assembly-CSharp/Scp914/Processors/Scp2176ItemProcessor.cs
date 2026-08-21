namespace Scp914.Processors
{
    public class Scp2176ItemProcessor : global::Scp914.Processors.StandardItemProcessor
    {
        private const float NumOfCoins = 12f;

        private const float NumOfFlashlights = 5f;

        private const float FlashlightChance = 0.2f;

        public override global::InventorySystem.Items.Pickups.ItemPickupBase OnPickupUpgraded(global::Scp914.Scp914KnobSetting setting, global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::UnityEngine.Vector3 newPosition)
        {
            if (ipb.Info.ItemId != ItemType.SCP2176)
            {
                return null;
            }
            switch (setting)
            {
                case global::Scp914.Scp914KnobSetting.Rough:
                    if (ipb is global::InventorySystem.Items.ThrowableProjectiles.Scp2176Projectile scp2176Projectile)
                    {
                        scp2176Projectile.ServerImmediatelyShatter();
                        return ipb;
                    }
                    break;
                case global::Scp914.Scp914KnobSetting.OneToOne:
                    {
                        for (int j = 0; (float)j < 12f; j++)
                        {
                            SpawnItem(ItemType.Coin, newPosition, ipb.transform.rotation);
                        }
                        ipb.DestroySelf();
                        return null;
                    }
                case global::Scp914.Scp914KnobSetting.VeryFine:
                    if (!(global::UnityEngine.Random.value < 0.2f))
                    {
                        for (int i = 0; (float)i < 5f; i++)
                        {
                            SpawnItem(ItemType.Flashlight, newPosition, ipb.transform.rotation);
                        }
                        ipb.DestroySelf();
                        return null;
                    }
                    break;
            }
            return base.OnPickupUpgraded(setting, ipb, newPosition);
        }

        public override global::InventorySystem.Items.ItemBase OnInventoryItemUpgraded(global::Scp914.Scp914KnobSetting setting, ReferenceHub hub, ushort serial)
        {
            switch (setting)
            {
                case global::Scp914.Scp914KnobSetting.OneToOne:
                    {
                        for (int j = 0; (float)j < 12f; j++)
                        {
                            SpawnItem(ItemType.Coin, hub.transform.position, global::UnityEngine.Quaternion.identity);
                        }
                        break;
                    }
                case global::Scp914.Scp914KnobSetting.VeryFine:
                    if (!(global::UnityEngine.Random.value < 0.2f))
                    {
                        for (int i = 0; (float)i < 5f; i++)
                        {
                            SpawnItem(ItemType.Flashlight, hub.transform.position, global::UnityEngine.Quaternion.identity);
                        }
                        global::InventorySystem.InventoryExtensions.ServerRemoveItem(hub.inventory, serial, null);
                        return null;
                    }
                    break;
            }
            return base.OnInventoryItemUpgraded(setting, hub, serial);
        }

        private void SpawnItem(ItemType itemType, global::UnityEngine.Vector3 position, global::UnityEngine.Quaternion rotation)
        {
            if (global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(itemType, out var value) && ReferenceHub.TryGetLocalHub(out var hub))
            {
                global::InventorySystem.Items.Pickups.PickupSyncInfo psi = new global::InventorySystem.Items.Pickups.PickupSyncInfo
                {
                    ItemId = itemType,
                    Serial = global::InventorySystem.Items.ItemSerialGenerator.GenerateNext(),
                    Weight = value.Weight
                };
                global::InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = global::InventorySystem.InventoryExtensions.ServerCreatePickup(hub.inventory, value, psi);
                itemPickupBase.transform.position = position;
                itemPickupBase.transform.rotation = rotation;
                itemPickupBase.RefreshPositionAndRotation();
            }
        }
    }
}
