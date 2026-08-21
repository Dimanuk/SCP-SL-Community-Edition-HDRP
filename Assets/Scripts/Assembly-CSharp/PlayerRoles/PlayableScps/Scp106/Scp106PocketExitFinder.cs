using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using Interactables;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps;
using UnityEngine;

using Random = UnityEngine.Random;
namespace PlayerRoles.PlayableScps.Scp106
{
    public static class Scp106PocketExitFinder
    {
        private const int RequiredTriggers = 2;
        private const int MaxArraySize = 64;
        private const int MaxDistanceSqr = 750;
        private const float ZombieSqrModifier = 0.3f;
        private const float RaycastRange = 11f;
        private const float SurfaceRaycastRange = 45f;
        private const float AngleVariation = 30f;

        private static readonly RoomName[] BlacklistedRooms = new RoomName[]
        {
            RoomName.Hcz079,
            RoomName.LczCheckpointA,
            RoomName.LczCheckpointB,
            RoomName.HczCheckpointToEntranceZone,
            RoomName.LczClassDSpawn,
            RoomName.HczTesla
        };

        private static readonly Dictionary<FacilityZone, DoorVariant[]> WhitelistedDoorsForZone = new Dictionary<FacilityZone, DoorVariant[]>();
        private static readonly DoorVariant[] DoorsNonAlloc = new DoorVariant[64];
        private static readonly Vector3[] PositionsCache = new Vector3[64];
        private static readonly bool[] PositionModifiers = new bool[64];
        private static readonly int Mask = LayerMask.GetMask("Default", "Glass");
        private static readonly Vector3 GroundOffset = new Vector3(0f, 0.25f, 0f);

        public static Vector3 GetBestExitPosition(IFpcRole role)
        {
            if (!NetworkServer.active)
                throw new InvalidOperationException("Scp106PocketExitFinder.GetBestExitPosition is a server-side only method!");

            if (!(role is PlayerRoleBase playerRoleBase) || !playerRoleBase.TryGetOwner(out ReferenceHub hub))
                throw new InvalidOperationException("Scp106PocketExitFinder.GetBestExitPosition provided with non-compatible role!");

            Vector3 position = hub.playerEffectsController.GetEffect<Corroding>().CapturePosition.Position;
            RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPositionRaycasts(position);
            
            if (roomIdentifier == null)
                return position;

            DoorVariant[] whitelistedDoorsForZone = GetWhitelistedDoorsForZone(roomIdentifier.Zone);
            if (whitelistedDoorsForZone.Length == 0)
                return position;

            DoorVariant randomDoor = GetRandomDoor(whitelistedDoorsForZone);
            float range = (roomIdentifier.Zone == FacilityZone.Surface) ? SurfaceRaycastRange : RaycastRange;
            return GetSafePositionForDoor(randomDoor, range, role.FpcModule.CharController);
        }

        private static Vector3 GetSafePositionForDoor(DoorVariant dv, float range, CharacterController ctrl)
        {
            Vector3 position = dv.transform.position;
            Vector3 forward = dv.transform.forward;
            
            if (Random.value > 0.5f)
                forward = -forward;

            forward = Quaternion.Euler(0f, Random.Range(-AngleVariation, AngleVariation), 0f) * forward;

            float radius = ctrl.radius;
            float num = Mathf.Lerp(radius, range, Random.value);
            Vector3 vector2 = Vector3.up * (ctrl.height / 2f);
            Vector3 vector3 = position + ctrl.center + GroundOffset + Vector3.up * radius;

            if (!Physics.SphereCast(vector3, radius, forward, out RaycastHit hitInfo, num + radius, FpcStateProcessor.Mask))
            {
                return vector3 + forward * num + vector2;
            }

            return hitInfo.point + hitInfo.normal * radius + vector2;
        }

        private static DoorVariant GetRandomDoor(DoorVariant[] doors)
        {
            int num = 0;
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.roleManager.CurrentRole is FpcStandardScp fpcStandardScp)
                {
                    PositionsCache[num] = fpcStandardScp.FpcModule.Position;
                    PositionModifiers[num] = fpcStandardScp.RoleTypeId == RoleTypeId.Scp0492;
                    if (++num >= MaxArraySize)
                        break;
                }
            }

            if (num == 0)
                return doors.RandomItem();

            DoorVariant result = null;
            float num2 = float.MaxValue;
            int num3 = 0;

            foreach (DoorVariant doorVariant in doors)
            {
                float num4 = 0f;
                bool flag = true;
                
                for (int j = 0; j < num; j++)
                {
                    float num5 = (doorVariant.transform.position - PositionsCache[j]).sqrMagnitude;
                    if (PositionModifiers[j])
                        num5 *= ZombieSqrModifier;

                    if (num5 < MaxDistanceSqr)
                        flag = false;

                    num4 += num5;
                }

                if (flag)
                    DoorsNonAlloc[num3++] = doorVariant;

                if (num4 < num2)
                {
                    result = doorVariant;
                    num2 = num4;
                }
            }

            if (num3 != 0)
                return DoorsNonAlloc[Random.Range(0, num3)];

            return result;
        }

        private static DoorVariant[] GetWhitelistedDoorsForZone(FacilityZone zone)
        {
            if (WhitelistedDoorsForZone.TryGetValue(zone, out DoorVariant[] value))
                return value;

            int num = 0;
            foreach (DoorVariant allDoor in DoorVariant.AllDoors)
            {
                if (allDoor.Rooms.Length != 0 && allDoor.Rooms[0].Zone == zone && ValidateDoor(allDoor))
                {
                    DoorsNonAlloc[num] = allDoor;
                    if (++num >= MaxArraySize)
                        break;
                }
            }

            DoorVariant[] array = new DoorVariant[num];
            Array.Copy(DoorsNonAlloc, array, num);
            WhitelistedDoorsForZone[zone] = array;
            return array;
        }

        private static bool ValidateDoor(DoorVariant dv)
        {
            if (!(dv is BasicDoor basicDoor))
                return false;

            if (dv.RequiredPermissions.RequiredPermissions != KeycardPermissions.None)
                return false;

            if (!InteractableCollider.AllInstances.TryGetValue(basicDoor, out Dictionary<byte, InteractableCollider> value))
                return false;

            if (value.Count < RequiredTriggers)
                return false;

            foreach (RoomIdentifier room in basicDoor.Rooms)
            {
                if (Array.IndexOf(BlacklistedRooms, room.Name) >= 0)
                    return false;
            }

            return true;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            SeedSynchronizer.OnMapGenerated += WhitelistedDoorsForZone.Clear;
        }
    }
}