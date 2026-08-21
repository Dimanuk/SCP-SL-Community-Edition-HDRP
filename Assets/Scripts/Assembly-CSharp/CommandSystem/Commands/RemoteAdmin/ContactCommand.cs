namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class ContactCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "contact";

		public string[] Aliases { get; }

		public string Description { get; } = "Return the contact email address of the server.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "Contact email address: " + global::GameCore.ConfigFile.ServerConfig.GetString("contact_email");
			return true;
		}
	}
}
