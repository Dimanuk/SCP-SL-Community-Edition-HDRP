using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

	public static class CreditsData
	{
		private static bool RemoteLoaded; 
		public static string Patreons =
	@"nz
[TFB]Bornjester
Dragon
ok yes
wiggy";
		private static string CurrentNicknamePlaceholder = "<current nickname>";
		private static string CachePath;
		public static CreditsCategory[] Data;

		static CreditsData()
		{
			string appFolder = FileManager.GetAppFolder(true, false, "");
			CachePath = appFolder + "CreditsCache.json";

			if (!File.Exists(CachePath))
			{
				File.Copy("CreditsCache.json", CachePath);
			}

			string json = File.ReadAllText(CachePath);
			SetCredits(json);
		}

		internal static void LoadData(string text)
		{
			SetCredits(text);
			File.WriteAllText(CachePath, text);
		}

		private static void SetCredits(string text)
		{
			CreditsList creditsList = JsonSerialize.FromJson<CreditsList>(text);
			var categoriesList = new List<CreditsCategory>();

			var manifestFiles = Directory.EnumerateFiles("Translations/", "manifest.json", SearchOption.AllDirectories);
			var manifests = manifestFiles.Select(JsonSerialize.FromFile<TranslationManifest>).ToList();

			foreach (var categoryData in creditsList.Members)
			{
				var entriesList = new List<CreditsEntry>();

				foreach (var member in categoryData.members)
				{
					CreditsEntry entry = ProcessEntry(member);
					if (entry != null)
						entriesList.Add(entry);
				}

				if (categoryData.category == "SPECIAL THANKS")
				{
					string thankYouText = TranslationReader.Get("NewMainMenu", 87, "For playing our game!");
					Color thankYouColor = new Color32(0xD7, 0xFF, 0xFF, 0xFF);
					var specialEntry = new CreditsEntry(thankYouText, CurrentNicknamePlaceholder, thankYouColor);
					entriesList.Add(specialEntry);

					categoriesList.Add(new CreditsCategory
					{
						Header = categoryData.category,
						Records = entriesList.ToArray(),
						DisplayTime = 2000
					});

					// Community translators are appended once, right after SPECIAL THANKS,
					// reusing the (now cleared) entries list — one entry per author.
					entriesList.Clear();

					foreach (var manifest in manifests)
					{
						if (manifest.Name == "English (default)")
							continue;

						string languageName = manifest.Name;
						int dashIndex = languageName.IndexOf('-');
						if (dashIndex >= 0)
							languageName = languageName.Substring(dashIndex + 2);

						if (manifest.Authors != null)
						{
							foreach (var author in manifest.Authors)
								entriesList.Add(new CreditsEntry(languageName, author));
						}

						// Blank spacer between languages.
						entriesList.Add(new CreditsEntry("", ""));
					}

					categoriesList.Add(new CreditsCategory
					{
						Header = "COMMUNITY TRANSLATORS",
						Records = entriesList.ToArray(),
						DisplayTime = 2000
					});
				}
				else
				{
					categoriesList.Add(new CreditsCategory
					{
						Header = categoryData.category,
						Records = entriesList.ToArray(),
						DisplayTime = 2000
					});
				}
			}

			Data = categoriesList.ToArray();
			RemoteLoaded = true; 
		}

		private static CreditsEntry ProcessEntry(CreditsListMember member)
		{

			if (string.IsNullOrEmpty(member.name))
				return new CreditsEntry();

			if (string.IsNullOrEmpty(member.title))
			{
				return new CreditsEntry(member.name);
			}

			Color32 color32 = HexToColor(member);
			Color color = color32;
			return new CreditsEntry(member.title, member.name, color);
		}

		private static Color32 HexToColor(CreditsListMember member)
		{
			if (Misc.TryParseColor(member.color, out Color32 parsedColor))
				return parsedColor;

			var errorParts = new string[7];
			errorParts[0] = "Error during processing credits color (";
			errorParts[1] = member.title ?? "";
			errorParts[2] = " - ";
			errorParts[3] = member.name ?? "";
			errorParts[4] = " - ";
			errorParts[5] = member.color ?? "";
			errorParts[6] = ").";
			string errorMsg = string.Concat(errorParts);
			Debug.LogError(errorMsg);

			return (Color32)Color.white;
		}
	}
