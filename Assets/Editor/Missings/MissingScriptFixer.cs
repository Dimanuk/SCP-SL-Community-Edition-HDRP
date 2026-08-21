using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.Security.Cryptography;

// ============================================================================
// DATA CLASSES
// ============================================================================
public class ScriptMapping
{
    public string ClassName { get; set; }
    public string Namespace { get; set; }
    public string FullName { get; set; }
    public int OldFileID { get; set; }
    public string OldGuid { get; set; }
    public int NewFileID { get; set; } = 11500000;
    public string NewGuid { get; set; }
    public string AssemblyName { get; set; }
    public string DllPath { get; set; }
    public bool FoundInProject { get; set; }
    public bool IsBuiltIn { get; set; }
}

public class MissingScriptData
{
    public GameObject GameObject { get; set; }
    public int ComponentIndex { get; set; }
    public int FileID { get; set; }
    public string Guid { get; set; }
    public string ClassName { get; set; }
    public MonoScript Replacement { get; set; }
    public string ScenePath { get; set; }
}

// ============================================================================
// MAIN WINDOW - AUTO YAML FIXER
// ============================================================================
[InitializeOnLoad]
public class MissingScriptAutoFixer : EditorWindow
{
    private enum Tab { Inspector, MultiDLL, AutoFix, Settings }
    private Tab activeTab = Tab.Inspector;

    // Inspector
    private List<MissingScriptData> missingScripts = new List<MissingScriptData>();
    private Dictionary<string, MonoScript> scriptCache = new Dictionary<string, MonoScript>();
    private string sceneContent = "";
    private bool autoScanEnabled = true;

    // Multi-DLL Export
    private List<ScriptMapping> exportMappings = new List<ScriptMapping>();
    private List<string> loadedDllPaths = new List<string>();
    private string exportSearch = "";
    private Vector2 exportScroll;

    // Auto-Fix
    private string jsonPath = "";
    private List<string> scenesToFix = new List<string>();
    private List<FixResult> fixResults = new List<FixResult>();
    private bool fixAllScenes = false;
    private Vector2 fixScroll;

    // Settings
    private bool createBackups = true;
    private bool showWarnings = true;
    private bool autoRefresh = true;
    private bool includeBuiltIn = false;

    private Vector2 mainScroll;

    [MenuItem("Tools/Missing Script Auto Fixer")]
    public static void OpenWindow()
    {
        var window = GetWindow<MissingScriptAutoFixer>("Auto Fixer");
        window.minSize = new Vector2(650, 550);
        window.Show();
    }

    private void OnEnable()
    {
        LoadSettings();
        if (autoScanEnabled && Selection.activeGameObject != null)
            ScanForMissingScripts();
        CacheAllScripts();
    }

    private void OnDisable() => SaveSettings();
    private void OnSelectionChange()
    {
        if (autoScanEnabled && Selection.activeGameObject != null && activeTab == Tab.Inspector)
            ScanForMissingScripts();
    }

    // ========================================================================
    // UI
    // ========================================================================
    private void OnGUI()
    {
        DrawToolbar();
        mainScroll = EditorGUILayout.BeginScrollView(mainScroll);

        switch (activeTab)
        {
            case Tab.Inspector: DrawInspectorTab(); break;
            case Tab.MultiDLL: DrawMultiDLLTab(); break;
            case Tab.AutoFix: DrawAutoFixTab(); break;
            case Tab.Settings: DrawSettingsTab(); break;
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Toggle(activeTab == Tab.Inspector, "🔍 Inspector", EditorStyles.toolbarButton)) activeTab = Tab.Inspector;
        if (GUILayout.Toggle(activeTab == Tab.MultiDLL, $"📦 DLLs ({loadedDllPaths.Count})", EditorStyles.toolbarButton)) activeTab = Tab.MultiDLL;
        if (GUILayout.Toggle(activeTab == Tab.AutoFix, "⚡ Auto-Fix", EditorStyles.toolbarButton)) activeTab = Tab.AutoFix;
        if (GUILayout.Toggle(activeTab == Tab.Settings, "⚙️ Settings", EditorStyles.toolbarButton)) activeTab = Tab.Settings;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(5);
    }

