namespace CommandSystem.Commands.RemoteAdmin.Tickets
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Tickets.TokensCommand))]
	public class InfoCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "info";

		public string[] Aliases { get; } = new string[1] { "fetch" };

		public string Description { get; } = "Fetches the ticket information.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			response = global::Respawning.RespawnManager.GetRemoteAdminInfoString() ?? "";
			return true;
		}
	}
}
