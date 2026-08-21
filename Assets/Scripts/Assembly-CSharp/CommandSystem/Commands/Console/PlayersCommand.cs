namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class PlayersCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "players";

		public string[] Aliases { get; } = new string[2] { "pl", "list" };

		public string Description { get; } = "Displays a list of all players.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (ReferenceHub.LocalHub == null)
			{
				response = "You must join a server to execute this command.";
				return false;
			}
			global::System.Collections.Generic.HashSet<ReferenceHub> allHubs = ReferenceHub.AllHubs;
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
			stringBuilder.AppendLine($"<color=cyan>List of players ({(ServerStatic.IsDedicated ? (allHubs.Count - 1) : allHubs.Count)}):</color>");
			foreach (ReferenceHub item in allHubs)
			{
				if (item.Mode != ClientInstanceMode.DedicatedServer)
				{
					stringBuilder.AppendLine(string.Format("- {0}: {1} [{2}]", item.nicknameSync.CombinedName ?? "(no nickname)", item.characterClassManager.UserId ?? "(no User ID)", item.PlayerId));
				}
			}
			response = stringBuilder.ToString();
			return true;
		}
	}
}
