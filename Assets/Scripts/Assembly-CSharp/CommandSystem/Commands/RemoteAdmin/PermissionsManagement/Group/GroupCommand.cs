namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.PermissionsManagementCommand))]
	public class GroupCommand : ParentCommand
	{
		public override string Command { get; } = "group";

		public override string[] Aliases { get; } = new string[1] { "gr" };

		public override string Description { get; } = "Group management";

		public static global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.GroupCommand Create()
		{
			global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.GroupCommand groupCommand = new global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.GroupCommand();
			groupCommand.LoadGeneratedCommands();
			return groupCommand;
		}

		protected override bool ExecuteParent(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "Invalid subcommand. Available commands: info, grant, setcolor, settag, enablecover.";
			return true;
		}

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.DisableCoverCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.EnableCoverCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.GrantCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.InfoCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.RevokeCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.SetColorCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.SetTagCommand());
		}
	}
}
