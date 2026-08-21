using System;
using System.Collections.Generic;
using InventorySystem;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.Ragdolls;
using PlayerRoles.Spectating;

namespace PlayerStatsSystem
{
    public class PlayerStats : NetworkBehaviour
    {
        public static readonly Type[] DefinedModules = new Type[]
        {
            typeof(HealthStat),
            typeof(AhpStat),
            typeof(StaminaStat),
            typeof(AdminFlagsStat),
            typeof(HumeShieldStat)
        };

        private ReferenceHub _hub;
        private bool _eventAssigned;
        private StatBase[] _statModules;
        private readonly Dictionary<Type, StatBase> _dictionarizedTypes = new();

        public static event Action<ReferenceHub, DamageHandlerBase> OnAnyPlayerDamaged;
        public static event Action<ReferenceHub, DamageHandlerBase> OnAnyPlayerDied;

        public event Action<DamageHandlerBase> OnThisPlayerDamaged;
        public event Action<DamageHandlerBase> OnThisPlayerDied;

        public StatBase[] StatModules
        {
            get
            {
                if (_statModules != null) return _statModules;
                _statModules = new StatBase[DefinedModules.Length];
                for (int i = 0; i < DefinedModules.Length; i++)
                    _statModules[i] = Activator.CreateInstance(DefinedModules[i]) as StatBase;
                return _statModules;
            }
        }

        private void Awake()
        {
            _hub = ReferenceHub.GetHub(gameObject);
            foreach (var stat in StatModules)
            {
                _dictionarizedTypes.Add(stat.GetType(), stat);
                stat.Init(_hub);
            }

            // Server subscribes here (Awake) to avoid race with role assignment that can happen before Start()
            if (NetworkServer.active)
            {
                PlayerRoleManager.OnRoleChanged += OnClassChanged;
                _eventAssigned = true;
            }
        }

        private void Start()
        {
            // Client local player subscribes here (isLocalPlayer is set reliably by Start time)
            if (_hub.isLocalPlayer && !_eventAssigned)
            {
                PlayerRoleManager.OnRoleChanged += OnClassChanged;
                _eventAssigned = true;
            }
        }

        private void OnDestroy()
        {
            if (_eventAssigned)
            {
                PlayerRoleManager.OnRoleChanged -= OnClassChanged;
                _eventAssigned = false;
            }
        }

        private void Update()
        {
            StatBase[] statModules = StatModules;
            for (int i = 0; i < statModules.Length; i++)
                statModules[i].Update();
        }

        public T GetModule<T>() where T : StatBase
        {
            if (_dictionarizedTypes.TryGetValue(typeof(T), out var stat))
                return (T)stat;
            throw new InvalidCastException($"Module {typeof(T).Name} not found!");
        }

        public bool TryGetModule<T>(out T module) where T : StatBase
        {
            if (_dictionarizedTypes.TryGetValue(typeof(T), out var stat))
            {
                module = (T)stat;
                return true;
            }
            module = null;
            return false;
        }

        public bool DealDamage(DamageHandlerBase handler)
        {
            if (_hub.characterClassManager.GodMode)
                return false;

            if (_hub.roleManager.CurrentRole is IDamageHandlerProcessingRole damageHandlerProcessingRole)
                handler = damageHandlerProcessingRole.ProcessDamageHandler(handler);

            DamageHandlerBase.HandlerOutput handlerOutput = handler.ApplyDamage(_hub);
            if (handlerOutput == DamageHandlerBase.HandlerOutput.Nothing)
                return false;

            PlayerStats.OnAnyPlayerDamaged?.Invoke(_hub, handler);
            this.OnThisPlayerDamaged?.Invoke(handler);

            if (handlerOutput == DamageHandlerBase.HandlerOutput.Death)
            {
                PlayerStats.OnAnyPlayerDied?.Invoke(_hub, handler);
                this.OnThisPlayerDied?.Invoke(handler);
                KillPlayer(handler);
            }
            return true;
        }

        private void KillPlayer(DamageHandlerBase handler)
        {
            RagdollManager.ServerSpawnRagdoll(_hub, handler);
            _hub.inventory.ServerDropEverything();
            _hub.roleManager.ServerSetRole(RoleTypeId.Spectator, RoleChangeReason.Died);
            _hub.gameConsoleTransmission.SendToClient("You died. Reason: " + handler.ServerLogsText, "yellow");
            if (_hub.roleManager.CurrentRole is SpectatorRole spectatorRole)
                spectatorRole.ServerSetData(handler);
        }

        private void OnClassChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            // OnRoleChanged is a static event fired for every player's role change. Since every
            // PlayerStats instance subscribes on the server (see Awake), only react to our own
            // hub — otherwise any role change resets the stats of ALL players to max.
            if (userHub != _hub)
                return;

            if (userHub.isLocalPlayer && UserMainInterface.Singleton != null)
            {
                // Show the personal stat panel only for *alive* healthbar roles. SpectatorRole
                // (and OverwatchRole, which derives from it) also implements IHealthbarRole so
                // that StatSlider can read the spectated target's stats, but the panel itself
                // must be driven by SpectatorStatBarEmulator (shown only while actually spectating
                // a valid IHealthbarRole target). Otherwise a spectator with no target — e.g. the
                // only player on the server — would see a stale HP/stamina bar that should be hidden.
                UserMainInterface.Singleton.PlyStats.SetActive(newRole is IHealthbarRole and not SpectatorRole);
            }

            foreach (var stat in StatModules)
                stat.ClassChanged();
        }
    }
}
