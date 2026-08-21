using GameObjectPools;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106Vigor : SubroutineBase, IPoolSpawnable
    {
        private bool _force;
        private int _lastSentAmount;
        private float _vigor;

        private const float SyncAccuracy = 120f;
        private const float AbsMoveSpeed = 0.03f;
        private const float LerpMoveSpeed = 2.7f;
        private const float StartAmount = 0f;

        public float VigorAmount
        {
            get => Mathf.Clamp01(_vigor);
            set => _vigor = value;
        }

        public float DisplayedVigor { get; private set; }

        private void Update()
        {
            if (NetworkServer.active)
                UpdateServerside();

            UpdateClientside();
        }

        private void UpdateClientside()
        {
            if (_force)
            {
                _force = false;
                DisplayedVigor = VigorAmount;
                return;
            }

            DisplayedVigor = Mathf.MoveTowards(DisplayedVigor, VigorAmount, Time.deltaTime * AbsMoveSpeed);

            DisplayedVigor = Mathf.Lerp(DisplayedVigor, VigorAmount, Time.deltaTime * LerpMoveSpeed);
        }

        private void UpdateServerside()
        {
            if (!Role.TryGetOwner(out ReferenceHub owner))
                return;

            int current = Mathf.FloorToInt(VigorAmount * SyncAccuracy);

            if (current != _lastSentAmount)
            {
                _lastSentAmount = current;

                ServerSendRpc(x => x == owner || x.roleManager.CurrentRole is SpectatorRole);
            }
            else
            {
                _force = false;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            PlayerRoleManager.OnRoleChanged += (hub, prev, cur) =>
            {
                if (NetworkServer.active && cur is SpectatorRole)
                    ServerSendRpc(hub);
            };
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);

            int payload = _lastSentAmount + 1;
            sbyte signed = (sbyte)(_force ? -payload : payload);

            NetworkWriterExtensions.WriteSByte(writer, signed);
            _force = false;
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            if (NetworkServer.active)
                return;

            sbyte received = NetworkReaderExtensions.ReadSByte(reader);
            _force = received < 0;

            VigorAmount = Mathf.Abs(received) / SyncAccuracy;
        }

        public void SpawnObject()
        {
            _lastSentAmount = -1;
            _vigor = StartAmount;
            DisplayedVigor = StartAmount;
            _force = true;
        }
    }
}