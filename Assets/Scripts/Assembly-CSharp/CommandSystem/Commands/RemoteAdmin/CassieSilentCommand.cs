namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class CassieSilentCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "cassie_silent";

		public string[] Aliases { get; } = new string[3] { "cassie_silentnoise", "cassie_sn", "cassie_sl" };

		public string Description { get; } = "Sends a silent (no preceding tone) announcement over the CASSIE system.";

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
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " started a silent cassie announcement: " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			global::Respawning.RespawnEffectsController.PlayCassieAnnouncement(text, makeHold: false, makeNoise: false, customAnnouncement: true);
			response = "Silent announcement sent!";
			return true;
		}
	}
}
