using System;

namespace CommandSystem.Commands.Console.Prefs
{
    public class PrefsCommand : ParentCommand
    {
        public override string Command { get; } = "prefs";

        public override string[] Aliases { get; } = new[] { "playerprefs" };

        public override string Description { get; } = "Controls game settings";

        public static PrefsCommand Create()
        {
            PrefsCommand prefsCommand = new PrefsCommand();
            prefsCommand.LoadGeneratedCommands();
            return prefsCommand;
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "You must specify a subcommand!";
            return false;
        }

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new ClearCommand());
            RegisterCommand(new DeleteCommand());
            RegisterCommand(new GetCommand());
            RegisterCommand(new ReloadCommand());
            RegisterCommand(new SetCommand());
        }
    }
}
