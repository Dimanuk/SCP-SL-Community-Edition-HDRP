namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class SetGroupCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "setgroup";

		public string[] Aliases { get; } = new string[2] { "sg", "setrole" };

		public string Description { get; } = "Temporarily assigns the specified player(s) to a specified group.";

		public string[] Usage { get; } = new string[2] { "%player%", "Group (-1 Removes)" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.SetGroup, out response))
			{
				return false;
			}
			if (!CharacterClassManager.OnlineMode)
			{
				response = "This command requires the server to operate in online mode!";
				return false;
			}
			if (arguments.Count >= 2)
			{
				string[] newargs;
				global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
				UserGroup userGroup = null;
				if (newargs[0] != "-1")
				{
					userGroup = ServerStatic.PermissionsHandler.GetGroup(newargs[0]);
					if (userGroup == null)
					{
						response = "Requested group doesn't exist! Use group \"-1\" to remove user group.";
						return false;
					}
				}
				int num = 0;
				int num2 = 0;
				string arg = string.Empty;
				foreach (ReferenceHub item in list)
				{
					if (!item.serverRoles.PublicKeyAccepted)
					{
						arg = "Public key not accepted for " + item.nicknameSync.MyNick;
						num2++;
						continue;
					}
					if (userGroup == null)
					{
						ServerLogs.AddLog(ServerLogs.Modules.Permissions, sender.LogName + " removed local group from player " + item.LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
					}
					else
					{
						ServerLogs.AddLog(ServerLogs.Modules.Permissions, sender.LogName + " set local group of player " + item.LoggedNameFromRefHub() + " to " + newargs[0] + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
					}
					item.serverRoles.SetGroup(userGroup, ovr: false, byAdmin: true);
					num++;
				}
				if (num2 == 0)
				{
					response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
					return true;
				}
				response = $"Failed to execute the command! Failures: {num2}\nLast error log:\n{arg}";
				return false;
			}
			response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
			return false;
		}
	}
}
