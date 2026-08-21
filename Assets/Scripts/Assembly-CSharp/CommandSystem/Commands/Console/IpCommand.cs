namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class IpCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "ip";

		public string[] Aliases { get; } = new string[2] { "whatismyip", "myip" };

		public string Description { get; } = "Returns the IP used when connecting to central servers";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			bool success;
			global::System.Net.HttpStatusCode code;
			string text = HttpQuery.Get(CentralServer.StandardUrl + "ip.php", out success, out code);
			if (!success)
			{
				response = $"HTTP request failed: {code}";
				return false;
			}
			response = "IP: " + (text.EndsWith(".") ? text.Remove(text.Length - 1) : text);
			return true;
		}
	}
}
