namespace MapGeneration
{
    public class ImageGenerator : global::UnityEngine.MonoBehaviour
    {
        [global::System.Serializable]
        public class ColorMap
        {
            public global::UnityEngine.Color color = global::UnityEngine.Color.white;

            public global::MapGeneration.ImageGenerator.RoomType type;

            public float rotationY;

            public global::UnityEngine.Vector2 centerOffset;

            public float RandomizedRotation;
        }

        [global::System.Serializable]
        public class RoomsOfType
        {
            public global::System.Collections.Generic.List<global::MapGeneration.ImageGenerator.Room> roomsOfType = new global::System.Collections.Generic.List<global::MapGeneration.ImageGenerator.Room>();

            public int amount;
        }

        [global::System.Serializable]
        public class Room
        {
            public global::System.Collections.Generic.List<global::UnityEngine.GameObject> room = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

            public global::MapGeneration.ImageGenerator.RoomType type;

            public bool required;

            public Room(global::MapGeneration.ImageGenerator.Room r)
            {
                room = r.room;
                type = r.type;
                required = r.required;
            }
        }

        [global::System.Serializable]
        public class MinimapElement
        {
            public string roomName;

            public global::UnityEngine.Texture icon;

            public global::UnityEngine.Vector2 position;

            public int rotation;

            public global::UnityEngine.GameObject roomSource;
        }

        [global::System.Serializable]
        public class MinimapLegend
        {
            public string containsInName;

            public global::UnityEngine.Texture icon;

            public string label;
        }

        public enum RoomType
        {
            Straight = 0,
            Curve = 1,
            RoomT = 2,
            Cross = 3,
            Endoff = 4,
            Prison = 5
        }

        public int height;

        public global::UnityEngine.Texture2D[] maps;

        private global::UnityEngine.Texture2D map;

        private global::UnityEngine.Color[] copy;

        private string alias;

        public float gridSize;

        public float minimapSize;

        public global::System.Collections.Generic.List<global::MapGeneration.ImageGenerator.ColorMap> colorMap = new global::System.Collections.Generic.List<global::MapGeneration.ImageGenerator.ColorMap>();

        public global::System.Collections.Generic.List<global::MapGeneration.ImageGenerator.Room> availableRooms = new global::System.Collections.Generic.List<global::MapGeneration.ImageGenerator.Room>();

        private global::System.Collections.Generic.List<global::MapGeneration.ImageGenerator.MinimapElement> minimap = new global::System.Collections.Generic.List<global::MapGeneration.ImageGenerator.MinimapElement>();

        public global::MapGeneration.ImageGenerator.MinimapLegend[] legend;

        public global::UnityEngine.RectTransform minimapTarget;

        public float y_offset;

        public static PocketDimensionGenerator pocketDimensionGenerator;

        public static global::MapGeneration.ImageGenerator[] ZoneGenerators = new global::MapGeneration.ImageGenerator[3];

        private global::UnityEngine.Transform entrRooms;

        public global::MapGeneration.ImageGenerator.RoomsOfType[] roomsOfType;

        public bool GenerateMap(int seed, string newAlias, out string blackbox)
        {
            blackbox = string.Empty;
            alias = newAlias;
            if (!NonFacilityCompatibility.currentSceneSettings.enableWorldGeneration)
            {
                return true;
            }
            try
            {
                blackbox = "Activating available rooms.";
                foreach (global::MapGeneration.ImageGenerator.Room availableRoom in availableRooms)
                {
                    foreach (global::UnityEngine.GameObject item in availableRoom.room)
                    {
                        item.SetActive(value: false);
                    }
                }
                pocketDimensionGenerator = GetComponent<PocketDimensionGenerator>();
                pocketDimensionGenerator.GenerateMap(seed);
                blackbox = "Randomizing...";
                global::UnityEngine.Random.InitState(seed);
                blackbox = "Picking up a map atlas...";
                map = maps[global::UnityEngine.Random.Range(0, maps.Length)];
                blackbox = "Getting pixels...";
                copy = map.GetPixels();
                blackbox = "Checking rooms...";
                GeneratorTask_CheckRooms();
                blackbox = "Removing not required rooms...";
                GeneratorTask_RemoveNotRequired();
                blackbox = "Setting up rooms...";
                GeneratorTask_SetRooms();
                blackbox = "Entrance Zone initializing...";
                InitEntrance();
                blackbox = "Cleaning up...";
                GeneratorTask_Cleanup();
                blackbox = "Reventing map...";
                map.SetPixels(copy);
                map.Apply();
                if (entrRooms != null)
                {
                    entrRooms.parent = null;
                }
                blackbox = "Completed.";
            }
            catch (global::System.Exception ex)
            {
                blackbox = blackbox + "\nError: " + ex.Message;
                return false;
            }
            return true;
        }

