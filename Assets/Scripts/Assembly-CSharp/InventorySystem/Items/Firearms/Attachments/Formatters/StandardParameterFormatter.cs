using UnityEngine;

namespace InventorySystem.Items.Firearms.Attachments.Formatters
{
    public class StandardParameterFormatter : IAttachmentsParameterFormatter
    {
        private readonly bool _isMultiplier;
        private readonly bool _moreIsBetter;
        private readonly bool _formatAsPercent;
        private readonly string _suffix;

        public float DefaultValue => _isMultiplier ? 1f : 0f;

        public StandardParameterFormatter(
            bool moreIsBetter,
            bool isMultiplier = true,
            bool formatAsPercent = true,
            string suffix = null)
        {
            _suffix = suffix;
            _isMultiplier = isMultiplier;
            _moreIsBetter = moreIsBetter;
            _formatAsPercent = formatAsPercent;
        }

        public bool FormatParameter(AttachmentParam param, Firearm firearm, int attId,
            float statsValue, out string formattedText, out bool isGood)
        {
            formattedText = null;
            isGood = false;

            var attachments = firearm.Attachments;

            AttachmentName targetName = attachments[attId].Name;
            int baseIdx = -1;
            for (int i = 0; i < attachments.Length; i++)
            {
                if (attachments[i].Name == targetName)
                {
                    baseIdx = i;
                    break;
                }
            }

            if (baseIdx < 0) return false;

            if (!attachments[baseIdx].TryGetValue(param, out float baseValue))
                baseValue = DefaultValue;

            if (!attachments[attId].TryGetValue(param, out float attValue))
                return false;

            float delta = _isMultiplier ? (attValue / baseValue) - 1f : attValue - baseValue;

            if (Mathf.Approximately(delta, 0f)) return false;

            string sign = delta > 0f ? "+" : "-";
            float abs = Mathf.Abs(delta);

            if (_formatAsPercent)
            {
                int rounded = Mathf.RoundToInt(abs * 100f);
                formattedText = string.Concat(sign, rounded, "%");
            }
            else
            {
                float rounded = Mathf.Round(abs * 10f) / 10f;
                formattedText = string.Concat(sign, rounded.ToString("0.0"));
            }

            if (!string.IsNullOrEmpty(_suffix))
                formattedText = string.Concat(formattedText, _suffix);

            isGood = (delta > 0f) == _moreIsBetter;
            return true;
        }
    }
}