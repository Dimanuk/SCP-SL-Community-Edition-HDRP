using TMPro;
using UnityEngine;

namespace RemoteAdmin.Elements
{
    public class RaEffectButton : SendButton
    {
        private const string DefaultIntensity = "1";
        private const string DefaultDuration = "0";
        private const string ClearValue = "0";

        [SerializeField]
        private TMP_InputField _durationField;

        [SerializeField]
        private TMP_InputField _intensityField;

        [SerializeField]
        private SendButton _clearButton;

        public string EffectId { get; set; }
        public string EffectName { get; set; }

        public TMP_InputField InputFieldA => _intensityField;
        public TMP_InputField InputFieldB => _durationField;

        public string Duration
        {
            get
            {
                if (_durationField == null || string.IsNullOrEmpty(_durationField.text))
                    return DefaultDuration;
                return _durationField.text;
            }
        }

        public string Intensity
        {
            get
            {
                if (_intensityField == null || string.IsNullOrEmpty(_intensityField.text))
                    return DefaultIntensity;
                return _intensityField.text;
            }
        }

        public void Setup()
        {
            if (Text != null)
                Text.text = EffectName;

            if (_clearButton == null)
                return;

            _clearButton.CustomCommand = string.Concat(CustomCommand, " ", EffectId, " 0 0");
            _clearButton.CommandMenu = CommandMenu;
        }

        public void ResetFields()
        {
            if (_durationField != null)
                _durationField.text = string.Empty;
            if (_intensityField != null)
                _intensityField.text = string.Empty;
        }

        protected override void SendCommand(string command, string format)
        {
            string fullCommand = string.Concat(
                CustomCommand, " ", EffectId, " ", Intensity, " ", Duration);

            // The menu's BuildCommand appends the selected players — the server's
            // effect command requires them as the fourth argument.
            base.SendCommand(fullCommand, format);
        }
    }
}