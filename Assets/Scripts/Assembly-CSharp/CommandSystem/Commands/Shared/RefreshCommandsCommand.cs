namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class RefreshCommandsCommand : global::CommandSystem.ICommand
	{
		private readonly global::CommandSystem.ICommandHandler _commandHandler;

		public string Command { get; } = "refreshcommands";

		public string[] Aliases { get; }

		public string Description { get; } = "Reloads all commands.";

		public RefreshCommandsCommand(global::CommandSystem.ICommandHandler commandHandler)
		{
			_commandHandler = commandHandler;
		}

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			_commandHandler.ClearCommands();
			_commandHandler.LoadGeneratedCommands();
			response = "Successfully reloaded all commands!";
			return true;
		}
	}
}
