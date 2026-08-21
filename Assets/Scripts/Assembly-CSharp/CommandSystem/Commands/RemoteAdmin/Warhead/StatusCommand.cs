namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Warhead.WarheadCommand))]
	public class StatusCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "status";

		public string[] Aliases { get; } = new string[1] { "s" };

		public string Description { get; } = "Returns the status of the alpha warhead.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			if (AlphaWarheadController.Detonated)
			{
				response = "Warhead has been detonated.";
			}
			else if (AlphaWarheadController.InProgress)
			{
				response = "Detonation is in progress.";
			}
			else if (!AlphaWarheadOutsitePanel.nukeside.nukenabled)
			{
				response = "Warhead is disabled.";
			}
			else if (AlphaWarheadController.Singleton.CooldownEndTime > global::Mirror.NetworkTime.time)
			{
				response = "Warhead is restarting.";
			}
			else
			{
				response = "Warhead is ready to detonation.";
			}
			return true;
		}
	}
}
