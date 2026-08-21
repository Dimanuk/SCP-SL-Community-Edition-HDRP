using System.Collections.Generic;
using System.Text;
using AudioPooling;
using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.PlayableScps.Scp079.Rewards;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079BlackoutRoomAbility : Scp079KeyAbilityBase, IScp079LevelUpNotifier
    {
        private enum ValidationError
        {
            None = 0,
            NotEnoughAux = 1,
            NoController = 26,
            MaxCapacityReached = 27,
            RoomOnCooldown = 28,
            AlreadyBlackedOut = 60
        }

        [SerializeField]
        private int[] _capacityPerTier;

        [SerializeField]
        private float _blackoutDuration;

        [SerializeField]
        private float _cooldown;

        [SerializeField]
        private int _cost;

        private string _textUnlock;
        private string _textCapacityIncreased;
        private string _nameFormat;
        private string _failMessage;
        private bool _hasFailMessage;
        private bool _hasController;
        private FlickerableLightController _successfulController;
        private FlickerableLightController _roomController;

        private readonly Dictionary<uint, double> _blackoutCooldowns = new Dictionary<uint, double>();
        private readonly HashSet<uint> _obsoleteCooldowns = new HashSet<uint>();

        public override ActionName ActivationKey => ActionName.Scp079Blackout;

        public override bool IsReady => ErrorCode == Scp079HudTranslation.Zoom;

        public override bool IsVisible
        {
            get
            {
                if (CurrentCapacity <= 0)
                    return false;
                return !Scp079CursorManager.LockCameras;
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
                    case Scp079HudTranslation.BlackoutRoomCooldown:
                        return _failMessage + "\n" + base.AuxManager.GenerateCustomETA(Mathf.CeilToInt(RemainingCooldown));
                    case Scp079HudTranslation.BlackoutRoomLimit:
                        return string.Format(_failMessage, RoomsOnCooldown, CurrentCapacity);
                    default:
                        return _failMessage;
                }
            }
        }

        [field: SerializeField]
        public AudioClip ConfirmationSound { get; private set; }

        private int CurrentCapacity => GetCapacityOfTier(base.TierManager.AccessTierIndex);

        private int RoomsOnCooldown
        {
            get
            {
                int num = 0;
                bool flag = false;

                foreach (KeyValuePair<uint, double> blackoutCooldown in _blackoutCooldowns)
                {
                    if (blackoutCooldown.Value < NetworkTime.time)
                    {
                        _obsoleteCooldowns.Add(blackoutCooldown.Key);
                        flag = true;
                    }
                    else
                    {
                        num++;
                    }
                }

                if (!flag)
                    return num;

                foreach (uint obsoleteCooldown in _obsoleteCooldowns)
                    _blackoutCooldowns.Remove(obsoleteCooldown);

                _obsoleteCooldowns.Clear();
                return num;
            }
        }

        private float RemainingCooldown
        {
            get
            {
                if (!_hasController || !_blackoutCooldowns.TryGetValue(_roomController.netId, out double value))
                    return 0f;

                double num = value - NetworkTime.time;
                return Mathf.Max(0f, (float)num);
            }
        }

        private Scp079HudTranslation ErrorCode
        {
            get
            {
                if (!_hasController)
                    return Scp079HudTranslation.BlackoutRoomUnavailable;
                if (!_roomController.LightsEnabled)
                    return Scp079HudTranslation.BlackoutAlreadyActive;
                if (RemainingCooldown > 0f)
                    return Scp079HudTranslation.BlackoutRoomCooldown;
                if (RoomsOnCooldown >= CurrentCapacity)
                    return Scp079HudTranslation.BlackoutRoomLimit;
                if (base.AuxManager.CurrentAuxFloored < _cost)
                    return Scp079HudTranslation.NotEnoughAux;
                return Scp079HudTranslation.Zoom;
            }
        }

        private void RefreshCurrentController()
        {
            _hasController = false;
            _hasFailMessage = false;
            _failMessage = null;

            RoomIdentifier room = base.CurrentCamSync.CurrentCamera.Room;

            foreach (FlickerableLightController instance in FlickerableLightController.Instances)
            {
                if (instance.Room != room)
                    continue;

                float y = base.CurrentCamSync.CurrentCamera.Position.y;
                float y2 = instance.transform.position.y;

                if (Mathf.Abs(y - y2) > 100f)
                    continue;

                _roomController = instance;
                _hasController = true;
                break;
            }
        }

        private int GetCapacityOfTier(int index)
        {
            index = Mathf.Clamp(index, 0, _capacityPerTier.Length - 1);
            return _capacityPerTier[index];
        }

        protected override void Start()
        {
            base.Start();
            _nameFormat = Translations.Get(Scp079HudTranslation.ActivateRoomBlackout);
            _textUnlock = Translations.Get(Scp079HudTranslation.BlackoutRoomAvailable);
            _textCapacityIncreased = Translations.Get(Scp079HudTranslation.BlackoutCapacityIncreased);
            base.CurrentCamSync.OnCameraChanged += RefreshCurrentController;
        }

        protected override void Trigger()
        {
            ClientSendCmd();
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (IsReady && !base.LostSignalHandler.Lost)
            {
                 base.AuxManager.CurrentAux -= _cost;
                    base.RewardManager.MarkRoom(_roomController.Room);
                    _blackoutCooldowns[_roomController.netId] = NetworkTime.time + _cooldown;
                    _roomController.ServerFlickerLights(_blackoutDuration);
                    _successfulController = _roomController;
                    ServerSendRpc(toAll: true);
            }
            else
            {
                _successfulController = null;
                ServerSendRpc(toAll: false);
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            NetworkWriterExtensions.WriteNetworkBehaviour(writer, _successfulController);
            writer.WriteByte((byte)RoomsOnCooldown);

            foreach (KeyValuePair<uint, double> blackoutCooldown in _blackoutCooldowns)
            {
                NetworkWriterExtensions.WriteUInt(writer, blackoutCooldown.Key);
                NetworkWriterExtensions.WriteDouble(writer, blackoutCooldown.Value);
            }
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            _successfulController = NetworkReaderExtensions.ReadNetworkBehaviour<FlickerableLightController>(reader);

            if (_successfulController != null)
                PlaySoundForController(_successfulController);

            int num = reader.ReadByte();
            _blackoutCooldowns.Clear();

            for (int i = 0; i < num; i++)
            {
                uint key = NetworkReaderExtensions.ReadUInt(reader);
                double value = NetworkReaderExtensions.ReadDouble(reader);
                _blackoutCooldowns.Add(key, value);
            }
        }

        public void PlaySoundForController(FlickerableLightController flc)
        {
            Vector3 position = flc.transform.position + Vector3.down * 15f;
            AudioSourcePoolManager.PlaySound(ConfirmationSound, position, 37f, 1f, FalloffType.Linear).minDistance = 15f;
        }

        public override void OnFailMessageAssigned()
        {
            base.OnFailMessageAssigned();
            _hasFailMessage = true;
            _failMessage = Translations.Get(ErrorCode);
        }

        public bool WriteLevelUpNotification(StringBuilder sb, int newLevel)
        {
            int capacityOfTier = GetCapacityOfTier(newLevel);
            int capacityOfTier2 = GetCapacityOfTier(newLevel - 1);

            if (capacityOfTier <= capacityOfTier2)
                return false;

            if (capacityOfTier2 > 0)
                sb.AppendFormat(_textCapacityIncreased, capacityOfTier);
            else
                sb.AppendFormat(_textUnlock, $"[{new ReadableKeyCode(ActivationKey)}]");

            return true;
        }
    }
}