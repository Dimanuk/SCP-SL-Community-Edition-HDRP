using Hazards;
using Mirror;
using PlayerRoles.PlayableScps.HumeShield;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerStatsSystem;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp173
{
    public class Scp173TantrumAbility : ScpKeySubroutine<Scp173Role>
    {
        private const float StainedKillReward = 400f;
        private const float CooldownTime = 30f;
        private const float RayMaxDistance = 3f;
        private const float TantrumHeight = 1.25f;

        private Scp173ObserversTracker _observersTracker;

        [SerializeField]
        private TantrumEnvironmentalHazard _tantrumPrefab;

        [SerializeField]
        private LayerMask _tantrumMask;

        public readonly AbilityCooldown Cooldown = new AbilityCooldown();

        protected override ActionName TargetKey => ActionName.ToggleFlashlight;

        protected override void OnKeyDown()
        {
            base.OnKeyDown();
            ClientSendCmd();
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _observersTracker);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            if (!Cooldown.IsReady || _observersTracker.IsObserved)
                return;

            if (!Physics.Raycast(base.ScpRole.FpcModule.Position, Vector3.down, out var hitInfo, RayMaxDistance, _tantrumMask))
                return;


            Cooldown.Trigger(CooldownTime);
            ServerSendRpc(toAll: true);

            TantrumEnvironmentalHazard tantrum = Object.Instantiate(_tantrumPrefab);
            Vector3 targetPos = hitInfo.point + Vector3.up * TantrumHeight;
            tantrum.SynchronizedPosition = new RelativePosition(targetPos);
            NetworkServer.Spawn(tantrum.gameObject);

            foreach (TeslaGate teslaGate in TeslaGateController.Singleton.TeslaGates)
            {
                if (teslaGate.PlayerInHurtRange(base.Owner.gameObject))
                    teslaGate.TantrumsToBeDestroyed.Add(tantrum);
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            Cooldown.WriteCooldown(writer);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            Cooldown.ReadCooldown(reader);
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            PlayerStats.OnAnyPlayerDied += CheckDeath;
        }

        public override void ResetObject()
        {
            base.ResetObject();
            Cooldown.Clear();
            PlayerStats.OnAnyPlayerDied -= CheckDeath;
        }

        private void CheckDeath(ReferenceHub ply, DamageHandlerBase handler)
        {
            if (!NetworkServer.active || !(handler is ScpDamageHandler scpDamageHandler))
                return;

            if (scpDamageHandler.Attacker.Hub != base.Owner)
                return;

            if (!ply.playerEffectsController.TryGetEffect<CustomPlayerEffects.Stained>(out var effect) || !effect.IsEnabled)
                return;

            HumeShieldModuleBase humeShield = base.ScpRole.HumeShieldModule;
            humeShield.HsCurrent = Mathf.Min(humeShield.HsMax, humeShield.HsCurrent + StainedKillReward);
        }
    }
}