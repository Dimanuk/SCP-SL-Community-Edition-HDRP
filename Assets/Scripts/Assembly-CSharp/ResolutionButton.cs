using UnityEngine;
using UnityEngine.UI;

public class ResolutionButton : MonoBehaviour
{
    private int cachedResolutionPreset;
    private int resolutionPreset;
    private Text txt;

    public void Click(int id)
    {
        int count = ResolutionManager.Presets?.Count ?? 0;
        int newPreset = Mathf.Clamp(ResolutionManager.Preset + id, 0, Mathf.Max(0, count - 1));

        ResolutionManager.Preset = newPreset;
        PlayerPrefsSl.Set("SavedResolutionSet", newPreset);
        ResolutionManager.RefreshScreen();
    }

    protected void Start()
    {
        txt = GetComponent <Text>();

        if (txt != null)
        {
            txt.text = ResolutionManager.CurrentResolutionString();
        }

        cachedResolutionPreset = ResolutionManager.Preset;
    }

    protected void Update()
    {
        resolutionPreset = ResolutionManager.Preset;

        if (cachedResolutionPreset != resolutionPreset)
        {
            if (txt != null)
            {
                txt.text = ResolutionManager.CurrentResolutionString();
            }

            cachedResolutionPreset = resolutionPreset;
        }
    }
}