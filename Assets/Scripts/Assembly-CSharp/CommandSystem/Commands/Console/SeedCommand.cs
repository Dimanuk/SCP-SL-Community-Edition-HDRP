namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class SeedCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "seed";

		public string[] Aliases { get; }

		public string Description { get; } = "Displays the seed for the current round.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = $"Map seed is: {(global::MapGeneration.SeedSynchronizer.Seed)}";
			return true;
		}
	}
}
