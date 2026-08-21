using System.Collections.Generic;

namespace InventorySystem.Configs
{
	public static class InventoryLimits
	{
		public static readonly Dictionary<ItemType, ushort> StandardAmmoLimits = new()
        {
            [ItemType.Ammo9x19] = 30,
            [ItemType.Ammo556x45] = 40,
            [ItemType.Ammo762x39] = 40,
            [ItemType.Ammo44cal] = 18,
            [ItemType.Ammo12gauge] = 14
        };

        public static readonly Dictionary<ItemCategory, sbyte> StandardCategoryLimits = new()
        {
			[ItemCategory.Armor] = -1,
			[ItemCategory.Grenade] = 2,
			[ItemCategory.Keycard] = 3,
			[ItemCategory.Medical] = 3,
			[ItemCategory.MicroHID] = -1,
			[ItemCategory.Radio] = -1,
			[ItemCategory.SCPItem] = 3,
			[ItemCategory.Firearm] = 1
        };

        private static ServerConfigSynchronizer Config => ServerConfigSynchronizer.Singleton;

        public static ushort GetAmmoLimit(ItemType ammoType, ReferenceHub player)
        {
            global::InventorySystem.Items.Armor.BodyArmor bodyArmor;
            return GetAmmoLimit((player != null && global::InventorySystem.Items.Armor.BodyArmorUtils.TryGetBodyArmor(player.inventory, out bodyArmor)) ? bodyArmor : null, ammoType);
        }

        public static sbyte GetCategoryLimit(ItemCategory category, ReferenceHub player)
        {
            global::InventorySystem.Items.Armor.BodyArmor bodyArmor;
            return GetCategoryLimit((player != null && global::InventorySystem.Items.Armor.BodyArmorUtils.TryGetBodyArmor(player.inventory, out bodyArmor)) ? bodyArmor : null, category);
        }

        public static ushort GetAmmoLimit(global::InventorySystem.Items.Armor.BodyArmor armor, ItemType ammoType)
        {
            int num = -1;
            foreach (ServerConfigSynchronizer.AmmoLimit item in Config.AmmoLimitsSync)
            {
                if (item.AmmoType == ammoType)
                {
                    if (item.Limit == 0)
                    {
                        return ushort.MaxValue;
                    }
                    num = item.Limit;
                    break;
                }
            }
            if (num == -1)
            {
                if (!StandardAmmoLimits.TryGetValue(ammoType, out var value))
                {
                    return ushort.MaxValue;
                }
                num = value;
            }
            if (armor != null)
            {
                global::InventorySystem.Items.Armor.BodyArmor.ArmorAmmoLimit[] ammoLimits = armor.AmmoLimits;
                for (int i = 0; i < ammoLimits.Length; i++)
                {
                    global::InventorySystem.Items.Armor.BodyArmor.ArmorAmmoLimit armorAmmoLimit = ammoLimits[i];
                    if (armorAmmoLimit.AmmoType == ammoType)
                    {
                        num += armorAmmoLimit.Limit;
                        break;
                    }
                }
            }
            return (ushort)global::UnityEngine.Mathf.Min(65535, num);
        }

        public static sbyte GetCategoryLimit(global::InventorySystem.Items.Armor.BodyArmor armor, ItemCategory category)
        {
            int num = Config.CategoryLimits.Count;
            int i = 0;
            int num2 = 0;
            for (; global::System.Enum.IsDefined(typeof(ItemCategory), (ItemCategory)i); i++)
            {
                ItemCategory itemCategory = (ItemCategory)i;
                if (itemCategory == category)
                {
                    num = num2;
                    break;
                }
                if (StandardCategoryLimits.TryGetValue(itemCategory, out var value) && value >= 0)
                {
                    num2++;
                }
            }
            int num3;
            if (num < Config.CategoryLimits.Count)
            {
                num3 = Config.CategoryLimits[num];
            }
            else
            {
                if (!StandardCategoryLimits.TryGetValue(category, out var value2))
                {
                    return 8;
                }
                num3 = value2;
            }
            if (armor != null)
            {
                global::InventorySystem.Items.Armor.BodyArmor.ArmorCategoryLimitModifier[] categoryLimits = armor.CategoryLimits;
                for (int j = 0; j < categoryLimits.Length; j++)
                {
                    global::InventorySystem.Items.Armor.BodyArmor.ArmorCategoryLimitModifier armorCategoryLimitModifier = categoryLimits[j];
                    if (armorCategoryLimitModifier.Category == category)
                    {
                        num3 += armorCategoryLimitModifier.Limit;
                        break;
                    }
                }
            }
            return (sbyte)global::UnityEngine.Mathf.Clamp(num3, -8, 8);
        }
    }
}
