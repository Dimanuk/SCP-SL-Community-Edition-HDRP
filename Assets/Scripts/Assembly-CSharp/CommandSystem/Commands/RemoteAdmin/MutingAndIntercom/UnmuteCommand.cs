namespace CommandSystem.Commands.RemoteAdmin.MutingAndIntercom
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class UnmuteCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "unmute";

		public string[] Aliases { get; }

		public string Description { get; } = "Allows the specified player(s) to speak again.";

		public string[] Usage { get; } = new string[1] { "%player%" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(new PlayerPermissions[3]
			{
				PlayerPermissions.BanningUpToDay,
				PlayerPermissions.LongTermBanning,
				PlayerPermissions.PlayersManagement
			}, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			int num = 0;
			if (list != null)
			{
				foreach (ReferenceHub item in list)
				{
                    global::VoiceChat.VoiceChatMutes.RevokeLocalMute(item.characterClassManager.UserId);
                    ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " unmuted player " + item.LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                    num++;
                }
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
