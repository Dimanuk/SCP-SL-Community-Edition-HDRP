using System;
using System.Collections.Generic;
using Mirror;
using PlayerRoles;
using UnityEngine;
using Utils.Networking;

namespace PlayerStatsSystem
{
    public abstract class SyncedStatBase : StatBase
    {
        public delegate void StatChange(float oldValue, float newValue);

        public enum SyncMode
        {
            Private = 0,
            PrivateAndSpectators = 1,
            Public = 2
        }

        private float _lastValue;

        private float _lastSent;

        private bool _valueDirty;

        private byte? _syncId;

        private static readonly Dictionary<uint, Dictionary<byte, SyncedStatBase>> AllSyncedStats = new Dictionary<uint, Dictionary<byte, SyncedStatBase>>();

        public override float CurValue
        {
            get
            {
                return _lastValue;
            }
            set
            {
                float lastValue = _lastValue;
                _lastValue = value;
                if (CheckDirty(_lastSent, value))
                {
                    _valueDirty = true;
                }
                if (lastValue != value)
                {
                    if (value != MaxValue)
                    {
                        this.OnStatChange?.Invoke(lastValue, value);
                    }
                    OnValueChanged(lastValue, value);
                }
            }
        }

        public byte SyncId
        {
            get
            {
                if (_syncId.HasValue)
                {
                    return _syncId.Value;
                }
                StatBase[] statModules = base.Hub.playerStats.StatModules;
                byte b = 0;
                for (int i = 0; i < statModules.Length; i++)
                {
                    if (statModules[i] is SyncedStatBase syncedStatBase)
                    {
                        syncedStatBase._syncId = b++;
                    }
                }
                return _syncId.Value;
            }
        }

        public abstract SyncMode Mode { get; }

        public event StatChange OnStatChange;

        public abstract float ReadValue(NetworkReader reader);

        public abstract void WriteValue(NetworkWriter writer);

        public abstract bool CheckDirty(float prevValue, float newValue);

        protected virtual void OnValueChanged(float prevValue, float newValue)
        {
        }

        public static SyncedStatBase GetStatOfUser(uint netId, byte syncId)
        {
            if (!AllSyncedStats.TryGetValue(netId, out var value))
            {
                if (!ReferenceHub.TryGetHubNetID(netId, out var hub))
                {
                    throw new InvalidOperationException($"Cannot generate stats for non-existing user of NetId={netId}");
                }
                value = new Dictionary<byte, SyncedStatBase>();
                StatBase[] statModules = hub.playerStats.StatModules;
                for (int i = 0; i < statModules.Length; i++)
                {
                    if (statModules[i] is SyncedStatBase syncedStatBase)
                    {
                        value.Add(syncedStatBase.SyncId, syncedStatBase);
                    }
                }
                AllSyncedStats[netId] = value;
            }
            if (!value.TryGetValue(syncId, out var value2))
            {
                throw new InvalidOperationException($"Stat of SyncId={syncId} does not exist.");
            }
            return value2;
        }

        internal override void Update()
        {
            base.Update();
            if (NetworkServer.active && _valueDirty)
            {
                new SyncedStatMessages.StatMessage
                {
                    Stat = this,
                    SyncedValue = CurValue
                }.SendToHubsConditionally(CanReceive);
                _lastSent = CurValue;
                _valueDirty = false;
            }
        }

        internal override void ClassChanged()
        {
            base.ClassChanged();
            if (NetworkServer.active)
            {
                _valueDirty = true;
            }
        }

        private bool CanReceive(ReferenceHub hub)
        {
            if (hub.isLocalPlayer)
            {
                return false;
            }
            switch (Mode)
            {
                case SyncMode.Private:
                    return hub == base.Hub;
                case SyncMode.PrivateAndSpectators:
                    if (hub.IsAlive())
                    {
                        return hub == base.Hub;
                    }
                    return true;
                case SyncMode.Public:
                    return true;
                default:
                    return false;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitOnLoad()
        {
            ReferenceHub.OnPlayerRemoved = (Action<ReferenceHub>)Delegate.Combine(ReferenceHub.OnPlayerRemoved, (Action<ReferenceHub>)delegate (ReferenceHub x)
            {
                AllSyncedStats.Remove(x.netId);
            });
        }
    }
}
