using System.Diagnostics;
using GameObjectPools;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;

namespace PlayerRoles.PlayableScps.Scp079.Cameras
{
    public class Scp079CameraRotationSync : SubroutineBase, IPoolSpawnable
    {
        private Scp079Role _role;
        private Scp079CurrentCameraSync _curCamSync;
        private Scp079LostSignalHandler _lostSignalHandler;
        private ReferenceHub _owner;

        private readonly Stopwatch _clientSendLimit = Stopwatch.StartNew();

        private const float ClientSendRate = 15f;

        private Scp079Camera CurrentCam => _curCamSync.CurrentCamera;

        private void Update()
        {
            if (_owner == null || !_owner.isLocalPlayer)
                return;

            if (_clientSendLimit.Elapsed.TotalSeconds < 1.0 / ClientSendRate)
                return;

            ClientSendCmd();
            _clientSendLimit.Restart();
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            writer.WriteUShort(CurrentCam.SyncId);
            CurrentCam.WriteAxes(writer);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (CurrentCam.SyncId != reader.ReadUShort())
                return;

            if (_lostSignalHandler.Lost)
                return;

            CurrentCam.ApplyAxes(reader);
            ServerSendRpc(toAll: true);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            writer.WriteUShort(CurrentCam.SyncId);
            CurrentCam.WriteAxes(writer);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            if (!Scp079InteractableBase.TryGetInteractable(
                    reader.ReadUShort(), out Scp079Camera cam))
                return;

            cam.ApplyAxes(reader);
        }

        public void SpawnObject()
        {
            _role = Role as Scp079Role;
            _role.TryGetOwner(out _owner);

            _role.SubroutineModule.TryGetSubroutine(out _curCamSync);
            _role.SubroutineModule.TryGetSubroutine(out _lostSignalHandler);
        }
    }
}
