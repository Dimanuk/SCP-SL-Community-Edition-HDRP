using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
    public class CustomSlider : Slider
    {
        public TMP_Text ValueDisplay;

        [SerializeField]
        private string _valueFormat = "000.00";

        protected override void Awake()
        {
            base.Awake();
            onValueChanged.AddListener(value => ChangeValue(value));
        }

        protected void ChangeValue(float newValue)
        {
            OnValueChanged(newValue);
            SetText(newValue);
        }

        protected virtual void OnValueChanged(float newValue)
        {
        }

        public virtual void SetText(float newValue)
        {
            if (ValueDisplay == null)
                return;

            ValueDisplay.text = newValue.ToString(_valueFormat);
        }
    }
}