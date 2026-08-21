using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Subroutines;
using RelativePositioning;
using UnityEngine;
using Utils.Networking;

namespace PlayerRoles.PlayableScps.Scp173
{
    public class Scp173SnapAbility : ScpKeySubroutine<Scp173Role>
    {
        private const float SnapRayDistance = 1.5f;
        private const float TargetBacktrackingTime = 0.4f;
        private const float OwnerBacktrackingTime = 0.1f;
        private const float ForetrackingTime = 0.2f;

        private Scp173ObserversTracker _observersTracker;
        private ReferenceHub _targetHub;

        private static int _snapMask;

        private static int SnapMask
        {
            get
            {
                if (_snapMask == 0)
                {
                    _snapMask = LayerMask.GetMask(
                        "Default",
                        "Hitbox",
                        "Glass",
                        "Door",
                        "BreakableGlass",
                        "Locker"
                    );
                }
                return _snapMask;
            }
        }

        public bool IsSpeeding
        {
            get
            {
                if (base.ScpRole.SubroutineModule.TryGetSubroutine(out Scp173BreakneckSpeedsAbility subroutine))
                    return subroutine.IsActive;
                return false;
            }
        }

        protected override ActionName TargetKey => ActionName.Shoot;

        protected override void OnKeyDown()
        {
            base.OnKeyDown();

            if (IsSpeeding)
                return;

            if (TryHitTarget(base.Owner.PlayerCameraReference, out _targetHub))
                ClientSendCmd();
        }

        private static bool TryHitTarget(Transform origin, out ReferenceHub target)
        {
            target = null;

            if (!Physics.Raycast(origin.position, origin.forward, out RaycastHit hitInfo, SnapRayDistance, SnapMask))
                return false;

            if (!hitInfo.collider.TryGetComponent(out IDestructible component))
                return false;

            if (component is not HitboxIdentity hitboxIdentity)
                return false;

            if (hitboxIdentity.TargetHub.roleManager.CurrentRole is not HumanRole)
                return false;

            target = hitboxIdentity.TargetHub;
            return true;
        }
        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _observersTracker);
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);

            ReferenceHubReaderWriter.WriteReferenceHub(writer, _targetHub);

            RelativePositionSerialization.WriteRelativePosition(
                writer,
                new RelativePosition(_targetHub.transform.position)
            );

            RelativePositionSerialization.WriteRelativePosition(
                writer,
                new RelativePosition(base.Owner.transform.position)
            );

            writer.WriteLowPrecisionQuaternion(
                new LowPrecisionQuaternion(base.Owner.PlayerCameraReference.rotation)
            );
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            _targetHub = ReferenceHubReaderWriter.ReadReferenceHub(reader);

            if (_observersTracker.IsObserved)
                return;

            if (_targetHub == null)
                return;

            if (_targetHub.roleManager.CurrentRole is not IFpcRole fpcRole)
                return;

            if (IsSpeeding)
                return;


            FirstPersonMovementModule fpcModule = base.ScpRole.FpcModule;
            FirstPersonMovementModule targetFpcModule = fpcRole.FpcModule;
            Transform playerCameraReference = base.Owner.PlayerCameraReference;

            Vector3 originalTargetPosition = targetFpcModule.Position;
            Vector3 originalOwnerPosition = fpcModule.Position;
            Quaternion originalRotation = playerCameraReference.rotation;

            targetFpcModule.Position = targetFpcModule.Tracer
                .GenerateBounds(TargetBacktrackingTime, ignoreTeleports: true)
                .ClosestPoint(RelativePositionSerialization.ReadRelativePosition(reader).Position);

            Bounds ownerBounds = fpcModule.Tracer.GenerateBounds(OwnerBacktrackingTime, ignoreTeleports: true);
            ownerBounds.Encapsulate(fpcModule.Position + fpcModule.Motor.Velocity * ForetrackingTime);
            fpcModule.Position = ownerBounds.ClosestPoint(RelativePositionSerialization.ReadRelativePosition(reader).Position);

            playerCameraReference.rotation = reader.ReadLowPrecisionQuaternion().Value;

            // Listen-server only: the host player's own hitbox colliders are disabled
            // (CharacterModel.SpawnObject -> SetColliders(!isLocalPlayer)), so the server-side
            // TryHitTarget raycast could never hit the host. Briefly re-enable them, same
            // pattern as ExplosionGrenade.Explode / ScpAttackAbilityBase.ServerProcessCmd.
            ReferenceHub.TryGetHostHub(out ReferenceHub hostHub);
            bool restoreHostHitboxes = !base.Owner.isLocalPlayer && HitboxIdentity.SetOwnHitboxes(hostHub, true);

            bool targetHit = TryHitTarget(playerCameraReference, out ReferenceHub target);

            if (restoreHostHitboxes)
            {
                HitboxIdentity.SetOwnHitboxes(hostHub, false);
            }

            if (targetHit)
            {
                if (target.playerStats.DealDamage(base.ScpRole.DamageHandler))
                {
                    Hitmarker.SendHitmarker(base.Owner, 1f);

                    if (base.ScpRole.SubroutineModule.TryGetSubroutine(out Scp173AudioPlayer audioPlayer))
                    {
                        audioPlayer.ServerSendSound(Scp173AudioPlayer.Scp173SoundId.Snap);
                    }
                }
            }

            targetFpcModule.Position = originalTargetPosition;
            fpcModule.Position = originalOwnerPosition;
            playerCameraReference.rotation = originalRotation;
        }
    }
}