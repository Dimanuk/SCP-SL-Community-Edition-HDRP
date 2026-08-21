using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items.Pickups;
using Mirror;
using NorthwoodLib.Pools;
using UnityEngine;

namespace MapGeneration.Distributors
{
    public class ItemDistributor : SpawnablesDistributorBase
    {
        protected override void PlaceSpawnables()
        {
            ItemSpawnpoint.RandomInstances.RemoveWhere(x => x == null);
            ItemSpawnpoint.AutospawnInstances.RemoveWhere(x => x == null);

            foreach (var item in Settings.SpawnableItems)
                PlaceItem(item);

            foreach (var autospawnInstance in ItemSpawnpoint.AutospawnInstances)
            {
                Transform t = autospawnInstance.Occupy();
                if (t != null)
                    CreatePickup(autospawnInstance.AutospawnItem, t, autospawnInstance.TriggerDoorName);
            }

            ItemSpawnpoint.RandomInstances.Clear();
            ItemSpawnpoint.AutospawnInstances.Clear();
        }

        private void PlaceItem(SpawnableItem item)
        {
            float amount = Random.Range(item.MinimalAmount, item.MaxAmount);

            var list = ListPool<ItemSpawnpoint>.Shared.Rent();

            foreach (var spawnPoint in ItemSpawnpoint.RandomInstances)
            {
                if (item.RoomNames.Contains(spawnPoint.RoomName) && spawnPoint.CanSpawn(item.PossibleSpawns))
                    list.Add(spawnPoint);
            }

            if (item.MultiplyBySpawnpointsNumber)
                amount *= list.Count;

            for (int i = 0; i < amount; i++)
            {
                if (list.Count == 0)
                    break;

                int index = Random.Range(0, list.Count);
                ItemSpawnpoint spawnPoint = list[index];
                var validItems = System.Array.FindAll(item.PossibleSpawns, x => spawnPoint.CanSpawn(x));

                if (validItems.Length == 0)
                {
                    list.RemoveAt(index);
                    continue;
                }

                ItemType itemType = validItems[Random.Range(0, validItems.Length)];
                Transform transform = spawnPoint.Occupy();

                if (transform == null)
                {
                    list.RemoveAt(index);
                    continue;
                }

                CreatePickup(itemType, transform, spawnPoint.TriggerDoorName);

                if (!spawnPoint.CanSpawn(item.PossibleSpawns))
                    list.RemoveAt(index);
            }

            ListPool<ItemSpawnpoint>.Shared.Return(list);
        }

        private void CreatePickup(ItemType id, Transform parentTransform, string triggerDoorName)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(id, out var itemBase) || itemBase == null)
                return;

            if (itemBase.PickupDropModel == null)
                return;

            if (!itemBase.PickupDropModel.GetComponent<Rigidbody>())
                return;

            ItemPickupBase pickup = Object.Instantiate(
                itemBase.PickupDropModel,
                parentTransform.position,
                parentTransform.rotation);

            pickup.Info.ItemId = id;
            pickup.Info.Weight = itemBase.Weight;

            (pickup as IPickupDistributorTrigger)?.OnDistributed();

            if (string.IsNullOrEmpty(triggerDoorName) ||
                !DoorNametagExtension.NamedDoors.TryGetValue(triggerDoorName, out var doorExtension))
            {
                SpawnPickup(pickup);
            }
            else
            {
                RegisterUnspawnedObject(doorExtension.TargetDoor, pickup.gameObject);
            }
        }

        public static void SpawnPickup(ItemPickupBase ipb)
        {
            if (ipb == null || !NetworkServer.active)
                return;

            PickupSyncInfo syncInfo = new(
                ipb.Info.ItemId,
                ipb.transform.position,
                ipb.transform.rotation,
                ipb.Info.Weight,
                0)
            {
                Locked = ipb.Info.Locked
            };

            InitiallySpawnedItems.Singleton.AddInitial(syncInfo.Serial);
            ipb.Info = syncInfo;
            ipb.InfoReceived(default, syncInfo);

            NetworkServer.Spawn(ipb.gameObject);
        }

        protected override void SpawnObject(GameObject objectToSpawn)
        {
            if (objectToSpawn != null && objectToSpawn.TryGetComponent(out ItemPickupBase component))
                SpawnPickup(component);
        }
    }
}