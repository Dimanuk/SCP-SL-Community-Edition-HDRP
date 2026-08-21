using System;
using Tooltips;
using UnityEngine;

namespace RemoteAdmin.Generic
{
    [Serializable]
    public abstract class TooltipSetting : CustomSliderSetting
    {
        [SerializeField]
        private TooltipManager _tooltipManager;

        public override float Value
        {
            get
            {
                CustomSlider slider = base.RepresentingSlider;
                if (slider == null)
                    return base.Value;

                return slider.value;
            }
            set
            {
                base.Value = value;
                OnSave();
                ChangeTooltip(value);
            }
        }

        public TooltipManager TooltipManager
        {
            get => _tooltipManager;
            set => _tooltipManager = value;
        }

        protected abstract void ChangeTooltip(float value);
    }
}