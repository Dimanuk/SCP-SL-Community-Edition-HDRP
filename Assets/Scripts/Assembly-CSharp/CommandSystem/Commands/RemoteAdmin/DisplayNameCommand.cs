namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class DisplayNameCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "name";

		public string[] Aliases { get; } = new string[2] { "nickname", "nick" };

		public string Description { get; } = "Displays player's name.";

		public string[] Usage { get; } = new string[1] { "%player%" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			if (list == null)
			{
				response = "Cannot find player! Try using the player ID!";
				return false;
			}
			if (list.Count == 1)
			{
				response = "Player " + arguments.At(0) + "'s name is: " + list[0].nicknameSync.CombinedName;
				return true;
			}
			response = global::System.Linq.Enumerable.Aggregate(list, "Display names of specified players:\n", (string current, ReferenceHub hub) => current + " - " + hub.nicknameSync.CombinedName + "\"");
			return true;
		}
	}
}
