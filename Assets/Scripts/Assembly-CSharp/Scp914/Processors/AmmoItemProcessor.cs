namespace Scp914.Processors
{
    public class AmmoItemProcessor : global::Scp914.Processors.Scp914ItemProcessor
    {
        [global::UnityEngine.SerializeField]
        private ItemType _previousAmmo;

        [global::UnityEngine.SerializeField]
        private ItemType _oneToOne;

        [global::UnityEngine.SerializeField]
        private ItemType _nextAmmo;

        public override global::InventorySystem.Items.ItemBase OnInventoryItemUpgraded(global::Scp914.Scp914KnobSetting setting, ReferenceHub hub, ushort serial)
        {
            return null;
        }

        public override global::InventorySystem.Items.Pickups.ItemPickupBase OnPickupUpgraded(global::Scp914.Scp914KnobSetting setting, global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::UnityEngine.Vector3 newPos)
        {
            ItemType itemType;
            switch (setting)
            {
                case global::Scp914.Scp914KnobSetting.Rough:
                case global::Scp914.Scp914KnobSetting.Coarse:
                    itemType = _previousAmmo;
                    break;
                case global::Scp914.Scp914KnobSetting.OneToOne:
                    itemType = _oneToOne;
                    break;
                case global::Scp914.Scp914KnobSetting.Fine:
                case global::Scp914.Scp914KnobSetting.VeryFine:
                    itemType = _nextAmmo;
                    break;
                default:
                    return ipb;
            }
            ipb.transform.position = newPos;
            if (!(ipb is global::InventorySystem.Items.Firearms.Ammo.AmmoPickup ammoPickup))
            {
                return ipb;
            }
            ExchangeAmmo(ammoPickup.Info.ItemId, itemType, ammoPickup.SavedAmmo, out var exchangedAmmo, out var change);
            if (change == 0)
            {
                ammoPickup.DestroySelf();
            }
            else
            {
                ammoPickup.SavedAmmo = (ushort)change;
            }
            return CreateAmmoPickup(itemType, exchangedAmmo, newPos);
        }

        public static global::InventorySystem.Items.Pickups.ItemPickupBase CreateAmmoPickup(ItemType type, int bullets, global::UnityEngine.Vector3 pos)
        {
            if (bullets <= 0 || !global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(type, out var value))
            {
                return null;
            }
            global::InventorySystem.Items.Pickups.PickupSyncInfo psi = new global::InventorySystem.Items.Pickups.PickupSyncInfo
            {
                ItemId = type,
                Serial = global::InventorySystem.Items.ItemSerialGenerator.GenerateNext(),
                Weight = value.Weight
            };
            if (global::InventorySystem.InventoryExtensions.ServerCreatePickup(ReferenceHub.LocalHub.inventory, value, psi) is global::InventorySystem.Items.Firearms.Ammo.AmmoPickup ammoPickup)
            {
                ammoPickup.SavedAmmo = (ushort)bullets;
                ammoPickup.transform.position = pos;
                ammoPickup.RefreshPositionAndRotation();
                return ammoPickup;
            }
            return null;
        }

        public static void ExchangeAmmo(ItemType ammoTypeToExchange, ItemType targetAmmoType, int amount, out int exchangedAmmo, out int change)
        {
            if (!TryGetAmmoItem(ammoTypeToExchange, out var ammoItem) || !global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(targetAmmoType, out var value) || !(value is global::InventorySystem.Items.Firearms.Ammo.AmmoItem ammoItem2))
            {
                exchangedAmmo = 0;
                change = 0;
                return;
            }
            int unitPrice = ammoItem.UnitPrice;
            int unitPrice2 = ammoItem2.UnitPrice;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            for (int i = 0; i < amount; i++)
            {
                num3 += unitPrice;
                num++;
                if (num3 % unitPrice2 == 0)
                {
                    num2 += num3 / unitPrice2;
                    num = 0;
                    num3 = 0;
                }
            }
            exchangedAmmo = num2;
            change = num;
        }

        private static bool TryGetAmmoItem(ItemType type, out global::InventorySystem.Items.Firearms.Ammo.AmmoItem ammoItem)
        {
            if (global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(type, out var value) && value is global::InventorySystem.Items.Firearms.Ammo.AmmoItem ammoItem2)
            {
                ammoItem = ammoItem2;
                return true;
            }
            ammoItem = null;
            return false;
        }
    }
}
