using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using GameCore;
using MEC;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils.NonAllocLINQ;

public class CustomNetworkManager : LiteNetLib4MirrorNetworkManager
{
    [Serializable]
    public class DisconnectLog
    {
        [Serializable]
        public class LogButton
        {
            public ConnInfoButton[] actions;
        }

        [Multiline]
        public string msg_en;

        public LogButton button;

        public bool autoHideOnSceneLoad;
    }

    public static readonly HashSet<Func<CustomNetworkManager, bool>> TryStartClientChecks = new HashSet<Func<CustomNetworkManager, bool>>();

    [SerializeField]
    private GameObject popup;

    [SerializeField]
    private GameObject createPopForce;

    [SerializeField]
    private GameObject loadingpop;

    public GameObject createpop;

    public RectTransform contSize;

    public Text content;

    public TextMeshProUGUI loading_title;

    private static QueryServer _queryserver;

    public DisconnectLog[] logs;

    private int _curLogId;

    private int _queryPort;

    internal static bool reconnecting;

    internal static float reconnectTime;

    internal static float triggerReconnectTime;

    private CoroutineHandle _reconnectHandle;

    private static byte _challengeRetries;

    private static byte _preauthReconnectAttempts;
    private const byte MaxPreauthReconnectAttempts = 8;

    private bool _queryEnabled;

    private bool _configLoaded;

    private bool _activated;

    private float _dictCleanupTime;

    private float _ipRateLimitTime;

    private float _userIdRateLimitTime;

    private float _preauthChallengeTime;

    private float _delayVolumeResetTime;

    private float _rejectSuppressionTime;

    private float _issuedSuppressionTime;

    private bool _disconnectDrop;

    private bool _cancelSceneLoading;

    private static readonly int[] _loadingLogId = new int[4] { 13, 14, 17, 33 };

    private readonly HashSet<IPEndPoint> _dictToRemove = new HashSet<IPEndPoint>();

    private readonly HashSet<string> _dict2ToRemove = new HashSet<string>();

    private static ushort _ipRateLimitWindow;

    private static ushort _userIdLimitWindow;

    private static ushort _preauthChallengeWindow;

    private static ushort _preauthChallengeClean;

    public string disconnectMessage = "";

    public static string ConnectionIp;

    public static string LastIp;

    [Space(30f)]
    public int GameFilesVersion;

    public static bool Modded = false;

    private static readonly int _expectedGameFilesVersion = 4;

    public static int slots;

    public static int reservedSlots;

    public static bool EnableFastRestart = true;

    public static float FastRestartDelay = 3.2f;

    private const int IpRetryDelay = 180;
    public static GameObject CreatePop => TypedSingleton.createpop;
    public static CustomNetworkManager TypedSingleton => (CustomNetworkManager)LiteNetLib4MirrorNetworkManager.singleton;

    public int MaxPlayers
    {
        get
        {
            return maxConnections;
        }
        set
        {
            maxConnections = value;
            LiteNetLib4MirrorTransport.Singleton.maxConnections = (ushort)value;
        }
    }

    public int ReservedMaxPlayers => slots;

    public static bool IsVerified { get; internal set; }

    public static event Action OnClientReady;

    public static event Action OnClientStarted;

    public static event Action OnServerStarted;

