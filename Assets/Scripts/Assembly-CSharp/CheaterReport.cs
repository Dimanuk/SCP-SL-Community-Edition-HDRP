using System;
using System.Collections.Generic;
using System.Threading;
using Cryptography;
using GameCore;
using Mirror;
using Mirror.RemoteCalls;
using NorthwoodLib;
using Security;
using UnityEngine;

public class CheaterReport : NetworkBehaviour
{
    private static readonly Dictionary<char, string> CharacterReplacements;

    internal static bool SendReportsByWebhooks;
    internal static string WebhookUrl;
    internal static string WebhookUsername;
    internal static string WebhookAvatar;
    internal static string ServerName;
    internal static string ReportHeader;
    internal static string ReportContent;
    internal static int WebhookColor;

    private int _reportedPlayersAmount;
    private float _lastReport;

    private HashSet<uint> _reportedPlayers;

    private RateLimit _commandRateLimit;

    private ReferenceHub _hub;

    private void Start()
    {
        _hub = ReferenceHub.GetHub(this);
        _commandRateLimit = _hub.playerRateLimitHandler.RateLimits[2];
    }

    internal void Report(uint playerNetId, string reason, bool notifyGm)
    {
        if (!ReferenceHub.TryGetHubNetID(playerNetId, out var targetHub))
        {
            GameCore.Console.AddLog("[REPORTING] Can't find player with that PlayerID.", Color.red);
            return;
        }

        byte[] signature = null;

        if (notifyGm)
        {
            string userId = targetHub.characterClassManager.UserId;
            string dataToSign = string.Concat(userId, ";", reason);
            
            var sessionKeys = CentralAuthManager.SessionKeys;
            signature = ECDSA.SignBytes(dataToSign, sessionKeys.Private);
        }

        CmdReport(playerNetId,reason,signature,notifyGm);
    }

