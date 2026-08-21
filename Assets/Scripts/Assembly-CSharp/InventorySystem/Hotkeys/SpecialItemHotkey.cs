using InventorySystem.Items;
using System.Collections.Generic;

namespace InventorySystem.Hotkeys
{
    public class SpecialItemHotkey : IHotkeyItemSelector
    {
        private readonly ItemType[] _bestToWorstItems;

        private readonly Dictionary<ItemType, int> _itemIndexes;

        private readonly ItemType[] _medicalItemTypes = new ItemType[8];
        private readonly ushort[] _medicalItemSerials = new ushort[8];

        private ItemType _lastMatch = ItemType.None;
        private ushort _bestSerial;
        private int _medicalItemsCount;

        public ActionName KeyActionName { get; private set; }

        public SpecialItemHotkey(ActionName actionName, params ItemType[] bestToWorstItems)
        {
            KeyActionName = actionName;
            _bestToWorstItems = bestToWorstItems;

            _itemIndexes = new Dictionary<ItemType, int>();
            for (int i = 0; i < _bestToWorstItems.Length; i++)
                _itemIndexes[_bestToWorstItems[i]] = i;

            Inventory.OnItemsModified += RefreshItems;
            Inventory.OnCurrentItemChanged += (ply, id1, id2) => RefreshHotkey(ply);
        }

        private void RefreshItems(ReferenceHub ply)
        {
            _medicalItemsCount = 0;
            foreach (var kvp in ply.inventory.UserInventory.Items)
            {
                ItemBase item = kvp.Value;
                if (!_itemIndexes.ContainsKey(item.ItemTypeId)) continue;
                if (_medicalItemsCount >= _medicalItemTypes.Length) break;

                _medicalItemTypes[_medicalItemsCount] = item.ItemTypeId;
                _medicalItemSerials[_medicalItemsCount] = kvp.Key;
                _medicalItemsCount++;
            }

            RefreshHotkey(ply);
        }

        private ushort HasItem(ItemType type)
        {
            for (int i = 0; i < _medicalItemsCount; i++)
            {
                if (_medicalItemTypes[i] == type)
                    return _medicalItemSerials[i];
            }
            return 0;
        }

        private void RefreshHotkey(ReferenceHub ply)
        {
            if (!ply.isLocalPlayer) return;

            _bestSerial = 0;

            ItemType curType = ply.inventory.CurItem.TypeId;
            int startIdx = 0;   

            if (_itemIndexes.TryGetValue(curType, out int curRankIdx))
            {
                // Holding a matching item: start at the NEXT rank so the hotkey cycles.
                startIdx = curRankIdx + 1;
                _lastMatch = curType;
            }
            else if (_itemIndexes.TryGetValue(_lastMatch, out int _))
            {
                ushort lastSerial = HasItem(_lastMatch);
                if (lastSerial != 0)
                {
                    _bestSerial = lastSerial;
                    return;
                }

                // Last used item is gone: restart the search from the best rank.
                _lastMatch = ItemType.None;
            }

            int len = _bestToWorstItems.Length;
            for (int i = 0; i < len; i++)
            {
                int idx = (startIdx + i) % len;
                ItemType t = _bestToWorstItems[idx];
                ushort serial = HasItem(t);

                _bestSerial = serial;
                if (serial != 0) return;
            }
        }

        public ushort GetCorrespondingItemSerial(ReferenceHub ply, ushort[] itemsOrder, bool smartFeatureEnabled)
        {
            return _bestSerial;
        }
    }
}