namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Warhead.WarheadCommand))]
	public class DetonateCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "detonate";

		public string[] Aliases { get; } = new string[2] { "det", "start" };

		public string Description { get; } = "Starts the alpha warhead detonation sequence.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " started warhead detonation.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			AlphaWarheadController.Singleton.StartDetonation();
			response = "Detonation sequence started.";
			return true;
		}
	}
}
