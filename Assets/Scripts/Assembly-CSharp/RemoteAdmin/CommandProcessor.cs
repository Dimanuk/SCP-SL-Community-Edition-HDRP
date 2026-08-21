using System.Collections.Generic;
using CommandSystem;
using PlayerStatsSystem;

namespace RemoteAdmin
{
	public static class CommandProcessor
	{
        public static readonly global::CommandSystem.RemoteAdminCommandHandler RemoteAdminCommandHandler = global::CommandSystem.RemoteAdminCommandHandler.Create();

        internal static string ProcessQuery(string q, CommandSender sender)
        {
            if (q.StartsWith("$", global::System.StringComparison.Ordinal))
            {
                string[] array = q.Remove(0, 1).Split(' ');
                if (array.Length <= 1)
                {
                    return null;
                }
                if (!int.TryParse(array[0], out var result))
                {
                    return null;
                }
                if (global::RemoteAdmin.Communication.CommunicationProcessor.ServerCommunication.TryGetValue(result, out var value))
                {
                    value.ReceiveData(sender, string.Join(" ", global::System.Linq.Enumerable.Skip(array, 1)));
                }
                return null;
            }
            global::RemoteAdmin.PlayerCommandSender playerCommandSender = sender as global::RemoteAdmin.PlayerCommandSender;
            if (q.StartsWith("@", global::System.StringComparison.Ordinal))
            {
                if (!CheckPermissions(sender, "Admin Chat", PlayerPermissions.AdminChat, string.Empty))
                {
                    playerCommandSender?.ReferenceHub.queryProcessor.TargetAdminChatAccessDenied(playerCommandSender.ReferenceHub.queryProcessor.connectionToClient);
                    return "You don't have permissions to access Admin Chat!";
                }
                q = q + " ~" + sender.Nickname;
                foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
                {
                    if ((allHub.serverRoles.AdminChatPerms || allHub.serverRoles.RaEverywhere) && allHub.Mode != ClientInstanceMode.Unverified)
                    {
                        allHub.queryProcessor.TargetReply(allHub.queryProcessor.connectionToClient, q, isSuccess: true, logInConsole: false, string.Empty);
                    }
                }
                return null;
            }
            string[] array2 = q.Trim().Split(global::RemoteAdmin.QueryProcessor.SpaceArray, 512, global::System.StringSplitOptions.RemoveEmptyEntries);
            if (RemoteAdminCommandHandler.TryGetCommand(array2[0], out var command))
            {
                try
                {
                    string response;
                    bool flag = command.Execute(array2.Segment(1), sender, out response);
                    if (!string.IsNullOrEmpty(response))
                    {
                        sender.RaReply(array2[0].ToUpperInvariant() + "#" + response, flag, logToConsole: true, "");
                    }
                    return response;
                }
                catch (global::System.Exception ex)
                {
                    string text = "Command execution failed! Error: " + Misc.RemoveStacktraceZeroes(ex.ToString());
                    sender.RaReply(text, success: false, logToConsole: true, array2[0].ToUpperInvariant() + "#" + text);
                    return text;
                }
            }
            sender.RaReply("SYSTEM#Unknown command!", success: false, logToConsole: true, string.Empty);
            return "Unknown command!";
        }
        internal static float GetRoundedStat<T>(ReferenceHub hub) where T : global::PlayerStatsSystem.StatBase
        {
            return global::UnityEngine.Mathf.Round(hub.playerStats.GetModule<T>().CurValue * 100f) / 100f;
        }

        internal static global::System.Collections.Generic.List<global::CommandSystem.ICommand> GetAllCommands()
        {
            return global::System.Linq.Enumerable.ToList(RemoteAdminCommandHandler.AllCommands);
        }

        internal static bool CheckPermissions(CommandSender sender, string queryZero, PlayerPermissions perm, string replyScreen = "", bool reply = true)
        {
            if (ServerStatic.IsDedicated && sender.FullPermissions)
            {
                return true;
            }
            if (PermissionsHandler.IsPermitted(sender.Permissions, perm))
            {
                return true;
            }
            if (reply)
            {
                sender.RaReply(queryZero + "#You don't have permissions to execute this command.\nRequired permission: " + perm, success: false, logToConsole: true, replyScreen);
            }
            return false;
        }

        internal static bool CheckPermissions(CommandSender sender, PlayerPermissions perm)
        {
            if (ServerStatic.IsDedicated && sender.FullPermissions)
            {
                return true;
            }
            if (PermissionsHandler.IsPermitted(sender.Permissions, perm))
            {
                return true;
            }
            return false;
        }
	}
}
