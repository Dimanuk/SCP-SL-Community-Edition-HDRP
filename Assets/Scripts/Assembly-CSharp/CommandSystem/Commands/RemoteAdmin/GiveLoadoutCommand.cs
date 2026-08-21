namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class GiveLoadoutCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "giveloadout";

		public string[] Aliases { get; } = new string[3] { "sendloadout", "giveinventory", "grantloadout" };

		public string Description { get; } = "Grant target(s) the specified role's loadout.";

		public string[] Usage { get; } = new string[2] { "%player%", "%role%" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!ReferenceHub.TryGetHostHub(out var hub))
			{
				response = "You are not connected to a server.";
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
			{
				return false;
			}
			CharacterClassManager characterClassManager = hub.characterClassManager;
			if (characterClassManager == null || !characterClassManager.isLocalPlayer || !characterClassManager.isServer || !characterClassManager.RoundStarted)
			{
				response = "Please start round before using this command.";
				return false;
			}
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			if (!TryParseRole(newargs[0], out var prb))
			{
				response = "Invalid role ID / name.";
				return false;
			}
			if (!global::InventorySystem.Configs.StartingInventories.DefinedInventories.ContainsKey(prb.RoleTypeId))
			{
				response = "Specified role does not have a defined inventory.";
				return false;
			}
			ProvideRoleFlag(newargs, out var spawnFlags);
			bool resetInventory = spawnFlags.HasFlag(global::PlayerRoles.RoleSpawnFlags.AssignInventory);
			int num = 0;
			foreach (ReferenceHub item in list)
			{
				if (!(item == null))
				{
					global::InventorySystem.InventoryItemProvider.ServerGrantLoadout(item, prb.RoleTypeId, resetInventory);
					ServerLogs.AddLog(ServerLogs.Modules.ClassChange, sender.LogName + " has given " + item.LoggedNameFromRefHub() + " the following role's loadout: " + prb.RoleName + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
					num++;
				}
			}
			response = string.Format("Done! Given {0}'s loadout to {1} player{2}!", prb.RoleName, num, (num == 1) ? "" : "s");
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
	}
}
