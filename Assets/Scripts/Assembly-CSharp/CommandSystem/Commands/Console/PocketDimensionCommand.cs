namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class PocketDimensionCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "pocketdimension";

		public string[] Aliases { get; } = new string[3] { "pocketdim", "shadowrealm", "pd" };

		public string Description { get; } = "Banishes yourself to the pocket dimension.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!global::Mirror.NetworkServer.active || !ReferenceHub.TryGetHostHub(out var hub))
			{
				response = "You are not connected to a local server.";
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			CharacterClassManager characterClassManager = hub.characterClassManager;
			if (characterClassManager == null || !characterClassManager.isLocalPlayer || !characterClassManager.isServer || !characterClassManager.RoundStarted)
			{
				response = "Please start round before using this command.";
				return false;
			}
			if (sender != null && sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender)
			{
				ReferenceHub referenceHub = playerCommandSender.ReferenceHub;
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " teleported to the Pocket Dimension.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
				referenceHub.playerEffectsController.EnableEffect<global::CustomPlayerEffects.Corroding>();
				response = "You banished yourself to the Pocket Dimension!";
				return true;
			}
			response = "Only players can run this command.";
			return false;
		}
	}
}
