using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Pickups;
using MapGeneration;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.ThrowableProjectiles
{
	public class Scp2176Projectile : EffectGrenade
	{
		private const float LockdownDuration = 13f;

		private const float LockdownDisableValue = 0.1f;

		private const float PanicDuration = 5f;

		private const float ShatterVelocity = 8.5f;

		private const float ActivateVelocity = 6.5f;

		private const float DropSoundRange = 20f;

		private bool _hasTriggered;

		[SerializeField]
		private AudioClip _dropSound;

		[SyncVar]
		private bool _playedDropSound;

		public static event Action<RoomIdentifier> OnServerShattered;

        protected override void ProcessCollision(global::UnityEngine.Collision collision)
        {
            float sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
            if (!global::Mirror.NetworkServer.active || !(sqrMagnitude >= 42.25f))
            {
                return;
            }
            base.ProcessCollision(collision);
            if (!_hasTriggered)
            {
                ServerActivate();
                if (sqrMagnitude >= 72.25f)
                {
                    ServerFuseEnd();
                }
                else if (!_playedDropSound)
                {
                    _playedDropSound = true;
                    RpcMakeSound();
                }
            }
        }

        protected override void ServerFuseEnd()
        {
            _hasTriggered = true;
            if (base.TargetTime <= 0f)
            {
                base.TargetTime = global::UnityEngine.Time.timeSinceLevelLoad;
            }
            base.ServerFuseEnd();
            ServerShatter();
        }

        [ClientRpc]
		private void RpcMakeSound()
		{
            AudioPooling.AudioSourcePoolManager.PlaySound(_dropSound, gameObject.transform, 2);
        }

        public override void ServerActivate()
        {
            base.ServerActivate();
            if (base.TargetTime <= 0f)
            {
                global::InventorySystem.Items.Pickups.PickupSyncInfo info = Info;
                info.Locked = true;
                base.Info = info;
            }
        }

        public void ServerImmediatelyShatter()
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Tried to call ServerImmediatelyShatter from the client!");
            }
            ServerActivate();
            ServerFuseEnd();
        }

        private void ServerShatter()
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Tried to call ServerShatter from the client!");
            }
            global::MapGeneration.RoomIdentifier rid = global::MapGeneration.RoomIdUtils.RoomAtPositionRaycasts(base.transform.position);
            if (rid == null)
            {
                return;
            }
            global::InventorySystem.Items.ThrowableProjectiles.Scp2176Projectile.OnServerShattered?.Invoke(rid);
            global::System.Collections.Generic.IEnumerable<FlickerableLightController> enumerable = global::System.Linq.Enumerable.Where(FlickerableLightController.Instances, (FlickerableLightController x) => x.Room == rid);
            if (rid.Name == global::MapGeneration.RoomName.HczTesla && TryFindTeslaAtRoom(rid, out var gate))
            {
                ServerOverloadTesla(rid, gate, enumerable);
            }
            else
            {
                foreach (FlickerableLightController item in enumerable)
                {
                    item.ServerFlickerLights(item.LightsEnabled ? 13f : 0.1f);
                }
            }
            if (global::Interactables.Interobjects.DoorUtils.DoorVariant.DoorsByRoom.TryGetValue(rid, out var value))
            {
                ServerLockdown(value);
            }
        }

        private static bool TryFindTeslaAtRoom(global::MapGeneration.RoomIdentifier rid, out TeslaGate gate)
        {
            gate = global::System.Linq.Enumerable.FirstOrDefault(TeslaGateController.Singleton.TeslaGates, (TeslaGate x) => rid == global::MapGeneration.RoomIdUtils.RoomAtPosition(x.transform.position));
            return gate != null;
        }

        private void ServerOverloadTesla(global::MapGeneration.RoomIdentifier rid, TeslaGate tg, global::System.Collections.Generic.IEnumerable<FlickerableLightController> lightControllers)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Tried to call ServerOverloadTesla from the client!");
            }
            foreach (FlickerableLightController lightController in lightControllers)
            {
                lightController.ServerFlickerLights(lightController.LightsEnabled ? 12f : 0.1f);
            }
            tg.InactiveTime = ((tg.InactiveTime > 0f) ? 0.1f : 12f);
            tg.RpcInstantBurst();
            tg.ServerSideIdle(shouldIdle: false);
        }

        private void ServerLockdown(global::System.Collections.Generic.IEnumerable<global::Interactables.Interobjects.DoorUtils.DoorVariant> doors)
        {
            bool inProgress = AlphaWarheadController.InProgress;
            foreach (global::Interactables.Interobjects.DoorUtils.DoorVariant door in doors)
            {
                if (door is global::Interactables.Interobjects.DoorUtils.INonInteractableDoor nonInteractableDoor && nonInteractableDoor.IgnoreLockdowns)
                {
                    continue;
                }
                global::Interactables.Interobjects.DoorUtils.DoorLockReason activeLocks = (global::Interactables.Interobjects.DoorUtils.DoorLockReason)door.ActiveLocks;
                if (!door.TargetState && (global::Interactables.Interobjects.DoorUtils.DoorLockUtils.HasFlagFast(activeLocks, global::Interactables.Interobjects.DoorUtils.DoorLockReason.Lockdown079) || global::Interactables.Interobjects.DoorUtils.DoorLockUtils.HasFlagFast(activeLocks, global::Interactables.Interobjects.DoorUtils.DoorLockReason.Lockdown2176) || global::Interactables.Interobjects.DoorUtils.DoorLockUtils.HasFlagFast(activeLocks, global::Interactables.Interobjects.DoorUtils.DoorLockReason.Regular079)))
                {
                    global::Interactables.Interobjects.DoorUtils.DoorScheduledUnlocker.UnlockLater(door, 0f, global::Interactables.Interobjects.DoorUtils.DoorLockReason.Lockdown2176);
                    if (!door.RequiredPermissions.Bypass2176)
                    {
                        door.TargetState = true;
                    }
                    continue;
                }
                global::Interactables.Interobjects.DoorUtils.DoorLockMode mode = global::Interactables.Interobjects.DoorUtils.DoorLockUtils.GetMode((global::Interactables.Interobjects.DoorUtils.DoorLockReason)door.ActiveLocks);
                if (global::Interactables.Interobjects.DoorUtils.DoorLockUtils.HasFlagFast(mode, global::Interactables.Interobjects.DoorUtils.DoorLockMode.CanClose) || global::Interactables.Interobjects.DoorUtils.DoorLockUtils.HasFlagFast(mode, global::Interactables.Interobjects.DoorUtils.DoorLockMode.ScpOverride))
                {
                    door.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.Lockdown2176, newState: true);
                    global::Interactables.Interobjects.DoorUtils.DoorScheduledUnlocker.UnlockLater(door, 13f, global::Interactables.Interobjects.DoorUtils.DoorLockReason.Lockdown2176);
                    if (!inProgress)
                    {
                        door.TargetState = false;
                    }
                }
            }
        }
	}
}
