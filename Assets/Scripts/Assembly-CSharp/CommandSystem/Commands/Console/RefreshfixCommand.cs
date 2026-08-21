using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class RefreshfixCommand : ICommand
    {
        public string Command { get; } = "refreshfix";

        public string[] Aliases { get; }

        public string Description { get; } = "Change console refresh mode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            GameCore.Console._alwaysRefreshing = !GameCore.Console._alwaysRefreshing;
            response = "Console log refresh mode set to: " + (GameCore.Console._alwaysRefreshing ? "OPTIMIZED" : "FIXED");
            return true;
        }
    }
}
