namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class KeyCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "key";

		public string[] Aliases { get; }

		public string Description { get; } = "Displays your encryption key.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			ReferenceHub localHub = ReferenceHub.LocalHub;
			if (localHub == null)
			{
				response = "You must join a server to execute this command.";
				return false;
			}
			byte[] encryptionKey = localHub.GetComponent<global::RemoteAdmin.RemoteAdminCryptographicManager>().EncryptionKey;
			if (encryptionKey == null)
			{
				response = "Encryption key: (null) - session not encrypted (probably due to online mode disabled).";
				return false;
			}
			response = "Encryption key (KEEP SECRET!): " + global::System.BitConverter.ToString(encryptionKey);
			return true;
		}
	}
}
