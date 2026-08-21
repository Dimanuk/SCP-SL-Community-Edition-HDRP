using System.Collections.Generic;

using MapGeneration;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp244
{
	public static class Scp244Spawner
	{
        private static readonly global::System.Collections.Generic.List<global::MapGeneration.RoomIdentifier> CompatibleRooms = new global::System.Collections.Generic.List<global::MapGeneration.RoomIdentifier>();

        private const int Amount = 1;

		private const float SpawnChance = 0.35f;

        private static readonly global::System.Collections.Generic.Dictionary<global::MapGeneration.RoomName, global::UnityEngine.Vector3> NameToPos = new global::System.Collections.Generic.Dictionary<global::MapGeneration.RoomName, global::UnityEngine.Vector3>
        {
            [global::MapGeneration.RoomName.Unnamed] = global::UnityEngine.Vector3.up,
            [global::MapGeneration.RoomName.HczWarhead] = new global::UnityEngine.Vector3(6.8f, 401f, 11.6f),
            [global::MapGeneration.RoomName.HczMicroHID] = new global::UnityEngine.Vector3(-7.4f, 1f, -6.8f),
            [global::MapGeneration.RoomName.HczArmory] = new global::UnityEngine.Vector3(-3.8f, 1f, 0.8f),
            [global::MapGeneration.RoomName.HczTestroom] = new global::UnityEngine.Vector3(0f, 0.26f, 7f)
        };

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::MapGeneration.SeedSynchronizer.OnMapGenerated += SpawnAllInstances;
        }

        private static void SpawnAllInstances()
        {
            if (!global::Mirror.NetworkServer.active)
            {
                return;
            }
            CompatibleRooms.Clear();
            if (!global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(ItemType.SCP244b, out var value))
            {
                return;
            }
            foreach (global::MapGeneration.RoomIdentifier allRoomIdentifier in global::MapGeneration.RoomIdentifier.AllRoomIdentifiers)
            {
                if (allRoomIdentifier != null && allRoomIdentifier.Zone == global::MapGeneration.FacilityZone.HeavyContainment && NameToPos.ContainsKey(allRoomIdentifier.Name))
                {
                    CompatibleRooms.Add(allRoomIdentifier);
                }
            }
            for (int i = 0; i < 1; i++)
            {
                SpawnScp244(value);
            }
        }

        private static void SpawnScp244(global::InventorySystem.Items.ItemBase ib)
        {
            if (CompatibleRooms.Count != 0 && !(global::UnityEngine.Random.value > 0.35f))
            {
                int index = global::UnityEngine.Random.Range(0, CompatibleRooms.Count);
                global::UnityEngine.Vector3 position = CompatibleRooms[index].transform.TransformPoint(NameToPos[CompatibleRooms[index].Name]);
                global::InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = global::UnityEngine.Object.Instantiate(ib.PickupDropModel, position, global::UnityEngine.Quaternion.identity);
                itemPickupBase.Info = new global::InventorySystem.Items.Pickups.PickupSyncInfo
                {
                    ItemId = ib.ItemTypeId,
                    Weight = ib.Weight,
                    Serial = global::InventorySystem.Items.ItemSerialGenerator.GenerateNext()
                };
                if (itemPickupBase is global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup scp244DeployablePickup)
                {
                    scp244DeployablePickup.State = global::InventorySystem.Items.Usables.Scp244.Scp244State.Active;
                }
                itemPickupBase.RefreshPositionAndRotation();
                global::Mirror.NetworkServer.Spawn(itemPickupBase.gameObject);
                itemPickupBase.InfoReceived(default(global::InventorySystem.Items.Pickups.PickupSyncInfo), itemPickupBase.Info);
                CompatibleRooms.RemoveAt(index);
            }
        }
	}
}
