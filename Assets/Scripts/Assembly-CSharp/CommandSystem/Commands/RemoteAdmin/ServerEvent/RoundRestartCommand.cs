namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class RoundRestartCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "ROUND_RESTART";

		public string[] Aliases { get; } = new string[3] { "RR", "RESTART", "ROUNDRESTART" };

		public string Description { get; } = "Restarts the current round.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced round restart.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			global::RoundRestarting.RoundRestart.InitiateRoundRestart();
			response = "Round restart forced.";
			return true;
		}
	}
}
