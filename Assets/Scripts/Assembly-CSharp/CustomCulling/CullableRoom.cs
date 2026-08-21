using Knife.DeferredDecals;
using System.Collections.Generic;
using UnityEngine;

namespace CustomCulling
{
    public class CullableRoom : CullableBase
    {
        internal enum RoomState : byte
        {
            Disabled = 0,
            Enabled = 1,
            LightCrossover = 2,
            AllCrossover = 3,
            DynamicBases = 4,
        }

        internal readonly struct UpdateType
        {
            public bool UpdateRenderers { get; }
            public bool UpdateBehaviours { get; }
            public bool UpdateLights { get; }

            public UpdateType(bool updateRenderers, bool updateBehaviours, bool updateLights)
            {
                UpdateRenderers = updateRenderers;
                UpdateBehaviours = updateBehaviours;
                UpdateLights = updateLights;
            }
        }

        private static readonly UpdateType _disableAllUpdateType = new UpdateType(false, false, false);
        private static readonly UpdateType _enableAllUpdateType = new UpdateType(true, true, true);
        private static readonly UpdateType _skipLightUpdateType = new UpdateType(true, true, false);

        private const RoomState InvalidState = (RoomState)255;
        private const int DisableGraceFrames = 1;

        private RoomState _previousRoomState = InvalidState;
        private int _graceFramesRemaining;

        internal List<DoorLinkingRooms> Doors = new List<DoorLinkingRooms>();
        internal DoorLinkingRooms CrossoverDoor;
        internal bool IsTracked;

        public List<CullableBase> CullableBases { get; private set; }
        internal RoomState CurrentRoomState { get; set; }
        internal UpdateType CurrentUpdateType { get; set; }

        internal override bool CullEnabled => CurrentRoomState != RoomState.Disabled;

        internal bool ReadyToRemove =>
            CurrentRoomState == RoomState.Disabled
            && _previousRoomState == RoomState.Disabled
            && _graceFramesRemaining == 0;

        protected override void OnAwake()
        {
            CullableBases = new List<CullableBase>();
            _graceFramesRemaining = 0;
        }

        internal void ForceRecheckState() => _previousRoomState = InvalidState;

        public void AddDecal(Decal d)
        {
            if (d == null || d.DecalMaterial == null)
                return;

            Decals ??= new List<Decal>();
            Decals.Add(d);
            d.enabled = CurrentUpdateType.UpdateRenderers;
        }

        internal void AddCullableBase(CullableBase cullableBase)
        {
            if (CullableBases == null || cullableBase == null)
                return;

            CullableBases.Add(cullableBase);

            UpdateBehaviours(cullableBase, CurrentUpdateType.UpdateBehaviours);
            UpdateRenderers(cullableBase, CurrentUpdateType.UpdateRenderers);
            UpdateLights(cullableBase, CurrentUpdateType.UpdateLights);

            if (cullableBase.Lights != null)
                foreach (Light light in cullableBase.Lights)
                    if (light != null)
                        AddLightToNearbyDoors(light);

            if (!cullableBase.StaticObject)
            {
                _previousRoomState = InvalidState;

                List<CullableRoom> tracked = CullingCamera.EnabledCullableRooms;
                if (tracked != null && !IsTracked)
                {
                    IsTracked = true;
                    tracked.Add(this);
                }
            }
        }

        internal void RemoveCullableBase(CullableBase cullableBase)
        {
            if (CullableBases == null || cullableBase == null)
                return;

            CullableBases.Remove(cullableBase);

            if (cullableBase.Lights != null)
                foreach (Light light in cullableBase.Lights)
                    if (light != null)
                        RemoveLightFromNearbyDoors(light);
        }

        internal void UpdateNearbyLight(Light light1)
        {
            if (Doors == null || light1 == null)
                return;

            foreach (DoorLinkingRooms door in Doors)
            {
                if (door == null)
                    continue;

                bool isForwardRoom = door.GetForwardRoom() == this;

                float distanceToDoor = Vector3.Distance(
                    light1.transform.position,
                    isForwardRoom ? door.transform.position : door.GetOtherSidePosition());

                door.RemoveLight(light1);

                if (distanceToDoor > light1.range)
                {
                    door.RefreshDistances(isForwardRoom);
                    continue;
                }

                door.AddLight(isForwardRoom, light1, distanceToDoor);
                light1.enabled = (isForwardRoom || CullEnabled) && !CullingManager.AllLightsDisabled;
            }
        }

        internal void AddLightToNearbyDoors(Light light1)
        {
            if (Doors == null || light1 == null)
                return;

            foreach (DoorLinkingRooms door in Doors)
            {
                if (door == null)
                    continue;

                float distance = Vector3.Distance(light1.transform.position, door.transform.position);
                if (distance > light1.range)
                    continue;

                bool isForward = door.GetForwardRoom() == this;
                door.AddLight(isForward, light1, distance);

                light1.enabled = (isForward || CullEnabled) && !CullingManager.AllLightsDisabled;
            }
        }

