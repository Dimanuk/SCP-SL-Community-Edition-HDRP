using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using UnityEngine;
using Utils.NonAllocLINQ;

using Vector3 = UnityEngine.Vector3;

namespace PlayerRoles.PlayableScps.Scp079.Cameras
{
	public class Scp079CurrentCameraSync : ScpStandardSubroutine<Scp079Role>, IScp079FailMessageProvider
	{
		public enum ClientSwitchState
		{
			None = 0,
			SwitchingRoom = 1,
			SwitchingZone = 2
		}

		public const float CostPerMeter = 0.16f;
		public const int CostPerFloor = 10;
		public const int CostPerZone = 20;
		public const int CostPerSkippedZone = 30;

		public static readonly FacilityZone[] ZoneQueue = new FacilityZone[4]
		{
			FacilityZone.Surface,
			FacilityZone.Entrance,
			FacilityZone.HeavyContainment,
			FacilityZone.LightContainment
		};

		private const float FloorHeight = 100f;
		private const int ErrorTranslationId = 2;
		private const float SameRoomSwitchDuration = 0.1f;
		private const float ZoneSwitchDuration = 0.98f;
		private const string DefaultCameraName = "079 CONT CHAMBER";

		private readonly Stopwatch _switchStopwatch = new Stopwatch();
		private Scp079Camera _lastCam;
		private Scp079Camera _switchTarget;
		private Scp079AuxManager _auxManager;
		private Scp079LostSignalHandler _lostSignalHandler;
		private bool _camSet;
		private bool _eventAssigned;
		private float _targetSwitchTime;
		private ushort _defaultCamId;
		private ushort _requestedCamId;
		private bool _initialized;
		private Scp079HudTranslation _errorCode;
		private ClientSwitchState _clientSwitchRequest;

		public string FailMessage { get; private set; }
		public ClientSwitchState CurClientSwitchState { get; private set; }
		public FacilityZone CurClientTargetZone { get; private set; }

		public Scp079Camera CurrentCamera
		{
			get
			{
				TryGetCurrentCamera(out var cam);
				return cam;
			}
			private set
			{
				value.IsActive = true;
				_camSet = true;
				if (value == _lastCam)
					return;

				_lastCam = value;
				OnCameraChanged?.Invoke();

				if (NetworkServer.active)
					ServerSendRpc(toAll: true);
			}
		}

		public event Action OnCameraChanged;

		private void Update()
		{
			if (!_initialized)
			{
				if (!TryGetCurrentCamera(out var _))
					return;
				_initialized = true;
			}

			if (CurClientSwitchState == ClientSwitchState.None)
				return;

			if (!base.Role.IsLocalPlayer)
				return;

			if (_switchStopwatch.Elapsed.TotalSeconds < _targetSwitchTime)
				return;

			_clientSwitchRequest = ClientSwitchState.None;
			_switchStopwatch.Stop();
			ClientSendCmd();
		}

		private bool TryGetDefaultCamera(out Scp079Camera cam)
		{
			if (_defaultCamId == 0)
			{
				if (Scp079InteractableBase.AllInstances.TryGetFirst(
					x => x is Scp079Camera scp079Camera
						&& x.SyncId != 0
						&& scp079Camera.Room.Name == RoomName.Hcz079
						&& scp079Camera.Label.Equals("079 CONT CHAMBER", StringComparison.InvariantCultureIgnoreCase),
					out var first))
				{
					cam = first as Scp079Camera;
					_defaultCamId = cam.SyncId;
					return true;
				}
				cam = null;
				return false;
			}

			if (Scp079InteractableBase.TryGetInteractable(_defaultCamId, out Scp079Camera result))
			{
				cam = result;
				return true;
			}
			cam = null;
			return false;
		}

		private void OnHubAdded(ReferenceHub hub)
		{
			if (NetworkServer.active)
				ServerSendRpc(hub);
		}

		public int GetSwitchCost(Scp079Camera target)
		{
			Vector3 position = CurrentCamera.Position;
			Vector3 position2 = target.Position;
			FacilityZone zone = CurrentCamera.Room.Zone;
			FacilityZone zone2 = target.Room.Zone;

			bool sameFloor = Mathf.Abs(position.y - position2.y) < 100f;
			bool sameZone = zone == zone2;

			int cost = !sameFloor
				? CostPerFloor
				: Mathf.CeilToInt(Vector3.Distance(position, position2) * CostPerMeter);

			if (sameZone)
			{
				if (!sameFloor)
					return cost;

				if (!CurrentCamera.Room.TryGetMainCoords(out var coords))
					return cost;
				if (!target.Room.TryGetMainCoords(out var coords2))
					return cost;
				if ((coords - coords2).magnitude > 1f)
					return cost;

				return 0; // Adjacent rooms, same floor, same zone = free
			}

			int num2 = Mathf.Abs(ZoneQueue.IndexOf(zone) - ZoneQueue.IndexOf(zone2)) - 1;
			return cost + CostPerZone + CostPerSkippedZone * num2;
		}

		public void ClientSwitchTo(Scp079Camera target)
		{
			if (!base.Role.IsLocalPlayer)
				throw new InvalidOperationException("Method ClientSwitchTo can only be called on the local client instance!");

			if (CurClientSwitchState != ClientSwitchState.None)
				return;

			bool sameZone = target.Room.Zone == CurrentCamera.Room.Zone;
			_switchTarget = target;
			_targetSwitchTime = sameZone ? SameRoomSwitchDuration : ZoneSwitchDuration;
			CurClientSwitchState = sameZone ? ClientSwitchState.SwitchingRoom : ClientSwitchState.SwitchingZone;
			CurClientTargetZone = target.Room.Zone;
			_clientSwitchRequest = CurClientSwitchState;
			ClientSendCmd();
			_switchStopwatch.Restart();
		}

