namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class RidListCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "ridlist";

		public string[] Aliases { get; } = new string[1] { "rids" };

		public string Description { get; } = "Displays a list of all room ids.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			global::MapGeneration.RoomIdentifier[] array = global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Where(global::MapGeneration.RoomIdentifier.AllRoomIdentifiers, (global::MapGeneration.RoomIdentifier x) => x.Name != global::MapGeneration.RoomName.Unnamed));
			if (array.Length == 0)
			{
				response = "There are no rooms!";
				return false;
			}
			global::System.Collections.Generic.Dictionary<global::MapGeneration.RoomName, int> dictionary = new global::System.Collections.Generic.Dictionary<global::MapGeneration.RoomName, int>();
			global::MapGeneration.RoomIdentifier[] array2 = array;
			foreach (global::MapGeneration.RoomIdentifier roomIdentifier in array2)
			{
				if (dictionary.TryGetValue(roomIdentifier.Name, out var value))
				{
					dictionary[roomIdentifier.Name] = value + 1;
				}
				else
				{
					dictionary.Add(roomIdentifier.Name, 1);
				}
			}
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			stringBuilder.Append("--- RID List ---\n");
			foreach (global::System.Collections.Generic.KeyValuePair<global::MapGeneration.RoomName, int> item in global::System.Linq.Enumerable.OrderBy(dictionary, (global::System.Collections.Generic.KeyValuePair<global::MapGeneration.RoomName, int> a) => a.Key))
			{
				stringBuilder.Append("- ");
				stringBuilder.Append(item.Key);
				stringBuilder.Append(" (");
				stringBuilder.Append(item.Value);
				stringBuilder.Append(")\n");
			}
			response = stringBuilder.ToString().TrimEnd();
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			return true;
		}
	}
}
