namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class IdCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "id";

		public string[] Aliases { get; } = new string[1] { "myid" };

		public string Description { get; } = "Displays your current player ID.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			ReferenceHub localHub = ReferenceHub.LocalHub;
			if (localHub == null)
			{
				response = "You must join a server to execute this command.";
				return false;
			}
			response = $"Your Player ID on the current server: {localHub.PlayerId}";
			return true;
		}
	}
}
