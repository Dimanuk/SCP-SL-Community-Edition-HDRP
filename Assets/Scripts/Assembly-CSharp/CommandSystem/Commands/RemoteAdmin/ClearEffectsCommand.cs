namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class ClearEffectsCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "cleareffects";

		public string[] Aliases { get; } = new string[2] { "cfx", "clearfx" };

		public string Description { get; } = "Clears all status effects from the specified player(s).";

		public string[] Usage { get; } = new string[1] { "%player%" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Effects, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				response = "To execute this command provide at least 1 arguments!\nUsage: " + Command + " " + string.Join(" ", Usage);
				return false;
			}
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			int num = 0;
			foreach (ReferenceHub item in list)
			{
				if (!(item == null))
				{
					global::CustomPlayerEffects.StatusEffectBase[] allEffects = item.playerEffectsController.AllEffects;
					for (int i = 0; i < allEffects.Length; i++)
					{
						allEffects[i].Intensity = 0;
					}
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " clear all effects for player " + item.LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
					num++;
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
