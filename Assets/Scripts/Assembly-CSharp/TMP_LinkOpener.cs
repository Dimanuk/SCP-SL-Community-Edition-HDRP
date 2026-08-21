using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class TMP_LinkOpener : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        TMP_Text text = GetComponent<TMP_Text>();
        if (text == null || eventData == null)
            return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, null);
        if (linkIndex == -1)
            return;

        TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
        string url = linkInfo.GetLinkID();

        if (SteamManager.IsSteamReady() && SteamUtils.IsOverlayEnabled)
            SteamFriends.OpenWebOverlay(url, false);
        else
            Application.OpenURL(url);
    }
}