    public override void Update()
    {
        base.Update();

        if (TypedSingleton == this && gameObject.scene.name != "DontDestroyOnLoad")
        {
            DontDestroyOnLoad(gameObject);
        }

        if (!popup.activeSelf && !loadingpop.activeSelf)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            reconnecting = false;
            triggerReconnectTime = 0f;
            reconnectTime = 0f;
            _cancelSceneLoading = true;

            if (loadingpop.activeSelf)
            {
                ShowLoadingScreen(4);
            }

            if (_curLogId < logs.Length)
            {
                var actions = logs[_curLogId].button?.actions;
                if (actions != null)
                {
                    foreach (var btn in actions)
                    {
                        btn.UseButton();
                    }
                }
            }

            SteamLobby.singleton.LeaveLobby();
        }
    }

    private void FixedUpdate()
    {
        if (!NetworkServer.active)
        {
            if (triggerReconnectTime > 0f)
            {
                triggerReconnectTime -= Time.fixedUnscaledDeltaTime;
                if (triggerReconnectTime <= 0f)
                {
                    triggerReconnectTime = 0f;
                    Reconnect();
                }
            }
            return;
        }

        _dictCleanupTime += Time.fixedUnscaledDeltaTime;
        _ipRateLimitTime += Time.fixedUnscaledDeltaTime;
        _userIdRateLimitTime += Time.fixedUnscaledDeltaTime;
        _preauthChallengeTime += Time.fixedUnscaledDeltaTime;
        _delayVolumeResetTime += Time.fixedUnscaledDeltaTime;
        _rejectSuppressionTime += Time.fixedUnscaledDeltaTime;
        _issuedSuppressionTime += Time.fixedUnscaledDeltaTime;

        if (_ipRateLimitTime >= (float)(int)_ipRateLimitWindow)
        {
            _ipRateLimitTime = 0f;
            CustomLiteNetLib4MirrorTransport.IpRateLimit.Clear();
        }
        if (_userIdRateLimitTime >= (float)(int)_userIdLimitWindow)
        {
            _userIdRateLimitTime = 0f;
            CustomLiteNetLib4MirrorTransport.UserRateLimit.Clear();
        }
        if (_delayVolumeResetTime > 5.5f)
        {
            _delayVolumeResetTime = 0f;
            CustomLiteNetLib4MirrorTransport.DelayVolume = 0;
        }
        if (_rejectSuppressionTime > 10f)
        {
            _rejectSuppressionTime = 0f;
            if (CustomLiteNetLib4MirrorTransport.SuppressRejections)
            {
                if (CustomLiteNetLib4MirrorTransport.Rejected <= CustomLiteNetLib4MirrorTransport.RejectionThreshold)
                {
                    CustomLiteNetLib4MirrorTransport.SuppressRejections = false;
                }
                ServerConsole.AddLog($"{CustomLiteNetLib4MirrorTransport.Rejected} incoming connections have been rejected within the last 10 seconds.", ConsoleColor.Yellow);
            }
            CustomLiteNetLib4MirrorTransport.Rejected = 0u;
        }
        if (_issuedSuppressionTime > 10f)
        {
            _issuedSuppressionTime = 0f;
            if (CustomLiteNetLib4MirrorTransport.SuppressIssued)
            {
                if (CustomLiteNetLib4MirrorTransport.ChallengeIssued <= CustomLiteNetLib4MirrorTransport.IssuedThreshold)
                {
                    CustomLiteNetLib4MirrorTransport.SuppressIssued = false;
                }
                ServerConsole.AddLog($"{CustomLiteNetLib4MirrorTransport.ChallengeIssued} challenges have been requested within the last 10 seconds.", ConsoleColor.Yellow);
            }
            CustomLiteNetLib4MirrorTransport.ChallengeIssued = 0u;
        }
        if (_preauthChallengeTime >= (float)(int)_preauthChallengeClean)
        {
            _preauthChallengeTime = 0f;
            long ticks = DateTime.Now.AddSeconds(_preauthChallengeWindow * -1).Ticks;
            foreach (KeyValuePair<string, PreauthChallengeItem> challenge in CustomLiteNetLib4MirrorTransport.Challenges)
            {
                if (challenge.Value.Added <= ticks)
                {
                    _dict2ToRemove.Add(challenge.Key);
                }
            }
            foreach (string item in _dict2ToRemove)
            {
                if (CustomLiteNetLib4MirrorTransport.Challenges.ContainsKey(item))
                {
                    CustomLiteNetLib4MirrorTransport.Challenges.Remove(item);
                }
            }
            _dict2ToRemove.Clear();
        }
        if (_dictCleanupTime <= 20f)
        {
            return;
        }
        _dictCleanupTime = 0f;
        long ticks2 = DateTime.Now.AddSeconds(-200.0).Ticks;
        foreach (KeyValuePair<IPEndPoint, PreauthItem> userId in CustomLiteNetLib4MirrorTransport.UserIds)
        {
            if (userId.Value.Added <= ticks2)
            {
                _dictToRemove.Add(userId.Key);
            }
        }
        foreach (IPEndPoint item2 in _dictToRemove)
        {
            if (CustomLiteNetLib4MirrorTransport.UserIds.ContainsKey(item2))
            {
                CustomLiteNetLib4MirrorTransport.UserIds.Remove(item2);
            }
        }
        _dictToRemove.Clear();
    }

    internal static void InvokeOnClientReady()
    {
        CustomNetworkManager.OnClientReady?.Invoke();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        CustomLiteNetLib4MirrorTransport.ResetClientChallenge();
        _challengeRetries = 0;
        _preauthReconnectAttempts = 0;
        reconnecting = false;
        CustomLiteNetLib4MirrorTransport.LastRejectionReason = RejectionReason.NotSpecified;
        CustomLiteNetLib4MirrorTransport.LastCustomReason = string.Empty;
        CustomLiteNetLib4MirrorTransport.LastBanExpiration = 0L;
        CustomNetworkManager.OnClientReady?.Invoke();
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (string.IsNullOrEmpty(newSceneName))
        {
            Debug.LogError("ServerChangeScene empty scene name");
            return;
        }
        if (NetworkServer.isLoadingScene && newSceneName == NetworkManager.networkSceneName)
        {
            Debug.LogError("Scene change is already in progress for " + newSceneName);
            return;
        }
        NetworkServer.SetAllClientsNotReady();
        NetworkManager.networkSceneName = newSceneName;
        OnServerChangeScene(newSceneName);
        NetworkServer.isLoadingScene = true;
        NetworkManager.loadingSceneAsync = SceneManager.LoadSceneAsync(newSceneName);
        if (NetworkServer.active)
        {
            if (EnableFastRestart)
            {
                Timing.CallDelayed(FastRestartDelay, delegate
                {
                    NetworkServer.SendToAll(new SceneMessage
                    {
                        sceneName = newSceneName
                    });
                });
            }
            else
            {
                NetworkServer.SendToAll(new SceneMessage
                {
                    sceneName = newSceneName
                });
            }
        }
        NetworkManager.startPositionIndex = 0;
        NetworkManager.startPositions.Clear();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        SteamManager.CancelTicket();

        LiteNetLib.DisconnectReason reason = LiteNetLib4MirrorCore.LastDisconnectReason;

        if (reason == LiteNetLib.DisconnectReason.ConnectionFailed ||
            reason == LiteNetLib.DisconnectReason.Timeout ||
            reason == LiteNetLib.DisconnectReason.HostUnreachable ||
            reason == LiteNetLib.DisconnectReason.NetworkUnreachable)
        {
            PrintConnectionDebug(reason.ToString());

            RejectionReason rejection = CustomLiteNetLib4MirrorTransport.LastRejectionReason;
            if (rejection != RejectionReason.NotSpecified)
            {
                PrintConnectionDebug("Preauthentication Error: " + rejection);
            }
        }

        string lastCustomReason = CustomLiteNetLib4MirrorTransport.LastCustomReason;
        string sanitizedReason = SanitizeRejectionReason(lastCustomReason);

        if (!string.IsNullOrWhiteSpace(sanitizedReason))
        {
            if (sanitizedReason.Length > 400)
                sanitizedReason = sanitizedReason.Substring(0, 400);

            DateTime banExpiration = DateTime.FromBinary(CustomLiteNetLib4MirrorTransport.LastBanExpiration).ToLocalTime();

            if (banExpiration > DateTime.Now)
            {
                ShowLog(32, banExpiration.ToShortDateString(), banExpiration.ToLongTimeString(), sanitizedReason);
            }
            else
            {
                ShowLog(29, sanitizedReason, "", "");
            }
            return;
        }

        if (!string.IsNullOrWhiteSpace(CentralAuthManager.GlobalBanReason))
        {
            ShowLog(8, CentralAuthManager.GlobalBanReason, "", "");
            return;
        }

        string serverReason = SanitizeRejectionReason(Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorClient.LastDisconnectReason);
        if (!string.IsNullOrWhiteSpace(serverReason))
        {
            if (serverReason.Length > 400)
                serverReason = serverReason.Substring(0, 400);
            ShowLog(29, serverReason, "", "");
            return;
        }

        RejectionReason lastRejection = CustomLiteNetLib4MirrorTransport.LastRejectionReason;

        if (lastRejection == RejectionReason.Challenge || lastRejection == RejectionReason.Delay)
        {
            if (_preauthReconnectAttempts >= MaxPreauthReconnectAttempts)
            {
                _preauthReconnectAttempts = 0;
                CustomLiteNetLib4MirrorTransport.ResetClientChallenge();
                ShowLog(34, "", "", "");
                return;
            }
            _preauthReconnectAttempts++;
            reconnecting = true;
            float initialDelay = (lastRejection == RejectionReason.Delay) ? Mathf.Max(triggerReconnectTime, 0.1f) : 0.1f;
            triggerReconnectTime = 0f;
            reconnectTime = 0f;
            Timing.KillCoroutines(_reconnectHandle);
            _reconnectHandle = Timing.RunCoroutine(_ReconnectAfterPreauth(initialDelay, lastRejection == RejectionReason.Delay));
            return;
        }

        // Stale/expired challenge id — the transport already cleared the local challenge state
        // (ResetClientChallenge), so retry once with a fresh handshake before giving up.
        if (lastRejection == RejectionReason.InvalidChallengeKey || lastRejection == RejectionReason.InvalidChallenge)
        {
            if (_challengeRetries < 2)
            {
                _challengeRetries++;
                reconnecting = true;
                triggerReconnectTime = 0f;
                reconnectTime = 0f;
                Timing.KillCoroutines(_reconnectHandle);
                _reconnectHandle = Timing.RunCoroutine(_ReconnectAfterPreauth(0.1f, isDelay: true));
                return;
            }
            _challengeRetries = 0;
        }

        switch (lastRejection)
        {
            case RejectionReason.ServerFull:
                ShowLog(4, "", "", "");
                break;

            case RejectionReason.VersionMismatch:
                ShowLog(9, "", "", "");
                break;

            case RejectionReason.InvalidToken:
            case RejectionReason.CentralServerAuthRejected:
                ShowLog(30, "", "", "");
                break;

            case RejectionReason.AuthenticationRequired:
                ShowLog(20, "", "", "");
                break;

            case RejectionReason.Banned:
                ShowLog(24, "", "", "");
                break;

            case RejectionReason.NotWhitelisted:
                ShowLog(21, "", "", "");
                break;

            case RejectionReason.GloballyBanned:
                ShowLog(22, "", "", "");
                break;

            case RejectionReason.Geoblocked:
                ShowLog(28, "", "", "");
                break;

            case RejectionReason.ExpiredAuth:
                ShowLog(26, "", "", "");
                break;

            case RejectionReason.RateLimit:
                ShowLog(27, "", "", "");
                break;

            case RejectionReason.InvalidChallengeKey:
            case RejectionReason.InvalidChallenge:
            case RejectionReason.VerificationRejected:
                ShowLog(34, "", "", "");
                break;
        }

        CustomLiteNetLib4MirrorTransport.ResetClientChallenge();

        if (popup.activeSelf) return;

        switch (reason)
        {
            case LiteNetLib.DisconnectReason.Timeout:
                ShowLog(6, "", "", "");
                break;
            case LiteNetLib.DisconnectReason.ConnectionFailed:
            case LiteNetLib.DisconnectReason.HostUnreachable:
            case LiteNetLib.DisconnectReason.NetworkUnreachable:
            case LiteNetLib.DisconnectReason.UnknownHost:
                ShowLog(1, "", "", "");
                break;
            case LiteNetLib.DisconnectReason.RemoteConnectionClose:
                ShowLog(0, "", "", "");
                break;
            case LiteNetLib.DisconnectReason.DisconnectPeerCalled:
                // Local, deliberate disconnect — no error popup.
                break;
            default:
                ShowLog(12, "", "", "");
                break;
        }
    }

    private static string SanitizeRejectionReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) return null;

        if (reason.Any(c => c < ' '))
        {
            reason = new string(reason.Where(c => c >= ' ').ToArray());
        }

        return reason.Replace("<", "(").Replace(">", ")");
    }

    private static void PrintConnectionDebug(string reason)
    {
        GameCore.Console.AddLog(reason, Color.red);
        GameCore.Console.AddLog("IP: " + LiteNetLib4MirrorTransport.Singleton.clientAddress, Color.red);
        GameCore.Console.AddLog("Port: " + LiteNetLib4MirrorTransport.Singleton.port, Color.red);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (ServerStatic.PermissionsHandler == null)
        {
            LoadServerPermissions();
        }
        CustomNetworkManager.OnServerStarted?.Invoke();
    }

    internal static void LoadServerPermissions()
    {
        string text = FileManager.GetAppFolder(addSeparator: true, serverConfig: true) + "config_remoteadmin.txt";
        if (!File.Exists(text))
        {
            File.Copy("ConfigTemplates/config_remoteadmin.template.txt", text);
        }
        ServerConsole.AddLog("Loading server permissions configuration...");
        ServerStatic.RolesConfigPath = text;
        ServerStatic.RolesConfig = new YamlConfig(text);
        ServerStatic.SharedGroupsConfig = (ConfigSharing.Paths[4] == null) ? null : new YamlConfig(ConfigSharing.Paths[4] + "shared_groups.txt");
        ServerStatic.SharedGroupsMembersConfig = (ConfigSharing.Paths[5] == null) ? null : new YamlConfig(ConfigSharing.Paths[5] + "shared_groups_members.txt");
        ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
        ServerConsole.AddLog("Server permissions configuration loaded.");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        _cancelSceneLoading = false;
        Timing.RunCoroutine(_ConnectToServer());
        CustomNetworkManager.OnClientStarted?.Invoke();
    }

    public new void StartClient()
    {
        bool allow = true;
        TryStartClientChecks.ForEach(delegate (Func<CustomNetworkManager, bool> x)
        {
            allow = x(this) && allow;
        });
        if (allow)
        {
            ShowLoadingScreen(0);
            base.StartClient();
        }
    }

    private IEnumerator<float> _ConnectToServer()
    {
        while (LiteNetLib4MirrorCore.State == LiteNetLib4MirrorCore.States.ClientConnecting || LiteNetLib4MirrorCore.State == LiteNetLib4MirrorCore.States.ClientConnected)
        {
            if (NetworkClient.isConnected)
            {
                ShowLoadingScreen(2);
                break;
            }
            yield return 0f;
        }
    }

    public bool IsFacilityLoading()
    {
        if (_curLogId != 17)
        {
            return _curLogId == 18;
        }
        return true;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (_disconnectDrop)
        {
            NetworkIdentity identity = conn.identity;
            if (identity != null && ReferenceHub.TryGetHubNetID(identity.netId, out var hub) && hub.IsAlive())
            {
                hub.playerStats.DealDamage(new UniversalDamageHandler(-1f, DeathTranslations.Unknown));
            }
        }
        if (CustomLiteNetLib4MirrorTransport.IpPassthroughEnabled)
        {
            int id = LiteNetLib4MirrorServer.Peers[conn.connectionId].Id;
            if (CustomLiteNetLib4MirrorTransport.RealIpAddresses.ContainsKey(id))
            {
                CustomLiteNetLib4MirrorTransport.RealIpAddresses.Remove(id);
            }
        }
        base.OnServerDisconnect(conn);
        conn.Disconnect();
    }



    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Facility")
        {
            if (!_cancelSceneLoading)
            {
                LoadingScreen.FinishedLoading();
            }
            else
            {
                GameCore.Console.AddLog("Scene loading cancelled.", Color.red);
                reconnecting = false;
                triggerReconnectTime = 0;
                StopClient();
            }
        }

        if (scene.name.Contains("menu", StringComparison.OrdinalIgnoreCase))
        {
            _curLogId = 0;
            if (!_activated)
            {
                _activated = true;
            }

            if (_cancelSceneLoading)
            {
                loadingpop.SetActive(false);
                return;
            }
        }

        if (!(reconnectTime <= 0f))
        {
            LoadingScreen.FinishedLoading();
            ShowLoadingScreen(1);
            Invoke("Reconnect", 3f);
        }
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        CustomNetworkManager.OnClientReady?.Invoke();
        if (reconnectTime <= 0f && logs[_curLogId].autoHideOnSceneLoad)
        {
            popup.SetActive(value: false);
            loadingpop.SetActive(value: false);
        }
    }

    public bool ShouldPlayIntensive()
    {
        if (_curLogId != 13)
        {
            return IsFacilityLoading();
        }
        return true;
    }

    private void Reconnect()
    {
        if (!(reconnectTime <= 0f))
        {
            reconnecting = true;
            CustomLiteNetLib4MirrorTransport.DelayConnections = true;
            IdleMode.PauseIdleMode = true;
            Invoke("TryConnecting", reconnectTime);
            reconnectTime = 0f;
        }
    }

    public void TryConnecting()
    {
        if (reconnecting)
        {
            StartClient();
        }
    }

    private IEnumerator<float> _ReconnectAfterPreauth(float initialDelay, bool isDelay)
    {
        yield return Timing.WaitForSeconds(initialDelay);

        float waited = 0f;
        while (CustomLiteNetLib4MirrorTransport.ClientChallengeState == ChallengeState.Processing && waited < 30f)
        {
            waited += Time.unscaledDeltaTime;
            yield return Timing.WaitForOneFrame;
        }

        if (!reconnecting)
        {
            yield break;
        }

        if (!isDelay && CustomLiteNetLib4MirrorTransport.ClientChallengeState == ChallengeState.None)
        {
            reconnecting = false;
            ShowLog(34, "", "", "");
            yield break;
        }

        StartClient();
    }

    public void StopReconnecting()
    {
        reconnecting = false;
        triggerReconnectTime = 0f;
        reconnectTime = 0f;
        Timing.KillCoroutines(_reconnectHandle);
    }

    public static void ResetPreauthReconnectState()
    {
        _challengeRetries = 0;
        _preauthReconnectAttempts = 0;
        CustomLiteNetLib4MirrorTransport.ResetClientChallenge();
    }

    public void ShowLog(int id, string obj1 = "", string obj2 = "", string obj3 = "", string textOverride = null)
    {
        _curLogId = id;
        popup.SetActive(true);
        loadingpop.SetActive(false);

        if (string.IsNullOrEmpty(textOverride))
        {
            if (string.IsNullOrEmpty(obj1))
            {
                content.text = TranslationReader.Get("Connection_Errors", id, "NO_TRANSLATION");
            }
            else if (string.IsNullOrEmpty(obj2))
            {
                content.text = TranslationReader.GetFormatted("Connection_Errors", id, "", obj1);
            }
            else if (string.IsNullOrEmpty(obj3))
            {
                content.text = TranslationReader.GetFormatted("Connection_Errors", id, "", obj1, obj2);
            }
            else
            {
                content.text = TranslationReader.GetFormatted("Connection_Errors", id, "", obj1, obj2, obj3);
            }

            if (!string.IsNullOrEmpty(disconnectMessage))
            {
                if (content.text.Contains(Environment.NewLine))
                {
                    string[] split = content.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    if (split.Length > 0)
                    {
                        content.text = split[0] + Environment.NewLine + disconnectMessage;
                    }
                }
                disconnectMessage = "";
            }
        }
        else
        {
            content.text = textOverride;
        }

        content.rectTransform.sizeDelta = Vector2.zero;
    }

    public void ShowLoadingScreen(int id)
    {
        if (id < _loadingLogId.Length)
        {
            _curLogId = _loadingLogId[id];
            loading_title.SetText(TranslationReader.Get("Connection_Info", id, "NO_TRANSLATION"));
            LoadingScreen comp = GetComponent<LoadingScreen>();
            if (comp != null) comp.description = TranslationReader.Get("Connection_Info_Desc", id, "");
        }

        popup.SetActive(false);
        loadingpop.SetActive(true);
    }

    public void ClickButton()
    {
        if (_curLogId < logs.Length)
        {
            var actions = logs[_curLogId].button.actions;
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].UseButton();
            }
        }
    }

    private void LoadConfigs(bool firstTime = false)
    {
        if (!_configLoaded)
        {
            _configLoaded = true;
            if (File.Exists("hoster_policy.txt"))
            {
                ConfigFile.HosterPolicy = new YamlConfig("hoster_policy.txt");
            }
            else if (File.Exists(FileManager.GetAppFolder(true, false, "") + "hoster_policy.txt"))
            {
                ConfigFile.HosterPolicy = new YamlConfig(FileManager.GetAppFolder(true, false, "") + "hoster_policy.txt");
            }
            else
            {
                ConfigFile.HosterPolicy = new YamlConfig();
            }
            FileManager.RefreshAppFolder();
            if (!ServerStatic.IsDedicated)
            {
                ServerConsole.AddLog("Loading configs...");
                ConfigFile.ReloadGameConfigs(firstTime);
                ServerConsole.AddLog("Config file loaded!");
            }
        }
    }

    public override void Start()
    {
        base.Start();
        LoadConfigs(firstTime: true);
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux && !File.Exists("/etc/ssl/certs/ca-certificates.crt"))
        {
            if (File.Exists("/etc/pki/tls/certs/ca-bundle.crt"))
            {
                ServerConsole.AddLog("System CA Cert store not available! Unity expects it to be in /etc/ssl/certs/ca-certificates.crt, but we've detected it's present in /etc/pki/tls/certs/ca-bundle.crt on your system, please symlink your store to the required location!");
            }
            else if (File.Exists("/etc/ssl/ca-bundle.pem"))
            {
                ServerConsole.AddLog("System CA Cert store not available! Unity expects it to be in /etc/ssl/certs/ca-certificates.crt, but we've detected it's present in /etc/ssl/ca-bundle.pem on your system, please symlink your store to the required location!");
            }
            else if (File.Exists("/etc/pki/tls/cacert.pem"))
            {
                ServerConsole.AddLog("System CA Cert store not available! Unity expects it to be in /etc/ssl/certs/ca-certificates.crt, but we've detected it's present in /etc/pki/tls/cacert.pem on your system, please symlink your store to the required location!");
            }
            else if (File.Exists("/etc/pki/ca-trust/extracted/pem/tls-ca-bundle.pem"))
            {
                ServerConsole.AddLog("System CA Cert store not available! Unity expects it to be in /etc/ssl/certs/ca-certificates.crt, but we've detected it's present in /etc/pki/ca-trust/extracted/pem/tls-ca-bundle.pem on your system, please symlink your store to the required location!");
            }
            else
            {
                ServerConsole.AddLog("System CA Cert store not available! Unity expects it to be in /etc/ssl/certs/ca-certificates.crt and we couldn't detect its location! Please provide access to it in the specified path!");
            }
        }

        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        string userId = GetUserId();

        if (userId != null && Directory.Exists("SCPSL_Data"))
        {
            string path = Path.Combine("SCPSL_Data", "Resources", "unity_builtin_data");
            if (File.Exists(path))
            {
                string[] lines = FileManager.ReadAllLines(path);
                if (lines.Length > 0)
                {
                    string content = lines[0].Replace("-", "");
                    string decoded = NorthwoodLib.StringUtils.Base64Decode(content);

                    if (decoded.Contains(";"))
                    {
                        string[] parts = decoded.Split(';');
                        if (parts.Length == 3)
                        {
                            if (parts[0] != GameCore.Version.VersionString || parts[2] != userId)
                            {
                                string newContent = parts[0] + ";" + (parts[1] != null ? parts[1] : "") + ";" + userId;
                                string encoded = NorthwoodLib.StringUtils.Base64Encode(newContent);
                                string splitGuid = GUIDSplit(encoded);
                                File.WriteAllText(path, splitGuid);
                                GameCore.Console.AddLog("Updated version file identity.", Color.red);
                            }
                        }
                        else
                        {
                            CreateVersionFile(false, userId, path);
                        }
                    }
                    else
                    {
                        CreateVersionFile(false, userId, path);
                    }
                }
            }
            else
            {
                CreateVersionFile(false, userId, path);
            }
        }

    }

    private string GUIDSplit(string GUID)
    {
        string result = "";
        string temp = GUID;
        while (temp.Length > 5)
        {
            string chunk = temp.Substring(0, 5);
            result += chunk + "-";
            temp = temp.Substring(5);
        }
        result += temp;
        return result;
    }

    private void CreateVersionFile(bool privbeta, string id, string path)
    {
        string content = GameCore.Version.VersionString + ";" + id + ";-";
        string encoded = NorthwoodLib.StringUtils.Base64Encode(content);
        string formatted = GUIDSplit(encoded);
        File.WriteAllText(path, formatted);
        GameCore.Console.AddLog("Created version file.", Color.gray);
    }

    private static string GetUserId()
    {
        if (CentralAuthManager.Platform == DistributionPlatform.Steam)
        {
            return SteamManager.SteamId64.ToString();
        }
        /*
        else if (CentralAuthManager.Platform == DistributionPlatform.Discord)
        {
            // Reconstructed using implied types from Discord SDK usually present in this context
            return Discord.DiscordLoader.GetUserManager().GetCurrentUser().Id.ToString();
        }
        */
        return null;
    }

    public void CreateMatch()
    {
        ServerConsole.AddLog("Game version: " + GameCore.Version.VersionString);
        if (GameCore.Version.PrivateBeta)
        {
            ServerConsole.AddLog("PRIVATE BETA VERSION - DO NOT SHARE");
        }
        if (GameFilesVersion != _expectedGameFilesVersion)
        {
            ServerConsole.AddLog("This source code file is made for different version of the game!");
            ServerConsole.AddLog("Please validate game files integrity using steam!");
            ServerConsole.AddLog("Aborting server startup.");
            return;
        }

        if (SteamLobby.singleton.Lobby.Id.IsValid)
        {
            SteamLobby.singleton.LeaveLobby();
        }
        

        CustomLiteNetLib4MirrorTransport.DelayConnections = true;
        IdleMode.PauseIdleMode = true;
        LoadConfigs();
        ShowLoadingScreen(0);
        createpop.SetActive(value: false);
        ServerConsole.AddLog("Loading configs...");
        ConfigFile.ReloadGameConfigs();
        LiteNetLib4MirrorTransport.Singleton.port = (ServerStatic.IsDedicated ? ServerStatic.ServerPort : GetFreePort());
        LiteNetLib4MirrorTransport.Singleton.useUpnP = ConfigFile.ServerConfig.GetBool("forward_ports", def: true);
        slots = ConfigFile.ServerConfig.GetInt("max_players", 20);
        CustomLiteNetLib4MirrorTransport.DelayVolumeThreshold = (byte)(Mathf.Clamp(slots, 5, 125) * 2);
        reservedSlots = Mathf.Max(ConfigFile.ServerConfig.GetInt("reserved_slots", ReservedSlot.Users.Count), 0);
        _disconnectDrop = ConfigFile.ServerConfig.GetBool("disconnect_drop", def: true);
        MaxPlayers = (slots + reservedSlots) * 2 + 50;
        int num = ConfigFile.HosterPolicy.GetInt("players_limit", -1);
        if (num > 0 && slots + reservedSlots > num)
        {
            MaxPlayers = num * 2 + 50;
            ServerConsole.AddLog("You have exceeded players limit set by your hosting provider. Max players value set to " + num);
        }
        ServerConsole.AddLog("Config files loaded from " + FileManager.GetAppFolder(addSeparator: true, serverConfig: true));
        _queryEnabled = ConfigFile.ServerConfig.GetBool("enable_query");
        LoadServerPermissions();
        CustomLiteNetLib4MirrorTransport.UseGlobalBans = ConfigFile.ServerConfig.GetBool("use_global_bans", def: true);
        CustomLiteNetLib4MirrorTransport.ReloadChallengeOptions();
        ServerConsole.AddLog(ConfigFile.ServerConfig.GetBool("online_mode") ? "Online mode is ENABLED." : "Online mode is DISABLED - SERVER CANNOT VALIDATE USER ID OF CONNECTING PLAYERS!!! Features like User ID admin authentication won't work.");
        ServerConsole.RunRefreshPublicKey();
        ServerConsole.AddLog("Starting server...");
        Timing.RunCoroutine(_CreateLobby());

    }

    internal static void ReloadTimeWindows()
    {
        _ipRateLimitWindow = ConfigFile.ServerConfig.GetUShort("ip_ratelimit_window", 3);
        _userIdLimitWindow = ConfigFile.ServerConfig.GetUShort("userid_ratelimit_window", 5);
        _preauthChallengeWindow = ConfigFile.ServerConfig.GetUShort("preauth_challenge_time_window", 8);
        _preauthChallengeClean = ConfigFile.ServerConfig.GetUShort("preauth_challenge_clean_period", 4);
        if (_ipRateLimitWindow == 0)
        {
            _ipRateLimitWindow = 1;
        }
        if (_userIdLimitWindow == 0)
        {
            _userIdLimitWindow = 1;
        }
        if (_preauthChallengeWindow == 0)
        {
            _preauthChallengeWindow = 1;
        }
        if (_preauthChallengeClean == 0)
        {
            _preauthChallengeClean = 1;
        }
    }

    private IEnumerator<float> _CreateLobby()
    {
        if (_queryEnabled)
        {
            _queryPort = LiteNetLib4MirrorTransport.Singleton.port + ConfigFile.ServerConfig.GetInt("query_port_shift");
            ServerConsole.AddLog("Query port will be enabled on port " + _queryPort + " TCP.");
            _queryserver = new QueryServer(_queryPort, ConfigFile.ServerConfig.GetBool("query_use_IPv6", def: true));
            _queryserver.StartServer();
        }
        else
        {
            ServerConsole.AddLog("Query port disabled in config!");
        }
        if (ConfigFile.HosterPolicy.GetString("server_ip", "none") != "none")
        {
            ServerConsole.Ip = ConfigFile.HosterPolicy.GetString("server_ip", "none");
            ServerConsole.AddLog("Server IP address set to " + ServerConsole.Ip + " by your hosting provider.");
        }
        else if (ConfigFile.ServerConfig.GetBool("online_mode"))
        {
            if (ConfigFile.ServerConfig.GetString("server_ip", "auto") != "auto")
            {
                ServerConsole.Ip = ConfigFile.ServerConfig.GetString("server_ip", "auto");
                ServerConsole.AddLog("Custom config detected. Your server IP address is " + ServerConsole.Ip);
            }
            else
            {
                ServerConsole.AddLog("Obtaining your external IP address...");
                while (true)
                {
                    using (UnityWebRequest www = UnityWebRequest.Get(CentralServer.StandardUrl + "ip.php"))
                    {
                        yield return Timing.WaitUntilDone(www.SendWebRequest());
                        if (string.IsNullOrEmpty(www.error))
                        {
                            ServerConsole.Ip = www.downloadHandler.text;
                            ServerConsole.AddLog("Done, your server IP address is " + ServerConsole.Ip);
                            break;
                        }
                        ServerConsole.AddLog("Error: connection to " + CentralServer.StandardUrl + " failed. Website returned: " + www.error + " | Retrying in " + IpRetryDelay + " seconds...", ConsoleColor.DarkRed);
                    }
                    yield return Timing.WaitForSeconds(IpRetryDelay);
                }
            }
        }
        else
        {
            ServerConsole.Ip = "127.0.0.1";
        }
        ServerConsole.AddLog("Initializing game server...");
        if (ConfigFile.HosterPolicy.GetString("ipv4_bind_ip", "none") != "none")
        {
            LiteNetLib4MirrorTransport.Singleton.serverIPv4BindAddress = ConfigFile.HosterPolicy.GetString("ipv4_bind_ip", "0.0.0.0");
            if (LiteNetLib4MirrorTransport.Singleton.serverIPv4BindAddress == "0.0.0.0")
            {
                ServerConsole.AddLog("Server starting at all IPv4 addresses and port " + LiteNetLib4MirrorTransport.Singleton.port + " - set by your hosting provider.");
            }
            else
            {
                ServerConsole.AddLog("Server starting at IPv4 " + LiteNetLib4MirrorTransport.Singleton.serverIPv4BindAddress + " and port " + LiteNetLib4MirrorTransport.Singleton.port + " - set by your hosting provider.");
            }
        }
        else
        {
            LiteNetLib4MirrorTransport.Singleton.serverIPv4BindAddress = ConfigFile.ServerConfig.GetString("ipv4_bind_ip", "0.0.0.0");
            if (LiteNetLib4MirrorTransport.Singleton.serverIPv4BindAddress == "0.0.0.0")
            {
                ServerConsole.AddLog("Server starting at all IPv4 addresses and port " + LiteNetLib4MirrorTransport.Singleton.port);
            }
            else
            {
                ServerConsole.AddLog("Server starting at IPv4 " + LiteNetLib4MirrorTransport.Singleton.serverIPv4BindAddress + " and port " + LiteNetLib4MirrorTransport.Singleton.port);
            }
        }
        if (ConfigFile.HosterPolicy.GetString("ipv6_bind_ip", "none") != "none")
        {
            LiteNetLib4MirrorTransport.Singleton.serverIPv6BindAddress = ConfigFile.HosterPolicy.GetString("ipv6_bind_ip", "::");
            if (LiteNetLib4MirrorTransport.Singleton.serverIPv6BindAddress == "::")
            {
                ServerConsole.AddLog("Server starting at all IPv6 addresses and port " + LiteNetLib4MirrorTransport.Singleton.port + " - set by your hosting provider.");
            }
            else
            {
                ServerConsole.AddLog("Server starting at IPv6 " + LiteNetLib4MirrorTransport.Singleton.serverIPv6BindAddress + " and port " + LiteNetLib4MirrorTransport.Singleton.port + " - set by your hosting provider.");
            }
        }
        else
        {
            LiteNetLib4MirrorTransport.Singleton.serverIPv6BindAddress = ConfigFile.ServerConfig.GetString("ipv6_bind_ip", "::");
            if (LiteNetLib4MirrorTransport.Singleton.serverIPv6BindAddress == "::")
            {
                ServerConsole.AddLog("Server starting at all IPv6 addresses and port " + LiteNetLib4MirrorTransport.Singleton.port);
            }
            else
            {
                ServerConsole.AddLog("Server starting at IPv6 " + LiteNetLib4MirrorTransport.Singleton.serverIPv6BindAddress + " and port " + LiteNetLib4MirrorTransport.Singleton.port);
            }
        }
        if (ServerConsole.PublicKey == null && ConfigFile.ServerConfig.GetBool("online_mode"))
        {
            ServerConsole.AddLog("Central server public key is not loaded. Waiting...");
           
            
            while (ServerConsole.PublicKey == null)
            {
                yield return Timing.WaitForSeconds(0.25f);
            }
            
            ServerConsole.AddLog("Continuing server startup sequence...");
        }
        StartHost();
        while (SceneManager.GetActiveScene().name != "Facility")
        {
            yield return float.NegativeInfinity;
        }
        ServerConsole.AddLog("Level loaded. Creating match...");
        if (!ConfigFile.ServerConfig.GetBool("online_mode"))
        {
            ServerConsole.AddLog("Server WON'T be visible on the public list due to online_mode turned off in server configuration.", ConsoleColor.DarkRed);
        }
        else if (ConfigFile.ServerConfig.GetBool("disable_global_badges"))
        {
            ServerConsole.AddLog("Server WON'T be visible on the public list due to disable_global_badges turned on in server configuration (this is servermod function - if you are not using servermod, you can safely remove this config value, it won't change anything).", ConsoleColor.DarkRed);
        }
        else if (ConfigFile.ServerConfig.GetBool("hide_global_badges"))
        {
            ServerConsole.AddLog("Server WON'T be visible on the public list due to hide_global_badges turned on in server configuration. You can still disable specific badges instead of using this command. (this is servermod function - if you are not using servermod, you can safely remove this config value, it won't change anything).", ConsoleColor.DarkRed);
        }
        else if (ConfigFile.ServerConfig.GetBool("disable_ban_bypass"))
        {
            ServerConsole.AddLog("Server WON'T be visible on the public list due to disable_ban_bypass turned on in server configuration. (this is servermod function - if you are not using servermod, you can safely remove this config value, it won't change anything).", ConsoleColor.DarkRed);
        }
        else
        {
            ServerConsole.singleton.RunServer();
        }
    }

    public void NonSteamHost()
    {
        onlineScene = "Facility";
        maxConnections = 20;
        LiteNetLib4MirrorTransport.Singleton.maxConnections = 20;

        int limit = ConfigFile.HosterPolicy.GetInt("players_limit", -1);
        if (limit > 0 && maxConnections > limit)
        {
            MaxPlayers = limit;
        }
        StartHostWithPort();
    }

    public void StartHostWithPort()
    {
        LiteNetLib4MirrorTransport.Singleton.serverIPv4BindAddress = ConfigFile.ServerConfig.GetString("ipv4_bind_ip", "0.0.0.0");
        if (LiteNetLib4MirrorTransport.Singleton.serverIPv4BindAddress != "0.0.0.0")
        {
            ServerConsole.AddLog($"Server starting at IPv4 {LiteNetLib4MirrorTransport.Singleton.serverIPv4BindAddress} and port {LiteNetLib4MirrorTransport.Singleton.port}");
        }
        else
        {
            ServerConsole.AddLog($"Server starting at all IPv4 addresses and port {LiteNetLib4MirrorTransport.Singleton.port}");
        }

        LiteNetLib4MirrorTransport.Singleton.serverIPv6BindAddress = ConfigFile.ServerConfig.GetString("ipv6_bind_ip", "::");
        if (LiteNetLib4MirrorTransport.Singleton.serverIPv6BindAddress != "::")
        {
            ServerConsole.AddLog($"Server starting at IPv6 {LiteNetLib4MirrorTransport.Singleton.serverIPv6BindAddress} and port {LiteNetLib4MirrorTransport.Singleton.port}");
        }
        else
        {
            ServerConsole.AddLog($"Server starting at all IPv6 addresses and port {LiteNetLib4MirrorTransport.Singleton.port}");
        }

        StartHost();
    }

    internal static void StartNondedicated(bool forceShow = false)
    {
        CustomNetworkManager.TypedSingleton.createPopForce?.SetActive(forceShow);
        CustomNetworkManager.TypedSingleton.createpop.SetActive(true);
    }

    public ushort GetFreePort()
    {
        var list = ConfigFile.ServerConfig.GetUShortList("port_queue");
        ushort[] array = list.ToArray();

        if (array.Length == 0)
        {
            array = new ushort[] { 7777, 7778, 7779, 7780, 7781, 7782, 7783, 7784 };
        }

        string joined = string.Join(", ", array);
        ServerConsole.AddLog("Port queue loaded: " + joined, ConsoleColor.Gray);

        return LiteNetLib4MirrorUtils.GetFirstFreePort(array);
    }
}