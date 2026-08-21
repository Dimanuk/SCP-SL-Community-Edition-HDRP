using System;
using System.Text;
using UnityEngine;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ColorsCommand : ICommand
    {
        public string Command { get; } = "colors";

        public string[] Aliases { get; } = new[] { "color" };

        public string Description { get; } = "Displays a list of available colors.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (ReferenceHub.LocalHub == null)
            {
                response = "You must join a server to execute this command.";
                return false;
            }

            StringBuilder stringBuilder = new StringBuilder("<color=" + Misc.ToHex(Color.white) + ">Available colors:</color>\n");
            foreach (ServerRoles.NamedColor namedColor in ReferenceHub.LocalHub.serverRoles.NamedColors)
            {
                stringBuilder.Append("<color=#" + namedColor.ColorHex + ">" + namedColor.Name + " - #" + namedColor.ColorHex + (namedColor.Restricted ? " (Restricted)" : string.Empty) + "</color>\n");
            }

            response = stringBuilder.ToString();
            return true;
        }
    }
}
