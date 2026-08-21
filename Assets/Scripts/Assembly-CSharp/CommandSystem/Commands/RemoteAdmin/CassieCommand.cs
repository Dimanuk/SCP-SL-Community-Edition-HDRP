namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class CassieCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "cassie";

		public string[] Aliases { get; }

		public string Description { get; } = "Sends an announcement over the CASSIE system.";

		public string[] Usage { get; } = new string[1] { "message" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Announcer, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string text = global::Utils.RAUtils.FormatArguments(arguments, 0);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " started a cassie announcement: " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			global::Respawning.RespawnEffectsController.PlayCassieAnnouncement(text, makeHold: false, makeNoise: true, customAnnouncement: true);
			response = "Announcement sent!";
			return true;
		}
	}
}
