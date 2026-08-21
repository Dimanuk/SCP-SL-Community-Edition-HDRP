namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class LockDoorCommand : global::CommandSystem.Commands.RemoteAdmin.Doors.BaseDoorCommand
	{
		public override string Command { get; } = "lock";

		public override string[] Aliases { get; } = new string[2] { "lockdoor", "l" };

		public override string Description { get; } = "Locks a specified door.";

		protected override void OnTargetFound(global::Interactables.Interobjects.DoorUtils.DoorVariant door)
		{
			door.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.AdminCommand, newState: true);
		}
	}
}
