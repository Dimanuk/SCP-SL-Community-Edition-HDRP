namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class GlobalTagCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "globaltag";

		public string[] Aliases { get; } = new string[1] { "gtag" };

		public string Description { get; } = "Shows your global tag.";

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
			serverRoles.GlobalBadge = serverRoles.PrevBadge;
			response = "Global tag refreshed!";
			return true;
		}
	}
}
