using System.Collections.Generic;
using System.Diagnostics;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;

namespace MapGeneration.Distributors
{
    public abstract class SpawnablesDistributorBase : MonoBehaviour
    {
        public static readonly HashSet<Rigidbody> BodiesToUnfreeze = new HashSet<Rigidbody>();

        private readonly Dictionary<DoorVariant, HashSet<GameObject>> _unspawnedObjects = new Dictionary<DoorVariant, HashSet<GameObject>>();

        [SerializeField]
        protected SpawnablesDistributorSettings Settings;

        private bool _eventsRegistered;

        private bool _placed;

        private bool _unfrozen;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private void Start()
        {
            if (NetworkServer.active)
            {
                _eventsRegistered = true;
                SeedSynchronizer.OnMapGenerated += _stopwatch.Restart;
                Scp079Camera.OnAnyCameraStateChanged += On079CamChanged;
                DoorEvents.OnDoorAction += OnDoorAction;
                Register();
            }
        }

        private void OnDestroy()
        {
            if (_eventsRegistered)
            {
                SeedSynchronizer.OnMapGenerated -= _stopwatch.Restart;
                Scp079Camera.OnAnyCameraStateChanged -= On079CamChanged;
                DoorEvents.OnDoorAction -= OnDoorAction;
                Unregister();
            }
        }

        private void Update()
        {
            if (!NetworkServer.active || !_stopwatch.IsRunning)
            {
                return;
            }
            float num = (float)_stopwatch.Elapsed.TotalSeconds;
            if (!_placed && num > Settings.SpawnerDelay)
            {
                PlaceSpawnables();
                _placed = true;
            }
            else if (!_unfrozen && num > Settings.UnfreezeDelay)
            {
                foreach (Rigidbody item in BodiesToUnfreeze)
                {
                    if (item != null)
                    {
                        item.isKinematic = false;
                    }
                }
                BodiesToUnfreeze.Clear();
                _unfrozen = true;
            }
            if (_placed && _unfrozen)
            {
                _stopwatch.Stop();
            }
        }

        private void OnDoorAction(DoorVariant door, DoorAction action, ReferenceHub hub)
        {
            if (action == DoorAction.Opened || action == DoorAction.Destroyed)
            {
                SpawnForDoor(door);
            }
        }

        private void On079CamChanged(Scp079Camera cam)
        {
            if (!cam.IsActive || !DoorVariant.DoorsByRoom.TryGetValue(cam.Room, out var value))
            {
                return;
            }
            foreach (DoorVariant item in value)
            {
                SpawnForDoor(item);
            }
        }

        protected abstract void PlaceSpawnables();

        protected virtual void Register()
        {
        }

        protected virtual void Unregister()
        {
        }

        protected void RegisterUnspawnedObject(DoorVariant door, GameObject unspawnedObject)
        {
            if (_unspawnedObjects.TryGetValue(door, out var value) && value != null)
            {
                value.Add(unspawnedObject);
                return;
            }
            _unspawnedObjects[door] = new HashSet<GameObject> { unspawnedObject };
        }

        protected virtual void SpawnObject(GameObject objectToSpawn)
        {
            if (objectToSpawn != null)
            {
                NetworkServer.Spawn(objectToSpawn);
            }
        }

        public void SpawnForDoor(DoorVariant door)
        {
            if (!_unspawnedObjects.TryGetValue(door, out var value) || value == null)
            {
                return;
            }
            foreach (GameObject item in value)
            {
                SpawnObject(item);
            }
            _unspawnedObjects.Remove(door);
        }
    }
}