    [Command(channel = 4)]
    private void CmdReport(uint playerNetId, string reason, byte[] signature, bool notifyGm)
    {
        if (!_commandRateLimit.CanExecute(countUsage: true) || reason == null)
            return;

        float timeSinceLastReport = Time.time - _lastReport;
        if (timeSinceLastReport < 2f)
        {
            _hub.gameConsoleTransmission.SendToClient(
                connectionToClient,
                "[REPORTING] Reporting rate limit exceeded (1).",
                "red");
            return;
        }

        if (timeSinceLastReport > 60f)
            _reportedPlayersAmount = 0;

        if (_reportedPlayersAmount > 5)
        {
            _hub.gameConsoleTransmission.SendToClient(
                connectionToClient,
                "[REPORTING] Reporting rate limit exceeded (2).",
                "red");
            return;
        }

        if (notifyGm && (!ServerStatic.PermissionsHandler.IsVerified || string.IsNullOrEmpty(ServerConsole.Password)))
        {
            _hub.gameConsoleTransmission.SendToClient(
                connectionToClient,
                "[REPORTING] Server is not verified - you can't use report feature on this server.",
                "red");
            return;
        }

        if (!ReferenceHub.TryGetHubNetID(playerNetId, out var reportedHub))
        {
            _hub.gameConsoleTransmission.SendToClient(
                connectionToClient,
                "[REPORTING] Can't find player with that PlayerID.",
                "red");
            return;
        }

        CharacterClassManager reportedCcm = reportedHub.characterClassManager;

        if (_reportedPlayers == null)
            _reportedPlayers = new HashSet<uint>();

        if (_reportedPlayers.Contains(playerNetId))
        {
            _hub.gameConsoleTransmission.SendToClient(
                connectionToClient,
                "[REPORTING] You have already reported that player.",
                "red");
            return;
        }

        if (string.IsNullOrEmpty(reportedCcm.UserId))
        {
            _hub.gameConsoleTransmission.SendToClient(
                connectionToClient,
                "[REPORTING] Failed: User ID of reported player is null.",
                "red");
            return;
        }

        if (string.IsNullOrEmpty(_hub.characterClassManager.UserId))
        {
            _hub.gameConsoleTransmission.SendToClient(
                connectionToClient,
                "[REPORTING] Failed: your User ID is null.",
                "red");
            return;
        }

        if (_hub.characterClassManager.UserId == reportedCcm.UserId)
        {
            _hub.gameConsoleTransmission.SendToClient(
                connectionToClient,
                "[REPORTING] You can't report yourself!" + Environment.NewLine,
                "yellow");
            return;
        }

        string reportedNickname = reportedHub.nicknameSync.MyNick;

        if (!notifyGm)
        {
            GameCore.Console.AddLog(
                "Player " + Misc.LoggedNameFromRefHub(_hub) + 
                " reported player " + Misc.LoggedNameFromRefHub(reportedHub) + 
                " with reason " + reason + ".",
                Color.gray);

            _hub.gameConsoleTransmission.SendToClient(
                connectionToClient,
                "[REPORTING] Player report successfully sent to local administrators.",
                "green");

            if (SendReportsByWebhooks)
            {
                var localReportedCcm = reportedCcm;
                var localReportedNickname = reportedNickname;
                var localReason = reason;
                var localPlayerNetId = playerNetId;

                Thread thread = new Thread(() =>
                {
                    LogReport(
                        _hub.gameConsoleTransmission,
                        _hub.characterClassManager.UserId,
                        localReportedCcm.UserId,
                        ref localReason,
                        localPlayerNetId,
                        notifyGm: false,
                        _hub.nicknameSync.MyNick,
                        localReportedNickname);
                });
                thread.Priority = System.Threading.ThreadPriority.Lowest;
                thread.IsBackground = true;
                thread.Name = "Reporting player (locally) - " + localReportedCcm.UserId + 
                            " by " + _hub.characterClassManager.UserId;
                thread.Start();
            }
            return;
        }

        if (signature == null)
            return;

        string verifyData = reportedCcm.SyncedUserId + ";" + reason;
        ServerRoles serverRoles = GetComponent<ServerRoles>();
        
        if (!ECDSA.VerifyBytes(verifyData, signature, serverRoles.PublicKey))
        {
            _hub.gameConsoleTransmission.SendToClient(
                connectionToClient,
                "[REPORTING] Invalid report signature.",
                "red");
            return;
        }

        _lastReport = Time.time;
        _reportedPlayersAmount++;

        GameCore.Console.AddLog(
            "Player " + Misc.LoggedNameFromRefHub(_hub) + 
            " reported player " + Misc.LoggedNameFromRefHub(reportedHub) + 
            " with reason " + reason + ". Sending report to Global Moderation.",
            Color.gray);

        var gmReportedCcm = reportedCcm;
        var gmReportedNickname = reportedNickname;
        var gmReason = reason;
        var gmSignature = signature;
        var gmPlayerNetId = playerNetId;

        Thread gmThread = new Thread(() =>
        {
            IssueReport(
                _hub.gameConsoleTransmission,
                _hub.characterClassManager.UserId,
                gmReportedCcm.UserId,
                gmReportedCcm.AuthToken,
                gmReportedCcm.connectionToClient.address,
                _hub.characterClassManager.AuthToken,
                _hub.connectionToClient.address,
                ref gmReason,
                ref gmSignature,
                ECDSA.KeyToString(serverRoles.PublicKey),
                gmPlayerNetId,
                _hub.nicknameSync.MyNick,
                gmReportedNickname);
        });
        gmThread.Priority = System.Threading.ThreadPriority.Lowest;
        gmThread.IsBackground = true;
        gmThread.Name = "Reporting player - " + gmReportedCcm.UserId + 
                       " by " + _hub.characterClassManager.UserId;
        gmThread.Start();
    }

