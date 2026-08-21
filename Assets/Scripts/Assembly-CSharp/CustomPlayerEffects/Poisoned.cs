using Mirror;
using UnityEngine;

namespace CustomPlayerEffects
{
    public class Poisoned : TickingEffectBase, IHealablePlayerEffect, IPulseEffect
    {
        private float damagePerTick = 2f;

        private readonly float minDamage = 2f;
        private readonly float maxDamage = 20f;
        private readonly float multPerTick = 2f;

        private VignettePulse _vignettePulse;

        public override bool AllowEnabling => !Vitality.CheckPlayer(Hub);

        protected override void OnAwake()
        {
            _vignettePulse = GetComponent <VignettePulse>();
        }

        public void ExecutePulse()
        {
            if (_vignettePulse != null)
                _vignettePulse.enabled = true;
        }

        public bool IsHealable(ItemType it)
        {
            return it == ItemType.SCP500;
        }

        protected override void OnTick()
        {
            if (!NetworkServer.active || Hub == null)
                return;

            Hub.playerStats.DealDamage(new global::PlayerStatsSystem.UniversalDamageHandler(damagePerTick, global::PlayerStatsSystem.DeathTranslations.Poisoned));
            Hub.playerEffectsController?.ServerSendPulse<Poisoned>();

            damagePerTick *= multPerTick;
            damagePerTick = Mathf.Clamp(damagePerTick, minDamage, maxDamage);
        }

        protected override void Enabled()
        {
            if (NetworkServer.active)
            {
                damagePerTick = minDamage;
            }
        }
    }
}