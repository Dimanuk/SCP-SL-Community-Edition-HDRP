using System;
using System.Runtime;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MemoryCleaner
{
    [RuntimeInitializeOnLoadMethod]
    public static void OnLoad()
    {
        SceneManager.sceneLoaded += CleanupMemory;
    }

    public static void CleanupMemory(Scene scene, LoadSceneMode mode)
    {
        Resources.UnloadUnusedAssets();
        Thread thread = new((ThreadStart)delegate
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        })
        {
            IsBackground = true
        };
        thread.Start();
    }
}