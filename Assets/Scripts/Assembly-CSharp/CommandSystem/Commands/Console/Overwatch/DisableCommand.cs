using System;

namespace CommandSystem.Commands.Console.Overwatch
{
    public class DisableCommand : ICommand
    {
        public string Command { get; } = "disable";

        public string[] Aliases { get; } = new[] { "off", "false", "0" };

        public string Description { get; } = "Disables overwatch.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (ReferenceHub.LocalHub == null)
            {
                response = "You must join a server to execute this command.";
                return false;
            }

            ReferenceHub.LocalHub.serverRoles.CmdSetOverwatchStatus(0);
            response = string.Empty;
            return true;
        }
    }
}
