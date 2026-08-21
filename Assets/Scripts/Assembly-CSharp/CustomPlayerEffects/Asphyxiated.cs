namespace CustomPlayerEffects
{
    public class Asphyxiated : global::CustomPlayerEffects.TickingEffectBase, global::PlayerRoles.FirstPersonControl.IStaminaModifier
    {
        public float staminaDrainPerTick = 5f;

        public float healthDrainPerTick = 2f;

        private global::PlayerStatsSystem.StaminaStat _stamina;

        public override bool AllowEnabling => !global::CustomPlayerEffects.Vitality.CheckPlayer(base.Hub);

        public bool StaminaModifierActive => base.IsEnabled;

        public float StaminaUsageMultiplier => 1f;

        public float StaminaRegenMultiplier => 0f;

        public bool SprintingDisabled => false;

        protected override void Enabled()
        {
            base.Enabled();
            _stamina = base.Hub.playerStats.GetModule<global::PlayerStatsSystem.StaminaStat>();
        }

        protected override void OnTick()
        {
            if (global::Mirror.NetworkServer.active)
            {
                _stamina.CurValue = global::UnityEngine.Mathf.Clamp01(_stamina.CurValue - staminaDrainPerTick * 0.01f);
                if (_stamina.CurValue <= 0f)
                {
                    float damage = healthDrainPerTick * global::CustomPlayerEffects.RainbowTaste.CurrentMultiplier(base.Hub);
                    base.Hub.playerStats.DealDamage(new global::PlayerStatsSystem.UniversalDamageHandler(damage, global::PlayerStatsSystem.DeathTranslations.Asphyxiated));
                }
            }
        }
    }
}
