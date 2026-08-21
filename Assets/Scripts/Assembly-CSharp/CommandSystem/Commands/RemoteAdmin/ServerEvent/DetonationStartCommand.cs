namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class DetonationStartCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "DETONATION_START";

		public string[] Aliases { get; }

		public string Description { get; } = "Force the alpha warhead detonation to start.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			AlphaWarheadController.Singleton.InstantPrepare();
			AlphaWarheadController.Singleton.StartDetonation();
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " started warhead detonation.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "Warhead detonation started.";
			return true;
		}
	}
}
