using System;
using System.Text;
using UnityEngine;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class CmdbindCommand : ICommand
    {
        public string Command { get; } = "cmdbind";

        public string[] Aliases { get; }

        public string Description { get; } = "Bind a key to a specific command.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 0)
            {
                string keyStr = arguments.Array[arguments.Offset].ToUpperInvariant();
                if (!Enum.TryParse(keyStr, out KeyCode key) && !NewInput.TryParseKeycode(keyStr, out key))
                {
                    response = "Invalid key code: " + keyStr;
                    return false;
                }

                if (arguments.Count < 2)
                {
                    response = "SYNTAX: \"cmdbind <key/keycode> <command>\"";
                    return false;
                }

                StringBuilder command = new StringBuilder(arguments.Array[arguments.Offset + 1]);
                for (int i = 2; i < arguments.Count; i++)
                {
                    command.Append(" " + arguments.Array[arguments.Offset + i]);
                }

                CmdBinding.KeyBind(key, command.ToString());
                CmdBinding.Save();
                response = string.Format("Command [{0}] has been bound to [{1}]!", command, key);
                return true;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<size=25>Command Binding List:</size>");
            foreach (CmdBinding.Bind bind in CmdBinding.Bindings)
            {
                stringBuilder.AppendLine(string.Format("{0} ({1}) : {2}", bind.key, (int)bind.key, bind.command));
            }

            response = stringBuilder.ToString();
            return true;
        }
    }
}