        private void InitEntrance()
        {
            if (height == -1001)
            {
                global::MapGeneration.RoomIdentifier[] array = global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Where(global::MapGeneration.RoomIdentifier.AllRoomIdentifiers, (global::MapGeneration.RoomIdentifier x) => x.Name == global::MapGeneration.RoomName.HczCheckpointToEntranceZone && x.Zone == global::MapGeneration.FacilityZone.HeavyContainment));
                if (array.Length == 2)
                {
                    global::UnityEngine.Transform transform = array[0].transform;
                    global::UnityEngine.Transform transform2 = array[1].transform;
                    global::UnityEngine.Transform transform3 = ((transform.position.z > transform2.position.z) ? transform2 : transform);
                    global::UnityEngine.Transform transform4 = global::UnityEngine.GameObject.Find("ChkpRef").transform;
                    global::UnityEngine.GameObject.Find("EntranceRooms").transform.position += transform3.position - transform4.position;
                }
            }
        }

        private void GeneratorTask_Cleanup()
        {
            global::MapGeneration.ImageGenerator.RoomsOfType[] array = roomsOfType;
            for (int i = 0; i < array.Length; i++)
            {
                foreach (global::MapGeneration.ImageGenerator.Room item in array[i].roomsOfType)
                {
                    foreach (global::UnityEngine.GameObject item2 in item.room)
                    {
                        if (item.type != global::MapGeneration.ImageGenerator.RoomType.Prison)
                        {
                            item2.SetActive(value: false);
                        }
                    }
                }
            }
        }

        private void GeneratorTask_SetRooms()
        {
            for (int i = 0; i < map.height; i++)
            {
                for (int j = 0; j < map.width; j++)
                {
                    global::UnityEngine.Color pixel = map.GetPixel(j, i);
                    foreach (global::MapGeneration.ImageGenerator.ColorMap item in colorMap)
                    {
                        if (!(item.color != pixel))
                        {
                            global::UnityEngine.Vector2 pos = new global::UnityEngine.Vector2(j, i) + item.centerOffset;
                            PlaceRoom(pos, item);
                        }
                    }
                }
            }
        }

        private void GeneratorTask_RemoveNotRequired()
        {
            foreach (global::MapGeneration.ImageGenerator.ColorMap item in colorMap)
            {
                bool flag = false;
                while (!flag)
                {
                    int num = 0;
                    foreach (global::MapGeneration.ImageGenerator.Room item2 in roomsOfType[(int)item.type].roomsOfType)
                    {
                        num += item2.room.Count;
                    }
                    if (num <= roomsOfType[(int)item.type].amount)
                    {
                        break;
                    }
                    flag = true;
                    foreach (global::MapGeneration.ImageGenerator.Room item3 in roomsOfType[(int)item.type].roomsOfType)
                    {
                        if (!item3.required && item3.room.Count > 0)
                        {
                            item3.room[0].SetActive(value: false);
                            item3.room.RemoveAt(0);
                            flag = false;
                            break;
                        }
                    }
                }
            }
        }

        private void GeneratorTask_CheckRooms()
        {
            for (int i = 0; i < map.height; i++)
            {
                for (int j = 0; j < map.width; j++)
                {
                    global::UnityEngine.Color pixel = map.GetPixel(j, i);
                    foreach (global::MapGeneration.ImageGenerator.ColorMap item in colorMap)
                    {
                        if (item.color != pixel)
                        {
                            continue;
                        }
                        BlankSquare(new global::UnityEngine.Vector2(j, i) + item.centerOffset);
                        roomsOfType[(int)item.type].amount++;
                        global::System.Collections.Generic.List<global::MapGeneration.ImageGenerator.Room> list = global::NorthwoodLib.Pools.ListPool<global::MapGeneration.ImageGenerator.Room>.Shared.Rent();
                        bool flag = global::System.Linq.Enumerable.Any(availableRooms, (global::MapGeneration.ImageGenerator.Room room) => room.type == item.type && room.room.Count > 0 && room.required);
                        bool flag2;
                        do
                        {
                            flag2 = false;
                            for (int num = 0; num < availableRooms.Count; num++)
                            {
                                if (availableRooms[num].type == item.type && availableRooms[num].room.Count > 0 && !(!availableRooms[num].required && flag))
                                {
                                    list.Add(new global::MapGeneration.ImageGenerator.Room(availableRooms[num]));
                                    availableRooms.RemoveAt(num);
                                    flag2 = true;
                                    break;
                                }
                            }
                        }
                        while (flag2);
                        foreach (global::MapGeneration.ImageGenerator.Room item2 in list)
                        {
                            roomsOfType[(int)item.type].roomsOfType.Add(new global::MapGeneration.ImageGenerator.Room(item2));
                        }
                        global::NorthwoodLib.Pools.ListPool<global::MapGeneration.ImageGenerator.Room>.Shared.Return(list);
                    }
                }
            }
            map.SetPixels(copy);
            map.Apply();
        }

