namespace CommandSystem.Commands.RemoteAdmin.Decontamination
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Decontamination.DecontaminationCommand))]
	public class DisableCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "disable";

		public string[] Aliases { get; } = new string[3] { "d", "0", "off" };

		public string Description { get; } = "Disables the LCZ decontamination.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (global::LightContainmentZoneDecontamination.DecontaminationController.Singleton.DecontaminationOverride == global::LightContainmentZoneDecontamination.DecontaminationController.DecontaminationStatus.Disabled)
			{
				response = "Decontamination is already disabled.";
				return true;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disabled the LCZ decontamination.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			global::LightContainmentZoneDecontamination.DecontaminationController.Singleton.DecontaminationOverride = global::LightContainmentZoneDecontamination.DecontaminationController.DecontaminationStatus.Disabled;
			response = "Decontamination has been disabled.";
			return true;
		}
	}
}
