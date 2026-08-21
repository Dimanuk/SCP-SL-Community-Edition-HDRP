namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class RoomTPCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "roomtp";

		public string[] Aliases { get; } = new string[2] { "rtp", "ridtp" };

		public string Description { get; } = "Teleports you to a room.";

		public string[] Usage { get; } = new string[1] { "RoomID" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!(sender is global::RemoteAdmin.PlayerCommandSender))
			{
				response = "Only players can run this command.";
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			if (!global::System.Enum.TryParse<global::MapGeneration.RoomName>(arguments.At(0), ignoreCase: true, out var result) || result == global::MapGeneration.RoomName.Unnamed)
			{
				response = "Room not defined.";
				return false;
			}
			global::System.Collections.Generic.List<global::UnityEngine.Vector3> list = global::NorthwoodLib.Pools.ListPool<global::UnityEngine.Vector3>.Shared.Rent();
			foreach (global::MapGeneration.RoomIdentifier rid in global::MapGeneration.RoomIdentifier.AllRoomIdentifiers)
			{
				if (rid.Name == result)
				{
					global::UnityEngine.Vector3 position = rid.transform.position;
					if (!global::Utils.NonAllocLINQ.HashsetExtensions.TryGetFirst(global::Interactables.Interobjects.DoorUtils.DoorVariant.AllDoors, (global::Interactables.Interobjects.DoorUtils.DoorVariant x) => x.Rooms.Contains(rid) && x is global::Interactables.Interobjects.BreakableDoor breakableDoor && !breakableDoor.IgnoreRemoteAdmin, out var first))
					{
						list.Add(position + global::UnityEngine.Vector3.up);
						continue;
					}
					global::UnityEngine.Vector3 position2 = first.transform.position;
					global::UnityEngine.Vector3 vector = (position - position2).NormalizeIgnoreY();
					list.Add(position2 + vector + global::UnityEngine.Vector3.up);
				}
			}
			if (list.Count == 0)
			{
				global::NorthwoodLib.Pools.ListPool<global::UnityEngine.Vector3>.Shared.Return(list);
				response = "Room couldn't be found.";
				return false;
			}
			global::UnityEngine.Vector3 position3 = list[global::UnityEngine.Random.Range(0, list.Count)];
			global::NorthwoodLib.Pools.ListPool<global::UnityEngine.Vector3>.Shared.Return(list);
			if (!(sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender) || !global::PlayerRoles.FirstPersonControl.FpcExtensionMethods.TryOverridePosition(playerCommandSender.ReferenceHub, position3, global::UnityEngine.Vector3.zero))
			{
				response = "Your current character role does not support this operation.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " teleported themself to room " + arguments.At(0) + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "You have been teleported.";
			return true;
		}
	}
}
