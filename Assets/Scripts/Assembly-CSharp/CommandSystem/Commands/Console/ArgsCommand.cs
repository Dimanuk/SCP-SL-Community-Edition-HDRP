namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class ArgsCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "args";

		public string[] Aliases { get; }

		public string Description { get; } = "Prints all startup args";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = global::System.Linq.Enumerable.Aggregate(StartupArgs.Args, "Startup args:\n", (string current, string arg) => current + "- " + arg + "\n");
			return true;
		}
	}
}
