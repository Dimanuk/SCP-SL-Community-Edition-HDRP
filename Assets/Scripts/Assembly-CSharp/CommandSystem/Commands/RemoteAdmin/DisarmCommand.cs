namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class DisarmCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "disarm";

		public string[] Aliases { get; } = new string[1] { "da" };

		public string Description { get; } = "Force disarming player(s), no matter if they have been cuffed before. They can be released only with RELEASE command.";

		public string[] Usage { get; } = new string[1] { "%player%" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			if (list == null)
			{
				response = "An unexpected problem has occurred during PlayerId or name array processing.";
				return false;
			}
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			int num = 0;
			foreach (ReferenceHub item in list)
			{
				if (!(item == null))
				{
					global::InventorySystem.Disarming.DisarmedPlayers.SetDisarmedStatus(item.inventory, null);
					global::InventorySystem.Disarming.DisarmedPlayers.Entries.Add(new global::InventorySystem.Disarming.DisarmedPlayers.DisarmedEntry(item.networkIdentity.netId, 0u));
					global::Utils.Networking.NetworkUtils.SendToAuthenticated(new global::InventorySystem.Disarming.DisarmedPlayersListMessage(global::InventorySystem.Disarming.DisarmedPlayers.Entries));
					if (num != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(item.LoggedNameFromRefHub());
					num++;
				}
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} administratively disarmed {stringBuilder}.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
