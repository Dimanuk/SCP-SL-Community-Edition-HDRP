using System;
using Mirror;
using UnityEngine;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class LocalhostCommand : ICommand
    {
        public string Command { get; } = "localhost";

        public string[] Aliases { get; } = new[] { "lh", "127.0.0.1", "127" };

        public string Description { get; } = "Connect to local server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!NetworkClient.active)
            {
                NewMainMenu menu = UnityEngine.Object.FindFirstObjectByType<NewMainMenu>();
                if (menu != null)
                {
                    menu.Connect("127.0.0.1", true);
                    FavoriteAndHistory.ResetServerID();
                    response = string.Empty;
                    return true;
                }
            }

            response = "Already connected to a server!";
            return false;
        }
    }
}
