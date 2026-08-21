using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class AuthRenewCommand : ICommand
    {
        public string Command { get; } = "authrenew";

        public string[] Aliases { get; } = new[] { "ar" };

        public string Description { get; } = "Renews your authentication session.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CentralAuthManager.ForceRenew = true;
            response = "Authentication renewal forced.";
            return true;
        }
    }
}
