using System;
using UnityEngine;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class MeasureCommand : ICommand
    {
        private static Vector3 _pos;

        public string Command { get; } = "measure";

        public string[] Aliases { get; } = new[] { "ms" };

        public string Description { get; } = "Measures a distance between two positions.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (ReferenceHub.LocalHub == null)
            {
                response = "You must join any server to use this command.";
                return false;
            }

            if (_pos != Vector3.zero)
            {
                Vector3 diff = ReferenceHub.LocalHub.transform.position - _pos;
                response = string.Format("--- Measurement results ---\nDistance: {0}\n\nX Diff: {1}\nY Diff: {2}\nZ Diff: {3}", diff.magnitude, diff.x, diff.y, diff.z);
                _pos = Vector3.zero;
                return true;
            }

            _pos = ReferenceHub.LocalHub.transform.position;
            response = "Position saved.";
            return true;
        }
    }
}
