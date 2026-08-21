using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using NorthwoodLib.Pools;
using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace InventorySystem.GUI.Descriptions
{
	public class ArmorDescriptionGui : RadialDescriptionBase
	{
		[SerializeField]
		private TextMeshProUGUI _title;

		[SerializeField]
		private TextMeshProUGUI _itemLimits;

		[SerializeField]
		private TextMeshProUGUI _ammoLimits;

		[SerializeField]
		private FirearmDescriptionGui.FirearmStatBar _helmetBar;

		[SerializeField]
		private FirearmDescriptionGui.FirearmStatBar _vestBar;

		[SerializeField]
		private FirearmDescriptionGui.FirearmStatBar _staminaBar;

		[SerializeField]
		private FirearmDescriptionGui.FirearmStatBar _movementBar;

        public override void UpdateInfo(ItemBase targetItem, Color roleColor)
        {
            BodyArmor armor = targetItem as BodyArmor;
            if (armor == null)
                return;

            // Use the translated item name (IItemNametag), not ToString() which prints
            // "BodyArmor(Clone) (InventorySystem.Items.Armor.BodyArmor)". BodyArmor implements
            // IItemNametag.Name explicitly, so cast to reach it.
            _title.text = ((IItemNametag)armor).Name;

            _itemLimits.color = roleColor;
            _ammoLimits.color = roleColor;

            // HelmetEfficacy/VestEfficacy are integer percentages (0-100): the bar is normalized
            // to 0-1 and the label shows the raw percentage. Multiplying by 100 gave "8000%".
            string helmetValueText = string.Concat(armor.HelmetEfficacy, "%");
            _helmetBar.SetValue(armor.HelmetEfficacy / 100f, helmetValueText, Color.white);

            string vestValueText = string.Concat(armor.VestEfficacy, "%");
            _vestBar.SetValue(armor.VestEfficacy / 100f, vestValueText, Color.white);

            float staminaUsageMultiplier = armor.StaminaUsageMultiplier;
            float movementSpeedMultiplier = armor.MovementSpeedMultiplier;

            // Stamina drain is shown as the extra usage over 1.0 ("+20%"); movement as the speed
            // loss below 1.0 ("-10%"). The bar fill uses the relative delta / raw multiplier.
            string staminaText = string.Concat("+", Mathf.Round((staminaUsageMultiplier - 1f) * 100f), "%");
            _staminaBar.SetValue(staminaUsageMultiplier - 1f, staminaText, Color.clear);

            string movementText = string.Concat("-", Mathf.Round((1f - movementSpeedMultiplier) * 100f), "%");
            _movementBar.SetValue(movementSpeedMultiplier, movementText, Color.clear);

            string totalWord = TranslationReader.Get("InventoryGUI", 14, "total");

            StringBuilder itemSb = StringBuilderPool.Shared.Rent();
            string val;
            if (TranslationReader.TryGet("InventoryGUI", 12, out val))
            {
                itemSb.Append("<size=11><color=white>");

                for (int i = 0; Enum.IsDefined(typeof(ItemCategory), (ItemCategory)i); i++)
                {
                    ItemCategory category = (ItemCategory)i;
                    string categoryLabel = TranslationReader.Get("Categories", i, category.ToString());

                    sbyte limit = InventoryLimits.GetCategoryLimit(armor, category);

                    if (limit != 0)
                    {
                        AddRecord(itemSb, limit, categoryLabel, limit, totalWord);
                    }
                }
                itemSb.Append("</color></size>");
            }
            _itemLimits.text = itemSb.ToString();
            StringBuilderPool.Shared.Return(itemSb);

            StringBuilder ammoSb = StringBuilderPool.Shared.Rent();
            if (TranslationReader.TryGet("InventoryGUI", 13, out val))
            {
                ammoSb.Append("<size=11><color=white>");

                foreach (ItemType ammoType in InventoryLimits.StandardAmmoLimits.Keys)
                {
                    ushort ammoLimit = InventoryLimits.GetAmmoLimit(armor, ammoType);
                    string ammoLabel = ammoType.ToString();

                    AddRecord(ammoSb, (int)ammoLimit, ammoLabel, (int)ammoLimit, totalWord);
                }
                ammoSb.Append("</color></size>");
            }
            _ammoLimits.text = ammoSb.ToString();
            StringBuilderPool.Shared.Return(ammoSb);
        }

        private void AddRecord(StringBuilder sb, int relativeLimit, string label, int total, string totalWord)
        {
            sb.Append("\n+");
            sb.Append(relativeLimit);
            sb.Append(" <color=white>");
            sb.Append(label);
            sb.Append("</color> (");
            sb.AppendFormat("{0} {1}", total, totalWord);
            sb.Append(")");
        }
    }
}
