using System.IO;
using UnityEditor;
using UnityEngine;

public static class AudioClipOptimizer
{
    private const float StreamingLengthThreshold = 12f;
    private const long StreamingSizeThreshold = 300 * 1024;
    private const float CompressedLengthThreshold = 1.5f;
    private const long CompressedSizeThreshold = 40 * 1024;

    [MenuItem("Tools/Audio/Optimize All Audio Clips (Load Type + Preload)")]
    public static void OptimizeAll()
    {
        string[] guids = AssetDatabase.FindAssets("t:AudioClip");
        int changed = 0;

        try
        {
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                if (EditorUtility.DisplayCancelableProgressBar("Optimizing Audio Clips", path, (float)i / guids.Length))
                    break;

                if (!(AssetImporter.GetAtPath(path) is AudioImporter importer))
                    continue;

                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                float length = clip != null ? clip.length : 0f;
                long fileSize = File.Exists(path) ? new FileInfo(path).Length : 0;

                AudioClipLoadType targetLoadType;
                bool targetPreload;

                if (length >= StreamingLengthThreshold || fileSize >= StreamingSizeThreshold)
                {
                    targetLoadType = AudioClipLoadType.Streaming;
                    targetPreload = false;
                }
                else if (length >= CompressedLengthThreshold || fileSize >= CompressedSizeThreshold)
                {
                    targetLoadType = AudioClipLoadType.CompressedInMemory;
                    targetPreload = false;
                }
                else
                {
                    targetLoadType = AudioClipLoadType.DecompressOnLoad;
                    targetPreload = true;
                }

                AudioImporterSampleSettings settings = importer.defaultSampleSettings;
                bool dirty = false;

                if (settings.loadType != targetLoadType || settings.preloadAudioData != targetPreload)
                {
                    settings.loadType = targetLoadType;
                    settings.preloadAudioData = targetPreload;
                    importer.defaultSampleSettings = settings;
                    dirty = true;
                }

                if (!importer.loadInBackground)
                {
                    importer.loadInBackground = true;
                    dirty = true;
                }

                // Ripped assets often carry a Standalone override that silently
                // wins over defaultSampleSettings — drop it so the defaults apply.
                if (importer.ContainsSampleSettingsOverride("Standalone"))
                {
                    importer.ClearSampleSettingOverride("Standalone");
                    dirty = true;
                }

                if (dirty)
                {
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                    changed++;
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Debug.Log($"[AudioClipOptimizer] Done. Reimported {changed} / {guids.Length} audio clips.");
    }
}
