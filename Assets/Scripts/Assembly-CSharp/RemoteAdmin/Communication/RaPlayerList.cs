using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameCore;
using NorthwoodLib.Pools;
using PlayerRoles;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace RemoteAdmin.Communication
{
    public class RaPlayerList : IServerCommunication, IClientCommunication
    {
        public enum PlayerSorting
        {
            Ids = 0,
            Alphabetical = 1,
            Class = 2,
            Team = 3
        }

        private const string OverwatchBadge = "<link=RA_OverwatchEnabled><color=white>[</color><color=#03f8fc>\uf06e</color><color=white>]</color></link>";

        public int DataId => 0;

        // === SERVER-SIDE ===
        public void ReceiveData(CommandSender sender, string data)
        {
            string[] array = data.Split(' ');
            if (array.Length != 3
                || !int.TryParse(array[0], out int result)
                || !int.TryParse(array[1], out int result2)
                || !Enum.IsDefined(typeof(PlayerSorting), result2))
            {
                return;
            }

            bool isSilent = result == 1;
            bool isDescending = array[2].Equals("1");
            PlayerSorting sortingType = (PlayerSorting)result2;

            bool viewHiddenBadges = CommandProcessor.CheckPermissions(sender, PlayerPermissions.ViewHiddenBadges);
            bool viewHiddenGlobalBadges = CommandProcessor.CheckPermissions(sender, PlayerPermissions.ViewHiddenGlobalBadges);

            if (sender is PlayerCommandSender playerCommandSender && playerCommandSender.ServerRoles.Staff)
            {
                viewHiddenBadges = true;
                viewHiddenGlobalBadges = true;
            }

            StringBuilder sb = StringBuilderPool.Shared.Rent("\n");

            foreach (ReferenceHub hub in isDescending ? SortPlayersDescending(sortingType) : SortPlayers(sortingType))
            {
                if (hub.Mode != ClientInstanceMode.DedicatedServer && hub.Mode != ClientInstanceMode.Unverified)
                {
                    bool isOverwatch = hub.serverRoles.IsInOverwatch;

                    sb.Append(GetPrefix(hub, viewHiddenBadges, viewHiddenGlobalBadges));
                    sb.Append(isOverwatch ? OverwatchBadge + " " : string.Empty);
                    sb.Append("<color={RA_ClassColor}>(").Append(hub.PlayerId).Append(") ");
                    string combinedName = hub.nicknameSync.CombinedName ?? string.Empty;
                    sb.Append(combinedName.Replace("\n", string.Empty).Replace("RA_", string.Empty)).Append("</color>");
                    sb.AppendLine();
                }
            }

            sender.RaReply($"${DataId} {StringBuilderPool.Shared.ToStringReturn(sb)}",
                success: true,
                logToConsole: !isSilent,
                string.Empty);
        }

        private IEnumerable<ReferenceHub> SortPlayers(PlayerSorting sortingType)
        {
            return sortingType switch
            {
                PlayerSorting.Team => ReferenceHub.AllHubs.OrderBy(h => h.roleManager.CurrentRole.Team),
                PlayerSorting.Alphabetical => ReferenceHub.AllHubs.OrderBy(h => h.nicknameSync.DisplayName ?? h.nicknameSync.MyNick),
                _ => ReferenceHub.AllHubs.OrderBy(h => h.PlayerId),
            };
        }

        private IEnumerable<ReferenceHub> SortPlayersDescending(PlayerSorting sortingType)
        {
            return sortingType switch
            {
                PlayerSorting.Team => ReferenceHub.AllHubs.OrderByDescending(h => h.roleManager.CurrentRole.Team),
                PlayerSorting.Alphabetical => ReferenceHub.AllHubs.OrderByDescending(h => h.nicknameSync.DisplayName ?? h.nicknameSync.MyNick),
                _ => ReferenceHub.AllHubs.OrderByDescending(h => h.PlayerId),
            };
        }

        private string GetPrefix(ReferenceHub hub, bool viewHiddenBadges = false, bool viewHiddenGlobalBadges = false)
        {
            ServerRoles serverRoles = hub.serverRoles;

            if (!string.IsNullOrEmpty(serverRoles.HiddenBadge)
                && (!serverRoles.GlobalHidden || !viewHiddenBadges)
                && (serverRoles.GlobalHidden || !viewHiddenGlobalBadges))
            {
                return string.Empty;
            }

            if (serverRoles.RaEverywhere)
            {
                return "<link=RA_RaEverywhere><color=white>[<color=#EFC01A>\uf3ed</color><color=white>]</color></link> ";
            }
            if (serverRoles.Staff)
            {
                return "<link=RA_StudioStaff><color=white>[<color=#005EBC>\uf0ad</color><color=white>]</color></link> ";
            }
            if (serverRoles.RemoteAdmin)
            {
                return "<link=RA_Admin><color=white>[\uf406]</color></link> ";
            }
            return string.Empty;
        }

        public void ReceiveData(string data, bool secure)
        {
            if (PlayerRequest.Singleton != null && ReferenceHub.TryGetLocalHub(out var localHub))
            {
                PlayerRequest.Singleton.ResponsePlayerList(data, true, localHub.queryProcessor.GameplayData);
            }
        }

        public static void Request(bool isSilent, PlayerSorting sortingType = PlayerSorting.Ids, bool isDescending = false)
        {
            if (ReferenceHub.TryGetLocalHub(out var localHub))
            {
                string query = string.Format("$0 {0} {1} {2}",
                    isSilent ? 1 : 0,
                    (int)sortingType,
                    isDescending ? 1 : 0);

                localHub.queryProcessor.CmdSendQuery(query, false);
            }
        }
    }
}