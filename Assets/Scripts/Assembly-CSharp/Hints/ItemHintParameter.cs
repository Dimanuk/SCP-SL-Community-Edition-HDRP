using Mirror;
using System;
using InventorySystem;
using InventorySystem.Items;

namespace Hints
{
    public class ItemHintParameter : IdHintParameter
    {
        public static ItemHintParameter FromNetwork(NetworkReader reader)
        {
            ItemHintParameter itemHintParameter = new ItemHintParameter();
            itemHintParameter.Deserialize(reader);
            return itemHintParameter;
        }

        private ItemHintParameter() : base()
        {
        }

        public ItemHintParameter(ItemType item) : base((byte)item)
        {
            if (item == ItemType.None)
            {
                throw new ArgumentException("Item cannot be none (no proper translation).", nameof(item));
            }
        }

        protected override string FormatId(float progress, out bool stopFormatting)
        {
            stopFormatting = true;

            ItemType itemType = (ItemType)base.Id;

            if (InventoryItemLoader.AvailableItems.TryGetValue(itemType, out ItemBase itemBase))
            {
                return itemBase.name;
            }

            return itemType.ToString();
        }
    }
}