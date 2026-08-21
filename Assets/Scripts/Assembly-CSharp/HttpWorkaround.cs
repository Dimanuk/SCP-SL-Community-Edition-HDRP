internal static class HttpWorkaround
{
    private class HttpProxyException : global::System.Exception
    {
        public HttpProxyException(string message)
            : base(message)
        {
        }
    }

    public static readonly bool Enabled;

    private const string HttpProxy = "HttpProxy";

    private const global::System.Runtime.InteropServices.CallingConvention Convention = global::System.Runtime.InteropServices.CallingConvention.StdCall;

    private const global::System.Runtime.InteropServices.CharSet Encoding = global::System.Runtime.InteropServices.CharSet.Unicode;

    [global::System.Runtime.InteropServices.DllImport("HttpProxy", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.StdCall, CharSet = global::System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern bool Initialize(string ptr, out global::System.IntPtr message);

    [global::System.Runtime.InteropServices.DllImport("HttpProxy", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.StdCall, CharSet = global::System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern global::System.IntPtr Get(string url, out bool success, out int code, out global::System.IntPtr exception);

    [global::System.Runtime.InteropServices.DllImport("HttpProxy", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.StdCall, CharSet = global::System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern global::System.IntPtr Post(string url, string data, out bool success, out int code, out global::System.IntPtr exception);

    [global::System.Runtime.InteropServices.DllImport("HttpProxy", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.StdCall)]
    private static extern void Free(global::System.IntPtr ptr);

    static HttpWorkaround()
    {
        if (!global::System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(global::System.Runtime.InteropServices.OSPlatform.Windows))
        {
            Enabled = false;
            return;
        }
        try
        {
            Enabled = Initialize(global::GameCore.Version.VersionString, out var message);
            global::UnityEngine.Debug.Log(global::System.Runtime.InteropServices.Marshal.PtrToStringUni(message));
            Free(message);
        }
        catch (global::System.Exception exception)
        {
            Enabled = false;
            global::UnityEngine.Debug.LogException(exception);
        }
    }

    internal static string Get(string url, out bool success, out global::System.Net.HttpStatusCode code)
    {
        int code2;
        global::System.IntPtr exception;
        global::System.IntPtr ptr = Get(url, out success, out code2, out exception);
        if (exception != global::System.IntPtr.Zero)
        {
            string message = global::System.Runtime.InteropServices.Marshal.PtrToStringUni(exception);
            Free(exception);
            throw new HttpWorkaround.HttpProxyException(message);
        }
        code = (global::System.Net.HttpStatusCode)code2;
        string result = global::System.Runtime.InteropServices.Marshal.PtrToStringUni(ptr);
        Free(ptr);
        return result;
    }

    internal static string Post(string url, string data, out bool success, out global::System.Net.HttpStatusCode code)
    {
        int code2;
        global::System.IntPtr exception;
        global::System.IntPtr ptr = Post(url, data, out success, out code2, out exception);
        if (exception != global::System.IntPtr.Zero)
        {
            string message = global::System.Runtime.InteropServices.Marshal.PtrToStringUni(exception);
            Free(exception);
            throw new HttpWorkaround.HttpProxyException(message);
        }
        code = (global::System.Net.HttpStatusCode)code2;
        string result = global::System.Runtime.InteropServices.Marshal.PtrToStringUni(ptr);
        Free(ptr);
        return result;
    }
}
