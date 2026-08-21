using UnityEngine;
using UnityEngine.UI;

public class GameMenuToggles : MonoBehaviour
{
    public static bool AdsToggleEnabled;

    public Toggle ToggleYInvert;
    public Toggle ToggleADS;

    private void Start()
    {
        ToggleYInvert.isOn = PlayerPrefsSl.Get("y_invert", false);
        SensitivitySettings.Invert = ToggleYInvert.isOn;

        ToggleADS.isOn = PlayerPrefsSl.Get("ads_toggle", false);
        AdsToggleEnabled = ToggleADS.isOn;
    }

    private void ChangeStateYInvert(bool b)
    {
        PlayerPrefsSl.Set("y_invert", b);
        SensitivitySettings.Invert = b;
    }

    private void ChangeStateADS(bool b)
    {
        PlayerPrefsSl.Set("ads_toggle", b);
        AdsToggleEnabled = b;
    }
}