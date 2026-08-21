using System;

namespace CommandSystem.Commands.Console.Prefs
{
    public class DeleteCommand : ICommand, IUsageProvider
    {
        public string Command { get; } = "delete";

        public string[] Aliases { get; } = new[] { "remove" };

        public string Description { get; } = "Removes a settings key";

        public string[] Usage { get; } = new[] { "Key" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count > 1)
            {
                if (Enum.TryParse(arguments.Array[arguments.Offset], out PlayerPrefsSl.DataType type))
                {
                    PlayerPrefsSl.DeleteKey(arguments.Array[arguments.Offset + 1], type);
                    response = string.Format("Removed key {0}", arguments.Array[arguments.Offset + 1]);
                    return true;
                }

                response = "Invalid key type!";
                return false;
            }

            response = "\nUsage: " + this.DisplayCommandUsage();
            return false;
        }
    }
}
