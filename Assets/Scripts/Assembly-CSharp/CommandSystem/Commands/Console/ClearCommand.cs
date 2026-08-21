using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ClearCommand : ICommand
    {
        public string Command { get; } = "clear";

        public string[] Aliases { get; } = new[] { "cls" };

        public string Description { get; } = "Clears the console.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            GameCore.Console.Logs.Clear();
            response = string.Empty;
            return true;
        }
    }
}
