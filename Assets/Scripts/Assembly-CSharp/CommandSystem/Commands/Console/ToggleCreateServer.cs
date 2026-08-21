using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ToggleCreateServer : ICommand
    {
        public string Command { get; } = "cst";

        public string[] Aliases { get; } = new[] { "tcs" };

        public string Description { get; } = "Toggles force create server button visibility";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            GameCore.Console.DisplayCreateServer = !GameCore.Console.DisplayCreateServer;
            response = "Toggled \"Create Server\" visibility.";
            return true;
        }
    }
}
