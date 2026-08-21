namespace CommandSystem.Commands.RemoteAdmin.Inventory
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class RemoveItemCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "removeitem";

		public string[] Aliases { get; }

		public string Description { get; } = "Remove the specified item from the player(s) inventory.";

		public string[] Usage { get; } = new string[2] { "%player%", "%item%" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
			{
				return false;
			}
			if (arguments.Count >= 2)
			{
				string[] newargs;
				global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
				if (newargs == null || newargs.Length == 0)
				{
					response = "You must specify item(s) to give.";
					return false;
				}
				ItemType[] array = global::System.Linq.Enumerable.ToArray(ParseItems(newargs[0]));
				if (array.Length == 0)
				{
					response = "You didn't input any items.";
					return false;
				}
				int num = 0;
				int num2 = 0;
				string arg = string.Empty;
				if (list != null)
				{
					foreach (ReferenceHub item in list)
					{
						try
						{
							ItemType[] array2 = array;
							foreach (ItemType id in array2)
							{
								RemoveItem(item, sender, id);
							}
						}
						catch (global::System.Exception ex)
						{
							num++;
							arg = ex.Message;
							continue;
						}
						num2++;
					}
				}
				response = ((num == 0) ? string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!") : $"Failed to execute the command! Failures: {num}\nLast error log:\n{arg}");
				return true;
			}
			response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
			return false;
		}

		private global::System.Collections.Generic.IEnumerable<ItemType> ParseItems(string argument)
		{
			string[] array = argument.Split('.');
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				if (int.TryParse(array2[i], out var result) && global::System.Enum.IsDefined(typeof(ItemType), result))
				{
					yield return (ItemType)result;
				}
			}
		}

		private void RemoveItem(ReferenceHub ply, global::CommandSystem.ICommandSender sender, ItemType id)
		{
			global::System.Collections.Generic.KeyValuePair<ushort, global::InventorySystem.Items.ItemBase> keyValuePair = global::System.Linq.Enumerable.FirstOrDefault(ply.inventory.UserInventory.Items, (global::System.Collections.Generic.KeyValuePair<ushort, global::InventorySystem.Items.ItemBase> i) => i.Value.ItemTypeId == id);
			if (!(keyValuePair.Value == null))
			{
				global::InventorySystem.InventoryExtensions.ServerRemoveItem(ply.inventory, keyValuePair.Key, null);
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} removed item {id} from {ply.LoggedNameFromRefHub()}.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			}
		}
	}
}
