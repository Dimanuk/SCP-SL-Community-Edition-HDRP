namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class HideTagCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "hidetag";

		public string[] Aliases { get; }

		public string Description { get; } = "Hides your tag.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!(sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender))
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			ServerRoles serverRoles = playerCommandSender.ReferenceHub.serverRoles;
			if (!serverRoles.BypassStaff)
			{
				if (!string.IsNullOrEmpty(serverRoles.HiddenBadge))
				{
					response = "Your badge is already hidden.";
					return false;
				}
				if (string.IsNullOrEmpty(serverRoles.MyText))
				{
					response = "Your don't have any badge.";
					return false;
				}
			}
			serverRoles.GlobalHidden = serverRoles.GlobalSet;
			serverRoles.HiddenBadge = serverRoles.MyText;
			serverRoles.GlobalBadge = null;
			serverRoles.SetText(null);
			serverRoles.SetColor(null);
			serverRoles.RefreshHiddenTag();
			response = "Tag hidden!";
			return true;
		}
	}
}
