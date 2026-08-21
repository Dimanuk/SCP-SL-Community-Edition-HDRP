using System;
using AudioPooling;
using Interactables.Interobjects;
using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.PlayableScps.Scp079.Overcons;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079ElevatorStateChanger : Scp079KeyAbilityBase
    {
        [SerializeField]
        private AudioClip _confirmationSound;

        [SerializeField]
        private int _cost;

        private ElevatorDoor _lastElevator;
        private Scp079HudTranslation _failedReason;
        private string _abilityName;

        public override bool IsVisible
        {
            get
            {
                if (Scp079CursorManager.LockCameras)
                    return false;

                if (OverconManager.Singleton.HighlightedOvercon is ElevatorOvercon elevatorOvercon 
                    && elevatorOvercon != null)
                {
                    _lastElevator = elevatorOvercon.Target;
                    return true;
                }

                return false;
            }
        }

        public override bool IsReady
        {
            get
            {
                if (ErrorCode != Scp079HudTranslation.Zoom)
                    return false;

                return _lastElevator.TargetPanel.AssignedChamber.IsReady;
            }
        }

        public override string FailMessage
        {
            get
            {
                switch (_failedReason)
                {
                    case Scp079HudTranslation.Zoom:
                        return null;
                    case Scp079HudTranslation.NotEnoughAux:
                        return GetNoAuxMessage(_cost);
                    default:
                        return Translations.Get(_failedReason);
                }
            }
        }

        public override ActionName ActivationKey => ActionName.Shoot;

        public override string AbilityName => string.Format(_abilityName, _cost);

        private Scp079HudTranslation ErrorCode
        {
            get
            {
                if (base.AuxManager.CurrentAux < (float)_cost)
                    return Scp079HudTranslation.NotEnoughAux;

                if (!ValidateLastElevator)
                    return Scp079HudTranslation.ElevatorAccessDenied;

                return Scp079HudTranslation.Zoom;
            }
        }

        private bool ValidateLastElevator
        {
            get
            {
                if (_lastElevator == null)
                    return false;

                if (!ElevatorDoor.AllElevatorDoors.TryGetValue(_lastElevator.Group, out var doors))
                    return false;

                if (ListExtensions.Any(doors, x => x.ActiveLocks != 0))
                    return false;

                return true;
            }
        }

        public static event Action<Scp079Role, ElevatorDoor> OnServerElevatorDoorClosed;

        protected override void Start()
        {
            base.Start();

            _translationNoAux = Translations.Get(Scp079HudTranslation.NotEnoughAux);
            _abilityName = Translations.Get(Scp079HudTranslation.SendElevator);

            base.CurrentCamSync.OnCameraChanged += () => _failedReason = Scp079HudTranslation.Zoom;
        }

        protected override void Trigger()
        {
            ClientSendCmd();
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            writer.WriteByte((byte)_lastElevator.Group);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (base.AuxManager.CurrentAux < (float)_cost || base.LostSignalHandler.Lost)
                return;

            ElevatorManager.ElevatorGroup elevatorGroup = (ElevatorManager.ElevatorGroup)reader.ReadByte();

            if (!ElevatorDoor.AllElevatorDoors.TryGetValue(elevatorGroup, out var doors) 
                || ListExtensions.Any(doors, x => x.ActiveLocks != 0))
            {
                return;
            }

            RoomIdentifier curRoom = base.CurrentCamSync.CurrentCamera.Room;

            if (!ListExtensions.TryGetFirst(doors, x => x.Rooms.Contains(curRoom), out var first))
                return;

            bool targetState = first.TargetState;
            int destination = (first.TargetPanel.AssignedChamber.CurrentLevel + 1) % doors.Count;

            if (ElevatorManager.TrySetDestination(elevatorGroup, destination))
            {
                base.AuxManager.CurrentAux -= _cost;
                doors.ForEach(x => base.RewardManager.MarkRooms(x.Rooms));
                ServerSendRpc(toAll: false);

                if (targetState)
                    OnServerElevatorDoorClosed?.Invoke(base.ScpRole, first);
            }
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            AudioSourcePoolManager.PlaySound(
                _confirmationSound, 
                null, 
                1f, 
                1f, 
                FalloffType.Exponential, 
                AudioMixerChannelType.DefaultSfx, 
                0f);
        }

        public override void OnFailMessageAssigned()
        {
            _failedReason = ErrorCode;
        }
    }
}