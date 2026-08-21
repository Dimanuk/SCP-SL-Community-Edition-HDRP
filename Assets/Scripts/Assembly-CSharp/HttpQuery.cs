public static class HttpQuery
{
    private const int SleepTime = 12;

    private static readonly global::System.Net.Http.HttpClient _client;

    private static readonly bool LockHttpMode;

    private static readonly HttpQueryMode HttpMode;

    static HttpQuery()
    {
        _client = new global::System.Net.Http.HttpClient
        {
            Timeout = global::System.TimeSpan.FromSeconds(15.0),
            DefaultRequestHeaders =
            {
                { "User-Agent", "SCP SL" },
                {
                    "Game-Version",
                    global::GameCore.Version.VersionString
                }
            }
        };
        if (global::System.Linq.Enumerable.Any(StartupArgs.Args, (string arg) => string.Equals(arg, "-lockhttpmode", global::System.StringComparison.OrdinalIgnoreCase)) || global::System.IO.File.Exists(FileManager.GetAppFolder() + "LockHttpMode.txt"))
        {
            LockHttpMode = true;
            global::GameCore.Console.AddLog("HTTP mode locked", global::UnityEngine.Color.gray);
        }
        if (global::System.Linq.Enumerable.Any(StartupArgs.Args, (string arg) => string.Equals(arg, "-httpproxy", global::System.StringComparison.OrdinalIgnoreCase)) || global::System.IO.File.Exists(FileManager.GetAppFolder() + "HttpProxy.txt"))
        {
            HttpMode = HttpQueryMode.HttpProxy;
            global::GameCore.Console.AddLog("HTTP mode switched to HttpProxy (startup argument)", global::UnityEngine.Color.gray);
        }
        if (global::System.Linq.Enumerable.Any(StartupArgs.Args, (string arg) => string.Equals(arg, "-unitywebrequest", global::System.StringComparison.OrdinalIgnoreCase)) || global::System.IO.File.Exists(FileManager.GetAppFolder() + "UnityWebRequest.txt"))
        {
            HttpMode = HttpQueryMode.UnityWebRequest;
            global::GameCore.Console.AddLog("HTTP mode switched to UnityWebRequest (startup argument)", global::UnityEngine.Color.gray);
        }
        if (global::System.Linq.Enumerable.Any(StartupArgs.Args, (string arg) => string.Equals(arg, "-unitywebrequestdispatcher", global::System.StringComparison.OrdinalIgnoreCase)) || global::System.IO.File.Exists(FileManager.GetAppFolder() + "UnityWebRequestDispatcher.txt"))
        {
            HttpMode = HttpQueryMode.UnityWebRequestDispatcher;
            global::GameCore.Console.AddLog("HTTP mode switched to UnityWebRequestDispatcher (startup argument)", global::UnityEngine.Color.gray);
        }
    }

    public static string Get(string url)
    {
        bool success;
        global::System.Net.HttpStatusCode code;
        string text = Get(url, out success, out code);
        if (!success)
        {
            throw new global::System.Exception(string.Concat("Error ", code, ".\n", text));
        }
        return text;
    }

    public static string Get(string url, out bool success)
    {
        global::System.Net.HttpStatusCode code;
        return Get(url, out success, out code);
    }

    public static string Get(string url, out bool success, out global::System.Net.HttpStatusCode code)
    {
        global::System.Collections.Generic.HashSet<HttpQueryMode> hashSet = global::NorthwoodLib.Pools.HashSetPool<HttpQueryMode>.Shared.Rent();
        HttpQueryMode httpQueryMode = HttpMode;
        try
        {
            while (true)
            {
                switch (httpQueryMode)
                {
                    case HttpQueryMode.HttpClient:
                        if (PlatformInfo.singleton == null || PlatformInfo.singleton.IsMainThread)
                        {
                            if (!hashSet.Add(HttpQueryMode.HttpClient) || LockHttpMode)
                            {
                                throw new global::System.NotSupportedException();
                            }
                            httpQueryMode = HttpQueryMode.HttpProxy;
                            break;
                        }
                        try
                        {
                            global::System.Net.Http.HttpResponseMessage result = _client.GetAsync(url).GetAwaiter().GetResult();
                            code = result.StatusCode;
                            success = result.IsSuccessStatusCode;
                            return result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        }
                        catch (global::System.Exception ex2)
                        {
                            if (!hashSet.Add(HttpQueryMode.HttpClient) || LockHttpMode)
                            {
                                throw;
                            }
                            httpQueryMode = HttpQueryMode.HttpProxy;
                            global::GameCore.Console.AddLog("Switched to HttpProxy (exception \"" + ex2.Message + "\" occured) [GET Request].", global::UnityEngine.Color.yellow);
                        }
                        break;
                    case HttpQueryMode.HttpProxy:
                        if (!HttpWorkaround.Enabled)
                        {
                            if (!hashSet.Add(HttpQueryMode.HttpProxy) || LockHttpMode)
                            {
                                throw new global::System.NotSupportedException();
                            }
                            httpQueryMode = HttpQueryMode.UnityWebRequest;
                            break;
                        }
                        try
                        {
                            return HttpWorkaround.Get(url, out success, out code);
                        }
                        catch (global::System.Exception ex4)
                        {
                            if (!hashSet.Add(HttpQueryMode.HttpProxy) || LockHttpMode)
                            {
                                throw;
                            }
                            httpQueryMode = HttpQueryMode.UnityWebRequest;
                            global::GameCore.Console.AddLog("Switched to UnityWebRequest (exception \"" + ex4.Message + "\" occured) [GET Request].", global::UnityEngine.Color.yellow);
                        }
                        break;
                    case HttpQueryMode.UnityWebRequest:
                        if (PlatformInfo.singleton == null || !PlatformInfo.singleton.IsMainThread)
                        {
                            if (!hashSet.Add(HttpQueryMode.UnityWebRequest) || LockHttpMode)
                            {
                                throw new global::System.NotSupportedException();
                            }
                            httpQueryMode = HttpQueryMode.UnityWebRequestDispatcher;
                            break;
                        }
                        try
                        {
                            using (global::UnityEngine.Networking.UnityWebRequest unityWebRequest = global::UnityEngine.Networking.UnityWebRequest.Get(url))
                            {
                                global::UnityEngine.Networking.UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = unityWebRequest.SendWebRequest();
                                while (!unityWebRequestAsyncOperation.isDone)
                                {
                                    global::System.Threading.Thread.Sleep(12);
                                }
                                success = !unityWebRequest.isNetworkError && !unityWebRequest.isHttpError;
                                code = (global::System.Net.HttpStatusCode)unityWebRequest.responseCode;
                                return string.IsNullOrEmpty(unityWebRequest.error) ? unityWebRequest.downloadHandler.text : unityWebRequest.error;
                            }
                        }
                        catch (global::System.Exception ex3)
                        {
                            if (!hashSet.Add(HttpQueryMode.UnityWebRequest) || LockHttpMode)
                            {
                                throw;
                            }
                            httpQueryMode = HttpQueryMode.UnityWebRequestDispatcher;
                            global::GameCore.Console.AddLog("Switched to UnityWebRequestDispatcher (exception \"" + ex3.Message + "\" occured) [GET Request].", global::UnityEngine.Color.yellow);
                        }
                        break;
                    case HttpQueryMode.UnityWebRequestDispatcher:
                        if (PlatformInfo.singleton == null || PlatformInfo.singleton.IsMainThread)
                        {
                            if (!hashSet.Add(HttpQueryMode.UnityWebRequestDispatcher) || LockHttpMode)
                            {
                                throw new global::System.NotSupportedException();
                            }
                            httpQueryMode = HttpQueryMode.HttpClient;
                            break;
                        }
                        try
                        {
                            UnityWebRequestDispatcher.GetRequest getRequest = UnityWebRequestDispatcher.Get(url);
                            while (!getRequest.Done)
                            {
                                global::System.Threading.Thread.Sleep(12);
                            }
                            success = getRequest.Successful;
                            code = getRequest.Code;
                            return getRequest.Text;
                        }
                        catch (global::System.Exception ex)
                        {
                            if (!hashSet.Add(HttpQueryMode.UnityWebRequestDispatcher) || LockHttpMode)
                            {
                                throw;
                            }
                            httpQueryMode = HttpQueryMode.HttpClient;
                            global::GameCore.Console.AddLog("Switched to HttpClient (exception \"" + ex.Message + "\" occured) [GET Request].", global::UnityEngine.Color.yellow);
                        }
                        break;
                    default:
                        throw new global::System.ArgumentOutOfRangeException();
                }
            }
        }
        finally
        {
            global::NorthwoodLib.Pools.HashSetPool<HttpQueryMode>.Shared.Return(hashSet);
        }
    }

    public static string Post(string url, string data)
    {
        bool success;
        global::System.Net.HttpStatusCode code;
        string text = Post(url, data, out success, out code);
        if (!success)
        {
            throw new global::System.Exception(string.Concat("Error ", code, ".\n", text));
        }
        return text;
    }

    public static string Post(string url, string data, out bool success)
    {
        global::System.Net.HttpStatusCode code;
        return Post(url, data, out success, out code);
    }

    public static string Post(string url, string data, out bool success, out global::System.Net.HttpStatusCode code)
    {
        global::System.Collections.Generic.HashSet<HttpQueryMode> hashSet = global::NorthwoodLib.Pools.HashSetPool<HttpQueryMode>.Shared.Rent();
        HttpQueryMode httpQueryMode = HttpMode;
        try
        {
            while (true)
            {
                switch (httpQueryMode)
                {
                    case HttpQueryMode.HttpClient:
                        if (PlatformInfo.singleton == null || PlatformInfo.singleton.IsMainThread)
                        {
                            if (!hashSet.Add(HttpQueryMode.HttpClient) || LockHttpMode)
                            {
                                throw new global::System.NotSupportedException();
                            }
                            httpQueryMode = HttpQueryMode.HttpProxy;
                            break;
                        }
                        try
                        {
                            global::System.Net.Http.HttpResponseMessage result = _client.PostAsync(url, new global::System.Net.Http.StringContent(data, global::System.Text.Encoding.UTF8, "application/x-www-form-urlencoded")).GetAwaiter().GetResult();
                            code = result.StatusCode;
                            success = result.IsSuccessStatusCode;
                            return result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        }
                        catch (global::System.Exception ex2)
                        {
                            if (!hashSet.Add(HttpQueryMode.HttpClient) || LockHttpMode)
                            {
                                throw;
                            }
                            httpQueryMode = HttpQueryMode.HttpProxy;
                            global::GameCore.Console.AddLog("Switched to HttpProxy (exception \"" + ex2.Message + "\" occured) [POST Request].", global::UnityEngine.Color.yellow);
                        }
                        break;
                    case HttpQueryMode.HttpProxy:
                        if (!HttpWorkaround.Enabled)
                        {
                            if (!hashSet.Add(HttpQueryMode.HttpProxy) || LockHttpMode)
                            {
                                throw new global::System.NotSupportedException();
                            }
                            httpQueryMode = HttpQueryMode.UnityWebRequest;
                            break;
                        }
                        try
                        {
                            return HttpWorkaround.Post(url, data, out success, out code);
                        }
                        catch (global::System.Exception ex4)
                        {
                            if (!hashSet.Add(HttpQueryMode.HttpProxy) || LockHttpMode)
                            {
                                throw;
                            }
                            httpQueryMode = HttpQueryMode.UnityWebRequest;
                            global::GameCore.Console.AddLog("Switched to UnityWebRequest (exception \"" + ex4.Message + "\" occured) [POST Request].", global::UnityEngine.Color.yellow);
                        }
                        break;
                    case HttpQueryMode.UnityWebRequest:
                        if (PlatformInfo.singleton == null || !PlatformInfo.singleton.IsMainThread)
                        {
                            if (!hashSet.Add(HttpQueryMode.UnityWebRequest) || LockHttpMode)
                            {
                                throw new global::System.NotSupportedException();
                            }
                            httpQueryMode = HttpQueryMode.UnityWebRequestDispatcher;
                            break;
                        }
                        try
                        {
                            using (global::UnityEngine.Networking.UnityWebRequest unityWebRequest = global::UnityEngine.Networking.UnityWebRequest.Post(url, ToUnityForm(data)))
                            {
                                global::UnityEngine.Networking.UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = unityWebRequest.SendWebRequest();
                                while (!unityWebRequestAsyncOperation.isDone)
                                {
                                    global::System.Threading.Thread.Sleep(12);
                                }
                                success = !unityWebRequest.isNetworkError && !unityWebRequest.isHttpError;
                                code = (global::System.Net.HttpStatusCode)unityWebRequest.responseCode;
                                return string.IsNullOrEmpty(unityWebRequest.error) ? unityWebRequest.downloadHandler.text : unityWebRequest.error;
                            }
                        }
                        catch (global::System.Exception ex3)
                        {
                            if (!hashSet.Add(HttpQueryMode.UnityWebRequest) || LockHttpMode)
                            {
                                throw;
                            }
                            httpQueryMode = HttpQueryMode.UnityWebRequestDispatcher;
                            global::GameCore.Console.AddLog("Switched to UnityWebRequestDispatcher (exception \"" + ex3.Message + "\" occured) [POST Request].", global::UnityEngine.Color.yellow);
                        }
                        break;
                    case HttpQueryMode.UnityWebRequestDispatcher:
                        if (PlatformInfo.singleton == null || PlatformInfo.singleton.IsMainThread)
                        {
                            if (!hashSet.Add(HttpQueryMode.UnityWebRequestDispatcher) || LockHttpMode)
                            {
                                throw new global::System.NotSupportedException();
                            }
                            httpQueryMode = HttpQueryMode.HttpClient;
                            break;
                        }
                        try
                        {
                            UnityWebRequestDispatcher.PostRequest postRequest = UnityWebRequestDispatcher.Post(url, data);
                            while (!postRequest.Done)
                            {
                                global::System.Threading.Thread.Sleep(12);
                            }
                            success = postRequest.Successful;
                            code = postRequest.Code;
                            return postRequest.Text;
                        }
                        catch (global::System.Exception ex)
                        {
                            if (!hashSet.Add(HttpQueryMode.UnityWebRequestDispatcher) || LockHttpMode)
                            {
                                throw;
                            }
                            httpQueryMode = HttpQueryMode.HttpClient;
                            global::GameCore.Console.AddLog("Switched to HttpClient (exception \"" + ex.Message + "\" occured) [POST Request].", global::UnityEngine.Color.yellow);
                        }
                        break;
                    default:
                        throw new global::System.ArgumentOutOfRangeException();
                }
            }
        }
        finally
        {
            global::NorthwoodLib.Pools.HashSetPool<HttpQueryMode>.Shared.Return(hashSet);
        }
    }

    public static string ToPostArgs(global::System.Collections.Generic.IEnumerable<string> data)
    {
        return global::System.Linq.Enumerable.Aggregate(data, (string current, string a) => current + "&" + a.Replace("&", "[AMP]")).TrimStart('&');
    }

    public static global::UnityEngine.WWWForm ToUnityForm(string data)
    {
        global::UnityEngine.WWWForm wWWForm = new global::UnityEngine.WWWForm();
        string[] array = data.Split('&');
        foreach (string text in array)
        {
            if (global::NorthwoodLib.StringUtils.Contains(text, "=", global::System.StringComparison.Ordinal))
            {
                string[] array2 = text.Split('=');
                wWWForm.AddField(array2[0], array2[1]);
            }
            else
            {
                wWWForm.AddField(text, string.Empty);
            }
        }
        return wWWForm;
    }
}
