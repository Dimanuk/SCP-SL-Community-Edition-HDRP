using System;
using RemoteAdmin.Generic;

namespace RemoteAdmin.Settings
{
    [Serializable]
    public class TooltipShowDelaySetting : TooltipSetting
    {
        public override string Path => "ra_tooltip_show";

        public override float DefaultValue
        {
            get
            {
                return 0.5f;
            }
        }

        protected override void ChangeTooltip(float value)
        {
            if (TooltipManager != null)
                TooltipManager.ShowDelay = value;
        }
    }
}