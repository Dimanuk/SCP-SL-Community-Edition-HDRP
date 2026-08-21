using System;
using System.Collections.Generic;
using GameCore;
using RemoteAdmin.Interfaces;

namespace RemoteAdmin.Communication
{
    public class RaPlayerAuth : IServerCommunication
    {
        public int DataId => 3;

        public void ReceiveData(CommandSender sender, string data)
        {
            if (sender is PlayerCommandSender playerCommandSender
                && !playerCommandSender.ServerRoles.Staff
                && !CommandProcessor.CheckPermissions(sender, PlayerPermissions.PlayerSensitiveDataAccess))
            {
                return;
            }

            string[] newargs;
            List<ReferenceHub> list = Utils.RAUtils.ProcessPlayerIdOrNamesList(
                new ArraySegment<string>(data.Split(' ')), 0, out newargs);

            if (list.Count == 0 || list.Count > 1)
                return;

            ReferenceHub target = list[0];

            if (string.IsNullOrEmpty(target.characterClassManager.AuthToken))
            {
                sender.RaReply(
                    "PlayerInfo#Can't obtain auth token. Is server using offline mode or you selected the host?",
                    success: false,
                    logToConsole: true,
                    "PlayerInfo");
                return;
            }

            ServerLogs.AddLog(
                ServerLogs.Modules.DataAccess,
                $"{sender.LogName} accessed authentication token of player {target.PlayerId} ({target.nicknameSync.MyNick}).",
                ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);

            sender.RaReply(
                $"PlayerInfo#<color=white>Authentication token of player {target.nicknameSync.MyNick} ({target.PlayerId}):\n{target.characterClassManager.AuthToken}</color>",
                success: true,
                logToConsole: true,
                "null");

            RaPlayerQR.Send(sender, isBig: true, target.characterClassManager.AuthToken);
        }

        public static void Request(string playerIds)
        {
            if (ReferenceHub.TryGetLocalHub(out var localHub))
            {
                string query = "$3 " + playerIds;
                localHub.queryProcessor.CmdSendQuery(query, false);
            }
        }
    }
}