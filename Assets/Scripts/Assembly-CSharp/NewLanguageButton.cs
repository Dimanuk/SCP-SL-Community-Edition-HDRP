using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class NewLanguageButton : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown dropdown;

    private void Start()
    {
        if (dropdown == null) return;

        List<TMP_Dropdown.OptionData> optionsList = new List<TMP_Dropdown.OptionData>();
        int selectedIndex = TranslationBrowser.GetTranslationList();
        var translations = TranslationBrowser.Translations;

        if (translations != null)
        {
            foreach (var translation in translations)
            {
                string displayName = translation.Item1;
                optionsList.Add(new TMP_Dropdown.OptionData(displayName));
            }
        }
        dropdown.options = optionsList;
        dropdown.value = Mathf.Clamp(selectedIndex, 0, optionsList.Count - 1);
    }

    public void ChangeLanguage()
    {
        if (dropdown == null) return;

        int selectedIndex = dropdown.value;
        if (selectedIndex < 0 || selectedIndex >= dropdown.options.Count) return;

        string rawText = dropdown.options[selectedIndex].text;
        string cleanName = rawText.Replace("<color=red>", "").Replace("</color>", "");

        PlayerPrefsSl.Set("translation_changed", true);
        string newTranslationPath = TranslationBrowser.NameToDirectory(cleanName);
        PlayerPrefsSl.Set("translation_path", newTranslationPath);

        if (TranslationReader.TranslationDirectoryName != newTranslationPath)
        {
            NewInput.Load();
            SimpleMenu.LoadCorrectScene();
        }
    }
}