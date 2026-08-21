namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class DetonationCancelCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "DETONATION_CANCEL";

		public string[] Aliases { get; }

		public string Description { get; } = "Cancels the alpha warhead detonation sequence.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			AlphaWarheadController.Singleton.CancelDetonation();
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " cancelled warhead detonation.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "Warhead detonation cancelled.";
			return true;
		}
	}
}
