namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.PermissionsManagementCommand))]
	public class ReloadCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "reload";

		public string[] Aliases { get; } = new string[1] { "rl" };

		public string Description { get; } = "Reloads the permissions file.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " reloaded permissions files.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
			global::GameCore.ConfigFile.ReloadGameConfigs();
			ServerStatic.RolesConfig.Reload();
			ServerStatic.SharedGroupsConfig = ((global::GameCore.ConfigSharing.Paths[4] == null) ? null : new YamlConfig(global::GameCore.ConfigSharing.Paths[4] + "shared_groups.txt"));
			ServerStatic.SharedGroupsMembersConfig = ((global::GameCore.ConfigSharing.Paths[5] == null) ? null : new YamlConfig(global::GameCore.ConfigSharing.Paths[5] + "shared_groups_members.txt"));
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			response = "Permissions file reloaded.";
			return true;
		}
	}
}
