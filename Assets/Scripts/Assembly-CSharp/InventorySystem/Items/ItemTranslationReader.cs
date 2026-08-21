using System.Collections.Generic;

namespace InventorySystem.Items
{
    public class ItemTranslationReader
    {
        private const char SplitChar = '~';

        private static readonly Dictionary<int, ItemTranslationReader> CachedTranslations;
        private static bool _eventAlreadyAssigned;

        public readonly string Name;
        public readonly string Description;

        public ItemTranslationReader(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public ItemTranslationReader(ItemType itemTypeId)
        {
            // The "Items" translation file is keyed by an explicit LEADING ItemType number
            // (e.g. "40~AK~...") and the numbers have GAPS (…18, 20, 23, 24, 25, 30…). The lookup
            // MUST index by that parsed number, NOT by line position — otherwise ItemType 40 (AK)
            // reads line 40 which is "45~SCP-244-B", etc. So build the cache once, keyed by parts[0].
            if (CachedTranslations.Count == 0)
            {
                string[] lines = TranslationReader.GetKeys("Items");
                if (lines != null)
                {
                    foreach (string rawLine in lines)
                    {
                        if (string.IsNullOrWhiteSpace(rawLine))
                            continue;

                        string[] parts = rawLine.Split(SplitChar);
                        if (parts.Length >= 2 && int.TryParse(parts[0], out int itemId))
                        {
                            string name = parts[1];
                            string description = parts.Length > 2 ? parts[2] : string.Empty;
                            CachedTranslations[itemId] = new ItemTranslationReader(name, description);
                        }
                    }
                }
            }

            if (CachedTranslations.TryGetValue((int)itemTypeId, out var cached))
            {
                Name = cached.Name;
                Description = cached.Description;
                return;
            }

            Name = itemTypeId.ToString();
            Description = string.Empty;
        }

        static ItemTranslationReader()
        {
            CachedTranslations = new Dictionary<int, ItemTranslationReader>();

            if (!_eventAlreadyAssigned)
            {
                TranslationReader.OnTranslationsRefreshed += ClearCache;
                _eventAlreadyAssigned = true;
            }
        }

        private static void ClearCache()
        {
            CachedTranslations.Clear();
        }

        public static string GetName(ItemType itemType)
        {
            return new ItemTranslationReader(itemType).Name;
        }

        public static string GetDescription(ItemType itemType)
        {
            return new ItemTranslationReader(itemType).Description;
        }
    }
}