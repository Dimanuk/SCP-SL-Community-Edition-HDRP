namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class NoclipCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "noclip";

		public string[] Aliases { get; } = new string[2] { "n", "nc" };

		public string Description => "Toggles noclip on the selected player(s). Press the Left Alt key (by default) to toggle, use scroll to change speed.";

		public string[] Usage { get; } = new string[2] { "%player%", "enable/disable (Leave blank for toggle)" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Noclip, out response))
			{
				return false;
			}
			string[] newargs = new string[0];
			global::System.Collections.Generic.List<ReferenceHub> list;
			if (sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender && (arguments.Count == 0 || (arguments.Count == 1 && !arguments.At(0).Contains(".") && !arguments.At(0).Contains("@"))))
			{
				list = new global::System.Collections.Generic.List<ReferenceHub>();
				list.Add(playerCommandSender.ReferenceHub);
				if (arguments.Count > 0)
				{
					newargs[0] = arguments.At(0);
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
			int num = 0;
			if (list != null)
			{
				foreach (ReferenceHub item in list)
				{
					ServerRoles serverRoles = item.serverRoles;
					switch (mode)
					{
					case Misc.CommandOperationMode.Enable:
						if (global::PlayerRoles.FirstPersonControl.FpcNoclip.IsPermitted(item))
						{
							continue;
						}
						global::PlayerRoles.FirstPersonControl.FpcNoclip.PermitPlayer(item);
						if (num != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(item.LoggedNameFromRefHub());
						break;
					case Misc.CommandOperationMode.Disable:
						if (!global::PlayerRoles.FirstPersonControl.FpcNoclip.IsPermitted(item))
						{
							continue;
						}
						global::PlayerRoles.FirstPersonControl.FpcNoclip.UnpermitPlayer(item);
						if (num != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(item.LoggedNameFromRefHub());
						break;
					case Misc.CommandOperationMode.Toggle:
						if (global::PlayerRoles.FirstPersonControl.FpcNoclip.IsPermitted(item))
						{
							global::PlayerRoles.FirstPersonControl.FpcNoclip.UnpermitPlayer(item);
						}
						else
						{
							global::PlayerRoles.FirstPersonControl.FpcNoclip.PermitPlayer(item);
						}
						if (serverRoles.BypassMode)
						{
							ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " enabled noclip for player " + item.LoggedNameFromRefHub() + " using noclip toggle command.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
						}
						else
						{
							ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disabled noclip for player " + item.LoggedNameFromRefHub() + " using noclip toggle command.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
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
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} enabled noclip for player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
					global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
					break;
				case Misc.CommandOperationMode.Disable:
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} disabled noclip for player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
					global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
					break;
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
