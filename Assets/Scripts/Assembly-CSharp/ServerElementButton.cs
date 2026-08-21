using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using MEC; // More Effective Coroutines
using NorthwoodLib;

public class ServerElementButton : MonoBehaviour
{
    public static string HighlightedIp;

    public GameObject[] icons;
    public Image FavorImage;
    public Sprite[] FavorSprites;

    public string ServerName;
    public string IpAddress;
    public int Players;
    public uint ServerID;
    public int MaxPlayers;

    private NewMainMenu newMainMenu;
    private NewServerBrowser newServerBrowser;
    private RectTransform serverInfo;
    private ReportServer reportServer;
    private TextMeshProUGUI infoText;
    private Image bgColor;

    private void Awake()
    {
        bgColor = GetComponent<Image>();
        newMainMenu = GetComponentInParent<NewMainMenu>();
        newServerBrowser = GetComponentInParent<NewServerBrowser>();
        serverInfo = newServerBrowser.ServerInfo;
        infoText = serverInfo.GetComponentInChildren<TextMeshProUGUI>();
        reportServer = newMainMenu.GetComponentInChildren<ReportServer>();
    }

    private void OnEnable()
    {
        SetValues();
    }

    public void SetValues()
    {
        PlayButton component = GetComponent<PlayButton>();
        if (component == null)
        {
            return;
        }

        IpAddress = string.Concat(component.Ip, ":", component.Port);
        ServerID = component.ServerID;

        if (component.Motd != null)
            ServerName = component.Motd.text;

        if (component.Players != null)
        {
            string[] split = component.Players.text.Split(new char[] { '/' });
            if (split.Length > 0)
                int.TryParse(split[0], out Players);
            if (split.Length > 1)
                int.TryParse(split[1], out MaxPlayers);
        }

        // v12 keys Favorites/History/last-joined on ServerID.ToString(), not the ip:port string.
        bool isFav = FavoriteAndHistory.IsInStorage((FavoriteAndHistory.StorageLocation)1, ServerID.ToString());
        SwitchFavorIcon(isFav);
    }

    public void SwitchFavorIcon(bool favored)
    {
        FavorImage.sprite = favored ? FavorSprites[1] : FavorSprites[0];
    }

    public void PlayButton()
    {
        newMainMenu.Connect(IpAddress, false);

        // IPHistory (2) is keyed on the raw ip:port for direct-connect recall.
        FavoriteAndHistory.Modify((FavoriteAndHistory.StorageLocation)2, IpAddress, false);

        // History (0) and the in-game favorite star key on ServerID.ToString().
        FavoriteAndHistory.ServerIDLastJoined = ServerID.ToString();
        FavoriteAndHistory.Modify((FavoriteAndHistory.StorageLocation)0, FavoriteAndHistory.ServerIDLastJoined, false);
    }

    public void ShowInfo()
    {
        PlayButton component = GetComponent<PlayButton>();
        if (component != null)
        {
            Timing.RunCoroutine(_Show(component.InfoType), Segment.Update);
            HighlightedIp = IpAddress;
        }
    }

    private void LateUpdate()
    {
        if (bgColor == null) return;

        bool isHighlighted = string.Equals(HighlightedIp, IpAddress);
        bgColor.color = isHighlighted ? Color.cyan : Color.white;
    }

    public void MarkAsFavorite()
    {
        // Favorites are keyed on ServerID.ToString() so the browser tab and the
        // in-game star (which read ServerID) stay in sync.
        string serverId = ServerID.ToString();
        bool isFav = FavoriteAndHistory.IsInStorage((FavoriteAndHistory.StorageLocation)1, serverId);

        if (!isFav)
        {
            FavoriteAndHistory.Modify((FavoriteAndHistory.StorageLocation)1, serverId, false);
            SwitchFavorIcon(true);
        }
        else
        {
            FavoriteAndHistory.Modify((FavoriteAndHistory.StorageLocation)1, serverId, true);
            SwitchFavorIcon(false);
        }
    }

    public void ReportForm()
    {
        if (reportServer != null)
            reportServer.Show(IpAddress);
    }

    private IEnumerator<float> _Show(string id)
    {
        if (serverInfo != null)
            serverInfo.gameObject.SetActive(true);

        if (infoText != null)
            infoText.text = string.Empty;

        if (!id.Contains("/"))
        {
            UnityWebRequest www = UnityWebRequest.Get("https://pastebin.com/raw/" + id);
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

                if (infoText != null)
                    infoText.text = content;
            }
            else if (StringUtils.Contains(www.error,
                "<title>Pastebin.com - Locked Paste</title>",
                System.StringComparison.Ordinal))
            {
                if (infoText != null)
                    infoText.text = "The provided paste is locked via password and cannot be displayed. Please contact the server owner.";
            }
            else
            {
                if (infoText != null)
                    infoText.text = www.error;
            }

            www.Dispose();
        }
        else
        {
            if (infoText != null)
                infoText.text = "The URL isn't directing to pastebin site. Please contact server owner.";
        }
    }
}