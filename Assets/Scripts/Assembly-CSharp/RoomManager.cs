public class RoomManager : global::UnityEngine.MonoBehaviour
{
    [global::System.Serializable]
    public class Room
    {
        public string label;

        public Offset roomOffset;

        public global::UnityEngine.GameObject roomPrefab;

        public string type;

        public global::UnityEngine.Transform readonlyPoint;

        public Offset iconoffset;
    }

    [global::System.Serializable]
    public struct RoomPosition : global::System.IEquatable<RoomManager.RoomPosition>
    {
        public string type;

        public global::UnityEngine.Transform point;

        public global::UnityEngine.RectTransform ui_point;

        public bool Equals(RoomManager.RoomPosition other)
        {
            if (string.Equals(type, other.type) && point == other.point)
            {
                return ui_point == other.ui_point;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is RoomManager.RoomPosition other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (((((type != null) ? type.GetHashCode() : 0) * 397) ^ ((point != null) ? point.GetHashCode() : 0)) * 397) ^ ((ui_point != null) ? ui_point.GetHashCode() : 0);
        }

        public static bool operator ==(RoomManager.RoomPosition left, RoomManager.RoomPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RoomManager.RoomPosition left, RoomManager.RoomPosition right)
        {
            return !left.Equals(right);
        }
    }

    public bool isGenerated;

    public int useSimulator = -1;

    public global::System.Collections.Generic.List<RoomManager.Room> rooms = new global::System.Collections.Generic.List<RoomManager.Room>();

    public global::System.Collections.Generic.List<RoomManager.RoomPosition> positions = new global::System.Collections.Generic.List<RoomManager.RoomPosition>();

    private void Start()
    {
        if (useSimulator != -1)
        {
            GenerateMap(useSimulator);
        }
    }

    public void GenerateMap(int seed)
    {
        global::UnityEngine.Object.FindFirstObjectByType<global::GameCore.Console>();
        GetComponent<PocketDimensionGenerator>().GenerateMap(seed);
        for (int i = 0; i < positions.Count; i++)
        {
            positions[i].point.name = "POINT" + i;
            if (!(positions[i].point.GetComponent<Point>() != null))
            {
                global::UnityEngine.Debug.LogError("RoomManager: Missing 'Point' script at current position.");
                return;
            }
        }
        global::UnityEngine.Random.InitState(seed);
        global::GameCore.Console.AddLog("[MG REPLY]: Successfully recieved map seed!", new global::UnityEngine.Color32(0, byte.MaxValue, 0, byte.MaxValue), nospace: true);
        global::System.Collections.Generic.List<RoomManager.RoomPosition> list = positions;
        global::GameCore.Console.AddLog("[MG TASK]: Setting rooms positions...", new global::UnityEngine.Color32(0, byte.MaxValue, 0, byte.MaxValue));
        foreach (RoomManager.Room room in rooms)
        {
            global::GameCore.Console.AddLog("\t\t[MG INFO]: " + room.label + " is about to set!", new global::UnityEngine.Color32(120, 120, 120, byte.MaxValue), nospace: true);
            global::System.Collections.Generic.List<int> list2 = global::NorthwoodLib.Pools.ListPool<int>.Shared.Rent();
            for (int j = 0; j < list.Count; j++)
            {
                if (!positions[j].type.Equals(room.type))
                {
                    continue;
                }
                bool flag = true;
                Point[] componentsInChildren = room.roomPrefab.GetComponentsInChildren<Point>();
                foreach (Point point in componentsInChildren)
                {
                    if (positions[j].point.name == point.gameObject.name)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    list2.Add(j);
                }
            }
            Point[] componentsInChildren2 = room.roomPrefab.GetComponentsInChildren<Point>();
            for (int num = list2.Count - 1; num >= 0; num--)
            {
                Point[] componentsInChildren = componentsInChildren2;
                foreach (Point point2 in componentsInChildren)
                {
                    if (positions[list2[num]].point.name == point2.gameObject.name)
                    {
                        list2.RemoveAt(num);
                        break;
                    }
                }
            }
            int index = list2[global::UnityEngine.Random.Range(0, list2.Count)];
            global::NorthwoodLib.Pools.ListPool<int>.Shared.Return(list2);
            RoomManager.RoomPosition roomPosition = positions[index];
            global::UnityEngine.GameObject roomPrefab = room.roomPrefab;
            room.readonlyPoint = roomPosition.point;
            roomPrefab.transform.SetParent(roomPosition.point);
            roomPrefab.transform.localPosition = room.roomOffset.position;
            roomPrefab.transform.localRotation = global::UnityEngine.Quaternion.Euler(room.roomOffset.rotation);
            roomPrefab.transform.localScale = room.roomOffset.scale;
            roomPrefab.SetActive(value: true);
            positions.RemoveAt(index);
        }
        global::GameCore.Console.AddLog("--Map successfully generated--", new global::UnityEngine.Color32(0, byte.MaxValue, 0, byte.MaxValue));
        isGenerated = true;
    }
}
