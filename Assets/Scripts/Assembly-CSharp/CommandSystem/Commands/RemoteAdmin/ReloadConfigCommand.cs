namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class ReloadConfigCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "reloadconfig";

		public string[] Aliases { get; } = new string[1] { "rc" };

		public string Description { get; } = "Reloads all configs.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConfigs, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " reloaded configuration files.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
			try
			{
				global::GameCore.ConfigFile.ReloadGameConfigs();
				response = "Reloaded all configs!";
				return true;
			}
			catch (global::System.Exception arg)
			{
				response = $"Reloading configs failed: {arg}";
				return false;
			}
		}
	}
}
