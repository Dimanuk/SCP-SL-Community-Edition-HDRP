namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class ForceMTFCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "FORCE_MTF";

		public string[] Aliases { get; }

		public string Description { get; } = "Forces the next team to respawn to be Mobile Task Forces.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			global::Respawning.RespawnManager.Singleton.NextKnownTeam = global::Respawning.SpawnableTeamType.NineTailedFox;
			global::Respawning.RespawnTokensManager.ForceTeamDominance(global::Respawning.SpawnableTeamType.NineTailedFox, 1f);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced MTF respawn next.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "Mobile Task Forces will be respawning next.";
			return true;
		}
	}
}
