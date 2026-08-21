using Interactables.Interobjects.DoorUtils;
using Interactables.Verification;
using Mirror;
using UnityEngine;

namespace Interactables.Interobjects
{
	public class AirlockController : NetworkBehaviour, IServerInteractable, IInteractable
	{
		private static readonly int AnimationTriggerHash = global::UnityEngine.Animator.StringToHash("Lockdown");

        public bool AirlockDisabled;

		[SerializeField]
		private DoorVariant _doorA;

		[SerializeField]
		private DoorVariant _doorB;

		[SerializeField]
		private float _lockdownDuration;

		[SerializeField]
		private float _lockdownCooldown;

		[SerializeField]
		private Animator _targetAnimator;

		private float _lockdownCombinedTimer;

		private byte _frameCooldownTimer;

		private bool _targetStateA;

		private bool _doorsLocked;

		private bool _warheadInProgress;

		private bool _readyToUse;

        public global::Interactables.Verification.IVerificationRule VerificationRule => global::Interactables.Verification.StandardDistanceVerification.Default;

        private void Start()
        {
            if (global::Mirror.NetworkServer.active)
            {
                global::Interactables.Interobjects.DoorUtils.DoorEvents.OnDoorAction += OnDoorAction;
                global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OnDoorsTriggerred += EventTriggerred;
            }
        }

        private void EventTriggerred(global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OpenerEventType eventType)
        {
            switch (eventType)
            {
                case global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OpenerEventType.WarheadStart:
                case global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OpenerEventType.WarheadCancel:
                    _warheadInProgress = eventType == global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OpenerEventType.WarheadStart;
                    _doorA.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.Warhead, _warheadInProgress);
                    _doorB.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.Warhead, _warheadInProgress);
                    if (_warheadInProgress)
                    {
                        global::Interactables.Interobjects.DoorUtils.DoorVariant doorA2 = _doorA;
                        bool networkTargetState = (_doorB.TargetState = true);
                        doorA2.TargetState = networkTargetState;
                        _frameCooldownTimer = 5;
                    }
                    else
                    {
                        ToggleAirlock();
                    }
                    break;
                case global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OpenerEventType.DeconFinish:
                    {
                        _doorsLocked = true;
                        _doorA.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.DecontLockdown, newState: true);
                        _doorB.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.DecontLockdown, newState: true);
                        global::Interactables.Interobjects.DoorUtils.DoorVariant doorA = _doorA;
                        bool networkTargetState = (_doorB.TargetState = false);
                        doorA.TargetState = networkTargetState;
                        _lockdownCombinedTimer = 65535f;
                        break;
                    }
            }
        }

        private void OnDestroy()
        {
            if (global::Mirror.NetworkServer.active)
            {
                global::Interactables.Interobjects.DoorUtils.DoorEvents.OnDoorAction -= OnDoorAction;
                global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OnDoorsTriggerred -= EventTriggerred;
            }
        }

        private void OnDoorAction(global::Interactables.Interobjects.DoorUtils.DoorVariant door, global::Interactables.Interobjects.DoorUtils.DoorAction action, ReferenceHub ply)
        {
            if (door.ActiveLocks <= 0 && (!(door != _doorA) || !(door != _doorB)) && !AirlockDisabled && !_warheadInProgress && _readyToUse)
            {
                if (action == global::Interactables.Interobjects.DoorUtils.DoorAction.Destroyed)
                {
                    AirlockDisabled = true;
                }
                else if ((_doorA.AllowInteracting(ply, 0) || _doorB.AllowInteracting(ply, 0)) && _frameCooldownTimer <= 0 && (action == global::Interactables.Interobjects.DoorUtils.DoorAction.Opened || action == global::Interactables.Interobjects.DoorUtils.DoorAction.Closed))
                {
                    ToggleAirlock();
                }
            }
        }

        private void ToggleAirlock()
        {
            _targetStateA = !_targetStateA;
            _doorB.TargetState = _targetStateA;
            _doorA.TargetState = !_targetStateA;
            _frameCooldownTimer = 5;
        }

        private void Update()
        {
            if (!global::Mirror.NetworkServer.active)
            {
                return;
            }
            if (_frameCooldownTimer > 0)
            {
                _frameCooldownTimer--;
            }
            if (_readyToUse)
            {
                if (_lockdownCombinedTimer > 0f - global::UnityEngine.Mathf.Abs(_lockdownCooldown))
                {
                    _lockdownCombinedTimer -= global::UnityEngine.Time.deltaTime;
                    if (_doorsLocked && _lockdownCombinedTimer <= 0f)
                    {
                        _doorsLocked = false;
                        _doorA.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.SpecialDoorFeature, newState: false);
                        _doorB.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.SpecialDoorFeature, newState: false);
                        _doorA.TargetState = _targetStateA;
                        _doorB.TargetState = !_targetStateA;
                    }
                }
            }
            else if (_frameCooldownTimer == 0)
            {
                if (global::UnityEngine.Mathf.RoundToInt(_doorA.GetExactState()) == global::UnityEngine.Mathf.RoundToInt(_doorB.GetExactState()))
                {
                    _doorB.TargetState = _targetStateA;
                    _doorA.TargetState = !_targetStateA;
                    _frameCooldownTimer = 200;
                }
                else
                {
                    _readyToUse = true;
                    _frameCooldownTimer = 10;
                }
            }
        }


        [global::Mirror.ClientRpc]
        private void RpcAlarm()
        {
            _targetAnimator.SetTrigger(AnimationTriggerHash);
        }

        public void ServerInteract(ReferenceHub ply, byte colliderId)
        {
            if (!AirlockDisabled && !(_lockdownCombinedTimer > 0f - global::UnityEngine.Mathf.Abs(_lockdownCooldown)))
            {
                _lockdownCombinedTimer = global::UnityEngine.Mathf.Abs(_lockdownDuration);
                _doorsLocked = true;
                _doorA.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.SpecialDoorFeature, newState: true);
                _doorB.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.SpecialDoorFeature, newState: true);
                global::Interactables.Interobjects.DoorUtils.DoorVariant doorA = _doorA;
                bool networkTargetState = (_doorB.TargetState = false);
                doorA.TargetState = networkTargetState;
                RpcAlarm();
            }
        }

	}
}
