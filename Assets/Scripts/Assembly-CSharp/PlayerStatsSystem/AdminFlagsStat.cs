using Mirror;
using UnityEngine;

namespace PlayerStatsSystem
{
    public class AdminFlagsStat : SyncedStatBase
    {
        public override SyncMode Mode => SyncMode.Public;

        public override float MinValue => 0f;

        public override float MaxValue => float.MaxValue;

        public AdminFlags Flags
        {
            get
            {
                return (AdminFlags)Mathf.RoundToInt(CurValue);
            }
            set
            {
                CurValue = (float)value;
            }
        }

        public bool HasFlag(AdminFlags flag)
        {
            return (flag & Flags) == flag;
        }

        public void InvertFlag(AdminFlags flag)
        {
            AdminFlags flags = Flags;
            Flags = (((flag & flags) != flag) ? (flags | flag) : (flags & ~flag));
        }

        public void SetFlag(AdminFlags flag, bool status)
        {
            Flags = (status ? (Flags | flag) : (Flags & ~flag));
        }

        public override float ReadValue(NetworkReader reader)
        {
            return (int)reader.ReadByte();
        }

        public override void WriteValue(NetworkWriter writer)
        {
            writer.WriteByte((byte)Flags);
        }

        public override bool CheckDirty(float prevValue, float newValue)
        {
            return (int)prevValue != (int)newValue;
        }
    }
}
