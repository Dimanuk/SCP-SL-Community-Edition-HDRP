using System;
using RemoteAdmin.Generic;
using UnityEngine;

namespace RemoteAdmin.Settings
{
    [Serializable]
    public class RedWindowSetting : ColorSliderSetting
    {
        public override string Path => "ra_color_r";

        protected override Color CreateColor(Color oldColor, float value)
        {
            return new Color(value, oldColor.g, oldColor.b, oldColor.a);
        }
    }
}