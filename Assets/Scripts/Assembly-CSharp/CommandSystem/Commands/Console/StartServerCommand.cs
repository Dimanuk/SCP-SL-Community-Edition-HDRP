using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class StartServerCommand : ICommand
    {
        public string Command { get; } = "host";

        public string[] Aliases { get; } = new[] { "startserver", "starthost" };

        public string Description { get; } = "Starts a non-dedicated server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CustomNetworkManager.StartNondedicated(true);
            response = "Started a non-dedicated server.";
            return true;
        }
    }
}
