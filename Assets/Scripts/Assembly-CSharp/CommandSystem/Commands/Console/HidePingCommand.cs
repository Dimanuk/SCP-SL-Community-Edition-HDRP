using System;
using Mirror;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class HidePingCommand : ICommand
    {
        public string Command { get; } = "hideping";

        public string[] Aliases { get; } = new[] { "hping" };

        public string Description { get; } = "Toggles showing your ping in the top left.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!NetworkClient.active)
            {
                response = "You are not connected to a server!";
                return false;
            }

            UnityEngine.GameObject pingObject = UserMainInterface.Singleton.Ping.gameObject;
            pingObject.SetActive(!pingObject.activeSelf);
            response = (UserMainInterface.Singleton.Ping.gameObject.activeSelf ? "Showing" : "Hiding") + " ping.";
            return true;
        }
    }
}
