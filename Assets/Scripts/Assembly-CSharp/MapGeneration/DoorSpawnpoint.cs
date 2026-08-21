using System.Collections.Generic;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using UnityEngine;

namespace MapGeneration
{
    public class DoorSpawnpoint : MonoBehaviour
    {
        private const float MinimumDistance = 2.5f;

        private static readonly Queue<DoorSpawnpoint> AllInstances = new Queue<DoorSpawnpoint>();

        public DoorVariant TargetPrefab;

        public string DesiredNametag;

        private void Start()
        {
            AllInstances.Enqueue(this);
        }

        public static void SetupAllDoors()
        {
            HashSet<DoorSpawnpoint> hashSet = new HashSet<DoorSpawnpoint>();
            while (AllInstances.Count > 0)
            {
                DoorSpawnpoint doorSpawnpoint = AllInstances.Dequeue();
                if (doorSpawnpoint == null || !doorSpawnpoint.gameObject.activeInHierarchy)
                {
                    continue;
                }
                Vector3 position = doorSpawnpoint.transform.position;
                bool flag = false;
                foreach (DoorSpawnpoint item in hashSet)
                {
                    Vector3 position2 = item.transform.position;
                    if (!(Mathf.Abs(position2.y - position.y) > 2.5f) && !(Mathf.Abs(position2.x - position.x) > 2.5f) && !(Mathf.Abs(position2.z - position.z) > 2.5f))
                    {
                        flag = true;
                        item.transform.position = Vector3.Lerp(position2, position, 0.5f);
                        if (!string.IsNullOrEmpty(doorSpawnpoint.DesiredNametag))
                        {
                            item.DesiredNametag = doorSpawnpoint.DesiredNametag;
                        }
                        break;
                    }
                }
                if (!flag)
                {
                    hashSet.Add(doorSpawnpoint);
                }
            }
            foreach (DoorSpawnpoint item2 in hashSet)
            {
                DoorVariant doorVariant = Object.Instantiate(item2.TargetPrefab, item2.transform.position, item2.transform.rotation);
                if (!string.IsNullOrEmpty(item2.DesiredNametag))
                {
                    doorVariant.gameObject.AddComponent<DoorNametagExtension>().UpdateName(item2.DesiredNametag);
                }
                NetworkServer.Spawn(doorVariant.gameObject);
            }
        }
    }
}