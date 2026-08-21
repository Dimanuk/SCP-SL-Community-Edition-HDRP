namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class TerminateUnconnectedCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "TERMINATE_UNCONN";

		public string[] Aliases { get; }

		public string Description { get; } = "Terminates unconnected players.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			foreach (global::Mirror.NetworkConnectionToClient value in global::Mirror.NetworkServer.connections.Values)
			{
				if (global::GameCore.Console.FindConnectedRoot(value) == null)
				{
					value.Disconnect();
				}
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " terminated unconnected players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "Terminated unconnected players.";
			return true;
		}
	}
}
