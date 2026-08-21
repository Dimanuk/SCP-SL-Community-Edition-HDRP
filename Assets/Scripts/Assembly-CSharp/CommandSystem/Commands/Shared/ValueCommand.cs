namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.Shared.ConfigCommand))]
	public class ValueCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "value";

		public string[] Aliases { get; } = new string[1] { "val" };

		public string Description { get; } = "Returns the value of specified config key";

		public string[] Usage { get; } = new string[1] { "Key" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConfigs, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				response = "Please specify the config key!";
				return false;
			}
			if (arguments.At(0).Equals("administrator_query_password", global::System.StringComparison.InvariantCultureIgnoreCase))
			{
				response = "You can't read value of this config key for security reasons.";
				return false;
			}
			if (global::GameCore.ConfigFile.ServerConfig.TryGetString(arguments.At(0), out var value))
			{
				response = "The value of <i>'" + arguments.At(0) + "'</i> is: " + value;
				return true;
			}
			response = "Key <i>'" + arguments.At(0) + "'</i> is not present in the config!";
			return false;
		}
	}
}
