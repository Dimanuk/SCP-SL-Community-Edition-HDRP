using UnityEngine;
using UnityEngine.UI;

public class MaintainPropotionsToggle : MonoBehaviour
{
    private bool _state;

    public Slider[] CompatibleSliders;

    public void SetValue(bool val)
    {
        _state = val;

        if (!val || CompatibleSliders == null || CompatibleSliders.Length == 0)
            return;

        Slider referenceSlider = CompatibleSliders[0];
        if (referenceSlider == null)
            return;

        float referenceValue = referenceSlider.value;

        for (int i = 1; i < CompatibleSliders.Length; i++)
        {
            Slider slider = CompatibleSliders[i];
            if (slider != null)
            {
                slider.value = referenceValue;
            }
        }
    }

    public void UseSlider(int sliderId)
    {
        if (!_state || CompatibleSliders == null ||
            sliderId < 0 || sliderId >= CompatibleSliders.Length)
            return;

        Slider sourceSlider = CompatibleSliders[sliderId];
        if (sourceSlider == null)
            return;

        float sourceValue = sourceSlider.value;

        for (int i = 0; i < CompatibleSliders.Length; i++)
        {
            if (i == sliderId)
                continue;

            Slider targetSlider = CompatibleSliders[i];
            if (targetSlider != null)
            {
                targetSlider.value = sourceValue;
            }
        }
    }
}