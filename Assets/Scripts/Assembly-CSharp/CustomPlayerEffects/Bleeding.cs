using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
    public class Bleeding : TickingEffectBase, IPulseEffect, IHealablePlayerEffect
    {
        public float minDamage = 2f;
        public float maxDamage = 20f;
        public float multPerTick = 0.5f;
        public float damagePerTick = 20f;

        private VignettePulse _vignettePulse;

        public override bool AllowEnabling => Hub == null || !Vitality.CheckPlayer(Hub);

        protected override void OnAwake()
        {
            _vignettePulse = GetComponent<VignettePulse>();
        }

        public void ExecutePulse()
        {
            _vignettePulse.enabled = true;
        }

        protected override void OnTick()
        {
            if (NetworkServer.active)
            {
                float damage = damagePerTick * RainbowTaste.CurrentMultiplier(Hub);
                Hub.playerStats.DealDamage(new UniversalDamageHandler(damage, DeathTranslations.Bleeding));
                Hub.playerEffectsController.ServerSendPulse<Bleeding>();

                damagePerTick *= multPerTick;
                damagePerTick = Mathf.Clamp(damagePerTick, minDamage, maxDamage);
            }
        }

        protected override void Enabled()
        {
            if (NetworkServer.active)
                damagePerTick = maxDamage;
        }

        public bool IsHealable(ItemType it)
        {
            if (it == ItemType.SCP500)
                damagePerTick = minDamage;

            return it == ItemType.Medkit;
        }
    }
}
