using System;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin.Generic
{
    [Serializable]
    public abstract class ColorSliderSetting : CustomSliderSetting
    {
        [SerializeField]
        private RawImage[] _raUIElements;

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

        // Sliders are 0–100 (whole numbers); the default is full (100), matching the
        // scene sliders' m_Value: 100. A default of 1 would collapse every channel to ~0.01.
        public override float DefaultValue { get; } = 100f;

        public RawImage[] RaUIElements
        {
            get => _raUIElements;
            set => _raUIElements = value;
        }

        protected override void OnSave()
        {
            PlayerPrefsSl.Set(Path, Value);
        }

        protected override void OnUpdateSlider(float value)
        {
            CustomSlider slider = RepresentingSlider;
            if (slider == null || slider.IsDestroyed())
                return; 

            if (slider.value != value)
                slider.value = value;

            slider.SetText(value);

            float normalized = value * 0.01f;
            if (_raUIElements == null)
                return; 

            for (int i = 0; i < _raUIElements.Length; i++)
            {
                RawImage image = _raUIElements[i];
                if (image == null)
                    continue; 

                image.color = CreateColor(image.color, normalized);
            }
        }

        protected virtual Color CreateColor(Color oldColor, float value)
        {
            return new Color(value, oldColor.g, oldColor.b, oldColor.a);
        }
    }
}