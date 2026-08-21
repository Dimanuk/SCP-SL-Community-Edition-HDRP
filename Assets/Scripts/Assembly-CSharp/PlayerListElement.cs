using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoiceChat;

public class PlayerListElement : MonoBehaviour
{
    public ReferenceHub instance;
    public TextMeshProUGUI TextNick;
    public TextMeshProUGUI TextBadge;
    public RawImage ImgVerified;
    public Image ImgBackground;
    public Toggle ToggleMute;
    public GameObject OpenProfile;

    private void Start()
    {
        VoiceChatMutes.OnFlagsSet += RefreshMute;
        RefreshMute(instance, VoiceChatMutes.GetFlags(instance));
    }

    private void OnDestroy()
    {
        VoiceChatMutes.OnFlagsSet -= RefreshMute;
    }

    private void RefreshMute(ReferenceHub hub, VcMuteFlags flags)
    {
        if (hub == instance)
        {
            ToggleMute.isOn = flags != VcMuteFlags.None;
        }
    }

    public void Mute(bool b)
    {
        if (instance == null) return;

        if (Mirror.NetworkClient.active)
        {
            string userId = instance.characterClassManager?.UserId;
            if (!string.IsNullOrEmpty(userId))
            {
                if (b)
                {
                    VoiceChatMutes.IssueLocalMute(userId, false);
                }
                else
                {
                    VoiceChatMutes.RevokeLocalMute(userId, false);
                }
            }
        }
    }

    public void OpenSteamAccount()
    {
        ReferenceHub hub = ReferenceHub.GetHub(instance);
        if (hub?.characterClassManager == null) return;

        string userId = hub.characterClassManager.RealUserId;
        if (string.IsNullOrEmpty(userId))
        {
            userId = hub.characterClassManager.UserId;
        }

        if (!string.IsNullOrEmpty(userId) &&
            userId.EndsWith("@steam", StringComparison.Ordinal))
        {
            string steamIdStr = userId.Replace("@steam", string.Empty);
            if (ulong.TryParse(steamIdStr, NumberStyles.None, CultureInfo.InvariantCulture, out ulong steamId))
            {
                SteamManager.OpenProfile(steamId);
            }
        }
    }

    public void Report()
    {
        PlayerList playerList = GetComponentInParent<PlayerList>();
        if (playerList?.reportForm == null) return;

        TextMeshProUGUI[] textComponents = playerList.reportForm.GetComponentsInChildren<TextMeshProUGUI>();

        for (int i = 0; i < textComponents.Length; i++)
        {
            TextMeshProUGUI text = textComponents[i];
            if (text.name == "Player Name")
            {
                text.text = instance?.nicknameSync?.CombinedName ?? string.Empty;
            }
            else if (text.name == "Player ID")
            {
                text.text = instance?.PlayerId.ToString() ?? string.Empty;
            }
        }

        playerList.reportForm.SetActive(true);
        playerList.mainPanel.SetActive(false);
        playerList.reportPopup.SetActive(false);
    }
}