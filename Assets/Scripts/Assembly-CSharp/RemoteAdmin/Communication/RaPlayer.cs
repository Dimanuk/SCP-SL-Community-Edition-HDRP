using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameCore;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using RemoteAdmin.Interfaces;
using UnityEngine;
using VoiceChat;

namespace RemoteAdmin.Communication
{
    public class RaPlayer : IServerCommunication, IClientCommunication
    {
        public int DataId => 1;

        public void ReceiveData(CommandSender sender, string data)
        {
            string[] array = data.Split(' ');
            if (array.Length != 2 || !int.TryParse(array[0], out int result))
                return;

            bool isShort = result == 1;
            PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;

            if (!isShort && playerCommandSender != null && !playerCommandSender.ServerRoles.Staff
                && !CommandProcessor.CheckPermissions(sender, PlayerPermissions.PlayerSensitiveDataAccess))
                return;

            string[] newargs;
            List<ReferenceHub> list = Utils.RAUtils.ProcessPlayerIdOrNamesList(
                new ArraySegment<string>(array.Skip(1).ToArray()), 0, out newargs);

            if (list.Count == 0)
                return;

            bool canViewUserId = PermissionsHandler.IsPermitted(sender.Permissions, 18007046uL);
            if (playerCommandSender != null && (playerCommandSender.ServerRoles.Staff || playerCommandSender.ServerRoles.RaEverywhere))
                canViewUserId = true;

            if (list.Count > 1)
            {
                StringBuilder sb = StringBuilderPool.Shared.Rent("<color=white>");
                sb.Append("Selecting multiple players:");
                sb.Append("\nPlayer ID: <color=green><link=CP_ID>\uf0c5</link></color>");
                sb.Append("\nIP Address: " + (!isShort ? "<color=green><link=CP_IP>\uf0c5</link></color>" : "[REDACTED]"));
                sb.Append("\nUser ID: " + (canViewUserId ? "<color=green><link=CP_USERID>\uf0c5</link></color>" : "[REDACTED]"));
                sb.Append("</color>");

                string ids = string.Empty;
                string ips = string.Empty;
                string userIds = string.Empty;

                foreach (ReferenceHub hub in list)
                {
                    ids += hub.PlayerId + ".";
                    if (!isShort)
                        ips += hub.networkIdentity.connectionToClient?.address + ",";
                    if (canViewUserId)
                        userIds += hub.characterClassManager.UserId + ".";
                }

                if (ids.Length > 0)
                    RaClipboard.Send(sender, RaClipboard.RaClipBoardType.PlayerId, ids);
                if (ips.Length > 0)
                    RaClipboard.Send(sender, RaClipboard.RaClipBoardType.Ip, ips);
                if (userIds.Length > 0)
                    RaClipboard.Send(sender, RaClipboard.RaClipBoardType.UserId, userIds);

                sender.RaReply($"${DataId} {sb}", success: true, logToConsole: true, string.Empty);
                StringBuilderPool.Shared.Return(sb);
                return;
            }

            ReferenceHub target = list[0];

            ServerLogs.AddLog(ServerLogs.Modules.DataAccess,
                $"{sender.LogName} accessed IP address of player {target.PlayerId} ({target.nicknameSync.MyNick}).",
                ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);

            bool hasGameplayData = PermissionsHandler.IsPermitted(sender.Permissions, PlayerPermissions.GameplayData);

            CharacterClassManager ccm = target.characterClassManager;
            NicknameSync nick = target.nicknameSync;
            NetworkConnectionToClient conn = target.networkIdentity.connectionToClient;
            ServerRoles sRoles = target.serverRoles;

            if (sender is PlayerCommandSender pcs2)
                pcs2.ReferenceHub.queryProcessor.GameplayData = hasGameplayData;

            StringBuilder sb2 = StringBuilderPool.Shared.Rent("<color=white>");

            sb2.Append("Nickname: " + nick.CombinedName);

            sb2.Append($"\nPlayer ID: {target.PlayerId} <color=green><link=CP_ID>\uf0c5</link></color>");
            RaClipboard.Send(sender, RaClipboard.RaClipBoardType.PlayerId, $"{target.PlayerId}");

            if (conn == null)
                sb2.Append("\nIP Address: null");
            else if (!isShort)
            {
                sb2.Append("\nIP Address: " + conn.address + " ");
                RaClipboard.Send(sender, RaClipboard.RaClipBoardType.Ip, conn.address ?? "");
                sb2.Append(" <color=green><link=CP_IP>\uf0c5</link></color>");
            }
            else
                sb2.Append("\nIP Address: [REDACTED]");

            sb2.Append("\nUser ID: " + (!canViewUserId
                ? "<color=#D4AF37>INSUFFICIENT PERMISSIONS</color>"
                : (string.IsNullOrEmpty(ccm.UserId)
                    ? "(none)"
                    : (ccm.UserId + " <color=green><link=CP_USERID>\uf0c5</link></color>"))));

            if (canViewUserId)
            {
                RaClipboard.Send(sender, RaClipboard.RaClipBoardType.UserId, ccm.UserId ?? "");
                if (ccm.SaltedUserId != null && ccm.SaltedUserId.Contains("$"))
                    sb2.Append("\nSalted User ID: " + ccm.SaltedUserId);
                if (!string.IsNullOrEmpty(ccm.UserId2))
                    sb2.Append("\nUser ID 2: " + ccm.UserId2);
            }

            sb2.Append("\nServer role: " + sRoles.GetColoredRoleString());

            bool canViewHidden = CommandProcessor.CheckPermissions(sender, PlayerPermissions.ViewHiddenBadges);
            bool canViewGlobalHidden = CommandProcessor.CheckPermissions(sender, PlayerPermissions.ViewHiddenGlobalBadges);
            if (playerCommandSender != null && playerCommandSender.ServerRoles.Staff)
            {
                canViewHidden = true;
                canViewGlobalHidden = true;
            }

            bool hasHiddenBadge = !string.IsNullOrEmpty(sRoles.HiddenBadge);
            bool showHidden = !hasHiddenBadge || (sRoles.GlobalHidden && canViewGlobalHidden) || (!sRoles.GlobalHidden && canViewHidden);

            if (showHidden)
            {
                if (hasHiddenBadge)
                {
                    sb2.Append("\n<color=#DC143C>Hidden role: </color>" + sRoles.HiddenBadge);
                    sb2.Append("\n<color=#DC143C>Hidden role type: </color>" + (sRoles.GlobalHidden ? "GLOBAL" : "LOCAL"));
                }
                if (sRoles.RaEverywhere)
                    sb2.Append("\nStudio Status: <color=#BCC6CC>Studio GLOBAL Staff (management or global moderation)</color>");
                else if (sRoles.Staff)
                    sb2.Append("\nStudio Status: <color=#94B9CF>Studio Staff</color>");
            }

            int muteFlags = (int)VoiceChatMutes.GetFlags(target);
            if (muteFlags != 0)
            {
                sb2.Append("\nMUTE STATUS:");
                foreach (int val in Enum.GetValues(typeof(VcMuteFlags)))
                {
                    if (val != 0 && (muteFlags & val) == val)
                        sb2.Append(" <color=#F70D1A>" + (VcMuteFlags)val + "</color>");
                }
            }

            sb2.Append("\nActive flag(s):");
            if (ccm.GodMode)
                sb2.Append(" <color=#659EC7>[GOD MODE]</color>");

            var adminFlags = target.playerStats.GetModule<AdminFlagsStat>();
            if (adminFlags.HasFlag(AdminFlags.Noclip))
                sb2.Append(" <color=#DC143C>[NOCLIP ENABLED]</color>");
            else if (FpcNoclip.IsPermitted(target))
                sb2.Append(" <color=#E52B50>[NOCLIP UNLOCKED]</color>");

            if (sRoles.DoNotTrack)
                sb2.Append(" <color=#BFFF00>[DO NOT TRACK]</color>");
            if (sRoles.BypassMode)
                sb2.Append(" <color=#BFFF00>[BYPASS MODE]</color>");
            if (showHidden && sRoles.RemoteAdmin)
                sb2.Append(" <color=#43C6DB>[RA AUTHENTICATED]</color>");
            if (sRoles.IsInOverwatch)
                sb2.Append(" <color=#008080>[OVERWATCH MODE]</color>");

            if (hasGameplayData)
            {
                sb2.Append("\nClass: ").Append(
                    PlayerRoleLoader.AllRoles.TryGetValue(PlayerRolesUtils.GetRoleId(target), out var role)
                    ? role.RoleName : "None");

                sb2.Append(" <color=#fcff99>[HP: ")
                    .Append(CommandProcessor.GetRoundedStat<HealthStat>(target))
                    .Append("]</color>");

                sb2.Append(" <color=green>[AHP: ")
                    .Append(CommandProcessor.GetRoundedStat<AhpStat>(target))
                    .Append("]</color>");

                sb2.Append(" <color=#977dff>[HS: ")
                    .Append(CommandProcessor.GetRoundedStat<HumeShieldStat>(target))
                    .Append("]</color>");

                sb2.Append("\nPosition: ").Append(target.transform.position.ToPreciseString());
            }
            else
                sb2.Append("\n<color=#D4AF37>Some fields were hidden. GameplayData permission required.</color>");

            sb2.Append("</color>");

            sender.RaReply($"${DataId} {StringBuilderPool.Shared.ToStringReturn(sb2)}",
                success: true, logToConsole: true, string.Empty);

            RaPlayerQR.Send(sender, isBig: false,
                string.IsNullOrEmpty(ccm.UserId) ? "(no User ID)" : ccm.UserId);
        }

        public void ReceiveData(string data, bool secure)
        {
            SubmenuSelector.Singleton?.SelectedMenu?.SetResponse(true, data);
        }

        public static void Request(bool isShort, string playerIds)
        {
            if (ReferenceHub.TryGetLocalHub(out var localHub))
            {
                string query = string.Format("$1 {0} {1}", isShort ? 1 : 0, playerIds);
                localHub.queryProcessor.CmdSendQuery(query, false);
            }
        }
    }
}