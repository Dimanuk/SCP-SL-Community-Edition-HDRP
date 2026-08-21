public class PlatformInfo : global::UnityEngine.MonoBehaviour
{
    public static PlatformInfo singleton;

    public bool IsHeadless { get; } = true;

    public bool IsIl2Cpp { get; }

    public bool IsEditor { get; }

    public bool UsesCustomLauncher { get; }

    public string BuildGuid { get; private set; }

    public bool IsWindows { get; } = global::System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(global::System.Runtime.InteropServices.OSPlatform.Windows);

    public bool IsLinux { get; } = global::System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(global::System.Runtime.InteropServices.OSPlatform.Linux);

    public int MainThreadId { get; private set; }

    public bool IsMainThread => global::System.Threading.Thread.CurrentThread.ManagedThreadId == MainThreadId;

    private void Awake()
    {
        MainThreadId = global::System.Threading.Thread.CurrentThread.ManagedThreadId;
        BuildGuid = global::UnityEngine.Application.buildGUID;
        singleton = this;
        global::GameCore.Console.AddLog("Loaded NorthwoodLib " + global::NorthwoodLib.PlatformSettings.Version, global::UnityEngine.Color.green);
        global::NorthwoodLib.PlatformSettings.Logged += OnLogged;
    }

    private static void OnLogged(string text, global::NorthwoodLib.Logging.LogType type)
    {
        switch (type)
        {
            case global::NorthwoodLib.Logging.LogType.Debug:
                global::UnityEngine.Debug.Log(text);
                break;
            case global::NorthwoodLib.Logging.LogType.Info:
                global::GameCore.Console.AddLog(text, global::UnityEngine.Color.blue);
                break;
            case global::NorthwoodLib.Logging.LogType.Warning:
                global::GameCore.Console.AddLog(text, global::UnityEngine.Color.yellow);
                break;
            case global::NorthwoodLib.Logging.LogType.Error:
                global::GameCore.Console.AddLog(text, global::UnityEngine.Color.red);
                break;
            default:
                global::UnityEngine.Debug.Log(text);
                break;
        }
    }
}
