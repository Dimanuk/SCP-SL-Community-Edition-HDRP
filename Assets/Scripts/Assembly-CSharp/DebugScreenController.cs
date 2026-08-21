using RemoteAdmin.Generic;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;

public class DebugScreenController : MonoBehaviour
{
    public GameObject Gui;
    public DebugLogScreen DebugLogScreen;
    public DebugLogReader DebugLogReader;

    public static int Asserts;
    public static int Errors;
    public static int Exceptions;

    private void Awake()
    {
        string dataPath = Application.dataPath;
        string fullPath = Path.GetFullPath(dataPath);
        string directoryName = Path.GetDirectoryName(fullPath);
        Directory.SetCurrentDirectory(directoryName);

        /*
        if (!Environment.GetCommandLineArgs().Contains<string>("-nographics"))
        {
            Shutdown.Quit();
        }
        */
    }

    private void Start()
    {
        GameObject gameObject = base.gameObject;
        UnityEngine.Object.DontDestroyOnLoad(gameObject);

        Application.LogCallback logCallback = new Application.LogCallback(LogMessage);
        Application.logMessageReceivedThreaded += logCallback;

        Log();
    }

    private static void Log()
    {
        string[] parts = new string[54];
        parts[0] = "Time: ";
        parts[1] = TimeBehaviour.Rfc3339Time();
        parts[2] = "\nGPU: ";
        parts[3] = SystemInfo.graphicsDeviceName;
        parts[4] = "\nGPU Driver version: ";
        parts[5] = GpuDriver.DriverVersion;
        parts[6] = "\nVRAM: ";
        parts[7] = SystemInfo.graphicsMemorySize.ToString();
        parts[8] = "MB\nShaderLevel: ";
        parts[9] = SystemInfo.graphicsShaderLevel.ToString().Insert(1, ".");
        parts[10] = "\nVendor: ";
        parts[11] = SystemInfo.graphicsDeviceVendor;
        parts[12] = "\nAPI: ";
        parts[13] = SystemInfo.graphicsDeviceType.ToString();
        parts[14] = "\nInfo: ";
        parts[15] = SystemInfo.graphicsDeviceVersion;
        parts[16] = "\nResolution: ";
        parts[17] = Screen.width.ToString();
        parts[18] = "x";
        parts[19] = Screen.height.ToString();
        parts[20] = "\nFPS Limit: ";
        parts[21] = Application.targetFrameRate.ToString();
        parts[22] = "\nFullscreen: ";
        parts[23] = Screen.fullScreenMode.ToString();
        parts[24] = "\nCPU: ";
        parts[25] = SystemInfo.processorType;
        parts[26] = "\nThreads: ";
        parts[27] = SystemInfo.processorCount.ToString();
        parts[28] = "\nFrequency: ";
        parts[29] = SystemInfo.processorFrequency.ToString();
        parts[30] = "MHz\nRAM: ";
        parts[31] = SystemInfo.systemMemorySize.ToString();
        parts[32] = "MB\nAudio Supported: ";
        parts[33] = SystemInfo.supportsAudio.ToString();
        parts[34] = "\nOS: ";
        parts[35] = NorthwoodLib.OperatingSystem.VersionString;
        parts[36] = "\nUnity: ";
        parts[37] = Application.unityVersion;
        parts[38] = "\nFramework: ";
        parts[39] = Misc.GetRuntimeVersion();
        parts[40] = "\nIL2CPP: ";
        parts[41] = PlatformInfo.singleton.IsIl2Cpp.ToString();
        parts[42] = "\nVersion: ";
        parts[43] = GameCore.Version.VersionString;
        parts[44] = "\nBuild: ";
        parts[45] = Application.buildGUID;
        parts[46] = "\nSystem Language: ";
        parts[47] = CultureInfo.CurrentCulture.EnglishName;
        parts[48] = " (";
        parts[49] = CultureInfo.CurrentCulture.Name;
        parts[50] = ")\nGame Language: ";
        parts[51] = "en";//UserSetting<string>.Get(UISetting.Language, "en");
        parts[52] = "\nLaunch arguments: ";
        parts[53] = Environment.CommandLine;
        string finalLog = string.Concat(parts);
        Debug.Log(finalLog);
    }

    private static void LogMessage(string condition, string stackTrace, LogType type)
    {
        int typeVal = (int)type;

        if (typeVal == 4)
        {
            Interlocked.Increment(ref Exceptions);
            return;
        }
        if (typeVal == 0) // LogType.Error
        {
            Interlocked.Increment(ref Errors);
            return;
        }
        if (typeVal == 1) // LogType.Assert
        {
            Interlocked.Increment(ref Asserts);
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (this.Gui.activeSelf != false)
            {
                this.DebugLogReader.OnDisable();
            }

            bool currentState = this.Gui.activeSelf;
            this.Gui.SetActive(!currentState);

            if (this.Gui.activeSelf != false)
            {
                if (this.DebugLogScreen.Log.activeSelf != false)
                {
                    if (DebugLogReader.SuccesfullyInitialized() != false)
                    {
                        this.DebugLogReader.OnEnable();
                    }
                }
            }
        }
    }
}