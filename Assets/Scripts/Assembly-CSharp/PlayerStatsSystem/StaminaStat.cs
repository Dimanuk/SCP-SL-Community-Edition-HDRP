using Mirror;
using PlayerRoles;
using UnityEngine;

namespace PlayerStatsSystem
{
    public class StaminaStat : SyncedStatBase
    {
        private const SyncMode DefaultSyncMode = SyncMode.PrivateAndSpectators;

        private SyncMode _syncMode = SyncMode.PrivateAndSpectators;

        private RoleTypeId _overrideRole = RoleTypeId.None;

        public override SyncMode Mode => _syncMode;

        public override float MinValue => 0f;

        public override float MaxValue => 1f;

        public void ModifyAmount(float f)
        {
            CurValue = Mathf.Clamp01(CurValue + f);
        }

        public void ChangeSyncMode(SyncMode newMode)
        {
            _syncMode = newMode;
            _overrideRole = base.Hub.GetRoleId();
        }

        private byte ToByte(float val)
        {
            return (byte)Mathf.RoundToInt(Mathf.Clamp01(val) * 255f);
        }

        public override float ReadValue(NetworkReader reader)
        {
            return (float)(int)reader.ReadByte() / 255f;
        }

        public override void WriteValue(NetworkWriter writer)
        {
            writer.WriteByte(ToByte(CurValue));
        }

        public override bool CheckDirty(float prevValue, float newValue)
        {
            return ToByte(prevValue) != ToByte(newValue);
        }

        internal override void ClassChanged()
        {
            CurValue = 1f;
            if (_overrideRole != RoleTypeId.None && base.Hub.GetRoleId() != _overrideRole)
            {
                _syncMode = SyncMode.PrivateAndSpectators;
                _overrideRole = RoleTypeId.None;
            }
            base.ClassChanged();
        }
    }
}
