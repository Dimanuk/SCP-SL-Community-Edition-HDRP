using Steamworks;
using UnityEngine;

public class FriendInfo : MonoBehaviour
{
    public SteamId SteamId;
    public string Username;

    public void OnButtonClick()
    {
        if (SteamId == default(SteamId))
        {
            var steamPreLobby = GetComponentInParent<SteamPreLobby>();
            if (steamPreLobby != null)
                steamPreLobby.InviteFriend();
        }
        else
        {
            SteamFriends.OpenUserOverlay(SteamId, "steamid");
        }
    }

    public void Clear()
    {
        SteamId = default(SteamId);
        Username = string.Empty;
    }
}