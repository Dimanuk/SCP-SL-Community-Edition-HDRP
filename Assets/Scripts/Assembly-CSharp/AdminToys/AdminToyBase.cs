using System;
using Footprinting;
using Mirror;
using UnityEngine;

namespace AdminToys
{
    public abstract class AdminToyBase : NetworkBehaviour
    {
        [SyncVar] public Vector3 Position;
        [SyncVar] public LowPrecisionQuaternion Rotation;
        [SyncVar] public Vector3 Scale;
        [SyncVar] public byte MovementSmoothing;
        [SyncVar] public bool IsStatic;

        private const float SmoothingMultiplier = 0.3f;

        public abstract string CommandName { get; }
        public Footprint SpawnerFootprint { get; private set; }

        protected virtual void LateUpdate()
        {
            if (isServer)
                UpdatePositionServer();
            else
                UpdatePositionClient();
        }

        public virtual void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
        {
            SpawnerFootprint = new Footprint(admin);
            NetworkServer.Spawn(gameObject);
            ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{admin.LoggedNameFromRefHub()} spawned an admin toy: {CommandName} with NetID {netId}.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
        }

        private void UpdatePositionServer()
        {
            if (IsStatic) return;

            Position = transform.position;
            Rotation = new LowPrecisionQuaternion(transform.rotation);
            Scale = transform.localScale;
        }

        private void UpdatePositionClient()
        {
            if (IsStatic) return;

            float lerpStep = Time.deltaTime * MovementSmoothing * SmoothingMultiplier;
            if (lerpStep == 0f) lerpStep = 1f;

            transform.SetPositionAndRotation(
                Vector3.Lerp(transform.position, Position, lerpStep),
                Quaternion.Lerp(transform.rotation, Rotation.Value, lerpStep)
            );
            transform.localScale = Vector3.Lerp(transform.localScale, Scale, lerpStep);
        }
    }
}