using System;
using System.Text;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ReportCommand : ICommand
    {
        public string Command { get; } = "report";

        public string[] Aliases { get; }

        public string Description { get; } = "Report a specified player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub _))
            {
                response = "You must join a server to execute this command.";
                return false;
            }

            if (arguments.Count < 3)
            {
                response = "Invalid player id specified.\nSyntax: \"report <playerid> <is for cheating: 0/1> <reason>\"";
                return false;
            }

            StringBuilder reason = new StringBuilder(arguments.Array[arguments.Offset + 2]);
            for (int i = 3; i < arguments.Count; i++)
            {
                reason.Append(" " + arguments.Array[arguments.Offset + i]);
            }

            if (!int.TryParse(arguments.Array[arguments.Offset], out int playerId) || !ReferenceHub.TryGetHub(playerId, out ReferenceHub target))
            {
                response = "Invalid player id specified.\nSyntax: \"report <playerid> <is for cheating: 0/1> <reason>\"";
                return false;
            }

            target.GetComponent<CheaterReport>().Report(target.netId, reason.ToString(), arguments.Array[arguments.Offset + 1] == "1");
            response = string.Empty;
            return true;
        }
    }
}
