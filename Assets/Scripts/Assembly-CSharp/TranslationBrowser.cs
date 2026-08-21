using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class TranslationBrowser : MonoBehaviour
{
	public static List<(string name, bool isComplete)> Translations = new List<(string, bool)>();
	public static Dictionary<string, string> Languages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	public static Dictionary<string, string> Names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	public Text instancePrefab;
	public Transform parent;
	public List<GameObject> spawns = new List<GameObject>();

	static TranslationBrowser()
	{
		Translations = new List<(string, bool)>();
		Languages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		Names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	}

	public TranslationBrowser()
	{
		spawns = new List<GameObject>();
	}

	private void OnEnable()
	{
		foreach (var go in spawns)
		{
			if (go != null) Destroy(go);
		}
		spawns.Clear();

		GetTranslationList();

		foreach (var translation in Translations)
		{
			if (instancePrefab == null || parent == null) continue;

			Text newText = Instantiate(instancePrefab, parent);
			newText.transform.localScale = Vector3.one;
			newText.text = translation.name;

			if (!translation.isComplete)
			{
				newText.color = Color.red;
			}

			spawns.Add(newText.gameObject);
		}
	}

	private void OnDisable()
	{
		foreach (var go in spawns)
		{
			if (go != null) Destroy(go);
		}
		spawns.Clear();
	}

	public static int GetTranslationList()
	{
		string[] directories = Directory.GetDirectories("Translations");
		string currentDirectory = TranslationReader.TranslationDirectoryName;

		Translations.Clear();
		Languages.Clear();
		Names.Clear();

		int selectedIndex = 0;

		for (int i = 0; i < directories.Length; i++)
		{
			string dirPath = directories[i];
			string dirName = Path.GetFileName(dirPath);
			bool isCurrent = dirName.Equals(currentDirectory, StringComparison.OrdinalIgnoreCase);

			string manifestPath = Path.Combine(dirPath, "manifest.json");
			string displayName;
			bool hasManifest = File.Exists(manifestPath);

			if (hasManifest)
			{
				string json = File.ReadAllText(manifestPath);
				var manifest = JsonSerialize.FromJson<TranslationManifest>(json);

				if (manifest != null && !string.IsNullOrWhiteSpace(manifest.Name))
				{
					displayName = GetFreeName(manifest.Name);
					Names[displayName] = dirName;
				}
				else
				{
					if (Misc.CultureInfoTryParse(dirName, out CultureInfo ci))
					{
						displayName = GetFreeName(GetTranslationName(ci));
						Languages[displayName] = dirName;
					}
					else
					{
						displayName = GetFreeName(dirName);
					}
				}
			}
			else
			{
				if (Misc.CultureInfoTryParse(dirName, out CultureInfo ci))
				{
					displayName = GetFreeName(GetTranslationName(ci));
					Languages[displayName] = dirName;
				}
				else
				{
					displayName = GetFreeName(dirName);
				}
			}

			bool hasTxtFiles = Directory.EnumerateFiles(dirPath, "*.txt").Any();
			Translations.Add((displayName, hasTxtFiles));

			if (isCurrent)
			{
				selectedIndex = i;
			}
		}

		return selectedIndex;
	}

	public static string NameToDirectory(string name)
	{
		if (Names.TryGetValue(name, out string dirFromNames))
			return dirFromNames;

		if (Languages.TryGetValue(name, out string dirFromLanguages))
			return dirFromLanguages;

		return name;
	}

	private static string GetFreeName(string name)
	{
		int suffix = 1;
		string uniqueName = name;
		while (Languages.ContainsKey(uniqueName) || Names.ContainsKey(uniqueName))
		{
			uniqueName = $"{name} {suffix}";
			suffix++;
		}
		return uniqueName;
	}

	private static string GetTranslationName(CultureInfo c)
	{
		string nativeName = FirstToUpper(c.Name, c);
		string displayName = FirstToUpper(c.DisplayName, c);
		return $"{nativeName} - {displayName}";
	}

	private static string FirstToUpper(string s, CultureInfo c)
	{
		if (string.IsNullOrEmpty(s) || !char.IsLower(s[0]))
			return s;

		return char.ToUpper(s[0], c) + s.Substring(1);
	}
}