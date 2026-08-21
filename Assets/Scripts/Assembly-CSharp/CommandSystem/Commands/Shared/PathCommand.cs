namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.Shared.ConfigCommand))]
	public class PathCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "path";

		public string[] Aliases { get; }

		public string Description { get; } = "Returns the path to the config file";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConfigs, out response))
			{
				return false;
			}
			response = "Configuration file path: <i>" + FileManager.GetAppFolder(addSeparator: true, serverConfig: true) + "</i>";
			return true;
		}
	}
}
