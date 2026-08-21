namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class PingCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "ping";

		public string[] Aliases { get; }

		public string Description { get; } = "Pong!";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			switch (arguments.Count)
			{
			case 0:
			{
				if (!(sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender))
				{
					response = "This command is only available for players!";
					return false;
				}
				int connectionId2 = playerCommandSender.ReferenceHub.networkIdentity.connectionToClient.connectionId;
				if (connectionId2 == 0)
				{
					response = "This command is not available for the host!";
					return false;
				}
				response = $"Your ping: {(global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorServer.Peers[connectionId2].Ping * 2)}ms";
				return true;
			}
			case 1:
			{
				if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
				{
					return false;
				}
				if (!int.TryParse(arguments.At(0), out var result))
				{
					response = "Invalid player id!";
					return false;
				}
				if (!ReferenceHub.TryGetHub(result, out var hub))
				{
					response = "Invalid player id!";
					return false;
				}
				int connectionId = hub.networkIdentity.connectionToClient.connectionId;
				if (connectionId == 0)
				{
					response = "This command is not available for the host!";
					return false;
				}
				response = $"Ping: {(global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorServer.Peers[connectionId].Ping * 2)}ms";
				return true;
			}
			default:
				response = "Too many arguments! (expected 0 or 1)";
				return false;
			}
		}
	}
}
