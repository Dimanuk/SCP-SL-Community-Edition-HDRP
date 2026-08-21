namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class DoorsListCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "doorslist";

		public string[] Aliases { get; } = new string[2] { "doors", "dl" };

		public string Description { get; } = "Lists all valid door names.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			string text = "List of named doors in the facility:\n";
			global::System.Collections.Generic.List<string> list = global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Where(global::System.Linq.Enumerable.ToArray(global::Interactables.Interobjects.DoorUtils.DoorNametagExtension.NamedDoors.Values), (global::Interactables.Interobjects.DoorUtils.DoorNametagExtension item) => !string.IsNullOrEmpty(item.GetName)), (global::Interactables.Interobjects.DoorUtils.DoorNametagExtension item) => item.GetName + " - " + (item.TargetDoor.TargetState ? "<color=green>OPENED</color>" : "<color=orange>CLOSED</color>") + ((item.TargetDoor.ActiveLocks > 0) ? " <color=red>[LOCKED]</color>" : "") + (((item.TargetDoor is global::Interactables.Interobjects.BasicDoor basicDoor && (int)basicDoor.RequiredPermissions.RequiredPermissions > 0) || (item.TargetDoor is global::Interactables.Interobjects.CheckpointDoor checkpointDoor && (int)checkpointDoor.RequiredPermissions.RequiredPermissions > 0)) ? " <color=blue>[CARD REQUIRED]</color>" : "")));
			list.Sort();
			text += global::System.Linq.Enumerable.Aggregate(list, (string current, string adding) => current + "\n" + adding);
			response = text;
			return true;
		}
	}
}
