using System;
using System.Globalization;
using PlayerRoles.FirstPersonControl;

namespace CommandSystem.Commands.Console.Noclip
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class NoclipSpeedCommand : ICommand
    {
        public string Command { get; } = "noclipspeed";

        public string[] Aliases { get; } = new[] { "nspeed", "ncs", "ns" };

        public string Description { get; } = "Set noclipping speed.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = string.Format("Current noclip speed is {0}.", Math.Round(FpcNoclip.CurSpeed, 3));
                return true;
            }

            if (arguments.Count == 1)
            {
                if (float.TryParse(arguments.Array[arguments.Offset], NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                {
                    FpcNoclip.CurSpeed = result;
                    response = string.Format("Noclip speed set to {0}.", result);
                    return true;
                }

                response = "New speed must be a float value";
                return false;
            }

            response = "Syntax: noclipspeed [new speed]";
            return false;
        }
    }
}
