using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class StartupArgs
{
    public static readonly string[] Args;

    static StartupArgs()
    {
#if UNITY_EDITOR
        Args = EditorStartupArgs.GetArgs();
#else
        Args = Environment.GetCommandLineArgs();
#endif
    }
}

#if UNITY_EDITOR
/// <summary>
/// Editor-only: configure fake launch arguments via
/// Tools → Startup Args in the Unity menu bar.
/// </summary>
[InitializeOnLoad]
public static class EditorStartupArgs
{
    private const string PrefKey = "EditorStartupArgs";
    private const string DefaultArgs = "-newmenu";

    // Cached on the main thread at editor load (InitializeOnLoad runs this static ctor),
    // because EditorPrefs can ONLY be accessed from the main thread. StartupArgs may be
    // first touched from a background thread (e.g. CentralAuthManager auth thread), so we
    // must never call EditorPrefs lazily from GetArgs().
    private static readonly string[] _cached = Parse(EditorPrefs.GetString(PrefKey, DefaultArgs));

    private static string[] Parse(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return Array.Empty<string>();

        return raw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[] GetArgs() => _cached;

    [MenuItem("Tools/Startup Args/Set Editor Args")]
    private static void OpenSetDialog()
    {
        string current = EditorPrefs.GetString(PrefKey, DefaultArgs);
        string result = current; 
        EditorStartupArgsWindow.Open(current);
    }

    [MenuItem("Tools/Startup Args/Reset to default (-newmenu)")]
    private static void ResetArgs()
    {
        EditorPrefs.SetString(PrefKey, DefaultArgs);
        UnityEngine.Debug.Log($"[StartupArgs] Reset to: {DefaultArgs}");
    }

    [MenuItem("Tools/Startup Args/Clear (no args)")]
    private static void ClearArgs()
    {
        EditorPrefs.SetString(PrefKey, "");
        UnityEngine.Debug.Log("[StartupArgs] Cleared.");
    }
}

public class EditorStartupArgsWindow : EditorWindow
{
    private const string PrefKey = "EditorStartupArgs";
    private string _current = "";

    public static void Open(string current)
    {
        var win = GetWindow<EditorStartupArgsWindow>(true, "Editor Startup Args", true);
        win._current = current;
        win.minSize = new UnityEngine.Vector2(360, 90);
        win.maxSize = new UnityEngine.Vector2(360, 90);
        win.ShowUtility();
    }

    private void OnGUI()
    {
        UnityEngine.GUILayout.Label("Fake launch arguments (space-separated):",
            EditorStyles.boldLabel);
        _current = EditorGUILayout.TextField(_current);
        UnityEngine.GUILayout.Space(6);

        UnityEngine.GUILayout.BeginHorizontal();
        if (UnityEngine.GUILayout.Button("Save"))
        {
            EditorPrefs.SetString(PrefKey, _current.Trim());
            UnityEngine.Debug.Log($"[StartupArgs] Editor args set to: \"{_current.Trim()}\"");
            Close();
        }
        if (UnityEngine.GUILayout.Button("Cancel"))
            Close();
        UnityEngine.GUILayout.EndHorizontal();
    }
}
#endif