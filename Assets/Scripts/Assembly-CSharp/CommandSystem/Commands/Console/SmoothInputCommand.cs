using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class SmoothInputCommand : ICommand
    {
        public string Command { get; } = "smoothinput";

        public string[] Aliases { get; } = new[] { "smooth" };

        public string Description { get; } = "Enables or disables the usage of smooth input for sensitivity.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 0)
            {
                if (!bool.TryParse(arguments.Array[arguments.Offset], out bool value))
                {
                    response = "Smooth input must be a boolean. Example: (smooth true) or (smooth false).";
                    return false;
                }

                PlayerPrefsSl.Set("SmoothInput", value);
                SensitivitySettings.SmoothInput = value;
                response = string.Format("New smooth input value saved! ({0})", value);
                return true;
            }

            bool toggled = !PlayerPrefsSl.Get("SmoothInput", false);
            PlayerPrefsSl.Set("SmoothInput", toggled);
            SensitivitySettings.SmoothInput = toggled;
            response = string.Format("Smooth input toggled to: {0}!", toggled);
            return true;
        }
    }
}
