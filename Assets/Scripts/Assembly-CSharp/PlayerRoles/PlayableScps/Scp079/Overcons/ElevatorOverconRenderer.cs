using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Overcons
{
    public class ElevatorOverconRenderer : PooledOverconRenderer
    {
        private const float Height = 1.25f;

        internal override void SpawnOvercons(Scp079Camera newCamera)
        {
            ReturnAll();

            if (!DoorVariant.DoorsByRoom.TryGetValue(newCamera.Room, out var doors))
                return;

            foreach (DoorVariant door in doors)
            {
                if (door is ElevatorDoor elevatorDoor)
                {
                    ElevatorOvercon overcon = GetFromPool<ElevatorOvercon>();
                    overcon.Target = elevatorDoor;
                    overcon.transform.position = elevatorDoor.transform.position + Vector3.up * Height;
                    overcon.Rescale(newCamera);
                }
            }
        }
    }
}
