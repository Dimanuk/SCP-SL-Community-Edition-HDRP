using System;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Attachments.Formatters
{
    public static class NonParameterFormatter
    {
        private const int FlagsIteration = 8;

        private static string WeightString =>
            "\n" + TranslationReader.Get("InventoryGUI", 5, "NO_TRANSLATION") + ": ";

        private static string LengthString =>
             "\n" + TranslationReader.Get("InventoryGUI", 6, "NO_TRANSLATION") + ": ";

        public static void Format(Firearm fa, int attachmentId,
            out string pros, out string cons)
        {
            pros = string.Empty;
            cons = string.Empty;

            if (attachmentId < 0 || attachmentId >= fa.Attachments.Length)
                return;

            var attachments = fa.Attachments;
            var attachment = attachments[attachmentId];

            string prosFlags = FormatFlags(
                (int)attachment.DescriptivePros,
                typeof(AttachmentDescriptiveAdvantages),
                "AttachmentDescriptiveAdvantages");
            pros = string.Concat(pros, prosFlags);

            string consFlags = FormatFlags(
                (int)attachment.DescriptiveCons,
                typeof(AttachmentDescriptiveDownsides),
                "AttachmentDescriptiveDownsides");
            cons = string.Concat(cons, consFlags);

            AttachmentsUtils.GetDefaultLengthAndWeight(
                fa,
                out float defaultWeight,
                out float defaultLength);

            float weight = attachment.Weight;
            float length = attachment.Length;

            float weightRatio = defaultWeight > 0f ? weight / defaultWeight : 1f;
            float lengthRatio = defaultLength > 0f ? length / defaultLength : 1f;

            if (weightRatio > 1f)
                cons = string.Concat(cons, WeightString, FormatPercent(weightRatio - 1f));
            else if (weightRatio < 1f)
                pros = string.Concat(pros, WeightString, FormatPercent(1f - weightRatio));

            if (lengthRatio > 1f)
                cons = string.Concat(cons, LengthString, FormatPercent(lengthRatio - 1f));
            else if (lengthRatio < 1f)
                pros = string.Concat(pros, LengthString, FormatPercent(1f - lengthRatio));
        }
        private static string FormatPercent(float percent)
        {
            string sign = percent < 0f ? "-" : "+";
            float absVal = Mathf.Abs(percent) * 100f;
            int rounded = Mathf.RoundToInt(absVal);
            return string.Concat(sign, rounded, "%");
        }

        private static string FormatFlags(int value, Type enumType, string translationKey)
        {
            int bit = 1;
            int index = 0;
            string result = string.Empty;

            for (int i = 0; i < FlagsIteration; i++, bit <<= 1, index++)
            {
                if (!Enum.IsDefined(enumType, index)) continue;
                if ((value & bit) == 0) continue;

                string translation = TranslationReader.Get(
                    translationKey, index, "NO_FLAGS_TRANSLATION");
                result = string.Concat(result, "\n", translation);
            }

            return result;
        }
    }
}