using System;
using System.Collections.Generic;
using System.Text;
using GameObjectPools;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.GUI;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079LockdownRoomAbility : Scp079KeyAbilityBase, IPoolResettable, IScp079LevelUpNotifier
    {
        private enum ValidationError
        {
            None = 0,
            Unknown = 1,
            NotEnoughAux = 6,
            TierTooLow = 8,
            Cooldown = 31,
            NoDoors = 32
        }

        [SerializeField]
        private int _minimalTierIndex;

        [SerializeField]
        private float[] _regenerationPerTier;

        [SerializeField]
        private float _lockdownDuration;

        [SerializeField]
        private float _cooldown;

        [SerializeField]
        private int _cost;

        [SerializeField]
        private float _minStateToClose;

        private string _nameFormat;
        private string _failMessage;
        private string _unlockText;
        private double _nextUseTime;
        private bool _hasFailMessage;
        private bool _lockdownInEffect;
        private Scp079DoorLockChanger _doorLockChanger;

        private readonly HashSet<DoorVariant> _roomDoors = new HashSet<DoorVariant>();
        private readonly HashSet<DoorVariant> _doorsToLockDown = new HashSet<DoorVariant>();
        private readonly HashSet<DoorVariant> _alreadyLockedDown = new HashSet<DoorVariant>();

        private RoomIdentifier _lastLockedRoom;

        public override ActionName ActivationKey => ActionName.Scp079Lockdown;

        public override bool IsReady => ErrorCode == Scp079HudTranslation.Zoom;

        public override bool IsVisible
        {
            get
            {
                if (Scp079CursorManager.LockCameras)
                    return false;

                return ErrorCode != Scp079HudTranslation.HigherTierRequired;
            }
        }

        public override string AbilityName => string.Format(_nameFormat, _cost);

        public override string FailMessage
        {
            get
            {
                if (!_hasFailMessage)
                    return null;

                switch (ErrorCode)
                {
                    case Scp079HudTranslation.Zoom:
                        return null;
                    case Scp079HudTranslation.NotEnoughAux:
                        return GetNoAuxMessage(_cost);
                    case Scp079HudTranslation.LockdownCooldown:
                        return _failMessage + "\n" + base.AuxManager.GenerateCustomETA(Mathf.CeilToInt(RemainingCooldown));
                    default:
                        return _failMessage;
                }
            }
        }

        public override float AuxRegenMultiplier
        {
            get
            {
                if (RemainingLockdownDuration == 0f)
                    return 1f;

                int accessTierIndex = base.TierManager.AccessTierIndex;
                int a = _regenerationPerTier.Length - 1;
                return _regenerationPerTier[Mathf.Min(a, accessTierIndex)];
            }
        }

        private Scp079HudTranslation ErrorCode
        {
            get
            {
                if (base.TierManager.AccessTierIndex < _minimalTierIndex)
                    return Scp079HudTranslation.HigherTierRequired;

                if (!HashsetExtensions.Any(_roomDoors, x => ValidateDoor(x)))
                    return Scp079HudTranslation.LockdownNoDoorsError;

                if (RemainingCooldown > 0f)
                    return Scp079HudTranslation.LockdownCooldown;

                if (base.AuxManager.CurrentAuxFloored < _cost)
                    return Scp079HudTranslation.NotEnoughAux;

                return Scp079HudTranslation.Zoom;
            }
        }

        private float RemainingCooldown
        {
            get => Mathf.Max(0f, (float)(_nextUseTime - NetworkTime.time));
            set => _nextUseTime = NetworkTime.time + (double)value;
        }

        private float RemainingLockdownDuration =>
            Mathf.Max(0f, (float)(_nextUseTime - (double)_cooldown - NetworkTime.time));

        public static event Action<Scp079Role, RoomIdentifier> OnServerLockdown;
        public static event Action<Scp079Role, DoorVariant> OnServerDoorLocked;

        private void ServerInitLockdown()
        {
            _lockdownInEffect = true;
            _lastLockedRoom = base.CurrentCamSync.CurrentCamera.Room;
            _doorsToLockDown.UnionWith(_roomDoors);
            OnServerLockdown?.Invoke(base.ScpRole, _lastLockedRoom);
        }

        private void ServerCancelLockdown()
        {
            _lockdownInEffect = false;
            RemainingCooldown = _cooldown;

            foreach (DoorVariant item in _alreadyLockedDown)
            {
                _doorLockChanger.SetDoorLock(item, lockState: false, skipChecks: true);
                item.ServerChangeLock(DoorLockReason.Lockdown079, newState: false);
            }

            _doorsToLockDown.Clear();
            _alreadyLockedDown.Clear();
            ServerSendRpc(toAll: false);
        }

        private bool ValidateDoor(DoorVariant dv)
        {
            Scp079Camera currentCamera = base.CurrentCamSync.CurrentCamera;
            if (!Scp079DoorAbility.ValidateAction(DoorAction.Closed, dv, currentCamera))
                return false;

            return Scp079DoorAbility.ValidateAction(DoorAction.Locked, dv, currentCamera);
        }

        protected override void Start()
        {
            base.Start();

            _translationNoAux = Translations.Get(Scp079HudTranslation.NotEnoughAux);
            _nameFormat = Translations.Get(Scp079HudTranslation.Lockdown);
            _unlockText = Translations.Get(Scp079HudTranslation.LockdownAvailable);

            base.CurrentCamSync.OnCameraChanged += () =>
            {
                _hasFailMessage = false;
                _failMessage = null;
                _roomDoors.Clear();

                if (DoorVariant.DoorsByRoom.TryGetValue(base.CurrentCamSync.CurrentCamera.Room, out var value))
                    _roomDoors.UnionWith(value);
            };

            GetSubroutine<Scp079DoorLockChanger>(out _doorLockChanger);
        }

        protected override void Update()
        {
            base.Update();

            if (!_lockdownInEffect || !NetworkServer.active)
                return;

            if (RemainingLockdownDuration <= 0f)
            {
                ServerCancelLockdown();
                return;
            }

            foreach (DoorVariant item in _doorsToLockDown)
            {
                if (!ValidateDoor(item) || _alreadyLockedDown.Contains(item))
                    continue;

                if (item.TargetState && item.GetExactState() < _minStateToClose)
                    continue;

                item.TargetState = false;
                item.ServerChangeLock(DoorLockReason.Lockdown079, newState: true);
                _doorLockChanger.SetDoorLock(item, lockState: false, skipChecks: true);
                base.RewardManager.MarkRooms(item.Rooms);
                OnServerDoorLocked?.Invoke(base.ScpRole, item);
                _alreadyLockedDown.Add(item);
            }
        }

        protected override void Trigger()
        {
            ClientSendCmd();
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (ErrorCode == Scp079HudTranslation.Zoom && !base.LostSignalHandler.Lost)
            {
                base.AuxManager.CurrentAux -= _cost;
                RemainingCooldown = _lockdownDuration + _cooldown;
                ServerInitLockdown();
            }

            ServerSendRpc(toAll: false);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            NetworkWriterExtensions.WriteDouble(writer, _nextUseTime);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            _nextUseTime = NetworkReaderExtensions.ReadDouble(reader);
        }

        public override void OnFailMessageAssigned()
        {
            base.OnFailMessageAssigned();
            _hasFailMessage = true;
            _failMessage = Translations.Get(ErrorCode);
        }

        public override void ResetObject()
        {
            base.ResetObject();

            if (NetworkServer.active)
                ServerCancelLockdown();
        }

        public static bool IsLockedDown(DoorVariant dv)
        {
            return DoorLockUtils.HasFlagFast((DoorLockReason)dv.ActiveLocks, DoorLockReason.Lockdown079);
        }

        public bool WriteLevelUpNotification(StringBuilder sb, int newLevel)
        {
            if (newLevel != _minimalTierIndex)
                return false;

            sb.AppendFormat(_unlockText, $"[{new ReadableKeyCode(ActivationKey)}]");
            return true;
        }
    }
}
