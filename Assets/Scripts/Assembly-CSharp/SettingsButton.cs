using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    [Serializable]
    public class ButtonSettingOption : BaseSettingsOption
    {
        public Button Button;
        public bool DefaultValue;

        public bool CurrentValue { get; set; }

        protected override GameObject UIelementGameObject
        {
            get => Button != null ? Button.gameObject : null;
        }
    }

    public ButtonSettingOption[] buttons;

    public Sprite enableState;
    public Sprite disableState;

    public NewFPSLimiter fpsLimiter;

    private List<BaseSettingsOption> _shadowDependants = new List<BaseSettingsOption>();
    private List<BaseSettingsOption> _lightDependants = new List<BaseSettingsOption>();

    private bool _shadowsEnabled = true;
    private bool _lightsEnabled;

    public GameObject[] Tabs;

    private void Start()
    {
        ApplyNonGUISettings();

        CollectDependencies();
        for (int i = 0; i < buttons.Length; i++)
        {
            ButtonSettingOption option = buttons[i];
            if (option == null || option.Button == null)
                continue;

            var display = new DisplayClass
            {
                This = this,
                ButtonSettingOption = option
            };

            string key = option.SettingOption.ToString();
            bool savedValue = PlayerPrefsSl.Get(key, option.DefaultValue);
            option.CurrentValue = savedValue;

            UpdateButtonVisual(option);

            option.Button.onClick.AddListener(display.OnClick);

            ApplyGraphicSetting(option);
        }

        UpdateToggles();
    }

    private void CollectDependencies()
    {
        _lightDependants.Clear();
        _shadowDependants.Clear();
    }

    private void ToggleValueChanged(ButtonSettingOption option)
    {
        if (option == null) return;

        option.CurrentValue = !option.CurrentValue;

        UpdateButtonVisual(option);

        string key = option.SettingOption.ToString();
        PlayerPrefsSl.Set(key, option.CurrentValue);

        ApplyGraphicSetting(option);
        UpdateToggles();
    }

    private void UpdateButtonVisual(ButtonSettingOption option)
    {
        if (option?.Button == null) return;

        Image image = option.Button.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = option.CurrentValue ? enableState : disableState;
        }
    }

    private void ApplyGraphicSetting(ButtonSettingOption option)
    {
        if (option == null) return;

        bool value = option.CurrentValue;
        string settingKey = option.SettingOption.ToString() ?? "";

        switch (settingKey)
        {
            case "VSync":
                QualitySettings.vSyncCount = value ? 1 : 0;
                if (fpsLimiter != null)
                    fpsLimiter.SetEnable(!value);
                break;

            case "Shadows":
                _shadowsEnabled = value;
                QualitySettings.shadows = value ? ShadowQuality.All : ShadowQuality.Disable;
                UpdateToggles();
                break;

            case "RichPresence":
                if (value)
                {
                    SteamManager.ChangePreset(-2);
                    SteamManager.ChangeLobbyStatus(0, 20);
                }
                else
                {
                    SteamManager.ClearRichPresence();
                }
                break;

            case "MaintainProportions":
                MaintainPropotionsToggle component = option.Button.GetComponent<MaintainPropotionsToggle>();
                if (component != null) component.SetValue(value);
                break;

        }
    }

    private void UpdateToggles()
    {
        foreach (var dep in _lightDependants)
        {
            if (dep.GetRootObject != null)
                dep.GetRootObject.SetActive(_lightsEnabled);
        }

        foreach (var dep in _shadowDependants)
        {
            if (dep.GetRootObject != null)
                dep.GetRootObject.SetActive(_shadowsEnabled);
        }
    }

    public void ChangeTab(int id)
    {
        if (Tabs == null) return;

        for (int i = 0; i < Tabs.Length; i++)
        {
            if (Tabs[i] != null)
                Tabs[i].SetActive(i == id);
        }
    }

    private void ApplyNonGUISettings()
    {
        int pixelLightCount = PlayerPrefsSl.Get("gfxsets_pxlc", 4);
        pixelLightCount = Mathf.Clamp(pixelLightCount, 0, 12);
        QualitySettings.pixelLightCount = pixelLightCount;
    }

    private sealed class DisplayClass
    {
        public SettingsButton This;
        public ButtonSettingOption ButtonSettingOption;

        public void OnClick()
        {
            This?.ToggleValueChanged(ButtonSettingOption);
        }
    }
}