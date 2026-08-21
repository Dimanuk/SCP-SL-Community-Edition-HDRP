using System;

namespace CommandSystem.Commands.Console.Central
{
    public class ForceChangeCommand : ICommand
    {
        public string Command { get; } = "forcechange";

        public string[] Aliases { get; } = new[] { "-fs" };

        public string Description { get; } = "Force switches to specific central server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "SYNTAX: central <-fs / -s> <server>";
                return false;
            }

            string server = arguments.Array[arguments.Offset].ToUpperInvariant();
            string url = "https://" + server.ToLowerInvariant() + ".scpslgame.com/";
            CentralServer.SelectedServer = server;
            CentralServer.StandardUrl = url;
            CentralServer.MasterUrl = url;
            CentralServer.TestServer = false;
            CentralServer.ServerSelected = true;
            response = "--- Central server force changed to " + server + " ---";
            return true;
        }
    }
}
