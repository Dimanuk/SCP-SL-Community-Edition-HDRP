namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class CloseDoorCommand : global::CommandSystem.Commands.RemoteAdmin.Doors.BaseDoorCommand
	{
		public override string Command { get; } = "close";

		public override string[] Aliases { get; } = new string[2] { "closedoor", "c" };

		public override string Description { get; } = "Closes a specified door.";

		protected override void OnTargetFound(global::Interactables.Interobjects.DoorUtils.DoorVariant door)
		{
			door.TargetState = false;
		}
	}
}
