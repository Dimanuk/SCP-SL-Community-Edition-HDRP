using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class AuthTokenCommand : ICommand, IHiddenCommand
    {
        public string Command { get; } = "authtoken";

        public string[] Aliases { get; } = new[] { "rqsgn" };

        public string Description { get; } = "Obtains an new authentication token.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CentralAuthManager.RequestAuthToken = true;
            response = "Obtaining authentication token forced.";
            return true;
        }
    }
}
