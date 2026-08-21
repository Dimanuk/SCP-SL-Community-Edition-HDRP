using System;
using GameCore;
using Mirror;
using UnityEngine;

internal class CentralAuthInterface : ICentralAuth
{
    private readonly ReferenceHub _hub;
    private readonly bool _isServerSide;

    public ReferenceHub GetHub() => _hub;

    public CentralAuthInterface(ReferenceHub hub, bool server)
    {
        _hub = hub;
        _isServerSide = server;
    }

    public void TokenGenerated(string token)
    {
        GameCore.Console.AddLog("Authentication token obtained from central server.", Color.green);
        _hub.characterClassManager.CmdSendToken(token);
    }

    public void RequestBadge(string token)
    {
        _hub.serverRoles.RequestBadge(token);
    }

    public void RequestPublicPart(string token)
    {
        _hub.serverRoles.SetPublicPart(token);
    }

    public void Fail()
    {
        if (_isServerSide)
        {
            ServerConsole.AddLog("Failed to validate authentication token.");
            ServerConsole.Disconnect(_hub.connectionToClient, "Failed to validate authentication token.");
        }
        else
        {
            GameCore.Console.AddLog("Failed to obtain authentication token from central server.", Color.red);
            _hub.connectionToServer.Disconnect();
        }
    }

