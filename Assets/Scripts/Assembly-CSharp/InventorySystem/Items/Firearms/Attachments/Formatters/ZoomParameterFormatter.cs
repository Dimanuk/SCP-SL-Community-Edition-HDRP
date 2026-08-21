using InventorySystem.Items.Firearms.Attachments.Components;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Attachments.Formatters
{
    public class ZoomParameterFormatter : IAttachmentsParameterFormatter
    {
        public float DefaultValue => 1f;

        public bool FormatParameter(AttachmentParam param, Firearm firearm, int attId,
            float val, out string formattedText, out bool isGood)
        {
            Attachment attachment = firearm.Attachments[attId];

            bool hasAds = attachment.TryGetValue(AttachmentParam.AdsZoomMultiplier, out _);
            bool hasZoom = attachment.TryGetValue(AttachmentParam.AdsMouseSensitivityMultiplier, out _);

            if (!hasAds && !hasZoom)
            {
                formattedText = null;
                isGood = false;
                return false;
            }

            float totalAds = AttachmentsUtils.AttachmentsValue(firearm, AttachmentParam.AdsZoomMultiplier);
            float totalZoom = AttachmentsUtils.AttachmentsValue(firearm, AttachmentParam.AdsMouseSensitivityMultiplier);

            float displayValue = Mathf.Round(totalAds * totalZoom * 100f) / 100f;
            formattedText = $"{displayValue}x";
            isGood = true;
            return true;
        }
    }
}