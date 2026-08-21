using System.Collections.Generic;
using Mirror;
using NorthwoodLib.Pools;
using UnityEngine;

namespace MapGeneration.Distributors
{
    [CreateAssetMenu(fileName = "New Spawner Settings Preset", menuName = "ScriptableObject/Map Generation/Spawnable Elements Settings")]
    public class SpawnablesDistributorSettings : ScriptableObject
    {
        [Range(0.05f, 5f)]
        public float SpawnerDelay;

        [Range(0.05f, 5f)]
        public float UnfreezeDelay;

        public SpawnableItem[] SpawnableItems;

        public SpawnableStructure[] SpawnableStructures;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientStarted += RegisterSpawnables;
        }

        private static void RegisterSpawnables()
        {
            Debug.Log("Registering spawnable prefabs...");
            SpawnablesDistributorSettings[] array = Resources.LoadAll<SpawnablesDistributorSettings>(string.Empty);
            HashSet<SpawnableStructure> hashSet = HashSetPool<SpawnableStructure>.Shared.Rent();
            SpawnablesDistributorSettings[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                SpawnableStructure[] spawnableStructures = array2[i].SpawnableStructures;
                foreach (SpawnableStructure spawnableStructure in spawnableStructures)
                {
                    if (hashSet.Add(spawnableStructure))
                    {
                        NetworkClient.RegisterPrefab(spawnableStructure.gameObject);
                    }
                }
            }
            HashSetPool<SpawnableStructure>.Shared.Return(hashSet);
        }
    }
}
