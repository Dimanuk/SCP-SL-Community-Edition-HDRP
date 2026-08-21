using System;
using System.Diagnostics;
using ServerOutput;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class ServerStatic : MonoBehaviour
{
    public enum NextRoundAction : byte
    {
        DoNothing = 0,
        Restart = 1,
        Shutdown = 2
    }

    internal static bool ProcessIdPassed;
    internal static bool DisableConfigValidation;
    internal static bool KeepSession;
    internal static bool EnableConsoleHeartbeat;
    private static bool _serverArgsProcessed;
    private static bool _serverPortSet;
    public const string EditorDedicatedPrefKey = "SCP_EditorDedicated";
    public static ushort ShutdownRedirectPort;
    internal static YamlConfig RolesConfig;
    internal static YamlConfig SharedGroupsConfig;
    internal static YamlConfig SharedGroupsMembersConfig;
    internal static string RolesConfigPath;
    internal static PermissionsHandler PermissionsHandler;
    private static short _serverTickrate = 60;
    public static IServerOutput ServerOutput;
    public static bool IsDedicated { get; set; }
    public static NextRoundAction StopNextRound { get; set; } = NextRoundAction.DoNothing;

    internal static short ServerTickrate
    {
        get => _serverTickrate;
        set
        {
            _serverTickrate = (value < -1 || value == 0) ? (short)60 : value;

            if (IsDedicated)
            {
                Application.targetFrameRate = _serverTickrate;
                ServerConsole.AddLog("Server tickrate set to: " + _serverTickrate);
            }
        }
    }

    public static ushort ServerPort { get; private set; }

    private void Awake()
    {

#if UNITY_EDITOR
        IsDedicated = UnityEditor.EditorPrefs.GetBool(EditorDedicatedPrefKey, false);
#endif
        ProcessServerArgs();

        if (!IsDedicated)
        {
            ServerOutput = new NonDedicatedOutput();
        }

#if UNITY_EDITOR
        if (IsDedicated && ServerOutput == null)
        {
            ServerOutput = new NonDedicatedOutput();
        }
#endif

        if (ServerOutput == null)
        {
            Shutdown.Quit();
            return;
        }

        ServerOutput.Start();

        if (IsDedicated)
        {
            AudioListener.volume = 0f;
            AudioListener.pause = true;
            QualitySettings.pixelLightCount = 0;

            ServerConsole.AddLog("SCP Secret Laboratory process started. Creating match...", ConsoleColor.Green);
            ServerTickrate = 60;

            if (!_serverPortSet)
            {
#if UNITY_EDITOR
                ServerPort = 7777;
                _serverPortSet = true;
                ServerConsole.AddLog("Editor dedicated server: no \"-port\" argument, defaulting to 7777.", ConsoleColor.Yellow);
#else
                ServerConsole.AddLog("\"-port\" argument is required for dedicated server. Aborting startup.", ConsoleColor.DarkRed);
                ServerConsole.AddLog("Make sure you are using latest version of LocalAdmin.", ConsoleColor.DarkRed);
                Shutdown.Quit();
                return;
#endif
            }
        }

        UnityEngine.Debug.Log("IsDedicated " + IsDedicated);
        SceneManager.sceneLoaded += OnSceneWasLoaded;
    }

    [RuntimeInitializeOnLoadMethod]
    internal static void ProcessServerArgs()
    {
        if (_serverArgsProcessed) return;
        _serverArgsProcessed = true;

        int txBuffer = 0;
        int rxBuffer = 0;

        string[] args = Environment.GetCommandLineArgs();

        if (args == null || args.Length == 0) return;

        for (int i = 0; i < args.Length; i++)
        {
            string text = args[i].ToLower();

            switch (text)
            {
                case "-nographics":
#if !UNITY_EDITOR
                    IsDedicated = true;
#endif
                    break;
                case "-keepsession": KeepSession = true; break;
                case "-heartbeat": EnableConsoleHeartbeat = true; break;
                case "-disableconfigvalidation": DisableConfigValidation = true; break;
                case "-stdout":
                    if (!_serverPortSet && ServerOutput == null)
                        ServerOutput = new StandardOutput();
                    break;
                case "-appdatapath":
                    if (i + 1 < StartupArgs.Args.Length) FileManager.SetAppFolder(StartupArgs.Args[++i]);
                    break;
                case "-configpath":
                    if (i + 1 < StartupArgs.Args.Length) FileManager.SetConfigFolder(StartupArgs.Args[++i]);
                    break;
                case "-txbuffer":
                    if (i + 1 < StartupArgs.Args.Length) int.TryParse(StartupArgs.Args[++i], out txBuffer);
                    break;
                case "-rxbuffer":
                    if (i + 1 < StartupArgs.Args.Length) int.TryParse(StartupArgs.Args[++i], out rxBuffer);
                    break;
            }

            if (text.StartsWith("-key") && text.Length > 4 && ServerOutput == null)
            {
                ServerOutput = new FileConsole(text.Substring(4));
            }
            else if (text.StartsWith("-console") && ServerOutput == null)
            {
                if (ushort.TryParse(text.Substring(8), out var consolePort))
                    ServerOutput = new TcpConsole(consolePort, rxBuffer, txBuffer);
            }
            else if (text.StartsWith("-id") && !ProcessIdPassed)
            {
                ProcessIdPassed = true;
                if (int.TryParse(text.Substring(3), out var pid))
                {
                    try
                    {
                        ServerConsole.ConsoleProcess = Process.GetProcessById(pid);
                        ServerConsole.ConsoleProcess.EnableRaisingEvents = true;
                        ServerConsole.ConsoleProcess.Exited += OnConsoleExited;
                    }
                    catch { OnConsoleExited(null, null); }
                }
            }
            else if (text.StartsWith("-port") && !_serverPortSet)
            {
                if (ushort.TryParse(text.Substring(5), out var port))
                {
                    ServerPort = port;
                    _serverPortSet = true;
                }
                else
                {
                    ServerConsole.AddLog("Invalid port. Aborting.");
                    Shutdown.Quit();
                }
            }
        }
    }

    private static void OnConsoleExited(object sender, EventArgs e)
    {
        ServerConsole.DisposeStatic();
        IsDedicated = false;
        UnityEngine.Debug.Log("OnConsoleExited");
        ServerConsole.ConsoleProcess?.Dispose();
        ServerConsole.ConsoleProcess = null;
        Shutdown.Quit();
    }

    private void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsDedicated)
        {
            if (scene.buildIndex == 3 || scene.buildIndex == 4)
            {
                if (TryGetComponent<CustomNetworkManager>(out var networkManager))
                    networkManager.CreateMatch();
            }
        }
    }

    public static PermissionsHandler GetPermissionsHandler() => PermissionsHandler;
}
