namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class ShowTagCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "showtag";

		public string[] Aliases { get; }

		public string Description { get; } = "Shows your local tag.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!(sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender))
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			ServerRoles serverRoles = playerCommandSender.ReferenceHub.serverRoles;
			serverRoles.HiddenBadge = null;
			serverRoles.GlobalHidden = false;
			serverRoles.RpcResetFixed();
			serverRoles.RefreshPermissions(disp: true);
			response = "Local tag refreshed!";
			return true;
		}
	}
}
