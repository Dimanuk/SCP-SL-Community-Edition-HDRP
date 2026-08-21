using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace LocalCentralServer
{
    /// <summary>
    /// Minimal stand-in for api.scpslgame.com for LOCAL / LAN / Radmin-VPN testing of SCP:SL 12.0.2.
    /// Trust-based: it does NOT verify Steam tickets — whatever UserId/Nickname the client asks for is
    /// what it gets. Only ever run this on a private network.
    /// </summary>
    internal static class Program
    {
        private sealed class Session
        {
            public string UserId;
            public string NicknameBase64;
            public string Country = "XX";
            public byte Flags;
            public long Expiration;
            public string Nonce;
            public string PublicKeyHash; // hex sha256 of the client's session public key (as sent by the client)
        }

        // Base64(UTF8(hex)) — matches NorthwoodLib.StringUtils.Base64Encode of the hex hash string.
        private static string PublicKeyField(string hexHash)
            => string.IsNullOrEmpty(hexHash) ? string.Empty : Convert.ToBase64String(Encoding.UTF8.GetBytes(hexHash));

        // apiToken -> session captured at authenticate.php, consumed at requestsignature.php
        private static readonly ConcurrentDictionary<string, Session> Sessions = new ConcurrentDictionary<string, Session>();

        private sealed class GameServer
        {
            public uint Id;
            public string Ip;
            public ushort Port;
            public string Players = "0/20";
            public string InfoB64 = string.Empty; // standard base64 of the display name
            public string Pastebin = "7wV681fT";
            public string Version = string.Empty;
            public bool FriendlyFire;
            public bool Modded;
            public bool Whitelist;
            public DateTime LastSeen;
        }

        // "ip:port" -> game server registered via /v5/authenticator.php, served by /lobbylist.php
        private static readonly ConcurrentDictionary<string, GameServer> GameServers = new ConcurrentDictionary<string, GameServer>();
        private static int _nextServerId;
        private static readonly TimeSpan ServerTtl = TimeSpan.FromSeconds(120);

        private static KeyStore _keys;
        private static string _credits = "Local Central Server (test)";

        private static int Main(string[] args)
        {
            Console.Title = "SCP:SL Local Central Server";

            string prefix = GetArg(args, "--prefix") ?? Environment.GetEnvironmentVariable("LCS_PREFIX") ?? "http://+:5000/";
            string keyDir = GetArg(args, "--keys") ?? ResolveKeyDir();

            Console.WriteLine("SCP:SL Local Central Server");
            Console.WriteLine("Loading keys from: " + keyDir);
            _keys = KeyStore.LoadOrCreate(keyDir);

            if (Array.Exists(args, a => string.Equals(a, "--selftest", StringComparison.OrdinalIgnoreCase)))
                return SelfTest();

            Console.WriteLine("Central public key SHA loaded. Master public key ready (see keys folder).");

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(prefix);

            try
            {
                listener.Start();
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine();
                Console.WriteLine("Failed to bind " + prefix + " : " + ex.Message);
                Console.WriteLine("On Windows, binding to + or a hostname needs admin rights or a URL ACL.");
                Console.WriteLine("Either run this as Administrator, or grant the ACL once (admin cmd):");
                Console.WriteLine("    netsh http add urlacl url=" + prefix + " user=Everyone");
                Console.WriteLine("Alternatively bind to localhost only:  --prefix http://localhost:5000/");
                return 1;
            }

            Console.WriteLine("Listening on: " + prefix);
            PrintLanHints(prefix);
            Console.WriteLine("Endpoints: /v5/publickey.php  /servers.php  /ip.php  /lobbylist.php  /v5/authenticator.php  /v5/steam/authenticate.php  /v5/requestsignature.php");
            Console.WriteLine("Press Ctrl+C to stop.");
            Console.WriteLine();

            while (true)
            {
                HttpListenerContext ctx;
                try { ctx = listener.GetContext(); }
                catch (Exception) { break; }
                ThreadPool.QueueUserWorkItem(_ => HandleSafe(ctx));
            }

            return 0;
        }

        private static void HandleSafe(HttpListenerContext ctx)
        {
            try { Handle(ctx); }
            catch (Exception ex)
            {
                Console.WriteLine("[ERR] " + ex.Message);
                try { WriteText(ctx, 500, "error"); } catch { }
            }
        }

        private static void Handle(HttpListenerContext ctx)
        {
            string path = ctx.Request.Url.AbsolutePath.ToLowerInvariant();
            string method = ctx.Request.HttpMethod;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {method} {ctx.Request.Url.PathAndQuery} <- {ctx.Request.RemoteEndPoint}");

            if (path == "/v5/publickey.php")
            {
                HandlePublicKey(ctx);
                return;
            }

            if (path == "/servers.php")
            {
                // CentralServer.RefreshServerList splits this by ';'. "API" keeps it on the primary URL.
                WriteText(ctx, 200, "API");
                return;
            }

            if (path == "/ip.php")
            {
                // CustomNetworkManager._CreateLobby asks the central server for its external IP.
                WriteText(ctx, 200, ctx.Request.RemoteEndPoint.Address.ToString());
                return;
            }

            if (path == "/v5/authenticator.php")
            {
                HandleAuthenticator(ctx);
                return;
            }

            if (path == "/lobbylist.php" || path == "/v5/lobbylist.php")
            {
                HandleLobbyList(ctx);
                return;
            }

            if (path == "/v5/contactaddress.php")
            {
                WriteText(ctx, 200, "OK");
                return;
            }

            if (path == "/v5/steam/authenticate.php" || path == "/v5/discord/authenticate.php")
            {
                HandleAuthenticate(ctx);
                return;
            }

            if (path == "/v5/renew.php")
            {
                HandleAuthenticate(ctx); // same shape; lets the client refresh its session
                return;
            }

            if (path == "/v5/requestsignature.php")
            {
                HandleRequestSignature(ctx);
                return;
            }

            if (path.StartsWith("/centralcommands/"))
            {
                // Server registration / central commands — just acknowledge so the game doesn't log errors.
                WriteText(ctx, 200, "OK");
                return;
            }

            WriteText(ctx, 404, "not found");
        }

        private static void HandlePublicKey(HttpListenerContext ctx)
        {
            string key = _keys.CentralPublicPem;
            // The game verifies: ECDSA.Verify(key, signature, MasterKey). Sign the exact string we send.
            string signature = Crypto.Sign(key, _keys.Master.Private);

            string json = "{" +
                "\"key\":" + JsonString(key) + "," +
                "\"signature\":" + JsonString(signature) + "," +
                "\"credits\":" + JsonString(_credits) +
                "}";
            WriteJson(ctx, 200, json);
        }

        /// <summary>
        /// Game servers post here every ~5 s (ServerConsole.RefreshServerData -> AuthenticatorQuery.SendData).
        /// We record them so lobbylist.php can serve a real server list. Response mirrors AuthenticatorResponse;
        /// verified=true keeps the "won't be visible on the public list" warning away.
        /// </summary>
        private static void HandleAuthenticator(HttpListenerContext ctx)
        {
            Dictionary<string, string> form = ReadForm(ctx);

            string ip = form.GetValueOrDefault("ip");
            if (string.IsNullOrWhiteSpace(ip) || ip == "127.0.0.1")
                ip = ctx.Request.RemoteEndPoint.Address.ToString();

            ushort.TryParse(form.GetValueOrDefault("port"), out ushort port);
            if (port == 0) port = 7777;

            string key = ip + ":" + port;
            GameServer server = GameServers.GetOrAdd(key, _ => new GameServer
            {
                Id = (uint)Interlocked.Increment(ref _nextServerId),
                Ip = ip,
                Port = port
            });

            server.LastSeen = DateTime.UtcNow;
            if (form.TryGetValue("players", out string players) && !string.IsNullOrEmpty(players))
                server.Players = players;
            // The game sends info=Base64(name) with '+' escaped as '-'; clients Base64Decode it as-is.
            if (form.TryGetValue("info", out string info) && !string.IsNullOrEmpty(info))
                server.InfoB64 = info.Replace('-', '+');
            if (form.TryGetValue("pastebin", out string pastebin) && !string.IsNullOrEmpty(pastebin))
                server.Pastebin = pastebin;
            if (form.TryGetValue("gameVersion", out string gameVersion) && !string.IsNullOrEmpty(gameVersion))
                server.Version = gameVersion;
            if (form.TryGetValue("friendlyFire", out string ff)) bool.TryParse(ff, out server.FriendlyFire);
            if (form.TryGetValue("modded", out string modded)) bool.TryParse(modded, out server.Modded);
            if (form.TryGetValue("whitelist", out string wl)) bool.TryParse(wl, out server.Whitelist);

            Console.WriteLine("       server heartbeat: " + key + " (" + server.Players + ")");

            WriteJson(ctx, 200, "{\"success\":true,\"verified\":true,\"error\":\"\",\"token\":\"\"," +
                "\"messages\":[],\"actions\":[],\"authAccepted\":[],\"authRejected\":[]," +
                "\"verificationChallenge\":\"\",\"verificationResponse\":\"\"}");
        }

        /// <summary>
        /// Server browser endpoint (format=json-signed-unix). The game verifies
        /// ECDSA.Verify(payload + "##" + timestamp, signature, ServerConsole.PublicKey) and then
        /// deserializes Base64Decode(payload) as ServerList {"servers":[ServerListItem...]}.
        /// </summary>
        private static void HandleLobbyList(HttpListenerContext ctx)
        {
            StringBuilder sb = new StringBuilder("{\"servers\":[");
            bool first = true;
            foreach (KeyValuePair<string, GameServer> kv in GameServers)
            {
                GameServer s = kv.Value;
                if (DateTime.UtcNow - s.LastSeen > ServerTtl)
                {
                    GameServers.TryRemove(kv.Key, out _);
                    continue;
                }
                if (!first) sb.Append(',');
                first = false;
                sb.Append("{\"serverId\":").Append(s.Id)
                  .Append(",\"ip\":").Append(JsonString(s.Ip))
                  .Append(",\"port\":").Append(s.Port)
                  .Append(",\"players\":").Append(JsonString(s.Players))
                  .Append(",\"info\":").Append(JsonString(s.InfoB64))
                  .Append(",\"pastebin\":").Append(JsonString(s.Pastebin))
                  .Append(",\"version\":").Append(JsonString(s.Version))
                  .Append(",\"friendlyFire\":").Append(s.FriendlyFire ? "true" : "false")
                  .Append(",\"modded\":").Append(s.Modded ? "true" : "false")
                  .Append(",\"whitelist\":").Append(s.Whitelist ? "true" : "false")
                  .Append(",\"officialCode\":0}");
            }
            sb.Append("]}");

            string payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string signature = Crypto.Sign(payload + "##" + timestamp, _keys.Central.Private);

            string json = "{" +
                "\"payload\":" + JsonString(payload) + "," +
                "\"timestamp\":" + timestamp + "," +
                "\"signature\":" + JsonString(signature) + "," +
                "\"nonce\":" + JsonString(RandomToken(24)) + "," +
                "\"error\":\"\"" +
                "}";
            WriteJson(ctx, 200, json);
        }

        private static void HandleAuthenticate(HttpListenerContext ctx)
        {
            Dictionary<string, string> form = ReadForm(ctx);

            string userId = NormalizeUserId(form.GetValueOrDefault("userid"), form.GetValueOrDefault("ticket"));
            string nickname = form.GetValueOrDefault("nickname");
            // Client may send the nickname already base64-encoded (preferred) or raw.
            string nicknameB64 = LooksBase64(nickname) ? nickname : Tokens.EncodeNickname(nickname);

            Session s = new Session
            {
                UserId = userId,
                NicknameBase64 = nicknameB64,
                Country = "XX",
                Flags = 0,
                Expiration = DateTimeOffset.UtcNow.AddHours(6).ToUnixTimeSeconds(),
                Nonce = RandomToken(24),
                PublicKeyHash = form.GetValueOrDefault("publickey")
            };

            string apiToken = RandomToken(40);
            Sessions[apiToken] = s;

            string preauthSig = Tokens.BuildPreauthSignature(s.UserId, s.Flags, s.Country, s.Expiration, _keys.Central.Private);

            Console.WriteLine("       authenticated userId=" + s.UserId);

            string json = "{" +
                "\"success\":true," +
                "\"error\":\"\"," +
                "\"token\":" + JsonString(apiToken) + "," +
                "\"id\":" + JsonString(s.UserId) + "," +
                "\"nonce\":" + JsonString(s.Nonce) + "," +
                "\"country\":" + JsonString(s.Country) + "," +
                "\"flags\":" + s.Flags + "," +
                "\"expiration\":" + s.Expiration + "," +
                "\"preauth\":" + JsonString(preauthSig) + "," +
                "\"globalBan\":\"NO\"," +
                "\"lifetime\":400" +
                "}";
            WriteJson(ctx, 200, json);
        }

        private static void HandleRequestSignature(HttpListenerContext ctx)
        {
            Dictionary<string, string> form = ReadForm(ctx);
            string apiToken = form.GetValueOrDefault("token");

            Session s = null;
            if (!string.IsNullOrEmpty(apiToken))
                Sessions.TryGetValue(apiToken, out s);

            if (s == null)
            {
                // Fallback: allow a userid/nickname-only request so the flow still works if the api token
                // was lost (e.g. server restart). Trust mode only.
                string userId = NormalizeUserId(form.GetValueOrDefault("userid"), null);
                string nickname = form.GetValueOrDefault("nickname");
                s = new Session
                {
                    UserId = userId,
                    NicknameBase64 = LooksBase64(nickname) ? nickname : Tokens.EncodeNickname(nickname),
                    Nonce = RandomToken(24),
                    PublicKeyHash = form.GetValueOrDefault("publickey")
                };
            }

            string authToken = Tokens.BuildAuthToken(s.UserId, s.NicknameBase64, PublicKeyField(s.PublicKeyHash), _keys.Central.Private);
            string authB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authToken));
            string newNonce = RandomToken(24);

            Console.WriteLine("       issued auth token for userId=" + s.UserId);

            // RequestSignatureResponse base64-decodes auth/badge/pub. We leave badge/pub empty.
            string json = "{" +
                "\"success\":true," +
                "\"error\":\"\"," +
                "\"auth\":" + JsonString(authB64) + "," +
                "\"badge\":\"\"," +
                "\"pub\":\"\"," +
                "\"nonce\":" + JsonString(newNonce) +
                "}";
            WriteJson(ctx, 200, json);
        }

        /// <summary>End-to-end crypto check mirroring exactly what the game verifies.</summary>
        private static int SelfTest()
        {
            int fails = 0;

            // 1) publickey.php signature: game does Verify(key, signature, MasterKey).
            string key = _keys.CentralPublicPem;
            string pkSig = Crypto.Sign(key, _keys.Master.Private);
            bool pkOk = Crypto.Verify(key, pkSig, _keys.Master.Public);
            Console.WriteLine("[selftest] publickey signature (master): " + (pkOk ? "OK" : "FAIL"));
            if (!pkOk) fails++;

            // 2) preauth token: game does VerifyBytes("UserId;Flags;Country;Expiration", sig, CentralPublic).
            string userId = "76561198000000001@steam";
            byte flags = 0; string country = "XX";
            long exp = DateTimeOffset.UtcNow.AddHours(6).ToUnixTimeSeconds();
            string preSig = Tokens.BuildPreauthSignature(userId, flags, country, exp, _keys.Central.Private);
            bool preOk = Crypto.Verify($"{userId};{flags};{country};{exp}", preSig, _keys.Central.Public);
            Console.WriteLine("[selftest] preauth signature (central):  " + (preOk ? "OK" : "FAIL"));
            if (!preOk) fails++;

            // 3) auth token: game splits on "<br>Signature: " and Verify(textBefore, sig, CentralPublic).
            string token = Tokens.BuildAuthToken(userId, Tokens.EncodeNickname("Tester"), PublicKeyField("DEADBEEF"), _keys.Central.Private);
            int i = token.IndexOf("<br>Signature: ", StringComparison.Ordinal);
            string text = token.Substring(0, i);
            string sig = token.Substring(i + 15).Replace("<br>", "");
            bool authOk = Crypto.Verify(text, sig, _keys.Central.Public);
            Console.WriteLine("[selftest] auth token signature (central):" + (authOk ? "OK" : "FAIL"));
            if (!authOk) fails++;

            Console.WriteLine(fails == 0 ? "[selftest] ALL PASSED" : $"[selftest] {fails} FAILED");
            return fails == 0 ? 0 : 1;
        }

        // ---------- helpers ----------

        private static string NormalizeUserId(string userId, string ticketFallback)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                userId = userId.Trim();
                // Strip any local salt so it matches between preauth and auth tokens.
                int dollar = userId.IndexOf('$');
                if (dollar >= 0) userId = userId.Substring(0, dollar);
                return userId.Contains("@") ? userId : userId + "@steam";
            }

            // Derive a stable pseudo-id from the ticket, or random.
            string basis = string.IsNullOrEmpty(ticketFallback) ? Guid.NewGuid().ToString("N") : ticketFallback;
            long num = 76561190000000000L + (uint)basis.GetHashCode();
            return num.ToString() + "@steam";
        }

        private static Dictionary<string, string> ReadForm(HttpListenerContext ctx)
        {
            string body;
            using (StreamReader r = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding ?? Encoding.UTF8))
                body = r.ReadToEnd();

            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(body)) return result;

            // Matches HttpQuery.ToPostArgs: pairs joined by '&', literal '&' inside values escaped as [AMP].
            foreach (string pair in body.Split('&'))
            {
                if (pair.Length == 0) continue;
                int eq = pair.IndexOf('=');
                if (eq < 0) { result[pair] = string.Empty; continue; }
                string k = pair.Substring(0, eq);
                string v = pair.Substring(eq + 1).Replace("[AMP]", "&");
                result[k] = v;
            }
            return result;
        }

        private static bool LooksBase64(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length % 4 != 0) return false;
            foreach (char c in s)
                if (!(char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '='))
                    return false;
            try { Convert.FromBase64String(s); return true; } catch { return false; }
        }

        private static string RandomToken(int len)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            byte[] buf = new byte[len];
            System.Security.Cryptography.RandomNumberGenerator.Fill(buf);
            StringBuilder sb = new StringBuilder(len);
            foreach (byte b in buf) sb.Append(chars[b % chars.Length]);
            return sb.ToString();
        }

        /// <summary>
        /// Keys must live in ONE stable place so the generated master key (pasted into the game) stays
        /// valid. Walk up from the build output to the tool's source folder (where LocalCentralServer.csproj
        /// lives); fall back to the executable folder if not found.
        /// </summary>
        private static string ResolveKeyDir()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(AppContext.BaseDirectory);
                while (dir != null)
                {
                    if (File.Exists(Path.Combine(dir.FullName, "LocalCentralServer.csproj")))
                        return Path.Combine(dir.FullName, "keys");
                    dir = dir.Parent;
                }
            }
            catch { }
            return Path.Combine(AppContext.BaseDirectory, "keys");
        }

        private static string GetArg(string[] args, string name)
        {
            for (int i = 0; i < args.Length - 1; i++)
                if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                    return args[i + 1];
            return null;
        }

        private static void PrintLanHints(string prefix)
        {
            try
            {
                string port = prefix.TrimEnd('/');
                int idx = port.LastIndexOf(':');
                if (idx > 0) port = port.Substring(idx + 1);
                foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            Console.WriteLine($"   reachable at: http://{ip.Address}:{port}/   ({ni.Name})");
                    }
                }
            }
            catch { }
        }

        // ---------- HTTP write ----------

        private static void WriteText(HttpListenerContext ctx, int status, string text)
            => Write(ctx, status, "text/plain; charset=utf-8", text);

        private static void WriteJson(HttpListenerContext ctx, int status, string json)
            => Write(ctx, status, "application/json; charset=utf-8", json);

        private static void Write(HttpListenerContext ctx, int status, string contentType, string body)
        {
            byte[] buf = Encoding.UTF8.GetBytes(body);
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = contentType;
            ctx.Response.ContentLength64 = buf.Length;
            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
            ctx.Response.OutputStream.Close();
        }

        private static string JsonString(string s)
        {
            if (s == null) return "null";
            StringBuilder sb = new StringBuilder(s.Length + 2);
            sb.Append('"');
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20) sb.Append("\\u").Append(((int)c).ToString("x4"));
                        else sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }
    }
}
