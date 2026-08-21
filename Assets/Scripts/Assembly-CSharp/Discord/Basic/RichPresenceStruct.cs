using System;
using System.Runtime.InteropServices;

namespace Discord.Basic
{
    [Serializable]
	[StructLayout(LayoutKind.Sequential)]
    public struct RichPresenceStruct : IEquatable<RichPresenceStruct>
    {
        public IntPtr state;
        public IntPtr details;
        public long startTimestamp;
        public long endTimestamp;
        public IntPtr largeImageKey;
        public IntPtr largeImageText;
        public IntPtr smallImageKey;
        public IntPtr smallImageText;
        public IntPtr partyId;
        public int partySize;
        public int partyMax;
        public IntPtr matchSecret;
        public IntPtr joinSecret;
        public IntPtr spectateSecret;
        public bool instance;

        public bool Equals(RichPresenceStruct other)
        {
            return state == other.state
                && details == other.details
                && startTimestamp == other.startTimestamp
                && endTimestamp == other.endTimestamp
                && largeImageKey == other.largeImageKey
                && largeImageText == other.largeImageText
                && smallImageKey == other.smallImageKey
                && smallImageText == other.smallImageText
                && partyId == other.partyId
                && partySize == other.partySize
                && partyMax == other.partyMax
                && matchSecret == other.matchSecret
                && joinSecret == other.joinSecret
                && spectateSecret == other.spectateSecret
                && instance == other.instance;
        }

        public override bool Equals(object obj)
        {
            return obj is RichPresenceStruct other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = state.GetHashCode();
                hash = (hash * 397) ^ details.GetHashCode();
                hash = (hash * 397) ^ startTimestamp.GetHashCode();
                hash = (hash * 397) ^ endTimestamp.GetHashCode();
                hash = (hash * 397) ^ largeImageKey.GetHashCode();
                hash = (hash * 397) ^ largeImageText.GetHashCode();
                hash = (hash * 397) ^ smallImageKey.GetHashCode();
                hash = (hash * 397) ^ smallImageText.GetHashCode();
                hash = (hash * 397) ^ partyId.GetHashCode();
                hash = (hash * 397) ^ partySize;
                hash = (hash * 397) ^ partyMax;
                hash = (hash * 397) ^ matchSecret.GetHashCode();
                hash = (hash * 397) ^ joinSecret.GetHashCode();
                hash = (hash * 397) ^ spectateSecret.GetHashCode();
                hash = (hash * 397) ^ instance.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(RichPresenceStruct left, RichPresenceStruct right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RichPresenceStruct left, RichPresenceStruct right)
        {
            return !left.Equals(right);
        }
    }
}