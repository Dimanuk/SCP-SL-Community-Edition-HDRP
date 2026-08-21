using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class GammaSlider : MonoBehaviour
{
    public const string GammaConfigKey = "UVBrightness2";
    public const float DefaultBrightness = 0f;
    public const float MinBrightness = -2f;
    public const float MaxBrightness = 1f;

    public Slider slider;
    public Text warningText;
    public Text indicatorText;

    public static event Action<float> OnGammaChanged;

    public static float SavedToGui(float intensity)
    {
        return intensity * 10f;
    }

    public static float GuiToSaved(float setting)
    {
        return setting * 0.1f;
    }

    private void Start()
    {
        if (slider == null)
            return;

        slider.minValue = SavedToGui(MinBrightness); 
        slider.maxValue = SavedToGui(MaxBrightness);   

        float savedValue = PlayerPrefsSl.Get(GammaConfigKey, DefaultBrightness);
        slider.value = SavedToGui(savedValue);

        warningText.enabled = slider.value > 0f;
    }

    public void SetValue(float f)
    {
        if (indicatorText != null)
            indicatorText.text = f.ToString(CultureInfo.InvariantCulture);

        if (warningText != null)
            warningText.enabled = f > 0f;

        float savedValue = GuiToSaved(f);
        PlayerPrefsSl.Set(GammaConfigKey, savedValue);

        OnGammaChanged?.Invoke(savedValue);
    }
}