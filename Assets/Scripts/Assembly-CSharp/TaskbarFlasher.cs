using System;
using System.Runtime.InteropServices;
using PlayerRoles;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TaskbarFlasher
{
#if UNITY_STANDALONE_WIN
    private struct FlashWindowInfo
    {
        public uint Size;
        public IntPtr Handle;
        public uint Flags;
        public uint Count;
        public uint Timeout;
    }

    private const string User32 = "user32.dll";
#endif

    private static IntPtr _windowHandle;

    public static bool EnableSpawnFlash { get; set; }

    private static IntPtr WindowHandle
    {
        get
        {
#if UNITY_STANDALONE_WIN
            if (_windowHandle != IntPtr.Zero)
                return _windowHandle;

            _windowHandle = FindWindow("UnityWndClass", "SCPSL");

            if (_windowHandle == IntPtr.Zero)
                _windowHandle = GetWindow();

            return _windowHandle;
#else
            return IntPtr.Zero;
#endif
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void RegisterEvent()
    {
        _ = WindowHandle;

        PlayerRoleManager.OnRoleChanged += OnRoleChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetFlash(false);
    }

    private static void OnRoleChanged(ReferenceHub hub, PlayerRoleBase oldRole, PlayerRoleBase newRole)
    {
        if (!hub.isLocalPlayer)
            return;

        bool roleChanged = oldRole.RoleTypeId != newRole.RoleTypeId;
        SetFlash(roleChanged);
    }

    private static void SetFlash(bool enabled)
    {
        if (!EnableSpawnFlash)
            return;

        IntPtr handle = WindowHandle;
        if (handle == IntPtr.Zero)
            return;

#if UNITY_STANDALONE_WIN
        var info = new FlashWindowInfo
        {
            Size = (uint)Marshal.SizeOf(typeof(FlashWindowInfo)),
            Handle = handle,
            Flags = enabled ? 14u : 0u, 
            Count = uint.MaxValue,
            Timeout = 0
        };

        try
        {
            FlashWindow(ref info);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
#endif
    }

#if UNITY_STANDALONE_WIN
    [DllImport(User32)]
    private static extern IntPtr GetWindow();

    [DllImport(User32)]
    private static extern IntPtr FindWindow(string windowClass, string windowName);

    [DllImport(User32)]
    private static extern bool FlashWindow(ref FlashWindowInfo info);
#endif
}