        private void PlaceRoom(global::UnityEngine.Vector2 pos, global::MapGeneration.ImageGenerator.ColorMap type)
        {
            string text = "";
            try
            {
                text = "ERR#1 (marking bitmap)";
                BlankSquare(pos);
                global::MapGeneration.ImageGenerator.Room room = null;
                text = "ERR#2 (looping)";
                do
                {
                    text = "ERR#3 (randomizing)";
                    int index = global::UnityEngine.Random.Range(0, roomsOfType[(int)type.type].roomsOfType.Count);
                    text = $"ERR#4 ({roomsOfType[(int)type.type].roomsOfType.Count} rooms remaining)";
                    room = roomsOfType[(int)type.type].roomsOfType[index];
                    if (room.room.Count == 0)
                    {
                        text = "ERR#5 (randomizing)";
                        roomsOfType[(int)type.type].roomsOfType.RemoveAt(index);
                    }
                }
                while (room.room.Count == 0);
                while (pos.x % 3f != 0f)
                {
                    pos.x += 1f;
                }
                pos.x /= 3f;
                while (pos.y % 3f != 0f)
                {
                    pos.y += 1f;
                }
                pos.y /= 3f;
                float num = type.rotationY + (float)global::UnityEngine.Random.Range(0, 4) * type.RandomizedRotation;
                room.room[0].transform.localPosition = new global::UnityEngine.Vector3(pos.x * gridSize, height, pos.y * gridSize);
                room.room[0].transform.localRotation = global::UnityEngine.Quaternion.Euler(global::UnityEngine.Vector3.up * (num + y_offset));
                text = "ERR#6 (preparing minimap)";
                if (minimapTarget != null)
                {
                    global::MapGeneration.ImageGenerator.MinimapLegend minimapLegend = null;
                    global::MapGeneration.ImageGenerator.MinimapLegend[] array = legend;
                    foreach (global::MapGeneration.ImageGenerator.MinimapLegend minimapLegend2 in array)
                    {
                        if (room.room[0].name.Contains(minimapLegend2.containsInName))
                        {
                            minimapLegend = minimapLegend2;
                        }
                    }
                    if (minimapLegend != null)
                    {
                        minimap.Add(new global::MapGeneration.ImageGenerator.MinimapElement
                        {
                            icon = minimapLegend.icon,
                            position = pos * 3f,
                            roomName = minimapLegend.label,
                            rotation = (int)num,
                            roomSource = room.room[0].gameObject
                        });
                    }
                }
                text = "ERR#7 (list element removal)";
                room.room[0].SetActive(value: true);
                room.room.RemoveAt(0);
            }
            catch (global::System.Exception ex)
            {
                global::MapGeneration.SeedSynchronizer.DebugError(isFatal: true, "Failed to generate a room of " + alias + " zone (TYPE#" + type.type.ToString() + "). Error code - " + text + " | Debug info - " + ex.Message);
            }
        }

        private void BlankSquare(global::UnityEngine.Vector2 centerPoint)
        {
            centerPoint = new global::UnityEngine.Vector2(centerPoint.x - 1f, centerPoint.y - 1f);
            for (ushort num = 0; num < 3; num++)
            {
                for (ushort num2 = 0; num2 < 3; num2++)
                {
                    map.SetPixel((int)centerPoint.x + num, (int)centerPoint.y + num2, new global::UnityEngine.Color(0.3921f, 0.3921f, 0.3921f, 1f));
                }
            }
            map.Apply();
        }

        private void Awake()
        {
            int num = -1;
            switch (height)
            {
                case 0:
                    num = 0;
                    break;
                case -1000:
                    num = 1;
                    break;
                case -1001:
                    num = 2;
                    break;
            }
            if (num < 0)
            {
                global::MapGeneration.SeedSynchronizer.DebugError(isFatal: true, "The array of Image Generators could not be set up. Height: " + height);
            }
            else
            {
                ZoneGenerators[num] = this;
            }
        }
    }
}
