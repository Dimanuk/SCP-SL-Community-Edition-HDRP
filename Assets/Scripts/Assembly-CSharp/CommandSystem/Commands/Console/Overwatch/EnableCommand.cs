using System;

namespace CommandSystem.Commands.Console.Overwatch
{
    public class EnableCommand : ICommand
    {
        public string Command { get; } = "enable";

        public string[] Aliases { get; } = new[] { "on", "true", "1" };

        public string Description { get; } = "Enables overwatch.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.Overwatch, out response))
            {
                return false;
            }

            if (ReferenceHub.LocalHub == null)
            {
                response = "You must join a server to execute this command.";
                return false;
            }

            ReferenceHub.LocalHub.serverRoles.CmdSetOverwatchStatus(1);
            response = string.Empty;
            return true;
        }
    }
}
