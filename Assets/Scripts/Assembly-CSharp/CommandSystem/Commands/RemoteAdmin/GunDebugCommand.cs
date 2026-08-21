namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class GunDebugCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "gundebug";

		public string[] Aliases { get; } = new string[3] { "gunsdebug", "weaponsdebug", "weapondebug" };

		public string Description { get; } = "Toggles debug mode for firearms.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(new PlayerPermissions[1] { PlayerPermissions.FacilityManagement }, out response))
			{
				return false;
			}
			global::InventorySystem.Items.Firearms.Modules.StandardHitregBase.DebugMode = !global::InventorySystem.Items.Firearms.Modules.StandardHitregBase.DebugMode;
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " toggled firearms debug mode.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
			return true;
		}
	}
}
