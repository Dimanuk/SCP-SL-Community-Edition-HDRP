namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class SetHealthCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "hp";

		public string[] Aliases { get; } = new string[2] { "sethealth", "sethp" };

		public string Description { get; } = "Sets the player(s) health to the specified amount.";

		public string[] Usage { get; } = new string[2] { "%player%", "Amount" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			int num = (int.TryParse(newargs[0], out num) ? num : 0);
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			int num2 = 0;
			if (list != null)
			{
				foreach (ReferenceHub item in list)
				{
					if (item != null)
					{
						global::PlayerStatsSystem.HealthStat module = item.playerStats.GetModule<global::PlayerStatsSystem.HealthStat>();
						module.CurValue = ((num > 0) ? ((float)num) : module.MaxValue);
						if (num2 != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(item.LoggedNameFromRefHub());
						num2++;
					}
				}
			}
			if (num2 > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} set health of player{1}{2} to {3}.", sender.LogName, (num2 == 1) ? " " : "s ", stringBuilder, num), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			}
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!");
			return true;
		}
	}
}