    [Server]
    private void IssueReport(
        GameConsoleTransmission reporter,
        string reporterUserId,
        string reportedUserId,
        string reportedAuth,
        string reportedIp,
        string reporterAuth,
        string reporterIp,
        ref string reason,
        ref byte[] signature,
        string reporterPublicKey,
        uint reportedNetId,
        string reporterNickname,
        string reportedNickname)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'System.Void CheaterReport::IssueReport(...)' called when server was not active");
            return;
        }

        try
        {
            object[] args = new object[10];
            args[0] = StringUtils.Base64Encode(reporterAuth);
            args[1] = reporterIp;
            args[2] = StringUtils.Base64Encode(reportedAuth);
            args[3] = reportedIp;
            args[4] = StringUtils.Base64Encode(ConvertToLatin(reason));
            args[5] = Convert.ToBase64String(signature);
            args[6] = StringUtils.Base64Encode(reporterPublicKey);
            args[7] = ServerConsole.Password;
            args[8] = ServerConsole.PortToReport;
            args[9] = ServerConsole.Ip;

            string data = string.Format(
                "reporterAuth={0}&reporterIp={1}&reportedAuth={2}&reportedIp={3}&reason={4}&signature={5}&reporterKey={6}&token={7}&port={8}&serverIp={9}",
                args);

            string response = HttpQuery.Post(CentralServer.StandardUrl + "ingamereport.php", data);

            if (reporter == null)
                return;

            switch (response)
            {
                case "OK":
                    _reportedPlayers.Add(reportedNetId);
                    reporter.SendToClient(
                        connectionToClient,
                        "[REPORTING] Player report successfully sent.",
                        "green");
                    break;

                case "ReportedUserIDAlreadyReported":
                    reporter.SendToClient(
                        connectionToClient,
                        "[REPORTING] A report for this User ID already exists!" + Environment.NewLine,
                        "yellow");
                    break;

                case "RateLimited":
                    reporter.SendToClient(
                        connectionToClient,
                        "[REPORTING] You are Ratelimited! Try again tomorrow." + Environment.NewLine,
                        "red");
                    break;

                default:
                    reporter.SendToClient(
                        connectionToClient,
                        "[REPORTING] Error during **PROCESSING** player report:" + Environment.NewLine + response,
                        "red");
                    break;
            }
        }
        catch (Exception ex)
        {
            GameCore.Console.AddLog(
                "[HOST] Error during **SENDING** player report:" + Environment.NewLine + ex.Message,
                Color.red);

            if (reporter != null)
            {
                reporter.SendToClient(
                    connectionToClient,
                    "[REPORTING] Error during **SENDING** player report.",
                    "yellow");
            }
        }

        if (SendReportsByWebhooks)
        {
            LogReport(reporter, reporterUserId, reportedUserId, ref reason, 
                reportedNetId, notifyGm: true, reporterNickname, reportedNickname);
        }
    }

    [Server]
    private void LogReport(
        GameConsoleTransmission reporter,
        string reporterUserId,
        string reportedUserId,
        ref string reason,
        uint reportedNetId,
        bool notifyGm,
        string reporterNickname,
        string reportedNickname)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'System.Void CheaterReport::LogReport(...)' called when server was not active");
            return;
        }

        if (SubmitReport(reporterUserId, reportedUserId, reason, reportedNetId, 
            reporterNickname, reportedNickname, friendlyFire: false))
        {
            if (!notifyGm)
            {
                _reportedPlayers.Add(reportedNetId);
                reporter.SendToClient(
                    connectionToClient,
                    "[REPORTING] Player report successfully sent to local administrators by webhooks.",
                    "green");
            }
        }
        else
        {
            reporter.SendToClient(
                connectionToClient,
                "[REPORTING] Failed to send report to local administrators by webhooks.",
                "red");
        }
    }

    internal static bool SubmitReport(
        string reporterUserId,
        string reportedUserId,
        string reason,
        uint reportedId,
        string reporterNickname,
        string reportedNickname,
        bool friendlyFire)
    {
        try
        {
            string webhookUrl = friendlyFire ? FriendlyFireConfig.WebhookUrl : WebhookUrl;

            DiscordEmbed[] embeds = new DiscordEmbed[1];
            DiscordEmbedField[] fields = new DiscordEmbedField[10];

            fields[0] = new DiscordEmbedField("Server Name", ServerName, inline: false);
            fields[1] = new DiscordEmbedField("Server Endpoint", 
                $"{ServerConsole.Ip}:{ServerConsole.PortToReport}", inline: false);
            fields[2] = new DiscordEmbedField("Reporter UserID", 
                AsDiscordCode(reporterUserId), inline: false);
            fields[3] = new DiscordEmbedField("Reporter Nickname", 
                DiscordSanitize(reporterNickname), inline: false);
            fields[4] = new DiscordEmbedField("Reported UserID", 
                AsDiscordCode(reportedUserId), inline: false);
            fields[5] = new DiscordEmbedField("Reported Nickname", 
                DiscordSanitize(reportedNickname), inline: false);
            fields[6] = new DiscordEmbedField("Reported NetID", 
                reportedId.ToString(), inline: false);
            fields[7] = new DiscordEmbedField("Reason", 
                DiscordSanitize(reason), inline: false);
            fields[8] = new DiscordEmbedField("Timestamp", 
                TimeBehaviour.Rfc3339Time(), inline: false);
            fields[9] = new DiscordEmbedField("UTC Timestamp", 
                TimeBehaviour.Rfc3339Time(DateTimeOffset.UtcNow), inline: false);

            embeds[0] = new DiscordEmbed(ReportHeader, "rich", ReportContent, WebhookColor, fields);

            DiscordWebhook webhook = new DiscordWebhook(
                string.Empty, WebhookUsername, WebhookAvatar, 
                tts: false, embeds);

            string payload = "payload_json=" + Utf8Json.JsonSerializer.ToJsonString(webhook);
            HttpQuery.Post(webhookUrl, payload);

            return true;
        }
        catch (Exception ex)
        {
            ServerConsole.AddLog("Failed to send report by webhook: " + ex.Message);
            Debug.LogException(ex);
            return false;
        }
    }

    private static string ConvertToLatin(string str)
    {
        foreach (var kvp in CharacterReplacements)
        {
            str = str.Replace(kvp.Key.ToString(), kvp.Value);
        }
        return str;
    }

    private static string AsDiscordCode(string text)
    {
        return "`" + text.Replace("`", "'") + "`";
    }

    private static string DiscordSanitize(string text)
    {
        return text
            .Replace("<", "(")
            .Replace(">", ")")
            .Replace("@", "@ ")
            .Replace("`", "'")
            .Replace("~~", "∼∼")
            .Replace("*", "★")
            .Replace("_", "＿")
            .Replace("&", " [AMP] ")
            .Replace("?", " [QM] ");
    }

    static CheaterReport()
    {
        CharacterReplacements = new Dictionary<char, string>
        {
            { 'а', "a" }, { 'б', "b" }, { 'в', "v" }, { 'г', "g" },
            { 'д', "d" }, { 'е', "e" }, { 'ё', "yo" }, { 'ж', "zh" },
            { 'з', "z" }, { 'и', "i" }, { 'й', "j" }, { 'к', "k" },
            { 'л', "l" }, { 'м', "m" }, { 'н', "n" }, { 'о', "o" },
            { 'п', "p" }, { 'р', "r" }, { 'с', "s" }, { 'т', "t" },
            { 'у', "u" }, { 'ф', "f" }, { 'х', "h" }, { 'ц', "c" },
            { 'ч', "ch" }, { 'ш', "sh" }, { 'щ', "sch" }, { 'ъ', "j" },
            { 'ы', "i" }, { 'ь', "j" }, { 'э', "e" }, { 'ю', "yu" },
            { 'я', "ya" },
            { 'А', "A" }, { 'Б', "B" }, { 'В', "V" }, { 'Г', "G" },
            { 'Д', "D" }, { 'Е', "E" }, { 'Ё', "Yo" }, { 'Ж', "Zh" },
            { 'З', "Z" }, { 'И', "I" }, { 'Й', "J" }, { 'К', "K" },
            { 'Л', "L" }, { 'М', "M" }, { 'Н', "N" }, { 'О', "O" },
            { 'П', "P" }, { 'Р', "R" }, { 'С', "S" }, { 'Т', "T" },
            { 'У', "U" }, { 'Ф', "F" }, { 'Х', "H" }, { 'Ц', "C" },
            { 'Ч', "Ch" }, { 'Ш', "Sh" }, { 'Щ', "Sch" }, { 'Ъ', "J" },
            { 'Ы', "I" }, { 'Ь', "J" }, { 'Э', "E" }, { 'Ю', "Yu" },
            { 'Я', "Ya" }
        };

        SendReportsByWebhooks = false;
    }
}
