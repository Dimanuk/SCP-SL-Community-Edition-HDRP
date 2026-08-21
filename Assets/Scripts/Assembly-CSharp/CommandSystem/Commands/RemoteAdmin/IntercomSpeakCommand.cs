namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class IntercomSpeakCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "icom";

		public string[] Aliases { get; } = new string[1] { "speak" };

		public string Description { get; } = "Toggles global voice over the intercom.";

		public string[] Usage { get; } = new string[2] { "%player%", "enable/disable" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Broadcasting, out response))
			{
				return false;
			}
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			response = "Invalid argument, state was not defined.";
			if (newargs.Length == 0)
			{
				return false;
			}
			bool flag;
			switch (newargs[0].ToUpper())
			{
			case "ENABLED":
			case "ENABLE":
			case "TRUE":
			case "1":
				flag = true;
				break;
			case "DISABLED":
			case "DISABLE":
			case "FALSE":
			case "0":
				flag = false;
				break;
			default:
				return false;
			}
			int num = 0;
			foreach (ReferenceHub item in list)
			{
				if (global::PlayerRoles.Voice.Intercom.TrySetOverride(item, flag))
				{
					if (num != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(item.LoggedNameFromRefHub());
					num++;
				}
			}
			if (num > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} {1} global intercom transmission for player{2}{3}.", sender.LogName, flag ? "enabled" : "disabled", (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
			}
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
