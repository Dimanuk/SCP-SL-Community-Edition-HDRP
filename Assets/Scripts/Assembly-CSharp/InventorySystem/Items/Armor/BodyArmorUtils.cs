namespace InventorySystem.Items.Armor
{
    public static class BodyArmorUtils
    {
        public static bool TryGetBodyArmor(this global::InventorySystem.Inventory inv, out global::InventorySystem.Items.Armor.BodyArmor bodyArmor)
        {
            return inv.TryGetBodyArmorAndItsSerial(out bodyArmor, out _);
        }

        public static bool TryGetBodyArmorAndItsSerial(this global::InventorySystem.Inventory inv, out global::InventorySystem.Items.Armor.BodyArmor bodyArmor, out ushort serial)
        {
            foreach (global::System.Collections.Generic.KeyValuePair<ushort, global::InventorySystem.Items.ItemBase> item in inv.UserInventory.Items)
            {
                if (item.Value is global::InventorySystem.Items.Armor.BodyArmor bodyArmor2)
                {
                    serial = item.Key;
                    bodyArmor = bodyArmor2;
                    return true;
                }
            }
            serial = 0;
            bodyArmor = null;
            return false;
        }

        public static float ProcessDamage(int efficacy, float baseDamage, int bulletPenetrationPercent)
        {
            float num = (float)efficacy / 100f;
            float num2 = (float)bulletPenetrationPercent / 100f;
            return baseDamage * (1f - num * (1f - num2));
        }

        public static void RemoveEverythingExceedingLimits(global::InventorySystem.Inventory inv, global::InventorySystem.Items.Armor.BodyArmor armor, bool removeItems = true, bool removeAmmo = true)
        {
            global::System.Collections.Generic.HashSet<ushort> hashSet = global::NorthwoodLib.Pools.HashSetPool<ushort>.Shared.Rent();
            global::System.Collections.Generic.Dictionary<ItemCategory, int> dictionary = new global::System.Collections.Generic.Dictionary<ItemCategory, int>();
            global::System.Collections.Generic.Dictionary<ItemType, ushort> dictionary2 = new global::System.Collections.Generic.Dictionary<ItemType, ushort>();
            foreach (global::System.Collections.Generic.KeyValuePair<ushort, global::InventorySystem.Items.ItemBase> item in inv.UserInventory.Items)
            {
                if (item.Value.Category != ItemCategory.Armor)
                {
                    int num = global::UnityEngine.Mathf.Abs(global::InventorySystem.Configs.InventoryLimits.GetCategoryLimit(armor, item.Value.Category));
                    int value = ((!dictionary.TryGetValue(item.Value.Category, out value)) ? 1 : (value + 1));
                    if (value > num)
                    {
                        hashSet.Add(item.Key);
                    }
                    dictionary[item.Value.Category] = value;
                }
            }
            foreach (global::System.Collections.Generic.KeyValuePair<ItemType, ushort> item2 in inv.UserInventory.ReserveAmmo)
            {
                ushort ammoLimit = global::InventorySystem.Configs.InventoryLimits.GetAmmoLimit(armor, item2.Key);
                if (item2.Value > ammoLimit)
                {
                    dictionary2.Add(item2.Key, (ushort)(item2.Value - ammoLimit));
                }
            }
            if (removeItems)
            {
                foreach (ushort item3 in hashSet)
                {
                    inv.ServerDropItem(item3);
                }
            }
            if (!removeAmmo)
            {
                return;
            }
            foreach (global::System.Collections.Generic.KeyValuePair<ItemType, ushort> item4 in dictionary2)
            {
                inv.ServerDropAmmo(item4.Key, item4.Value);
            }
        }
    }
}
