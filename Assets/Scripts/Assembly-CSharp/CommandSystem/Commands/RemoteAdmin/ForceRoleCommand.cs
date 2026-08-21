namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class ForceRoleCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "forcerole";

		public string[] Aliases { get; } = new string[3] { "fc", "fr", "forceclass" };

		public string Description { get; } = "Forces a player to a specified role.";

		public string[] Usage { get; } = new string[2] { "%player%", "%role%" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!ReferenceHub.TryGetHostHub(out var _))
			{
				response = "You are not connected to a server.";
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			bool self = list.Count == 1 && sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender && playerCommandSender.ReferenceHub == list[0];
			if (!TryParseRole(newargs[0], out var prb))
			{
				response = "Invalid role ID / name.";
				return false;
			}
			if (!HasPerms(prb.RoleTypeId, self, sender, out response))
			{
				return false;
			}
			ProvideRoleFlag(newargs, out var spawnFlags);
			int num = 0;
			foreach (ReferenceHub item in list)
			{
				if (!(item == null))
				{
					item.roleManager.ServerSetRole(prb.RoleTypeId, global::PlayerRoles.RoleChangeReason.RemoteAdmin, spawnFlags);
					ServerLogs.AddLog(ServerLogs.Modules.ClassChange, sender.LogName + " changed role of player " + item.LoggedNameFromRefHub() + " to " + prb.RoleName + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
					num++;
				}
			}
			response = string.Format("Done! Changed role of {0} player{1} to {2}!", num, (num == 1) ? "" : "s", prb.RoleName);
			return true;
		}

		private void ProvideRoleFlag(string[] arguments, out global::PlayerRoles.RoleSpawnFlags spawnFlags)
		{
			if (arguments.Length > 1 && byte.TryParse(arguments[1], out var result))
			{
				spawnFlags = (global::PlayerRoles.RoleSpawnFlags)result;
			}
			else
			{
				spawnFlags = global::PlayerRoles.RoleSpawnFlags.All;
			}
		}

		private bool TryParseRole(string s, out global::PlayerRoles.PlayerRoleBase prb)
		{
			if (global::System.Enum.TryParse<global::PlayerRoles.RoleTypeId>(s, ignoreCase: true, out var result))
			{
				return global::PlayerRoles.PlayerRoleLoader.TryGetRoleTemplate<global::PlayerRoles.PlayerRoleBase>(result, out prb);
			}
			foreach (global::PlayerRoles.PlayerRoleBase value in global::PlayerRoles.PlayerRoleLoader.AllRoles.Values)
			{
				if (!string.Equals(global::System.Text.RegularExpressions.Regex.Replace(value.RoleName, "\\s+", ""), s, global::System.StringComparison.InvariantCultureIgnoreCase))
				{
					prb = value;
					return true;
				}
			}
			prb = null;
			return false;
		}

		private bool HasPerms(global::PlayerRoles.RoleTypeId targetRole, bool self, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (self)
			{
				if (targetRole != global::PlayerRoles.RoleTypeId.Overwatch || !sender.CheckPermission(PlayerPermissions.Overwatch, out response))
				{
					return sender.CheckPermission(new PlayerPermissions[3]
					{
						PlayerPermissions.ForceclassWithoutRestrictions,
						PlayerPermissions.ForceclassToSpectator,
						PlayerPermissions.ForceclassSelf
					}, out response);
				}
				return true;
			}
			switch (targetRole)
			{
			case global::PlayerRoles.RoleTypeId.Spectator:
				return sender.CheckPermission(new PlayerPermissions[2]
				{
					PlayerPermissions.ForceclassWithoutRestrictions,
					PlayerPermissions.ForceclassToSpectator
				}, out response);
			case global::PlayerRoles.RoleTypeId.Overwatch:
				return sender.CheckPermission(new PlayerPermissions[2]
				{
					PlayerPermissions.ForceclassWithoutRestrictions,
					PlayerPermissions.Overwatch
				}, out response);
			default:
				return sender.CheckPermission(PlayerPermissions.ForceclassWithoutRestrictions, out response);
			}
		}
	}
}
