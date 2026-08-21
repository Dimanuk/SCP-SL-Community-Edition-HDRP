namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class UptimeRoundsCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "uptime";

		public string[] Aliases { get; } = new string[1] { "rounds" };

		public string Description { get; } = "Displays the uptime of the game.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = $"Server uptime: {(global::UnityEngine.Time.unscaledTime)} seconds, {(global::RoundRestarting.RoundRestart.UptimeRounds)} rounds.";
			return true;
		}
	}
}
