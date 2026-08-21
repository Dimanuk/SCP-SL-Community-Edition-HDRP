using System;
using System.IO;

/// <summary>
/// Local test-mode central server override (see Tools/LocalCentralServer).
///
/// Activated by dropping a "localcentral.txt" file into the game/server app folder. The file format
/// is simple "key=value" lines (or just the URL on the first line):
///
///     url=http://192.168.0.50:5000/
///     userid=76561198000000001@steam   ; optional, identity this client requests (online clients)
///     nickname=Gleb                     ; optional, display name this client requests
///
/// When active:
///   * <see cref="CentralServerKeyCache"/> trusts the local test master key instead of Northwood's.
///   * <see cref="CentralServer"/> points StandardUrl/MasterUrl at <see cref="Url"/> over plain HTTP.
///   * <see cref="CentralAuthManager"/> authenticates against the local server without Steam/launcher.
///
/// Only ever use this on a private network (LAN / Radmin VPN). It disables real identity verification.
/// </summary>
public static class LocalCentralServer
{
    private const string FileName = "localcentral.txt";

    private static bool _loaded;
    private static bool _active;
    private static string _url;
    private static string _userId;
    private static string _nickname;

    public static bool IsActive { get { EnsureLoaded(); return _active; } }

    /// <summary>Base URL ending with '/', e.g. "http://192.168.0.50:5000/". Null when inactive.</summary>
    public static string Url { get { EnsureLoaded(); return _url; } }

    /// <summary>Optional UserId this client should request (online client identity). May be null.</summary>
    public static string UserId { get { EnsureLoaded(); return _userId; } }

    /// <summary>Optional nickname this client should request. May be null.</summary>
    public static string Nickname { get { EnsureLoaded(); return _nickname; } }

    private static void EnsureLoaded()
    {
        if (_loaded)
            return;
        _loaded = true;

        try
        {
            string path = FileManager.GetAppFolder() + FileName;
            if (!File.Exists(path))
                return;

            foreach (string raw in File.ReadAllLines(path))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";"))
                    continue;

                int eq = line.IndexOf('=');
                if (eq < 0)
                {
                    // Bare line: treat as the URL.
                    if (_url == null && line.Contains("://"))
                        _url = NormalizeUrl(line);
                    continue;
                }

                string key = line.Substring(0, eq).Trim().ToLowerInvariant();
                string value = line.Substring(eq + 1).Trim();
                switch (key)
                {
                    case "url": _url = NormalizeUrl(value); break;
                    case "userid": _userId = value; break;
                    case "nickname": _nickname = value; break;
                }
            }

            _active = !string.IsNullOrEmpty(_url);
        }
        catch
        {
            _active = false;
        }
    }

    private static string NormalizeUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return url;
        return url.EndsWith("/") ? url : url + "/";
    }

    /// <summary>
    /// Resolves the identity this client should authenticate with, in priority order:
    ///   1. explicit userid/nickname from localcentral.txt,
    ///   2. the real Steam account (SteamID64 + persona name) if Steam is running,
    ///   3. a per-machine persistent random id (so bans/roles stay stable across restarts).
    /// </summary>
    public static void ResolveIdentity(out string userId, out string nickname)
    {
        userId = UserId;
        nickname = Nickname;

        if (string.IsNullOrEmpty(userId))
        {
            try
            {
                if (!SteamManager.IsSteamReady())
                    SteamManager.StartClient();
            }
            catch { }

            if (SteamManager.IsSteamReady() && SteamManager.SteamId64 != 0UL)
            {
                userId = SteamManager.SteamId64 + "@steam";
                if (string.IsNullOrEmpty(nickname))
                {
                    try { nickname = SteamManager.GetPersonaName(SteamManager.SteamId64); } catch { }
                }
            }
            else
            {
                userId = GetOrCreateFallbackUserId();
            }
        }

        if (string.IsNullOrEmpty(nickname) || nickname == "Unknown")
            nickname = SafeUserName();
    }

    private static string GetOrCreateFallbackUserId()
    {
        try
        {
            string path = FileManager.GetAppFolder() + "localcentral_identity.txt";
            if (File.Exists(path))
            {
                string existing = File.ReadAllText(path).Trim();
                if (!string.IsNullOrEmpty(existing))
                    return existing;
            }

            long num = 76561190000000000L + new System.Random().Next(1, int.MaxValue);
            string id = num + "@steam";
            File.WriteAllText(path, id);
            return id;
        }
        catch
        {
            long num = 76561190000000000L + (System.Environment.TickCount & 0x7FFFFFFF);
            return num + "@steam";
        }
    }

    private static string SafeUserName()
    {
        try
        {
            string name = System.Environment.UserName;
            return string.IsNullOrEmpty(name) ? "Player" : name;
        }
        catch
        {
            return "Player";
        }
    }
}
