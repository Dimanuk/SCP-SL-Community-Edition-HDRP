using System;
using RemoteAdmin.Generic;

namespace RemoteAdmin.Settings
{
    [Serializable]
    public class BandwidthCooldownSetting : CustomSliderSetting
    {
        public override string Path => "ra_bandwidth_cooldown";

        public override float DefaultValue
        {
            get
            {
                return 1.0f;
            }
        }
    }
}