using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Ripples
{
    public class TeslaRippleTrigger : RippleTriggerBase
    {
        private const float CooldownDuration = 0.7f;
        private const float IdleRangeSqr = 120f;
        private const float BurstRangeSqr = 2400f;

        private static readonly Vector3 PosOffset = Vector3.up * 1.35f;

        private readonly AbilityCooldown _cooldown = new AbilityCooldown();

        public override void SpawnObject()
        {
            base.SpawnObject();
            TeslaGate.OnBursted += OnTeslaBursted;
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _cooldown.Clear();
            TeslaGate.OnBursted -= OnTeslaBursted;
        }

        private void OnTeslaBursted(TeslaGate tg)
        {
            if (!base.IsLocalOrSpectated)
                return;

            PlayInRange(tg.transform.position + PosOffset, BurstRangeSqr, Color.red);
        }

        private void Update()
        {
            if (!base.IsLocalOrSpectated || !_cooldown.IsReady)
                return;

            _cooldown.Trigger(CooldownDuration);

            foreach (TeslaGate teslaGate in TeslaGateController.Singleton.TeslaGates)
            {
                if (teslaGate.isIdling)
                {
                    PlayInRange(teslaGate.transform.position + PosOffset, IdleRangeSqr, Color.red);
                }
            }
        }
    }
}
