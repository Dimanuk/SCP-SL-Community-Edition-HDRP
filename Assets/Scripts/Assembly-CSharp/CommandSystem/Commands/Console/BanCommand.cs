namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class BanCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "ban";

		public string[] Aliases { get; }

		public string Description { get; } = "Ban specified player from server.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!ReferenceHub.TryGetHostHub(out var hub) || !hub.isLocalPlayer)
			{
				response = "You are not connected to a local server.";
				return false;
			}
			if (arguments.Count < 2)
			{
				global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
				stringBuilder.AppendLine("Syntax: BAN [player name / ip] [minutes or time]");
				foreach (global::Mirror.NetworkConnectionToClient value in global::Mirror.NetworkServer.connections.Values)
				{
					string text = string.Empty;
					global::UnityEngine.GameObject gameObject = global::GameCore.Console.FindConnectedRoot(value);
					if (gameObject != null)
					{
						text = gameObject.GetComponent<NicknameSync>().MyNick;
					}
					if (text == string.Empty)
					{
						stringBuilder.AppendLine("Player :: " + value.address);
					}
					else
					{
						stringBuilder.AppendLine("Player :: " + text + " :: " + value.address);
					}
				}
				response = stringBuilder.ToString();
				return true;
			}
			bool flag = false;
			long duration;
			try
			{
				duration = Misc.RelativeTimeToSeconds(arguments.At(1), 60);
			}
			catch
			{
				response = "Invalid time: " + arguments.At(1);
				return false;
			}
			foreach (global::Mirror.NetworkConnectionToClient value2 in global::Mirror.NetworkServer.connections.Values)
			{
				global::UnityEngine.GameObject gameObject2 = global::GameCore.Console.FindConnectedRoot(value2);
				if (global::NorthwoodLib.StringUtils.Contains(value2.address, arguments.At(0), global::System.StringComparison.OrdinalIgnoreCase) || (!(gameObject2 == null) && global::NorthwoodLib.StringUtils.Contains(gameObject2.GetComponent<NicknameSync>().MyNick, arguments.At(0), global::System.StringComparison.OrdinalIgnoreCase)))
				{
					flag = true;
					BanPlayer.BanUser(ReferenceHub.GetHub(gameObject2), sender, string.Empty, duration);
				}
			}
			response = (flag ? "Player banned." : "Player not found.");
			return flag;
		}
	}
}
