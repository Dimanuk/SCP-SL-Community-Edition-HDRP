using System;
using System.Runtime.InteropServices;

namespace Discord.Basic
{
	 [StructLayout(LayoutKind.Sequential)]
	public struct EventHandlers : IEquatable<EventHandlers>
	{
		public CallbackController.OnReadyInfo readyCallback;
		public CallbackController.OnDisconnectedInfo disconnectedCallback;
		public CallbackController.OnErrorInfo errorCallback;
		public CallbackController.OnJoinInfo joinCallback;
		public CallbackController.OnSpectateInfo spectateCallback;
		public CallbackController.OnRequestInfo requestCallback;

		public bool Equals(EventHandlers other)
		{
			return Equals(readyCallback, other.readyCallback)
				&& Equals(disconnectedCallback, other.disconnectedCallback)
				&& Equals(errorCallback, other.errorCallback)
				&& Equals(joinCallback, other.joinCallback)
				&& Equals(spectateCallback, other.spectateCallback)
				&& Equals(requestCallback, other.requestCallback);
		}

		public override bool Equals(object obj)
		{
			return obj is EventHandlers other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = (readyCallback != null ? readyCallback.GetHashCode() : 0);
				hash = (hash * 397) ^ (disconnectedCallback != null ? disconnectedCallback.GetHashCode() : 0);
				hash = (hash * 397) ^ (errorCallback != null ? errorCallback.GetHashCode() : 0);
				hash = (hash * 397) ^ (joinCallback != null ? joinCallback.GetHashCode() : 0);
				hash = (hash * 397) ^ (spectateCallback != null ? spectateCallback.GetHashCode() : 0);
				hash = (hash * 397) ^ (requestCallback != null ? requestCallback.GetHashCode() : 0);
				return hash;
			}
		}

		public static bool operator ==(EventHandlers left, EventHandlers right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(EventHandlers left, EventHandlers right)
		{
			return !left.Equals(right);
		}
	}
}