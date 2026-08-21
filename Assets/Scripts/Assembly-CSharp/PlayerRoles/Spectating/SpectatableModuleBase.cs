using System.Collections.Generic;
using System.Runtime.CompilerServices;

using GameObjectPools;
using UnityEngine;

namespace PlayerRoles.Spectating
{
	public abstract class SpectatableModuleBase : MonoBehaviour, IPoolSpawnable, IPoolResettable
	{
		public delegate void Added(SpectatableModuleBase module);

		public delegate void Removed(SpectatableModuleBase module);

        public static readonly global::System.Collections.Generic.HashSet<global::PlayerRoles.Spectating.SpectatableModuleBase> AllInstances = new global::System.Collections.Generic.HashSet<global::PlayerRoles.Spectating.SpectatableModuleBase>();

        public SpectatableListElementType ListElementType;

		private PlayerRoleBase _cachedRole;

		private bool _roleCacheSet;

		public abstract Vector3 CameraPosition { get; }

		public abstract Vector3 CameraRotation { get; }

        public global::PlayerRoles.PlayerRoleBase MainRole
        {
            get
            {
                if (!_roleCacheSet)
                {
                    if (!TryGetComponent<global::PlayerRoles.PlayerRoleBase>(out _cachedRole))
                    {
                        throw new global::System.InvalidOperationException("SpectatableModuleBase of name '" + base.name + "' is not assigned to any role!");
                    }
                    _roleCacheSet = true;
                }
                return _cachedRole;
            }
        }

        public ReferenceHub TargetHub
        {
            get
            {
                if (!MainRole.TryGetOwner(out var hub))
                {
                    throw new global::System.InvalidOperationException("SpectatableModuleBase of name '" + base.name + "' does not have an owner!");
                }
                return hub;
            }
        }

		public static event Added OnAdded;

		public static event Removed OnRemoved;

        public virtual void ResetObject()
        {
            AllInstances.Remove(this);
            global::PlayerRoles.Spectating.SpectatableModuleBase.OnRemoved?.Invoke(this);
        }

        public virtual void SpawnObject()
        {
            AllInstances.Add(this);
            global::PlayerRoles.Spectating.SpectatableModuleBase.OnAdded?.Invoke(this);
        }

        internal virtual void OnBeganSpectating()
		{
			
		}

		internal virtual void OnStoppedSpectating()
		{
			
		}
	}
}
