using System;
using System.Runtime.InteropServices;
using UnityEngine;

internal static class LauncherCommunicator
{
    private delegate void PostAuthDelegate();
    private delegate void SendDelegate(string data, out IntPtr response);
    private delegate void FreeStringDelegate(IntPtr ptr);

    private static SendDelegate _send;
    private static PostAuthDelegate _postAuth;
    private static FreeStringDelegate _freeString;

    private static IntPtr _moduleHandle = IntPtr.Zero;
    private static bool _loadedByUs = false;
    private static bool _initialized = false;

    private static IntPtr _shmHandle = IntPtr.Zero;
    private static IntPtr _shmView = IntPtr.Zero;

    private const string LauncherDll = "launcher_bridge.dll";
    private const string SharedMemName = "LauncherAssetVerification";
    private const int SharedMemSize = 8; 

    internal static bool IsAvailable
    {
        get
        {
#if UNITY_EDITOR
            return _initialized; 
#else
            return _initialized && _send != null;
#endif
        }
    }

    internal static float AssetVerificationProgress
    {
        get
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR)
            if (_shmView == IntPtr.Zero) return 0f;
            return Marshal.PtrToStructure<float>(_shmView);
#else
            return 0f;
#endif
        }
    }

    internal static bool VerificationFinished
    {
        get
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR)
            if (_shmView == IntPtr.Zero) return false;
            return Marshal.ReadByte(_shmView, 4) != 0;
#else
            return false;
#endif
        }
    }

    internal static void Initialize()
    {
        if (_initialized) return;

#if UNITY_EDITOR
        Debug.Log("[LauncherCommunicator] Editor mode: skipping native init, marking available.");
        _initialized = true;
        return;
#elif !UNITY_STANDALONE_WIN
        Debug.LogWarning("[LauncherCommunicator] Only supported on Windows. Skipping.");
        return;
#else
        try
        {
            // 1. Check if the launcher already injected the DLL.
            _moduleHandle = GetModuleHandle(LauncherDll);

            // 2. If not injected, try to load it ourselves.
            if (_moduleHandle == IntPtr.Zero)
            {
                _moduleHandle = LoadLibraryW(LauncherDll);
                _loadedByUs   = (_moduleHandle != IntPtr.Zero);
            }

            if (_moduleHandle == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                Debug.LogWarning($"[LauncherCommunicator] '{LauncherDll}' not found (Win32 error {err}). Running without launcher.");
                return;
            }

            _send       = GetDelegate<SendDelegate>("getcommsptr");
            _postAuth   = GetDelegate<PostAuthDelegate>("getpostauthptr");
            _freeString = GetDelegate<FreeStringDelegate>("freeresponse");

            InitSharedMemory();

            _initialized = true;
            Debug.Log($"[LauncherCommunicator] Ready. " +
                      $"Send={_send != null}, PostAuth={_postAuth != null}, " +
                      $"FreeString={_freeString != null}, SharedMem={_shmView != IntPtr.Zero}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LauncherCommunicator] Initialize failed: {ex}");
        }
#endif
    }

    internal static string Send(string data)
    {
        if (_send == null) return string.Empty;

        try
        {
            _send(data, out IntPtr responsePtr);
            if (responsePtr == IntPtr.Zero) return string.Empty;

            string result = Marshal.PtrToStringUni(responsePtr) ?? string.Empty;
            _freeString?.Invoke(responsePtr);
            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LauncherCommunicator] Send failed: {ex.Message}");
            return string.Empty;
        }
    }

    internal static void Heartbeat() => _postAuth?.Invoke();

    internal static void Shutdown()
    {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR)
        if (_shmView   != IntPtr.Zero) { UnmapViewOfFile(_shmView);   _shmView   = IntPtr.Zero; }
        if (_shmHandle != IntPtr.Zero) { CloseHandle(_shmHandle);     _shmHandle = IntPtr.Zero; }

        if (_loadedByUs && _moduleHandle != IntPtr.Zero)
            FreeLibrary(_moduleHandle);
#endif
        _moduleHandle = IntPtr.Zero;
        _loadedByUs = false;
        _initialized = false;
        _send = null;
        _postAuth = null;
        _freeString = null;
    }

    private static void InitSharedMemory()
    {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR)
        _shmHandle = OpenFileMappingW(0x0004, false, SharedMemName);

        if (_shmHandle == IntPtr.Zero)
        {
            Debug.LogWarning($"[LauncherCommunicator] Shared memory '{SharedMemName}' not found – progress unavailable.");
            return;
        }

        _shmView = MapViewOfFile(_shmHandle, 0x0004, 0, 0, (UIntPtr)SharedMemSize);

        if (_shmView == IntPtr.Zero)
        {
            Debug.LogWarning("[LauncherCommunicator] MapViewOfFile failed.");
            CloseHandle(_shmHandle);
            _shmHandle = IntPtr.Zero;
        }
#endif
    }

    private static T GetDelegate<T>(string procName) where T : Delegate
    {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR)
        if (_moduleHandle == IntPtr.Zero) return null;
        IntPtr addr = GetProcAddress(_moduleHandle, procName);
        return addr != IntPtr.Zero ? Marshal.GetDelegateForFunctionPointer<T>(addr) : null;
#else
        return null;
#endif
    }

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR)
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryW")]
    private static extern IntPtr LoadLibraryW(string lpFileName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "OpenFileMappingW")]
    private static extern IntPtr OpenFileMappingW(uint dwDesiredAccess,
                                                  [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
                                                  string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess,
                                               uint dwFileOffsetHigh, uint dwFileOffsetLow,
                                               UIntPtr dwNumberOfBytesToMap);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);
#endif
}