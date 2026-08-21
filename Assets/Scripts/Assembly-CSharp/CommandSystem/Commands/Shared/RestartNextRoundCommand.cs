namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class RestartNextRoundCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "restartnextround";

		public string[] Aliases { get; } = new string[1] { "rnr" };

		public string Description { get; } = "Restarts the server after the current round is finished.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (sender is CommandSender sender2 && !sender2.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			if (!global::Mirror.NetworkServer.active)
			{
				response = "This command can only be used on a server.";
				return false;
			}
			if (ServerStatic.StopNextRound == ServerStatic.NextRoundAction.Restart)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " canceled server restart after the round end.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
				ServerStatic.StopNextRound = ServerStatic.NextRoundAction.DoNothing;
				ServerConsole.AddOutputEntry(default(global::ServerOutput.ExitActionResetEntry));
				response = "Server WON'T restart after next round.";
			}
			else
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " scheduled server restart after the round end.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
				ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
				ServerConsole.AddOutputEntry(default(global::ServerOutput.ExitActionRestartEntry));
				response = "Server WILL restart after next round.";
			}
			return true;
		}
	}
}
