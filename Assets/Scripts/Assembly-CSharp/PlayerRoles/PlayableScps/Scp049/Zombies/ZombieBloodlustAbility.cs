using System.Diagnostics;
using GameObjectPools;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{
    public class ZombieBloodlustAbility : SubroutineBase, IPoolResettable
    {
        [SerializeField]
        private float _maxViewDistance;

        private float _simulatedStareTime;

        private readonly Stopwatch _simulatedStareSw;

        private bool _lastSentLookingAtTarget;

        public bool LookingAtTarget { get; private set; }

        public float SimulatedStare
        {
            get => Mathf.Max(0f, _simulatedStareTime - (float)_simulatedStareSw.Elapsed.TotalSeconds);
            set
            {
                _simulatedStareTime = value;
                _simulatedStareSw.Restart();
            }
        }

        private void Update() => RefreshChaseState();

        public void RefreshChaseState()
        {
            if (!NetworkServer.active)
                return;

            if (!base.Role.TryGetOwner(out var hub))
                return;

            LookingAtTarget = SimulatedStare > 0f || AnyTargets(hub, hub.PlayerCameraReference);

            // v12 broadcast a reliable RPC to every ready client each server frame
            // regardless of whether the value changed, so the load scaled as
            // zombies * clients * server FPS. Only the owner meaningfully consumes
            // LookingAtTarget (its own client-side movement prediction) and it is
            // always ready, so sending solely on change is behaviourally identical
            // while dropping the per-frame reliable-message spam. Both server and
            // client start at false, so the gated state stays in sync from spawn.
            if (LookingAtTarget != _lastSentLookingAtTarget)
            {
                _lastSentLookingAtTarget = LookingAtTarget;
                ServerSendRpc(toAll: true);
            }
        }

        private bool AnyTargets(ReferenceHub owner, Transform camera)
        {
            foreach (ReferenceHub target in ReferenceHub.AllHubs)
            {
                if (!target.IsHuman())
                    continue;

                if (target.playerEffectsController.GetEffect<CustomPlayerEffects.Invisible>()?.IsEnabled == true)
                    continue;

                if (target.roleManager.CurrentRole is not FirstPersonControl.IFpcRole fpcRole)
                    continue;

                var vision = VisionInformation.GetVisionInformation(
                    owner, 
                    camera, 
                    fpcRole.FpcModule.Position, 
                    fpcRole.FpcModule.CharacterControllerSettings.Radius, 
                    _maxViewDistance
                );

                if (vision.IsLooking)
                    return true;
            }

            return false;
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            NetworkWriterExtensions.WriteBool(writer, LookingAtTarget);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);
            LookingAtTarget = NetworkReaderExtensions.ReadBool(reader);
        }
        public void ResetObject()
        {
            _simulatedStareTime = 0f;
            LookingAtTarget = false;
            _lastSentLookingAtTarget = false;
        }
        public ZombieBloodlustAbility()
        {
            _simulatedStareSw = Stopwatch.StartNew();
        }
    }
}
