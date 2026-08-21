using UnityEngine;
using UnityEngine.UI;

public class VSyncFPSlimit : MonoBehaviour
{
    public void Check()
    {
        Slider slider = GetComponent<Slider>();
        if (slider != null && !slider.IsDestroyed())
        {
            Application.targetFrameRate = PlayerPrefsSl.Get("MaxFramerate", -1);
        }
    }
}