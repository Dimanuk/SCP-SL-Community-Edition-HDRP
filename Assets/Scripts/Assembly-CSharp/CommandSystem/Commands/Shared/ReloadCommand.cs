namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.Shared.ConfigCommand))]
	public class ReloadCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "reload";

		public string[] Aliases { get; } = new string[2] { "r", "rld" };

		public string Description { get; } = "Reloads the games config";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConfigs, out response))
			{
				return false;
			}
			try
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " reloaded configuration and permissions files.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
				global::GameCore.ConfigFile.ReloadGameConfigs();
				ServerStatic.RolesConfig = new YamlConfig(ServerStatic.RolesConfigPath ?? (FileManager.GetAppFolder(addSeparator: true, serverConfig: true) + "config_remoteadmin.txt"));
				ServerStatic.SharedGroupsConfig = ((global::GameCore.ConfigSharing.Paths[4] == null) ? null : new YamlConfig(global::GameCore.ConfigSharing.Paths[4] + "shared_groups.txt"));
				ServerStatic.SharedGroupsMembersConfig = ((global::GameCore.ConfigSharing.Paths[5] == null) ? null : new YamlConfig(global::GameCore.ConfigSharing.Paths[5] + "shared_groups_members.txt"));
				ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			}
			catch (global::System.Exception arg)
			{
				response = $"Failed to reload the configuration file. Error: {arg}";
				return false;
			}
			response = "Configuration file successfully reloaded. Some of the changes will be applied in the next round.";
			return true;
		}
	}
}