    // ========================================================================
    // INSPECTOR TAB
    // ========================================================================
    private void DrawInspectorTab()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("🔍 Missing Script Inspector", EditorStyles.boldLabel);

        autoScanEnabled = EditorGUILayout.ToggleLeft("Auto-scan selected GameObject", autoScanEnabled);

        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorGUILayout.HelpBox("Select a GameObject to scan for Missing Scripts", MessageType.Info);
        }
        else
        {
            if (GUILayout.Button("Scan Now", GUILayout.Height(30))) ScanForMissingScripts();

            if (missingScripts.Count > 0)
            {
                EditorGUILayout.LabelField($"Found {missingScripts.Count} missing script(s):", EditorStyles.boldLabel);
                foreach (var data in missingScripts) DrawMissingScriptEntry(data);
            }
            else
            {
                EditorGUILayout.LabelField("✅ No Missing Scripts", EditorStyles.miniLabel);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawMissingScriptEntry(MissingScriptData data)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"#{data.ComponentIndex}", EditorStyles.boldLabel, GUILayout.Width(40));
        EditorGUILayout.LabelField(data.ClassName ?? "Unknown", GUILayout.ExpandWidth(true));
        if (GUILayout.Button("Copy GUID", GUILayout.Width(80)))
            EditorGUIUtility.systemCopyBuffer = data.Guid ?? "";
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField($"FileID: {data.FileID} | GUID: {data.Guid ?? "N/A"}", EditorStyles.miniLabel);

        MonoScript suggested = null;
        if (!string.IsNullOrEmpty(data.ClassName) && scriptCache.ContainsKey(data.ClassName))
            suggested = scriptCache[data.ClassName];

        data.Replacement = (MonoScript)EditorGUILayout.ObjectField(suggested ?? data.Replacement, typeof(MonoScript), false);

        if (data.Replacement != null && GUILayout.Button("APPLY FIX", GUILayout.Height(25)))
        {
            if (showWarnings && EditorUtility.DisplayDialog("Confirm", $"Fix {data.ClassName}?", "Yes", "No"))
                ApplyFix(data);
            else if (!showWarnings)
                ApplyFix(data);
        }
        EditorGUILayout.EndVertical();
    }

    private void ScanForMissingScripts()
    {
        missingScripts.Clear();
        var go = Selection.activeGameObject;
        if (go == null) return;

        var components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                var data = new MissingScriptData { GameObject = go, ComponentIndex = i };
                ExtractInfoFromScene(data);
                missingScripts.Add(data);
            }
        }
        if (missingScripts.Count > 0) LoadSceneContent();
        Repaint();
    }

    private void LoadSceneContent()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (!string.IsNullOrEmpty(scene.path) && File.Exists(scene.path))
            sceneContent = File.ReadAllText(scene.path);
    }

    private void ExtractInfoFromScene(MissingScriptData data)
    {
        if (string.IsNullOrEmpty(sceneContent)) return;
        var go = data.GameObject;
        var pattern = $"--- !u!1 &{go.GetInstanceID()}";
        int objIndex = sceneContent.IndexOf(pattern);
        if (objIndex >= 0)
        {
            int nextIndex = sceneContent.IndexOf("---", objIndex + 1);
            if (nextIndex == -1) nextIndex = sceneContent.Length;
            string section = sceneContent.Substring(objIndex, nextIndex - objIndex);
            var matches = Regex.Matches(section, @"m_Script:\s*\{fileID:\s*(-?\d+),\s*guid:\s*([a-f0-9]{32})");
            if (matches.Count > data.ComponentIndex)
            {
                var match = matches[data.ComponentIndex];
                data.FileID = int.Parse(match.Groups[1].Value);
                data.Guid = match.Groups[2].Value;
                data.ClassName = FindClassNameByGuid(data.Guid);
            }
        }
    }

    private void CacheAllScripts()
    {
        scriptCache.Clear();
        foreach (var script in MonoImporter.GetAllRuntimeMonoScripts())
        {
            if (script != null)
            {
                var className = script.GetClass()?.Name;
                if (!string.IsNullOrEmpty(className) && !scriptCache.ContainsKey(className))
                    scriptCache[className] = script;
            }
        }
    }

    private string FindClassNameByGuid(string guid)
    {
        foreach (var kvp in scriptCache)
        {
            var path = AssetDatabase.GetAssetPath(kvp.Value);
            if (!string.IsNullOrEmpty(path) && AssetDatabase.AssetPathToGUID(path) == guid)
                return kvp.Key;
        }
        return null;
    }

    private void ApplyFix(MissingScriptData data)
    {
        if (data.Replacement == null) return;
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (string.IsNullOrEmpty(scene.path) || !File.Exists(scene.path))
        {
            EditorUtility.DisplayDialog("Error", "Scene not saved!", "OK");
            return;
        }

        try
        {
            string newGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(data.Replacement));
            if (createBackups)
            {
                string backup = scene.path + ".backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                File.Copy(scene.path, backup, true);
            }

            string content = File.ReadAllText(scene.path);
            var pattern = $"--- !u!1 &{data.GameObject.GetInstanceID()}";
            int objIndex = content.IndexOf(pattern);
            if (objIndex >= 0)
            {
                int nextIndex = content.IndexOf("---", objIndex + 1);
                if (nextIndex == -1) nextIndex = content.Length;
                string section = content.Substring(objIndex, nextIndex - objIndex);
                var matches = Regex.Matches(section, @"m_Script:\s*\{fileID:\s*-?\d+,\s*guid:\s*[a-f0-9]{32}");
                if (matches.Count > data.ComponentIndex)
                {
                    var oldMatch = matches[data.ComponentIndex];
                    string newMatch = $"m_Script: {{fileID: 11500000, guid: {newGuid}}}";
                    content = content.Replace(oldMatch.Value, newMatch);
                    File.WriteAllText(scene.path, content);
                    if (autoRefresh) AssetDatabase.Refresh();
                    EditorUtility.DisplayDialog("Success", $"Fixed {data.ClassName}!", "OK");
                    ScanForMissingScripts();
                }
            }
        }
        catch (Exception e) { EditorUtility.DisplayDialog("Error", e.Message, "OK"); }
    }

    // ========================================================================
    // MULTI-DLL TAB
    // ========================================================================
    private void DrawMultiDLLTab()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("📦 Multi-DLL Export", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Load multiple DLLs to create mapping JSON", MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add DLL", GUILayout.Height(30)))
        {
            string path = EditorUtility.OpenFilePanel("Select DLL", "", "dll");
            if (!string.IsNullOrEmpty(path) && !loadedDllPaths.Contains(path))
            {
                loadedDllPaths.Add(path);
                LoadDllMappings(path);
            }
        }
        if (GUILayout.Button("Clear All", GUILayout.Width(100)))
        {
            loadedDllPaths.Clear();
            exportMappings.Clear();
        }
        EditorGUILayout.EndHorizontal();

        if (loadedDllPaths.Count > 0)
        {
            foreach (var dll in loadedDllPaths)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Path.GetFileName(dll), GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    loadedDllPaths.Remove(dll);
                    exportMappings.RemoveAll(m => m.DllPath == dll);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Analyze All DLLs", GUILayout.Height(35)))
            {
                exportMappings.Clear();
                foreach (var dll in loadedDllPaths) LoadDllMappings(dll);
                EditorUtility.DisplayDialog("Done", $"Processed {exportMappings.Count} mappings", "OK");
            }
        }

        if (exportMappings.Count > 0)
        {
            exportSearch = EditorGUILayout.TextField("Search:", exportSearch);
            EditorGUILayout.LabelField($"Total: {exportMappings.Count} | Found: {exportMappings.Count(m => !string.IsNullOrEmpty(m.NewGuid))}", EditorStyles.miniLabel);

            exportScroll = EditorGUILayout.BeginScrollView(exportScroll, GUILayout.Height(200));
            foreach (var m in exportMappings.Where(x => string.IsNullOrEmpty(exportSearch) || x.FullName.IndexOf(exportSearch, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                EditorGUILayout.BeginHorizontal();
                string status = !string.IsNullOrEmpty(m.NewGuid) ? "✅" : "❌";
                EditorGUILayout.LabelField($"{status} {m.FullName}", GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Export to JSON", GUILayout.Height(35))) ExportToJson();
        }
        EditorGUILayout.EndVertical();
    }

    private void LoadDllMappings(string dllPath)
    {
        if (string.IsNullOrEmpty(dllPath) || !File.Exists(dllPath)) return;
        string oldGuid = GetGuidFromMeta(dllPath + ".meta") ?? Guid.NewGuid().ToString("N").Substring(0, 32);
        Assembly asm = Assembly.LoadFrom(dllPath);

        var types = asm.GetTypes()
            .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t) || typeof(ScriptableObject).IsAssignableFrom(t))
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => includeBuiltIn || !IsBuiltInType(t))
            .ToList();

        foreach (var type in types)
        {
            if (exportMappings.Any(m => m.FullName == type.FullName && m.DllPath == dllPath)) continue;

            var mapping = new ScriptMapping
            {
                ClassName = type.Name,
                Namespace = type.Namespace ?? "",
                FullName = string.IsNullOrEmpty(type.Namespace) ? type.Name : $"{type.Namespace}.{type.Name}",
                OldFileID = FileIDUtil.Compute(type),
                OldGuid = oldGuid,
                AssemblyName = type.Assembly.GetName().Name,
                DllPath = dllPath,
                IsBuiltIn = IsBuiltInType(type)
            };
            FindCurrentDllGuid(mapping);
            exportMappings.Add(mapping);
        }
    }

    private bool IsBuiltInType(Type t)
    {
        string asm = t.Assembly.GetName().Name;
        return asm.StartsWith("UnityEngine.") || asm == "UnityEngine.UI";
    }

    private void FindCurrentDllGuid(ScriptMapping mapping)
    {
        var mono = MonoImporter.GetAllRuntimeMonoScripts().FirstOrDefault(s =>
            s?.GetClass()?.Name == mapping.ClassName);

        if (mono != null)
        {
            string path = AssetDatabase.GetAssetPath(mono);
            if (!string.IsNullOrEmpty(path))
            {
                mapping.NewGuid = GetGuidFromMeta(path + ".meta");
                mapping.FoundInProject = true;
                return;
            }
        }
        mapping.NewGuid = FindDllGuidByAssembly(mapping.AssemblyName);
    }

    private string FindDllGuidByAssembly(string assemblyName)
    {
        string[] paths = { "Library/ScriptAssemblies", "Packages", "Assets/Plugins" };
        foreach (string basePath in paths)
        {
            if (!Directory.Exists(basePath)) continue;
            var files = Directory.GetFiles(basePath, "*.dll.meta", SearchOption.AllDirectories)
                .Where(f => Path.GetFileName(f).Contains(assemblyName));
            foreach (var f in files)
            {
                string guid = GetGuidFromMeta(f);
                if (!string.IsNullOrEmpty(guid)) return guid;
            }
        }
        return null;
    }

    private void ExportToJson()
    {
        string path = EditorUtility.SaveFilePanel("Save JSON", Application.dataPath,
            $"ScriptMappings_{DateTime.Now:yyyyMMdd_HHmm}.json", "json");
        if (string.IsNullOrEmpty(path)) return;

        var uniqueMappings = exportMappings.GroupBy(m => m.FullName).Select(g => g.First()).ToList();
        var data = new
        {
            exportDate = DateTime.Now.ToString(),
            totalDlls = loadedDllPaths.Count,
            totalMappings = exportMappings.Count,
            uniqueMappings = uniqueMappings.Count,
            mappings = uniqueMappings
        };

        File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented), System.Text.Encoding.UTF8);
        EditorUtility.DisplayDialog("Success", $"Saved {uniqueMappings.Count} mappings to:\n{path}", "OK");
    }

    // ========================================================================
    // AUTO-FIX TAB - DIRECT YAML REPLACEMENT
    // ========================================================================
    private void DrawAutoFixTab()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("⚡ Auto-Fix Scenes (Direct YAML)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Automatically fixes all Missing Scripts in scenes using JSON mapping", MessageType.Info);

        // JSON Path
        EditorGUILayout.BeginHorizontal();
        jsonPath = EditorGUILayout.TextField("JSON Mapping File:", jsonPath);
        if (GUILayout.Button("Browse", GUILayout.Width(70)))
            jsonPath = EditorUtility.OpenFilePanel("Select JSON", "", "json");
        EditorGUILayout.EndHorizontal();

        // Scene selection
        fixAllScenes = EditorGUILayout.ToggleLeft("Fix ALL scenes in project", fixAllScenes);

        if (!fixAllScenes)
        {
            EditorGUILayout.LabelField("Scenes to fix:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Current Scene"))
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                if (!string.IsNullOrEmpty(scene.path) && !scenesToFix.Contains(scene.path))
                    scenesToFix.Add(scene.path);
            }
            if (GUILayout.Button("Clear List"))
                scenesToFix.Clear();
            EditorGUILayout.EndHorizontal();

            if (scenesToFix.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                for (int i = 0; i < scenesToFix.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(Path.GetFileName(scenesToFix[i]), GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("X", GUILayout.Width(30)))
                        scenesToFix.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }

        // Analyze button
        if (GUILayout.Button("🔍 Analyze & Show Plan", GUILayout.Height(35)))
            AnalyzeAndFix();

        // Results
        if (fixResults.Count > 0)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"Results: {fixResults.Count} changes", EditorStyles.boldLabel);

            fixScroll = EditorGUILayout.BeginScrollView(fixScroll, GUILayout.Height(250));
            foreach (var result in fixResults)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"📄 {Path.GetFileName(result.ScenePath)}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"  Class: {result.ClassName}");
                EditorGUILayout.LabelField($"  Old GUID: {result.OldGuid}");
                EditorGUILayout.LabelField($"  New GUID: {result.NewGuid}");
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("✅ APPLY ALL CHANGES", GUILayout.Height(40)))
                ApplyAllChanges();
        }
        EditorGUILayout.EndVertical();
    }

    private void AnalyzeAndFix()
    {
        fixResults.Clear();

        if (string.IsNullOrEmpty(jsonPath) || !File.Exists(jsonPath))
        {
            EditorUtility.DisplayDialog("Error", "Select JSON file first!", "OK");
            return;
        }

        List<string> targetScenes = fixAllScenes ?
            Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories).ToList() :
            scenesToFix;

        if (targetScenes.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No scenes selected!", "OK");
            return;
        }

        try
        {
            var wrapper = JsonConvert.DeserializeObject<JsonWrapper>(File.ReadAllText(jsonPath));
            var guidMap = wrapper.mappings
                .Where(m => !string.IsNullOrEmpty(m.OldGuid) && !string.IsNullOrEmpty(m.NewGuid))
                .ToDictionary(m => m.OldGuid, m => m.NewGuid);

            foreach (string scenePath in targetScenes)
            {
                if (!File.Exists(scenePath)) continue;

                string content = File.ReadAllText(scenePath);
                var matches = Regex.Matches(content, @"m_Script:\s*\{fileID:\s*(-?\d+),\s*guid:\s*([a-f0-9]{32})");

                foreach (Match match in matches)
                {
                    string oldGuid = match.Groups[2].Value;
                    if (guidMap.TryGetValue(oldGuid, out string newGuid))
                    {
                        fixResults.Add(new FixResult
                        {
                            ScenePath = scenePath,
                            OldGuid = oldGuid,
                            NewGuid = newGuid,
                            ClassName = wrapper.mappings.FirstOrDefault(m => m.OldGuid == oldGuid)?.FullName ?? "Unknown"
                        });
                    }
                }
            }

            if (fixResults.Count == 0)
                EditorUtility.DisplayDialog("Info", "No missing scripts found in selected scenes", "OK");
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to analyze: {e.Message}", "OK");
        }
    }

    private void ApplyAllChanges()
    {
        if (fixResults.Count == 0) return;

        if (showWarnings && !EditorUtility.DisplayDialog("Confirm",
            $"Apply {fixResults.Count} changes?\n\nBackups: {(createBackups ? "Yes" : "No")}",
            "Yes, Apply", "Cancel"))
            return;

        int successCount = 0;
        var grouped = fixResults.GroupBy(r => r.ScenePath);

        foreach (var group in grouped)
        {
            try
            {
                string content = File.ReadAllText(group.Key);

                if (createBackups)
                {
                    string backup = group.Key + ".backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    File.Copy(group.Key, backup, true);
                }

                foreach (var result in group)
                {
                    string oldPattern = $"guid: {result.OldGuid}";
                    string newPattern = $"guid: {result.NewGuid}";
                    content = content.Replace(oldPattern, newPattern);
                    successCount++;
                }

                File.WriteAllText(group.Key, content);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to fix {group.Key}: {e.Message}");
            }
        }

        if (autoRefresh) AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", $"Applied {successCount} changes successfully!", "OK");
        fixResults.Clear();
    }

    // ========================================================================
    // SETTINGS TAB
    // ========================================================================
    private void DrawSettingsTab()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("⚙️ Settings", EditorStyles.boldLabel);

        createBackups = EditorGUILayout.ToggleLeft("Create backups before changes", createBackups);
        showWarnings = EditorGUILayout.ToggleLeft("Show confirmation dialogs", showWarnings);
        autoRefresh = EditorGUILayout.ToggleLeft("Auto-refresh AssetDatabase", autoRefresh);
        autoScanEnabled = EditorGUILayout.ToggleLeft("Auto-scan on selection", autoScanEnabled);
        includeBuiltIn = EditorGUILayout.ToggleLeft("Include built-in Unity components", includeBuiltIn);

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Reset to Defaults"))
        {
            createBackups = true;
            showWarnings = true;
            autoRefresh = true;
            autoScanEnabled = true;
            includeBuiltIn = false;
        }

        EditorGUILayout.HelpBox("Settings saved automatically", MessageType.Info);
        EditorGUILayout.EndVertical();
    }

    private void LoadSettings()
    {
        createBackups = EditorPrefs.GetBool("MSF_Backups", true);
        showWarnings = EditorPrefs.GetBool("MSF_Warnings", true);
        autoRefresh = EditorPrefs.GetBool("MSF_Refresh", true);
        autoScanEnabled = EditorPrefs.GetBool("MSF_AutoScan", true);
        includeBuiltIn = EditorPrefs.GetBool("MSF_BuiltIn", false);
    }

    private void SaveSettings()
    {
        EditorPrefs.SetBool("MSF_Backups", createBackups);
        EditorPrefs.SetBool("MSF_Warnings", showWarnings);
        EditorPrefs.SetBool("MSF_Refresh", autoRefresh);
        EditorPrefs.SetBool("MSF_AutoScan", autoScanEnabled);
        EditorPrefs.SetBool("MSF_BuiltIn", includeBuiltIn);
    }

    private string GetGuidFromMeta(string metaPath)
    {
        if (string.IsNullOrEmpty(metaPath) || !File.Exists(metaPath)) return null;
        try
        {
            var match = Regex.Match(File.ReadAllText(metaPath), @"guid:\s*([a-f0-9]{32})");
            return match.Success ? match.Groups[1].Value : null;
        }
        catch { return null; }
    }

    [System.Serializable]
    private class JsonWrapper { public List<ScriptMapping> mappings; }

    private class FixResult
    {
        public string ScenePath { get; set; }
        public string OldGuid { get; set; }
        public string NewGuid { get; set; }
        public string ClassName { get; set; }
    }
}

