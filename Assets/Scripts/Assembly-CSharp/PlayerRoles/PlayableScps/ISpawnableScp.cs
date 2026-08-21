using System.Collections.Generic;

namespace PlayerRoles.PlayableScps
{
	public interface ISpawnableScp
	{
		float GetSpawnChance(List<RoleTypeId> alreadySpawned);
	}
}
