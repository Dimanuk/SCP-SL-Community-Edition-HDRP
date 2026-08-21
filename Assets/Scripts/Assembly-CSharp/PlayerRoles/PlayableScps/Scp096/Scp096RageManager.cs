using Mirror;
using PlayerRoles.PlayableScps.HumeShield;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp096
{
    public class Scp096RageManager : ScpStandardSubroutine<Scp096Role>, IHumeShieldBlocker
    {
        public const float NormalHumeRegenerationRate = 15f;
        public const float MaxRageTime = 35f;
        public const float MinimumEnrageTime = 20f;
        
        private const float TimePerExtraTarget = 3f;
        private const float CalmingShieldMultiplier = 2f;
        private const float EnragingShieldMultiplier = 0.5f;

        public readonly AbilityCooldown HudRageDuration = new AbilityCooldown();

        private DynamicHumeShieldController _shieldController;
        private Scp096TargetsTracker _targetsTracker;
        private float _enragedTimeLeft;

        public bool HumeShieldBlocked { get; set; }

        public bool IsEnragedOrDistressed
        {
            get
            {
                if (!IsEnraged)
                {
                    return IsDistressed;
                }
                return true;
            }
        }

        public bool IsEnraged => base.ScpRole.IsRageState(Scp096RageState.Enraged);

        public bool IsDistressed => base.ScpRole.IsRageState(Scp096RageState.Distressed);

        public float EnragedTimeLeft
        {
            get
            {
                return _enragedTimeLeft;
            }
            set
            {
                if (value < 0f)
                {
                    value = 0f;
                }
                HudRageDuration.Remaining = value;
                _enragedTimeLeft = value;
                if (NetworkServer.active && _enragedTimeLeft == 0f)
                {
                    ServerEndEnrage(clearTime: false);
                }
            }
        }

        public float TotalRageTime { get; private set; }

        public void ServerEnrage(float initialDuration = 20f)
        {
            if (NetworkServer.active)
            {
                base.Role.TryGetOwner(out var hub);

                    EnragedTimeLeft = initialDuration;
                    TotalRageTime = initialDuration;
                    base.ScpRole.StateController.SetRageState(Scp096RageState.Distressed);
                    ServerIncreaseDuration(Mathf.Max((float)_targetsTracker.Targets.Count - 3f, 0f));

            }
        }

        public void ServerEndEnrage(bool clearTime = true)
        {
            if (NetworkServer.active)
            {
                if (clearTime)
                {
                    EnragedTimeLeft = 0f;
                }
                base.ScpRole.StateController.SetRageState(Scp096RageState.Calming);
                ServerSendRpc(toAll: true);
            }
        }

        public void ServerIncreaseDuration(float addedDuration = 3f)
        {
            if (NetworkServer.active)
            {
                addedDuration = Mathf.Min(addedDuration, MaxRageTime - TotalRageTime);
                TotalRageTime += addedDuration;
                EnragedTimeLeft += addedDuration;
                ServerSendRpc(toAll: true);
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            NetworkWriterExtensions.WriteFloat(writer, EnragedTimeLeft);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            if (!NetworkServer.active)
            {
                EnragedTimeLeft = NetworkReaderExtensions.ReadFloat(reader);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _shieldController = base.ScpRole.HumeShieldModule as DynamicHumeShieldController;
            GetSubroutine(out _targetsTracker);
            _targetsTracker.OnTargetAdded += delegate(ReferenceHub h)
            {
                ServerIncreaseDuration(TimePerExtraTarget);
            };
            base.ScpRole.StateController.OnRageUpdate += OnRageUpdate;
        }

        private void OnRageUpdate(Scp096RageState newState)
        {
            if (newState == Scp096RageState.Enraged)
            {
                HudRageDuration.Trigger(EnragedTimeLeft);
            }

            // Blocker bookkeeping must run on clients too — IsBlocked drives the red
            // warning blink of the shield bar (HsWarningColor), which the local 096
            // otherwise only sees when playing on a listen-server host.
            if (newState == Scp096RageState.Enraged)
            {
                HumeShieldBlocked = true;
                _shieldController.AddBlocker(this);
            }
            else
            {
                HumeShieldBlocked = false;
            }

            if (!NetworkServer.active)
            {
                return;
            }

            float multiplier;
            switch (newState)
            {
                case Scp096RageState.Enraged:
                    multiplier = EnragingShieldMultiplier;
                    break;
                case Scp096RageState.Calming:
                    multiplier = CalmingShieldMultiplier;
                    TotalRageTime = 0f;
                    break;
                default:
                    return;
            }

            HumeShieldModuleBase humeShieldModule = base.ScpRole.HumeShieldModule;
            humeShieldModule.HsCurrent = Mathf.Clamp(humeShieldModule.HsCurrent * multiplier, 0f, humeShieldModule.HsMax);
        }

        private void Update()
        {
            if (NetworkServer.active)
            {
                UpdateRage();
            }
        }

        private void UpdateRage()
        {
            if (IsEnraged)
            {
                EnragedTimeLeft -= Time.deltaTime;
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            HudRageDuration.Clear();
            _shieldController.RegenerationRate = NormalHumeRegenerationRate;
            _enragedTimeLeft = 0f;
            TotalRageTime = 0f;
            HumeShieldBlocked = false;
        }
    }
}