using Mirror;

namespace Respawning.NamingRules
{
	public struct UnitNameMessage : NetworkMessage
	{
		public string UnitName;

		public SpawnableTeamType Team;

		public UnitNamingRule NamingRule;

		public NetworkReader Data;
	}
}
