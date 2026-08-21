namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class AddExperienceCommand : global::CommandSystem.Commands.RemoteAdmin.Scp079CommandBase
	{
		public override string Command { get; } = "addexperience";

		public override string[] Aliases { get; } = new string[3] { "addexp", "add079exp", "addxp" };

		public override string Description { get; } = "Adds the specified experience of the player playing as SCP-079.";

		public override string[] Usage { get; } = new string[2] { "%player%", "Experience" };

		public override void ApplyChanges(global::PlayerRoles.PlayableScps.Scp079.Scp079TierManager manager, int input)
		{
			manager.ServerGrantExperience(input, global::PlayerRoles.PlayableScps.Scp079.Scp079HudTranslation.ExpGainAdminCommand);
		}
	}
}
