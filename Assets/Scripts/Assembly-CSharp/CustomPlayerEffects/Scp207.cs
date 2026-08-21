namespace CustomPlayerEffects
{
    public class Scp207 : global::CustomPlayerEffects.StatusEffectBase, global::CustomPlayerEffects.ISpectatorDataPlayerEffect, global::CustomPlayerEffects.IHealablePlayerEffect, global::PlayerRoles.FirstPersonControl.IMovementSpeedModifier, global::PlayerRoles.FirstPersonControl.IStaminaModifier, global::RemoteAdmin.Interfaces.ICustomRADisplay
    {
        [global::System.Serializable]
        public class NumberOfDrinks
        {
            public float DamageMultiplier;

            public float SpeedMultiplier;

            public float PostProcessIntensity;
        }

        public global::CustomPlayerEffects.Scp207.NumberOfDrinks[] numberOfDrinks;

        public float baseSpeedMultiplier = 1.2f;

        private float _damageCounter;

        private global::CustomPlayerEffects.Scp207.NumberOfDrinks _currentDrink;

        private global::SCPE.SpeedLines _speedLines;

        public override global::CustomPlayerEffects.StatusEffectBase.EffectClassification Classification => global::CustomPlayerEffects.StatusEffectBase.EffectClassification.Mixed;

        public string DisplayName => "SCP-207";

        public bool CanBeDisplayed => true;

        public bool MovementModifierActive => base.IsEnabled;

        public bool StaminaModifierActive => base.IsEnabled;

        public float MovementSpeedMultiplier => baseSpeedMultiplier * _currentDrink.SpeedMultiplier;

        public float MovementSpeedLimit => float.MaxValue;

        public float StaminaUsageMultiplier => 0f;

        public float StaminaRegenMultiplier => 1f;

        public bool SprintingDisabled => false;

        public bool GetSpectatorText(out string s)
        {
            s = ((base.Intensity > 1) ? $"SCP-207 (x{base.Intensity})" : "SCP-207");
            return true;
        }

        public bool IsHealable(ItemType it)
        {
            return it == ItemType.SCP500;
        }

        protected override void OnAwake()
        {
            _currentDrink = numberOfDrinks[0];

            var component = GetComponent<global::UnityEngine.Rendering.PostProcessing.PostProcessVolume>();
            if (component != null)
            {
                var profile = component.profile;
                _speedLines = profile?.GetSetting<global::SCPE.SpeedLines>();
            }
        }

        protected override void IntensityChanged(byte prevState, byte newState)
        {
            _currentDrink = numberOfDrinks[global::UnityEngine.Mathf.Clamp(newState, 0, numberOfDrinks.Length - 1)];

            if (_speedLines != null)
            {
                _speedLines.intensity.value = _currentDrink.PostProcessIntensity;
            }
        }

        protected override void Enabled()
        {
            base.Enabled();
            if (global::Mirror.NetworkServer.active && base.Hub.playerStats.TryGetModule<global::PlayerStatsSystem.StaminaStat>(out var module))
            {
                module.CurValue = module.MaxValue;
            }
        }

        protected override void OnEffectUpdate()
        {
            if (global::Mirror.NetworkServer.active && !global::CustomPlayerEffects.Vitality.CheckPlayer(base.Hub))
            {
                _damageCounter += global::UnityEngine.Time.deltaTime;
                if (!(_damageCounter < 1f))
                {
                    _damageCounter -= 1f;
                    float num = global::PlayerRoles.FirstPersonControl.FpcExtensionMethods.GetVelocity(base.Hub).SqrMagnitudeIgnoreY();
                    float num2 = ((num <= 2f) ? 0.1f : ((num <= 5f) ? 0.15f : ((!(num <= 50f)) ? 1f : 0.4f)));
                    num2 *= _currentDrink.DamageMultiplier * global::CustomPlayerEffects.RainbowTaste.CurrentMultiplier(base.Hub);
                    base.Hub.playerStats.DealDamage(new global::PlayerStatsSystem.UniversalDamageHandler(num2, global::PlayerStatsSystem.DeathTranslations.Scp207));
                }
            }
        }
    }
}
