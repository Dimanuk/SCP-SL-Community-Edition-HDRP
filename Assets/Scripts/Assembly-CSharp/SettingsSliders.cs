using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsSliders : MonoBehaviour
{
    [Serializable]
    public class SliderSettingOption
    {
        public Slider Slider;
        public TextMeshProUGUI NumberIndicator;
        public SettingsOption SettingOption;  

        public GameObject UIelementGameObject => Slider != null ? Slider.gameObject : null;
    }

    [SerializeField] private SliderSettingOption[] sliders;

    private void Start()
    {
        foreach (SliderSettingOption option in sliders)
        {
            int savedValue = PlayerPrefsSl.Get(option.SettingOption.ToString(), 0);
            option.Slider.value = savedValue;

            if (option.NumberIndicator != null)
            {
                option.NumberIndicator.text = savedValue.ToString();
            }

            option.Slider.onValueChanged.AddListener((value) => SliderValueChanged(option));
        }
    }

    private void SliderValueChanged(SliderSettingOption option)
    {
        if (option == null || option.Slider == null)
            return;


        int newValue = (int)option.Slider.value;

        PlayerPrefsSl.Set(option.SettingOption.ToString(), newValue);

        if (option.NumberIndicator != null)
        {
            option.NumberIndicator.text = newValue.ToString();
        }
    }

    private void ApplyGraphicSetting(SliderSettingOption option)
    {
      
    }
}