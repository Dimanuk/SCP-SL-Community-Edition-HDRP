namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class ElevatorCommand : ParentCommand, global::CommandSystem.IUsageProvider
	{
		public override string Command { get; } = "elevator";

		public override string[] Aliases { get; } = new string[3] { "lift", "elev", "el" };

		public override string Description { get; } = "Allows to check status of all elevators and force an elevator to move to a specific floor.";

		public string[] Usage { get; } = new string[2] { "list/lock/send/unlock", "Elevator ID / \"all\"" };

		public static global::CommandSystem.Commands.RemoteAdmin.ElevatorCommand Create()
		{
			global::CommandSystem.Commands.RemoteAdmin.ElevatorCommand elevatorCommand = new global::CommandSystem.Commands.RemoteAdmin.ElevatorCommand();
			elevatorCommand.LoadGeneratedCommands();
			return elevatorCommand;
		}

		protected override bool ExecuteParent(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand!\n- elevator list [detailed]\n- elevator lock <Elevator ID / all>\n- elevator send <Elevator ID / all>\n- elevator unlock <Elevator ID / all>\n";
			return false;
		}

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ElevatorListCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ElevatorLockCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ElevatorSendCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ElevatorUnlockCommand());
		}

		internal static bool TryParseGroup(string txt, out global::Interactables.Interobjects.ElevatorManager.ElevatorGroup group)
		{
			if (!int.TryParse(txt, out var result))
			{
				return global::System.Enum.TryParse<global::Interactables.Interobjects.ElevatorManager.ElevatorGroup>(txt, ignoreCase: true, out group);
			}
			group = (global::Interactables.Interobjects.ElevatorManager.ElevatorGroup)result;
			return global::System.Enum.IsDefined(typeof(global::Interactables.Interobjects.ElevatorManager.ElevatorGroup), group);
		}
	}
}
