using System;

namespace CommandSystem.Commands.Console.Central
{
    public class RandomCommand : ICommand
    {
        public string Command { get; } = "random";

        public string[] Aliases { get; } = new[] { "-r" };

        public string Description { get; } = "Switches to random central server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CentralServer.ChangeCentralServer(false);
            response = "--- Central server changed ---";
            return true;
        }
    }
}
