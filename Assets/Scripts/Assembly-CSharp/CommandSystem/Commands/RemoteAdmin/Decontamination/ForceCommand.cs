namespace CommandSystem.Commands.RemoteAdmin.Decontamination
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Decontamination.DecontaminationCommand))]
	public class ForceCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "force";

		public string[] Aliases { get; } = new string[1] { "f" };

		public string Description { get; } = "Forces the LCZ decontamination to begin.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (global::LightContainmentZoneDecontamination.DecontaminationController.Singleton.DecontaminationOverride == global::LightContainmentZoneDecontamination.DecontaminationController.DecontaminationStatus.Disabled)
			{
				response = "Decontamination is currently disabled.";
				return true;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced the LCZ decontamination to begin.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			global::LightContainmentZoneDecontamination.DecontaminationController.Singleton.ForceDecontamination();
			response = "Decontamination has begun.";
			return true;
		}
	}
}
