using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ClearBroadcastCommand : ICommand
    {
        public string Command { get; } = "clearbroadcast";

        public string[] Aliases { get; } = new[] { "broadcastclear", "clearbc", "bcclear", "alertclear", "clearalert" };

        public string Description { get; } = "Clears the active broadcast on screen.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Broadcast.Messages.Clear();
            response = "All broadcasts locally cleared.";
            return true;
        }
    }
}
