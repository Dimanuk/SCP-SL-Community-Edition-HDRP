namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class RespawnCICommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "RESPAWN_CI";

		public string[] Aliases { get; }

		public string Description { get; } = "Forces the CI to respawn.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			global::Respawning.RespawnManager.Singleton.ForceSpawnTeam(global::Respawning.SpawnableTeamType.ChaosInsurgency);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced CI respawn.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "CI respawn forced.";
			return true;
		}
	}
}
