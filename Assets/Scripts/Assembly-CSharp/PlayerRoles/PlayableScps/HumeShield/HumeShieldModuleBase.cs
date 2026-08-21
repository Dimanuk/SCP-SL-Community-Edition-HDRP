using System;
using GameObjectPools;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;

namespace PlayerRoles.PlayableScps.HumeShield
{
    public abstract class HumeShieldModuleBase : MonoBehaviour, IPoolSpawnable
    {

        [SerializeField]
        private PlayerRoleBase _role;

        protected HumeShieldStat HsStat { get; private set; }
        protected ReferenceHub Owner { get; private set; }

        public PlayerRoleBase Role => _role;

        public float HsCurrent
        {
            get => HsStat.CurValue;
            set
            {
                if (!NetworkServer.active)
                    throw new InvalidOperationException("Hume Shield cannot be assigned by a client!");
                HsStat.CurValue = value;
            }
        }

        public abstract float HsMax { get; }
        public abstract float HsRegeneration { get; }
        public abstract Color? HsWarningColor { get; }

        public virtual void OnHsValueChanged(float prevValue, float newValue) { }

        public virtual void SpawnObject()
        {
            if (!Role.TryGetOwner(out ReferenceHub hub))
                throw new InvalidOperationException("'" + base.name + "' Hume Shield Controller spawned without a role!");
            Owner = hub;
            HsStat = hub.playerStats.GetModule<HumeShieldStat>();
        }
    }
}
