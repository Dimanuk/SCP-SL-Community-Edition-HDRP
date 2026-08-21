namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class AddCandyCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "addcandy";

		public string[] Aliases { get; } = new string[2] { "candy", "trickortreat" };

		public string Description { get; } = "Adds specific candies to player(s) bags. If a player does not have a bag, it will create a new one with a random candy.";

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
					response = "Could not process provided arguments.";
					return false;
				}
				string text = (newargs[0].ToUpper()[0] + newargs[0].ToLower()).Remove(1, 1);
				if (!global::System.Enum.TryParse<global::InventorySystem.Items.Usables.Scp330.CandyKindID>(text, out var result))
				{
					response = newargs[0] + " could not be parsed as candy type";
					return false;
				}
				if (!global::System.Enum.IsDefined(typeof(global::InventorySystem.Items.Usables.Scp330.CandyKindID), result) || result == global::InventorySystem.Items.Usables.Scp330.CandyKindID.None)
				{
					response = text + " isn't a valid candy type!";
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
							ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} gave {result} candy to {item.LoggedNameFromRefHub()}.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
							bool flag = false;
							if (global::InventorySystem.Items.Usables.Scp330.Scp330Bag.TryGetBag(item, out var bag))
							{
								goto IL_0123;
							}
							flag = true;
							global::InventorySystem.InventoryExtensions.ServerAddItem(item.inventory, ItemType.SCP330, 0);
							if (global::InventorySystem.Items.Usables.Scp330.Scp330Bag.TryGetBag(item, out bag))
							{
								goto IL_0123;
							}
							goto end_IL_00cf;
							IL_0123:
							if (flag)
							{
								bag.Candies = new global::System.Collections.Generic.List<global::InventorySystem.Items.Usables.Scp330.CandyKindID> { result };
								bag.ServerRefreshBag();
								num2++;
							}
							else if (bag.TryAddSpecific(result))
							{
								bag.ServerRefreshBag();
								num2++;
							}
							end_IL_00cf:;
						}
						catch (global::System.Exception ex)
						{
							num++;
							arg = ex.Message;
						}
					}
				}
				response = ((num == 0) ? (response = string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!")) : $"Failed to execute the command! Failures: {num}\nLast error log:\n{arg}");
				return true;
			}
			response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
			return false;
		}
	}
}
