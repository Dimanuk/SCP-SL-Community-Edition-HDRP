namespace CommandSystem.Commands.RemoteAdmin.Decontamination
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class DecontaminationCommand : ParentCommand, global::CommandSystem.IUsageProvider
	{
		public override string Command { get; } = "decontamination";

		public override string[] Aliases { get; } = new string[1] { "decont" };

		public override string Description { get; } = "Controls the LCZ decontamination.";

		public string[] Usage { get; } = new string[1] { "status/enable/disable" };

		public static global::CommandSystem.Commands.RemoteAdmin.Decontamination.DecontaminationCommand Create()
		{
			global::CommandSystem.Commands.RemoteAdmin.Decontamination.DecontaminationCommand decontaminationCommand = new global::CommandSystem.Commands.RemoteAdmin.Decontamination.DecontaminationCommand();
			decontaminationCommand.LoadGeneratedCommands();
			return decontaminationCommand;
		}

		protected override bool ExecuteParent(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand (" + Usage[0] + ")!";
			return false;
		}

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Decontamination.DisableCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Decontamination.EnableCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Decontamination.ForceCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Decontamination.StatusCommand());
		}
	}
}
