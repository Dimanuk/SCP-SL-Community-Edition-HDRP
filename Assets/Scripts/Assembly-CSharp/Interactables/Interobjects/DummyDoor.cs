using Interactables.Interobjects.DoorUtils;
using Mirror;
using UnityEngine;

namespace Interactables.Interobjects
{
	public class DummyDoor : DoorVariant, IDamageableDoor, INonInteractableDoor
	{
		public bool IsDestroyed
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		[field: SerializeField]
		public bool IgnoreLockdowns { get; private set; }

		[field: SerializeField]
		public bool IgnoreRemoteAdmin { get; private set; }

		public override void LockBypassDenied(ReferenceHub ply, byte colliderId)
		{
		}

		public override void PermissionsDenied(ReferenceHub ply, byte colliderId)
		{
		}

		public override bool AllowInteracting(ReferenceHub ply, byte colliderId)
		{
			return 0 != 0;
		}

		public override bool AnticheatPassageApproved()
		{
			return 0 != 0;
		}

        public override float GetExactState()
        {
            return TargetState ? 1 : 0;
        }

        public float GetHealthPercent()
		{
			return 0f;
		}

		public override bool IsConsideredOpen()
		{
			return TargetState;
		}

		public bool ServerDamage(float hp, DoorDamageType type)
		{
			return 0 != 0;
		}

		public void ClientDestroyEffects()
		{
		}
	}
}
