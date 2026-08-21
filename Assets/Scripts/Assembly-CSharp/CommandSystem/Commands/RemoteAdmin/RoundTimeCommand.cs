namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class RoundTimeCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "roundtime";

		public string[] Aliases { get; } = new string[2] { "rtime", "rt" };

		public string Description { get; } = "Displays the current round duration.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (global::GameCore.RoundStart.RoundLength.Ticks == 0L)
			{
				response = "The round has not started yet!";
				return false;
			}
			response = "Round time: " + global::GameCore.RoundStart.RoundLength.ToString("hh\\:mm\\:ss\\.fff", global::System.Globalization.CultureInfo.InvariantCulture);
			return true;
		}
	}
}
