using UnityEngine;

namespace PlayerRoles
{
	public interface IAmbientLightRole
	{
		Color AmbientLight { get; }

		bool InsufficientLight { get; }
	}
}
