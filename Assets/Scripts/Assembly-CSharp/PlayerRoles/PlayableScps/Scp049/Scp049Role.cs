using System.Collections.Generic;
using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.PlayableScps.HumeShield;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp049
{
    public class Scp049Role : FpcStandardScp, ISubroutinedScpRole, IArmoredRole, IHumeShieldedRole, IHudScp, ISpawnableScp
    {
        [SerializeField]
        private int _armorEfficacy;

        [field: SerializeField]
        public HumeShieldModuleBase HumeShieldModule { get; private set; }

        [field: SerializeField]
        public SubroutineManagerModule SubroutineModule { get; private set; }


        [field: SerializeField]
        public ScpHudBase HudPrefab { get; private set; }

        public int GetArmorEfficacy(HitboxType hitbox)
        {
            if (HumeShieldModule.HsCurrent > 0f)
                return 0;

            return _armorEfficacy;
        }

        public float GetSpawnChance(List<RoleTypeId> alreadySpawned) => 1f;

    }
}