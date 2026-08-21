namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class RoundRestartCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "roundrestart";

		public string[] Aliases { get; } = new string[2] { "rr", "restart" };

		public string Description { get; } = "Restarts the round.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!global::Mirror.NetworkServer.active)
			{
				response = "You are not connected to a local server.";
				return false;
			}
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
