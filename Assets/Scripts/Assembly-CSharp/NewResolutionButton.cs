using TMPro;
using UnityEngine;

public class NewResolutionButton : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    public unsafe void ChangeResolution()
    {
        if (dropdown != null)
        {
            ResolutionManager.SetResolution(dropdown.value);
        }
    }

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        if (dropdown != null)
        {
            var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();

            foreach (var preset in ResolutionManager.Presets)
            {
                string label = $"{preset.Width} X {preset.Height}";
                options.Add(new TMP_Dropdown.OptionData(label));
            }

            dropdown.options = options;
            dropdown.value = ResolutionManager.Preset;
        }
    }
}