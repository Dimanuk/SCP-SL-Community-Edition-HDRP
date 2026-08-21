using System;
using System.Text;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class DebugCommand : ICommand
    {
        public string Command { get; } = "debug";

        public string[] Aliases { get; }

        public string Description { get; } = "Modify importance of debug levels.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 2)
            {
                string module = arguments.Array[arguments.Offset].ToUpperInvariant();
                if (!int.TryParse(arguments.Array[arguments.Offset + 1], out int newLevel) || newLevel > 4)
                {
                    response = string.Format("Could not change the Debug Mode importance: '{0}' is supposed to be a integer value between 0 and {1}.", arguments.Array[arguments.Offset + 1], 4);
                    return false;
                }

                if (ConsoleDebugMode.ChangeImportance(module, newLevel))
                {
                    response = "Debug Level was modified. " + ConsoleDebugMode.ConsoleGetLevel(module);
                    return true;
                }

                response = "Could not change the Debug Mode importance: Module '" + module + "' could not be found.";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "SYNTAX: debug [level] [importance]";
                return false;
            }

            ConsoleDebugMode.GetList(out string[] keys, out string[] descriptions);
            StringBuilder stringBuilder = new StringBuilder("Welcome to Debug Mode. The following modules were found:");
            for (int i = 0; i < keys.Length; i++)
            {
                stringBuilder.Append("\n- <b>" + keys[i] + "</b> - " + descriptions[i]);
            }

            response = stringBuilder.ToString();
            return true;
        }
    }
}
