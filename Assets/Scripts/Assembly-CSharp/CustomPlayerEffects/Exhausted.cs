namespace CustomPlayerEffects
{
    public class Exhausted : global::CustomPlayerEffects.StatusEffectBase, global::PlayerRoles.FirstPersonControl.IStaminaModifier
    {
        private const float MaxStamina = 0.5f;

        private const float StaminaRegenSpeed = 0.5f;

        private global::PlayerStatsSystem.StaminaStat _staminaCache;

        private bool _cacheSet;

        public override bool AllowEnabling => !global::CustomPlayerEffects.Vitality.CheckPlayer(base.Hub);

        public bool StaminaModifierActive => base.IsEnabled;

        public float StaminaUsageMultiplier => 1f;

        public bool SprintingDisabled => false;

        public float StaminaRegenMultiplier
        {
            get
            {
                if (!(CurStamina < 0.5f))
                {
                    return 0f;
                }
                return 0.5f;
            }
        }

        private float CurStamina
        {
            get
            {
                PrepCache();
                return _staminaCache.CurValue;
            }
            set
            {
                PrepCache();
                _staminaCache.CurValue = value;
            }
        }

        private void PrepCache()
        {
            if (!_cacheSet)
            {
                _cacheSet = true;
                _staminaCache = base.Hub.playerStats.GetModule<global::PlayerStatsSystem.StaminaStat>();
            }
        }

        protected override void Enabled()
        {
            base.Enabled();
            CurStamina = global::UnityEngine.Mathf.Min(CurStamina, 0.5f);
        }
    }
}
