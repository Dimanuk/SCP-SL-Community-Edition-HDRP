using System;
using System.Collections.Generic;
using System.Text;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class SkinsCommand : ICommand
    {
        public string Command { get; } = "skins";

        public string[] Aliases { get; }

        public string Description { get; } = "Displays a list of your skins.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (ReferenceHub.LocalHub == null)
            {
                response = "You must join a server to execute this command.";
                return false;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<size=25>Your Skins:</size>");
            foreach (KeyValuePair<string, int> skin in ReferenceHub.LocalHub.serverRoles.PlayerSkins)
            {
                stringBuilder.AppendLine(string.Format("- {0}: {1}", skin.Key, skin.Value));
            }

            response = stringBuilder.ToString();
            return true;
        }
    }
}
