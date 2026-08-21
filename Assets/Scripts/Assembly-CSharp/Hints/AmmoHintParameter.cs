using Mirror;
using System;
using InventorySystem;
using InventorySystem.Items;
using System.Collections.Generic;

namespace Hints
{
    public class AmmoHintParameter : IdHintParameter
    {
        public static AmmoHintParameter FromNetwork(NetworkReader reader)
        {
            AmmoHintParameter ammoHintParameter = new AmmoHintParameter();
            ammoHintParameter.Deserialize(reader);

            if (ammoHintParameter == null) throw new NullReferenceException();

            return ammoHintParameter;
        }

        public AmmoHintParameter() : base()
        {
        }

        public AmmoHintParameter(byte id) : base()
        {
            this.Id = id;
        }

        protected override string FormatId(float progress, out bool stopFormatting)
        {
            stopFormatting = false;
            byte currentId = (byte)this.Id;

            var availableItems = InventoryItemLoader.AvailableItems;

            if (availableItems.TryGetValue((ItemType)currentId, out ItemBase item))
            {
                return item.ToString();
            }

            return currentId.ToString();
        }
    }
}