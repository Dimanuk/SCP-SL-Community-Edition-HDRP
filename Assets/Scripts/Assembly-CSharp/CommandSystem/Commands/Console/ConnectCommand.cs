using System;
using UnityEngine;
using Mirror;
using CommandSystem;

namespace CommandSystem.Commands.Console
{
    [global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
    public class ConnectCommand : ICommand
    {
        public string Command { get; } = "connect";
        public string[] Aliases { get; } = new[] { "join", "cn" };
        public string Description { get; } = "Connect to the specified server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (ServerStatic.IsDedicated)
            {
                response = "Dedicated server cannot connect to another server...";
                return false;
            }

            if (arguments.Count < 1 || arguments.Count > 2)
            {
                response = "Syntax: \"connect [-f] IP\" or \"connect [-f] IP:Port\", for IPv6 with port: \"connect [-f] [IP]:port\"\nAdd \"-f\" to skip IP validation.";
                return false;
            }

            NewMainMenu menu = UnityEngine.Object.FindFirstObjectByType<NewMainMenu>();
            if (menu == null)
            {
                response = "Connection menu not found.";
                return false;
            }

            if (NetworkClient.isConnected)
            {
                response = "Already connected to a server!";
                return false;
            }

            bool skipValidation = false;
            string targetAddress;

            if (arguments.Count == 2)
            {
                string flag = arguments.Array[arguments.Offset];
                if (!flag.Equals("-f", StringComparison.OrdinalIgnoreCase))
                {
                    response = "Syntax: \"connect [-f] IP\" or \"connect [-f] IP:Port\", for IPv6 with port: \"connect [-f] [IP]:port\"\nAdd \"-f\" to skip IP validation.";
                    return false;
                }
                skipValidation = true;
                targetAddress = arguments.Array[arguments.Offset + 1];
            }
            else
            {
                targetAddress = arguments.Array[arguments.Offset];
            }

            menu.Connect(targetAddress, skipValidation);
            FavoriteAndHistory.ResetServerID();

            response = string.Empty;
            return true;
        }
    }
}