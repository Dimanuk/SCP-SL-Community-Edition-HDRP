namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class VersionCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "version";

		public string[] Aliases { get; }

		public string Description { get; } = "Returns the version of the server.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "Server Version: " + global::GameCore.Version.VersionString + " " + global::UnityEngine.Application.buildGUID;
			return true;
		}
	}
}