// ============================================================================
// MD4 & FILEID UTIL
// ============================================================================
public class MD4 : HashAlgorithm
{
    private uint _a, _b, _c, _d;
    private uint[] _x;
    private int _bytesProcessed;
    public MD4() { _x = new uint[16]; Initialize(); }
    public override void Initialize() { _a = 0x67452301; _b = 0xefcdab89; _c = 0x98badcfe; _d = 0x10325476; _bytesProcessed = 0; }
    protected override void HashCore(byte[] array, int offset, int length) { ProcessMessage(Bytes(array, offset, length)); }
    protected override byte[] HashFinal() { try { ProcessMessage(Padding()); return new[] { _a, _b, _c, _d }.SelectMany(word => Bytes(word)).ToArray(); } finally { Initialize(); } }
    private void ProcessMessage(IEnumerable<byte> bytes)
    {
        foreach (byte b in bytes)
        {
            int c = _bytesProcessed & 63;
            int i = c >> 2;
            int s = (c & 3) << 3;
            _x[i] = (_x[i] & ~((uint)255 << s)) | ((uint)b << s);
            if (c == 63) Process16WordBlock();
            _bytesProcessed++;
        }
    }
    private static IEnumerable<byte> Bytes(byte[] bytes, int offset, int length) { for (int i = offset; i < offset + length; i++) yield return bytes[i]; }
    private IEnumerable<byte> Bytes(uint word) { yield return (byte)(word & 255); yield return (byte)((word >> 8) & 255); yield return (byte)((word >> 16) & 255); yield return (byte)((word >> 24) & 255); }
    private IEnumerable<byte> Repeat(byte value, int count) { for (int i = 0; i < count; i++) yield return value; }
    private IEnumerable<byte> Padding() { return Repeat(128, 1).Concat(Repeat(0, ((_bytesProcessed + 8) & 0x7fffffc0) + 55 - _bytesProcessed)).Concat(Bytes((uint)_bytesProcessed << 3)).Concat(Repeat(0, 4)); }
    private void Process16WordBlock()
    {
        uint aa = _a, bb = _b, cc = _c, dd = _d;
        foreach (int k in new[] { 0, 4, 8, 12 }) { aa = Round1Operation(aa, bb, cc, dd, _x[k], 3); dd = Round1Operation(dd, aa, bb, cc, _x[k + 1], 7); cc = Round1Operation(cc, dd, aa, bb, _x[k + 2], 11); bb = Round1Operation(bb, cc, dd, aa, _x[k + 3], 19); }
        foreach (int k in new[] { 0, 1, 2, 3 }) { aa = Round2Operation(aa, bb, cc, dd, _x[k], 3); dd = Round2Operation(dd, aa, bb, cc, _x[k + 4], 5); cc = Round2Operation(cc, dd, aa, bb, _x[k + 8], 9); bb = Round2Operation(bb, cc, dd, aa, _x[k + 12], 13); }
        foreach (int k in new[] { 0, 2, 1, 3 }) { aa = Round3Operation(aa, bb, cc, dd, _x[k], 3); dd = Round3Operation(dd, aa, bb, cc, _x[k + 8], 9); cc = Round3Operation(cc, dd, aa, bb, _x[k + 4], 11); bb = Round3Operation(bb, cc, dd, aa, _x[k + 12], 15); }
        unchecked { _a += aa; _b += bb; _c += cc; _d += dd; }
    }
    private static uint ROL(uint value, int numberOfBits) => (value << numberOfBits) | (value >> (32 - numberOfBits));
    private static uint Round1Operation(uint a, uint b, uint c, uint d, uint xk, int s) => unchecked(ROL(a + ((b & c) | (~b & d)) + xk, s));
    private static uint Round2Operation(uint a, uint b, uint c, uint d, uint xk, int s) => unchecked(ROL(a + ((b & c) | (b & d) | (c & d)) + xk + 0x5a827999, s));
    private static uint Round3Operation(uint a, uint b, uint c, uint d, uint xk, int s) => unchecked(ROL(a + (b ^ c ^ d) + xk + 0x6ed9eba1, s));
}

public static class FileIDUtil
{
    public static int Compute(Type t)
    {
        string toBeHashed = "s\0\0\0" + t.Namespace + t.Name;
        using (HashAlgorithm hash = new MD4())
        {
            byte[] hashed = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(toBeHashed));
            int result = 0;
            for (int i = 3; i >= 0; --i) { result <<= 8; result |= hashed[i]; }
            return result;
        }
    }
}