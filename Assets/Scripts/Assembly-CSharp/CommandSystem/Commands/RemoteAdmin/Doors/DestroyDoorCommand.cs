namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class DestroyDoorCommand : global::CommandSystem.Commands.RemoteAdmin.Doors.BaseDoorCommand
	{
		public override string Command { get; } = "destroy";

		public override string[] Aliases { get; } = new string[1] { "destroydoor" };

		public override string Description { get; } = "Destroys a specified door.";

		public override bool AllowNonDamageableTargets => false;

		protected override void OnTargetFound(global::Interactables.Interobjects.DoorUtils.DoorVariant door)
		{
			(door as global::Interactables.Interobjects.DoorUtils.IDamageableDoor).ServerDamage(65535f, global::Interactables.Interobjects.DoorUtils.DoorDamageType.ServerCommand);
		}
	}
}
