namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group.GroupCommand))]
	public class GrantCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "grant";

		public string[] Aliases { get; }

		public string Description { get; } = "Grants a permission to a group.";

		public string[] Usage { get; } = new string[2] { "Group Name", "Permission Name" };

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
			string text2 = arguments.At(1);
			if (ServerStatic.PermissionsHandler.GetGroup(text) == null)
			{
				response = "Group can't be found.";
				return false;
			}
			if (!ServerStatic.PermissionsHandler.GetAllPermissions().Contains(text2))
			{
				response = "Permission can't be found.";
				return false;
			}
			global::System.Collections.Generic.Dictionary<string, string> stringDictionary = ServerStatic.RolesConfig.GetStringDictionary("Permissions");
			global::System.Collections.Generic.List<string> list = null;
			foreach (string key in stringDictionary.Keys)
			{
				if (!(key != text2))
				{
					list = global::System.Linq.Enumerable.ToList(YamlConfig.ParseCommaSeparatedString(stringDictionary[key]));
				}
			}
			if (list == null)
			{
				response = "Permissions can't be found in the config.";
				return false;
			}
			if (list.Contains(text2))
			{
				response = "Group already has that permission.";
				return false;
			}
			list.Add(text2);
			list.Sort();
			string text3 = "[";
			foreach (string item in list)
			{
				if (text3 != "[")
				{
					text3 += ", ";
				}
				text3 += item;
			}
			text3 += "]";
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " granted permission " + text2 + " to group " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
			ServerStatic.RolesConfig.SetStringDictionaryItem("Permissions", text2, text3);
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			response = "Permissions updated.";
			return true;
		}
	}
}
