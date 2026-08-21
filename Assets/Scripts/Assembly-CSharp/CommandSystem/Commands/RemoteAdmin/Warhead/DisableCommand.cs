namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Warhead.WarheadCommand))]
	public class DisableCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "disable";

		public string[] Aliases { get; } = new string[1] { "d" };

		public string Description { get; } = "Disables the alpha warhead.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disabled the warhead.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			AlphaWarheadOutsitePanel.nukeside.nukenabled = false;
			response = "Warhead has been disabled.";
			return true;
		}
	}
}
