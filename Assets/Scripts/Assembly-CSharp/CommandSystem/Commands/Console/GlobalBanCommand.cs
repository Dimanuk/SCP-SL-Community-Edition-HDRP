using System;
using System.IO;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class GlobalBanCommand : ICommand
    {
        public string Command { get; } = "globalban";

        public string[] Aliases { get; } = new[] { "gban", "superban" };

        public string Description { get; } = "Issues global ban.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!File.Exists(FileManager.GetAppFolder() + "StaffAPI.txt"))
            {
                response = "You are not authorized to issue global bans!";
                return false;
            }

            if (ReferenceHub.LocalHub == null)
            {
                response = "You must join a server to execute this command.";
                return false;
            }

            if (arguments.Count < 2 ||
                (!arguments.Array[arguments.Offset].Equals("id", StringComparison.OrdinalIgnoreCase) &&
                 !arguments.Array[arguments.Offset].Equals("userid", StringComparison.OrdinalIgnoreCase)))
            {
                response = "SYNTAX: globalban <selector: id OR userid> <player to ban>";
                return false;
            }

            string key = arguments.Array[arguments.Offset + 1].ToUpperInvariant();
            int keytype = arguments.Array[arguments.Offset].Equals("id", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            global::RemoteAdmin.Communication.RaGlobalBan.Request(key, keytype);
            response = "Requesting global ban issuance...";
            return true;
        }
    }
}
