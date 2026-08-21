namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class ConfigCommand : ParentCommand, global::CommandSystem.IUsageProvider
	{
		public override string Command { get; } = "config";

		public override string[] Aliases { get; } = new string[1] { "cfg" };

		public override string Description { get; } = "Allows for config debugging and reloading";

		public string[] Usage { get; } = new string[1] { "Reload/Path/Value" };

		public static global::CommandSystem.Commands.Shared.ConfigCommand Create()
		{
			global::CommandSystem.Commands.Shared.ConfigCommand configCommand = new global::CommandSystem.Commands.Shared.ConfigCommand();
			configCommand.LoadGeneratedCommands();
			return configCommand;
		}

		protected override bool ExecuteParent(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand! The valid commands are Reload/Path/Value";
			return false;
		}

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new global::CommandSystem.Commands.Shared.PathCommand());
			RegisterCommand(new global::CommandSystem.Commands.Shared.ReloadCommand());
			RegisterCommand(new global::CommandSystem.Commands.Shared.ValueCommand());
		}
	}
}
