using System;
using System.Globalization;

namespace CommandSystem.Commands.Console.Volume
{
    public class MasterCommand : ICommand
    {
        public string Command { get; } = "master";

        public string[] Aliases { get; }

        public string Description { get; } = "Modifies the volume for master.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string raw = arguments.Count >= 1 ? arguments.Array[arguments.Offset] : string.Empty;
            char.TryParse(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, out char decimalSeparator);
            string normalized = raw.Replace(decimalSeparator, '.');

            if (float.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
            {
                foreach (GameSettings.GameSettingsAudioSlider slider in GameSettings.singleton.AudioSliders)
                {
                    if (slider.Key == "Master")
                    {
                        slider.SliderReference.value = value;
                        response = string.Format("Set master volume to {0}", value);
                        return true;
                    }
                }

                response = "Unable to set master volume.";
                return false;
            }

            response = "SYNTAX: volume (master, voice or effect) 0 to 1.0.\n         Example: \"volume master 0.5\" makes master volume 50%.";
            return false;
        }
    }
}
