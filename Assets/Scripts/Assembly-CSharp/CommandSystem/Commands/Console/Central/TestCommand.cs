using System;

namespace CommandSystem.Commands.Console.Central
{
    public class TestCommand : ICommand
    {
        public string Command { get; } = "test";

        public string[] Aliases { get; } = new[] { "-t" };

        public string Description { get; } = "Switches to TEST central server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CentralServer.SelectedServer = "TEST";
            CentralServer.StandardUrl = "https://test.scpslgame.com/";
            CentralServer.MasterUrl = "https://test.scpslgame.com/";
            CentralServer.TestServer = true;
            CentralServer.ServerSelected = true;
            response = "--- Central server changed to TEST SERVER ---";
            return true;
        }
    }
}
