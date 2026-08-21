using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
    public class Burned : StatusEffectBase, IHealablePlayerEffect, IDamageModifierEffect
    {
        private TiltShiftWave _tiltShiftOffsetWave;
        private bool _initialized;

        public float DamageMultiplier => 0.25f * RainbowTaste.CurrentMultiplier(Hub) + 1f;

        public override bool AllowEnabling => Hub == null || !Vitality.CheckPlayer(Hub);

        public bool IsHealable(ItemType it)
        {
            return it == ItemType.Medkit || it == ItemType.SCP500;
        }

        public float GetDamageModifier(float baseDamage, DamageHandlerBase handler, HitboxType hitboxType)
        {
            return DamageMultiplier;
        }

        protected override void OnAwake()
        {
            if (Hub.isLocalPlayer)
            {
                _tiltShiftOffsetWave = GetComponent<TiltShiftWave>();
                _initialized = true;
            }
        }

        protected override void Enabled()
        {
            if (Hub.isLocalPlayer && _initialized)
            {
                _tiltShiftOffsetWave.EnableEffect();
            }
        }

        protected override void Disabled()
        {
            if (Hub.isLocalPlayer && _initialized && _tiltShiftOffsetWave.enabled)
            {
                _tiltShiftOffsetWave.DisableEffect();
            }
        }
    }
}
