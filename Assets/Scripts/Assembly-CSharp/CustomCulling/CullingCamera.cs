using MapGeneration;
using PlayerRoles;
using PlayerStatsSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; // +

namespace CustomCulling
{
    [DisallowMultipleComponent]
    public class CullingCamera : MonoBehaviour
    {
        public int RoomDepth = 2;

        private const float FarVisionAngleMultiplier = 1.2f;
        private const float DoorDistanceAdjustment = 1f;
        public const float NoClipInitialCullDistance = 75f;
        private const float NullRoomFallbackDistance = 30f;
        private const float ForwardDotThreshold = -0.25f;

        internal static List<CullableRoom> EnabledCullableRooms;
        internal static CullableRoom[] AllRooms;
        internal static CullableRoom OutsideRoom;
        internal static NoClipCamExtraPoint[] NoClipCamPoints;
        internal static AspectRatioSync AspectSyncer;

        private static float _farVisionAngle;
        private static float _closeToDoorDistance;
        private static Vector3 _cachedPosition;
        private static Vector2 _cached2DPos;
        private static Vector2 _cached2DLookDir;
        private static CullableRoom _myCachedRoom;

        private Transform _transform;
        private Camera _camera; // +

        static CullingCamera()
        {
            EnabledCullableRooms = new List<CullableRoom>(8);
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _camera = GetComponent<Camera>(); // +
        }
                // Далее до CacheFrameValues идёт изменённый код, убран onprecul т.к. он не совместим с hdrp. Изменил логику вызова, так что может что-то ещё сломаться.
        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {

            if (camera != _camera)
                return;

            if (_transform == null || AspectSyncer == null)
                return;

            CacheFrameValues();
            ResetCullableBases();

            if (Outside.Singleton != null && Outside.Singleton.LocalHostIsOutside)
            {
                EnableCullableRoom(OutsideRoom, CullableRoom.RoomState.Enabled, null);
            }
            else
            {
                if (ReferenceHub.TryGetLocalHub(out ReferenceHub hub)
                    && hub.playerStats.GetModule<AdminFlagsStat>().HasFlag((AdminFlags)1))
                {
                    DistanceBasedCulling(NoClipInitialCullDistance);
                    UpdateCullableBases();
                    return;
                }

                RoomIdentifier roomId = RoomIdUtils.RoomAtPositionRaycasts(_cachedPosition, true);
                _myCachedRoom = roomId != null ? roomId.GetComponent<CullableRoom>() : null;

                if (_myCachedRoom != null)
                {
                    EnableCullableRoom(_myCachedRoom, CullableRoom.RoomState.Enabled, null);

                    if (_myCachedRoom.Doors != null)
                        foreach (DoorLinkingRooms door in _myCachedRoom.Doors)
                            EnableNeighborRoom(door);
                }
                else
                {
                    DistanceBasedCulling(NullRoomFallbackDistance);
                }
            }

            UpdateCullableBases();
        }

        private void CacheFrameValues()
        {
            Vector3 position = _transform.position;
            _cachedPosition = position;
            _cached2DPos = new Vector2(position.x, position.z);

            Vector3 forward = _transform.forward;
            _cached2DLookDir = new Vector2(forward.x, forward.z);

            _farVisionAngle = AspectSyncer.XScreenEdge * FarVisionAngleMultiplier;
            _closeToDoorDistance = AspectSyncer.AspectRatio * 1.1f + DoorDistanceAdjustment;
        }

        private void EnableNeighborRoom(DoorLinkingRooms door)
        {
            if (door == null) return;

            bool inForward = door.ForwardRoom != null
                             && door.ForwardRoom.gameObject == _myCachedRoom.gameObject;

            CullableRoom neighbor = inForward ? door.BackwardRoom : door.ForwardRoom;
            if (neighbor == null) return;

            EnableCullableRoom(neighbor, CullableRoom.RoomState.Enabled, null);

            if (RoomDepth < 2) return;

            Vector3 toDoor = door.transform.position - _cachedPosition;
            Vector2 toDoor2D = new Vector2(toDoor.x, toDoor.z);
            float dot = Vector2.Dot(toDoor2D.normalized, _cached2DLookDir.normalized);

            if (dot >= ForwardDotThreshold)
                TestFarDoor(neighbor, door, RoomDepth - 1);
        }

        private void DistanceBasedCulling(float distanceToCheck)
        {
            if (AllRooms != null)
            {
                foreach (CullableRoom room in AllRooms)
                {
                    if (room == null) continue;
                    if (Vector3.Distance(_cachedPosition, room.transform.position) < distanceToCheck)
                        EnableCullableRoom(room, CullableRoom.RoomState.Enabled, null);
                }
            }

            if (NoClipCamPoints == null)
                return;

            foreach (NoClipCamExtraPoint point in NoClipCamPoints)
            {
                if (point == null) continue;
                if (Vector3.Distance(_cachedPosition, point.transform.position) < distanceToCheck)
                {
                    CullableRoom cr = point.CullableRoom;
                    if (cr != null)
                        EnableCullableRoom(cr, CullableRoom.RoomState.Enabled, null);
                }
            }
        }

