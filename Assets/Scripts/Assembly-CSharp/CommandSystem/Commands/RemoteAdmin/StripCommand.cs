namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class StripCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "strip";

		public string[] Aliases { get; } = new string[1] { "clear" };

		public string Description { get; } = "Clears the specified player(s) inventory.";

		public string[] Usage { get; } = new string[1] { "%player%" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count >= 1)
			{
				string[] newargs;
				global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
				int num = 0;
				int num2 = 0;
				string arg = "";
				if (list != null)
				{
					foreach (ReferenceHub item in list)
					{
						try
						{
							global::InventorySystem.InventoryInfo userInventory = item.inventory.UserInventory;
							while (userInventory.Items.Count > 0)
							{
								global::InventorySystem.InventoryExtensions.ServerRemoveItem(item.inventory, global::System.Linq.Enumerable.ElementAt(userInventory.Items, 0).Key, null);
							}
							userInventory.ReserveAmmo.Clear();
							ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " cleared " + item.LoggedNameFromRefHub() + "'s inventory.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
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
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} ran the STRIP command on {list.Count} players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
				response = ((num == 0) ? string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!") : $"Failed to execute the command! Failures: {num}\nLast error log:\n {arg}");
				return true;
			}
			response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
			return false;
		}
	}
}
