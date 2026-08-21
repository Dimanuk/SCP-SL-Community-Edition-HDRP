public class ServerLogs : global::UnityEngine.MonoBehaviour
{
    public enum ServerLogType : byte
    {
        ConnectionUpdate = 0,
        RemoteAdminActivity_GameChanging = 1,
        RemoteAdminActivity_Misc = 2,
        KillLog = 3,
        GameEvent = 4,
        InternalMessage = 5,
        RateLimit = 6,
        Teamkill = 7,
        Suicide = 8
    }

    public enum Modules : byte
    {
        Warhead = 0,
        Networking = 1,
        ClassChange = 2,
        Permissions = 3,
        Administrative = 4,
        Logger = 5,
        DataAccess = 6,
        Detector = 7
    }

    private enum LoggingState : byte
    {
        Off = 0,
        Standby = 1,
        Write = 2,
        Terminate = 3,
        Restart = 4
    }

    public readonly struct ServerLog : global::System.IEquatable<ServerLogs.ServerLog>
    {
        public readonly string Content;

        public readonly string Type;

        public readonly string Module;

        public readonly string Time;

        public ServerLog(string content, string type, string module, string time)
        {
            Content = content;
            Type = type;
            Module = module;
            Time = time;
        }

        public bool Equals(ServerLogs.ServerLog other)
        {
            if (Content == other.Content && Type == other.Type && Module == other.Module)
            {
                return Time == other.Time;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is ServerLogs.ServerLog other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (((((((Content != null) ? Content.GetHashCode() : 0) * 397) ^ ((Type != null) ? Type.GetHashCode() : 0)) * 397) ^ ((Module != null) ? Module.GetHashCode() : 0)) * 397) ^ ((Time != null) ? Time.GetHashCode() : 0);
        }

        public static bool operator ==(ServerLogs.ServerLog left, ServerLogs.ServerLog right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ServerLogs.ServerLog left, ServerLogs.ServerLog right)
        {
            return !left.Equals(right);
        }
    }

    private static readonly string[] Txt;

    private static readonly string[] Modulestxt;

    private static readonly global::System.Collections.Generic.Queue<ServerLogs.ServerLog> Queue;

    private static readonly object LockObject;

    private static global::System.Threading.Thread _appendThread;

    private static int _maxlen;

    private static int _modulemaxlen;

    private static volatile ServerLogs.LoggingState _state;

    static ServerLogs()
    {
        Txt = new string[9] { "Connection update", "Remote Admin", "Remote Admin - Misc", "Kill", "Game Event", "Internal", "Rate Limit", "Teamkill", "Suicide" };
        Modulestxt = new string[8] { "Warhead", "Networking", "Class change", "Permissions", "Administrative", "Logger", "Data access", "FF Detector" };
        Queue = new global::System.Collections.Generic.Queue<ServerLogs.ServerLog>();
        LockObject = new object();
        string[] txt = Txt;
        foreach (string text in txt)
        {
            _maxlen = global::System.Math.Max(_maxlen, text.Length);
        }
        txt = Modulestxt;
        foreach (string text2 in txt)
        {
            _modulemaxlen = global::System.Math.Max(_modulemaxlen, text2.Length);
        }
    }

    internal static void StartLogging()
    {
        if (global::Mirror.NetworkServer.active)
        {
            if (_state != ServerLogs.LoggingState.Off)
            {
                _state = ServerLogs.LoggingState.Restart;
            }
            else if (_appendThread == null || !_appendThread.IsAlive)
            {
                _appendThread = new global::System.Threading.Thread(AppendLog)
                {
                    Name = "Saving server logs to file",
                    Priority = global::System.Threading.ThreadPriority.BelowNormal,
                    IsBackground = true
                };
                _appendThread.Start();
            }
        }
    }

    public static void AddLog(ServerLogs.Modules module, string msg, ServerLogs.ServerLogType type, bool init = false)
    {
        string time = TimeBehaviour.Rfc3339Time();
        lock (LockObject)
        {
            Queue.Enqueue(new ServerLogs.ServerLog(msg, Txt[(uint)type], Modulestxt[(uint)module], time));
        }
        if (!init)
        {
            _state = ServerLogs.LoggingState.Write;
        }
    }

    private void OnApplicationQuit()
    {
        _state = ServerLogs.LoggingState.Terminate;
    }

    private static void AppendLog()
    {
        _state = ServerLogs.LoggingState.Standby;
        global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
        while (_state != ServerLogs.LoggingState.Terminate)
        {
            lock (LockObject)
            {
                Queue.Clear();
                _state = ServerLogs.LoggingState.Standby;
            }
            while (!global::Mirror.NetworkServer.active)
            {
                if (_state == ServerLogs.LoggingState.Terminate)
                {
                    _state = ServerLogs.LoggingState.Off;
                    global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
                    return;
                }
                global::System.Threading.Thread.Sleep(200);
            }
            string text = TimeBehaviour.FormatTime("yyyy-MM-dd HH.mm.ss");
            string text2 = global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port.ToString();
            AddLog(ServerLogs.Modules.Logger, "Started logging. Game version: " + global::GameCore.Version.VersionString + ", private beta: " + (global::GameCore.Version.PrivateBeta ? "YES" : "NO") + ".", ServerLogs.ServerLogType.InternalMessage, init: true);
            while (global::Mirror.NetworkServer.active && _state != ServerLogs.LoggingState.Terminate && _state != ServerLogs.LoggingState.Restart)
            {
                global::System.Threading.Thread.Sleep(100);
                if (_state == ServerLogs.LoggingState.Standby)
                {
                    continue;
                }
                if (!global::System.IO.Directory.Exists(FileManager.GetAppFolder()))
                {
                    return;
                }
                if (!global::System.IO.Directory.Exists(FileManager.GetAppFolder() + "ServerLogs"))
                {
                    global::System.IO.Directory.CreateDirectory(FileManager.GetAppFolder() + "ServerLogs");
                }
                if (!global::System.IO.Directory.Exists(FileManager.GetAppFolder() + "ServerLogs/" + text2))
                {
                    global::System.IO.Directory.CreateDirectory(FileManager.GetAppFolder() + "ServerLogs/" + text2);
                }
                stringBuilder.Clear();
                lock (LockObject)
                {
                    ServerLogs.ServerLog element;
                    while (CollectionExtensions.TryDequeue(Queue, out element))
                    {
                        stringBuilder.Append(element.Time + " | " + ToMax(element.Type, _maxlen) + " | " + ToMax(element.Module, _modulemaxlen) + " | " + element.Content + global::System.Environment.NewLine);
                    }
                }
                using (global::System.IO.StreamWriter streamWriter = new global::System.IO.StreamWriter(FileManager.GetAppFolder() + "ServerLogs/" + text2 + "/Round " + text + ".txt", append: true))
                {
                    streamWriter.Write(stringBuilder.ToString());
                }
                if (_state == ServerLogs.LoggingState.Terminate || _state == ServerLogs.LoggingState.Restart)
                {
                    break;
                }
                _state = ServerLogs.LoggingState.Standby;
            }
        }
        _state = ServerLogs.LoggingState.Off;
        global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
    }

    private static string ToMax(string text, int max)
    {
        while (text.Length < max)
        {
            text += " ";
        }
        return text;
    }
}
