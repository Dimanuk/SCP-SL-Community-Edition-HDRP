using System;

namespace CommandSystem.Commands.Console.Prefs
{
    public class SetCommand : ICommand, IUsageProvider
    {
        public string Command { get; } = "set";

        public string[] Aliases { get; }

        public string Description { get; } = "Sets the config key value";

        public string[] Usage { get; } = new[] { "Key", "Value" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count > 2)
            {
                if (Enum.TryParse(arguments.Array[arguments.Offset], out PlayerPrefsSl.DataType type))
                {
                    PlayerPrefsSl.SetKey(arguments.Array[arguments.Offset + 1], type, arguments.Array[arguments.Offset + 2]);
                    response = string.Format("Set {0} to {1}", arguments.Array[arguments.Offset + 1], arguments.Array[arguments.Offset + 2]);
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
