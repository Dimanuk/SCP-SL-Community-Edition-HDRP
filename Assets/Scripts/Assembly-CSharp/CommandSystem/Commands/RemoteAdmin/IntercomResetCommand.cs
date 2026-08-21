namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class IntercomResetCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "intercom-reset";

		public string[] Aliases { get; } = new string[2] { "icomreset", "ir" };

		public string Description { get; } = "Resets the timer on the intercom to ready-to-speak.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(new PlayerPermissions[3]
			{
				PlayerPermissions.RoundEvents,
				PlayerPermissions.FacilityManagement,
				PlayerPermissions.PlayersManagement
			}, out response))
			{
				return false;
			}
			if (global::PlayerRoles.Voice.Intercom.State != global::PlayerRoles.Voice.IntercomState.Cooldown)
			{
				response = "Intercom is not currently on cooldown. Current state: " + global::PlayerRoles.Voice.Intercom.State;
				return false;
			}
			global::PlayerRoles.Voice.Intercom.State = global::PlayerRoles.Voice.IntercomState.Ready;
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " reset the intercom cooldown.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
			response = "Done! Intercom cooldown reset.";
			return true;
		}
	}
}
