using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class AdminMeCommand : ICommand
    {
        public string Command { get; } = "adminme";

        public string[] Aliases { get; } = new[] { "override" };

        public string Description { get; } = "Grants you remote admin privileges on a local server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (ReferenceHub.TryGetHostHub(out ReferenceHub hub) && hub.isLocalPlayer)
            {
                if (!string.IsNullOrEmpty(hub.characterClassManager.UserId))
                {
                    ServerLogs.AddLog(ServerLogs.Modules.Permissions, sender.LogName + " assigned all local permissions to themselves using the " + Command + " command.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                    hub.serverRoles.RemoteAdmin = true;
                    hub.serverRoles.LocalRemoteAdmin = true;
                    hub.serverRoles.Permissions = ServerStatic.PermissionsHandler.FullPerm;
                    hub.queryProcessor.GameplayData = PermissionsHandler.IsPermitted(hub.serverRoles.Permissions, PlayerPermissions.GameplayData);
                    hub.serverRoles.RaEverywhere = true;
                    hub.serverRoles.OpenRemoteAdmin(false);
                    response = "Remote admin privileges has been granted to you.";
                    return true;
                }

                response = "Authentication was not performed. Is the server running in online mode?";
                return false;
            }

            response = "You are not connected to a local server.";
            return false;
        }
    }
}
