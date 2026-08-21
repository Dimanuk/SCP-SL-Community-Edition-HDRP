namespace CustomPlayerEffects
{
    public class Deafened : global::CustomPlayerEffects.StatusEffectBase, global::CustomPlayerEffects.IHealablePlayerEffect
    {
        public bool IsHealable(ItemType it)
        {
            return it == ItemType.SCP500;
        }

        private void OnDestroy()
        {
            base.Hub.playerEffectsController.mixer.SetFloat("MasterVolumeLowpassFreq", 22000f);
        }
    }
}
