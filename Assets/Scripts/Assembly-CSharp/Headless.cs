public static class Headless
{
    public static readonly string version = "1.6.4";

    private static bool isHeadless = false;

    private static bool checkedHeadless = false;

    private static bool initializedHeadless = false;

    private static bool buildingHeadless = false;

    private static bool debuggingHeadless = false;

    private static HeadlessRuntime headlessRuntime;

    private static string currentProfile = "";

    public static string GetProfileName()
    {
        if (!IsHeadless())
        {
            return null;
        }
        InitializeHeadless();
        return currentProfile;
    }

    public static bool IsHeadless()
    {
        if (checkedHeadless)
        {
            return isHeadless;
        }
        if (global::System.IO.File.Exists(global::UnityEngine.Application.dataPath + "/~HeadlessDebug.txt"))
        {
            debuggingHeadless = true;
            isHeadless = true;
        }
        else if (global::System.Array.IndexOf(global::System.Environment.GetCommandLineArgs(), "-batchmode") >= 0)
        {
            isHeadless = true;
        }
        else if (global::UnityEngine.SystemInfo.graphicsDeviceType == global::UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            isHeadless = true;
        }
        checkedHeadless = true;
        return isHeadless;
    }

    [global::UnityEngine.RuntimeInitializeOnLoadMethod(global::UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoadRuntimeMethod()
    {
        if (IsHeadless())
        {
            InitializeHeadless();
            HeadlessCallbacks.InvokeCallbacks("HeadlessBeforeSceneLoad");
        }
    }

    [global::UnityEngine.RuntimeInitializeOnLoadMethod(global::UnityEngine.RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAfterSceneLoadRuntimeMethod()
    {
        if (!IsHeadless())
        {
            return;
        }
        if (headlessRuntime.valueCamera)
        {
            global::UnityEngine.GameObject gameObject = global::UnityEngine.GameObject.Find("HeadlessBehaviour");
            if (gameObject == null)
            {
                gameObject = (global::UnityEngine.GameObject)global::UnityEngine.Object.Instantiate(global::UnityEngine.Resources.Load("HeadlessBehaviour"));
            }
            HeadlessBehaviour headlessBehaviour = gameObject.GetComponent<HeadlessBehaviour>();
            if (headlessBehaviour == null)
            {
                headlessBehaviour = gameObject.AddComponent<HeadlessBehaviour>();
            }
            global::UnityEngine.Camera.onPreCull = (global::UnityEngine.Camera.CameraCallback)global::System.Delegate.Combine(global::UnityEngine.Camera.onPreCull, new global::UnityEngine.Camera.CameraCallback(headlessBehaviour.GetComponent<HeadlessBehaviour>().NullifyCamera));
        }
        HeadlessCallbacks.InvokeCallbacks("HeadlessAfterSceneLoad");
    }

    private static void InitializeHeadless()
    {
        if (initializedHeadless)
        {
            return;
        }
        headlessRuntime = global::UnityEngine.Resources.Load("HeadlessRuntime") as HeadlessRuntime;
        if (headlessRuntime != null)
        {
            currentProfile = headlessRuntime.profileName;
            if (headlessRuntime.valueConsole && !global::UnityEngine.Application.isEditor)
            {
                global::Windows.HeadlessConsole headlessConsole = new global::Windows.HeadlessConsole();
                headlessConsole.Initialize();
                headlessConsole.SetTitle(global::UnityEngine.Application.productName);
                global::UnityEngine.Application.logMessageReceived += HandleLog;
            }
            if (headlessRuntime.valueLimitFramerate)
            {
                global::UnityEngine.Application.targetFrameRate = headlessRuntime.valueFramerate;
                global::UnityEngine.QualitySettings.vSyncCount = 0;
                global::UnityEngine.Debug.Log("Application target framerate set to " + headlessRuntime.valueFramerate);
            }
        }
        initializedHeadless = true;
        HeadlessCallbacks.InvokeCallbacks("HeadlessBeforeFirstSceneLoad");
    }

    private static void HandleLog(string logString, string stackTrace, global::UnityEngine.LogType type)
    {
        global::System.Console.WriteLine(logString);
        if (stackTrace.Length > 1)
        {
            global::System.Console.WriteLine("in: " + stackTrace);
        }
    }

    public static bool IsBuildingHeadless()
    {
        if (buildingHeadless)
        {
            return true;
        }
        return false;
    }

    public static bool IsDebuggingHeadless()
    {
        if (debuggingHeadless)
        {
            return true;
        }
        return false;
    }

    public static void SetBuildingHeadless(bool value, string profileName)
    {
        buildingHeadless = value;
        currentProfile = profileName;
    }
}
