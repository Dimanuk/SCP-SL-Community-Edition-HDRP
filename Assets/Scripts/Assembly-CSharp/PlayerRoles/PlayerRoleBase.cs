using System;
using System.Diagnostics;
using GameObjectPools;
using UnityEngine;

namespace PlayerRoles
{
    public abstract class PlayerRoleBase : PoolObject
    {
        private ReferenceHub _lastOwner;

        private RoleSpawnFlags _spawnFlags;

        private RoleChangeReason _spawnReason;

        public readonly Stopwatch _activeTime = Stopwatch.StartNew();

        public Action<RoleTypeId> OnRoleDisabled;

        public abstract RoleTypeId RoleTypeId { get; }

        public abstract Team Team { get; }

        public abstract Color RoleColor { get; }

        [field: SerializeField]
        public virtual GameObject RoleHelpInfo { get; private set; }

        public string RoleName
        {
            get
            {
                if (this is ICustomNameRole customNameRole)
                {
                    return customNameRole.CustomRoleName ?? RoleTranslations.GetRoleName(RoleTypeId);
                }
                return RoleTranslations.GetRoleName(RoleTypeId);
            }
        }

        public float ActiveTime => (float)_activeTime.Elapsed.TotalSeconds;

        public bool IsLocalPlayer => _lastOwner != null && _lastOwner.isLocalPlayer;
        public ReferenceHub LastOwner => _lastOwner;

        public RoleChangeReason ServerSpawnReason
        {
            get
            {
                if (!Mirror.NetworkServer.active)
                {
                    UnityEngine.Debug.LogError("Server-only property ServerSpawnReason cannot be called on the client!");
                }
                return _spawnReason;
            }
            private set
            {
                _spawnReason = value;
            }
        }

        public RoleSpawnFlags ServerSpawnFlags
        {
            get
            {
                if (!Mirror.NetworkServer.active)
                {
                    UnityEngine.Debug.LogError("Server-only property ServerSpawnFlags cannot be called on the client!");
                }
                return _spawnFlags;
            }
            internal set
            {
                _spawnFlags = value;
            }
        }

        internal virtual void Init(ReferenceHub hub, RoleChangeReason spawnReason, RoleSpawnFlags spawnFlags)
        {
            _lastOwner = hub;
            _spawnReason = spawnReason;
            _spawnFlags = spawnFlags;
            _activeTime.Restart();
        }

        public bool TryGetOwner(out ReferenceHub hub)
        {
            hub = _lastOwner;
            return _lastOwner != null;
        }

        public virtual void DisableRole(RoleTypeId newRole)
        {
            try
            {
                OnRoleDisabled?.Invoke(newRole);
                ReturnToPool(true);
                UnityEngine.Debug.Log($"Disabling role {RoleTypeId} for {_lastOwner}");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} (RoleTypeId = '{1}', Owner = '{2}', ActiveTime = '{3}')", GetType().Name, RoleTypeId, _lastOwner, ActiveTime);
        }
    }
}