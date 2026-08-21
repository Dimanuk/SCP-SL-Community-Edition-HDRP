using Mirror;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Subroutines
{

    public class AbilityCooldown
    {

        public double InitialTime { get; set; }

        public double NextUse { get; set; }

        public virtual bool IsReady => NetworkTime.time >= NextUse;

        public float Remaining
        {
            get => Mathf.Max(0f, (float)(NextUse - NetworkTime.time));
            set => NextUse = NetworkTime.time + (double)value;
        }
        public float Readiness => Mathf.Clamp01((float)((NetworkTime.time - InitialTime) / (NextUse - InitialTime)));

        public void WriteCooldown(NetworkWriter writer)
        {
            NetworkWriterExtensions.WriteDouble(writer, NextUse);
        }

        public void ReadCooldown(NetworkReader reader)
        {
            InitialTime = NetworkTime.time;
            NextUse = NetworkReaderExtensions.ReadDouble(reader);
        }
        public void Clear()
        {
            InitialTime = 0.0;
            NextUse = 1.0;
        }

        public virtual void Trigger(float cooldown)
        {
            InitialTime = NetworkTime.time;
            NextUse = InitialTime + (double)cooldown;
        }
    }
}