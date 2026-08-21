using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using PostProcessing;
using System.Collections.Generic;
using UnityEngine;

namespace CustomCulling
{
    public class DoorLinkingRooms : CullableBase
    {
        internal float ForwardRoomLightRange;
        internal CullableRoom ForwardRoom;
        internal List<Light> ForwardRoomNearbyLights = new List<Light>();

        internal float BackwardRoomLightRange;
        internal CullableRoom BackwardRoom;
        internal List<Light> BackwardRoomNearbyLights = new List<Light>();

        private DoorVariant _doorVariant;
        private bool _initialized;
        private bool _lastCanNotSeeThrough;

        internal bool CanNotSeeThrough
        {
            get
            {
                if (_doorVariant == null || _doorVariant.CanSeeThrough)
                    return false;

                return !_doorVariant.IsConsideredOpen();
            }
        }

        internal CullableRoom GetForwardRoom() => ForwardRoom;
        internal CullableRoom GetBackwardRoom() => BackwardRoom;

        internal CullableRoom GetOtherRoom(CullableRoom currentRoom)
        {
            if (ForwardRoom != null && ForwardRoom.gameObject == currentRoom.gameObject)
                return BackwardRoom;
            return ForwardRoom;
        }

        internal Vector3 GetOtherSidePosition() => transform.position - transform.forward;

        internal void RemoveLight(Light light1)
        {
            if (light1 == null) return;
            ForwardRoomNearbyLights.Remove(light1);
            BackwardRoomNearbyLights.Remove(light1);
        }

        internal void AddLight(bool isForwardRoom, Light light1, float distanceToDoor)
        {
            if (light1 == null) return;

            float effectiveRange = light1.range - distanceToDoor;

            if (isForwardRoom)
            {
                if (effectiveRange > ForwardRoomLightRange)
                    ForwardRoomLightRange = effectiveRange;
                ForwardRoomNearbyLights.Add(light1);
            }
            else
            {
                if (effectiveRange > BackwardRoomLightRange)
                    BackwardRoomLightRange = effectiveRange;
                BackwardRoomNearbyLights.Add(light1);
            }
        }

        internal void RefreshDistances(bool forwardRoom)
        {
            List<Light> lights = forwardRoom ? ForwardRoomNearbyLights : BackwardRoomNearbyLights;
            Vector3 doorPos = transform.position;
            float maxEffectiveRange = 0f;

            if (lights != null)
            {
                for (int i = lights.Count - 1; i >= 0; i--)
                {
                    Light light = lights[i];
                    if (light == null || light.transform == null)
                    {
                        lights.RemoveAt(i);
                        continue;
                    }

                    float effectiveRange = light.range - Vector3.Distance(doorPos, light.transform.position);
                    if (effectiveRange > maxEffectiveRange)
                        maxEffectiveRange = effectiveRange;
                }
            }

            if (forwardRoom)
                ForwardRoomLightRange = maxEffectiveRange;
            else
                BackwardRoomLightRange = maxEffectiveRange;
        }

        internal void Initialize()
        {
            if (_initialized || !SeedSynchronizer.MapGenerated)
                return;

            Transform t = transform;
            Vector3 pos = t.position;
            Vector3 forward = t.forward;

            RoomIdentifier forwardId = RoomIdUtils.RoomAtPosition(pos + forward);
            RoomIdentifier backwardId = RoomIdUtils.RoomAtPosition(pos - forward);

            if (forwardId != null) ForwardRoom = forwardId.GetComponent<CullableRoom>();
            if (backwardId != null) BackwardRoom = backwardId.GetComponent<CullableRoom>();

            if (ForwardRoom == null || BackwardRoom == null)
                return;

            _initialized = true;

            if (ForwardRoom == BackwardRoom)
            {
                ForwardRoom.AddCullableBase(this);
                return;
            }

            _doorVariant = GetComponent<DoorVariant>();
            _lastCanNotSeeThrough = CanNotSeeThrough;

            ForwardRoom.Doors ??= new List<DoorLinkingRooms>();
            BackwardRoom.Doors ??= new List<DoorLinkingRooms>();

            ForwardRoom.Doors.Add(this);
            BackwardRoom.Doors.Add(this);

            GetNearbyLights(true);
            GetNearbyLights(false);
        }

        private void GetNearbyLights(bool isForward)
        {
            CullableRoom room = isForward ? ForwardRoom : BackwardRoom;
            List<Light> lights = room?.Lights;
            if (lights == null) return;

            Vector3 doorPos = transform.position;

            foreach (Light light in lights)
            {
                if (light == null || light.transform == null)
                    continue;

                float distance = Vector3.Distance(doorPos, light.transform.position);
                if (light.range > distance)
                    AddLight(isForward, light, distance);
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (!_initialized || _doorVariant == null)
                return;

            bool current = CanNotSeeThrough;
            if (current == _lastCanNotSeeThrough)
                return;

            _lastCanNotSeeThrough = current;

            ForwardRoom?.ForceRecheckState();
            BackwardRoom?.ForceRecheckState();
        }
    }
}