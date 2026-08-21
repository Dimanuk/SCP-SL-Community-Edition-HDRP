namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class StopNextRoundCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "stopnextround";

		public string[] Aliases { get; } = new string[1] { "snr" };

		public string Description { get; } = "Stops the server after the next round.";

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
			if (!ServerStatic.IsDedicated)
			{
				response = "This command can be only executed on a dedicated servers.";
				return false;
			}
			if (ServerStatic.StopNextRound == ServerStatic.NextRoundAction.Shutdown)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " canceled server stop after the round end.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
				ServerStatic.StopNextRound = ServerStatic.NextRoundAction.DoNothing;
				ServerConsole.AddOutputEntry(default(global::ServerOutput.ExitActionResetEntry));
				response = "Server WON'T stop after next round.";
			}
			else
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " scheduled server stop after the round end.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
				ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Shutdown;
				ServerConsole.AddOutputEntry(default(global::ServerOutput.ExitActionShutdownEntry));
				response = "Server WILL stop after next round.";
			}
			return true;
		}
	}
}
