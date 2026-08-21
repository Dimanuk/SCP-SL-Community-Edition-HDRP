namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class HealCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "heal";

		public string[] Aliases { get; }

		public string Description { get; } = "Heals player(s) a specified amount.";

		public string[] Usage { get; } = new string[2] { "%player%", "Amount (0 = full)" };

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
			int num = ((newargs != null && newargs.Length != 0 && int.TryParse(newargs[0], out num)) ? num : 0);
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			int num2 = 0;
			if (list != null)
			{
				foreach (ReferenceHub item in list)
				{
					if (item != null)
					{
						global::PlayerStatsSystem.HealthStat module = item.playerStats.GetModule<global::PlayerStatsSystem.HealthStat>();
						if (num > 0)
						{
							module.ServerHeal(num);
						}
						else
						{
							module.CurValue = module.MaxValue;
						}
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
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} healed player{1}{2}.", sender.LogName, (num2 == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			}
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!");
			return true;
		}
	}
}
