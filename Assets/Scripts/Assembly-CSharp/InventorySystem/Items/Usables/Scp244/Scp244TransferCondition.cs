using System.Collections.Generic;

using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp244
{
	public class Scp244TransferCondition
	{
		private static readonly Vector3 DoorDetectionThickness = new Vector3(3f, 100f, 3f);

		private const float MinimalDoorGapSqrt = 9f;

		private const float BorderDoorCheck = 1.05f;

		public readonly Bounds BoundsToEncapsulate;

		public readonly DoorVariant[] Doors;

		public readonly float ClosestPoint;

        private Scp244TransferCondition(global::UnityEngine.Bounds b, global::Interactables.Interobjects.DoorUtils.DoorVariant[] dv, global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup scp)
        {
            global::UnityEngine.Vector3 position = scp.transform.position;
            ClosestPoint = global::UnityEngine.Vector3.Distance(position, b.ClosestPoint(position));
            BoundsToEncapsulate = b;
            Doors = dv ?? new global::Interactables.Interobjects.DoorUtils.DoorVariant[0];
        }

        public static global::InventorySystem.Items.Usables.Scp244.Scp244TransferCondition[] GenerateTransferConditions(global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup scp244)
        {
            global::System.Collections.Generic.List<global::InventorySystem.Items.Usables.Scp244.Scp244TransferCondition> list = new global::System.Collections.Generic.List<global::InventorySystem.Items.Usables.Scp244.Scp244TransferCondition>();
            global::UnityEngine.Vector3Int vector3Int = global::MapGeneration.RoomIdUtils.PositionToCoords(scp244.transform.position);
            if (!global::MapGeneration.RoomIdentifier.RoomsByCoordinates.TryGetValue(vector3Int, out var value) || value == null)
            {
                global::UnityEngine.Bounds b = new global::UnityEngine.Bounds(scp244.transform.position, global::UnityEngine.Vector3.one * scp244.MaxDiameter * 2f);
                list.Add(new global::InventorySystem.Items.Usables.Scp244.Scp244TransferCondition(b, null, scp244));
                return list.ToArray();
            }
            global::System.Collections.Generic.List<global::UnityEngine.Bounds> extraBounds = new global::System.Collections.Generic.List<global::UnityEngine.Bounds>();
            global::System.Collections.Generic.HashSet<global::UnityEngine.Vector3> hashSet = new global::System.Collections.Generic.HashSet<global::UnityEngine.Vector3>();
            global::UnityEngine.Vector3 position = scp244.transform.position;
            global::UnityEngine.Bounds[] obj = value.SubBounds ?? new global::UnityEngine.Bounds[0];
            global::UnityEngine.Bounds item = GetBoundsOfEntireRoom(value);
            global::System.Collections.Generic.HashSet<global::Interactables.Interobjects.DoorUtils.DoorVariant> doors = new global::System.Collections.Generic.HashSet<global::Interactables.Interobjects.DoorUtils.DoorVariant>();
            float num = float.MaxValue;
            global::UnityEngine.Bounds[] array = obj;
            foreach (global::UnityEngine.Bounds refBounds in array)
            {
                global::UnityEngine.Bounds relativeBounds = GetRelativeBounds(value.transform, refBounds);
                extraBounds.Add(relativeBounds);
                float sqrMagnitude = (relativeBounds.ClosestPoint(position) - position).sqrMagnitude;
                if (!(sqrMagnitude > num))
                {
                    item = relativeBounds;
                    num = sqrMagnitude;
                }
            }
            if (extraBounds.Count == 0)
            {
                extraBounds.Add(item);
            }
            global::System.Collections.Generic.List<global::UnityEngine.Vector3Int> nearbyRooms = new global::System.Collections.Generic.List<global::UnityEngine.Vector3Int>();
            AddNearbyRooms(ref nearbyRooms, vector3Int, value);
            foreach (global::UnityEngine.Vector3Int item2 in nearbyRooms)
            {
                if (global::MapGeneration.RoomIdentifier.RoomsByCoordinates.TryGetValue(item2, out var value2) && !(value2 == null))
                {
                    HandleRoomBounds(ref extraBounds, value2);
                }
            }
            foreach (global::UnityEngine.Bounds item3 in extraBounds)
            {
                if (hashSet.Add(item3.center))
                {
                    global::UnityEngine.Bounds bounds = new global::UnityEngine.Bounds(item.center, DoorDetectionThickness);
                    bounds.Encapsulate(item3.center);
                    AddDoorsFromRoom(ref doors, bounds, item.center, extraBounds);
                    AddDoorsFromRoom(ref doors, bounds, item3.center, extraBounds);
                    list.Add(new global::InventorySystem.Items.Usables.Scp244.Scp244TransferCondition(item3, global::System.Linq.Enumerable.ToArray(doors), scp244));
                    doors.Clear();
                }
            }
            for (int j = 0; j < list.Count; j++)
            {
                if (!(list[j].ClosestPoint < scp244.MaxDiameter))
                {
                    list.RemoveAt(j);
                    j--;
                }
            }
            return list.ToArray();
        }

        private static global::UnityEngine.Bounds GetBoundsOfEntireRoom(global::MapGeneration.RoomIdentifier rid)
        {
            global::UnityEngine.Bounds result = new global::UnityEngine.Bounds(rid.transform.position, global::UnityEngine.Vector3.zero);
            global::UnityEngine.Vector3 gridScale = global::MapGeneration.RoomIdentifier.GridScale;
            global::UnityEngine.Vector3Int[] occupiedCoords = rid.OccupiedCoords;
            foreach (global::UnityEngine.Vector3Int vector3Int in occupiedCoords)
            {
                result.Encapsulate(new global::UnityEngine.Bounds(global::UnityEngine.Vector3.Scale(vector3Int, gridScale), gridScale));
            }
            return result;
        }


        private static void HandleRoomBounds(ref global::System.Collections.Generic.List<global::UnityEngine.Bounds> extraBounds, global::MapGeneration.RoomIdentifier rid)
        {
            if (rid.SubBounds == null || rid.SubBounds.Length == 0)
            {
                extraBounds.Add(GetBoundsOfEntireRoom(rid));
                return;
            }
            global::UnityEngine.Bounds[] subBounds = rid.SubBounds;
            foreach (global::UnityEngine.Bounds refBounds in subBounds)
            {
                extraBounds.Add(GetRelativeBounds(rid.transform, refBounds));
            }
        }

        private static void AddNearbyRooms(ref global::System.Collections.Generic.List<global::UnityEngine.Vector3Int> nearbyRooms, global::UnityEngine.Vector3Int startCoords, global::MapGeneration.RoomIdentifier startRoom)
        {
            if (!global::Interactables.Interobjects.DoorUtils.DoorVariant.DoorsByRoom.TryGetValue(startRoom, out var value))
            {
                return;
            }
            global::UnityEngine.Vector3 position = startRoom.transform.position;
            foreach (global::Interactables.Interobjects.DoorUtils.DoorVariant item in value)
            {
                global::UnityEngine.Vector3 vector = item.transform.position - position;
                global::UnityEngine.Vector3Int vector3Int = global::MapGeneration.RoomIdUtils.PositionToCoords(position + vector * 1.05f);
                if (vector3Int != startCoords)
                {
                    nearbyRooms.Add(vector3Int);
                }
            }
        }

        private static void AddDoorsFromRoom(ref global::System.Collections.Generic.HashSet<global::Interactables.Interobjects.DoorUtils.DoorVariant> doors, global::UnityEngine.Bounds bounds, global::UnityEngine.Vector3 room, global::System.Collections.Generic.List<global::UnityEngine.Bounds> allBounds)
        {
            if (!global::MapGeneration.RoomIdentifier.RoomsByCoordinates.TryGetValue(global::MapGeneration.RoomIdUtils.PositionToCoords(room), out var value) || !global::Interactables.Interobjects.DoorUtils.DoorVariant.DoorsByRoom.TryGetValue(value, out var value2))
            {
                return;
            }
            foreach (global::Interactables.Interobjects.DoorUtils.DoorVariant item in value2)
            {
                global::UnityEngine.Vector3 position = item.transform.position;
                if (!bounds.Contains(position))
                {
                    continue;
                }
                int num = 0;
                foreach (global::UnityEngine.Bounds allBound in allBounds)
                {
                    if (!(allBound.SqrDistance(position) > 9f) && ++num > 1)
                    {
                        doors.Add(item);
                        break;
                    }
                }
            }
        }

        private static global::UnityEngine.Bounds GetRelativeBounds(global::UnityEngine.Transform relativeTo, global::UnityEngine.Bounds refBounds)
        {
            global::UnityEngine.Vector3 center = relativeTo.TransformPoint(refBounds.center);
            global::UnityEngine.Vector3 size = relativeTo.rotation * refBounds.size;
            size.x = global::UnityEngine.Mathf.Abs(size.x);
            size.z = global::UnityEngine.Mathf.Abs(size.z);
            return new global::UnityEngine.Bounds(center, size);
        }
	}
}
