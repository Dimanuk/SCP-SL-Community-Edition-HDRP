using System;
using System.IO;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ConfirmCommand : ICommand
    {
        public string Command { get; } = "confirm";

        public string[] Aliases { get; }

        public string Description { get; } = "Confirms global ban issuance.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (File.Exists(FileManager.GetAppFolder() + "StaffAPI.txt"))
            {
                if (ReferenceHub.LocalHub == null)
                {
                    response = "You must join a server to execute this command.";
                    return false;
                }

                global::RemoteAdmin.Communication.RaGlobalBan.ConfirmGlobalBanning();
                response = string.Empty;
                return true;
            }

            response = "You are not authorized to confirm global bans!";
            return false;
        }
    }
}
