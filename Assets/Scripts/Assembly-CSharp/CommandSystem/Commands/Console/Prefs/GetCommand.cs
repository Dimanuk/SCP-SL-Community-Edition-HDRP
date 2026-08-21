using System;

namespace CommandSystem.Commands.Console.Prefs
{
    public class GetCommand : ICommand, IUsageProvider
    {
        public string Command { get; } = "get";

        public string[] Aliases { get; }

        public string Description { get; } = "Displays the config key value";

        public string[] Usage { get; } = new[] { "Key" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count > 1)
            {
                if (Enum.TryParse(arguments.Array[arguments.Offset], out PlayerPrefsSl.DataType type))
                {
                    if (PlayerPrefsSl.TryGetKey(arguments.Array[arguments.Offset + 1], type, out string value))
                    {
                        response = string.Format("{0}: {1}", arguments.Array[arguments.Offset + 1], value);
                        return true;
                    }

                    response = string.Format("Key {0} not found!", arguments.Array[arguments.Offset + 1]);
                    return false;
                }

                response = "Invalid key type!";
                return false;
            }

            response = "\nUsage: " + this.DisplayCommandUsage();
            return false;
        }
    }
}
