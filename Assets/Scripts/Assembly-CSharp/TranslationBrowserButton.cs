using UnityEngine;
using UnityEngine.UI;

public class TranslationBrowserButton : MonoBehaviour
{
    public void OnClick()
    {
        string selectedKey = GetComponent<Text>().text;
        PlayerPrefsSl.Set("translation_changed", true);

        string translationPath = selectedKey;

        if (TranslationBrowser.Names.TryGetValue(selectedKey, out string fromNames))
        {
            translationPath = fromNames;
        }
        else if (TranslationBrowser.Languages.TryGetValue(selectedKey, out string fromLanguages))
        {
            translationPath = fromLanguages;
        }

        PlayerPrefsSl.Set("translation_path", translationPath);
        SimpleMenu.LoadCorrectScene();
    }
}