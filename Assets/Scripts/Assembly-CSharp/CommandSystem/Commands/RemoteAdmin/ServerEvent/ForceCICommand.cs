namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class ForceCICommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "FORCE_CI";

		public string[] Aliases { get; }

		public string Description { get; } = "Forces the next team to respawn to be Chaos Insurgency.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			global::Respawning.RespawnManager.Singleton.NextKnownTeam = global::Respawning.SpawnableTeamType.ChaosInsurgency;
			global::Respawning.RespawnTokensManager.ForceTeamDominance(global::Respawning.SpawnableTeamType.ChaosInsurgency, 1f);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced CI respawn next.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "Chaos Insurgency will be respawning next.";
			return true;
		}
	}
}
