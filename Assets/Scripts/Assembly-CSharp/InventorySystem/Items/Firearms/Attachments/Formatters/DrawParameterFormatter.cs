using UnityEngine;

namespace InventorySystem.Items.Firearms.Attachments.Formatters
{
    public class DrawParameterFormatter : IAttachmentsParameterFormatter
    {
        public float DefaultValue => 0f;

        public bool FormatParameter(AttachmentParam param, Firearm firearm, int attId,
            float val, out string formattedText, out bool isGood)
        {
            formattedText = null;
            isGood = false;

            int paramInt = (int)param;

            if (paramInt != 10 && paramInt != 13)
                return false;

            float currentTotal = AttachmentsUtils.AttachmentsValue(firearm, param);

            AttachmentParam other = paramInt == 10
                ? (AttachmentParam)13
                : (AttachmentParam)10;

            float otherTotal = AttachmentsUtils.AttachmentsValue(firearm, other);
            float displayValue = currentTotal >= otherTotal ? currentTotal : otherTotal;

            float delta = displayValue - 1f;
            float percent = delta * 100f;

            string sign = delta >= 0f ? "+" : "-";
            int rounded = Mathf.RoundToInt(Mathf.Abs(percent));
            formattedText = string.Concat(sign, rounded, "%");

            isGood = delta < 0f;
            return true;
        }
    }
}