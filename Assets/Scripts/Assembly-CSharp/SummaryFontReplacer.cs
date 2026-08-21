using TMPro;
using UnityEngine;

public class SummaryFontReplacer : MonoBehaviour
{
    public TMP_FontAsset newFont;

    private void Awake()
    {
        string translationPath = PlayerPrefsSl.Get("translation_path", "en");
        if (!string.IsNullOrEmpty(translationPath) && translationPath.Contains("zh"))
        {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            if (text != null && newFont != null)
            {
                text.font = newFont;
            }
        }
    }
}