using System.Collections.Generic;
using PlayerRoles.PlayableScps.HumeShield;
using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerStatsSystem;
using UnityEngine;
using GameObjectPools;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106Role : FpcStandardScp,
        ISubroutinedScpRole, IHudScp, IPoolResettable,
        IHumeShieldedRole, IDamageHandlerProcessingRole,
        ITeslaControllerRole, ISpawnableScp
    {
        public static readonly HashSet<Scp106Role> AllInstances = new HashSet<Scp106Role>();
        private Scp106SinkholeController _sinkholeCtrl;
        private bool _sinkholeSet;

        [field: SerializeField]
        public HumeShieldModuleBase HumeShieldModule { get; private set; }

        [field: SerializeField]
        public SubroutineManagerModule SubroutineModule { get; private set; }

        [field: SerializeField]
        public ScpHudBase HudPrefab { get; private set; }
        [field: SerializeField]
        public AudioClip ItemSpawnSound { get; private set; }

        public Scp106SinkholeController Sinkhole
        {
            get
            {
                if (!_sinkholeSet)
                {
                    if (SubroutineModule != null)
                        SubroutineModule.TryGetSubroutine(out _sinkholeCtrl);

                    _sinkholeSet = true;
                }
                return _sinkholeCtrl;
            }
        }

        public bool CanActivateIdle => !IsSubmerged;

        public bool CanActivateShock => !IsSubmerged;

        public bool IsSubmerged
        {
            get
            {
                if (SubroutineModule == null)
                    return false;

                if (SubroutineModule.TryGetSubroutine(out Scp106StalkAbility subroutine))
                {
                    return subroutine.IsSubmerged;
                }
                return false;
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            AllInstances.Add(this);
        }

        public void ResetObject()
        {
            AllInstances.Remove(this);
        }

        public DamageHandlerBase ProcessDamageHandler(DamageHandlerBase dhb)
        {
            Scp106SinkholeController sinkhole = Sinkhole;
            if (sinkhole == null || !sinkhole.IsHidden)
            {
                return dhb;
            }
            if (!ValidateDamageHandler(dhb))
            {
                dhb = new UniversalDamageHandler();
            }
            return dhb;
        }

        private bool ValidateDamageHandler(DamageHandlerBase dhb)
        {
            if (dhb is UniversalDamageHandler universalDamageHandler && universalDamageHandler.TranslationId == DeathTranslations.Tesla.Id)
            {
                return false;
            }
            if (dhb is ExplosionDamageHandler || dhb is MicroHidDamageHandler || dhb is FirearmDamageHandler || (dhb is Scp018DamageHandler && IsSubmerged))
            {
                return false;
            }
            return true;
        }

        public float GetSpawnChance(List<RoleTypeId> alreadySpawned)
        {
            return 1f;
        }
    }
}