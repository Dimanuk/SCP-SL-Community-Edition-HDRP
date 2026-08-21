namespace CustomPlayerEffects
{
    public class DamageReduction : global::CustomPlayerEffects.StatusEffectBase, global::CustomPlayerEffects.ISpectatorDataPlayerEffect, global::CustomPlayerEffects.IDamageModifierEffect
    {
        private float CurrentMultiplier => 1f - (float)(int)base.Intensity * 0.005f;

        public override global::CustomPlayerEffects.StatusEffectBase.EffectClassification Classification => global::CustomPlayerEffects.StatusEffectBase.EffectClassification.Positive;

        public bool GetSpectatorText(out string s)
        {
            s = $"Damage Reduction (All, -{(global::UnityEngine.Mathf.Round((1f - CurrentMultiplier) * 1000f) / 10f)}%)";
            return base.IsEnabled;
        }

        public float GetDamageModifier(float baseDamage, global::PlayerStatsSystem.DamageHandlerBase handler, HitboxType hitboxType)
        {
            return CurrentMultiplier;
        }
    }
}
