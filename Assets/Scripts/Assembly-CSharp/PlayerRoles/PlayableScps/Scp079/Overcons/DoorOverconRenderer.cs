using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Overcons
{
    public class DoorOverconRenderer : PooledOverconRenderer
    {
        private const float DoorHeight = 1.25f;

        internal override void SpawnOvercons(Scp079Camera newCamera)
        {
            ReturnAll();

            if (!DoorVariant.DoorsByRoom.TryGetValue(newCamera.Room, out var doors))
                return;

            foreach (DoorVariant door in doors)
            {
                if (!Scp079DoorAbility.CheckVisibility(door, newCamera))
                    continue;

                Vector3 position;

                if (door is CheckpointDoor checkpointDoor)
                {
                    Vector3 zero = Vector3.zero;
                    
                    foreach (DoorVariant subDoor in checkpointDoor.SubDoors)
                        zero += subDoor.transform.position;
                    
                    position = zero / checkpointDoor.SubDoors.Length;
                }
                else
                {
                    position = door.transform.position;
                }

                DoorOvercon overcon = GetFromPool<DoorOvercon>();
                overcon.Target = door;
                overcon.transform.position = position + Vector3.up * DoorHeight;
                overcon.Rescale(newCamera);
            }
        }
    }
}