    public void Ok(string userId, string userId2, string ban, string server, bool bypass, bool bypassWl, bool DNT, string serial, string vacSession, string rqIp, string asn, bool bypassIpCheck)
    {
        switch (ban)
        {
            case "NO":
                ServerConsole.AddLog($"Accepted authentication token of user {userId} signed by {server}. No active global bans.");
                break;
            case "M1":
                ServerConsole.AddLog($"Accepted authentication token of user {userId} signed by {server}. Player is globally muted.");
                break;
            case "M2":
                ServerConsole.AddLog($"Accepted authentication token of user {userId} signed by {server}. Player is globally muted on intercom.");
                break;
            default:
                ServerConsole.AddLog($"Accepted authentication token of user {userId} signed by {server}. Active global ban present in the token.");
                break;
        }

        string banStatus = ban == "NO" ? "without any global bans present" : $"with global ban status {ban}";
        _hub.gameConsoleTransmission.SendToClient($"Accepted your authentication token (your user id is {userId}) {banStatus} signed by {server} server.", "green");

        CharacterClassManager characterClassManager = _hub.characterClassManager;
        ServerRoles serverRoles = _hub.serverRoles;

        characterClassManager.AuthTokenSerial = serial;
        characterClassManager.RequestIp = rqIp;
        characterClassManager.VacSession = vacSession;
        characterClassManager.Asn = asn;

        if (DNT)
        {
            serverRoles.SetDoNotTrack();
        }

        string userIdClean = userId.Contains("$") ? userId.Substring(0, userId.IndexOf("$", StringComparison.Ordinal)) : userId;

        bool hasLocalBan = (userIdClean != null && (!bypass || !ServerStatic.GetPermissionsHandler().IsVerified) && BanHandler.QueryBan(userIdClean, null).Key != null) ||
                           (userId2 != null && (!bypass || !ServerStatic.GetPermissionsHandler().IsVerified) && BanHandler.QueryBan(userId2, null).Key != null);

        if (hasLocalBan)
        {
            TargetConsolePrint("You are banned from this server.", "red");
            ServerConsole.AddLog("Player kicked due to local User ID ban.");
            ServerConsole.Disconnect(_hub.connectionToClient, "You are banned from this server.");
            return;
        }

        if ((ConfigFile.ServerConfig.GetBool("use_global_bans", def: true) || ServerStatic.PermissionsHandler.IsVerified) && ban != "NO" && ban != "M1" && ban != "M2")
        {
            TargetConsolePrint(ban, "red");
            ServerConsole.AddLog($"Player with ID {userId} kicked due to an active global ban: {ban}.");
            ServerConsole.Disconnect(_hub.connectionToClient, ban);
            return;
        }

        if (!bypassIpCheck && rqIp != "N/A" && ServerConsole.EnforceSameIp)
        {
            string address = _hub.connectionToClient.address;
            bool ipFormatMatches = (address.Contains(".") && rqIp.Contains(".")) || (address.Contains(":") && rqIp.Contains(":"));

            if (ipFormatMatches)
            {
                bool isLocalAddress = false;
                if (ServerConsole.SkipEnforcementForLocalAddresses)
                {
                    isLocalAddress = address == "127.0.0.1" || address.StartsWith("10.") || address.StartsWith("192.168.");
                    if (!isLocalAddress && address.StartsWith("172."))
                    {
                        string[] segments = address.Split('.');
                        if (segments.Length == 4 && byte.TryParse(segments[1], out byte secondOctet) && secondOctet >= 16 && secondOctet <= 31)
                        {
                            isLocalAddress = true;
                        }
                    }
                }

                if (!isLocalAddress && address != rqIp)
                {
                    TargetConsolePrint("Authentication token has been issued to a different IP address.", "red");
                    TargetConsolePrint($"Your IP address: {address}", "red");
                    TargetConsolePrint($"Issued to: {rqIp}", "red");
                    ServerConsole.AddLog("Player kicked due to IP addresses mismatch.");
                    ServerConsole.Disconnect(_hub.connectionToClient, "Authentication token has been issued to a different IP address.");
                    return;
                }
            }
        }

        VoiceChat.VcMuteFlags vcMuteFlags = VoiceChat.VcMuteFlags.None;

        if (VoiceChat.VoiceChatMutes.QueryLocalMute(userIdClean) || (userId2 != null && VoiceChat.VoiceChatMutes.QueryLocalMute(userId2)))
        {
            vcMuteFlags |= VoiceChat.VcMuteFlags.LocalRegular;
            TargetConsolePrint("You are muted on the voice chat by the server administrator.", "red");
        }

        if ((ConfigFile.ServerConfig.GetBool("global_mutes_voicechat", def: true) || ServerStatic.PermissionsHandler.IsVerified) && ban == "M1")
        {
            vcMuteFlags |= VoiceChat.VcMuteFlags.GlobalRegular;
            TargetConsolePrint("You are globally muted on the voice chat.", "red");
        }

        if (VoiceChat.VoiceChatMutes.QueryLocalMute(userIdClean, intercom: true) || (userId2 != null && VoiceChat.VoiceChatMutes.QueryLocalMute(userId2, intercom: true)))
        {
            vcMuteFlags |= VoiceChat.VcMuteFlags.LocalIntercom;
            TargetConsolePrint("You are muted on the intercom by the server administrator.", "red");
        }
        else if ((ConfigFile.ServerConfig.GetBool("global_mutes_intercom", def: true) || ServerStatic.PermissionsHandler.IsVerified) && ban == "M2")
        {
            vcMuteFlags |= VoiceChat.VcMuteFlags.GlobalIntercom;
            TargetConsolePrint("You are globally muted on the intercom.", "red");
        }

        serverRoles.BypassStaff |= bypass;
        if (serverRoles.BypassStaff)
        {
            vcMuteFlags = VoiceChat.VcMuteFlags.None;
            TargetConsolePrint("You have the ban bypass flag, so you can't be banned from this server.", "cyan");
        }

        VoiceChat.VoiceChatMutes.SetFlags(_hub, vcMuteFlags);
        serverRoles.StartServerChallenge(0);
    }

    public void FailToken(string reason)
    {
        TargetConsolePrint($"Your authentication token is invalid - {reason}", "red");
        ServerConsole.AddLog("Rejected invalid authentication token.");
        ServerConsole.Disconnect(_hub.connectionToClient, reason);
    }

    private void TargetConsolePrint(string txt, string color)
    {
        _hub.gameConsoleTransmission.SendToClient(txt, color);
    }
}