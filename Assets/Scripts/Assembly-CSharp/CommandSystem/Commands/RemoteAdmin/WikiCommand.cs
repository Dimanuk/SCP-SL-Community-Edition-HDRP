namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class WikiCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "wiki";

		public string[] Aliases { get; }

		public string Description { get; } = "Opens RA help page on the game wiki.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "Wiki page has been opened.";
			return true;
		}
	}
}
