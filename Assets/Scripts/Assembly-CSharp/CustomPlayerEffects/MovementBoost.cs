namespace CustomPlayerEffects
{
    public class MovementBoost : global::CustomPlayerEffects.StatusEffectBase, global::PlayerRoles.FirstPersonControl.IMovementSpeedModifier, global::CustomPlayerEffects.ISpectatorDataPlayerEffect
    {
        public bool MovementModifierActive => base.IsEnabled;

        public float MovementSpeedMultiplier => (100f + (float)(int)base.Intensity) / 100f;

        public float MovementSpeedLimit => float.MaxValue;

        public override global::CustomPlayerEffects.StatusEffectBase.EffectClassification Classification => global::CustomPlayerEffects.StatusEffectBase.EffectClassification.Positive;

        public bool GetSpectatorText(out string s)
        {
            s = $"+{base.Intensity}% Movement Boost";
            return true;
        }
    }
}
