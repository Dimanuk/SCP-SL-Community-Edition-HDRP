namespace CustomPlayerEffects
{
    public class Sinkhole : global::CustomPlayerEffects.StatusEffectBase, global::PlayerRoles.FirstPersonControl.IStaminaModifier, global::PlayerRoles.FirstPersonControl.IMovementSpeedModifier, global::CustomPlayerEffects.IFootstepEffect
    {
        [global::UnityEngine.SerializeField]
        [global::UnityEngine.Range(0f, 100f)]
        private float _slowAmount;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.AudioClip[] _footstepSounds;

        [global::UnityEngine.SerializeField]
        private float _originalLoudness;

        public bool MovementModifierActive => base.IsEnabled;

        public float MovementSpeedMultiplier => 1f - _slowAmount * 0.01f;

        public float MovementSpeedLimit => float.MaxValue;

        public bool StaminaModifierActive => base.IsEnabled;

        public float StaminaUsageMultiplier => 1f;

        public float StaminaRegenMultiplier => 1f;

        public bool SprintingDisabled => true;

        public float ProcessFootstepOverrides(float dis)
        {
            global::AudioPooling.AudioSourcePoolManager.PlaySound(_footstepSounds.RandomItem(), base.transform, dis);
            return _originalLoudness;
        }
    }
}
