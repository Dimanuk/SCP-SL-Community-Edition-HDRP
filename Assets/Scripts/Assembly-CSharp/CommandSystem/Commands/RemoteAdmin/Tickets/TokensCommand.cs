namespace CommandSystem.Commands.RemoteAdmin.Tickets
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class TokensCommand : ParentCommand, global::CommandSystem.IUsageProvider
	{
		public override string Command { get; } = "tickets";

		public override string[] Aliases { get; } = new string[1] { "tix" };

		public override string Description { get; } = "Reads or sets the amount of NTF/CI respawn tokens.";

		public string[] Usage { get; } = new string[2] { "NTF/CI/Fetch/Info", "Value (Optional)" };

		public static global::CommandSystem.Commands.RemoteAdmin.Tickets.TokensCommand Create()
		{
			global::CommandSystem.Commands.RemoteAdmin.Tickets.TokensCommand tokensCommand = new global::CommandSystem.Commands.RemoteAdmin.Tickets.TokensCommand();
			tokensCommand.LoadGeneratedCommands();
			return tokensCommand;
		}

		protected override bool ExecuteParent(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			float num = global::UnityEngine.Mathf.Round(global::Respawning.RespawnTokensManager.GetTeamDominance(global::Respawning.SpawnableTeamType.NineTailedFox) * 100000f) * 0.001f;
			response = $"Domination: <color=#4179D6>MTF: {num}%</color> - <color=#3DB735>CI: {100f - num}%</color>";
			return false;
		}

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Tickets.GrantCommand());
			RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Tickets.InfoCommand());
		}
	}
}
