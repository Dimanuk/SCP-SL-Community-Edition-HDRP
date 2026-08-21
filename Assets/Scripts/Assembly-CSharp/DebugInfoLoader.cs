using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugInfoLoader : MonoBehaviour
{
    public Text Audio;
    public Text Cpu;
    public Text CpuThreadsAndFrequency;
    public Text Gpu;
    public Text DriverVersion;
    public Text GraphicApi;
    public Text Os;
    public Text Ram;
    public Text Resolution;
    public Text Fullscreen;
    public Text ShaderLevel;
    public Text Steam;
    public Text UnityVersion;
    public Text GameVersion;
    public Text Build;
    public Text GameLanguage;
    public Text GameScene;
    public Text Errors;
    public Text CentralServerText;

    private string _centralserver = "";
    private bool _updateOnNextFrame;
    private readonly Stopwatch _stopwatch = new Stopwatch();

    private void Awake()
    {
        PlayerPrefsSl.SettingChanged += (s, s1) => _updateOnNextFrame = true;
        PlayerPrefsSl.SettingRemoved += s => _updateOnNextFrame = true;
        PlayerPrefsSl.SettingsRefreshed += () => _updateOnNextFrame = true;
        SceneManager.sceneLoaded += (arg0, mode) => _updateOnNextFrame = true;
        GpuDriver.DriverLoaded += s => _updateOnNextFrame = true;
    }

    private void OnEnable()
    {
        _stopwatch.Restart();
        UpdateData();
    }

    private void UpdateData()
    {
        if (!enabled)
            return;

        Gpu.text = "GPU: " + SystemInfo.graphicsDeviceName;
        DriverVersion.text = "GPU Driver: " + GpuDriver.DriverVersion;

        string shaderLevelStr = SystemInfo.graphicsShaderLevel.ToString().Insert(1, ".");
        ShaderLevel.text = string.Concat("VRAM: ", SystemInfo.graphicsMemorySize, "MB ShaderLevel: ", shaderLevelStr);

        GraphicApi.text = string.Concat("Graphic API: ", SystemInfo.graphicsDeviceType, " ", SystemInfo.graphicsDeviceVersion);

        string fpsOrVsync = QualitySettings.vSyncCount != 0
            ? string.Format("VSync: {0}", QualitySettings.vSyncCount)
            : string.Format("FPS Limit: {0}", Application.targetFrameRate);
        Resolution.text = string.Concat("Resolution: ", Screen.width, "x", Screen.height, "  ", fpsOrVsync);

        Fullscreen.text = "Fullscreen: " + ResolutionManager.ScreenMode;
        Cpu.text = "CPU: " + SystemInfo.processorType;
        CpuThreadsAndFrequency.text = string.Concat("Threads: ", SystemInfo.processorCount, "   ", SystemInfo.processorFrequency, "MHz");
        Ram.text = "RAM: " + SystemInfo.systemMemorySize + "MB";
        Audio.text = "Audio Supported: " + SystemInfo.supportsAudio;
        Os.text = "OS: " + NorthwoodLib.OperatingSystem.VersionString;

        switch ((int)CentralAuthManager.Platform)
        {
            case 1:
                Steam.text = "Steam: " + SteamManager.GetApiState();
                break;
            case 2:
                Steam.text = "Discord: " + CentralAuthManager.DiscordState;
                break;
            case 0:
                Steam.text = "No auth platform!";
                break;
            default:
                Steam.text = "Unknown platform: " + CentralAuthManager.Platform;
                break;
        }

        UnityVersion.text = "Unity " + Application.unityVersion;
        GameVersion.text = "Version: " + GameCore.Version.VersionString;
        Build.text = "Build: " + PlatformInfo.singleton.BuildGuid;

        _centralserver = "Central Server: " + CentralServer.SelectedServer;
        CentralServerText.text = _centralserver;

        GameLanguage.text = "Language: " + TranslationReader.TranslationDirectoryName;
        GameScene.text = "Scene: " + SceneManager.GetActiveScene().name;
        Errors.text = string.Concat("Asserts: ", DebugScreenController.Asserts, " Errors: ", DebugScreenController.Errors, " Exceptions: ", DebugScreenController.Exceptions);
    }

    private void Update()
    {
        if (_stopwatch.ElapsedMilliseconds >= 15000L)
        {
            _stopwatch.Restart();
            _updateOnNextFrame = true;
        }

        if (_updateOnNextFrame)
        {
            _updateOnNextFrame = false;
            UpdateData();
        }
    }

    private void FixedUpdate()
    {
        string selectedServer = CentralServer.SelectedServer;
        if (!string.IsNullOrEmpty(selectedServer))
        {
            if (!_centralserver.Contains(selectedServer))
            {
                _centralserver = "Central Server: " + selectedServer;
                if (CentralServerText != null)
                    CentralServerText.text = _centralserver;
            }
        }
    }
}