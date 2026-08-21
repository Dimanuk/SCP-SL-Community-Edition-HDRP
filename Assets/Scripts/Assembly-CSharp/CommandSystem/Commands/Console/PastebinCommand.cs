using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PastebinCommand : ICommand
    {
        public string Command { get; } = "pastebin";

        public string[] Aliases { get; }

        public string Description { get; } = "Copies the server pastebin url.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (ReferenceHub.LocalHub == null)
            {
                response = "You must join a server to execute this command.";
                return false;
            }

            GameCore.Console.CopyPastebin();
            response = string.Empty;
            return true;
        }
    }
}
