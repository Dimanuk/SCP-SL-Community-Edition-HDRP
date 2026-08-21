using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleMenu : MonoBehaviour
{
    public bool isPreloader;

    private static bool _server;

    private static bool _forceSettings;

    private static string _targetSceneName;

    private const float minLoadingTime = 3f;

    internal static readonly string[] MenuSceneNames = new string[3] { "MainMenuRemastered", "NewMainMenu", "FastMenu" };
    public float Progress => LauncherCommunicator.AssetVerificationProgress;

    private void Awake()
    {
        if (global::System.Linq.Enumerable.Any(StartupArgs.Args, (string arg) => arg.Equals("-nographics", global::System.StringComparison.OrdinalIgnoreCase)))
        {
            ServerStatic.IsDedicated = true;
        }
        if (isPreloader)
        {
            return;
        }
        CentralAuthManager.InitAuth();
        string[] args = StartupArgs.Args;
        for (int num = 0; num < args.Length; num++)
        {
            switch (args[num])
            {
                case "-fastmenu":
                    PlayerPrefsSl.Set("fastmenu", value: true);
                    PlayerPrefsSl.Set("menumode", 2);
                    break;
                case "-newmenu":
                    PlayerPrefsSl.Set("menumode", 1);
                    break;
                case "-nographics":
                    _server = true;
                    break;
                case "-forcemenu":
                    _forceSettings = true;
                    break;
            }
        }
        Refresh();
    }

    private void Start()
    {
        MEC.Timing.RunCoroutine(StartLoad());
    }

    private IEnumerator<float> StartLoad()
    {
        if (isPreloader)
        {
            float startTime = global::UnityEngine.Time.time;
            global::UnityEngine.AsyncOperation asyncOperation = global::UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Loader", global::UnityEngine.SceneManagement.LoadSceneMode.Single);
            while (!asyncOperation.isDone)
            {
                yield return float.NegativeInfinity;
            }
            float num = 3f - (global::UnityEngine.Time.time - startTime);
            if (num > 0f)
            {
                yield return global::MEC.Timing.WaitForSeconds(num);
            }
            asyncOperation.allowSceneActivation = true;
        }
    }

    private bool IsDone()
    {
        return LauncherCommunicator.VerificationFinished;
    }

    public void ChangeMode()
    {
        PlayerPrefsSl.Set("fastmenu", false);
        PlayerPrefsSl.Set("menumode", 1);
        Refresh();
        LoadCorrectScene();
    }

    public static void ChangeMode(int id)
    {
        PlayerPrefsSl.Set("menumode", id);
        Refresh();
        LoadCorrectScene();
    }

    private static void Refresh()
    {
        _targetSceneName = (_server ? "FastMenu" : MenuSceneNames[(!_forceSettings) ? 1 : global::UnityEngine.Mathf.Clamp(PlayerPrefsSl.Get("menumode", 1), 0, 2)]);

        var netManager = global::UnityEngine.Object.FindFirstObjectByType<CustomNetworkManager>();
        if (netManager != null)
        {
            netManager.offlineScene = _targetSceneName;
        }
    }

    public static void LoadCorrectScene()
    {
        Refresh();
        SceneManager.LoadScene(_targetSceneName);
    }
}
