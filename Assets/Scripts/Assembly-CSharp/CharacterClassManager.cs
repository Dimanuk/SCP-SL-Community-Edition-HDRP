using GameCore;
using MEC;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using Security;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClassManager : NetworkBehaviour
{
    private CentralAuthInterface _centralAuthInt;

    private ReferenceHub _hub;

    private bool _hubSet;

    [NonSerialized]
    public string UserId2;

    public static bool OnlineMode;

    [NonSerialized]
    public bool GodMode;

    private bool _wasAnytimeAlive;

    private bool _commandtokensent;

    internal static bool EnableSyncServerCmdBinding;

    [NonSerialized]
    internal string AuthToken;

    [NonSerialized]
    internal string AuthTokenSerial;

    [NonSerialized]
    public string RequestIp;

    [NonSerialized]
    public string Asn;

    [SyncVar]
    public string Pastebin;

    [SyncVar]
    public byte MaxPlayers;

    internal static bool CuffedChangeTeam;

    internal static bool ForceCuffedChangeTeam;

    [SyncVar]
    public bool RoundStarted;

    [SyncVar(hook = nameof(UserIdHook))]
    public string SyncedUserId;

    internal string RealUserId;

    private static KeyCode _noclipKey;

    private static KeyCode _noclipFogToggleKey;

    private string _privUserId;

    private ClientInstanceMode _targetInstanceMode;

    private const string HostId = "ID_Host";

    private const string DedicatedId = "ID_Dedicated";

    private RateLimit _commandRateLimit;

    private readonly RateLimit _deathScreenRateLimit = new RateLimit(2, 4f);

    internal ServerRoles SrvRoles { get; private set; }

    internal NetworkConnection Connection { get; private set; }

    private ReferenceHub Hub
    {
        get
        {
            if (!_hubSet && ReferenceHub.TryGetHub(base.gameObject, out _hub))
                _hubSet = true;
            return _hub;
        }
    }

    public string VacSession { get; internal set; }

    public ClientInstanceMode InstanceMode
    {
        get => _targetInstanceMode;
        private set
        {
            if (value != _targetInstanceMode)
            {
                _targetInstanceMode = value;
                CharacterClassManager.OnInstanceModeChanged?.Invoke(Hub, _targetInstanceMode);
            }
        }
    }

    public string UserId
    {
        get
        {
            if (!global::Mirror.NetworkServer.active)
                return SyncedUserId;
            if (_privUserId == null)
                return null;
            if (_privUserId.Contains("$"))
                return _privUserId.Substring(0, _privUserId.IndexOf("$", global::System.StringComparison.Ordinal));
            return _privUserId;
        }
        set
        {
            if (global::Mirror.NetworkServer.active)
            {
                _privUserId = value;
                RefreshSyncedId();
                Hub.serverRoles.RefreshRealId();
            }
        }
    }

    public string SaltedUserId
    {
        get
        {
            if (!global::Mirror.NetworkServer.active)
                return SyncedUserId;
            return _privUserId;
        }
    }

    public static event Action OnRoundStarted;

    public static event Action<ReferenceHub> OnSyncedUserIdAssigned;

    public static event Action<ReferenceHub, ClientInstanceMode> OnInstanceModeChanged;

    [global::Mirror.Server]
    public void RefreshSyncedId()
    {
        if (!global::Mirror.NetworkServer.active)
        {
            global::UnityEngine.Debug.LogWarning("[Server] function 'System.Void CharacterClassManager::RefreshSyncedId()' called when server was not active");
            return;
        }
        if (_privUserId == null)
        {
            SyncedUserId = null;
            RunUserIdHookIfServerOnly();
            return;
        }
        if (base.isLocalPlayer || (_privUserId.EndsWith("@steam") && !SrvRoles.DoNotTrack && !SrvRoles.SyncHashed))
            SyncedUserId = _privUserId;
        else
            SyncedUserId = global::Cryptography.Sha.HashToString(global::Cryptography.Sha.Sha512(_privUserId));
        RunUserIdHookIfServerOnly();
    }

    // Mirror's generated setter invokes the hook on the host ONLY when the object is
    // already present in NetworkClient.spawned — which is not yet the case when the
    // server assigns SyncedUserId during the player's Start frame. On a dedicated
    // server hooks never auto-fire at all. Either way InstanceMode would silently stay
    // Unverified (empty Tab list, empty RA list, broken lobby ready count), so after
    // every server-side assignment we invoke the hook manually. Both UserIdHook and
    // the InstanceMode setter are idempotent, so a duplicate hook call is harmless.
    private void RunUserIdHookIfServerOnly()
    {
        if (global::Mirror.NetworkServer.active)
            UserIdHook(string.Empty, SyncedUserId);
    }

    private void Awake()
    {
        SrvRoles = GetComponent<ServerRoles>();
        if (base.isLocalPlayer)
            CustomLiteNetLib4MirrorTransport.ResetRedirectCounter();
    }

    private void Start()
    {
        _commandRateLimit = Hub.playerRateLimitHandler.RateLimits[2];
        if (base.isLocalPlayer)
            Pastebin = global::GameCore.ConfigFile.ServerConfig.GetString("serverinfo_pastebin_id");
        if (!string.IsNullOrEmpty(UserId))
            UserIdHook(string.Empty, UserId);
        _centralAuthInt = new CentralAuthInterface(Hub, base.isServer);
        Connection = base.connectionToClient;
        if (base.isLocalPlayer)
        {
            _noclipKey = NewInput.GetKey((ActionName)18, default);
            _noclipFogToggleKey = NewInput.GetKey((ActionName)28, default);
        }
        StartCoroutine(Init());
        if (base.isLocalPlayer && !ServerStatic.IsDedicated)
            CentralAuth.singleton.GenerateToken(_centralAuthInt);
    }

    private void Update()
    {
        if (base.isLocalPlayer)
            MaxPlayers = (byte)global::Mirror.NetworkManager.singleton.maxConnections;
    }

    public void UserIdHook(string p, string i)
    {
        CharacterClassManager.OnSyncedUserIdAssigned?.Invoke(Hub);
        if (string.IsNullOrEmpty(i))
            InstanceMode = ClientInstanceMode.Unverified;
        else if (i == "ID_Dedicated")
            InstanceMode = ClientInstanceMode.DedicatedServer;
        else if (i == "ID_Host")
            InstanceMode = ClientInstanceMode.Host;
        else
            InstanceMode = ClientInstanceMode.ReadyClient;
    }

    [TargetRpc]
    internal void TargetSetRealId(global::Mirror.NetworkConnection conn, string userId)
    {
        RealUserId = userId;
        PlayerList.RefreshPlayerId(gameObject);
    }

    public void SyncServerCmdBinding()
    {
        if (!base.isServer || !EnableSyncServerCmdBinding) return;

        foreach (CmdBinding.Bind binding in CmdBinding.Bindings)
        {
            if (binding.command.StartsWith(".") || binding.command.StartsWith("/"))
            {
                TargetChangeCmdBinding(base.connectionToClient, binding.key, binding.command);
            }
        }
    }

    [TargetRpc]
    public void TargetChangeCmdBinding(NetworkConnection connection, KeyCode code, string cmd)
    {
        CmdBinding.ChangeKeybinding(code, cmd);
    }

    private void ConsolePrint(string text, string color)
    {
        Hub.gameConsoleTransmission.SendToClient(base.connectionToClient, text, color);
    }

    private global::System.Collections.IEnumerator Init()
    {
        if (global::Mirror.NetworkServer.active)
        {
            if (base.isLocalPlayer)
            {
                ServerLogs.StartLogging();
            }
            if (OnlineMode && !base.isLocalPlayer)
            {
                float timeout = 0f;
                while (true)
                {
                    timeout += global::MEC.Timing.DeltaTime;
                    yield return null;
                    if (!string.IsNullOrEmpty(UserId))
                    {
                        break;
                    }
                    if (!(timeout < 45f))
                    {
                        ServerConsole.Disconnect(base.connectionToClient, "Your client has failed to authenticate in time.");
                        yield break;
                    }
                }
            }
            else if (base.isLocalPlayer)
            {
                UserId = (ServerStatic.IsDedicated ? "ID_Dedicated" : "ID_Host");
            }
            else
            {
                _privUserId = $"OFFLINE_MODE_{base.netId}_{(global::System.DateTimeOffset.Now.ToUnixTimeSeconds())}";
                SyncedUserId = _privUserId;
                RunUserIdHookIfServerOnly();
            }
        }
        while (!ReferenceHub.TryGetHostHub(out ReferenceHub hub))
        {
            yield return null;
        }
        if (!base.isLocalPlayer || !global::Mirror.NetworkServer.active)
        {
            yield break;
        }
        FriendlyFireConfig.PauseDetector = false;
        CustomLiteNetLib4MirrorTransport.DelayConnections = false;
        IdleMode.PauseIdleMode = false;
        ServerConsole.AddOutputEntry(default(global::ServerOutput.RoundRestartedEntry));
        if (ServerStatic.IsDedicated)
        {
            ServerConsole.AddLog("Waiting for players...");
        }
        if (NonFacilityCompatibility.currentSceneSettings.roundAutostart)
        {
            ForceRoundStart();
        }
        else
        {
            global::GameCore.RoundStart.singleton.ShowButton();
            short originalTimeLeft = global::GameCore.ConfigFile.ServerConfig.GetShort("lobby_waiting_time", 20);
            short timeLeft = originalTimeLeft;
            int topPlayers = 2;
            while (global::GameCore.RoundStart.singleton.Timer != -1)
            {
                if (timeLeft == -2)
                {
                    timeLeft = originalTimeLeft;
                }
                int num = global::System.Linq.Enumerable.Count(ReferenceHub.AllHubs, (ReferenceHub x) => x.Mode == ClientInstanceMode.ReadyClient);
                if (!global::GameCore.RoundStart.LobbyLock && num > 1)
                {
                    if (num > topPlayers)
                    {
                        topPlayers = num;
                        if (timeLeft < originalTimeLeft)
                        {
                            do
                            {
                                timeLeft++;
                            }
                            while (timeLeft % 5 == 0 && timeLeft < originalTimeLeft);
                        }
                    }
                    else
                    {
                        timeLeft--;
                    }
                    if (num >= ((CustomNetworkManager)global::Mirror.NetworkManager.singleton).ReservedMaxPlayers)
                    {
                        timeLeft = -1;
                    }
                    if (timeLeft == -1)
                    {
                        ForceRoundStart();
                    }
                }
                else
                {
                    timeLeft = -2;
                }
                if (global::GameCore.RoundStart.singleton.Timer != -1)
                {
                    global::GameCore.RoundStart.singleton.Timer = timeLeft;
                }
                yield return new global::UnityEngine.WaitForSeconds(1f);
            }
        }

        RoundStarted = true;
        OnRoundStarted?.Invoke();
        RpcRoundStarted();
    }


    [global::Mirror.Command(channel = 4)]
    public void CmdSendToken(string token)
    {
        if (_commandtokensent && !base.isLocalPlayer)
        {
            ServerConsole.Disconnect(base.connectionToClient, "Your client sent second authentication token.");
            return;
        }
        if (OnlineMode)
        {
            if (string.IsNullOrEmpty(token) || _commandtokensent)
            {
                if (!base.isLocalPlayer || !base.isServer)
                {
                    ServerConsole.Disconnect(base.connectionToClient, "Your client sent an empty authentication token. Make sure you are running the game by steam.");
                    return;
                }
            }
            else if (!base.isLocalPlayer || !base.isServer)
            {
                if (token.StartsWith("ERROR: "))
                {
                    ServerConsole.AddLog("Player from IP " + base.connectionToClient.address + " kicked due to authentication error: " + token.Substring(7));
                    ServerLogs.AddLog(ServerLogs.Modules.Networking, "Player from IP " + base.connectionToClient.address + " kicked due to authentication error: " + token.Substring(7), ServerLogs.ServerLogType.ConnectionUpdate);
                    ServerConsole.Disconnect(base.connectionToClient, "Error during authentication: " + token.Substring(7));
                }
                else
                {
                    CentralAuth.singleton.StartValidateToken(_centralAuthInt, token, global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorServer.Peers[base.connectionToClient.connectionId].EndPoint);
                    AuthToken = token;
                }
            }
            else
            {
                if (token.StartsWith("ERROR: "))
                    global::GameCore.Console.AddLog("Error during authentication: " + token.Substring(7), global::UnityEngine.Color.red);
                else
                    CentralAuth.singleton.StartValidateToken(_centralAuthInt, token, null);
                AuthToken = token;
            }
        }
        _commandtokensent = true;
    }

    [global::Mirror.Command(channel = 4)]
    public void CmdRequestContactEmail()
    {
        if (_commandRateLimit.CanExecute())
        {
            if (SrvRoles.RemoteAdmin || SrvRoles.Staff)
                ConsolePrint("Contact email address: " + global::GameCore.ConfigFile.ServerConfig.GetString("contact_email"), "green");
            else
                ConsolePrint("You don't have permissions to execute this command.", "red");
        }
    }

    [global::Mirror.Command(channel = 4)]
    public void CmdRequestServerConfig()
    {
        if (_commandRateLimit.CanExecute())
        {
            YamlConfig serverConfig = global::GameCore.ConfigFile.ServerConfig;
            if (base.isLocalPlayer || SrvRoles.Staff || SrvRoles.RaEverywhere || PermissionsHandler.IsPermitted(SrvRoles.Permissions, PlayerPermissions.ServerConsoleCommands | PlayerPermissions.ServerConfigs))
                ConsolePrint("Extended server configuration:\nServer name: " + serverConfig.GetString("server_name") + "\nServer IP: " + serverConfig.GetString("server_ip") + "\nCurrent Server IP: " + CustomNetworkManager.singleton.networkAddress + "\nServer port: " + global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port + "\nServer pastebin ID: " + serverConfig.GetString("serverinfo_pastebin_id") + "\nServer max players: " + serverConfig.GetInt("max_players") + "\nOnline mode: " + OnlineMode.ToString() + "\nRA password authentication: " + GetComponent<global::RemoteAdmin.QueryProcessor>().OverridePasswordEnabled.ToString() + "\nIP banning: " + serverConfig.GetBool("ip_banning").ToString() + "\nWhitelist: " + serverConfig.GetBool("enable_whitelist").ToString() + "\nQuery status: " + serverConfig.GetBool("enable_query").ToString() + " with port shift " + serverConfig.GetInt("query_port_shift") + "\nFriendly fire: " + ServerConsole.FriendlyFire.ToString() + "\nMap seed: " + serverConfig.GetInt("map_seed"), "green");
            else
                ConsolePrint("Basic server configuration:\nServer name: " + serverConfig.GetString("server_name") + "\nServer pastebin ID: " + serverConfig.GetString("serverinfo_pastebin_id") + "\nServer max players: " + serverConfig.GetInt("max_players") + "\nRA password authentication: " + GetComponent<global::RemoteAdmin.QueryProcessor>().OverridePasswordEnabled.ToString() + "\nOnline mode: " + OnlineMode.ToString() + "\nWhitelist: " + serverConfig.GetBool("enable_whitelist").ToString() + "\nFriendly fire: " + ServerConsole.FriendlyFire.ToString() + "\nFriendly fire: " + ServerConsole.FriendlyFire.ToString(), "green");
        }
    }

    [global::Mirror.Command(channel = 4)]
    public void CmdRequestServerGroups()
    {
        if (!_commandRateLimit.CanExecute())
            return;
        string text = "Groups defined on this server:";
        global::System.Collections.Generic.Dictionary<string, UserGroup> allGroups = ServerStatic.PermissionsHandler.GetAllGroups();
        ServerRoles.NamedColor[] namedColors = SrvRoles.NamedColors;
        foreach (global::System.Collections.Generic.KeyValuePair<string, UserGroup> permentry in allGroups)
        {
            try
            {
                if (namedColors != null)
                    text = text + "\n" + permentry.Key + " (" + permentry.Value.Permissions + ") - <color=#" + global::System.Linq.Enumerable.FirstOrDefault(namedColors, (ServerRoles.NamedColor x) => x.Name == permentry.Value.BadgeColor)?.ColorHex + ">" + permentry.Value.BadgeText + "</color> in color " + permentry.Value.BadgeColor;
            }
            catch
            {
                text = text + "\n" + permentry.Key + " (" + permentry.Value.Permissions + ") - " + permentry.Value.BadgeText + " in color " + permentry.Value.BadgeColor;
            }
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.KickingAndShortTermBanning)) text += " BN1";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.BanningUpToDay)) text += " BN2";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.LongTermBanning)) text += " BN3";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ForceclassSelf)) text += " FSE";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ForceclassToSpectator)) text += " FSP";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ForceclassWithoutRestrictions)) text += " FWR";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.GivingItems)) text += " GIV";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.WarheadEvents)) text += " EWA";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.RespawnEvents)) text += " ERE";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.RoundEvents)) text += " ERO";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.SetGroup)) text += " SGR";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.GameplayData)) text += " GMD";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.Overwatch)) text += " OVR";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.FacilityManagement)) text += " FCM";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.PlayersManagement)) text += " PLM";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.PermissionsManagement)) text += " PRM";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ServerConsoleCommands)) text += " SCC";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ViewHiddenBadges)) text += " VHB";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ServerConfigs)) text += " CFG";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.Broadcasting)) text += " BRC";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.PlayerSensitiveDataAccess)) text += " CDA";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.Noclip)) text += " NCP";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.AFKImmunity)) text += " AFK";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.AdminChat)) text += " ATC";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.ViewHiddenGlobalBadges)) text += " GHB";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.Announcer)) text += " ANN";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.Effects)) text += " EFF";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.FriendlyFireDetectorImmunity)) text += " FFI";
            if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, PlayerPermissions.FriendlyFireDetectorTempDisable)) text += " FFT";
        }
        ConsolePrint("Defined groups on server " + text, "grey");
    }

    [global::Mirror.Command(channel = 4)]
    public void CmdRequestHideTag()
    {
        if (_commandRateLimit.CanExecute())
        {
            if (!string.IsNullOrEmpty(SrvRoles.HiddenBadge))
            {
                ConsolePrint("Your badge is already hidden.", "yellow");
                return;
            }
            if (string.IsNullOrEmpty(SrvRoles.MyText))
            {
                ConsolePrint("You don't have a badge.", "red");
                return;
            }
            SrvRoles.GlobalHidden = SrvRoles.GlobalSet;
            SrvRoles.HiddenBadge = SrvRoles.MyText;
            SrvRoles.GlobalBadge = null;
            SrvRoles.SetText(null);
            SrvRoles.SetColor(null);
            SrvRoles.RefreshHiddenTag();
            ConsolePrint("Badge hidden.", "green");
        }
    }

    [global::Mirror.Command(channel = 4)]
    public void CmdRequestShowTag(bool global)
    {
        if (!_commandRateLimit.CanExecute())
            return;
        if (global)
        {
            if (string.IsNullOrEmpty(SrvRoles.PrevBadge))
            {
                ConsolePrint("You don't have a global tag.", "magenta");
                return;
            }
            if ((string.IsNullOrEmpty(SrvRoles.MyText) || !SrvRoles.RemoteAdmin) && (((SrvRoles.GlobalBadgeType == 3 || SrvRoles.GlobalBadgeType == 4) && global::GameCore.ConfigFile.ServerConfig.GetBool("block_gtag_banteam_badges") && !ServerStatic.PermissionsHandler.IsVerified) || (SrvRoles.GlobalBadgeType == 1 && global::GameCore.ConfigFile.ServerConfig.GetBool("block_gtag_staff_badges")) || (SrvRoles.GlobalBadgeType == 2 && global::GameCore.ConfigFile.ServerConfig.GetBool("block_gtag_management_badges") && !ServerStatic.PermissionsHandler.IsVerified) || (SrvRoles.GlobalBadgeType == 0 && global::GameCore.ConfigFile.ServerConfig.GetBool("block_gtag_patreon_badges") && !ServerStatic.PermissionsHandler.IsVerified)))
            {
                ConsolePrint("You can't show this type of global badge on this server. Try joining server with global badges allowed.", "red");
                return;
            }
            SrvRoles.GlobalBadge = SrvRoles.PrevBadge;
            SrvRoles.GlobalHidden = false;
            SrvRoles.HiddenBadge = null;
            SrvRoles.RpcResetFixed();
            ConsolePrint("Global tag refreshed.", "green");
        }
        else
        {
            SrvRoles.GlobalBadge = null;
            SrvRoles.HiddenBadge = null;
            SrvRoles.RpcResetFixed();
            SrvRoles.RefreshPermissions(disp: true);
            ConsolePrint("Local tag refreshed.", "green");
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdForceRoundStart()
    {
        ForceRoundStart();
    }

    public static bool ForceRoundStart()
    {
        if (!NetworkServer.active) return false;

        ServerLogs.AddLog(ServerLogs.Modules.Logger, "Round has been started.", ServerLogs.ServerLogType.GameEvent);
        ServerConsole.AddLog("New round has been started.", ConsoleColor.Gray);

        RoundStart.singleton.Timer = -1;
        RoundStart.RoundStartTimer.Restart();
        return true;
    }

    [TargetRpc]
    private void TargetSetDisconnectError(NetworkConnection conn, string message)
    {
        if (LiteNetLib4MirrorNetworkManager.singleton is CustomNetworkManager manager)
        {
            manager.disconnectMessage = message;
        }
        CmdConfirmDisconnect();
    }

    [Command(channel = 4)]
    private void CmdConfirmDisconnect()
    {
        base.connectionToClient?.Disconnect();
    }

    public void DisconnectClient(NetworkConnection conn, string message)
    {
        TargetSetDisconnectError(conn, message);
        Timing.RunCoroutine(_DisconnectAfterTimeout(conn), Segment.FixedUpdate);
    }

    private static IEnumerator<float> _DisconnectAfterTimeout(NetworkConnection conn)
    {
        for (int i = 0; i < 150; i++)
        {
            yield return Timing.WaitForOneFrame;
        }
        conn?.Disconnect();
    }

    [global::Mirror.ClientRpc]
    private void RpcRoundStarted()
    {
        OnRoundStarted?.Invoke();
        Debug.Log("Round started event invoked.");
    }
}