public class HitboxIdentity : global::UnityEngine.MonoBehaviour, IDestructible
{
    public static readonly global::System.Collections.Generic.HashSet<HitboxIdentity> Instances = new global::System.Collections.Generic.HashSet<HitboxIdentity>();

    [global::UnityEngine.SerializeField]
    private HitboxType _dmgMultiplier;

    public ReferenceHub TargetHub => CharacterModel.OwnerHub;

    public HitboxType HitboxType => _dmgMultiplier;

    public global::PlayerRoles.FirstPersonControl.Thirdperson.CharacterModel CharacterModel { get; private set; }

    public global::UnityEngine.Collider[] TargetColliders { get; private set; }

    public uint NetworkId => TargetHub.inventory.netId;

    public global::UnityEngine.Vector3 CenterOfMass => base.transform.position;

    public bool Damage(float damage, global::PlayerStatsSystem.DamageHandlerBase handler, global::UnityEngine.Vector3 exactPos)
    {
        if (TargetHub == null)
        {
            return false;
        }
        if (handler is global::PlayerStatsSystem.StandardDamageHandler standardDamageHandler)
        {
            standardDamageHandler.Hitbox = _dmgMultiplier;
        }
        return TargetHub.playerStats.DealDamage(handler);
    }

    public void SetColliders(bool newState)
    {
        global::UnityEngine.Collider[] targetColliders = TargetColliders;
        for (int i = 0; i < targetColliders.Length; i++)
        {
            targetColliders[i].enabled = newState;
        }
    }

    // CharacterModel.SpawnObject disables a local player's own hitbox colliders (so their own
    // hitscan/raycasts never hit themselves). On a listen-server host this is the same physics
    // scene the server uses for authoritative AoE/linecast checks, so it also hides the host from
    // their own explosions unless briefly re-enabled first, mirroring StandardHitregBase's target
    // re-enable around ServerPerformShot.
    public static bool SetOwnHitboxes(ReferenceHub hub, bool state)
    {
        if (hub == null || hub.roleManager == null || !(hub.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole))
        {
            return false;
        }
        HitboxIdentity[] hitboxes = fpcRole.FpcModule.CharacterModelInstance.Hitboxes;
        for (int i = 0; i < hitboxes.Length; i++)
        {
            hitboxes[i].SetColliders(state);
        }
        return true;
    }

    private void Awake()
    {
        CharacterModel = GetComponentInParent<global::PlayerRoles.FirstPersonControl.Thirdperson.CharacterModel>();
        TargetColliders = GetComponents<global::UnityEngine.Collider>();
    }

    private void OnDestroy()
    {
        Instances.Remove(this);
    }

    public static bool CheckFriendlyFire(ReferenceHub attacker, ReferenceHub victim, bool ignoreConfig = false)
    {
        return CheckFriendlyFire(global::PlayerRoles.PlayerRolesUtils.GetTeam(attacker), global::PlayerRoles.PlayerRolesUtils.GetTeam(victim), ignoreConfig);
    }

    public static bool CheckFriendlyFire(global::PlayerRoles.RoleTypeId attacker, global::PlayerRoles.RoleTypeId victim, bool ignoreConfig = false)
    {
        global::PlayerRoles.Team team = global::PlayerRoles.PlayerRolesUtils.GetTeam(attacker);
        global::PlayerRoles.Team team2 = global::PlayerRoles.PlayerRolesUtils.GetTeam(victim);
        return CheckFriendlyFire(team, team2, ignoreConfig);
    }

    public static bool CheckFriendlyFire(global::PlayerRoles.Team attackerTeam, global::PlayerRoles.Team victimTeam, bool ignoreConfig = false)
    {
        if (attackerTeam == global::PlayerRoles.Team.Dead || victimTeam == global::PlayerRoles.Team.Dead)
        {
            return false;
        }
        if (attackerTeam == global::PlayerRoles.Team.SCPs && victimTeam == global::PlayerRoles.Team.SCPs)
        {
            return false;
        }
        if (ignoreConfig || (ServerConfigSynchronizer.Singleton.MainBoolsSync & 1) != 1)
        {
            return global::PlayerRoles.PlayerRolesUtils.GetFaction(attackerTeam) != global::PlayerRoles.PlayerRolesUtils.GetFaction(victimTeam);
        }
        return true;
    }
}
