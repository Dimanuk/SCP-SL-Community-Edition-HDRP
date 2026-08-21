using System.Collections.Generic;
using UnityEngine;
using MEC;
using MapGeneration;
using PlayerRoles;

namespace CustomCulling
{
    public class CullingManager : MonoBehaviour
    {
        private static bool _allLightsDisabled;

        public static bool Initialized { get; set; }

        public static bool AllLightsDisabled
        {
            get => _allLightsDisabled;
            set
            {
                if (_allLightsDisabled == value)
                    return;

                _allLightsDisabled = value;

                if (CullingCamera.EnabledCullableRooms == null)
                    return;

                for (int i = CullingCamera.EnabledCullableRooms.Count - 1; i >= 0; i--)
                {
                    CullableRoom room = CullingCamera.EnabledCullableRooms[i];
                    if (room == null)
                    {
                        CullingCamera.EnabledCullableRooms.RemoveAt(i);
                        continue;
                    }
                    room.UpdateAllLighting();
                }
            }
        }

        public static void TryAddBehaviours<T>(T[] behaviours, CullableRoom room = null)
            where T : Behaviour
        {
            if (behaviours == null || behaviours.Length == 0)
                return;

            if (room == null)
            {
                if (behaviours[0] == null || !CheckForRoom(behaviours[0].transform, out room))
                    return;
            }

            foreach (T behaviour in behaviours)
                TryAddBehaviour(behaviour, room);
        }

        public static void TryAddBehaviours(Renderer[] renderers, CullableRoom room = null)
        {
            if (renderers == null || renderers.Length == 0)
                return;

            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                    continue;

                RoomIdentifier rid = RoomIdUtils.RoomAtPositionRaycasts(renderer.transform.position, false);
                CullableRoom rendererRoom = rid != null ? rid.GetComponent<CullableRoom>() : null;

                if (rendererRoom == null)
                    continue;

                rendererRoom.Renderers.Add(renderer);
                renderer.enabled = rendererRoom.CurrentUpdateType.UpdateRenderers;
            }
        }

        public static void TryAddBehaviour<T>(T behaviour, CullableRoom room = null)
            where T : Behaviour
        {
            if (behaviour == null)
                return;

            if (room == null && !CheckForRoom(behaviour.transform, out room))
                return;

            Light light = behaviour.GetComponent<Light>();
            if (light != null)
            {
                if (room != null)
                {
                    room.Lights.Add(light);
                    light.enabled = room.CurrentUpdateType.UpdateLights && !AllLightsDisabled;
                    room.AddLightToNearbyDoors(light);
                }
                return;
            }

            var decal = behaviour.GetComponent<Knife.DeferredDecals.Decal>();
            if (decal != null)
            {
                room?.AddDecal(decal);
                return;
            }

            if (room != null)
            {
                room.OtherBehaviours.Add(behaviour);
                behaviour.enabled = room.CurrentUpdateType.UpdateBehaviours;
            }
        }

        public static void TryAddBehaviour(Renderer renderer, CullableRoom room = null)
        {
            if (renderer == null)
                return;

            if (room == null && !CheckForRoom(renderer.transform, out room))
                return;

            if (room != null)
            {
                room.Renderers.Add(renderer);
                renderer.enabled = room.CurrentUpdateType.UpdateRenderers;
            }
        }

        internal static bool IsRendererValid(Renderer targetRenderer)
        {
            if (targetRenderer == null) return false;
            GameObject go = targetRenderer.gameObject;
            string name = go != null ? go.name : null;
            return name != null && !name.StartsWith("RID");
        }

        private void Awake()
        {
            Timing.RunCoroutine(DelayInitCull());
        }

        private void OnDestroy()
        {
            Initialized = false;
            _allLightsDisabled = false;

            CullingCamera.AspectSyncer = null;
            CullingCamera.AllRooms = null;
            CullingCamera.OutsideRoom = null;
            CullingCamera.NoClipCamPoints = null;

            if (CullingCamera.EnabledCullableRooms != null)
            {
                CullingCamera.EnabledCullableRooms.Clear();
                CullingCamera.EnabledCullableRooms = null;
            }
        }

        private bool CanInitializeCulling()
        {
            if (!SeedSynchronizer.MapGenerated)
                return false;

            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub hub))
                return false;

            return PlayerRolesUtils.GetRoleId(hub) != RoleTypeId.None;
        }

        private IEnumerator<float> DelayInitCull()
        {
            yield return Timing.WaitUntilTrue(CanInitializeCulling);

            Initialized = true;
            AllLightsDisabled = false;

            CullingCamera.NoClipCamPoints = Object.FindObjectsByType<NoClipCamExtraPoint>(FindObjectsSortMode.None);

            CullableRoom[] allRooms = Object.FindObjectsByType<CullableRoom>(FindObjectsSortMode.None);
            CullingCamera.AllRooms = allRooms;

            foreach (CullableRoom room in allRooms)
            {
                if (room == null) continue;
                if (room.transform.position.y > 900f)
                {
                    CullingCamera.OutsideRoom = room;
                    break;
                }
            }

            foreach (DoorLinkingRooms door in Object.FindObjectsByType<DoorLinkingRooms>(FindObjectsSortMode.None))
                door?.Initialize();

            TryAddBehaviours(Object.FindObjectsByType<Terrain>(FindObjectsSortMode.None));

            ReferenceHub localHub = ReferenceHub.LocalHub;
            if (localHub != null && localHub.aspectRatioSync != null)
                CullingCamera.AspectSyncer = localHub.aspectRatioSync;

            CullingCamera.EnabledCullableRooms = new List<CullableRoom>();

            yield break;
        }

        private static bool CheckForRoom(Transform cullTransform, out CullableRoom room)
        {
            room = null;
            if (cullTransform == null)
                return false;

            RoomIdentifier roomId = RoomIdUtils.RoomAtPositionRaycasts(cullTransform.position, false);
            if (roomId != null)
                room = roomId.GetComponent<CullableRoom>();

            return room != null;
        }
    }
}