        private void ResetCullableBases()
        {
            if (EnabledCullableRooms == null)
                return;

            foreach (CullableRoom room in EnabledCullableRooms)
            {
                if (room == null) continue;
                room.CurrentRoomState = CullableRoom.RoomState.Disabled;
                room.CrossoverDoor = null;
            }
        }

        private void TestFarDoor(CullableRoom targetRoom, DoorLinkingRooms myDoor, int renderDepth)
        {
            if (targetRoom?.Doors == null || myDoor == null)
                return;

            CullableRoom nextRoom = null;
            DoorLinkingRooms nextDoor = null;

            foreach (DoorLinkingRooms door in targetRoom.Doors)
            {
                if (door == null || door.gameObject == myDoor.gameObject)
                    continue;

                if (TestFarDoorInner(door, targetRoom, renderDepth, out CullableRoom candidate))
                {
                    nextRoom = candidate;
                    nextDoor = door;
                }
            }

            if (nextRoom != null)
                TestFarDoor(nextRoom, nextDoor, renderDepth - 1);
        }

        private bool TestFarDoorInner(
            DoorLinkingRooms door,
            CullableRoom currentRoom,
            int renderDepth,
            out CullableRoom nextRoom)
        {
            nextRoom = null;

            if (door == null || currentRoom == null)
                return false;

            CullableRoom targetRoom = (door.ForwardRoom != null && door.ForwardRoom.gameObject == currentRoom.gameObject)
                ? door.BackwardRoom
                : door.ForwardRoom;

            if (targetRoom == null)
                return false;

            Vector3 doorPos = door.transform.position;
            Vector2 doorPos2D = new Vector2(doorPos.x, doorPos.z);
            float angle = Vector2.Angle(doorPos2D - _cached2DPos, _cached2DLookDir);
            bool inVision = angle < _farVisionAngle;

            if (door.CanNotSeeThrough)
            {
                if (inVision)
                    EnableCullableRoom(targetRoom, CullableRoom.RoomState.DynamicBases, door);
                return false;
            }

            if (inVision)
            {
                EnableCullableRoom(targetRoom, CullableRoom.RoomState.Enabled, door);

                if (renderDepth > 0)
                {
                    nextRoom = targetRoom;
                    return true;
                }
                return false;
            }

            if (renderDepth <= 0)
                return false;

            bool lookingThroughForward = door.ForwardRoom != null
                                            && door.ForwardRoom.gameObject == currentRoom.gameObject;
            Vector3 doorDir = lookingThroughForward ? door.transform.forward : -door.transform.forward;

            Vector2 projected = doorPos2D + new Vector2(doorDir.x, doorDir.z);
            float dist = Vector2.Distance(_cached2DPos, projected);
            float range = lookingThroughForward ? door.BackwardRoomLightRange : door.ForwardRoomLightRange;

            if (Mathf.Max(range, _closeToDoorDistance) > dist)
                EnableCullableRoom(targetRoom, CullableRoom.RoomState.AllCrossover, door);

            return false;
        }

        private void UpdateCullableBases()
        {
            if (EnabledCullableRooms == null)
                return;

            for (int i = EnabledCullableRooms.Count - 1; i >= 0; i--)
            {
                CullableRoom room = EnabledCullableRooms[i];
                if (room == null)
                {
                    EnabledCullableRooms.RemoveAt(i);
                    continue;
                }

                room.UpdateCulling();

                if (room.ReadyToRemove)
                {
                    room.IsTracked = false;
                    EnabledCullableRooms.RemoveAt(i);
                }
            }
        }

        private static int GetStatePriority(CullableRoom.RoomState state)
        {
            switch (state)
            {
                case CullableRoom.RoomState.Enabled: return 4;
                case CullableRoom.RoomState.LightCrossover: return 3;
                case CullableRoom.RoomState.DynamicBases: return 2;
                case CullableRoom.RoomState.AllCrossover: return 1;
                case CullableRoom.RoomState.Disabled: return 0;
                default: return -1;
            }
        }

        private void EnableCullableRoom(
            CullableRoom cullableRoom,
            CullableRoom.RoomState roomState,
            DoorLinkingRooms crossoverDoor)
        {
            if (cullableRoom == null)
                return;

            if (GetStatePriority(cullableRoom.CurrentRoomState) < GetStatePriority(roomState))
            {
                cullableRoom.CrossoverDoor = crossoverDoor;
                cullableRoom.CurrentRoomState = roomState;
            }

            EnabledCullableRooms ??= new List<CullableRoom>();

            if (!cullableRoom.IsTracked)
            {
                cullableRoom.IsTracked = true;
                EnabledCullableRooms.Add(cullableRoom);
            }
        }
    }
}