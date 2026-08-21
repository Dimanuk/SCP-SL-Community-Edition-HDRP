using UnityEngine;
using UnityEngine.UI;

public class GameplayOptionsMenu : MonoBehaviour
{
    public Slider classIntroFastFadeSlider;
    public Slider headBobSlider;
    public Slider toggleSprintSlider;
    public Slider modeSwitchToggle079;
    public Slider postProcessing079;
    public Slider healthBarShowsExact;
    public Slider richPresence;
    public Slider publicLobby;
    public Slider hideIP;
    public Slider toggleSearch;

    private bool _isAwake;

    public void Awake()
    {
        SetSlider(classIntroFastFadeSlider, "ClassIntroFastFade", false);
        SetSlider(headBobSlider, "HeadBob", true);
        SetSlider(toggleSprintSlider, "ToggleSprint", false);
        SetSlider(modeSwitchToggle079, "ModeSwitchSetting079", false);
        SetSlider(postProcessing079, "PostProcessing079", true);
        SetSlider(healthBarShowsExact, "HealthBarShowsExact", true);
        SetSlider(richPresence, "RichPresence", true);
        SetSlider(publicLobby, "PublicLobby", true);
        SetSlider(hideIP, "HideIP", false);
        SetSlider(toggleSearch, "ToggleSearch", false);

        _isAwake = true;
    }

    private void SetSlider(Slider slider, string key, bool defaultValue)
    {
        if (slider != null)
            slider.value = PlayerPrefsSl.Get(key, defaultValue) ? 1f : 0f;
    }

    public void SaveSettings()
    {
        if (!_isAwake)
            return;

        SaveSlider(classIntroFastFadeSlider, "ClassIntroFastFade");
        SaveSlider(headBobSlider, "HeadBob");
        SaveSlider(toggleSprintSlider, "ToggleSprint");
        SaveSlider(modeSwitchToggle079, "ModeSwitchSetting079");
        SaveSlider(postProcessing079, "PostProcessing079");
        SaveSlider(healthBarShowsExact, "HealthBarShowsExact");
        SaveSlider(richPresence, "RichPresence");
        SaveSlider(publicLobby, "PublicLobby");
        SaveSlider(hideIP, "HideIP");
        SaveSlider(toggleSearch, "ToggleSearch");

        if (richPresence != null)
        {
            bool rpEnabled = Mathf.RoundToInt(richPresence.value) == 1;
            if (rpEnabled)
            {
                SteamManager.ChangePreset(-2);
                SteamManager.ChangeLobbyStatus(0, 20);
            }
            else
            {
                SteamManager.ClearRichPresence();
            }
        }
    }

    private void SaveSlider(Slider slider, string key)
    {
        if (slider != null)
            PlayerPrefsSl.Set(key, Mathf.RoundToInt(slider.value) == 1);
    }
}