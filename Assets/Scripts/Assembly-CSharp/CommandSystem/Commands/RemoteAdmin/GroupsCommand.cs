namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.PermissionsManagementCommand))]
	public class GroupsCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "groups";

		public string[] Aliases { get; }

		public string Description { get; } = "Lists all defined permission groups.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent("Groups defined on this server:");
			global::System.Collections.Generic.Dictionary<string, UserGroup> allGroups = ServerStatic.PermissionsHandler.GetAllGroups();
			ServerRoles.NamedColor[] namedColors = ReferenceHub.LocalHub.serverRoles.NamedColors;
			foreach (global::System.Collections.Generic.KeyValuePair<string, UserGroup> permentry in allGroups)
			{
				try
				{
					stringBuilder.AppendFormat("\n{0} ({1}) - <color=#{2}>{3}</color> in color {4}", permentry.Key, permentry.Value.Permissions, global::System.Linq.Enumerable.FirstOrDefault(namedColors, (ServerRoles.NamedColor x) => x.Name == permentry.Value.BadgeColor)?.ColorHex, permentry.Value.BadgeText, permentry.Value.BadgeColor);
				}
				catch
				{
					stringBuilder.AppendFormat("\n{0} ({1}) - {2} in color {3}", permentry.Key, permentry.Value.Permissions, permentry.Value.BadgeText, permentry.Value.BadgeColor);
				}
				foreach (global::System.Collections.Generic.KeyValuePair<PlayerPermissions, string> permissionCode in PermissionsHandler.PermissionCodes)
				{
					if (PermissionsHandler.IsPermitted(permentry.Value.Permissions, permissionCode.Key))
					{
						stringBuilder.Append(" " + permissionCode.Value);
					}
				}
			}
			response = stringBuilder.ToString();
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			return true;
		}
	}
}
