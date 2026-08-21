using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// One-click Dedicated Server build tool (Windows or Linux).
/// Builds with the "Server" subtarget using the project's real scene list (so scene
/// build-indices stay exactly what <see cref="ServerStatic.OnSceneWasLoaded"/> expects),
/// syncs scripting define symbols from the Standalone target to the Server subtarget
/// (Unity 6 keeps them separate), excludes the Windows-only Steamworks plugin from Linux
/// builds, wipes the previous build output first, and drops a ready-to-run launch script
/// (-nographics -batchmode -port) next to the built executable.
/// </summary>
public class DedicatedServerBuildTool : EditorWindow
{
    private enum ServerPlatform
    {
        Windows,
        Linux
    }

    private const string SteamworksDllPath = "Assets/Plugins/Facepunch.Steamworks.Win64.dll";
    private const int DefaultPort = 7777;

    private ServerPlatform _platform = ServerPlatform.Windows;
    private bool _cleanOutputFolder = true;
    private bool _developmentBuild;
    private bool _revealAfterBuild = true;

    [MenuItem("SCP/Build Dedicated Server.../Open Build Window", false, 0)]
    private static void Open()
    {
        var window = GetWindow<DedicatedServerBuildTool>(true, "Dedicated Server Build", true);
        window.minSize = new Vector2(380, 200);
    }

    [MenuItem("SCP/Build Dedicated Server.../Windows (Quick Build)", false, 20)]
    private static void QuickBuildWindows()
    {
        Build(ServerPlatform.Windows, clean: true, development: false, reveal: true);
    }

    [MenuItem("SCP/Build Dedicated Server.../Linux (Quick Build)", false, 21)]
    private static void QuickBuildLinux()
    {
        Build(ServerPlatform.Linux, clean: true, development: false, reveal: true);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Target Platform", EditorStyles.boldLabel);
        _platform = (ServerPlatform)EditorGUILayout.EnumPopup(_platform);

        EditorGUILayout.Space();
        _cleanOutputFolder = EditorGUILayout.ToggleLeft(
            "Clean output folder before build (removes previous server build junk)", _cleanOutputFolder);
        _developmentBuild = EditorGUILayout.ToggleLeft("Development build", _developmentBuild);
        _revealAfterBuild = EditorGUILayout.ToggleLeft("Reveal build folder when finished", _revealAfterBuild);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Builds a headless Dedicated Server player (subtarget=Server, real scene list) " +
            "and writes a launch script (-nographics -batchmode -port " + DefaultPort + ") next to it.",
            MessageType.Info);

        EditorGUILayout.Space();
        if (GUILayout.Button("Build", GUILayout.Height(32)))
        {
            Build(_platform, _cleanOutputFolder, _developmentBuild, _revealAfterBuild);
        }
    }

    private static void Build(ServerPlatform platform, bool clean, bool development, bool reveal)
    {
        BuildTarget target = platform == ServerPlatform.Windows
            ? BuildTarget.StandaloneWindows64
            : BuildTarget.StandaloneLinux64;

        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, target))
        {
            EditorUtility.DisplayDialog("Dedicated Server Build",
                $"Build support for {target} is not installed.\nAdd it via Unity Hub > Installs > Add Modules.",
                "OK");
            return;
        }

        string platformFolder = platform.ToString();
        string outputDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Builds",
            "DedicatedServer", platformFolder);
        string exeName = platform == ServerPlatform.Windows ? "SCPSL_Server.exe" : "SCPSL_Server";
        string locationPathName = Path.Combine(outputDir, exeName);

        if (clean && Directory.Exists(outputDir))
        {
            Directory.Delete(outputDir, true);
        }

        Directory.CreateDirectory(outputDir);

        if (platform == ServerPlatform.Linux)
        {
            ExcludeSteamworksFromLinux();
        }

        SyncServerScriptingDefines();

        string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = locationPathName,
            target = target,
            subtarget = (int)StandaloneBuildSubtarget.Server,
            options = development ? BuildOptions.Development : BuildOptions.None
        };

        Debug.Log($"[DedicatedServerBuildTool] Building {platform} dedicated server -> {locationPathName} " +
                  $"({scenes.Length} scenes)");

        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            WriteLaunchScript(platform, outputDir, exeName);
            Debug.Log($"[DedicatedServerBuildTool] Build succeeded: " +
                      $"{report.summary.totalSize / (1024 * 1024)} MB in {report.summary.totalTime}");
            if (reveal)
            {
                EditorUtility.RevealInFinder(locationPathName);
            }
        }
        else
        {
            Debug.LogError($"[DedicatedServerBuildTool] Build failed: {report.summary.result}");
            EditorUtility.DisplayDialog("Dedicated Server Build",
                $"Build failed: {report.summary.result}\nSee Console for details.", "OK");
        }
    }

    /// <summary>
    /// Unity 6 stores scripting define symbols for the Server subtarget separately from
    /// Standalone. Without this, MIRROR/LITENETLIB4MIRROR/etc. may be missing from a server build.
    /// </summary>
    private static void SyncServerScriptingDefines()
    {
        string[] standaloneDefines = SplitDefines(PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone));
        string[] serverDefines = SplitDefines(PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Server));
        string[] merged = standaloneDefines.Union(serverDefines).Distinct().ToArray();

        if (!merged.OrderBy(d => d).SequenceEqual(serverDefines.OrderBy(d => d)))
        {
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Server, string.Join(";", merged));
            Debug.Log("[DedicatedServerBuildTool] Synced scripting define symbols (Standalone -> Server subtarget).");
        }
    }

    private static string[] SplitDefines(string defines)
    {
        return string.IsNullOrEmpty(defines)
            ? new string[0]
            : defines.Split(';').Where(d => !string.IsNullOrEmpty(d)).ToArray();
    }

    /// <summary>
    /// Facepunch.Steamworks.Win64.dll is a Windows PE binary marked "Any Platform" — it would
    /// otherwise get copied into Linux server builds, where it can never load.
    /// </summary>
    private static void ExcludeSteamworksFromLinux()
    {
        var importer = AssetImporter.GetAtPath(SteamworksDllPath) as PluginImporter;
        if (importer == null)
        {
            return;
        }

        if (!importer.GetExcludeFromAnyPlatform(BuildTarget.StandaloneLinux64))
        {
            importer.SetExcludeFromAnyPlatform(BuildTarget.StandaloneLinux64, true);
            importer.SaveAndReimport();
            Debug.Log("[DedicatedServerBuildTool] Excluded Facepunch.Steamworks.Win64.dll from Linux builds " +
                      "(Windows-only native DLL).");
        }
    }

    private static void WriteLaunchScript(ServerPlatform platform, string outputDir, string exeName)
    {
        if (platform == ServerPlatform.Windows)
        {
            string bat = "@echo off\r\n" +
                         $"\"%~dp0{exeName}\" -nographics -batchmode -stdout -port{DefaultPort}\r\n" +
                         "pause\r\n";
            File.WriteAllText(Path.Combine(outputDir, "RunServer.bat"), bat);
        }
        else
        {
            string sh = "#!/bin/bash\n" +
                        "DIR=\"$(cd \"$(dirname \"${BASH_SOURCE[0]}\")\" && pwd)\"\n" +
                        $"chmod +x \"$DIR/{exeName}\"\n" +
                        $"\"$DIR/{exeName}\" -nographics -batchmode -stdout -port{DefaultPort} \"$@\"\n";
            File.WriteAllText(Path.Combine(outputDir, "run_server.sh"), sh);
        }
    }
}
