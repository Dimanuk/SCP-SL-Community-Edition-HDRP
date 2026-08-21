namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.ElevatorCommand))]
	public class ElevatorListCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "list";

		public string[] Aliases { get; } = new string[6] { "ls", "lst", "elevators", "lifts", "els", "elevs" };

		public string Description { get; } = "Lists all elevators.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			bool getLevels = arguments.Count > 0 && (arguments.At(0).Equals("detailed", global::System.StringComparison.OrdinalIgnoreCase) || arguments.At(0).Equals("d", global::System.StringComparison.OrdinalIgnoreCase) || arguments.At(0).Equals("det", global::System.StringComparison.OrdinalIgnoreCase));
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			try
			{
				bool result = true;
				stringBuilder.Append("Detected the following elevators:");
				foreach (global::Interactables.Interobjects.ElevatorManager.ElevatorGroup key in global::Interactables.Interobjects.ElevatorDoor.AllElevatorDoors.Keys)
				{
					stringBuilder.Append("\n- ");
					if (!GetElevatorData(key, getLevels, stringBuilder))
					{
						result = false;
					}
				}
				response = stringBuilder.ToString();
				return result;
			}
			finally
			{
				global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			}
		}

		private static bool GetElevatorData(global::Interactables.Interobjects.ElevatorManager.ElevatorGroup group, bool getLevels, global::System.Text.StringBuilder sb)
		{
			if (!global::Interactables.Interobjects.ElevatorDoor.AllElevatorDoors.TryGetValue(group, out var value))
			{
				sb.AppendFormat("Elevator \"{0}\" could not be found in the Facility.", group);
				return false;
			}
			sb.AppendFormat("Elevator \"{0}\" detected with {1} levels. Currently ", group, value.Count);
			global::Interactables.Interobjects.ElevatorDoor elevatorDoor = global::System.Linq.Enumerable.FirstOrDefault(value, (global::Interactables.Interobjects.ElevatorDoor x) => x.TargetState);
			if (elevatorDoor == null)
			{
				sb.Append("in transit");
			}
			else
			{
				sb.AppendFormat("at level {0}", value.IndexOf(elevatorDoor));
			}
			global::Interactables.Interobjects.ElevatorDoor elevatorDoor2 = global::System.Linq.Enumerable.FirstOrDefault(value);
			if (elevatorDoor2 != null)
			{
				if (global::Interactables.Interobjects.DoorUtils.DoorLockUtils.HasFlagFast(elevatorDoor2.TargetPanel.AssignedChamber.ActiveLocks, global::Interactables.Interobjects.DoorUtils.DoorLockReason.AdminCommand))
				{
					sb.Append(" and administratively locked");
				}
				else if (elevatorDoor2.TargetPanel.AssignedChamber.ActiveLocks != global::Interactables.Interobjects.DoorUtils.DoorLockReason.None)
				{
					sb.Append(" and locked");
				}
			}
			else
			{
				sb.Append(" (lock status unknown)");
			}
			sb.Append(".");
			if (!getLevels)
			{
				return true;
			}
			for (int num = 0; num < value.Count; num++)
			{
				global::UnityEngine.Vector3 position = value[num].transform.position;
				sb.AppendFormat("\n-   Level {0} at height {1}", num, global::UnityEngine.Mathf.Round(position.y));
				if (global::MapGeneration.RoomIdentifier.RoomsByCoordinates.TryGetValue(global::MapGeneration.RoomIdUtils.PositionToCoords(position), out var value2))
				{
					sb.AppendFormat(" (room: \"{0}\")", value2.Name);
				}
			}
			sb.Append('\n');
			return true;
		}
	}
}
