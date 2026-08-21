namespace CommandSystem.Commands.RemoteAdmin.MutingAndIntercom
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class IntercomTextCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "intercomtext";

		public string[] Aliases { get; } = new string[1] { "icomtxt" };

		public string Description { get; } = "Changes the intercom text.";

		public string[] Usage { get; } = new string[1] { "text" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Broadcasting, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string text = string.Join(" ", arguments);
			if (string.IsNullOrEmpty(text.Trim()))
			{
				if (!global::PlayerRoles.Voice.IntercomDisplay.TrySetDisplay(null))
				{
					response = "Intercom text reset failed. Display not found.";
					return false;
				}
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " cleared the intercom text.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
				response = "Reset intercom text.";
				return true;
			}
			if (!global::PlayerRoles.Voice.IntercomDisplay.TrySetDisplay(text))
			{
				response = "Intercom text override failed. Display not found.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " set the intercom text to \"" + text + "\".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "Set intercom text to: " + text;
			return true;
		}
	}
}
