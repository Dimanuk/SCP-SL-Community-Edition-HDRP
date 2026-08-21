using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class RawInputCommand : ICommand
    {
        public string Command { get; } = "rawinput";

        public string[] Aliases { get; }

        public string Description { get; } = "Enables or disables the usage of raw input for sensitivity.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 0)
            {
                if (!bool.TryParse(arguments.Array[arguments.Offset], out bool value))
                {
                    response = "Raw input must be a boolean.";
                    return false;
                }

                PlayerPrefsSl.Set("RawInput", value);
                SensitivitySettings.RawInput = value;
            }

            response = string.Format("The current raw input is: {0}", SensitivitySettings.RawInput);
            return true;
        }
    }
}
