using PlayerRoles;
using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamManager : MonoBehaviour
{
    private const uint SteamAppId = 700330u;
    private static AuthTicket _ticket;
    private static string _state = "";

    public static bool Running { get; private set; }
    public static ulong SteamId64 { get; private set; }

    public static bool IsSteamReady() => Running && SteamClient.IsValid;

    private void Awake()
    {
        if (!Running) StartClient();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        ReferenceHub.OnPlayerAdded += RefreshPlayerCount;
        ReferenceHub.OnPlayerRemoved += RefreshPlayerCount;
        PlayerRoleManager.OnRoleChanged += OnPlayerRoleChanged;

        ChangeLobbyStatus(0, 20);
    }

    private void FixedUpdate()
    {
        if (Running) SteamClient.RunCallbacks();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        ReferenceHub.OnPlayerAdded -= RefreshPlayerCount;
        ReferenceHub.OnPlayerRemoved -= RefreshPlayerCount;
        PlayerRoleManager.OnRoleChanged -= OnPlayerRoleChanged;

        CancelTicket();
        StopClient();
    }

    public static void StartClient()
    {
        if (SteamClient.IsValid) return;

        Steamworks.Dispatch.OnException = (ex) => Debug.LogError($"[Steam Error] {ex.Message}");

        try
        {
            SteamClient.Init(SteamAppId, true);
            if (SteamClient.IsLoggedOn)
            {
                SteamId64 = SteamClient.SteamId;
                Running = true;
                Debug.Log($"[SteamManager] Started: {SteamClient.Name} ({SteamId64})");
            }
            else
            {
                _state = "Client isn't logged-on.";
                Debug.LogWarning("[SteamManager] " + _state);
                StopClient();
            }
        }
        catch (Exception ex)
        {
            _state = "Steam Init Failed";
            Debug.LogError($"[SteamManager] {_state}: {ex.Message}");
            StopClient();
        }
    }

    public static void StopClient()
    {
        Running = false;
        try { SteamClient.Shutdown(); } catch { }
        _ticket = null;
    }

    public static void RestartSteam()
    {
        StopClient();
        StartClient();
    }

    public static void ChangePreset(int classId)
    {
        if (!IsSteamReady()) return;
        try
        {
            if (PlayerPrefsSl.Get("RichPresence", true))
            {
                SteamFriends.SetRichPresence("class", classId.ToString());
                string display = classId == -1 ? "#Status_WaitingForPlayers" : "#Status_MainMenu";
                SteamFriends.SetRichPresence("steam_display", display);
            }
        }
        catch { }
    }

    public static void ChangeLobbyStatus(int cur, int max)
    {
        if (IsSteamReady())
            SteamFriends.SetRichPresence("lobbystatus", $"{cur}/{max}");
    }

    public static void ClearRichPresence()
    {
        if (IsSteamReady()) SteamFriends.ClearRichPresence();
    }

    private void OnRichPresenceChanged(bool val)
    {
        if (val)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name.Contains("Menu")) ChangePreset(-2);
            else if (activeScene.name == "Facility") ChangePreset(-1);
        }
        else ClearRichPresence();
    }

    public static void SetAchievement(string key)
    {
        try { new Achievement(key).Trigger(true); } catch { }
    }

    public static void ResetAchievement(string key)
    {
        try { new Achievement(key).Clear(); } catch { }
    }

    public static bool CheckAchievement(string key)
    {
        try { return new Achievement(key).State; } catch { return false; }
    }

    public static void SetStat(string key, int value)
    {
        if (IsSteamReady()) SteamUserStats.SetStat(key, value);
    }

    public static int GetStat(string key)
    {
        return IsSteamReady() ? SteamUserStats.GetStatInt(key) : 0;
    }

    public static string GetApiState()
    {
        if (IsSteamReady()) return "Loaded";
        return string.IsNullOrEmpty(_state) ? "Not Loaded" : _state;
    }

    public static string GetPersonaName(ulong steamid)
    {
        if (!IsSteamReady()) return "Unknown";
        try { return new Friend(steamid).Name; } catch { return "Unknown"; }
    }

    public static void OpenProfile(ulong steamid)
    {
        if (!IsSteamReady()) return;
        SteamFriends.OpenUserOverlay(steamid == 0 ? SteamId64 : steamid, "steamid");
    }

    public static AuthTicket GetAuthSessionTicket()
    {
        if (!IsSteamReady()) return null;
        _ticket = SteamUser.GetAuthSessionTicket();
        return _ticket;
    }

    public static void RefreshToken()
    {
        if (!IsSteamReady()) return;
        CancelTicket();
        _ticket = SteamUser.GetAuthSessionTicket();
        Debug.Log("[SteamManager] Token refreshed.");
    }

    public static void CancelTicket()
    {
        _ticket?.Cancel();
        _ticket = null;
    }

    private void OnPlayerRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase curRole)
    {
        if (hub != null && hub.isLocalPlayer) ChangePreset((int)curRole.RoleTypeId);
    }

    private void RefreshPlayerCount(ReferenceHub hub)
    {
        if (ReferenceHub.TryGetHostHub(out _)) ChangeLobbyStatus(ReferenceHub.AllHubs.Count, 20);
    }

    private static void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Menu")) ChangePreset(-2);
        else if (scene.name == "Facility") ChangePreset(-1);
    }
}