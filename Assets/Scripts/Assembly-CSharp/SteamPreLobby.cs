using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Image = UnityEngine.UI.Image;

public class SteamPreLobby : MonoBehaviour
{
    public GameObject root;
    public GameObject Counter;
    public GameObject LeaveButton;
    public Animator SlotAnim;
    public Image Host;
    public Image[] Friends;
    public Sprite InviteButton;
    public RectTransform ScrollView;
    public RectTransform Content;
    public RectTransform Element;

    private List<GameObject> _elements = new List<GameObject>();
    private static readonly int _bTrigger = Animator.StringToHash("bTrigger");

    private void Start()
    {
        root.SetActive(SteamManager.Running);
        SetAvatar(Host, SteamClient.SteamId);

        var info = Host.GetComponent<FriendInfo>();
        if (info != null)
        {
            info.SteamId = SteamClient.SteamId;
        }
    }

    public void InviteFriend()
    {
        SteamLobby.singleton.CreateLobby(20, true);
        LeaveButton.SetActive(true);
    }

    public void OnMemberJoined(Friend friend)
    {
        RefreshList(SteamLobby.singleton.Lobby);
    }

    public void OnMemberLeave(Friend friend)
    {
        RefreshList(SteamLobby.singleton.Lobby);
    }

    public void OnJoinLobby(Lobby lobby)
    {
        RefreshList(lobby);
        LeaveButton.SetActive(true);
    }

    public void OnAnimOver()
    {
        bool isTriggered = SlotAnim.GetBool(_bTrigger);
        Counter.SetActive(isTriggered);
    }

    public void OnCounterClick()
    {
        bool isActive = ScrollView.gameObject.activeSelf;
        ScrollView.gameObject.SetActive(!isActive);
    }

    public void Leave()
    {
        SteamLobby.singleton.LeaveLobby();
        LeaveButton.SetActive(false);

        foreach (var img in Friends)
        {
            img.sprite = InviteButton;
            var info = img.GetComponent<FriendInfo>();
            if (info != null) info.Clear();
        }

        SetAvatar(Host, SteamClient.SteamId);
        var hostInfo = Host.GetComponent<FriendInfo>();
        if (hostInfo != null) hostInfo.SteamId = SteamClient.SteamId;
    }

    public void RefreshList(Lobby lobby)
    {
        foreach (var el in _elements)
        {
            Destroy(el);
        }
        _elements.Clear();

        if (lobby.GetData("IsPreLobby") != "true") return;

        SetAvatar(Host, lobby.Owner.Id);
        var hostInfo = Host.GetComponent<FriendInfo>();
        if (hostInfo != null)
        {
            hostInfo.SteamId = lobby.Owner.Id;
            hostInfo.Username = lobby.Owner.Name;
        }

        var countText = Host.GetComponentInChildren<TextMeshProUGUI>();
        if (countText != null)
        {
            countText.text = lobby.MemberCount.ToString();
        }

        int friendIndex = 0;
        foreach (var member in lobby.Members)
        {
            if (member.Id == lobby.Owner.Id) continue;

            if (friendIndex < Friends.Length)
            {
                SetAvatar(Friends[friendIndex], member.Id);
                var fInfo = Friends[friendIndex].GetComponent<FriendInfo>();
                if (fInfo != null)
                {
                    fInfo.SteamId = member.Id;
                    fInfo.Username = member.Name;
                }
                friendIndex++;
            }
            else
            {
                AddElement(member);
            }
        }

        for (int i = friendIndex; i < Friends.Length; i++)
        {
            Friends[i].sprite = InviteButton;
            var fInfo = Friends[i].GetComponent<FriendInfo>();
            if (fInfo != null) fInfo.Clear();
        }
    }

    public void AddElement(Friend friend)
    {
        RectTransform newElement = Instantiate(Element, Content, true);
        _elements.Add(newElement.gameObject);

        Image img = newElement.GetComponentInChildren<Image>();
        SetAvatar(img, friend.Id);

        FriendInfo info = newElement.GetComponent<FriendInfo>();
        if (info != null)
        {
            info.SteamId = friend.Id;
            info.Username = friend.Name;
        }
    }

    public async void SetAvatar(Image image, SteamId steamid)
    {
        var task = await SteamFriends.GetLargeAvatarAsync(steamid);
        if (task.HasValue)
        {
            var val = task.Value;
            Texture2D tex = new Texture2D((int)val.Width, (int)val.Height, TextureFormat.RGBA32, false);
            tex.LoadRawTextureData(val.Data);
            tex.Apply();

            image.color = UnityEngine.Color.white;
            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}