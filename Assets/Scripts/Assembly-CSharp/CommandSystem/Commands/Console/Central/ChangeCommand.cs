using System;
using System.Linq;

namespace CommandSystem.Commands.Console.Central
{
    public class ChangeCommand : ICommand
    {
        public string Command { get; } = "change";

        public string[] Aliases { get; } = new[] { "-s" };

        public string Description { get; } = "Switches to specific central server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "SYNTAX: central <-fs / -s> <server>";
                return false;
            }

            string server = arguments.Array[arguments.Offset].ToUpperInvariant();
            if (CentralServer.Servers.Contains(server))
            {
                CentralServer.StandardUrl = "https://" + server.ToLowerInvariant() + ".scpslgame.com/";
                CentralServer.SelectedServer = server;
                CentralServer.TestServer = false;
                CentralServer.ServerSelected = true;
                response = "--- Central server changed to " + server + " ---";
                return true;
            }

            response = "Server " + server + " is not a central server. Use " + Command.ToUpperInvariant() + " -fs " + server + " to force the change.";
            return false;
        }
    }
}
