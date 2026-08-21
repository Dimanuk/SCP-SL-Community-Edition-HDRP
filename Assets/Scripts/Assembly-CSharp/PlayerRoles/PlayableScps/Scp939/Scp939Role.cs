using System.Collections.Generic;
using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.PlayableScps.HumeShield;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939
{
    public class Scp939Role : FpcStandardScp, ISubroutinedScpRole, IHumeShieldedRole, IHudScp, ISpawnableScp
    {
        [field: SerializeField]
        public HumeShieldModuleBase HumeShieldModule { get; private set; }

        [field: SerializeField]
        public SubroutineManagerModule SubroutineModule { get; private set; }

        [field: SerializeField]
        public ScpHudBase HudPrefab { get; private set; }

        public float GetSpawnChance(List<RoleTypeId> alreadySpawned)
        {
            return 1f;
        }
    }
}