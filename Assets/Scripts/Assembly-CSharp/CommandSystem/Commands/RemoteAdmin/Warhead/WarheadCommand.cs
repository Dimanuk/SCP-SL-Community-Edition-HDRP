namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class WarheadCommand : ParentCommand, global::CommandSystem.IUsageProvider
	{
		public override string Command { get; } = "warhead";

		public override string[] Aliases { get; } = new string[1] { "wh" };

		public override string Description { get; } = "Manages the alpha warhead.";

		public string[] Usage { get; } = new string[1] { "SubCommand" };

		public static global::CommandSystem.Commands.RemoteAdmin.Warhead.WarheadCommand Create()
		{
			global::CommandSystem.Commands.RemoteAdmin.Warhead.WarheadCommand warheadCommand = new global::CommandSystem.Commands.RemoteAdmin.Warhead.WarheadCommand();
			warheadCommand.LoadGeneratedCommands();
			return warheadCommand;
		}

		protected override bool ExecuteParent(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand!";
			return false;
		}

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Warhead.CancelCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Warhead.DetonateCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Warhead.DisableCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Warhead.EnableCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Warhead.InstantCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Warhead.LockCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Warhead.SetTimeCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Warhead.StatusCommand());
		}
	}
}
