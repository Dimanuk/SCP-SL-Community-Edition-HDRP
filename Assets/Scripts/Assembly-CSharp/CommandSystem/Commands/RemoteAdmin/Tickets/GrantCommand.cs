namespace CommandSystem.Commands.RemoteAdmin.Tickets
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Tickets.TokensCommand))]
	public class GrantCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "grant";

		public string[] Aliases { get; }

		public string Description { get; } = "Grants a team a specific amount of tokens.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			response = "You must specify a valid team and token amount.";
			if (arguments.Count <= 1)
			{
				return false;
			}
			string text = arguments.At(0).ToUpper();
			if (!global::System.Enum.TryParse<global::Respawning.SpawnableTeamType>(text, ignoreCase: true, out var result))
			{
				switch (text)
				{
				case "CI":
				case "CHI":
				case "CHAOS":
					result = global::Respawning.SpawnableTeamType.ChaosInsurgency;
					break;
				case "NTF":
				case "MTF":
				case "MOBILE":
					result = global::Respawning.SpawnableTeamType.NineTailedFox;
					break;
				default:
					return false;
				}
			}
			if (!float.TryParse(arguments.At(1), out var result2))
			{
				return false;
			}
			float teamDominance = global::Respawning.RespawnTokensManager.GetTeamDominance(result);
			global::Respawning.RespawnTokensManager.ForceTeamDominance(result, result2 + teamDominance);
			response = $"Set the <color=yellow>{result}'s</color> tokens to <color=orange>{(global::Respawning.RespawnTokensManager.GetTeamDominance(result))}</color>. (Old value: <color=orange>{teamDominance}</color>)</color>";
			return true;
		}
	}
}
