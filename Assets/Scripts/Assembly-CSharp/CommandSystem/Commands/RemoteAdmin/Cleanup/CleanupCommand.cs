namespace CommandSystem.Commands.RemoteAdmin.Cleanup
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class CleanupCommand : ParentCommand, global::CommandSystem.IUsageProvider
	{
		public override string Command { get; } = "cleanup";

		public override string[] Aliases { get; } = new string[0];

		public override string Description { get; } = "Controls and cleans several elements of the map.";

		public string[] Usage { get; } = new string[1] { "ragdolls/items" };

		public static global::CommandSystem.Commands.RemoteAdmin.Cleanup.CleanupCommand Create()
		{
			global::CommandSystem.Commands.RemoteAdmin.Cleanup.CleanupCommand cleanupCommand = new global::CommandSystem.Commands.RemoteAdmin.Cleanup.CleanupCommand();
			cleanupCommand.LoadGeneratedCommands();
			return cleanupCommand;
		}

		protected override bool ExecuteParent(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand (" + Usage[0] + ").";
			return false;
		}

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Cleanup.CorpsesCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Cleanup.ItemsCommand());
		}
	}
}
