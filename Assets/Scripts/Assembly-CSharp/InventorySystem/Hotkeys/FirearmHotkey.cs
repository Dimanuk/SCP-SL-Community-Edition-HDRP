using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using PlayerRoles;

namespace InventorySystem.Hotkeys
{
    public class FirearmHotkey : IHotkeyItemSelector
    {
        private readonly bool _isPrimary;

        public ActionName KeyActionName => _isPrimary ? ActionName.HotkeyPrimaryFirearm : ActionName.HotkeySecondaryFirearm;

        public FirearmHotkey(bool primary)
        {
            _isPrimary = primary;
        }

        public ushort GetCorrespondingItemSerial(ReferenceHub ply, ushort[] itemsOrder, bool smartFeatureEnabled)
        {
            if (itemsOrder == null || ply == null)
                return 0;

            var inventory = ply.inventory;
            if (inventory == null)
                return 0;

            var userInventory = inventory.UserInventory;
            if (userInventory == null)
                return 0;

            var items = userInventory.Items;
            if (items == null)
                return 0;

            bool foundFirst = false;

            for (int i = 0; i < itemsOrder.Length; i++)
            {
                ushort itemSerial = itemsOrder[i];

                if (!items.TryGetValue(itemSerial, out ItemBase item) || item == null)
                    continue;

                if (!(item is Firearm))
                    continue;

                if (_isPrimary)
                    return itemSerial;

                if (!foundFirst)
                {
                    foundFirst = true;
                    continue;
                }

                return itemSerial;
            }

            return 0;
        }
    }
}
