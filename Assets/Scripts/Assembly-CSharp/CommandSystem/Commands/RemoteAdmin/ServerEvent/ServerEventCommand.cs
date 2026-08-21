namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class ServerEventCommand : ParentCommand, global::CommandSystem.IUsageProvider
	{
		public override string Command { get; } = "SERVER_EVENT";

		public override string[] Aliases { get; }

		public override string Description { get; } = "Various server event controls";

		public string[] Usage { get; } = new string[1] { "SubCommand" };

		public static global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand Create()
		{
			global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand serverEventCommand = new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand();
			serverEventCommand.LoadGeneratedCommands();
			return serverEventCommand;
		}

		protected override bool ExecuteParent(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "Invalid subcommand!";
			return false;
		}

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.DetonationCancelCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.DetonationInstantCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.DetonationStartCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ForceCICommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ForceMTFCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.PlayEffectCICommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.PlayEffectMTFCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.RespawnCICommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.RespawnMTFCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.RoundRestartCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.TerminateUnconnectedCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ServerEvent.TokenResetCommand());
		}
	}
}
