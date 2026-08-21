using System;
using System.Collections.Generic;
using GameObjectPools;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.Cameras;

using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp079
{
	public class Scp079DoorLockChanger : Scp079DoorAbility, IPoolResettable
	{

		[SerializeField]
		private AnimationCurve _costOverTime;

		[SerializeField]
		private int _startCost;

		[SerializeField]
		private float _reLockCooldown;

		private readonly HashSet<DoorVariant> _lockedDoors;

		private readonly HashSet<DoorVariant> _doorsToUnlock;

		private readonly Dictionary<DoorVariant, double> _toggleTimes;

		private static string _lockText;
		private static string _unlockText;
		private static string _cooldownText;

		private DoorVariant _failedDoor;

		private bool _syncvarsDirty;

		private int _prevLockedCount;

		private const DoorLockReason LockReason = DoorLockReason.Regular079;

		public int TotalLocked => _lockedDoors.Count;

		public override ActionName ActivationKey => ActionName.Zoom;

		public override float AuxRegenMultiplier
		{
			get
			{
				if (_lockedDoors.Count <= 0)
					return 1f;
				return 0f;
			}
		}

		public override string AbilityName
		{
			get
			{
				bool isLocked = IsLockedBy079;
				string text = isLocked ? _unlockText : _lockText;
				int cost = GetCostForDoor(TargetAction, LastDoor);
				return string.Format(text, cost);
			}
		}

		public override bool IsReady
		{
			get
			{
				if (GetRemainingCooldown(LastDoor) == 0f)
					return base.IsReady;
				return false;
			}
		}

		public override string FailMessage
		{
			get
			{
				if (Scp079LockdownRoomAbility.IsLockedDown(_failedDoor))
					return _cooldownText;

				float remainingCooldown = GetRemainingCooldown(_failedDoor);
				if (remainingCooldown <= 0f)
					return base.FailMessage;

				int secondsRemaining = Mathf.CeilToInt(remainingCooldown);
				return _cooldownText + "\n" + AuxManager.GenerateCustomETA(secondsRemaining);
			}
		}

		protected override DoorAction TargetAction
		{
			get
			{
				if (!IsLockedBy079)
					return DoorAction.Locked;
				return DoorAction.Unlocked;
			}
		}

		private bool IsLockedBy079
		{
			get
			{
				if (LastDoor == null)
					return false;
				return DoorLockUtils.HasFlagFast((DoorLockReason)LastDoor.ActiveLocks, DoorLockReason.Regular079);
			}
		}

		private double CurrentTime => NetworkTime.time;

		public static event Action<Scp079Role, DoorVariant> OnServerDoorLocked;

		public Scp079DoorLockChanger()
		{
			_lockedDoors = new HashSet<DoorVariant>();
			_doorsToUnlock = new HashSet<DoorVariant>();
			_toggleTimes = new Dictionary<DoorVariant, double>();
		}

		private float GetRemainingCooldown(DoorVariant dv)
		{
			if (_lockedDoors.Contains(dv) || !_toggleTimes.TryGetValue(dv, out double toggleTime))
				return 0f;

			double remaining = toggleTime + _reLockCooldown - CurrentTime;
			return Mathf.Max(0f, (float)remaining);
		}

		protected override int GetCostForDoor(DoorAction action, DoorVariant door)
		{
			if (action != DoorAction.Locked)
				return 0;
			return _startCost;
		}

		protected virtual void OnDestroy()
		{
			ServerUnlockAll();
		}

		protected override void Start()
		{
			base.Start();

			_lockText = Translations.Get(Scp079HudTranslation.LockDoor);
			_unlockText = Translations.Get(Scp079HudTranslation.UnlockDoor);
			_cooldownText = Translations.Get(Scp079HudTranslation.DoorLockCooldown);

			LostSignalHandler.OnStatusChanged += delegate
			{
				if (NetworkServer.active && LostSignalHandler.Lost)
					ServerUnlockAll();
			};
		}

		protected override void Update()
		{
			base.Update();

			if (!NetworkServer.active)
				return;

			Scp079Camera currentCamera = CurrentCamSync.CurrentCamera;
			float aux = AuxManager.CurrentAux;
			bool needsUnlock = false;

			foreach (DoorVariant lockedDoor in _lockedDoors)
			{
				float elapsed = (float)(CurrentTime - _toggleTimes[lockedDoor]);
				aux -= _costOverTime.Evaluate(elapsed) * Time.deltaTime;

				if (!Scp079DoorAbility.ValidateAction(DoorAction.Locked, lockedDoor, currentCamera))
				{
					_doorsToUnlock.Add(lockedDoor);
					needsUnlock = true;
				}
			}

			AuxManager.CurrentAux = aux;

			if (aux <= 0f)
			{
				ServerUnlockAll();
			}
			else if (needsUnlock)
			{
				HashsetExtensions.ForEach(_doorsToUnlock, x => SetDoorLock(x, false));
				_doorsToUnlock.Clear();
			}

			if (_syncvarsDirty)
			{
				ServerSendRpc(toAll: false);
				_syncvarsDirty = false;
			}
		}

		public void ServerUnlockAll()
		{
			if (!NetworkServer.active)
				return;

			HashSet<DoorVariant> doorsToRemove = new HashSet<DoorVariant>();

			foreach (DoorVariant lockedDoor in _lockedDoors)
			{
				lockedDoor.ServerChangeLock(LockReason, false);
				_toggleTimes[lockedDoor] = CurrentTime;
				doorsToRemove.Add(lockedDoor);
			}

			_syncvarsDirty = true;
			_lockedDoors.RemoveWhere(x => doorsToRemove.Contains(x));
		}

		public bool SetDoorLock(DoorVariant door, bool lockState, bool skipChecks = false)
		{
			if (door == null)
				return false;

			door.ServerChangeLock(LockReason, lockState);
			_syncvarsDirty = true;

			if (lockState)
			{
				if (!_lockedDoors.Add(door) && !skipChecks)
					return false;
			}
			else
			{
				if (!_lockedDoors.Remove(door) && !skipChecks)
					return false;
			}

			_toggleTimes[door] = CurrentTime;
			return true;
		}

		public override void OnFailMessageAssigned()
		{
			base.OnFailMessageAssigned();
			_failedDoor = LastDoor;
		}

		public override void ClientWriteCmd(NetworkWriter writer)
		{
			base.ClientWriteCmd(writer);
			NetworkWriterExtensions.WriteUInt(writer, LastDoor.netId);
		}

		public override void ServerProcessCmd(NetworkReader reader)
		{
			base.ServerProcessCmd(reader);

			uint netId = NetworkReaderExtensions.ReadUInt(reader);

			if (!NetworkServer.spawned.TryGetValue(netId, out NetworkIdentity identity))
				return;
			if (!identity.TryGetComponent(out DoorVariant door))
				return;

			LastDoor = door;

			if (!IsReady)
				return;

			if (TargetAction == DoorAction.Locked)
			{
				if (GetRemainingCooldown(LastDoor) > 0f)
					return;

				if (Scp079LockdownRoomAbility.IsLockedDown(LastDoor))
					return;

				if (LostSignalHandler.Lost)
					return;

				SetDoorLock(LastDoor, true);
				RewardManager.MarkRooms(LastDoor.Rooms);
				OnServerDoorLocked?.Invoke(ScpRole, LastDoor);
				AuxManager.CurrentAux -= GetCostForDoor(DoorAction.Locked, LastDoor);
			}
			else
			{
				SetDoorLock(LastDoor, false);
			}
		}

		public override void ServerWriteRpc(NetworkWriter writer)
		{
			base.ServerWriteRpc(writer);

			foreach (var kvp in _toggleTimes)
			{
				DoorVariant door = kvp.Key;
				double time = kvp.Value;

				if (_lockedDoors.Contains(door))
				{
					NetworkWriterExtensions.WriteDouble(writer, time);
					NetworkWriterExtensions.WriteUInt(writer, door.netId);
				}
				else if (time > CurrentTime - _reLockCooldown)
				{
					NetworkWriterExtensions.WriteDouble(writer, time * -1.0);
					NetworkWriterExtensions.WriteUInt(writer, door.netId);
				}
			}
		}

		public override void ClientProcessRpc(NetworkReader reader)
		{
			int lockedCount = 0;
			_lockedDoors.Clear();
			_toggleTimes.Clear();

			while (reader.Position < reader.buffer.Count)
			{
				double time = NetworkReaderExtensions.ReadDouble(reader);

				if (!NetworkClient.spawned.TryGetValue(NetworkReaderExtensions.ReadUInt(reader), out NetworkIdentity identity))
					break;

				if (!identity.TryGetComponent(out DoorVariant door))
					break;

				if (time >= 0.0)
				{
					if (_lockedDoors.Add(door))
						lockedCount++;

					_toggleTimes[door] = time;
				}
				else
				{
					_toggleTimes[door] = time * -1.0;
				}
			}

			if (_prevLockedCount != lockedCount)
			{
				base.ClientProcessRpc(reader);
				_prevLockedCount = lockedCount;
			}
		}

		public override void ResetObject()
		{
			base.ResetObject();
			ServerUnlockAll();
			_lockedDoors.Clear();
			_toggleTimes.Clear();
			_prevLockedCount = 0;
		}

	}
}
