using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TranslationReader
{
    private static string _translationPath;

    public static readonly Dictionary<string, string[]> Elements;

    public static readonly Dictionary<string, string[]> Fallback;

    private static readonly Dictionary<string, Dictionary<int, Dictionary<string, int>>> _positions;

    private static readonly Regex _matchFormat;

    private static TMP_FontAsset[] defaultFallbacks;

    public const string NoTranslation = "NO_TRANSLATION";

    public const string TranslationDirectory = "Translations/";

    public static string TranslationPath
    {
        get => _translationPath;
        private set
        {
            _translationPath = Path.GetFullPath(value);
            TranslationDirectoryName = Path.GetFileName(_translationPath);
        }
    }

    public static string TranslationDirectoryName { get; private set; }

    public static TranslationManifest TranslationManifest { get; private set; }

    public static CultureInfo TranslationCulture { get; private set; }

    public static event Action OnTranslationsRefreshed;

    static TranslationReader()
    {
        Elements = new Dictionary<string, string[]>();
        Fallback = new Dictionary<string, string[]>();
        _positions = new Dictionary<string, Dictionary<int, Dictionary<string, int>>>();
        _matchFormat = new Regex(@"\{.*?\}|\[.*?\]");
        Refresh();
        LoadPositions();
        SceneManager.sceneLoaded += OnSceneWasLoaded;
    }

    private static void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerPrefsSl.Refresh();
        Refresh();
    }

    [RuntimeInitializeOnLoadMethod]
    public static void Refresh()
    {
        TranslationPath = GetTranslationPath();
        TranslationManifest = LoadTranslation(TranslationPath, Elements);
        
        string[] fallbackPaths = new string[] { "en", "English (default)" };
        string fallbackPath = CheckPath(fallbackPaths);
        if (fallbackPath != null)
        {
            LoadTranslation(fallbackPath, Fallback);
        }
        
        TranslationCulture = null;

        string[] interfaceLocales = TranslationManifest.InterfaceLocales ?? Array.Empty<string>();
        foreach (string name in interfaceLocales)
        {
            try
            {
                TranslationCulture = CultureInfo.GetCultureInfo(name);
                break;
            }
            catch
            {
                TranslationCulture = null;
            }
        }
        
        if (TranslationCulture == null)
        {
            TranslationCulture = CultureInfo.CurrentCulture;
        }
        
        CultureInfo.CurrentCulture = TranslationCulture;
        CultureInfo.CurrentUICulture = TranslationCulture;
        
        TMP_Settings.fallbackFontAssets.Clear();
        TMP_Settings.fallbackFontAssets.AddRange(defaultFallbacks ?? Array.Empty<TMP_FontAsset>());

        while (TMP_Settings.fallbackFontAssets.Count > 0 && 
               TMP_Settings.fallbackFontAssets[0].name.EndsWith("Dynamic", StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"Removing {TMP_Settings.fallbackFontAssets[0].name} from first fallback");
            TMP_Settings.fallbackFontAssets.RemoveAt(0);
        }

        string manifestPath = Path.Combine(TranslationPath, "manifest.json");
        if (File.Exists(manifestPath))
        {
            string manifestJson = File.ReadAllText(manifestPath);
            TranslationManifest manifest = JsonSerialize.FromJson<TranslationManifest>(manifestJson);
            
            if (manifest.InterfaceLocales != null)
            {
                for (int i = manifest.InterfaceLocales.Length - 1; i >= 0; i--)
                {
                    string fontName = manifest.InterfaceLocales[i];
                    
                    if (fontName != null && fontName.EndsWith("Dynamic", StringComparison.OrdinalIgnoreCase))
                    {
                        TMP_FontAsset fontAsset = TMP_Settings.fallbackFontAssets
                            .FirstOrDefault(asset => asset != null && 
                                         asset.name.Equals(fontName, StringComparison.OrdinalIgnoreCase));
                        
                        if (fontAsset != null)
                        {
                            Debug.Log($"Forcing {fontAsset.name} as first fallback");
                            TMP_Settings.fallbackFontAssets.Insert(0, fontAsset);
                        }
                    }
                }
            }
        }
        
        Translations.ResetCache();
        OnTranslationsRefreshed?.Invoke();
    }

    private static string GetTranslationPath()
    {
        if (PlayerPrefsSl.Get("translation_changed", false))
        {
            string savedPath = PlayerPrefsSl.Get("translation_path", string.Empty);
            if (!string.IsNullOrEmpty(savedPath))
            {
                string[] checkPaths = new string[] { savedPath };
                string result = CheckPath(checkPaths);
                if (result != null)
                {
                    return result;
                }
            }
        }
        List<(string path, TranslationManifest manifest)> translations = new List<(string, TranslationManifest)>();

        foreach (string file in Directory.EnumerateFiles(TranslationDirectory, "manifest.json", SearchOption.AllDirectories))
        {
            string fullPath = Path.GetFullPath(file);
            string dirPath = Path.GetDirectoryName(fullPath);
            TranslationManifest manifest = JsonSerialize.FromFile<TranslationManifest>(fullPath);
            
            if (manifest != null && Directory.Exists(dirPath))
            {
                translations.Add((dirPath, manifest));
            }
        }

        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        string currentName = currentCulture.Name;
        
        foreach (var (path, manifest) in translations)
        {
            if (manifest.InterfaceLocales != null && 
                manifest.InterfaceLocales.Contains(currentName, StringComparer.OrdinalIgnoreCase))
            {
                return path;
            }
        }

        CultureInfo parentCulture = currentCulture.Parent;
        if (parentCulture != null && !string.IsNullOrEmpty(parentCulture.Name))
        {
            string parentName = parentCulture.Name;
            foreach (var (path, manifest) in translations)
            {
                if (manifest.InterfaceLocales != null && 
                    manifest.InterfaceLocales.Contains(parentName, StringComparer.OrdinalIgnoreCase))
                {
                    return path;
                }
            }
        }
        
        string[] defaultPaths = new string[] { "en", "English (default)" };
        string defaultResult = CheckPath(defaultPaths);
        
        if (defaultResult != null)
        {
            return defaultResult;
        }
        
        throw new DirectoryNotFoundException();
    }

    private static void LoadPositions()
    {
        string[] checkPaths = new string[] { "en", "English (default)" };
        string basePath = CheckPath(checkPaths);
        
        if (basePath == null)
        {
            return;
        }
        
        string[] files = Directory.GetFiles(basePath, "*.txt");
        
        foreach (string filePath in files)
        {
            string[] lines = File.ReadAllLines(filePath);
            Dictionary<int, Dictionary<string, int>> filePositions = new Dictionary<int, Dictionary<string, int>>();
            
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                Dictionary<string, int> lineMatches = new Dictionary<string, int>();
                MatchCollection matches = _matchFormat.Matches(lines[lineIndex]);
                
                for (int matchIndex = 0; matchIndex < matches.Count; matchIndex++)
                {
                    string matchValue = matches[matchIndex].Value;
                    if (!lineMatches.ContainsKey(matchValue))
                    {
                        lineMatches.Add(matchValue, matchIndex);
                    }
                }
                
                filePositions.Add(lineIndex, lineMatches);
            }
            
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            _positions[fileName] = filePositions;
        }
    }

    private static TranslationManifest LoadTranslation(string translationPath, Dictionary<string, string[]> dictionary)
    {
        dictionary.Clear();

        string legacyPath = Path.Combine(translationPath, "Legacy_Interfaces.txt");
        string legancyPath = Path.Combine(translationPath, "Legancy_Interfaces.txt");
        
        if (File.Exists(legacyPath) && File.Exists(legancyPath))
        {
            File.Delete(legancyPath);
        }
        
        foreach (string filePath in Directory.EnumerateFiles(translationPath, "*.txt"))
        {
            string[] lines = FileManager.ReadAllLines(filePath);
            
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace("\\n", Environment.NewLine);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (_positions.TryGetValue(fileName, out var filePos) && 
                    filePos.TryGetValue(i, out var linePos))
                {
                    foreach (Match match in _matchFormat.Matches(lines[i]))
                    {
                        if (linePos.TryGetValue(match.Value, out var placeholderIndex))
                        {
                            lines[i] = lines[i].Replace(match.Value, "{" + placeholderIndex + "}");
                        }
                    }
                }
            }
            
            string key = Path.GetFileNameWithoutExtension(filePath);
            if (key == "Legancy_Interfaces")
            {
                key = "Legacy_Interfaces";
            }
            
            dictionary[key] = lines;
        }
        
        string manifestPath = Path.Combine(translationPath, "manifest.json");
        try
        {
            return JsonSerialize.FromFile<TranslationManifest>(manifestPath);
        }
        catch (FileNotFoundException)
        {
            return new TranslationManifest(
                Path.GetFileName(translationPath),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()
            );
        }
    }

    private static string CheckPath(params string[] suffixes)
    {
        foreach (string suffix in suffixes)
        {
            if (!string.IsNullOrWhiteSpace(suffix))
            {
                string path = TranslationDirectory + suffix;
                if (Directory.Exists(path))
                {
                    return path;
                }
            }
        }
        return null;
    }

    public static string[] GetKeys(string keyName)
    {
        if (keyName == "Legancy_Interfaces")
        {
            keyName = "Legacy_Interfaces";
        }

        if (Elements.TryGetValue(keyName, out var value))
        {
            return value;
        }
        
        Debug.LogWarning("Tried to get translation from nonexistent file " + keyName);

        if (Fallback.TryGetValue(keyName, out var fallbackValue))
        {
            return fallbackValue;
        }
        
        Debug.LogWarning("Tried to get **FALLBACK** translation from nonexistent file " + keyName);
        return null;
    }

    public static string[] GetFallbackKeys(string keyName)
    {
        if (keyName == "Legancy_Interfaces")
        {
            keyName = "Legacy_Interfaces";
        }
        
        if (Fallback.TryGetValue(keyName, out var value))
        {
            return value;
        }
        return null;
    }

    public static string Get(string keyName, int index, string defaultValue = NoTranslation)
    {
        if (GameCore.Console.TranslationDebugMode)
        {
            return $"{keyName}:{index}";
        }
        
        if (keyName == "Legancy_Interfaces")
        {
            keyName = "Legacy_Interfaces";
        }

        if (Elements.TryGetValue(keyName, out var elementArray) &&
            index >= 0 && index < elementArray.Length &&
            !string.IsNullOrWhiteSpace(elementArray[index]))
        {
            return elementArray[index];
        }

        string result = GetFallback(keyName, index, defaultValue);
        return result ?? defaultValue;
    }

    public static bool TryGet(string keyName, int index, out string val)
    {
        if (Elements.TryGetValue(keyName, out var elementArray) && 
            index < elementArray.Length)
        {
            string element = elementArray[index];
            if (!string.IsNullOrWhiteSpace(element))
            {
                val = element;
                return true;
            }
        }

        if (Fallback.TryGetValue(keyName, out var fallbackArray) && 
            fallbackArray.TryGet(index, out var fallbackElement) && 
            !string.IsNullOrWhiteSpace(fallbackElement))
        {
            val = fallbackElement;
            return true;
        }

        val = GetFallback(keyName, index, NoTranslation);
        return false;
    }

    private static string GetFallback(string keyName, int index, string defaultvalue)
    {
        if (Fallback.TryGetValue(keyName, out var value) && 
            value.TryGet(index, out var element) && 
            !string.IsNullOrWhiteSpace(element))
        {
            return element;
        }

        string safeDefault = defaultvalue?.Replace("<", "(<<)") ?? "";
        Debug.LogWarning(string.Format(
            "Missing **FALLBACK** translation! {0}:{1}. Default value: {2}", 
            keyName, index, safeDefault));
        
        return null;
    }

    public static string GetFormatted(string keyName, int index, string defaultvalue, object obj1)
    {
        return string.Format(Get(keyName, index, defaultvalue), obj1);
    }

    public static string GetFormatted(string keyName, int index, string defaultvalue, object obj1, object obj2)
    {
        return string.Format(Get(keyName, index, defaultvalue), obj1, obj2);
    }

    public static string GetFormatted(string keyName, int index, string defaultvalue, object obj1, object obj2, object obj3)
    {
        return string.Format(Get(keyName, index, defaultvalue), obj1, obj2, obj3);
    }

    public static string GetFormatted(string keyName, int index, string defaultvalue, params object[] format)
    {
        return string.Format(Get(keyName, index, defaultvalue), format);
    }
}