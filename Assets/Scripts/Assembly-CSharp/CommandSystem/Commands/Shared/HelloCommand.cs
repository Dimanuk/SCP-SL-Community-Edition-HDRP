namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class HelloCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "hello";

		public string[] Aliases { get; }

		public string Description { get; } = "Hi!";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "Hello World!";
			return true;
		}
	}
}
