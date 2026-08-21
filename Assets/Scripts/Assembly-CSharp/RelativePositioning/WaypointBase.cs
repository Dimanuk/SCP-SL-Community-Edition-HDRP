namespace RelativePositioning
{
    public abstract class WaypointBase : global::UnityEngine.MonoBehaviour
    {
        private static readonly bool[] SetWaypoints = new bool[255];

        private static readonly byte[] WaypointIndexes = new byte[255];

        private static readonly global::RelativePositioning.WaypointBase[] AllWaypoints = new global::RelativePositioning.WaypointBase[255];

        private byte _id;

        private byte _index;

        protected abstract float SqrDistanceTo(global::UnityEngine.Vector3 pos);

        public abstract global::UnityEngine.Vector3 GetWorldspacePosition(global::UnityEngine.Vector3 relPosition);

        public abstract global::UnityEngine.Vector3 GetRelativePosition(global::UnityEngine.Vector3 worldPos);

        public virtual global::UnityEngine.Quaternion GetWorldspaceRotation(global::UnityEngine.Quaternion relRotation)
        {
            return relRotation;
        }

        public virtual global::UnityEngine.Quaternion GetRelativeRotation(global::UnityEngine.Quaternion worldRot)
        {
            return worldRot;
        }

        protected virtual void Start()
        {
            for (byte b = 1; b < byte.MaxValue; b++)
            {
                if (!SetWaypoints[b])
                {
                    _index = b;
                    AllWaypoints[b] = this;
                    SetWaypoints[b] = true;
                    return;
                }
            }
            global::UnityEngine.Debug.LogError("Could not add waypoint '" + base.name + "' - the list is full.");
        }

        protected virtual void OnDestroy()
        {
            AllWaypoints[_index] = null;
            SetWaypoints[_index] = false;
        }

        protected void SetId(byte newId)
        {
            if (newId == 0)
            {
                throw new global::System.InvalidOperationException("Cannot assign ID of 0 to a waypoint. This ID is reserved for the value of null.");
            }
            _id = newId;
            WaypointIndexes[_id] = _index;
        }

        public static void GetRelativePosition(global::UnityEngine.Vector3 point, out byte closestId, out global::UnityEngine.Vector3 rel)
        {
            float num = float.MaxValue;
            rel = global::UnityEngine.Vector3.zero;
            closestId = 0;
            global::RelativePositioning.WaypointBase waypointBase = null;
            for (byte b = 1; b < byte.MaxValue; b++)
            {
                if (SetWaypoints[b])
                {
                    global::RelativePositioning.WaypointBase waypointBase2 = AllWaypoints[b];
                    float num2 = waypointBase2.SqrDistanceTo(point);
                    if (!(num2 > num))
                    {
                        num = num2;
                        waypointBase = waypointBase2;
                        closestId = waypointBase2._id;
                    }
                }
            }
            if (closestId != 0)
            {
                rel = waypointBase.GetRelativePosition(point);
            }
        }

        public static global::UnityEngine.Vector3 GetWorldPosition(byte id, global::UnityEngine.Vector3 point)
        {
            if (!TryGetWaypoint(id, out var wp))
            {
                return point;
            }
            return wp.GetWorldspacePosition(point);
        }

        public static global::UnityEngine.Quaternion GetRelativeRotation(byte id, global::UnityEngine.Quaternion rot)
        {
            if (!TryGetWaypoint(id, out var wp))
            {
                return rot;
            }
            return wp.GetRelativeRotation(rot);
        }

        public static global::UnityEngine.Quaternion GetWorldRotation(byte id, global::UnityEngine.Quaternion rot)
        {
            if (!TryGetWaypoint(id, out var wp))
            {
                return rot;
            }
            return wp.GetWorldspaceRotation(rot);
        }

        public static bool TryGetWaypoint(byte id, out global::RelativePositioning.WaypointBase wp)
        {
            int num = WaypointIndexes[id];
            bool result = SetWaypoints[num];
            wp = AllWaypoints[num];
            return result;
        }
    }
}
