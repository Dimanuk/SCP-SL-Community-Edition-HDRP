using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror.LiteNetLib4Mirror;
using Color = UnityEngine.Color;
using Console = GameCore.Console;

public class SteamLobby : MonoBehaviour
{
    private enum LobbyPrivacy
    {
        Private = 0,
        Friends = 1,
        Public = 2
    }

    public static SteamLobby singleton;
    public static List<string> FriendsServer;

    private Lobby _lobby;
    private Lobby _newLobby;
    private bool _waitingForScene;
    private bool _waitingForSafeTime;
    private float _safeTime;

    public Lobby Lobby => _lobby;

    private void Awake()
    {
        if (singleton != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        singleton = this;
        DontDestroyOnLoad(gameObject);
        FriendsServer = new List<string>();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
    }

    private void FixedUpdate()
    {
        if (_safeTime > 0)
        {
            _safeTime -= Time.fixedDeltaTime;
        }
        else if (_waitingForSafeTime)
        {
            _waitingForSafeTime = false;
            Console.AddLog($"Connecting to lobby: {_newLobby.Id}", Color.white);
            Connect(_newLobby);
        }
    }

    public void SetLobbyPrivacy(int privacyType)
    {
        bool success = false;
        switch (privacyType)
        {
            case 0: success = _lobby.SetPrivate(); break;
            case 1: success = _lobby.SetFriendsOnly(); break;
            case 2: success = _lobby.SetPublic(); break;
        }

        if (success)
            Console.AddLog($"Setting Steam Lobby to {(LobbyPrivacy)privacyType}!", Color.green);
        else
            Console.AddLog($"Failed to set Steam Lobby to {(LobbyPrivacy)privacyType}.", Color.red);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_waitingForScene)
        {
            if (scene.name.Contains("Menu"))
            {
                _waitingForScene = false;
                _safeTime = 1.0f;
                _waitingForSafeTime = true;
            }
        }

        if (scene.name.Contains("Facility"))
        {
            return;
        }

        if (!scene.name.Contains("Menu"))
        {
            LeaveLobby();
        }
    }

    private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        Console.AddLog($"Join requested for lobby {lobby.Id} from {steamId}", Color.white);

        if (IsInLobby(lobby, SteamClient.SteamId.ToString()))
        {
            return;
        }

        RoomEnter result = await lobby.Join();
        if (result == RoomEnter.Success)
        {
            _newLobby = lobby;
            _lobby = lobby;

            if (SceneManager.GetActiveScene().name.Contains("Menu"))
            {
                _waitingForSafeTime = true;
                _safeTime = 0.5f;
            }
            else
            {
                Connect(lobby);
            }
        }
    }

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        Console.AddLog($"Lobby created: Result: {result}", result == Result.OK ? Color.green : Color.red);
        if (result == Result.OK)
        {
            _lobby = lobby;
        }
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        _lobby = lobby;
        ShowLobbyInfo(lobby);
    }

    private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        Console.AddLog($"{friend.Name} joined lobby", Color.white);
        if (lobby.GetData("IsPreLobby") == "true")
        {
            SteamPreLobby preLobby = FindFirstObjectByType<SteamPreLobby>();
            if (preLobby != null) preLobby.OnMemberJoined(friend);
        }
    }

    private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        Console.AddLog($"{friend.Name} left lobby", Color.white);
        if (lobby.GetData("IsPreLobby") == "true")
        {
            SteamPreLobby preLobby = FindFirstObjectByType<SteamPreLobby>();
            if (preLobby != null) preLobby.OnMemberLeave(friend);
        }
    }

    private void Connect(Lobby lobby)
    {
        if (lobby.GetData("IsPreLobby") == "true")
        {
            SteamPreLobby preLobby = FindFirstObjectByType<SteamPreLobby>();
            if (preLobby != null) preLobby.OnJoinLobby(lobby);
        }
        else
        {
            string ip = lobby.GetData("ipaddress");
            if (!string.IsNullOrEmpty(ip))
            {
                NewMainMenu menu = FindFirstObjectByType<NewMainMenu>();
                if (menu != null)
                {
                    Console.AddLog($"[SteamLobby] Connecting to {ip}", Color.white);
                    menu.Connect(ip);
                }
                else
                {
                    Console.AddLog($"[SteamLobby] Cannot connect to {ip}. MainMenu is not loaded.", Color.red);
                }
            }
        }
    }

    public async void CreateLobby(int maxMembers, bool preLobby)
    {
        LeaveLobby();
        if (maxMembers > 30) maxMembers = 30;

        await SteamMatchmaking.CreateLobbyAsync(maxMembers);
    }

    public void LeaveLobby()
    {
        if (_lobby.Id != 0)
        {
            Console.AddLog("Leaving lobby", Color.white);
            _lobby.Leave();
            _lobby = default;
        }
    }

    public void ShowLobbyInfo(Lobby lobby)
    {
        string members = "";
        foreach (var m in lobby.Members) members += m.Name + "\n";

        string info = $"[Steam Lobby Info]\nLobbyId: {lobby.Id}\nOwner: {lobby.Owner.Name}\nIP: {lobby.GetData("ipaddress")}\nMembers ({lobby.MemberCount}/{lobby.MaxMembers}):\n{members}";
        Console.AddLog(info, Color.white);
    }

    public bool IsInLobby(Lobby lobby, string steamId)
    {
        foreach (var member in lobby.Members)
        {
            if (member.Id.ToString() == steamId) return true;
        }

        string myIp = $"{LiteNetLib4MirrorTransport.Singleton.clientAddress}:{LiteNetLib4MirrorTransport.Singleton.port}";
        return lobby.GetData("ipaddress").Equals(myIp);
    }

    public static async void RefreshFriendsServer()
    {
        FriendsServer = new List<string>();
        foreach (var friend in SteamFriends.GetFriends())
        {
            if (friend.IsOnline && friend.IsPlayingThisGame)
            {
                var game = friend.GameInfo;
                if (game.HasValue && game.Value.Lobby.HasValue)
                {
                    string ip = game.Value.Lobby.Value.GetData("ipaddress");
                    if (!string.IsNullOrEmpty(ip) && !FriendsServer.Contains(ip))
                        FriendsServer.Add(ip);
                }
            }
        }
        await Task.CompletedTask;
    }
}