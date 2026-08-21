namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class CassieWordsCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "cassiewords";

		public string[] Aliases { get; }

		public string Description { get; } = "Lists CASSIE words.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			bool flag = false;
			global::System.Text.RegularExpressions.Regex regex = null;
			if (arguments.Count > 0)
			{
				regex = new global::System.Text.RegularExpressions.Regex(arguments.FirstElement(), global::System.Text.RegularExpressions.RegexOptions.IgnoreCase);
				flag = true;
			}
			response = "CASSIE words: " + (flag ? string.Join(" ", global::System.Linq.Enumerable.Where(global::System.Linq.Enumerable.Select(NineTailedFoxAnnouncer.singleton.voiceLines, (NineTailedFoxAnnouncer.VoiceLine line) => line.apiName), (string line) => regex.IsMatch(line))) : string.Join(" ", global::System.Linq.Enumerable.Select(NineTailedFoxAnnouncer.singleton.voiceLines, (NineTailedFoxAnnouncer.VoiceLine line) => line.apiName)));
			return true;
		}
	}
}
