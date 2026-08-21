namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class OpenDoorCommand : global::CommandSystem.Commands.RemoteAdmin.Doors.BaseDoorCommand
	{
		public override string Command { get; } = "open";

		public override string[] Aliases { get; } = new string[2] { "opendoor", "o" };

		public override string Description { get; } = "Opens a specified door.";

		protected override void OnTargetFound(global::Interactables.Interobjects.DoorUtils.DoorVariant door)
		{
			door.TargetState = true;
		}
	}
}