        internal void RemoveLightFromNearbyDoors(Light light1)
        {
            if (Doors == null || light1 == null)
                return;

            foreach (DoorLinkingRooms door in Doors)
            {
                if (door == null)
                    continue;

                bool isForward = door.GetForwardRoom() == this;
                door.RemoveLight(light1);
                door.RefreshDistances(isForward);
            }
        }

        internal void UpdateAllLighting()
        {
            if (CullableBases == null)
                return;

            foreach (CullableBase cullBase in CullableBases)
                if (cullBase != null)
                    UpdateLights(cullBase, CurrentUpdateType.UpdateLights);

            UpdateLights(this, CurrentUpdateType.UpdateLights);
        }

        internal void UpdateCulling()
        {
            if (CurrentRoomState != RoomState.Disabled)
                _graceFramesRemaining = DisableGraceFrames;

            if (CurrentRoomState == _previousRoomState)
                return;

            if (CurrentRoomState == RoomState.Disabled && _graceFramesRemaining > 0)
            {
                _graceFramesRemaining--;
                return;
            }

            CurrentUpdateType = GetUpdateType(CurrentRoomState);

            UpdateDoors();
            UpdateCullableBases();
            UpdateRoom();
            ApplyCrossoverLighting();

            _previousRoomState = CurrentRoomState;
        }

        private static UpdateType GetUpdateType(RoomState state)
        {
            switch (state)
            {
                case RoomState.Enabled: return _enableAllUpdateType;
                case RoomState.LightCrossover: return _skipLightUpdateType;
                case RoomState.DynamicBases: return _enableAllUpdateType;
                default: return _disableAllUpdateType; 
            }
        }

        private void UpdateDoors()
        {
            if (Doors == null)
                return;

            foreach (DoorLinkingRooms door in Doors)
            {
                if (door == null)
                    continue;

                bool isForward = door.GetForwardRoom() == this;
                CullableRoom other = isForward ? door.GetBackwardRoom() : door.GetForwardRoom();

                door.CullEnabled = CheckRoom(other) || CullEnabled;
                door.UpdateBehaviours();
            }
        }

        private void UpdateRoom()
        {
            UpdateBehaviours(this, CurrentUpdateType.UpdateBehaviours);
            UpdateRenderers(this, CurrentUpdateType.UpdateRenderers);
            UpdateLights(this, CurrentUpdateType.UpdateLights);
        }

        private void UpdateCullableBases()
        {
            if (CullableBases == null)
                return;

            foreach (CullableBase cullBase in CullableBases)
            {
                if (cullBase == null)
                    continue;

                UpdateBehaviours(cullBase, CurrentUpdateType.UpdateBehaviours);
                UpdateRenderers(cullBase, CurrentUpdateType.UpdateRenderers);
                UpdateLights(cullBase, CurrentUpdateType.UpdateLights);
            }
        }

        private void ApplyCrossoverLighting()
        {
            if (Doors == null || CrossoverDoor == null)
                return;

            if (CurrentRoomState != RoomState.LightCrossover && CurrentRoomState != RoomState.AllCrossover)
                return;

            foreach (DoorLinkingRooms door in Doors)
            {
                if (door == null || door != CrossoverDoor)
                    continue;

                door.GetOtherRoom(this)?.UpdateAllLighting();
            }
        }

        private void UpdateBehaviours(CullableBase cullBase, bool isEnabled)
        {
            if (cullBase == null)
                return;

            bool finalEnabled = ResolveEnabled(cullBase, isEnabled);

            if (cullBase.OtherBehaviours != null)
                foreach (Behaviour b in cullBase.OtherBehaviours)
                    if (b != null) b.enabled = finalEnabled;

            if (cullBase.Decals != null)
                foreach (Decal d in cullBase.Decals)
                    if (d != null) d.enabled = finalEnabled;
        }

        private void UpdateRenderers(CullableBase cullBase, bool isEnabled)
        {
            if (cullBase == null)
                return;

            bool finalEnabled = ResolveEnabled(cullBase, isEnabled);

            if (cullBase.Renderers != null)
                foreach (Renderer r in cullBase.Renderers)
                    if (r != null) r.enabled = finalEnabled;
        }

        private void UpdateLights(CullableBase cullBase, bool isEnabled)
        {
            if (cullBase == null)
                return;

            bool lightEnabled = ResolveEnabled(cullBase, isEnabled) && !CullingManager.AllLightsDisabled;

            if (cullBase.Lights != null)
                foreach (Light l in cullBase.Lights)
                    if (l != null) l.enabled = lightEnabled;
        }

        private bool ResolveEnabled(CullableBase cullBase, bool isEnabled)
        {
            if (CurrentRoomState == RoomState.Disabled)
                return false;

            if (CurrentRoomState == RoomState.DynamicBases)
                return !cullBase.StaticObject;

            if (!cullBase.StaticObject)
                return true;

            return isEnabled;
        }

        private static bool CheckRoom(CullableRoom room) => room != null && room.CullEnabled;
    }
}