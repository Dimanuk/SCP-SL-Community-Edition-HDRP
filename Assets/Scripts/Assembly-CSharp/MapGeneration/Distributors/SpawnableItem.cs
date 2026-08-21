using System;

namespace MapGeneration.Distributors
{
	[Serializable]
	public struct SpawnableItem
	{
		public float MinimalAmount;

		public float MaxAmount;

		public bool MultiplyBySpawnpointsNumber;

		public ItemType[] PossibleSpawns;

		public RoomName[] RoomNames;
	}
}
