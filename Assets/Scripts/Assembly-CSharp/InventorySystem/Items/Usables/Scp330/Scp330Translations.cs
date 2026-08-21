using System;

namespace InventorySystem.Items.Usables.Scp330
{
    public static class Scp330Translations
    {
        public enum Entry
        {
            Candies = 0
        }

        private const int CandiesOffset = 1;

        public static string GetSpecificTranslation(int index, string fallback)
        {
            return TranslationReader.Get("SCP330", index, fallback);
        }

        public static string GetEntryTranslation(Entry entry)
        {
            return TranslationReader.Get("SCP330", (int)entry, null);
        }

        public static void GetCandyTranslation(CandyKindID candyKind, out string name, out string desc, out string fx)
        {
            int index = (int)candyKind * 3;
            
            name = TranslationReader.Get("SCP330", index - 2, null) ?? "Unknown Candy";
            desc = TranslationReader.Get("SCP330", index - 1, null) ?? "No Description";
            fx = TranslationReader.Get("SCP330", index, null) ?? "Unknown effects";
        }
    }
}