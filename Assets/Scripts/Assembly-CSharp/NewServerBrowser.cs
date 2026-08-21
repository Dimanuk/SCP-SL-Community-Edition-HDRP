using System;
using System.Threading;
using Cryptography;
using NorthwoodLib;
using UnityEngine;
using UnityEngine.UI;


public class NewServerBrowser : MonoBehaviour
{
    private static bool _errorDirty;
    private static bool _refresh;
    private static bool _redownload;
    private static bool _threadStarted;
    private static string _errorMessage;
    private static Thread _refreshThread;

    public static ServerListItem[] Servers;

    public RectTransform ServerInfo;
    public Text LoadingText;
    public GameObject ServerTabs;

    private ServerFilter _serverFilter;

    private static void set_LoadingErrorMessage(string value)
    {
        _errorMessage = value;
        _errorDirty = true;
    }

    static NewServerBrowser()
    {
        _errorMessage = string.Empty;
        _errorDirty = false;

        ThreadStart start = new ThreadStart(DownloadList);
        _refreshThread = new Thread(start);
        _refreshThread.Name = "Server list download thread";
        _refreshThread.Priority = System.Threading.ThreadPriority.Lowest;
        _refreshThread.IsBackground = true;
    }

    private void Start()
    {
        _serverFilter = GetComponent<ServerFilter>();
        ServerTabs.SetActive(false);
        ServerTabs.SetActive(true);

        if (!_threadStarted)
        {
            _refreshThread.Start();
            _threadStarted = true;
        }
    }

    private void OnEnable()
    {
        if (_serverFilter == null)
            _serverFilter = GetComponent<ServerFilter>();

        if (_serverFilter != null && _serverFilter.CurrentTab == ServerTab.Friends)
            SteamLobby.RefreshFriendsServer();

        _redownload = true;
    }

    private void OnDisable()
    {
        _redownload = false;
    }

    private void OnDestroy()
    {
        _redownload = false;
    }

    private void Update()
    {
        if (_errorDirty)
        {
            if (LoadingText != null)
                LoadingText.text = _errorMessage;
            _errorDirty = false;
        }

        if (_refresh)
        {
            _refresh = false;
            if (_serverFilter != null)
                _serverFilter.ReapplyFilters(false);
        }
    }

    public void Refresh()
    {
        if (_serverFilter != null && _serverFilter.CurrentTab == ServerTab.Friends)
            SteamLobby.RefreshFriendsServer();

        _redownload = true;
    }

    public void AuthCompleted()
    {
        if (_errorMessage == TranslationReader.Get("NewMainMenu", 67, "AUTHENTICATING"))
        {
            string newMsg = TranslationReader.Get("MainMenu", 53, "DOWNLOADING DATA");
            set_LoadingErrorMessage(newMsg);
        }

        LauncherCommunicator.Heartbeat();
    }

    internal static void StopThread()
    {
        if (_threadStarted)
            _threadStarted = false;
    }

    private static void DownloadList()
    {
        int centralServerIndex = 0;
        bool firstRun = true;

        while (_threadStarted)
        {
            if (firstRun)
            {
                _redownload = true;
                firstRun = false;
            }

            while (!_redownload && _threadStarted)
            {
                Thread.Sleep(150);
            }

            if (!_threadStarted)
                break;

            var authStatus = CentralAuthManager.AuthStatusType;
            if (authStatus == AuthStatusType.PlatformAuthFailure)
                break;

            while (authStatus != AuthStatusType.Success)
            {
                set_LoadingErrorMessage(authStatus == AuthStatusType.Failure
                    ? TranslationReader.Get("NewMainMenu", 70, "<color=red>Connection failure (Check console for details)</color>")
                    : TranslationReader.Get("NewMainMenu", 67, "AUTHENTICATING"));
                if (!_threadStarted || !_redownload) break;
                Thread.Sleep(150);
                authStatus = CentralAuthManager.AuthStatusType;
                if (authStatus == AuthStatusType.PlatformAuthFailure) return;
            }

            if (!_threadStarted)
                break;

            if (!_redownload || authStatus != AuthStatusType.Success)
                continue;

            _redownload = false;
            set_LoadingErrorMessage(TranslationReader.Get("MainMenu", 53, "DOWNLOADING DATA"));
            ServerListItem[] newServers = null;

            while (centralServerIndex < 2)
            {
                string url = CentralServer.MasterUrl + "lobbylist.php?format=json-signed-unix&version=2&minimal=1";
                string data = "token=" + CentralAuthManager.ApiToken + "&nonce=" + CentralAuthManager.Nonce;

                GameCore.Console.AddLog("Loading server list...", Color.white, true, GameCore.Console.ConsoleLogType.Log);
                ServerListSigned signed;
                try
                {
                    string response = HttpQuery.Post(url, data);
                    signed = JsonSerialize.FromJson<ServerListSigned>(response);
                }
                catch (Exception ex)
                {
                    GameCore.Console.AddLog("Failed to download server list: " + ex.Message, Color.red, true, GameCore.Console.ConsoleLogType.Log);
                    set_LoadingErrorMessage("Failed to download server list: " + ex.Message);
                    centralServerIndex++;
                    continue;
                }

                if (string.IsNullOrEmpty(signed.payload) && string.IsNullOrEmpty(signed.error))
                {
                    GameCore.Console.AddLog("Server list deserialization error", Color.red, true, GameCore.Console.ConsoleLogType.Log);
                    set_LoadingErrorMessage("Server list deserialization error");
                    centralServerIndex++;
                    continue;
                }

                if (!string.IsNullOrEmpty(signed.error))
                {
                    string errMsg = "Server returned an error.\n" + signed.error;
                    GameCore.Console.AddLog(errMsg, Color.red, true, GameCore.Console.ConsoleLogType.Log);
                    set_LoadingErrorMessage(errMsg);
                }
                else
                {
                    string signData = signed.payload + "##" + signed.timestamp;
                    if (ECDSA.Verify(signData, signed.signature, ServerConsole.PublicKey))
                    {
                        long currentTime = TimeBehaviour.CurrentUnixTimestamp;
                        long diff = Math.Abs(currentTime - signed.timestamp);

                        if (diff <= 14400)
                        {
                            string decoded = NorthwoodLib.StringUtils.Base64Decode(signed.payload);
                            ServerList list = JsonSerialize.FromJson<ServerList>(decoded);
                            newServers = list.servers;
                            break;
                        }
                        else
                        {
                            int minutes = (int)(diff / 60);
                            string msg = "Server list response has expired! Make sure that time and timezone set on your PC is correct. We recommend synchronizing the time. Offset: " + minutes + " minutes.";
                            ServerConsole.AddLog(msg, ConsoleColor.White);
                            set_LoadingErrorMessage(msg);
                        }
                    }
                    else
                    {
                        GameCore.Console.AddLog("Server list has an invalid signature!", Color.red, true, GameCore.Console.ConsoleLogType.Log);
                        set_LoadingErrorMessage("Server list has an invalid signature!");
                    }
                }

                centralServerIndex++;
            }

            if (newServers != null)
            {
                Servers = newServers;
                _refresh = true;
            }
            else
            {

                bool switched = CentralServer.ChangeCentralServer(true);
                if (switched)
                {
                    GameCore.Console.AddLog("Failed to download servers list.\nChecking other servers...", Color.red, true, GameCore.Console.ConsoleLogType.Log);
                }
                else
                {
                    GameCore.Console.AddLog("Failed to download servers list.", Color.red, true, GameCore.Console.ConsoleLogType.Log);
                }
            }

            centralServerIndex = 0;
        }
    }
}