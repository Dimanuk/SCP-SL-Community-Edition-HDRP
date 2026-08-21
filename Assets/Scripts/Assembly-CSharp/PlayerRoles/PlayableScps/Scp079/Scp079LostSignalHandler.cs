using System;
using GameObjectPools;
using InventorySystem.Items.ThrowableProjectiles;
using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079LostSignalHandler : SubroutineBase, IPoolSpawnable
    {
        [SerializeField]
        private float _ghostlightLockoutDuration;

        private Scp079CurrentCameraSync _curCamSync;
        private Scp079AuxManager _auxManager;
        private double _recoveryTime;
        private bool _prevLost;

        public bool Lost => _recoveryTime > NetworkTime.time;

        public float RemainingTime => Mathf.Max(0f, (float)(_recoveryTime - NetworkTime.time));

        public event Action OnStatusChanged;

        private void Update()
        {
            bool lost = Lost;

            if (NetworkServer.active && lost)
                _auxManager.CurrentAux = 0f;

            if (_prevLost != lost)
            {
                _prevLost = lost;
                OnStatusChanged?.Invoke();

                if (_curCamSync.TryGetCurrentCamera(out var cam))
                    cam.IsActive = !lost;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (base.Role is ISubroutinedScpRole subroutinedRole)
            {
                subroutinedRole.SubroutineModule.TryGetSubroutine(out _curCamSync);
                subroutinedRole.SubroutineModule.TryGetSubroutine(out _auxManager);
            }

            Scp2176Projectile.OnServerShattered += rid =>
            {
                if (!NetworkServer.active || base.Role.Pooled)
                    return;

                if (rid != _curCamSync.CurrentCamera.Room)
                    return;

                if (!base.Role.TryGetOwner(out var hub))
                    return;

                if (hub.characterClassManager.GodMode)
                    return;

                ServerLoseSignal(Lost ? 0f : _ghostlightLockoutDuration);
            };
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            NetworkWriterExtensions.WriteDouble(writer, _recoveryTime);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            _recoveryTime = NetworkReaderExtensions.ReadDouble(reader);
        }

        public void ServerLoseSignal(float duration)
        {
            _recoveryTime = NetworkTime.time + (double)duration;
            ServerSendRpc(toAll: true);
        }

        public void SpawnObject()
        {
            _recoveryTime = 0.0;
            _prevLost = false;
        }
    }
}