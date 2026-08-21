using UnityEngine;
using UnityEngine.UI;

public class FPSLimiter : MonoBehaviour
{
    public GameObject Warning;
    private Dropdown _dropdown;

    private void Awake()
    {
        _dropdown = GetComponent<Dropdown>();
    }

    private void OnEnable()
    {
        Warning.SetActive(QualitySettings.vSyncCount != 0);

        int savedLimit = PlayerPrefsSl.Get("MaxFramerate", -1);
        Application.targetFrameRate = savedLimit;

        int currentLimit = Application.targetFrameRate;
        bool found = false;

        if (currentLimit != -1 && _dropdown != null)
        {
            var options = _dropdown.options;
            for (int i = 1; i < options.Count; i++)
            {
                if (!found && int.TryParse(options[i].text, out int parsed) && parsed == currentLimit)
                {
                    _dropdown.value = i;
                    found = true;
                }
            }

            if (!found)
            {
                var optionsList = _dropdown.options;
                int targetRate = Application.targetFrameRate;
                var newOption = new Dropdown.OptionData(targetRate.ToString());
                optionsList.Add(newOption);
                _dropdown.RefreshShownValue();
                _dropdown.value = optionsList.Count - 1;
            }
        }
    }

    public void OnValueChange()
    {
        if (_dropdown == null) return;

        var options = _dropdown.options;
        int selectedIndex = _dropdown.value;
        if (selectedIndex >= options.Count)
            throw new System.ArgumentOutOfRangeException();

        string selectedText = options[selectedIndex].text;

        if (!int.TryParse(selectedText, out int limit))
        {
            Application.targetFrameRate = -1;
            PlayerPrefsSl.Set("MaxFramerate", -1);
            return;
        }

        int clamped = Mathf.Clamp(limit, 15, 999);
        Application.targetFrameRate = clamped;
        PlayerPrefsSl.Set("MaxFramerate", Mathf.Clamp(limit, 15, 999));
    }

    private static void ChangeLimit(string limit)
    {
        if (!int.TryParse(limit, out int value))
        {
            Application.targetFrameRate = -1;
        }
        else
        {
            int clamped = Mathf.Clamp(value, 15, 999);
            Application.targetFrameRate = clamped;
        }

        PlayerPrefsSl.Set("MaxFramerate", 
            int.TryParse(limit, out int val) ? Mathf.Clamp(val, 15, 999) : -1);
    }
}