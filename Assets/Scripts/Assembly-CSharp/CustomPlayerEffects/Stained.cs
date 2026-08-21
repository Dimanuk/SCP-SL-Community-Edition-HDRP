namespace CustomPlayerEffects
{
    public class Stained : global::CustomPlayerEffects.StatusEffectBase, global::PlayerRoles.FirstPersonControl.IStaminaModifier, global::PlayerRoles.FirstPersonControl.IMovementSpeedModifier, global::CustomPlayerEffects.IFootstepEffect
    {
        [global::UnityEngine.SerializeField]
        private global::UnityEngine.AudioClip[] _stainedFootsteps;

        [global::UnityEngine.SerializeField]
        private float _originalLoudness;

        private const float SpeedMultiplier = 0.8f;

        public bool MovementModifierActive => base.IsEnabled;

        public float MovementSpeedMultiplier => 0.8f;

        public float MovementSpeedLimit => float.MaxValue;

        public bool StaminaModifierActive => base.IsEnabled;

        public float StaminaUsageMultiplier => 1f;

        public float StaminaRegenMultiplier => 1f;

        public bool SprintingDisabled => true;

        public float ProcessFootstepOverrides(float dis)
        {
            global::AudioPooling.AudioSourcePoolManager.PlaySound(_stainedFootsteps.RandomItem(), base.transform, dis);
            return _originalLoudness;
        }
    }
}
