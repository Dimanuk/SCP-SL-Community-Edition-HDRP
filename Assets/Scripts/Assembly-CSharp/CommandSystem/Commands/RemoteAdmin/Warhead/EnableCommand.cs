namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Warhead.WarheadCommand))]
	public class EnableCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "enable";

		public string[] Aliases { get; } = new string[1] { "e" };

		public string Description { get; } = "Enables the alpha warhead.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " enabled the warhead.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			AlphaWarheadOutsitePanel.nukeside.nukenabled = true;
			response = "Warhead has been enabled.";
			return true;
		}
	}
}
