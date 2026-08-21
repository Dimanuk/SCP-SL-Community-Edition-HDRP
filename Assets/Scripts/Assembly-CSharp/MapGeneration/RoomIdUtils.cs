using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    public static class RoomIdUtils
    {
        public static Vector3Int PositionToCoords(Vector3 position)
        {
            Vector3Int zero = Vector3Int.zero;
            for (int i = 0; i < 3; i++)
            {
                zero[i] = Mathf.RoundToInt(position[i] / RoomIdentifier.GridScale[i]);
            }
            return zero;
        }

        public static Vector3 CoordsToCenterPos(Vector3Int position)
        {
            Vector3 zero = Vector3.zero;
            for (int i = 0; i < 3; i++)
            {
                zero[i] = (float)position[i] * RoomIdentifier.GridScale[i];
            }
            return zero;
        }

        public static bool TryFindRoom(RoomName name, FacilityZone zone, RoomShape shape, out RoomIdentifier foundRoom)
        {
            foreach (RoomIdentifier allRoomIdentifier in RoomIdentifier.AllRoomIdentifiers)
            {
                if ((name == RoomName.Unnamed || name == allRoomIdentifier.Name) && (zone == FacilityZone.None || zone == allRoomIdentifier.Zone) && (shape == RoomShape.Undefined || shape == allRoomIdentifier.Shape))
                {
                    foundRoom = allRoomIdentifier;
                    return true;
                }
            }
            foundRoom = null;
            return false;
        }

        public static HashSet<RoomIdentifier> FindRooms(RoomName name, FacilityZone zone, RoomShape shape)
        {
            HashSet<RoomIdentifier> hashSet = new HashSet<RoomIdentifier>();
            foreach (RoomIdentifier allRoomIdentifier in RoomIdentifier.AllRoomIdentifiers)
            {
                if ((name == RoomName.Unnamed || name == allRoomIdentifier.Name) && (zone == FacilityZone.None || zone == allRoomIdentifier.Zone) && (shape == RoomShape.Undefined || shape == allRoomIdentifier.Shape))
                {
                    hashSet.Add(allRoomIdentifier);
                }
            }
            return hashSet;
        }

        public static bool IsTheSameRoom(Vector3 pos1, Vector3 pos2)
        {
            return RoomAtPosition(pos1) == RoomAtPosition(pos2);
        }

        public static bool IsWithinRoomBoundaries(RoomIdentifier room, Vector3 pos, float extension = 0f, bool accurateMode = false)
        {
            if (accurateMode)
            {
                if (RoomAtPositionRaycasts(pos) == room)
                {
                    return true;
                }
                if (extension == 0f)
                {
                    return false;
                }
                pos += (room.transform.position - pos).normalized * extension;
                if (RoomAtPositionRaycasts(pos) == room)
                {
                    return true;
                }
            }
            if (extension != 0f)
            {
                pos += (room.transform.position - pos).normalized * extension;
            }
            return RoomAtPosition(pos) == room;
        }

        public static RoomIdentifier RoomAtPosition(Vector3 position)
        {
            if (RoomIdentifier.RoomsByCoordinates.TryGetValue(PositionToCoords(position), out var value))
            {
                return value;
            }
            return null;
        }

        public static RoomIdentifier RoomAtPositionRaycasts(Vector3 position, bool prioritizeRaycast = true)
        {
            if (!prioritizeRaycast && RoomIdentifier.RoomsByCoordinates.TryGetValue(PositionToCoords(position), out var value) && value != null)
            {
                return value;
            }
            if (TryCastRayToFindRoom(new Ray(position, Vector3.up), 8f, out value) || TryCastRayToFindRoom(new Ray(position, Vector3.down), 8f, out value))
            {
                return value;
            }
            if (prioritizeRaycast)
            {
                return RoomAtPosition(position);
            }
            return null;
        }

        private static bool TryCastRayToFindRoom(Ray ray, float distance, out RoomIdentifier room)
        {
            room = null;
            try
            {
                if (Physics.Raycast(ray, out var hitInfo, distance, 1))
                {
                    room = hitInfo.collider.GetComponentInParent<RoomIdentifier>();
                }
                return room != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
