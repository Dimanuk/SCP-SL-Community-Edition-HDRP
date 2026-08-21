#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace FontEditorTools
{
	// Repairs the "…Dynamic" TMP font assets whose non-Latin glyphs render as empty squares (tofu),
	// e.g. Russian/Cyrillic, CJK, Arabic, Georgian, Armenian, etc.
	//
	// ROOT CAUSE (found by inspecting the .asset YAML):
	//   The fallback fonts referenced from "TMP Settings" (NotoSans-RegularDynamic, the CJK/Arabic/…
	//   Noto variants and the Dynamic* Roboto/Teko/etc.) were DYNAMIC atlas fonts in the original
	//   v12 build — they render glyphs from the linked .ttf/.otf at runtime. AssetRipper exported them
	//   with:
	//     * m_AtlasPopulationMode: 0  (Static)  -> should be 1 (Dynamic); runtime never populates glyphs
	//     * empty m_GlyphTable / m_CharacterTable
	//     * a DANGLING m_AtlasTextures[0] reference (guid points at nothing)
	//   The default font (LiberationSans SDF) is Latin-only, so any Cyrillic/CJK char falls through the
	//   fallback chain to these empty shells and TMP draws the missing-glyph box.
	//   The source .ttf/.otf files are all present in Assets/Font and are still linked (sourceFontFile),
	//   so the fix is to restore Dynamic mode and give each font a fresh, writable Alpha8 atlas texture.
	//
	// WHAT THIS DOES (in place, preserving each asset's GUID so the TMP Settings fallback list and every
	// UI reference stay intact):
	//   1. sets atlasPopulationMode = Dynamic
	//   2. creates a fresh readable Alpha8 atlas texture, embeds it as a sub-asset
	//   3. clears the (empty) tables and re-inits via ClearFontAssetData
	//   4. repoints the font material's _MainTex at the new atlas
	//
	// Only empty fonts that still have a source font file are touched; baked static fonts
	// (LiberationSans SDF, NotoSans-RegularStatic, …) are skipped. Reversible via source control.
	public static class DynamicTmpFontRepair
	{
		[MenuItem("Tools/Fonts/Repair Dynamic TMP Fonts (fix tofu ▯)")]
		public static void Repair()
		{
			FontEngine.InitializeFontEngine();

			string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
			var repaired = new List<string>();
			var skipped = new List<string>();

			AssetDatabase.StartAssetEditing();
			try
			{
				foreach (string guid in guids)
				{
					string path = AssetDatabase.GUIDToAssetPath(guid);
					var fa = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
					if (fa == null)
						continue;

					bool empty = fa.glyphTable == null || fa.glyphTable.Count == 0;
					bool hasSource = fa.sourceFontFile != null;
					bool needsFix = empty && hasSource &&
					                (fa.atlasPopulationMode != AtlasPopulationMode.Dynamic ||
					                 fa.atlasTexture == null);

					if (!needsFix)
					{
						skipped.Add(fa.name);
						continue;
					}

					RepairOne(fa, path);
					repaired.Add(fa.name);
				}
			}
			finally
			{
				AssetDatabase.StopAssetEditing();
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			var sb = new StringBuilder();
			sb.AppendLine($"[DynamicTmpFontRepair] Repaired {repaired.Count} font(s), skipped {skipped.Count}.");
			if (repaired.Count > 0)
				sb.AppendLine("Repaired: " + string.Join(", ", repaired));
			Debug.Log(sb.ToString());
		}

		private static void RepairOne(TMP_FontAsset fa, string path)
		{
			int w = fa.atlasWidth > 0 ? fa.atlasWidth : 1024;
			int h = fa.atlasHeight > 0 ? fa.atlasHeight : 1024;

			// 1) Dynamic mode so glyphs populate from sourceFontFile at runtime.
			fa.atlasPopulationMode = AtlasPopulationMode.Dynamic;

			// 2) Fresh, readable, empty Alpha8 atlas texture (the shipped reference was dangling/null).
			var tex = new Texture2D(w, h, TextureFormat.Alpha8, false)
			{
				name = fa.name + " Atlas"
			};
			
			// --- ИЗМЕНЕНИЕ ДЛЯ UNITY 6 ---
			// Вместо FontEngine.ResetAtlasTexture(tex) заполняем текстуру нулевыми байтами (прозрачный Alpha8)
			byte[] clearBytes = new byte[w * h]; // по умолчанию заполнен нулями (0x00)
			tex.SetPixelData(clearBytes, 0);
			tex.Apply(false, false);
			// -----------------------------

			fa.atlasTextures = new[] { tex };

			// Embed the atlas inside the font asset (so it saves with the font, same GUID).
			AssetDatabase.AddObjectToAsset(tex, path);

			// 3) Normalise tables / free-glyph rects and rebuild the runtime lookup dictionaries.
			// В Unity 6 этот метод корректно перенастроит внутренние Rects (свободные зоны) для нового атласа.
			fa.ClearFontAssetData(false);

			// 4) Point the font material at the real atlas.
			Material mat = fa.material;
			if (mat != null)
			{
				mat.SetTexture(ShaderUtilities.ID_MainTex, tex);
				mat.SetFloat(ShaderUtilities.ID_TextureWidth, w);
				mat.SetFloat(ShaderUtilities.ID_TextureHeight, h);
				mat.SetFloat(ShaderUtilities.ID_GradientScale, fa.atlasPadding + 1);
				EditorUtility.SetDirty(mat);
			}

			EditorUtility.SetDirty(fa);
		}
	}
}
#endif
