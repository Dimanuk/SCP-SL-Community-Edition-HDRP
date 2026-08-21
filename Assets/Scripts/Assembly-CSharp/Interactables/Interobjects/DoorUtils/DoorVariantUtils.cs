namespace Interactables.Interobjects.DoorUtils
{
    public static class DoorVariantUtils
    {
        public static bool IsInZone(this global::Interactables.Interobjects.DoorUtils.DoorVariant door, global::MapGeneration.FacilityZone facilityZone)
        {
            global::MapGeneration.RoomIdentifier[] rooms = door.Rooms;
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].Zone == facilityZone)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
