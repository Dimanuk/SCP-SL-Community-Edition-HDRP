using System.Collections.Generic;

namespace MapGeneration.Distributors
{
    public class StructureSpawnpoint : DistributorSpawnpointBase
    {
        public static readonly HashSet<StructureSpawnpoint> AvailableInstances = new HashSet<StructureSpawnpoint>();

        public StructureType[] CompatibleStructures;

        public string TriggerDoorName;

        private void Start()
        {
            AvailableInstances.Add(this);
        }

        private void OnDestroy()
        {
            AvailableInstances.Remove(this);
        }
    }
}
