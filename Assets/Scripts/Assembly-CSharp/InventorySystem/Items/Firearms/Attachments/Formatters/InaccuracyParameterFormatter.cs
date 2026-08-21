using UnityEngine;

namespace InventorySystem.Items.Firearms.Attachments.Formatters
{
    public class InaccuracyParameterFormatter : IAttachmentsParameterFormatter
    {
        public float DefaultValue => 1f;

        public bool FormatParameter(AttachmentParam param, Firearm firearm, int attId,
            float val, out string formattedText, out bool isGood)
        {
            float delta = val - 1f;
            float percent = delta * 100f;

            string sign = delta >= 0f ? "+" : "-";
            formattedText = string.Concat(sign, Mathf.Abs(percent).ToString("0"), "%");
            isGood = delta < 0f;
            return true;
        }
    }
}