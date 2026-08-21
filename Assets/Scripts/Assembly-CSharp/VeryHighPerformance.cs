using CustomCulling;
using UnityEngine;

public class VeryHighPerformance : MonoBehaviour
{
    public static readonly Color NormalColor = new Color(0.01f, 0.01f, 0.01f, 1f);
    public static readonly Color VHColor = new Color(0.22f, 0.22f, 0.22f, 1f);

    public static bool LightsOff { get; private set; }

    public static Color TargetColor
    {
        get
        {
            return LightsOff ? VHColor : NormalColor;
        }
    }

    private void Start()
    {
        LightsOff = !PlayerPrefsSl.Get(SettingsOption.gfxsets_RenderLight.ToString(), true) || ServerStatic.IsDedicated;
    }

    private void Update()
    {
        CullingManager.AllLightsDisabled = LightsOff;
    }
}