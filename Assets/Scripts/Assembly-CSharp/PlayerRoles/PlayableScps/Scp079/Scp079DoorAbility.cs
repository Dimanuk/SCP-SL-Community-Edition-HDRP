using System;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.PlayableScps.Scp079.Overcons;
using UnityEngine;
using AudioPooling;

namespace PlayerRoles.PlayableScps.Scp079
{
    public abstract class Scp079DoorAbility : Scp079KeyAbilityBase
    {
        #region Fields

        protected DoorVariant LastDoor;

        [SerializeField]
        private AudioClip _confirmationSound;

        private static string _deniedText;

        private int _lastCost;
        private bool _lastActionValid;
        private int _failMessageAux;
        private bool _failMessageDenied;

        #endregion

        #region Properties

        public override bool IsVisible
        {
            get
            {
                if (Scp079CursorManager.LockCameras)
                    return false;

                if (OverconManager.Singleton.HighlightedOvercon is DoorOvercon doorOvercon && doorOvercon != null)
                {
                    LastDoor = doorOvercon.Target;
                    return true;
                }

                return false;
            }
        }

        public override bool IsReady
        {
            get
            {
                DoorAction targetAction = TargetAction;

                _lastActionValid = ValidateAction(targetAction, LastDoor, CurrentCamSync.CurrentCamera);
                _lastCost = GetCostForDoor(targetAction, LastDoor);

                if (_lastActionValid)
                    return (float)_lastCost <= AuxManager.CurrentAux;

                return false;
            }
        }

        public override string FailMessage
        {
            get
            {
                if (_failMessageDenied)
                    return _deniedText;

                if (!(AuxManager.CurrentAux < (float)_failMessageAux))
                    return null;

                return GetNoAuxMessage(_failMessageAux);
            }
        }

        protected abstract DoorAction TargetAction { get; }

        #endregion

        #region Events

        public static event Action<Scp079Role, DoorVariant> OnServerAnyDoorInteraction;

        protected abstract int GetCostForDoor(DoorAction action, DoorVariant door);

        #endregion

        #region Overrides

        protected override void Trigger()
        {
            ClientSendCmd();
        }

        protected override void Start()
        {
            base.Start();

            _deniedText = Translations.Get(Scp079HudTranslation.DoorAccessDenied);

            CurrentCamSync.OnCameraChanged += delegate
            {
                _failMessageAux = 0;
                _failMessageDenied = false;
            };
        }

        public override void OnFailMessageAssigned()
        {
            _failMessageDenied = !_lastActionValid;
            _failMessageAux = _lastCost;
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
                0f
            );
        }

        #endregion

        #region Static Validation

        public static bool ValidateAction(
            DoorAction action,
            DoorVariant door,
            Scp079Camera currentCamera)
        {
            if (!CheckVisibility(door, currentCamera))
                return false;

            if (NetworkServer.active && AlphaWarheadController.InProgress)
                return false;

            DoorLockMode mode = DoorLockUtils.GetMode((DoorLockReason)door.ActiveLocks);

            if (door is IDamageableDoor damageableDoor && damageableDoor.IsDestroyed)
                return false;

            if (DoorLockUtils.HasFlagFast(mode, DoorLockMode.ScpOverride))
                return true;

            switch (action)
            {
                case DoorAction.Opened:
                    return DoorLockUtils.HasFlagFast(mode, DoorLockMode.CanOpen);

                case DoorAction.Closed:
                    return DoorLockUtils.HasFlagFast(mode, DoorLockMode.CanClose);

                case DoorAction.Locked:
                    if (mode != DoorLockMode.FullLock)
                        return !(door is CheckpointDoor);
                    return false;

                case DoorAction.Unlocked:
                    return true;

                default:
                    return false;
            }
        }

        public static bool CheckVisibility(DoorVariant door, Scp079Camera currentCamera)
        {
            RoomIdentifier[] rooms = door.Rooms;

            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i] == currentCamera.Room)
                {
                    if (door is INonInteractableDoor nonInteractableDoor)
                        return !nonInteractableDoor.IgnoreLockdowns;

                    return true;
                }
            }

            return false;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            Scp079DoorLockChanger.OnServerDoorLocked += (role, dv) =>
                OnServerAnyDoorInteraction?.Invoke(role, dv);

            Scp079DoorStateChanger.OnServerDoorToggled += (role, dv) =>
                OnServerAnyDoorInteraction?.Invoke(role, dv);

            Scp079ElevatorStateChanger.OnServerElevatorDoorClosed += (role, dv) =>
                OnServerAnyDoorInteraction?.Invoke(role, dv);

            Scp079LockdownRoomAbility.OnServerDoorLocked += (role, dv) =>
                OnServerAnyDoorInteraction?.Invoke(role, dv);
        }

        #endregion
    }
}
