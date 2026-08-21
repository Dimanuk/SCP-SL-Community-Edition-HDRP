namespace CustomPlayerEffects
{
    public class BodyshotReduction : global::CustomPlayerEffects.StatusEffectBase, global::CustomPlayerEffects.ISpectatorDataPlayerEffect, global::CustomPlayerEffects.IDamageModifierEffect
    {
        private static readonly float[] Multipliers = new float[5] { 1f, 0.95f, 0.9f, 0.875f, 0.85f };

        public override global::CustomPlayerEffects.StatusEffectBase.EffectClassification Classification => global::CustomPlayerEffects.StatusEffectBase.EffectClassification.Positive;

        private float CurrentMultiplier => Multipliers[global::UnityEngine.Mathf.Min(base.Intensity, Multipliers.Length - 1)];

        public bool GetSpectatorText(out string s)
        {
            s = $"Damage Reduction (Body, -{(global::UnityEngine.Mathf.Round((1f - CurrentMultiplier) * 1000f) / 10f)}%)";
            return base.IsEnabled;
        }

        public float GetDamageModifier(float baseDamage, global::PlayerStatsSystem.DamageHandlerBase handler, HitboxType hitboxType)
        {
            if (hitboxType == HitboxType.Body)
            {
                return CurrentMultiplier;
            }
            return 1f;
        }
    }
}
