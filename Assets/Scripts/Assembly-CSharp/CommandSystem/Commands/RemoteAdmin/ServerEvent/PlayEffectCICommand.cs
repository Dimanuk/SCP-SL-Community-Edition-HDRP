namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class PlayEffectCICommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "PLAY_EFFECT_CI";

		public string[] Aliases { get; }

		public string Description { get; } = "Forces the CI Van to appear.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			global::Respawning.RespawnEffectsController.ExecuteAllEffects(global::Respawning.RespawnEffectsController.EffectType.Selection, global::Respawning.SpawnableTeamType.ChaosInsurgency);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced CI Van to spawn.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "CI Van spawned.";
			return true;
		}
	}
}
