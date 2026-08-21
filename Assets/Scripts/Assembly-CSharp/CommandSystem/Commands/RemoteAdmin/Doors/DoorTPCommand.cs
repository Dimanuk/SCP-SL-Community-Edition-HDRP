namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class DoorTPCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		private const float WallDetectionRadius = 0.35f;

		public string Command { get; } = "doortp";

		public string[] Aliases { get; } = new string[2] { "dtp", "doorteleport" };

		public string Description { get; } = "Teleports player(s) to the specified door.";

		public string[] Usage { get; } = new string[2] { "%player%", "%door%" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage() + " [Door name]";
				return false;
			}
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			if (newargs == null || string.IsNullOrEmpty(newargs[0]))
			{
				response = "Invalid door name provided.";
				return false;
			}
			string text = newargs[0].Split('.')[0].ToUpper();
			if (!global::Interactables.Interobjects.DoorUtils.DoorNametagExtension.NamedDoors.TryGetValue(text, out var value))
			{
				response = "Can't find door " + text + ".";
				return false;
			}
			global::UnityEngine.Vector3 position;
			for (position = value.transform.position + global::UnityEngine.Vector3.up; global::UnityEngine.Physics.CheckSphere(position, 0.35f, global::PlayerRoles.FirstPersonControl.FpcStateProcessor.Mask); position += value.transform.forward * 0.35f)
			{
			}
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			int num = 0;
			foreach (ReferenceHub item in list)
			{
				if (global::PlayerRoles.FirstPersonControl.FpcExtensionMethods.TryOverridePosition(item, position, global::UnityEngine.Vector3.zero))
				{
					if (num != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(item.LoggedNameFromRefHub());
					num++;
				}
			}
			if (num > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} teleported player{1}{2} to door {3}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder, newargs[0]), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			}
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
