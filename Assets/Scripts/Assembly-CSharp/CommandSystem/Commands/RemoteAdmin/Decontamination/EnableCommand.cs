namespace CommandSystem.Commands.RemoteAdmin.Decontamination
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Decontamination.DecontaminationCommand))]
	public class EnableCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "enable";

		public string[] Aliases { get; } = new string[3] { "e", "1", "on" };

		public string Description { get; } = "Enables the LCZ decontamination.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (global::LightContainmentZoneDecontamination.DecontaminationController.Singleton.DecontaminationOverride == global::LightContainmentZoneDecontamination.DecontaminationController.DecontaminationStatus.None)
			{
				response = "Decontamination is already enabled.";
				return true;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " enabled the LCZ decontamination.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			global::LightContainmentZoneDecontamination.DecontaminationController.Singleton.DecontaminationOverride = global::LightContainmentZoneDecontamination.DecontaminationController.DecontaminationStatus.None;
			response = "Decontamination has been enabled.";
			return true;
		}
	}
}
