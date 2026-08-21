using System;
using System.IO;
using System.Text;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class SyncCmdCommand : ICommand
    {
        public string Command { get; } = "synccmd";

        public string[] Aliases { get; }

        public string Description { get; } = "Sync keybinds between server and client.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (File.Exists(GameCore.Console.SyncbindPath))
            {
                File.Delete(GameCore.Console.SyncbindPath);
                response = "SyncServerCommandBinding has been disabled.";
                return true;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SyncServerCommandBinding can mess up your binds and even can be a security risk (eg. server can log pressed keys).");

            if (!GameCore.Console.BindSyncingEnabled)
            {
                stringBuilder.AppendLine("If you want to continue anyway, start game with \"-allow-syncbind\" argument.");
                response = stringBuilder.ToString();
                return true;
            }

            if (!GameCore.Console._bindSyncingContinue)
            {
                GameCore.Console._bindSyncingContinue = true;
                stringBuilder.AppendLine("<color=yellow>[WARNING] Your command binding might be messed up, and your key might be logged by the server you join.</color>");
                stringBuilder.AppendLine("<color=yellow>If you really want to enable command binding syncing with server, please run this command again.</color>");
                response = stringBuilder.ToString();
                return true;
            }

            using (StreamWriter streamWriter = new(GameCore.Console.SyncbindPath))
            {
                streamWriter.Write(string.Empty);
            }

            stringBuilder.AppendLine("SyncServerCommandBinding has been enabled.");
            response = stringBuilder.ToString();
            return true;
        }
    }
}
