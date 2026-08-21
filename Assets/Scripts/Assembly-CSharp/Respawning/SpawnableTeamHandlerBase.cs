using System.Collections.Generic;
using PlayerRoles;

namespace Respawning
{
	public abstract class SpawnableTeamHandlerBase
	{
		public abstract int MaxWaveSize { get; }

		public abstract int StartTokens { get; }

		public abstract float EffectTime { get; }

		public abstract void GenerateQueue(Queue<RoleTypeId> queueToFill, int playersToSpawn);
	}
}
