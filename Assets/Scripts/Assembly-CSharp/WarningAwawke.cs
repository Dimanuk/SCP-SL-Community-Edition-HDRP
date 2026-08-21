using UnityEngine;
using UnityEngine.UI;

public class WarningAwawke : MonoBehaviour
{
    public Toggle toggle;

    private void Awake()
    {
        if (PlayerPrefsSl.Get("warningToggle", "false") == "true")
        {
            gameObject.SetActive(false);
        }
    }

    public void Close()
    {
        if (toggle != null && toggle.isOn)
        {
            PlayerPrefsSl.Set("warningToggle", "true");
        }
        gameObject.SetActive(false);
    }
}
