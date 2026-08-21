namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class OverwatchCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "overwatch";

		public string[] Aliases { get; } = new string[1] { "ovr" };

		public string Description { get; } = "Changes the status of overwatch mode for the specified player(s).";

		public string[] Usage { get; } = new string[2] { "%player%", "enable/disable (Leave blank for toggle)" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Overwatch, out response))
			{
				return false;
			}
			string[] newargs = new string[0];
			global::System.Collections.Generic.List<ReferenceHub> list;
			if (sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender && (arguments.Count == 0 || (arguments.Count == 1 && !arguments.At(0).Contains(".") && !arguments.At(0).Contains("@"))))
			{
				list = new global::System.Collections.Generic.List<ReferenceHub>();
				list.Add(playerCommandSender.ReferenceHub);
				if (arguments.Count > 1)
				{
					newargs[0] = arguments.At(1);
				}
				else
				{
					newargs = null;
				}
			}
			else
			{
				list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			}
			if (!Misc.TryCommandModeFromArgs(ref newargs, out var mode))
			{
				response = "Invalid option " + newargs[0] + " - leave null for toggle or use 1/0, true/false, enable/disable or on/off.";
				return false;
			}
			global::System.Text.StringBuilder stringBuilder = ((mode == Misc.CommandOperationMode.Toggle) ? null : global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent());
			global::RemoteAdmin.PlayerCommandSender playerCommandSender2 = (global::RemoteAdmin.PlayerCommandSender)sender;
			bool flag = playerCommandSender2.ServerRoles.Staff || playerCommandSender2.CheckPermission(PlayerPermissions.PermissionsManagement);
			int num = 0;
			if (list != null)
			{
				foreach (ReferenceHub item in list)
				{
					ServerRoles serverRoles = item.serverRoles;
					if (serverRoles.BypassStaff && !flag)
					{
						continue;
					}
					switch (mode)
					{
					case Misc.CommandOperationMode.Enable:
						if (serverRoles.IsInOverwatch)
						{
							continue;
						}
						serverRoles.SetOverwatchStatus(1);
						if (num != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(item.LoggedNameFromRefHub());
						break;
					case Misc.CommandOperationMode.Disable:
						if (!serverRoles.IsInOverwatch)
						{
							continue;
						}
						serverRoles.SetOverwatchStatus(0);
						if (num != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(item.LoggedNameFromRefHub());
						break;
					case Misc.CommandOperationMode.Toggle:
						serverRoles.SetOverwatchStatus(2);
						if (serverRoles.IsInOverwatch)
						{
							ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " enabled overwatch mode for player " + item.LoggedNameFromRefHub() + " using overwatch mode toggle command.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
						}
						else
						{
							ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disabled overwatch mode for player " + item.LoggedNameFromRefHub() + " using overwatch mode toggle command.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
						}
						break;
					}
					num++;
				}
			}
			if (num > 0)
			{
				switch (mode)
				{
				case Misc.CommandOperationMode.Enable:
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} enabled overwatch mode for player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
					global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
					break;
				case Misc.CommandOperationMode.Disable:
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} disabled overwatch mode for player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
					global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
					break;
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