		public override void SpawnObject()
		{
			base.SpawnObject();
			if (!NetworkServer.active)
				return;

			_eventAssigned = true;
			ReferenceHub.OnPlayerAdded = (Action<ReferenceHub>)Delegate.Combine(
				ReferenceHub.OnPlayerAdded,
				new Action<ReferenceHub>(OnHubAdded));

			(base.Role as Scp079Role).SubroutineModule.TryGetSubroutine(out _auxManager);
			(base.Role as Scp079Role).SubroutineModule.TryGetSubroutine(out _lostSignalHandler);
		}

		public override void ResetObject()
		{
			base.ResetObject();
			if (_eventAssigned)
			{
				ReferenceHub.OnPlayerAdded = (Action<ReferenceHub>)Delegate.Remove(
					ReferenceHub.OnPlayerAdded,
					new Action<ReferenceHub>(OnHubAdded));
			}

			_defaultCamId = 0;
			_errorCode = Scp079HudTranslation.Zoom;
			_lastCam = null;
			_camSet = false;
			_initialized = false;
			_eventAssigned = false;
			CurClientSwitchState = ClientSwitchState.None;
		}

		public override void ClientWriteCmd(NetworkWriter writer)
		{
			base.ClientWriteCmd(writer);
			writer.WriteByte((byte)_clientSwitchRequest);
			NetworkWriterExtensions.WriteUShort(writer, _switchTarget.SyncId);
		}

		public override void ServerProcessCmd(NetworkReader reader)
		{
			base.ServerProcessCmd(reader);
			_clientSwitchRequest = (ClientSwitchState)reader.ReadByte();
			_requestedCamId = NetworkReaderExtensions.ReadUShort(reader);

			if (_clientSwitchRequest != ClientSwitchState.None)
			{
				// Switching in progress - notify spectators
				ServerSendRpc(x => x.roleManager.CurrentRole is SpectatorRole);
				return;
			}

			if (!Scp079InteractableBase.TryGetInteractable(_requestedCamId, out _switchTarget))
			{
				_errorCode = Scp079HudTranslation.InvalidCamera;
				ServerSendRpc(toAll: true);
				return;
			}

			float cost = GetSwitchCost(_switchTarget);
			if (cost > _auxManager.CurrentAux)
			{
				_errorCode = Scp079HudTranslation.NotEnoughAux;
				ServerSendRpc(toAll: true);
				return;
			}

			if (_lostSignalHandler.Lost)
			{
				_errorCode = Scp079HudTranslation.SignalLost;
				ServerSendRpc(toAll: true);
				return;
			}

			_auxManager.CurrentAux -= cost;
			_errorCode = Scp079HudTranslation.Zoom;

			if (_switchTarget != CurrentCamera)
				CurrentCamera = _switchTarget;
			else
				ServerSendRpc(toAll: true);
		}

		public override void ServerWriteRpc(NetworkWriter writer)
		{
			base.ServerWriteRpc(writer);
			writer.WriteByte((byte)_clientSwitchRequest);

			if (_clientSwitchRequest == ClientSwitchState.None)
			{
				NetworkWriterExtensions.WriteUShort(writer, CurrentCamera.SyncId);
				writer.WriteByte((byte)_errorCode);
			}
			else
			{
				NetworkWriterExtensions.WriteUShort(writer, _requestedCamId);
			}

			_errorCode = Scp079HudTranslation.Zoom;
		}

		public override void ClientProcessRpc(NetworkReader reader)
		{
			base.ClientProcessRpc(reader);
			CurClientSwitchState = (ClientSwitchState)reader.ReadByte();
			ushort num = NetworkReaderExtensions.ReadUShort(reader);

			bool flag = Scp079InteractableBase.TryGetInteractable(num, out Scp079Camera result);

			switch (CurClientSwitchState)
			{
				case ClientSwitchState.SwitchingRoom:
					return;
				case ClientSwitchState.SwitchingZone:
					if (flag)
						CurClientTargetZone = result.Room.Zone;
					return;
			}

			_errorCode = (Scp079HudTranslation)reader.ReadByte();

			if (_errorCode != Scp079HudTranslation.Zoom)
			{
				if (base.Role.IsLocalPlayer || base.ScpRole.IsSpectated)
					Scp079AbilityList.Singleton.TrackedFailMessage = this;
			}
			else if (!NetworkServer.active)
			{
				if (flag)
					CurrentCamera = result;
				else
				{
					_camSet = false;
					_defaultCamId = num;
				}
			}
		}

		public bool TryGetCurrentCamera(out Scp079Camera cam)
		{
			if (base.Role.Pooled)
			{
				cam = null;
				return false;
			}

			if (_camSet)
			{
				cam = _lastCam;
				return true;
			}

			if (TryGetDefaultCamera(out var cam2))
			{
				cam = cam2;
				CurrentCamera = cam;
				return true;
			}

			cam = null;
			return false;
		}

		public void OnFailMessageAssigned()
		{
			FailMessage = Translations.Get(_errorCode);
		}
	}
}
