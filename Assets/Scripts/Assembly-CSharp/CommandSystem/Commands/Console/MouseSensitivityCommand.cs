using System;
using UnityEngine;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class MouseSensitivityCommand : ICommand
    {
        public string Command { get; } = "mousesens";

        public string[] Aliases { get; } = new[] { "sens", "sensitivity" };

        public string Description { get; } = "Modify the mouse sensitivity.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 0)
            {
                if (!float.TryParse(arguments.Array[arguments.Offset], out float value))
                {
                    response = "Sensitivity must be an integer or decimal.";
                    return false;
                }

                PlayerPrefsSl.Set("Sens", value);
                SensitivitySettings.SensMultiplier = value;

                foreach (SensitivitySlider slider in UnityEngine.Object.FindObjectsOfType<SensitivitySlider>())
                {
                    slider.SetSliderValues(value, SensitivitySettings.AdsReductionMultiplier);
                }

                response = string.Format("New sensitivity saved! ({0})", SensitivitySettings.SensMultiplier);
                return true;
            }

            response = string.Format("The current sensitivity is: {0}", SensitivitySettings.SensMultiplier);
            return true;
        }
    }
}
