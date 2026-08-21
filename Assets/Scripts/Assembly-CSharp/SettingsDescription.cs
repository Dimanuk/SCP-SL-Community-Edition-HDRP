using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TextMeshProUGUI Description;

    [SerializeField]
    private string englishVersion;

    [SerializeField]
    private int index;

    [SerializeField]
    private string keyName;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Description == null)
            throw new System.NullReferenceException();

        string translated = TranslationReader.Get(keyName, index, englishVersion);
        Description.text = translated;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Description == null)
            throw new System.NullReferenceException();

        Description.text = string.Empty;
    }
}