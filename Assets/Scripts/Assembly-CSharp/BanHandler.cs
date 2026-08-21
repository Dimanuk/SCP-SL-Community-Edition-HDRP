using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NorthwoodLib.Pools;

public static class BanHandler
{
    public enum BanType
    {
        NULL = -1,
        UserId = 0,
        IP = 1
    }

    public static BanType GetBanType(int type)
    {
        if (!Enum.IsDefined(typeof(BanType), type))
        {
            return BanType.UserId;
        }
        return (BanType)type;
    }

    public static void Init()
    {
        try
        {
            string uPath = GetPath(BanType.UserId);
            string iPath = GetPath(BanType.IP);

            string dir = Path.GetDirectoryName(uPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(uPath)) File.Create(uPath).Close();
            if (!File.Exists(iPath)) File.Create(iPath).Close();
        }
        catch (Exception e)
        {
            ServerConsole.AddLog($"Can't create ban files! Error: {e.Message}");
        }
        ValidateBans();
    }

    public static bool IssueBan(BanDetails ban, BanType banType, bool forced = false)
    {
        try
        {
            if (banType == BanType.IP && (ban.Id.Equals("localClient", StringComparison.OrdinalIgnoreCase) || ban.Id == "127.0.0.1"))
            {
                return false;
            }

            ban.OriginalName = ban.OriginalName?.Replace(";", ":").Replace("\n", "") ?? "Unknown";
            ban.Issuer = ban.Issuer?.Replace(";", ":").Replace("\n", "") ?? "Server";
            ban.Reason = ban.Reason?.Replace(";", ":").Replace("\n", "") ?? "No reason";

            List<BanDetails> currentBans = GetBans(banType);

            if (!currentBans.Any(b => b.Id == ban.Id))
            {
                File.AppendAllText(GetPath(banType), ban.ToString() + Environment.NewLine);
            }
            else
            {
                RemoveBan(ban.Id, banType, forced: true);
                IssueBan(ban, banType, forced: true);
            }
            return true;
        }
        catch (Exception e)
        {
            ServerConsole.AddLog($"Error issuing ban: {e.Message}");
            return false;
        }
    }

    public static void ValidateBans()
    {
        ServerConsole.AddLog("Validating bans...");
        ValidateBans(BanType.UserId);
        ValidateBans(BanType.IP);
        ServerConsole.AddLog("Bans have been validated.");
    }

    public static void ValidateBans(BanType banType)
    {
        string path = GetPath(banType);
        if (!File.Exists(path)) return;

        List<string> list = File.ReadAllLines(path).ToList();

        List<int> toRemove = ListPool<int>.Shared.Rent();

        try
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                BanDetails details = ProcessBanItem(list[i], banType);
                if (details == null || !CheckExpiration(details, banType))
                {
                    toRemove.Add(i);
                }
            }

            if (toRemove.Count > 0)
            {
                foreach (int index in toRemove)
                {
                    list.RemoveAt(index);
                }
                File.WriteAllLines(path, list);
            }
        }
        finally
        {

            ListPool<int>.Shared.Return(toRemove);
        }
    }

    public static bool CheckExpiration(BanDetails ban, BanType banType)
    {
        if (ban == null) return false;

        if (ban.Expires != 0 && ban.Expires < DateTime.UtcNow.Ticks)
        {
            return false;
        }
        return true;
    }

    public static void RemoveBan(string id, BanType banType, bool forced = false)
    {
        string path = GetPath(banType);
        if (!File.Exists(path)) return;

        id = id.Trim();

        var linesToKeep = File.ReadAllLines(path).Where(l =>
        {
            var b = ProcessBanItem(l, banType);
            return b != null && !string.Equals(b.Id, id, StringComparison.OrdinalIgnoreCase);
        }).ToArray();

        File.WriteAllLines(path, linesToKeep);
    }

    public static List<BanDetails> GetBans(BanType banType)
    {
        string path = GetPath(banType);
        if (!File.Exists(path)) return new List<BanDetails>();

        return File.ReadAllLines(path)
                   .Select(line => ProcessBanItem(line, banType))
                   .Where(b => b != null)
                   .ToList();
    }

    public static BanDetails GetBan(string id, BanType banType)
    {
        return GetBans(banType).FirstOrDefault(b => string.Equals(b.Id, id, StringComparison.OrdinalIgnoreCase));
    }

    public static KeyValuePair<BanDetails, BanDetails> QueryBan(string userId, string ip)
    {
        BanDetails userBan = null;
        BanDetails ipBan = null;

        if (!string.IsNullOrEmpty(userId))
        {
            userId = userId.Replace(";", "").Trim();
            userBan = GetBan(userId, BanType.UserId);
            if (userBan != null && !CheckExpiration(userBan, BanType.UserId)) userBan = null;
        }

        if (!string.IsNullOrEmpty(ip))
        {
            ip = ip.Replace(";", "").Trim();
            ipBan = GetBan(ip, BanType.IP);
            if (ipBan != null && !CheckExpiration(ipBan, BanType.IP)) ipBan = null;
        }

        return new KeyValuePair<BanDetails, BanDetails>(userBan, ipBan);
    }

    public static BanDetails ProcessBanItem(string line, BanType banType)
    {
        if (string.IsNullOrWhiteSpace(line) || !line.Contains(";")) return null;

        string[] parts = line.Split(';');
        if (parts.Length < 6) return null;

        string id = parts[1].Trim();

        if (banType == BanType.UserId && !id.Contains("@") && !id.Contains(":"))
        {
            id += "@steam";
        }

        try
        {
            return new BanDetails
            {
                OriginalName = parts[0],
                Id = id,
                Expires = long.Parse(parts[2].Trim()),
                Reason = parts[3],
                Issuer = parts[4],
                IssuanceTime = long.Parse(parts[5].Trim())
            };
        }
        catch
        {
            return null;
        }
    }

    public static string GetPath(BanType banType)
    {
        if (banType == BanType.IP)
        {
            return global::GameCore.ConfigSharing.Paths[0] + "IpBans.txt";
        }
        return global::GameCore.ConfigSharing.Paths[0] + "UserIdBans.txt";
    }
}