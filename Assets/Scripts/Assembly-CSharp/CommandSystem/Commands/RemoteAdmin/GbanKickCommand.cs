namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class GbanKickCommand : global::CommandSystem.ICommand, global::CommandSystem.IHiddenCommand
	{
		public string Command { get; } = "gban-kick";

		public string[] Aliases { get; }

		public string Description { get; } = "Internal global banning use only.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!(sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender))
			{
				response = "This command can only be executed by players in-game.";
				return false;
			}
			if (!playerCommandSender.ServerRoles.RaEverywhere && !playerCommandSender.ServerRoles.Staff)
			{
				response = "You do not have permission to run this command. Did you mean \"ban\"?";
				return false;
			}
			if (arguments.Count != 1)
			{
				response = "To run this program, type exactly 1 argument!.";
				return false;
			}
			global::System.Collections.Generic.List<int> list = Misc.ProcessRaPlayersList(arguments.At(0));
			if (list != null && list.Count == 1)
			{
				if (ReferenceHub.TryGetHub(list[0], out var hub))
				{
					BanPlayer.GlobalBanUser(hub, playerCommandSender);
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " globally banned and kicked " + arguments.At(0) + " player.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
					response = "Done! Globally banned and kicked " + arguments.At(0) + ".";
					return true;
				}
				response = "Error finding player with that ID.";
				return false;
			}
			response = "An unexpected problem has occurred during PlayerId processing. (This command only accepts one ID).";
			return false;
		}
	}
}
