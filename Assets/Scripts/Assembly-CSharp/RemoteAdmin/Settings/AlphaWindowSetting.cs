using System;
using RemoteAdmin.Generic;
using UnityEngine;

namespace RemoteAdmin.Settings
{
    [Serializable]
    public class AlphaWindowSetting : ColorSliderSetting
    {
        public override string Path => "ra_color_a";

        protected override Color CreateColor(Color oldColor, float value)
        {
            return new Color(oldColor.r, oldColor.g, oldColor.b, value);
        }
    }
}