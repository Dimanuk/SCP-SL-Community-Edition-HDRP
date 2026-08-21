using System;

namespace CommandSystem.Commands.Console.Prefs
{
    public class ClearCommand : ICommand
    {
        public string Command { get; } = "clear";

        public string[] Aliases { get; } = new[] { "reset" };

        public string Description { get; } = "Clears game settings";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            PlayerPrefsSl.DeleteAll();
            response = "Cleared all settings!";
            return true;
        }
    }
}
