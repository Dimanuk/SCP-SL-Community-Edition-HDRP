namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Warhead.WarheadCommand))]
	public class InstantCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "instant";

		public string[] Aliases { get; } = new string[3] { "insdet", "inst", "i" };

		public string Description { get; } = "Instantly detonates the alpha warhead.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " instantly detonated the warhead.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			AlphaWarheadController.Singleton.ForceTime(0f);
			response = "Instant detonation started.";
			return true;
		}
	}
}
