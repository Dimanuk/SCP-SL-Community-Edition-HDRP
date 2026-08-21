namespace Footprinting
{
    public readonly struct Footprint : global::System.IEquatable<global::Footprinting.Footprint>
    {
        public readonly ReferenceHub Hub;

        public readonly bool IsSet;

        public readonly int PlayerId;

        public readonly uint NetId;

        public readonly global::PlayerRoles.RoleTypeId Role;

        public readonly byte Unit;

        public readonly string Nickname;

        public readonly string LogUserID;

        public readonly global::System.Diagnostics.Stopwatch Stopwatch;

        private readonly int _serial;

        private static int _serialClock;

        public string UnitName
        {
            get
            {
                if (!global::PlayerRoles.PlayerRoleLoader.TryGetRoleTemplate<global::PlayerRoles.HumanRole>(Role, out var result))
                {
                    return string.Empty;
                }
                if (!global::Respawning.NamingRules.UnitNameMessageHandler.ReceivedNames.TryGetValue(result.AssignedSpawnableTeam, out var value))
                {
                    return string.Empty;
                }
                int count = value.Count;
                if (Unit < 0 || Unit >= count)
                {
                    return string.Empty;
                }
                return value[Unit];
            }
        }

        public Footprint(ReferenceHub hub)
        {
            IsSet = true;
            Stopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            bool flag = hub == null;
            Hub = (flag ? null : hub);
            PlayerId = ((!flag) ? hub.PlayerId : 0);
            NetId = ((!flag) ? hub.networkIdentity.netId : 0u);
            Role = (flag ? global::PlayerRoles.RoleTypeId.None : global::PlayerRoles.PlayerRolesUtils.GetRoleId(hub));
            Unit = (byte)((!flag && hub.roleManager.CurrentRole is global::PlayerRoles.HumanRole humanRole) ? humanRole.UnitNameId : 0);
            LogUserID = (flag ? string.Empty : hub.characterClassManager.UserId);
            Nickname = (flag ? "(null)" : hub.nicknameSync.MyNick);
            if (flag)
            {
                _serial = 0;
                return;
            }
            if (++_serialClock == 0)
            {
                _serialClock++;
            }
            _serial = _serialClock;
        }

        public bool Equals(global::Footprinting.Footprint other)
        {
            if (IsSet == other.IsSet)
            {
                return _serial == other._serial;
            }
            return false;
        }

        public bool SameLife(global::Footprinting.Footprint other)
        {
            if (NetId == other.NetId && Role == other.Role)
            {
                return Unit == other.Unit;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _serial;
        }
    }
}
