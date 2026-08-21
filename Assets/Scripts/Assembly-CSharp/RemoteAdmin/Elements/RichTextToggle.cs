using RemoteAdmin.Presets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin.Elements
{
    public class RichTextToggle : CustomButton
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private ToggleColorPreset _colorPreset;

        [SerializeField]
        private string _configKey;

        public override void SetState(bool isSelected)
        {
            base.SetState(isSelected);

            if (Text != null)
                Text.richText = isSelected;

            if (_button != null && _colorPreset != null)
                _button.colors = isSelected ? _colorPreset.Selected : _colorPreset.Unselected;

            if (!string.IsNullOrEmpty(_configKey))
                PlayerPrefsSl.Set(_configKey, isSelected);
        }

        public void Toggle()
        {
            SetState(!IsSelected);
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(_configKey))
                return;

            bool saved = PlayerPrefsSl.Get(_configKey, false);
            SetState(saved);
        }
    }
}