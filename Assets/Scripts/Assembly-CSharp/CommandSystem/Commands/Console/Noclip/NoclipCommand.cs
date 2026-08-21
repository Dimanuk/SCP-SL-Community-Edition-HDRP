using System;

namespace CommandSystem.Commands.Console.Noclip
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class NoclipCommand : ICommand
    {
        public string Command { get; } = "noclip";

        public string[] Aliases { get; } = new[] { "togglenoclip", "n", "nc" };

        public string Description { get; } = "Toggle noclipping mode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            ReferenceHub localHub = ReferenceHub.LocalHub;
            if (localHub == null)
            {
                response = "You must join a server to execute this command.";
                return false;
            }

            localHub.queryProcessor.CmdSendQuery(Command, false);
            response = "Request sent to Remote Admin! Noclip key: " + new ReadableKeyCode(ActionName.Noclip);
            return true;
        }
    }
}
