using System;
using CustomPlayerEffects;
using InventorySystem.Items.MicroHID;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using RelativePositioning;
using UnityEngine;
using Utils.Networking;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106Attack : Scp106VigorAbilityBase
    {
        private const float TargetTraceTime = 0.35f;
        private const float VigorCaptureReward = 0.3f;
        private const float CooldownReductionReward = 5f;
        private const float TraumatizedDuration = 180f;

        private ReferenceHub _targetHub;
        private Quaternion _ownerCamRotation;
        private Vector3 _ownerPosition;
        private Vector3 _targetPosition;
        private double _nextAttack;

        [SerializeField]
        private AnimationCurve _dotOverDistance;

        [SerializeField]
        private float _maxRangeSqr;

        [SerializeField]
        private float _hitCooldown;

        [SerializeField]
        private float _missCooldown;

        [SerializeField]
        private int _damage;

        private Transform OwnerCam => Owner.PlayerCameraReference;

        protected override ActionName TargetKey => ActionName.Shoot;

        public static event Action<ReferenceHub> OnPlayerTeleported;

        protected override void OnKeyDown()
        {
            base.OnKeyDown();

            float bestDot = -1f;
            _ownerPosition = ScpRole.FpcModule.Position;
            _ownerCamRotation = OwnerCam.rotation;

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (!(hub.roleManager.CurrentRole is HumanRole humanRole))
                    continue;

                Vector3 position = humanRole.FpcModule.Position;
                Vector3 diff = position - _ownerPosition;

                if (diff.sqrMagnitude > _maxRangeSqr)
                    continue;

                float dot = Vector3.Dot(diff.normalized, OwnerCam.forward);
                if (dot < bestDot)
                    continue;

                _targetPosition = position;
                _targetHub = hub;
                bestDot = dot;
            }

            if (bestDot != -1f)
                ClientSendCmd();
        }

        private void SendCooldown(float cooldown)
        {
            if (cooldown <= 0f)
                return;

            _nextAttack = NetworkTime.time + cooldown;
            ServerSendRpc(x => x == Owner || SpectatorNetworking.IsSpectatedBy(Owner, x));
        }

        private void ReduceSinkholeCooldown()
        {
            ScpRole.Sinkhole.Cooldown.NextUse -= CooldownReductionReward;
        }

        private void ServerShoot()
        {
            using (new FpcBacktracker(_targetHub, _targetPosition, TargetTraceTime))
            {
                Vector3 diff = _targetPosition - _ownerPosition;
                float sqrMag = diff.sqrMagnitude;

                if (sqrMag > _maxRangeSqr)
                    return;

                Vector3 forward = OwnerCam.forward;
                forward.y = 0f;
                diff.y = 0f;

                if (Physics.Linecast(_ownerPosition, _targetPosition, MicroHIDItem.WallMask))
                    return;

                if (_dotOverDistance.Evaluate(sqrMag) > Vector3.Dot(diff.normalized, forward.normalized))
                {
                    SendCooldown(_missCooldown);
                    return;
                }

                var handler = new ScpDamageHandler(Owner, _damage, DeathTranslations.PocketDecay);
                if (!_targetHub.playerStats.DealDamage(handler))
                    return;
            }

            SendCooldown(_hitCooldown);
            Vigor.VigorAmount += VigorCaptureReward;
            ReduceSinkholeCooldown();
            Hitmarker.SendHitmarker(Owner, 1f);
            OnPlayerTeleported?.Invoke(_targetHub);

            PlayerEffectsController fx = _targetHub.playerEffectsController;
            fx.EnableEffect<Traumatized>(TraumatizedDuration);
            fx.EnableEffect<Corroding>();
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            ReferenceHubReaderWriter.WriteReferenceHub(writer, _targetHub);
            RelativePositionSerialization.WriteRelativePosition(writer, new RelativePosition(_targetPosition));
            NetworkWriterExtensions.WriteQuaternion(writer, _ownerCamRotation);
            RelativePositionSerialization.WriteRelativePosition(writer, new RelativePosition(_ownerPosition));
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (_nextAttack > NetworkTime.time || ScpRole.Sinkhole.NormalizedState > 0f)
                return;

            _targetHub = ReferenceHubReaderWriter.ReadReferenceHub(reader);
            _targetPosition = RelativePositionSerialization.ReadRelativePosition(reader).Position;
            _ownerCamRotation = NetworkReaderExtensions.ReadQuaternion(reader);
            _ownerPosition = RelativePositionSerialization.ReadRelativePosition(reader).Position;

            if (_targetHub == null || !_targetHub.IsHuman())
                return;

            using (new FpcBacktracker(Owner, _ownerPosition, _ownerCamRotation))
            {
                ServerShoot();
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            NetworkWriterExtensions.WriteDouble(writer, _nextAttack);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            ReduceSinkholeCooldown();
            Scp106Hud.PlayCooldownAnimation(NetworkReaderExtensions.ReadDouble(reader));
        }

    }
}