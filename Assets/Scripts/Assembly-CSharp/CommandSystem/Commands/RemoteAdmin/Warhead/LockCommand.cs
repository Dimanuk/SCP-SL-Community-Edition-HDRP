namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Warhead.WarheadCommand))]
	public class LockCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "lock";

		public string[] Aliases { get; } = new string[2] { "l", "lck" };

		public string Description { get; } = "Locks the alpha warhead detonation so it cannot be activated.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			if (AlphaWarheadController.Singleton == null)
			{
				response = "Warhead not found";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " locked the alpha warhead detonation.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			AlphaWarheadController.Singleton.IsLocked = !AlphaWarheadController.Singleton.IsLocked;
			response = "Alpha Warhead Lock " + (AlphaWarheadController.Singleton.IsLocked ? "enabled!" : "disabled!");
			return true;
		}
	}
}
