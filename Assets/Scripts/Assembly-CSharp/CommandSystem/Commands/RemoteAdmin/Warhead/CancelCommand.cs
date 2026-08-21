namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Warhead.WarheadCommand))]
	public class CancelCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "cancel";

		public string[] Aliases { get; } = new string[2] { "stop", "c" };

		public string Description { get; } = "Stops the alpha warhead detonation sequence.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " cancelled warhead detonation.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			AlphaWarheadController.Singleton.CancelDetonation(null);
			response = "Detonation has been canceled.";
			return true;
		}
	}
}
