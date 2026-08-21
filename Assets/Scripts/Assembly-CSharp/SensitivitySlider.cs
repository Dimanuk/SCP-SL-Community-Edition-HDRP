
using System;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
	[SerializeField]
	private Slider _mainSlider;

	[SerializeField]
	private Slider _adsSlider;

	[SerializeField]
	private Text _adsPercent;

	private const float AdsRatio = 10f;

    private void Start()
    {
        float mainSens = PlayerPrefsSl.Get("Sens", 1f);     
        float adsSens = PlayerPrefsSl.Get("SensAds", 1f);    

        SetSliderValues(mainSens, adsSens);
    }

    private void SaveSliderValues()
    {
        if (_mainSlider == null || _mainSlider.IsDestroyed())
            throw new NullReferenceException();

        PlayerPrefsSl.Set("Sens", _mainSlider.value);

        if (_adsSlider == null || _adsSlider.IsDestroyed())
            throw new NullReferenceException();

        PlayerPrefsSl.Set("SensAds", _adsSlider.value / AdsRatio);
    }

    public void OnMainValueChanged(float val)
    {
        SensitivitySettings.SensMultiplier = val;  
        SaveSliderValues();
    }

    public void OnAdsValueChanged(float val)
    {
        SensitivitySettings.AdsReductionMultiplier = val / AdsRatio;

        float displayPercent = val * AdsRatio;           
        int rounded = Mathf.RoundToInt(displayPercent);  
        float formatted = rounded / AdsRatio;            

        _adsPercent.text = formatted.ToString("0.0") + "x";

        SaveSliderValues();
    }

    public void SetSliderValues(float main, float ads)
    {
        if (_adsSlider == null) 
            throw new NullReferenceException();

        _adsSlider.value = ads * AdsRatio;

        if (_mainSlider == null) 
            throw new NullReferenceException();

        _mainSlider.minValue = Mathf.Min(_mainSlider.minValue, main);
        _mainSlider.maxValue = Mathf.Max(_mainSlider.maxValue, main);
        _mainSlider.value = main;

        SensitivitySettings.AdsReductionMultiplier = ads;

        float displayPercent = ads * AdsRatio;
        int rounded = Mathf.RoundToInt(displayPercent);
        float formatted = rounded / AdsRatio;
        _adsPercent.text = formatted.ToString("0.0") + "x";

        SaveSliderValues();

        SensitivitySettings.SensMultiplier = main;
        SaveSliderValues();
    }
}
