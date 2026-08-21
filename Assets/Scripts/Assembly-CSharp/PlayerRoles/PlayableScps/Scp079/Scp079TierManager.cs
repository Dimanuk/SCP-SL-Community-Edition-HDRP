using System;
using System.Collections.Generic;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079TierManager : ScpStandardSubroutine<Scp079Role>
    {
        // IL2CPP offsets:
        // 0x38 (56)  -> _expGainQueue
        // 0x40 (64)  -> _totalExp
        // 0x44 (68)  -> _valueDirty
        // 0x48 (72)  -> _accessTier
        // 0x4C (76)  -> _thresholdsCount
        // 0x50 (80)  -> <AbsoluteThresholds>k__BackingField
        // 0x58 (88)  -> _levelupThresholds
        // 0x60 (96)  -> OnLevelledUp
        // 0x68 (104) -> OnTierChanged
        // 0x70 (112) -> OnExpChanged

        private readonly Queue<KeyValuePair<Scp079HudTranslation, int>> _expGainQueue;

        private int _totalExp;
        private bool _valueDirty;
        private int _accessTier;
        private int _thresholdsCount;

        [SerializeField]
        private int[] _levelupThresholds;

        public int[] AbsoluteThresholds { get; private set; }

        public Action OnLevelledUp;
        public Action OnTierChanged;
        public Action OnExpChanged;

        public int TotalExp
        {
            get => _totalExp;
            set
            {
                _totalExp = value;
                OnExpChanged?.Invoke();

                int num = 0;
                for (int i = 0; i < _thresholdsCount && _totalExp >= AbsoluteThresholds[i]; i++)
                    num++;

                AccessTierIndex = num;

                if (NetworkServer.active)
                    _valueDirty = true;
            }
        }

        public int RelativeExp
        {
            get
            {
                int index = AccessTierIndex - 1;
                if (index < 0)
                {
                    return Mathf.FloorToInt(TotalExp);
                }

                float progress = TotalExp - AbsoluteThresholds[index];
                return Mathf.Min(NextLevelThreshold, Mathf.FloorToInt(progress));
            }
        }

        public int NextLevelThreshold
        {
            get
            {
                if (AccessTierIndex >= _thresholdsCount)
                    return -1;

                return _levelupThresholds[AccessTierIndex];
            }
        }

        public int AccessTierIndex
        {
            get => Mathf.Clamp(_accessTier, 0, _thresholdsCount);
            private set
            {
                if (_accessTier == value)
                    return;

                int diff = value - _accessTier;
                if (diff > 0)
                {
                    for (int i = 0; i < diff; i++)
                    {
                        _accessTier++;
                        OnLevelledUp?.Invoke();
                    }
                }

                _accessTier = value;
                OnTierChanged?.Invoke();
            }
        }

        public int AccessTierLevel => AccessTierIndex + 1;

        private void Update()
        {
            if (NetworkServer.active && _valueDirty)
            {
                ServerSendRpc(toAll: true);
                _valueDirty = false;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _thresholdsCount = _levelupThresholds.Length;
            AbsoluteThresholds = new int[_thresholdsCount];

            int accumulated = 0;
            for (int i = 0; i < _thresholdsCount; i++)
            {
                accumulated += _levelupThresholds[i];
                AbsoluteThresholds[i] = accumulated;
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            TotalExp = 0;
        }

        public void ServerGrantExperience(int amount, Scp079HudTranslation reason)
        {
            if (!NetworkServer.active)
                throw new InvalidOperationException("SCP-079 experience cannot be granted by local player!");

            if (amount > 0)
            {
                _expGainQueue.Enqueue(new KeyValuePair<Scp079HudTranslation, int>(reason, amount));
                TotalExp += amount;
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);

            NetworkWriterExtensions.WriteUShort(writer, (ushort)TotalExp);

            while (CollectionExtensions.TryDequeue(_expGainQueue, out var element))
            {
                writer.WriteByte((byte)element.Key);
                writer.WriteByte((byte)Mathf.Min(element.Value, 255));
            }
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            ushort totalExp = NetworkReaderExtensions.ReadUShort(reader);

            if (!Scp079Role.LocalInstanceActive && !base.ScpRole.IsSpectated && !NetworkServer.active)
            {
                TotalExp = totalExp;
                return;
            }

            while (reader.Position < reader.Capacity)
            {
                byte translation = reader.ReadByte();
                int amount = reader.ReadByte();
                GUI.Scp079NotificationManager.AddNotification((Scp079HudTranslation)translation, amount);
            }

            if (!NetworkServer.active)
                TotalExp = totalExp;
        }

        public Scp079TierManager()
        {
            _expGainQueue = new Queue<KeyValuePair<Scp079HudTranslation, int>>();
        }
    }
}
