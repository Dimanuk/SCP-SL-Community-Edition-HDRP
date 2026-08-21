using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using MEC;
using NorthwoodLib;

public class ServerInfo : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
    private Canvas canvas;
    public GameObject root;
    public TextMeshProUGUI text;

    public static void ShowInfo(string id)
    {
        ServerInfo instance = UnityEngine.Object.FindFirstObjectByType<ServerInfo>();
        if (instance != null)
        {
            Timing.RunCoroutine(instance._Show(id), Segment.Update);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (text == null) return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
            text,
            Input.mousePosition,
            canvas != null ? canvas.worldCamera : null);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }

    public IEnumerator<float> _Show(string id)
    {
        if (root != null)
            root.SetActive(true);

        MainMenuScript menu = UnityEngine.Object.FindFirstObjectByType<MainMenuScript>();
        if (menu != null)
            menu.ResetMenu();

        if (text != null)
            text.text = string.Empty;

        if (!id.Contains("/"))
        {
            string url = "https://pastebin.com/raw/" + UnityWebRequest.EscapeURL(id);
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return Timing.WaitUntilDone(www.SendWebRequest());

            if (string.IsNullOrEmpty(www.error))
            {
                string content = www.downloadHandler.text;

                if (content.Length > 0x1388)
                {
                    content = StringUtils.TruncateToLast(content, 0x1388, '\n')
                        + "...\n<i><color=#87CEFA><u><link=\"https://pastebin.com/"
                        + id + "\">(Click here for full content)</link></u></color></i>";
                }

                if (text != null)
                    text.text = content;
            }
            else if (StringUtils.Contains(www.error,
                "<title>Pastebin.com - Locked Paste</title>",
                System.StringComparison.Ordinal))
            {
                if (text != null)
                    text.text = "The provided paste is locked via password and cannot be displayed. Please contact the server owner.";
            }
            else
            {
                if (text != null)
                    text.text = www.error;
            }

            www.Dispose();
        }
        else
        {
            if (text != null)
                text.text = "The URL isn't directing to pastebin site. Please contact server owner.";
        }
    }

    public void Close()
    {
        ServerInfo instance = UnityEngine.Object.FindFirstObjectByType<ServerInfo>();
        if (instance != null && instance.root != null)
            instance.root.SetActive(false);

        MainMenuScript menu = UnityEngine.Object.FindFirstObjectByType<MainMenuScript>();
        if (menu != null)
            menu.ResetMenu();
    }
}