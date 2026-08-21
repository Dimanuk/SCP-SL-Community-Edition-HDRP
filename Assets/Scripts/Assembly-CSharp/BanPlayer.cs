public static class BanPlayer
{
    private const string _globalBanReason = "You have been Globally Banned.";

    public static bool GlobalBanUser(ReferenceHub target, global::CommandSystem.ICommandSender issuer)
    {
        ApplyIpBan(target, issuer, "You have been Globally Banned.", 4294967295L);
        ServerConsole.Disconnect(target.gameObject, "You have been Globally Banned.");
        return true;
    }

    public static bool KickUser(ReferenceHub target, global::CommandSystem.ICommandSender issuer, string reason)
    {
        ServerConsole.Disconnect(target.gameObject, "You have been kicked. Reason: " + reason);
        return true;
    }

    public static bool KickUser(ReferenceHub target, ReferenceHub issuer, string reason)
    {
        return KickUser(target, new global::RemoteAdmin.PlayerCommandSender(issuer), reason);
    }

    public static bool KickUser(ReferenceHub target, string reason)
    {
        return KickUser(target, ServerConsole.Scs, reason);
    }

    public static bool BanUser(ReferenceHub target, string reason, long duration)
    {
        return BanUser(target, ServerConsole.Scs, reason, duration);
    }

    public static bool BanUser(ReferenceHub target, ReferenceHub issuer, string reason, long duration)
    {
        return BanUser(target, new global::RemoteAdmin.PlayerCommandSender(issuer), reason, duration);
    }

    public static bool BanUser(ReferenceHub target, global::CommandSystem.ICommandSender issuer, string reason, long duration)
    {
        if (duration == 0L)
        {
            return KickUser(target, issuer, reason);
        }
        if (target.serverRoles.BypassStaff)
        {
            return false;
        }
        ApplyIpBan(target, issuer, reason, duration);
        long issuanceTime = TimeBehaviour.CurrentTimestamp();
        long banExpirationTime = TimeBehaviour.GetBanExpirationTime((uint)duration);
        string originalName = ValidateNick(target.nicknameSync.MyNick);
        BanHandler.IssueBan(new BanDetails
        {
            OriginalName = originalName,
            Id = target.characterClassManager.UserId,
            IssuanceTime = issuanceTime,
            Expires = banExpirationTime,
            Reason = reason,
            Issuer = issuer.LogName
        }, BanHandler.BanType.UserId);
        if (!string.IsNullOrEmpty(target.characterClassManager.UserId2))
        {
            BanHandler.IssueBan(new BanDetails
            {
                OriginalName = originalName,
                Id = target.characterClassManager.UserId2,
                IssuanceTime = issuanceTime,
                Expires = banExpirationTime,
                Reason = reason,
                Issuer = issuer.LogName
            }, BanHandler.BanType.UserId);
        }
        ServerConsole.Disconnect(target.gameObject, "You have been banned. Reason: " + reason);
        return true;
    }

    private static void ApplyIpBan(ReferenceHub target, global::CommandSystem.ICommandSender issuer, string reason, long duration)
    {
        if (global::GameCore.ConfigFile.ServerConfig.GetBool("ip_banning") && global::GameCore.ConfigFile.ServerConfig.GetBool("gban_ban_ip"))
        {
            long issuanceTime = TimeBehaviour.CurrentTimestamp();
            long banExpirationTime = TimeBehaviour.GetBanExpirationTime((uint)duration);
            BanHandler.IssueBan(new BanDetails
            {
                OriginalName = ValidateNick(target.nicknameSync.MyNick),
                Id = target.connectionToClient.address,
                IssuanceTime = issuanceTime,
                Expires = banExpirationTime,
                Reason = reason,
                Issuer = issuer.LogName
            }, BanHandler.BanType.IP);
        }
    }

    private static string ValidateNick(string username)
    {
        int num = global::GameCore.ConfigFile.ServerConfig.GetInt("ban_nickname_maxlength", 30);
        bool num2 = global::GameCore.ConfigFile.ServerConfig.GetBool("ban_nickname_trimunicode", def: true);
        string text = (string.IsNullOrEmpty(username) ? "(no nick)" : username);
        if (num2)
        {
            text = global::NorthwoodLib.StringUtils.StripUnicodeCharacters(text);
        }
        if (text.Length > num)
        {
            text = text.Substring(0, num);
        }
        return text;
    }
}
