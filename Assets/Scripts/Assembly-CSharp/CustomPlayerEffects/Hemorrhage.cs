using Mirror;
using PlayerRoles.Spectating;
using UnityEngine;

namespace CustomPlayerEffects
{
    public class Hemorrhage : TickingEffectBase
    {
        public float damagePerTick = 1f;
        public float effectIncrease = 0.05f;

        private bool _isSprinting;
        private DiminishingLerpVisuals _diminishingPlayerEffect;

        public override bool AllowEnabling => !Vitality.CheckPlayer(Hub);

        protected override void OnAwake()
        {
            if (Hub != null && (Hub.isLocalPlayer || SpectatorNetworking.IsLocallySpectated(Hub)))
            {
                _diminishingPlayerEffect = GetComponent<DiminishingLerpVisuals>();
            }
        }

        protected override void OnTick()
        {
            if (!NetworkServer.active || !_isSprinting || Hub == null)
                return;

            float damage = damagePerTick * RainbowTaste.CurrentMultiplier(Hub);
            Hub.playerStats.DealDamage(new global::PlayerStatsSystem.UniversalDamageHandler(damage, global::PlayerStatsSystem.DeathTranslations.Bleeding));
        }

        protected override void OnEffectUpdate()
        {
            base.OnEffectUpdate();

            if (Hub?.roleManager?.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole)
            {
                _isSprinting = fpcRole.FpcModule.CurrentMovementState == global::PlayerRoles.FirstPersonControl.PlayerMovementState.Sprinting;
            }

            if (Hub != null && (Hub.isLocalPlayer || SpectatorNetworking.IsLocallySpectated(Hub)))
            {
                if (_diminishingPlayerEffect != null && _isSprinting)
                {
                    _diminishingPlayerEffect.Intensity += effectIncrease;
                }
            }
        }
    }
}