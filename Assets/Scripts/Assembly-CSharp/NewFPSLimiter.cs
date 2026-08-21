using TMPro;
using UnityEngine;

public class NewFPSLimiter : MonoBehaviour
{
    public TMP_Dropdown Dropdown;
    public TextMeshProUGUI Text;

    private void OnEnable()
    {
        if (ServerStatic.IsDedicated == true)
            return;

        int targetFPS = PlayerPrefsSl.Get("MaxFramerate", -1);
        Application.targetFrameRate = targetFPS;

        if (Dropdown == null) return;

        int currentFPS = Application.targetFrameRate;
        if (currentFPS == -1)
        {
            if (Dropdown.options.Count > 0)
                Dropdown.value = 0;
            return;
        }

        bool found = false;
        for (int i = 0; i < Dropdown.options.Count; i++)
        {
            if (int.TryParse(Dropdown.options[i].text, out int val) && val == currentFPS)
            {
                Dropdown.value = i;
                found = true;
                break;
            }
        }

        if (!found)
        {
            var newOption = new TMP_Dropdown.OptionData(currentFPS.ToString());
            Dropdown.options.Add(newOption);
            Dropdown.value = Dropdown.options.Count - 1;
            Dropdown.RefreshShownValue();
        }

        if (Dropdown.options.Count > 0)
        {
            Dropdown.options[0].text = TranslationReader.Get("NewMainMenu", 30, "Off");
            Dropdown.RefreshShownValue();
        }
    }

    public void OnValueChange()
    {
        if (Dropdown == null) return;

        int idx = Dropdown.value;
        if (idx < 0 || idx >= Dropdown.options.Count) return;

        string selectedText = Dropdown.options[idx].text;
        if (int.TryParse(selectedText, out int fps))
        {
            fps = Mathf.Clamp(fps, 15, 999);
            Application.targetFrameRate = fps;
            PlayerPrefsSl.Set("MaxFramerate", fps);
        }
        else
        {
            Application.targetFrameRate = -1;
            PlayerPrefsSl.Set("MaxFramerate", -1);
        }
    }

    public void SetEnable(bool enable)
    {
        if (Text != null)
        {
            Text.color = enable ? Color.white : Color.gray;
        }
        if (Dropdown != null)
        {
            Dropdown.interactable = enable;
        }
    }
    private void ChangeLimit(string limit)
    {
        if (int.TryParse(limit, out int fps))
        {
            fps = Mathf.Clamp(fps, 15, 999);
            Application.targetFrameRate = fps;
            PlayerPrefsSl.Set("MaxFramerate", fps);
        }
        else
        {
            Application.targetFrameRate = -1;
            PlayerPrefsSl.Set("MaxFramerate", -1);
        }
    }
}