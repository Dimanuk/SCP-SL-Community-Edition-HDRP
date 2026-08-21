namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.GroupCommand))]
	public class SetColorCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "setcolor";

		public string[] Aliases { get; }

		public string Description { get; } = "Sets the badge color for a group.";

		public string[] Usage { get; } = new string[2] { "Group Name", "Color" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string text = arguments.At(0);
			string colorName = arguments.At(1);
			if (ServerStatic.PermissionsHandler.GetGroup(text) == null)
			{
				response = "Group can't be found.";
				return false;
			}
			ServerRoles.NamedColor namedColor = global::System.Linq.Enumerable.SingleOrDefault(ReferenceHub.LocalHub.serverRoles.NamedColors, (ServerRoles.NamedColor c) => c.Name == colorName);
			if (namedColor == null)
			{
				response = "The entered color was invalid! Type \"colors\" for a list of available colors.";
				return false;
			}
			if (namedColor.Restricted)
			{
				response = "The color you have chosen is restricted to global badges only.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " set color of group " + text + " to " + colorName + ".", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
			ServerStatic.RolesConfig.SetString(text + "_color", colorName);
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			response = "Group color updated to: " + colorName;
			return true;
		}
	}
}
