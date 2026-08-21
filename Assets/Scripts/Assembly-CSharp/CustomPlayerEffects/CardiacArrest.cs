using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
    public class CardiacArrest : ParentEffectBase<SubEffectBase>, IHealablePlayerEffect, PlayerRoles.FirstPersonControl.IStaminaModifier
    {
        private const float SprintStaminaUsage = 3f;
        private const float DamagePerTick = 8f;

        private Footprinting.Footprint _attacker;

        public float TimeBetweenTicks;
        private float _timeTillTick;

        public bool StaminaModifierActive => IsEnabled;
        public float StaminaUsageMultiplier => 3f;
        public float StaminaRegenMultiplier => 1f;
        public bool SprintingDisabled => false;

        protected override void Enabled()
        {
            _timeTillTick = 0f;
        }

        protected override void Disabled()
        {
            _attacker = default;
        }

        public void SetAttacker(ReferenceHub ply)
        {
            _attacker = new Footprinting.Footprint(ply);
        }

        public bool IsHealable(ItemType it)
        {
            return it == ItemType.SCP500 || it == ItemType.Adrenaline;
        }

        protected override void OnEffectUpdate()
        {
            if (NetworkServer.active)
            {
                ServerUpdate();
            }
            UpdateSubEffects();
        }

        private void ServerUpdate()
        {
            _timeTillTick -= Time.deltaTime;
            if (!(_timeTillTick > 0f))
            {
                _timeTillTick += TimeBetweenTicks;
                Hub.playerStats.DealDamage(new Scp049DamageHandler(_attacker, 8f, Scp049DamageHandler.AttackType.CardiacArrest));
            }
        }
    }
}
