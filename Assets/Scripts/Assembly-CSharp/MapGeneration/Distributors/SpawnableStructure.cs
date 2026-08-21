using Mirror;
using UnityEngine;

namespace MapGeneration.Distributors
{
    [RequireComponent(typeof(StructurePositionSync))]
    public class SpawnableStructure : NetworkBehaviour
    {
        public StructureType StructureType;

        public int MinAmount;

        public int MaxAmount;
    }
}
