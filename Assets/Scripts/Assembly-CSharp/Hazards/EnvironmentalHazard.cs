using System.Collections.Generic;
using GameCore;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace Hazards
{
    public abstract class EnvironmentalHazard : NetworkBehaviour
    {
        public List<ReferenceHub> AffectedPlayers { get; } = new List<ReferenceHub>();

        [field: SerializeField]
        public virtual float MaxDistance { get; set; }

        [field: SerializeField]
        public virtual float MaxHeightDistance { get; set; }

        [field: SerializeField]
        public virtual Vector3 SourceOffset { get; set; }

        public virtual bool IsActive { get; } = true;

        public virtual Vector3 SourcePosition
        {
            get
            {
                return base.transform.position + SourceOffset;
            }
            set
            {
                base.transform.position = value;
            }
        }

        public virtual void OnEnter(ReferenceHub player)
        {
            AffectedPlayers.Add(player);
        }

        public virtual void OnStay(ReferenceHub player)
        {
        }

        public virtual void OnExit(ReferenceHub player)
        {
            AffectedPlayers.Remove(player);
        }

        public virtual bool IsInArea(Vector3 sourcePos, Vector3 targetPos)
        {
            if (Mathf.Abs(targetPos.y - sourcePos.y) > MaxHeightDistance)
            {
                return false;
            }
            return (sourcePos - targetPos).SqrMagnitudeIgnoreY() <= MaxDistance * MaxDistance;
        }

        protected virtual void UpdateTargets()
        {
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (!(allHub.roleManager.CurrentRole is IFpcRole fpcRole))
                {
                    continue;
                }
                bool flag = AffectedPlayers.Contains(allHub);
                if (IsInArea(SourcePosition, fpcRole.FpcModule.Position))
                {
                    if (!flag)
                    {
                        OnEnter(allHub);
                    }
                    else
                    {
                        OnStay(allHub);
                    }
                }
                else if (flag)
                {
                    OnExit(allHub);
                }
            }
        }

        private void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (AffectedPlayers.Contains(userHub))
            {
                OnExit(userHub);
            }
        }

        protected virtual void Start()
        {
            if (NetworkServer.active)
            {
                NetworkServer.Spawn(base.gameObject);
                if (!RoundStart.RoundStarted)
                {
                    Console.AddDebugLog("MAPGEN", "Spawning hazard: \"" + base.gameObject.name + "\"", MessageImportance.LessImportant, nospace: true);
                }
                PlayerRoleManager.OnRoleChanged += OnRoleChanged;
            }
        }

        protected virtual void Update()
        {
            if (NetworkServer.active)
            {
                UpdateTargets();
            }
        }

        protected virtual void OnDestroy()
        {
            if (NetworkServer.active)
            {
                PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
            }
        }
    }
}
