namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class KillCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "kill";

		public string[] Aliases { get; }

		public string Description { get; } = "Kills the specified player(s).";

		public string[] Usage { get; } = new string[1] { "PlayerId" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = $"To execute this command provide at least 1 argument!\nUsage: {arguments.Array[0]} {Usage}";
				return false;
			}
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			int num = 0;
			if (list != null)
			{
				foreach (ReferenceHub item in list)
				{
					if (item != null && item.playerStats.DealDamage(new global::PlayerStatsSystem.UniversalDamageHandler(-1f, global::PlayerStatsSystem.DeathTranslations.Unknown)))
					{
						if (num != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(item.LoggedNameFromRefHub());
						num++;
					}
				}
			}
			if (num > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} administratively killed player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			}
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
