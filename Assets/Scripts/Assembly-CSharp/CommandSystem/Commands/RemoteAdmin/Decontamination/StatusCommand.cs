namespace CommandSystem.Commands.RemoteAdmin.Decontamination
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Decontamination.DecontaminationCommand))]
	public class StatusCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "status";

		public string[] Aliases { get; } = new string[3] { "s", "c", "check" };

		public string Description { get; } = "Returns status of the LCZ decontamination.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			response = "Decontamination is " + ((global::LightContainmentZoneDecontamination.DecontaminationController.Singleton.DecontaminationOverride == global::LightContainmentZoneDecontamination.DecontaminationController.DecontaminationStatus.Disabled) ? "DISABLED" : "ENABLED") + ".";
			return true;
		}
	}
}
