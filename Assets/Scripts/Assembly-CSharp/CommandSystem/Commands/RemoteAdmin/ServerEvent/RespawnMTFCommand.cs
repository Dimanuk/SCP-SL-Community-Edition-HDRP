namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class RespawnMTFCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "RESPAWN_MTF";

		public string[] Aliases { get; }

		public string Description { get; } = "Forces the MTF to respawn.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			global::Respawning.RespawnManager.Singleton.ForceSpawnTeam(global::Respawning.SpawnableTeamType.NineTailedFox);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced MTF respawn.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "MTF respawn forced.";
			return true;
		}
	}
}
