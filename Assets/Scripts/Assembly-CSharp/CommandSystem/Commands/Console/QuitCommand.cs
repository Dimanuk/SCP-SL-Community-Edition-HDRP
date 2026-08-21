namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class QuitCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "quit";

		public string[] Aliases { get; } = new string[2] { "exit", "stop" };

		public string Description { get; } = "Quit the game.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			IdleMode.SetIdleMode(state: false);
			ServerConsole.AddOutputEntry(default(global::ServerOutput.ExitActionShutdownEntry));
			Shutdown.Quit();
			response = "<size=50>GOODBYE!</size>";
			return true;
		}
	}
}
