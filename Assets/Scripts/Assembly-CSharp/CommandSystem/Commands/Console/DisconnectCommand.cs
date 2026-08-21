using System;
using Mirror;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class DisconnectCommand : ICommand
    {
        public string Command { get; } = "disconnect";

        public string[] Aliases { get; } = new[] { "dc" };

        public string Description { get; } = "Disconnect from server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!NetworkClient.active)
            {
                response = "You are not connected to a server!";
                return false;
            }

            CustomNetworkManager.triggerReconnectTime = 0f;
            CustomLiteNetLib4MirrorTransport.ClientChallengeState = ChallengeState.None;

            if (!NetworkServer.active)
            {
                NetworkManager.singleton.StopClient();
            }
            else
            {
                NetworkManager.singleton.StopHost();
            }

            response = "Disconnecting...";
            return true;
        }
    }
}
