using System;
using UnityEngine;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class KeybindCommand : ICommand
    {
        public string Command { get; } = "keybind";

        public string[] Aliases { get; }

        public string Description { get; } = "Bind key to execute console command.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "SYNTAX: \"keybind <key/keycode> <action>\"";
                return false;
            }

            string keyStr = arguments.Array[arguments.Offset].ToUpperInvariant();
            if (!Enum.TryParse(keyStr, out KeyCode key) && !NewInput.TryParseKeycode(keyStr, out key))
            {
                response = "Invalid key code: " + keyStr;
                return false;
            }

            if (NewInput.TryParseActionName(arguments.Array[arguments.Offset + 1], out ActionName action))
            {
                NewInput.SetKey(action, key);
                response = string.Format("Action [{0}] has been bound to [{1}]!", action, key);
                return true;
            }

            response = "Invalid action code: " + arguments.Array[arguments.Offset + 1];
            return false;
        }
    }
}
