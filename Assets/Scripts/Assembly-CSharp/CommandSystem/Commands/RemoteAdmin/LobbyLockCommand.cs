namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class LobbyLockCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "lobbylock";

		public string[] Aliases { get; } = new string[2] { "ll", "llock" };

		public string Description { get; } = "Locks or unlocks the lobby (prevents the round from starting).";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (global::GameCore.RoundStart.RoundStarted)
			{
				response = "This command can only be ran while in the lobby.";
				return false;
			}
			global::GameCore.RoundStart.LobbyLock = !global::GameCore.RoundStart.LobbyLock;
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + (global::GameCore.RoundStart.LobbyLock ? " enabled " : " disabled ") + "lobby lock.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "Lobby lock " + (global::GameCore.RoundStart.LobbyLock ? "enabled!" : "disabled!");
			return true;
		}
	}
}
