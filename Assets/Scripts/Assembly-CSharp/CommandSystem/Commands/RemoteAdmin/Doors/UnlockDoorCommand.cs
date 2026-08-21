namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class UnlockDoorCommand : global::CommandSystem.Commands.RemoteAdmin.Doors.BaseDoorCommand
	{
		public override string Command { get; } = "unlock";

		public override string[] Aliases { get; } = new string[2] { "unlockdoor", "ul" };

		public override string Description { get; } = "Unlocks a specified door.";

		protected override void OnTargetFound(global::Interactables.Interobjects.DoorUtils.DoorVariant door)
		{
			door.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.AdminCommand, newState: false);
		}
	}
}
