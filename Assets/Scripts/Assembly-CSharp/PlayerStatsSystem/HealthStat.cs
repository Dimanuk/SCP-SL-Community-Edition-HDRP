using Mirror;
using PlayerRoles;
using UnityEngine;

namespace PlayerStatsSystem
{
    public class HealthStat : SyncedStatBase
    {
        public override SyncMode Mode => SyncMode.PrivateAndSpectators;

        public override float MinValue => 0f;

        public override float MaxValue
        {
            get
            {
                if (!(base.Hub.roleManager.CurrentRole is IHealthbarRole healthbarRole))
                {
                    return 0f;
                }
                return healthbarRole.MaxHealth;
            }
        }

        public bool FullyHealed => CurValue >= MaxValue;

        public override float ReadValue(NetworkReader reader)
        {
            return (int)reader.ReadUShort();
        }

        public override void WriteValue(NetworkWriter writer)
        {
            int num = Mathf.Clamp(Mathf.CeilToInt(CurValue), 0, 65535);
            writer.WriteUShort((ushort)num);
        }

        public override bool CheckDirty(float prevValue, float newValue)
        {
            return Mathf.CeilToInt(prevValue) != Mathf.CeilToInt(newValue);
        }

        internal override void ClassChanged()
        {
            base.ClassChanged();
            if (NetworkServer.active)
            {
                CurValue = MaxValue;
            }
        }

        public void ServerHeal(float healAmount)
        {
            CurValue = Mathf.Min(CurValue + Mathf.Abs(healAmount), MaxValue);
        }
    }
}
