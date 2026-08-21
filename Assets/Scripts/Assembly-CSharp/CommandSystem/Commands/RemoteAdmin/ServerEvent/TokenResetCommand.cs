namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class TokenResetCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "TOKEN_RESET";

		public string[] Aliases { get; }

		public string Description { get; } = "Forces the tokens to be reset to their starting values.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			global::Respawning.RespawnManager.Singleton.NextKnownTeam = global::Respawning.SpawnableTeamType.None;
			global::Respawning.RespawnTokensManager.ResetTokens();
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced token reset.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "Tokens have been reset.";
			return true;
		}
	}
}
