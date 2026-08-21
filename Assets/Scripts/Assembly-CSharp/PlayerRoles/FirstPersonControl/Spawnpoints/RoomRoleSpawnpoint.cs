using System;
using System.Collections.Generic;
using MapGeneration;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl.Spawnpoints
{
    [Serializable]
    public class RoomRoleSpawnpoint : ISpawnpointHandler
    {
        private readonly List<BoundsRoleSpawnpoint> _spawnpoints;

        [SerializeField]
        private RoomName _fName;

        [SerializeField]
        private FacilityZone _fZone;

        [SerializeField]
        private RoomShape _fShape;

        [SerializeField]
        private Vector3 _localPoint;

        [SerializeField]
        private float _lookAngle;

        [SerializeField]
        private float _angleVar;

        [SerializeField]
        private float _width;

        [SerializeField]
        private float _length;

        [SerializeField]
        private int _wNum;

        [SerializeField]
        private int _lNum;

        private static readonly RoomName[] ExcludedRooms = new RoomName[1] { RoomName.HczCheckpointToEntranceZone };

        public RoomRoleSpawnpoint(Vector3 localPoint, float lookRotation, float lookAngleVariation, float boundsWidth, float boundsLength, int spawnpointsInWidth, int spawnpoitnsInLength, RoomName nameFilter = RoomName.Unnamed, FacilityZone zoneFilter = FacilityZone.None, RoomShape shapeFilter = RoomShape.Undefined)
        {
            _spawnpoints = new List<BoundsRoleSpawnpoint>();
            _fName = nameFilter;
            _fZone = zoneFilter;
            _fShape = shapeFilter;
            _localPoint = localPoint;
            _lookAngle = lookRotation;
            _angleVar = lookAngleVariation;
            _width = boundsWidth;
            _length = boundsLength;
            _wNum = spawnpointsInWidth;
            _lNum = spawnpoitnsInLength;
            RefreshSpawnpoints();
            SeedSynchronizer.OnMapGenerated += RefreshSpawnpoints;
        }

        public RoomRoleSpawnpoint(RoomRoleSpawnpoint t)
            : this(t._localPoint, t._lookAngle, t._angleVar, t._width, t._length, t._wNum, t._lNum, t._fName, t._fZone, t._fShape)
        {
        }

        public bool TryGetSpawnpoint(out Vector3 position, out float horizontalRot)
        {
            return _spawnpoints.RandomItem().TryGetSpawnpoint(out position, out horizontalRot);
        }

        private void RefreshSpawnpoints()
        {
            _spawnpoints.Clear();
            foreach (RoomIdentifier item in RoomIdUtils.FindRooms(_fName, _fZone, _fShape))
            {
                if (!ExcludedRooms.Contains(item.Name))
                {
                    Transform transform = item.transform;
                    Bounds bounds = new Bounds(transform.TransformPoint(_localPoint), transform.rotation * new Vector3(_width, 0f, _length));
                    Vector3 vector = transform.rotation * new Vector3(_wNum, 0f, _lNum);
                    Vector3Int size = new Vector3Int(Mathf.RoundToInt(Mathf.Abs(vector.x)), 1, Mathf.RoundToInt(Mathf.Abs(vector.z)));
                    float num = transform.rotation.eulerAngles.y + _lookAngle;
                    _spawnpoints.Add(new BoundsRoleSpawnpoint(bounds, num - _angleVar, num + _angleVar, size));
                }
            }
        }
    }
}
