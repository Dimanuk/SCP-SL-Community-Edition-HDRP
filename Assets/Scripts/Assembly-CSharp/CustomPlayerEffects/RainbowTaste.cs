namespace CustomPlayerEffects
{
    public class RainbowTaste : global::CustomPlayerEffects.StatusEffectBase, global::CustomPlayerEffects.ISpectatorDataPlayerEffect
    {
        private static readonly float[] Multipliers = new float[4] { 1f, 0.6f, 0.4f, 0.35f };

        public override global::CustomPlayerEffects.StatusEffectBase.EffectClassification Classification => global::CustomPlayerEffects.StatusEffectBase.EffectClassification.Positive;

        public bool GetSpectatorText(out string s)
        {
            s = "Rainbow Taste";
            return base.IsEnabled;
        }

        public static float CurrentMultiplier(ReferenceHub ply)
        {
            byte intensity = ply.playerEffectsController.GetEffect<global::CustomPlayerEffects.RainbowTaste>().Intensity;
            return Multipliers[global::UnityEngine.Mathf.Clamp(intensity, 0, Multipliers.Length - 1)];
        }

        public static bool CheckPlayer(ReferenceHub ply)
        {
            return ply.playerEffectsController.GetEffect<global::CustomPlayerEffects.RainbowTaste>().Intensity > 0;
        }
    }
}
