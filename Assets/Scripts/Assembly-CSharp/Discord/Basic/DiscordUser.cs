using System;
using System.Runtime.InteropServices;

namespace Discord.Basic
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]  
	public struct DiscordUser : IEquatable<DiscordUser>
	{
		    [MarshalAs(UnmanagedType.LPStr)]
        public string userId;
        
        [MarshalAs(UnmanagedType.LPStr)]
        public string username;
        
        [MarshalAs(UnmanagedType.LPStr)]
        public string discriminator;
        
        [MarshalAs(UnmanagedType.LPStr)]
        public string avatar;
		public bool Equals(DiscordUser other)
		{
			return string.Equals(userId, other.userId)
				&& string.Equals(username, other.username)
				&& string.Equals(discriminator, other.discriminator)
				&& string.Equals(avatar, other.avatar);
		}

		public override bool Equals(object obj)
		{
			return obj is DiscordUser other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = (userId != null ? userId.GetHashCode() : 0);
				hash = (hash * 397) ^ (username != null ? username.GetHashCode() : 0);
				hash = (hash * 397) ^ (discriminator != null ? discriminator.GetHashCode() : 0);
				hash = (hash * 397) ^ (avatar != null ? avatar.GetHashCode() : 0);
				return hash;
			}
		}

		public static bool operator ==(DiscordUser left, DiscordUser right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(DiscordUser left, DiscordUser right)
		{
			return !left.Equals(right);
		}
	}
}
