namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class PermCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "perm";

		public string[] Aliases { get; }

		public string Description { get; } = "Lists your permissions.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!(sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender))
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			ulong permissions = playerCommandSender.ReferenceHub.gameObject.GetComponent<ServerRoles>().Permissions;
			string text = "Your permissions:";
			foreach (string allPermission in ServerStatic.PermissionsHandler.GetAllPermissions())
			{
				string text2 = (ServerStatic.PermissionsHandler.IsRaPermitted(ServerStatic.PermissionsHandler.GetPermissionValue(allPermission)) ? "*" : "");
				text += string.Format("\n{0}{1} ({2}): {3}", allPermission, text2, ServerStatic.PermissionsHandler.GetPermissionValue(allPermission), ServerStatic.PermissionsHandler.IsPermitted(permissions, allPermission) ? "YES" : "NO");
			}
			response = text;
			return true;
		}
	}
}
