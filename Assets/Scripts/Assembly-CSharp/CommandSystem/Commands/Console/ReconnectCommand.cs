using System;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ReconnectCommand : ICommand
    {
        public string Command { get; } = "reconnect";

        public string[] Aliases { get; } = new[] { "rc" };

        public string Description { get; } = "Reconnect to last played server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!NetworkClient.active)
            {
                NewMainMenu newMainMenu = UnityEngine.Object.FindAnyObjectByType<NewMainMenu>();
                if (newMainMenu != null)
                {
                    if (string.IsNullOrEmpty(CustomNetworkManager.ConnectionIp))
                    {
                        response = "Connect to a server before you use this command!";
                        return false;
                    }

                    newMainMenu.Connect(CustomNetworkManager.ConnectionIp + ":" + LiteNetLib4MirrorTransport.Singleton.port, false);
                    response = "Reconnecting to " + CustomNetworkManager.ConnectionIp + ":" + LiteNetLib4MirrorTransport.Singleton.port;
                    return true;
                }

                MainMenuScript mainMenuScript = UnityEngine.Object.FindAnyObjectByType<MainMenuScript>();
                if (mainMenuScript != null)
                {
                    if (string.IsNullOrEmpty(CustomNetworkManager.ConnectionIp))
                    {
                        response = "Connect to a server before you use this command!";
                        return false;
                    }

                    mainMenuScript.Connect(CustomNetworkManager.ConnectionIp + ":" + LiteNetLib4MirrorTransport.Singleton.port, false);
                    response = "Reconnecting to " + CustomNetworkManager.ConnectionIp + ":" + LiteNetLib4MirrorTransport.Singleton.port;
                    return true;
                }
            }

            response = "Already connected to a server!";
            return false;
        }
    }
}
