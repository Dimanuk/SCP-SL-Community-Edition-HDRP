namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class PlayEffectMTFCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "PLAY_EFFECT_MTF";

		public string[] Aliases { get; }

		public string Description { get; } = "Forces the MTF Helicopter to appear.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			global::Respawning.RespawnEffectsController.ExecuteAllEffects(global::Respawning.RespawnEffectsController.EffectType.Selection, global::Respawning.SpawnableTeamType.NineTailedFox);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced MTF Helicopter to spawn.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "MTF Helicopter spawned.";
			return true;
		}
	}
}
