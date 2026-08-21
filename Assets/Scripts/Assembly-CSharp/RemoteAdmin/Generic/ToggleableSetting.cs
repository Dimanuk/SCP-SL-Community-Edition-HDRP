using System;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin.Generic
{
    [Serializable]
    public abstract class ToggleableSetting : RaSetting<bool>
    {
        [SerializeField]
        private Button _representingButton;

        public override bool Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                SetButton(value);
            }
        }

        public Button RepresentingButton
        {
            get => _representingButton;
            set => _representingButton = value;
        }

        public void Toggle()
        {
            Value = !Value;
        }

        protected override void OnSave()
        {
            PlayerPrefsSl.Set(Path, Value);
        }

        protected override void OnLoad()
        {
            Value = PlayerPrefsSl.Get(Path, DefaultValue);
        }

        protected virtual void SetButton(bool value)
        {
            if (RepresentingButton == null)
                return;

            SubmenuSelector selector = SubmenuSelector.Singleton;
            if (selector == null)
                return;

            RepresentingButton.colors = value ? selector.ToggleOn : selector.ToggleOff;
        }
    }
}