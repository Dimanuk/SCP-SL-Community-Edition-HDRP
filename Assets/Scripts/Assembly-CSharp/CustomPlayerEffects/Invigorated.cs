namespace CustomPlayerEffects
{
    public class Invigorated : global::CustomPlayerEffects.StatusEffectBase, global::PlayerRoles.FirstPersonControl.IStaminaModifier
    {
        public override global::CustomPlayerEffects.StatusEffectBase.EffectClassification Classification => global::CustomPlayerEffects.StatusEffectBase.EffectClassification.Positive;

        public bool StaminaModifierActive => base.IsEnabled;

        public float StaminaUsageMultiplier => 0f;

        public float StaminaRegenMultiplier => 1f;

        public bool SprintingDisabled => false;
    }
}
