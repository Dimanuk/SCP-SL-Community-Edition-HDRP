public class CentralServer : global::UnityEngine.MonoBehaviour
{
    public static object RefreshLock;

    private static string _serversPath;

    private static bool _started;

    private static global::System.Collections.Generic.List<string> _workingServers;

    private static global::System.DateTime _lastReset;

    internal static bool Abort;

    public static string MasterUrl { get; internal set; }

    public static string StandardUrl { get; internal set; }

    public static string SelectedServer { get; internal set; }

    public static bool TestServer { get; internal set; }

    public static bool ServerSelected { get; set; }

    internal static string[] Servers { get; private set; }

    private void Start()
    {
        Init();
    }

    internal static void Init()
    {
        if (_started)
        {
            return;
        }
        _started = true;
        if (LocalCentralServer.IsActive)
        {
            StandardUrl = LocalCentralServer.Url;
            MasterUrl = LocalCentralServer.Url;
            SelectedServer = "LOCAL";
            TestServer = false;
            ServerSelected = true;
            _workingServers = new global::System.Collections.Generic.List<string>();
            Servers = new string[0];
            RefreshLock = new object();
            ServerConsole.AddLog("Using LOCAL central server: " + MasterUrl);
            return;
        }
        if (global::System.IO.File.Exists(FileManager.GetAppFolder() + "testserver.txt"))
        {
            StandardUrl = "https://test.scpslgame.com/";
            MasterUrl = "https://test.scpslgame.com/";
            SelectedServer = "TEST";
            TestServer = true;
            ServerSelected = true;
            ServerConsole.AddLog("Using TEST central server: " + MasterUrl);
            return;
        }
        MasterUrl = "https://api.scpslgame.com/";
        StandardUrl = "https://api.scpslgame.com/";
        TestServer = false;
        _lastReset = global::System.DateTime.MinValue;
        Servers = new string[0];
        _workingServers = new global::System.Collections.Generic.List<string>();
        RefreshLock = new object();
        _serversPath = FileManager.GetAppFolder() + "internal/";
        if (!global::System.IO.Directory.Exists(_serversPath))
        {
            global::System.IO.Directory.CreateDirectory(_serversPath);
        }
        _serversPath += "CentralServers";
        if (global::System.IO.File.Exists(_serversPath))
        {
            Servers = FileManager.ReadAllLines(_serversPath);
            if (global::System.Linq.Enumerable.Any(Servers, (string server) => !global::System.Text.RegularExpressions.Regex.IsMatch(server, "^[a-zA-Z0-9]*$")))
            {
                global::GameCore.Console.AddLog("Malformed server found on the list. Removing the list and redownloading it from api.scpslgame.com.", global::UnityEngine.Color.yellow);
                Servers = new string[0];
                try
                {
                    global::System.IO.File.Delete(_serversPath);
                }
                catch (global::System.Exception ex)
                {
                    global::GameCore.Console.AddLog("Failed to delete malformed central server list.\nException: " + ex.Message, global::UnityEngine.Color.red);
                }
                global::System.Threading.Thread thread = new global::System.Threading.Thread((global::System.Threading.ThreadStart)delegate
                {
                    RefreshServerList(planned: true, loop: true);
                });
                thread.IsBackground = true;
                thread.Priority = global::System.Threading.ThreadPriority.BelowNormal;
                thread.Name = "SCP:SL Server list refreshing";
                thread.Start();
                return;
            }
            _workingServers = global::System.Linq.Enumerable.ToList(Servers);
            if (!ServerStatic.IsDedicated)
            {
                global::GameCore.Console.AddLog("Cached central servers count: " + Servers.Length, global::UnityEngine.Color.grey);
            }
            if (Servers.Length != 0)
            {
                global::System.Random random = new global::System.Random();
                SelectedServer = Servers[random.Next(Servers.Length)];
                StandardUrl = "https://" + SelectedServer.ToLower() + ".scpslgame.com/";
                if (ServerStatic.IsDedicated)
                {
                    ServerConsole.AddLog("Selected central server: " + SelectedServer + " (" + StandardUrl + ")");
                }
                else
                {
                    global::GameCore.Console.AddLog("Selected central server: " + SelectedServer + " (" + StandardUrl + ")", global::UnityEngine.Color.grey);
                }
            }
        }
        global::System.Threading.Thread thread2 = new global::System.Threading.Thread((global::System.Threading.ThreadStart)delegate
        {
            RefreshServerList(planned: true, loop: true);
        });
        thread2.IsBackground = true;
        thread2.Priority = global::System.Threading.ThreadPriority.BelowNormal;
        thread2.Name = "SCP:SL Server list refreshing";
        thread2.Start();
    }

    private static void RefreshServerList(bool planned = false, bool loop = false)
    {
        while (!Abort)
        {
            lock (RefreshLock)
            {
                if (ServerSelected)
                {
                    break;
                }
                if (_workingServers.Count == 0)
                {
                    if (Servers.Length == 0)
                    {
                        StandardUrl = "https://api.scpslgame.com/";
                        SelectedServer = "Primary API";
                    }
                    else
                    {
                        _workingServers = global::System.Linq.Enumerable.ToList(Servers);
                        StandardUrl = "https://" + _workingServers[0] + ".scpslgame.com/";
                        SelectedServer = _workingServers[0];
                    }
                }
                byte b = 1;
                while (!Abort && b != 3)
                {
                    b++;
                    try
                    {
                        string[] array = HttpQuery.Get(StandardUrl + "servers.php").Split(';');
                        if (global::System.IO.File.Exists(_serversPath))
                        {
                            global::System.IO.File.Delete(_serversPath);
                        }
                        FileManager.WriteToFile(array, _serversPath);
                        global::GameCore.Console.AddLog("Updated list of central servers.", global::UnityEngine.Color.green);
                        global::GameCore.Console.AddLog("Central servers count: " + array.Length, global::UnityEngine.Color.cyan);
                        Servers = array;
                        if (planned && global::System.Linq.Enumerable.All(Servers, (string srv) => srv != SelectedServer))
                        {
                            _workingServers = global::System.Linq.Enumerable.ToList(Servers);
                            ChangeCentralServer(remove: false);
                        }
                        ServerSelected = true;
                    }
                    catch (global::System.Exception ex)
                    {
                        global::GameCore.Console.AddLog("Can't update central servers list!", global::UnityEngine.Color.red);
                        global::GameCore.Console.AddLog("Error: " + ex.Message, global::UnityEngine.Color.red);
                        if (SelectedServer == "Primary API")
                        {
                            ServerSelected = true;
                            break;
                        }
                        ChangeCentralServer(remove: true);
                        continue;
                    }
                    break;
                }
            }
            if (!loop)
            {
                break;
            }
            for (uint num = 0u; num < 180; num++)
            {
                if (Abort)
                {
                    break;
                }
                global::System.Threading.Thread.Sleep(5000);
            }
        }
    }

    internal static bool ChangeCentralServer(bool remove)
    {
        ServerSelected = false;
        TestServer = false;
        if (SelectedServer == "Primary API")
        {
            if (_lastReset >= global::System.DateTime.Now.AddMinutes(-2.0))
            {
                return false;
            }
            RefreshServerList();
            return true;
        }
        if (_workingServers.Count == 0)
        {
            global::GameCore.Console.AddLog("All known central servers aren't working.", global::UnityEngine.Color.yellow);
            _workingServers.Add("API");
            SelectedServer = "Primary API";
            StandardUrl = "https://api.scpslgame.com/";
            global::GameCore.Console.AddLog("Changed central server: " + SelectedServer + " (" + StandardUrl + ")", global::UnityEngine.Color.yellow);
            return true;
        }
        if (remove && _workingServers.Contains(SelectedServer))
        {
            _workingServers.Remove(SelectedServer);
        }
        if (_workingServers.Count == 0)
        {
            _workingServers.Add("API");
            SelectedServer = "Primary API";
            StandardUrl = "https://api.scpslgame.com/";
            global::GameCore.Console.AddLog("Changed central server: " + SelectedServer + " (" + StandardUrl + ")", global::UnityEngine.Color.yellow);
            return true;
        }
        global::System.Random random = new global::System.Random();
        SelectedServer = _workingServers[random.Next(0, _workingServers.Count)];
        StandardUrl = "https://" + SelectedServer.ToLower() + ".scpslgame.com/";
        global::GameCore.Console.AddLog("Changed central server: " + SelectedServer + " (" + StandardUrl + ")", global::UnityEngine.Color.yellow);
        return true;
    }
}
