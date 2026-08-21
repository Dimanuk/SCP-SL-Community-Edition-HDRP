using TMPro;
using UnityEngine;

public class WindowModeButton : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    public void ChangeWindowMode()
    {
        if (dropdown != null)
        {
            int mode = dropdown.value;
            ResolutionManager.ChangeScreenMode(mode);
        }
    }

    private void OnEnable()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        if (dropdown == null) return;

        int savedMode = PlayerPrefsSl.Get("ScreenMode", 1);
        dropdown.value = savedMode;

        var options = dropdown.options;
        if (options.Count > 0)
        {
            options[0].text = TranslationReader.Get("NewMainMenu", 24, "Exclusive Fullscreen");
        }
        if (options.Count > 1)
        {
            options[1].text = TranslationReader.Get("NewMainMenu", 25, "Borderless Window");
        }
        if (options.Count > 2)
        {
            options[2].text = TranslationReader.Get("NewMainMenu", 26, "Maximized Window");
        }
        if (options.Count > 3)
        {
            options[3].text = TranslationReader.Get("NewMainMenu", 27, "Windowed");
        }
        dropdown.RefreshShownValue();
    }
}