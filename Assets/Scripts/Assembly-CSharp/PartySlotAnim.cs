using UnityEngine;

public class PartySlotAnim : MonoBehaviour
{
    public void OnAnimOver()
    {
        var steamPreLobby = GetComponentInParent<SteamPreLobby>();
        if (steamPreLobby != null)
            steamPreLobby.OnAnimOver();
    }
}