namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class RoundLockCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "roundlock";

		public string[] Aliases { get; } = new string[2] { "rl", "rlock" };

		public string Description { get; } = "Locks or unlocks the current round (prevents it from ending).";

		public string[] Usage { get; } = new string[1] { "enable/disable (Leave blank for toggle)" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (arguments.Count >= 1)
			{
				string[] newargs = new string[1] { arguments.Array[1] };
				if (Misc.TryCommandModeFromArgs(ref newargs, out var mode))
				{
					switch (mode)
					{
					case Misc.CommandOperationMode.Enable:
						RoundSummary.RoundLock = true;
						break;
					case Misc.CommandOperationMode.Disable:
						RoundSummary.RoundLock = false;
						break;
					case Misc.CommandOperationMode.Toggle:
						RoundSummary.RoundLock = !RoundSummary.RoundLock;
						break;
					}
					response = "Round lock set to " + (RoundSummary.RoundLock ? "enabled!" : "disabled!");
					return true;
				}
			}
			RoundSummary.RoundLock = !RoundSummary.RoundLock;
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + (RoundSummary.RoundLock ? " enabled " : " disabled ") + "round lock.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "Round lock " + (RoundSummary.RoundLock ? "enabled!" : "disabled!");
			return true;
		}
	}
}
