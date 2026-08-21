using System;
using RemoteAdmin.Generic;
using Tooltips;

namespace RemoteAdmin.Settings
{
    [Serializable]
    public class ToggleTooltipSetting : ToggleableSetting
    {
        public override bool DefaultValue { get; } = true;
        public override string Path { get; } = "ra_tooltip_enabled";

        public override bool Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                ChangeTooltip(value);
            }
        }

        [UnityEngine.SerializeField]
        private TooltipManager _tooltipManager;

        public TooltipManager TooltipManager
        {
            get => _tooltipManager;
            set => _tooltipManager = value;
        }

        private void ChangeTooltip(bool value)
        {
            if (TooltipManager != null)
                TooltipManager.IsEnabled = value;
        }
    }
}