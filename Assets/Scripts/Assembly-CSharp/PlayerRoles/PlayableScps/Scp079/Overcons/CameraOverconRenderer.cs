using System.Collections.Generic;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Overcons
{
    public class CameraOverconRenderer : PooledOverconRenderer
    {
        private const float MaxSqrDistanceOtherRoom = 2165f;
        private const float ElevatorIconHeight = 3f;
        private const float ElevatorHeightMaxDiff = 50f;

        public static HashSet<CameraOvercon> VisibleOvercons = new HashSet<CameraOvercon>();

        internal override void SpawnOvercons(Scp079Camera newCamera)
        {
            ReturnAll();
            VisibleOvercons.Clear();

            foreach (Scp079InteractableBase allInstance in Scp079InteractableBase.AllInstances)
            {
                if (CheckVisibility(newCamera, allInstance))
                {
                    CameraOvercon fromPool = GetFromPool<CameraOvercon>();
                    fromPool.Setup(newCamera, allInstance as Scp079Camera, isElevator: false);
                    VisibleOvercons.Add(fromPool);
                }
            }

            if (!DoorVariant.DoorsByRoom.TryGetValue(newCamera.Room, out var doors))
                return;

            foreach (DoorVariant door in doors)
            {
                if (!(door is ElevatorDoor elevatorDoor)
                    || Mathf.Abs(elevatorDoor.TargetPosition.y - newCamera.Position.y) > ElevatorHeightMaxDiff
                    || GetClosestCamera(elevatorDoor.transform.position, newCamera.Room) != newCamera
                    || !ElevatorDoor.AllElevatorDoors.TryGetValue(elevatorDoor.Group, out var elevatorGroup))
                {
                    continue;
                }

                ElevatorDoor otherDoor = elevatorGroup[(elevatorGroup.IndexOf(elevatorDoor) + 1) % elevatorGroup.Count];

                if (otherDoor.Rooms.Length == 1)
                {
                    Scp079Camera closestCamera = GetClosestCamera(otherDoor.transform.position, otherDoor.Rooms[0]);
                    if (closestCamera != null)
                    {
                        CameraOvercon fromPool = GetFromPool<CameraOvercon>();
                        fromPool.Setup(newCamera, closestCamera, isElevator: true);
                        fromPool.Position = elevatorDoor.transform.position + Vector3.up * ElevatorIconHeight;
                        fromPool.Rescale(newCamera);
                        VisibleOvercons.Add(fromPool);
                    }
                }
            }
        }

        private Scp079Camera GetClosestCamera(Vector3 pos, RoomIdentifier targetRoom)
        {
            Scp079Camera result = null;
            float bestSqr = float.MaxValue;

            foreach (Scp079InteractableBase allInstance in Scp079InteractableBase.AllInstances)
            {
                if (allInstance is Scp079Camera cam && cam.Room == targetRoom)
                {
                    float sqr = (cam.Position - pos).sqrMagnitude;
                    if (sqr <= bestSqr)
                    {
                        result = cam;
                        bestSqr = sqr;
                    }
                }
            }

            return result;
        }

        private bool CheckVisibility(Scp079Camera cur, Scp079InteractableBase target)
        {
            if (!(target is Scp079Camera scp079Camera) || cur == scp079Camera)
                return false;

            if (cur.Room == target.Room)
                return true;

            if ((cur.Position - target.Position).sqrMagnitude > MaxSqrDistanceOtherRoom)
                return false;

            if (!scp079Camera.IsMain)
                return false;

            foreach (DoorVariant door in DoorVariant.DoorsByRoom[cur.Room])
            {
                if (door.Rooms.Contains(scp079Camera.Room))
                    return true;
            }

            return false;
        }
    }
}