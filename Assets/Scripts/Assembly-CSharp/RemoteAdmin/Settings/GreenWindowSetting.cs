using System;
using RemoteAdmin.Generic;
using UnityEngine;

namespace RemoteAdmin.Settings
{
    [Serializable]
    public class GreenWindowSetting : ColorSliderSetting
    {
        public override string Path => "ra_color_g";

        protected override Color CreateColor(Color oldColor, float value)
        {
            return new Color(oldColor.r, value, oldColor.b, oldColor.a);
        }
    }
}