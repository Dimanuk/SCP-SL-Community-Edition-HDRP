internal class FriendlyFireHandler
{
    private static bool _eventsAssigned;

    internal readonly RoundFriendlyFireDetector Round;

    internal readonly LifeFriendlyFireDetector Life;

    internal readonly WindowFriendlyFireDetector Window;

    internal readonly RespawnFriendlyFireDetector Respawn;

    internal FriendlyFireHandler(ReferenceHub hub)
    {
        Round = new RoundFriendlyFireDetector(hub);
        Life = new LifeFriendlyFireDetector(hub);
        Window = new WindowFriendlyFireDetector(hub);
        Respawn = new RespawnFriendlyFireDetector(hub);
        if (!_eventsAssigned)
        {
            global::PlayerStatsSystem.PlayerStats.OnAnyPlayerDamaged += OnAnyDamaged;
            global::PlayerStatsSystem.PlayerStats.OnAnyPlayerDied += OnAnyDied;
            _eventsAssigned = true;
        }
    }

    private static void OnAnyDied(ReferenceHub deadPlayer, global::PlayerStatsSystem.DamageHandlerBase handler)
    {
        if (IsFriendlyFire(deadPlayer, handler, out var attackerHandler))
        {
            ReferenceHub hub = attackerHandler.Attacker.Hub;
            if (!hub.FriendlyFireHandler.Respawn.RegisterKill() && !hub.FriendlyFireHandler.Window.RegisterKill() && !hub.FriendlyFireHandler.Life.RegisterKill())
            {
                hub.FriendlyFireHandler.Round.RegisterKill();
            }
        }
    }

    private static void OnAnyDamaged(ReferenceHub damagedPlayer, global::PlayerStatsSystem.DamageHandlerBase handler)
    {
        if (IsFriendlyFire(damagedPlayer, handler, out var attackerHandler))
        {
            ReferenceHub hub = attackerHandler.Attacker.Hub;
            float damage = attackerHandler.Damage;
            if (!hub.FriendlyFireHandler.Respawn.RegisterDamage(damage) && !hub.FriendlyFireHandler.Window.RegisterDamage(damage) && !hub.FriendlyFireHandler.Life.RegisterDamage(damage))
            {
                hub.FriendlyFireHandler.Round.RegisterDamage(damage);
            }
        }
    }

    private static bool IsFriendlyFire(ReferenceHub damagedPlayer, global::PlayerStatsSystem.DamageHandlerBase handler, out global::PlayerStatsSystem.AttackerDamageHandler attackerHandler)
    {
        attackerHandler = null;
        if (FriendlyFireConfig.PauseDetector || !global::Mirror.NetworkServer.active)
        {
            return false;
        }
        if (!(handler is global::PlayerStatsSystem.AttackerDamageHandler attackerDamageHandler) || attackerDamageHandler.IgnoreFriendlyFireDetector)
        {
            return false;
        }
        attackerHandler = attackerDamageHandler;
        if (!global::PlayerRoles.PlayerRoleLoader.TryGetRoleTemplate<global::PlayerRoles.PlayerRoleBase>(attackerHandler.Attacker.Role, out var result))
        {
            return false;
        }
        if (global::PlayerRoles.PlayerRolesUtils.GetFaction(damagedPlayer) != global::PlayerRoles.PlayerRolesUtils.GetFaction(result.Team))
        {
            return false;
        }
        ReferenceHub hub = attackerHandler.Attacker.Hub;
        if (hub == damagedPlayer || hub == null)
        {
            return false;
        }
        if (PermissionsHandler.IsPermitted(hub.serverRoles.Permissions, PlayerPermissions.FriendlyFireDetectorImmunity))
        {
            return false;
        }
        if (FriendlyFireConfig.IgnoreClassDTeamkills && global::PlayerRoles.PlayerRolesUtils.GetRoleId(damagedPlayer) == global::PlayerRoles.RoleTypeId.ClassD)
        {
            return global::PlayerRoles.PlayerRolesUtils.GetRoleId(hub) != global::PlayerRoles.RoleTypeId.ClassD;
        }
        return true;
    }
}
