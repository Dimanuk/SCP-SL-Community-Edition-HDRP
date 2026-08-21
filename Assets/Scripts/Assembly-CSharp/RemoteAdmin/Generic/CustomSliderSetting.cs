using System;
using UnityEngine;

namespace RemoteAdmin.Generic
{
    [Serializable]
    public abstract class CustomSliderSetting : RaSetting<float>
    {
        [SerializeField]
        private CustomSlider _representingSlider;

        public override float Value
        {
            get
            {
                CustomSlider slider = RepresentingSlider;
                if (slider == null || slider.IsDestroyed())
                    return base.Value;

                return slider.value;
            }
            set
            {
                base.Value = value;
                OnUpdateSlider(value);
            }
        }


        public CustomSlider RepresentingSlider
        {
            get => _representingSlider;
            set => _representingSlider = value;
        }

        public void SliderToValue()
        {
            CustomSlider slider = RepresentingSlider;
            if (slider == null || slider.IsDestroyed())
                throw new NullReferenceException();

            Value = slider.value;
        }

        public void Refresh()
        {
            OnUpdateSlider(Value);
        }

        protected override void OnSave()
        {
            PlayerPrefsSl.Set(Path, Value);
        }

        protected override void OnLoad()
        {
            Value = PlayerPrefsSl.Get(Path, DefaultValue);
        }

        protected virtual void OnUpdateSlider(float value)
        {
            CustomSlider slider = RepresentingSlider;
            if (slider == null || slider.IsDestroyed())
                return; 

            if (slider.value != value)
                slider.value = value;

            slider.SetText(value);
        }
    }
}