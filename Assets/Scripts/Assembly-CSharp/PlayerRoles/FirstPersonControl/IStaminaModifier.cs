namespace PlayerRoles.FirstPersonControl
{
	public interface IStaminaModifier
	{
		bool StaminaModifierActive { get; }

		float StaminaUsageMultiplier { get; }

		float StaminaRegenMultiplier { get; }

		bool SprintingDisabled { get; }
	}
}
