namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class ContactCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "contact";

		public string[] Aliases { get; }

		public string Description { get; } = "Retrieves the server host's email.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			ReferenceHub localHub = ReferenceHub.LocalHub;
			if (localHub == null)
			{
				response = "You must join a server to execute this command.";
				return false;
			}
			response = "Requesting server host contact email...";
			localHub.characterClassManager.CmdRequestContactEmail();
			return true;
		}
	}
}
