using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    public GameObject checkmark;
    public bool isOn;

    private void OnEnable()
    {
        isOn = PlayerPrefsSl.Get("SavedFullscreen", true);
        checkmark.SetActive(isOn);
    }

    public void Click()
    {
        isOn = !isOn;
        checkmark.SetActive(isOn);
        PlayerPrefsSl.Set("SavedFullscreen", isOn);
        ResolutionManager.ChangeFullscreen(isOn);
    }
}