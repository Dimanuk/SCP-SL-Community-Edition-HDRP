using System;

namespace CommandSystem.Commands.Console.Prefs
{
    public class ReloadCommand : ICommand
    {
        public string Command { get; } = "reload";

        public string[] Aliases { get; } = new[] { "refresh" };

        public string Description { get; } = "Reloads the settings file";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            PlayerPrefsSl.Refresh();
            response = "Reloaded all settings!";
            return true;
        }
    }
}
