namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class GroupsCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "groups";

		public string[] Aliases { get; }

		public string Description { get; } = "Displays all the server's groups.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			ReferenceHub localHub = ReferenceHub.LocalHub;
			if (localHub == null)
			{
				response = "You must join a server to execute this command.";
				return false;
			}
			response = "Requesting server groups...";
			localHub.characterClassManager.CmdRequestServerGroups();
			return true;
		}
	}
}
