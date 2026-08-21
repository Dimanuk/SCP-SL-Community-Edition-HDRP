namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ElevatorCommand))]
	public class ElevatorUnlockCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "unlock";

		public string[] Aliases { get; } = new string[3] { "u", "ul", "ulck" };

		public string Description { get; } = "Unlocks an elevator.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			if (arguments.Count == 1)
			{
				return global::CommandSystem.Commands.RemoteAdmin.ElevatorLockCommand.TrySetLock(arguments.At(0), locked: false, out response, sender);
			}
			response = "Syntax error: elevator unlock <Elevator ID / all>";
			return false;
		}
	}
}
