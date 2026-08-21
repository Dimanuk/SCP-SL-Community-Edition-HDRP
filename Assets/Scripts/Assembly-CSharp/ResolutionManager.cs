using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class ResolutionManager : MonoBehaviour
{
    [global::System.Serializable]
    public class ResolutionPreset
    {
        public int Height;

        public int Width;

        public ResolutionPreset(global::UnityEngine.Resolution template)
        {
            Width = template.width;
            Height = template.height;
        }

        public void SetResolution()
        {
            global::UnityEngine.Screen.SetResolution(Width, Height, ScreenMode);
        }
    }

    public static int Preset;

	public static FullScreenMode ScreenMode;

	private static bool _initialized;

    public static global::System.Collections.Generic.List<ResolutionManager.ResolutionPreset> Presets = new global::System.Collections.Generic.List<ResolutionManager.ResolutionPreset>();

    private static bool FindResolution(global::UnityEngine.Resolution res)
    {
        foreach (ResolutionManager.ResolutionPreset preset in Presets)
        {
            if (preset.Height == res.height && preset.Width == res.width)
            {
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        if (!_initialized)
        {
            InitialisePresets();
        }
        Preset = global::UnityEngine.Mathf.Clamp(PlayerPrefsSl.Get("SavedResolutionSet", Presets.Count - 1), 0, Presets.Count - 1);
        ScreenMode = (global::UnityEngine.FullScreenMode)PlayerPrefsSl.Get("ScreenMode", 1);
        if (!ServerStatic.IsDedicated)
        {
            global::UnityEngine.Application.targetFrameRate = PlayerPrefsSl.Get("MaxFramerate", -1);
        }
        RefreshScreen();
        global::UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneWasLoaded;
    }

    private static void OnSceneWasLoaded(global::UnityEngine.SceneManagement.Scene scene, global::UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (!ServerStatic.IsDedicated)
        {
            int num = PlayerPrefsSl.Get("MaxFramerate", -1);
            global::UnityEngine.Application.targetFrameRate = ((scene.name == "NewMainMenu") ? global::System.Math.Min(num, 60) : num);
            if (global::UnityEngine.Application.targetFrameRate == -1 && scene.name == "NewMainMenu")
            {
                global::UnityEngine.Application.targetFrameRate = 60;
            }
            global::UnityEngine.QualitySettings.vSyncCount = (PlayerPrefsSl.Get("gfxsets_vsync", defaultValue: true) ? 1 : 0);
        }
        RefreshScreen();
    }

    private static void InitialisePresets()
    {
        Presets.Clear();
        global::UnityEngine.Resolution[] resolutions = global::UnityEngine.Screen.resolutions;
        foreach (global::UnityEngine.Resolution resolution in resolutions)
        {
            if (!FindResolution(resolution))
            {
                Presets.Add(new ResolutionManager.ResolutionPreset(resolution));
            }
        }
        _initialized = true;
    }

    public static void RefreshScreen()
    {
        if (!_initialized)
        {
            InitialisePresets();
        }
        if (Presets.Count != 0)
        {
            int idx = global::UnityEngine.Mathf.Clamp(Preset, 0, Presets.Count - 1);
            Presets[idx].SetResolution();

            ResolutionText resolutionText = Object.FindAnyObjectByType<ResolutionText>();
            if (resolutionText != null && resolutionText.txt != null)
                resolutionText.txt.text = CurrentResolutionString();
        }
    }

    public static void SetResolution(int id)
    {
        Preset = global::UnityEngine.Mathf.Clamp(id, 0, Presets.Count - 1);
        PlayerPrefsSl.Set("SavedResolutionSet", Preset);
        RefreshScreen();
    }

    public static void ChangeResolution(int id)
    {
        Preset = global::UnityEngine.Mathf.Clamp(Preset + id, 0, Presets.Count - 1);
        PlayerPrefsSl.Set("SavedResolutionSet", Preset);
        RefreshScreen();
    }

    public static void ChangeFullscreen(bool isTrue)
    {
        ChangeScreenMode(isTrue ? global::UnityEngine.FullScreenMode.FullScreenWindow : global::UnityEngine.FullScreenMode.Windowed);
    }

    public static void ChangeScreenMode(int mode)
    {
        ChangeScreenMode((global::UnityEngine.FullScreenMode)mode);
    }

    public static void ChangeScreenMode(global::UnityEngine.FullScreenMode mode)
    {
        ScreenMode = mode;
        PlayerPrefsSl.Set("ScreenMode", (int)mode);
        RefreshScreen();
    }

    public static string CurrentResolutionString()
    {
        return Presets[global::UnityEngine.Mathf.Clamp(Preset, 0, Presets.Count - 1)].Width + " × " + Presets[global::UnityEngine.Mathf.Clamp(Preset, 0, Presets.Count - 1)].Height;
    }
}
