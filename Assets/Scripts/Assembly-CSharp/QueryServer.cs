internal class QueryServer
{
    private readonly int _port;

    private readonly bool _useV6;

    internal global::System.Collections.Generic.List<QueryUser> Users;

    private global::System.Threading.Thread _thr;

    private global::System.Threading.Thread _checkThr;

    internal global::System.Diagnostics.Stopwatch Stopwatch;

    private global::System.Net.Sockets.TcpListener _listner;

    private global::System.Net.Sockets.TcpListener _listnerv6;

    internal int TimeoutThreshold = 10;

    private bool _serverStop;

    internal QueryServer(int p, bool v6)
    {
        _port = p;
        _useV6 = v6;
    }

    internal void StartServer()
    {
        _serverStop = false;
        Stopwatch = new global::System.Diagnostics.Stopwatch();
        _thr = new global::System.Threading.Thread(StartUp)
        {
            IsBackground = true
        };
        _thr.Start();
        _checkThr = new global::System.Threading.Thread(CheckClients)
        {
            IsBackground = true,
            Priority = global::System.Threading.ThreadPriority.BelowNormal
        };
    }

    private void CheckClients()
    {
        while (!_serverStop)
        {
            for (int num = Users.Count - 1; num >= 0; num--)
            {
                if (!Users[num].IsConnected())
                {
                    ServerConsole.AddLog("Query user connected from " + Users[num].Ip + " timed out.");
                    try
                    {
                        Users[num].CloseConn();
                        Users.RemoveAt(num);
                    }
                    catch
                    {
                    }
                }
            }
            global::System.Threading.Thread.Sleep(10000);
        }
    }

    internal void StopServer()
    {
        ServerConsole.AddLog("Stopping query server...");
        _checkThr.Abort();
        _serverStop = true;
    }

    private void StartUp()
    {
        ServerConsole.AddLog("Starting query server on port " + _port + " TCP...");
        Users = new global::System.Collections.Generic.List<QueryUser>();
        Stopwatch.Start();
        _checkThr.Start();
        try
        {
            _listner = new global::System.Net.Sockets.TcpListener(global::System.Net.IPAddress.Any, _port);
            _listner.Start();
            if (_useV6)
            {
                _listnerv6 = new global::System.Net.Sockets.TcpListener(global::System.Net.IPAddress.IPv6Any, _port);
                _listnerv6.Start();
            }
            while (!_serverStop)
            {
                if (_listner.Pending())
                {
                    AcceptSocket(_listner);
                }
                else if (_useV6 && _listnerv6.Pending())
                {
                    AcceptSocket(_listnerv6);
                }
                else
                {
                    global::System.Threading.Thread.Sleep(500);
                }
            }
            _listner.Stop();
            if (_useV6)
            {
                _listnerv6.Stop();
            }
            foreach (QueryUser user in Users)
            {
                user.CloseConn(shuttingdown: true);
            }
            Users.Clear();
            ServerConsole.AddLog("Query server stopped.");
        }
        catch (global::System.Exception ex)
        {
            ServerConsole.AddLog("Server ERROR: " + ex.StackTrace);
        }
    }

    private void AcceptSocket(global::System.Net.Sockets.TcpListener lst)
    {
        global::System.Net.Sockets.TcpClient tcpClient = lst.AcceptTcpClient();
        QueryUser item = new QueryUser(this, tcpClient, tcpClient.Client.RemoteEndPoint.ToString());
        Users.Add(item);
        ServerConsole.AddLog(string.Concat("New query connection from ", tcpClient.Client.RemoteEndPoint, " on ", tcpClient.Client.LocalEndPoint, "."));
    }
}
