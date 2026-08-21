using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SettingsDropdown : MonoBehaviour
{
    [Serializable]
    public class DropdownSettingOption : BaseSettingsOption
    {
        public TMP_Dropdown DropdownMenu;
        public int DefaultValue;

        protected override GameObject UIelementGameObject
        {
            get => DropdownMenu != null ? DropdownMenu.gameObject : null;
        }
    }

    private readonly float[] _shadowDistances = { 25f, 37f, 50f };

    private bool _initialized;

    public DropdownSettingOption[] dropdowns;

    private void Start()
    {
        if (dropdowns == null || dropdowns.Length == 0)
            return;

        for (int i = 0; i < dropdowns.Length; i++)
        {
            DropdownSettingOption option = dropdowns[i];
            if (option == null || option.DropdownMenu == null)
                continue;

            var display = new DisplayClass
            {
                This = this,
                DropdownOption = option
            };

            string key = option.SettingOption.ToString(); 
            int savedValue = PlayerPrefsSl.Get(key, option.DefaultValue);

            option.DropdownMenu.value = Mathf.Clamp(savedValue, 0, option.DropdownMenu.options.Count - 1);
            option.DropdownMenu.onValueChanged.AddListener(display.OnValueChanged);

            ApplyGraphicSetting(option);
        }

        _initialized = true;
    }

    private void ApplyGraphicSetting(DropdownSettingOption option)
    {
        if (option?.DropdownMenu == null)
            return;

        int value = option.DropdownMenu.value;
        string settingKey = option.SettingOption.ToString();

        switch (settingKey)
        {
            case "QualityLevel":
            case "Quality":
                QualitySettings.SetQualityLevel(value);
                break;

            case "ShadowDistance":
                int shadowIndex = Mathf.Clamp(value, 0, _shadowDistances.Length - 1);
                QualitySettings.shadowDistance = _shadowDistances[shadowIndex];
                break;

            case "ShadowResolution":
                int resIndex = Mathf.Clamp(value, 0, 3);
                QualitySettings.shadowResolution = (ShadowResolution)resIndex;
                break;

            default:
                break;
        }
    }

    public void ShowPopupIfInitialized(GameObject popup)
    {
        if (_initialized && popup != null)
        {
            popup.SetActive(true);
        }
    }

    private sealed class DisplayClass
    {
        public SettingsDropdown This;
        public DropdownSettingOption DropdownOption;

        public void OnValueChanged(int value)
        {
            if (DropdownOption?.DropdownMenu == null)
                return;

            string key = DropdownOption.SettingOption.ToString();

            PlayerPrefsSl.Set(key, DropdownOption.DropdownMenu.value);
            This?.ApplyGraphicSetting(DropdownOption);
        }
    }
}