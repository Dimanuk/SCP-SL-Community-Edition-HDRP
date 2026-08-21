using System;
using RemoteAdmin.Generic;
using UnityEngine;

namespace RemoteAdmin.Settings
{
    [Serializable]
    public class BlueWindowSetting : ColorSliderSetting
    {
        public override string Path => "ra_color_b";

        protected override Color CreateColor(Color oldColor, float value)
        {
            return new Color(oldColor.r, oldColor.g, value, oldColor.a);
        }
    }
}