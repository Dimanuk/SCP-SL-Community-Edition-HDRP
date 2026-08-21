
using PlayerRoles.FirstPersonControl;
using RelativePositioning;
using UnityEngine;

namespace CustomPlayerEffects
{
	public class Corroding : TickingEffectBase, IFootstepEffect, IMovementSpeedModifier, IStaminaModifier
	{
		private const float ActivationHeight = -1998.5f;

		private const float DeactivationHeight = -1800f;

		[SerializeField]
		private float _startingDamage = 1f;

		[SerializeField]
		private AudioClip[] _footstepSounds;

		[SerializeField]
		private float _originalLoudness;

		private float _damagePerTick = 1f;

        public global::RelativePositioning.RelativePosition CapturePosition { get; private set; }

        public bool MovementModifierActive => base.IsEnabled;

        public bool StaminaModifierActive => base.IsEnabled;

        public float StaminaUsageMultiplier => 1f;

        public float MovementSpeedMultiplier => 1f;

        public float StaminaRegenMultiplier => 1f;

        public bool SprintingDisabled => true;

        public float MovementSpeedLimit => float.MaxValue;

        protected override void OnTick()
        {
            if (global::Mirror.NetworkServer.active)
            {
                if (base.Hub.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole && fpcRole.FpcModule.Position.y > -1800f)
                {
                    ServerDisable();
                    return;
                }
                base.Hub.playerStats.DealDamage(new global::PlayerStatsSystem.UniversalDamageHandler(_damagePerTick, global::PlayerStatsSystem.DeathTranslations.PocketDecay));
                _damagePerTick += 0.1f;
            }
        }

        protected override void Enabled()
        {
            if (global::Mirror.NetworkServer.active)
            {
                _damagePerTick = _startingDamage;
                if (base.Hub.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole)
                {
                    CapturePosition = new global::RelativePositioning.RelativePosition(fpcRole.FpcModule.Position);
                    fpcRole.FpcModule.ServerOverridePosition(global::UnityEngine.Vector3.up * -1998.5f, global::UnityEngine.Vector3.zero);
                }
            }
        }

        public float ProcessFootstepOverrides(float dis)
        {
            global::AudioPooling.AudioSourcePoolManager.PlaySound(_footstepSounds.RandomItem(), base.transform, dis);
            return _originalLoudness;
        }
	}
}
