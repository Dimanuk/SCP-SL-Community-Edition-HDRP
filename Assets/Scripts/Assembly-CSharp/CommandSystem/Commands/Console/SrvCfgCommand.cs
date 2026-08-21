namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class SrvCfgCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "srvcfg";

		public string[] Aliases { get; }

		public string Description { get; } = "Displays the server config.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			ReferenceHub localHub = ReferenceHub.LocalHub;
			if (localHub == null)
			{
				response = "You must join a server to execute this command.";
				return false;
			}
			response = "Requesting server config...";
			localHub.characterClassManager.CmdRequestServerConfig();
			return true;
		}
	}
}
