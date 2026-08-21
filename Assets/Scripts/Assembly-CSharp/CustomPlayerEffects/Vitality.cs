namespace CustomPlayerEffects
{
    public class Vitality : global::CustomPlayerEffects.StatusEffectBase, global::CustomPlayerEffects.ISpectatorDataPlayerEffect
    {
        public override global::CustomPlayerEffects.StatusEffectBase.EffectClassification Classification => global::CustomPlayerEffects.StatusEffectBase.EffectClassification.Positive;

        public bool GetSpectatorText(out string s)
        {
            s = "Vitality";
            return true;
        }

        public static bool CheckPlayer(ReferenceHub ply)
        {
            if (ply != null)
            {
                return ply.playerEffectsController.GetEffect<global::CustomPlayerEffects.Vitality>().IsEnabled;
            }
            return false;
        }
    }
}
