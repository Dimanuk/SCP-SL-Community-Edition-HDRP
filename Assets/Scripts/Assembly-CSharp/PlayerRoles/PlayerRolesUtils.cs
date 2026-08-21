namespace PlayerRoles
{
    public static class PlayerRolesUtils
    {
        public static global::PlayerRoles.RoleTypeId GetRoleId(this ReferenceHub hub)
        {
            return hub.roleManager.CurrentRole.RoleTypeId;
        }

        public static global::PlayerRoles.Team GetTeam(this ReferenceHub hub)
        {
            return hub.roleManager.CurrentRole.Team;
        }

        public static global::PlayerRoles.Team GetTeam(this global::PlayerRoles.RoleTypeId role)
        {
            if (!global::PlayerRoles.PlayerRoleLoader.TryGetRoleTemplate<global::PlayerRoles.PlayerRoleBase>(role, out var result))
            {
                return global::PlayerRoles.Team.OtherAlive;
            }
            return result.Team;
        }

        public static global::PlayerRoles.Faction GetFaction(this ReferenceHub hub)
        {
            return hub.GetTeam().GetFaction();
        }

        public static global::PlayerRoles.Faction GetFaction(this global::PlayerRoles.RoleTypeId role)
        {
            return role.GetTeam().GetFaction();
        }

        public static global::PlayerRoles.Faction GetFaction(this global::PlayerRoles.Team t)
        {
            switch (t)
            {
                case global::PlayerRoles.Team.SCPs:
                    return global::PlayerRoles.Faction.SCP;
                case global::PlayerRoles.Team.FoundationForces:
                case global::PlayerRoles.Team.Scientists:
                    return global::PlayerRoles.Faction.FoundationStaff;
                case global::PlayerRoles.Team.ChaosInsurgency:
                case global::PlayerRoles.Team.ClassD:
                    return global::PlayerRoles.Faction.FoundationEnemy;
                default:
                    return global::PlayerRoles.Faction.Unclassified;
            }
        }

        public static bool IsHuman(this global::PlayerRoles.RoleTypeId role)
        {
            global::PlayerRoles.Team team = role.GetTeam();
            if (team != global::PlayerRoles.Team.Dead)
            {
                return team != global::PlayerRoles.Team.SCPs;
            }
            return false;
        }

        public static bool IsHuman(this ReferenceHub hub)
        {
            if (hub.IsAlive())
            {
                return !hub.IsSCP();
            }
            return false;
        }

        public static bool IsAlive(this global::PlayerRoles.RoleTypeId role)
        {
            return role.GetTeam() != global::PlayerRoles.Team.Dead;
        }

        public static bool IsAlive(this ReferenceHub hub)
        {
            return hub.GetTeam() != global::PlayerRoles.Team.Dead;
        }

        public static bool IsSCP(this ReferenceHub hub, bool includeZombies = true)
        {
            if (hub.GetTeam() == global::PlayerRoles.Team.SCPs)
            {
                if (!includeZombies)
                {
                    return hub.GetRoleId() != global::PlayerRoles.RoleTypeId.Scp0492;
                }
                return true;
            }
            return false;
        }

        public static void ForEachRole<T>(global::System.Action<ReferenceHub, T> action) where T : global::PlayerRoles.PlayerRoleBase
        {
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.roleManager.CurrentRole is T arg)
                {
                    action?.Invoke(allHub, arg);
                }
            }
        }

        public static void ForEachRole<T>(global::System.Action<T> action) where T : global::PlayerRoles.PlayerRoleBase
        {
            ForEachRole(delegate (ReferenceHub x, T y)
            {
                action?.Invoke(y);
            });
        }

        public static void ForEachRole<T>(global::System.Action<ReferenceHub> action) where T : global::PlayerRoles.PlayerRoleBase
        {
            ForEachRole(delegate (ReferenceHub x, T y)
            {
                action?.Invoke(x);
            });
        }
    }
}
