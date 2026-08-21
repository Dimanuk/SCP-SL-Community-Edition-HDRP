namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class GotoCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "goto";

		public string[] Aliases { get; }

		public string Description { get; } = "Teleports you to the specified player.";

		public string[] Usage { get; } = new string[1] { "%player%" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (!(sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender))
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			if (!global::PlayerRoles.PlayerRolesUtils.IsAlive(playerCommandSender.ReferenceHub))
			{
				response = "Command disabled when you are not alive!";
				return false;
			}
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			if (list == null || list.Count != 1)
			{
				response = "This commands you to specify exactly one specific player!";
				return false;
			}
			if (global::PlayerRoles.FirstPersonControl.FpcExtensionMethods.TryOverridePosition(playerCommandSender.ReferenceHub, list[0].transform.position, global::UnityEngine.Vector3.zero))
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " teleported themself to player " + list[0].LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
				response = "Done!";
				return true;
			}
			response = "Your current role does not support this operation.";
			return false;
		}
	}
}
