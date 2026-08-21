using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror;
using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.PlayableScps.HumeShield;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerStatsSystem;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp173
{
    public class Scp173Role : FpcStandardScp, ISubroutinedScpRole, IArmoredRole, IHumeShieldedRole, IHudScp, ISpawnableScp
    {
        [SerializeField]
        private int _armorEfficacy;

        private ReferenceHub _owner;
        private Scp173AudioPlayer _audio;
        private bool _damagedEventAssigned;

        [SerializeField]
        private HumeShieldModuleBase _humeShieldModule;

        [SerializeField]
        private SubroutineManagerModule _subroutineModule;

        [SerializeField]
        private ScpHudBase _hudPrefab;

        private bool DamagedEventActive
        {
            get => _damagedEventAssigned;
            set
            {
                if (value == _damagedEventAssigned)
                    return;

                if (value && !NetworkServer.active)
                    return;

                PlayerStats playerStats = _owner.playerStats;

                if (value)
                    playerStats.OnThisPlayerDamaged += OnDamaged;
                else
                    playerStats.OnThisPlayerDamaged -= OnDamaged;

                _damagedEventAssigned = value;
            }
        }
        public ScpDamageHandler DamageHandler
        {
            get
            {
                if (!TryGetOwner(out ReferenceHub hub))
                    throw new InvalidOperationException("Damage handler could not be created for an inactive instance of SCP-173.");

                return new ScpDamageHandler(hub, DeathTranslations.Scp173);
            }
        }

        public HumeShieldModuleBase HumeShieldModule
        {
            get => _humeShieldModule;
            private set => _humeShieldModule = value;
        }

        public SubroutineManagerModule SubroutineModule
        {
            get => _subroutineModule;
            private set => _subroutineModule = value;
        }

        public ScpHudBase HudPrefab
        {
            get => _hudPrefab;
            private set => _hudPrefab = value;
        }

        private void OnDamaged(DamageHandlerBase obj)
        {
            if (obj is not FirearmDamageHandler)
                return;

            _audio.ServerSendSound(Scp173AudioPlayer.Scp173SoundId.Hit);
        }

        private void Awake()
        {
            if (_subroutineModule == null)
                _subroutineModule = GetComponent<SubroutineManagerModule>();

            if (SubroutineModule != null)
                SubroutineModule.TryGetSubroutine(out _audio);
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            TryGetOwner(out _owner);
            DamagedEventActive = true;

            if (_audio == null && SubroutineModule != null)
                SubroutineModule.TryGetSubroutine(out _audio);
        }

        public override void DisableRole(RoleTypeId newRole)
        {
            base.DisableRole(newRole);
            DamagedEventActive = false;
        }

        public int GetArmorEfficacy(HitboxType hitbox)
        {
            if (HumeShieldModule != null && HumeShieldModule.HsCurrent > 0f)
                return 0;

            return _armorEfficacy;
        }

        public float GetSpawnChance(List<RoleTypeId> alreadySpawned) => 1f;
    }
}