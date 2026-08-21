using Interactables.Interobjects.DoorUtils;
using Mirror;
using PlayerRoles;
using UnityEngine;

namespace Interactables.Interobjects
{
	public class PryableDoor : BasicDoor, IScp106PassableDoor
	{
        private static readonly int PryAnimHash = Animator.StringToHash("PryGate");

        [SerializeField]
		private AudioClip _prySound;

		[SerializeField]
		private DoorLockReason _blockPryingMask = DoorLockReason.None;

        [SerializeField]
		private float _pryAnimDuration;

		public Transform[] PryPositions;

		private float _remainingPryCooldown;

		public bool IsScp106Passable => true;

        [global::Mirror.Server]
        public bool TryPryGate(ReferenceHub player)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                global::UnityEngine.Debug.LogWarning("[Server] function 'System.Boolean Interactables.Interobjects.PryableDoor::TryPryGate(ReferenceHub)' called when server was not active");
                return default(bool);
            }
            if (_blockPryingMask != global::Interactables.Interobjects.DoorUtils.DoorLockReason.None && global::Interactables.Interobjects.DoorUtils.DoorLockUtils.HasFlagFast((global::Interactables.Interobjects.DoorUtils.DoorLockReason)ActiveLocks, _blockPryingMask))
            {
                return false;
            }
            if (AllowInteracting(null, 0))
            {
                RpcPryGate();
                _remainingPryCooldown = _pryAnimDuration;
                return true;
            }
            return false;
        }

        [ClientRpc]
        private void RpcPryGate()
        {
            if (MainSource != null && _prySound != null)
                MainSource.PlayOneShot(_prySound);

            if (MainAnimator != null)
                MainAnimator.SetTrigger(PryAnimHash);
        }

        public override bool AllowInteracting(ReferenceHub ply, byte colliderId)
        {
            if (_remainingPryCooldown <= 0f)
            {
                return base.AllowInteracting(ply, colliderId);
            }
            return false;
        }

        protected override void Update()
        {
            base.Update();
            if (_remainingPryCooldown > 0f)
            {
                _remainingPryCooldown -= global::UnityEngine.Time.deltaTime;
                if (_remainingPryCooldown <= 0f)
                {
                    MainAnimator.ResetTrigger(PryAnimHash);
                }
            }
        }
    }
}
