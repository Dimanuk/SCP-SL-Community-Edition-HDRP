namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand))]
	public class DetonationInstantCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "DETONATION_INSTANT";

		public string[] Aliases { get; }

		public string Description { get; } = "Instantly detonates the alpha warhead.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			AlphaWarheadController.Singleton.ForceTime(0f);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " instantly detonated the warhead.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "Warhead will detonate instantly.";
			return true;
		}
	}
}
