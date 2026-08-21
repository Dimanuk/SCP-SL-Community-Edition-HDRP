using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    public class RoomIdentifier : MonoBehaviour
    {
        public RoomShape Shape;

        public RoomName Name;

        public FacilityZone Zone;

        public Vector3Int[] AdditionalZones;

        public Bounds[] SubBounds;

        public Sprite Icon;

        public static readonly HashSet<RoomIdentifier> AllRoomIdentifiers = new HashSet<RoomIdentifier>();

        public static readonly Dictionary<Vector3Int, RoomIdentifier> RoomsByCoordinates = new Dictionary<Vector3Int, RoomIdentifier>();

        public static readonly Vector3 GridScale = new Vector3(15f, 100f, 15f);

        public Vector3Int[] OccupiedCoords { get; private set; }

        public static event Action<RoomIdentifier> OnAdded;

        public static event Action<RoomIdentifier> OnRemoved;

        private void Awake()
        {
            AllRoomIdentifiers.Add(this);
            RoomIdentifier.OnAdded?.Invoke(this);
        }

        private void OnDestroy()
        {
            AllRoomIdentifiers.Remove(this);
            RoomIdentifier.OnRemoved?.Invoke(this);
            if (OccupiedCoords != null)
            {
                Vector3Int[] occupiedCoords = OccupiedCoords;
                foreach (Vector3Int key in occupiedCoords)
                {
                    RoomsByCoordinates.Remove(key);
                }
            }
        }

        public bool TryAssignId()
        {
            if (!base.gameObject.activeInHierarchy)
            {
                return false;
            }
            Vector3Int vector3Int = RoomIdUtils.PositionToCoords(base.transform.position);
            RoomsByCoordinates[vector3Int] = this;
            OccupiedCoords = new Vector3Int[AdditionalZones.Length + 1];
            OccupiedCoords[0] = vector3Int;
            int num = 1;
            Vector3Int[] additionalZones = AdditionalZones;
            for (int i = 0; i < additionalZones.Length; i++)
            {
                Vector3 direction = Vector3.Scale(additionalZones[i], GridScale);
                vector3Int = RoomIdUtils.PositionToCoords(base.transform.position + base.transform.TransformDirection(direction));
                RoomsByCoordinates[vector3Int] = this;
                OccupiedCoords[num] = vector3Int;
                num++;
            }
            return true;
        }

        public bool TryGetMainCoords(out Vector3Int coords)
        {
            if (OccupiedCoords == null || OccupiedCoords.Length == 0)
            {
                coords = Vector3Int.zero;
                return false;
            }
            coords = OccupiedCoords[0];
            return true;
        }
    }
}