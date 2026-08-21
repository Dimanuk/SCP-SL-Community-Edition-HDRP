using System;
using System.Collections.Generic;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using UnityEngine;
using Utils.Networking;

namespace PlayerRoles.PlayableScps.Scp049
{
    public class Scp049SenseAbility : ScpKeySubroutine<Scp049Role>
    {
        private const float BaseCooldown = 40f;
        private const float ReducedCooldown = 20f;
        private const float AttemptFailCooldown = 5f;
        private const float EffectDuration = 20f;
        private const float HeightDiffIgnoreY = 0.1f;
        private const float NearbyDistanceSqr = 4.5f;

        public readonly AbilityCooldown Cooldown = new AbilityCooldown();
        public readonly AbilityCooldown Duration = new AbilityCooldown();
        public readonly HashSet<ReferenceHub> DeadTargets = new HashSet<ReferenceHub>();

        [SerializeField]
        private GameObject _effectPrefab;

        [SerializeField]
        private Scp049AudioPlayer _audio;

        [SerializeField]
        private float _dotThreshold = 0.88f;

        [SerializeField]
        private float _distanceThreshold = 100f;

        private Scp049AttackAbility _attackAbility;
        private Transform _pulseEffect;
        private bool _hasPulseUnsafe;

        public ReferenceHub Target { get; private set; }
        public bool HasTarget { get; private set; }
        public float DistanceFromTarget { get; private set; }

        protected override ActionName TargetKey => ActionName.ToggleFlashlight;

        public event Action OnFailed;

        public void ServerLoseTarget()
        {
            HasTarget = false;
            Cooldown.Trigger(ReducedCooldown);
            ServerSendRpc(true);
        }

        public void ServerProcessKilledPlayer(ReferenceHub hub)
        {
            if (HasTarget && Target == hub)
            {
                DeadTargets.Add(hub);
                Cooldown.Trigger(BaseCooldown);
                HasTarget = false;
                ServerSendRpc(true);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!HasTarget)
                return;

            bool targetIsHuman = false;
            if (Target.roleManager.CurrentRole is HumanRole humanRole)
            {
                targetIsHuman = true;
                Vector3 targetPos = humanRole.FpcModule.Position;
                Vector3 myPos = ScpRole.FpcModule.Position;
                DistanceFromTarget = (targetPos - myPos).sqrMagnitude;
            }

            if (NetworkServer.active && !(ScpRole.VisibilityController.ValidateVisibility(Target) && !Duration.IsReady && targetIsHuman))
            {
                ServerLoseTarget();
            }
        }

        protected override void OnKeyDown()
        {
            base.OnKeyDown();

            if (!Duration.IsReady || !Cooldown.IsReady)
                return;

            if (!CanFindTarget(out var bestTarget))
            {
                Target = null;
                OnFailed?.Invoke();
                ClientSendCmd();
            }
            else
            {
                Target = bestTarget;
                ClientSendCmd();
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();

            GetSubroutine<Scp049AttackAbility>(out _attackAbility);

            PlayerRoleManager.OnRoleChanged += OnRoleChanged;
            SpectatorTargetTracker.OnTargetChanged += OnSpectatorTargetChanged;
            _attackAbility.OnServerHit += OnServerHit;
        }

        public override void ResetObject()
        {
            base.ResetObject();

            Cooldown.Clear();
            Duration.Clear();
            DeadTargets.Clear();
            HasTarget = false;

            PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
            SpectatorTargetTracker.OnTargetChanged -= OnSpectatorTargetChanged;
            _attackAbility.OnServerHit -= OnServerHit;
        }

        private void OnServerHit(ReferenceHub hub)
        {
            if (HasTarget && hub != Target)
            {
                ServerLoseTarget();
            }
        }

        private void OnSpectatorTargetChanged()
        {
            if (_hasPulseUnsafe && _pulseEffect != null)
            {
                Destroy(_pulseEffect.gameObject);
                _hasPulseUnsafe = false;
            }
        }

        private void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (NetworkServer.active && (newRole is HumanRole || newRole is ZombieRole))
            {
                DeadTargets.Remove(userHub);
            }
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            if (!Cooldown.IsReady || !Duration.IsReady)
                return;

            HasTarget = false;
            Target = ReferenceHubReaderWriter.ReadReferenceHub(reader);

            if (Target == null)
            {
                Cooldown.Trigger(AttemptFailCooldown);
                ServerSendRpc(true);
                return;
            }

            if (Target.roleManager.CurrentRole is HumanRole humanRole)
            {
                float radius = humanRole.FpcModule.CharController.radius;
                Vector3 cameraPosition = humanRole.CameraPosition;

                if (VisionInformation.GetVisionInformation(Owner, Owner.PlayerCameraReference, cameraPosition, radius, _distanceThreshold).IsLooking)
                {
                    Duration.Trigger(EffectDuration);
                    HasTarget = true;
                    ServerSendRpc(true);
                }
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            ReferenceHubReaderWriter.WriteReferenceHub(writer, HasTarget ? Target : null);
            Cooldown.WriteCooldown(writer);
            Duration.WriteCooldown(writer);
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            ReferenceHubReaderWriter.WriteReferenceHub(writer, Target);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            Target = ReferenceHubReaderWriter.ReadReferenceHub(reader);
            HasTarget = Target != null;

            if (_hasPulseUnsafe && _pulseEffect != null)
            {
                Destroy(_pulseEffect.gameObject);
                _hasPulseUnsafe = false;
            }

            if (HasTarget && (Owner.isLocalPlayer || SpectatorNetworking.IsLocallySpectated(Owner)))
            {
                _pulseEffect = Instantiate(_effectPrefab, Target.transform).transform;
                _hasPulseUnsafe = true;
                Destroy(_pulseEffect.gameObject, 3.5f);
            }

            Cooldown.ReadCooldown(reader);
            Duration.ReadCooldown(reader);
        }

        private bool CanFindTarget(out ReferenceHub bestTarget)
        {
            Transform cam = Owner.PlayerCameraReference;
            float bestDistSqr = _distanceThreshold * _distanceThreshold;
            float bestDot = _dotThreshold;
            bool found = false;
            bestTarget = null;

            Vector3 myPos = ScpRole.FpcModule.Position;

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (!(hub.roleManager.CurrentRole is HumanRole humanRole))
                    continue;

                Vector3 targetPos = humanRole.FpcModule.Position;
                Vector3 toTarget = targetPos - cam.position;
                Vector3 forward = cam.forward;

                if (Mathf.Abs((targetPos - myPos).y) < HeightDiffIgnoreY && toTarget.sqrMagnitude < NearbyDistanceSqr)
                {
                    forward.y = 0f;
                    forward.Normalize();
                    toTarget.y = 0f;
                }

                float dot = Vector3.Dot(forward, toTarget.normalized);
                if (dot < bestDot)
                    continue;

                float distSqr = (targetPos - myPos).sqrMagnitude;
                if (distSqr > bestDistSqr)
                    continue;

                float radius = humanRole.FpcModule.CharacterControllerSettings.Radius;
                if (VisionInformation.GetVisionInformation(Owner, cam, humanRole.CameraPosition, radius, _distanceThreshold).IsLooking)
                {
                    bestDistSqr = distSqr;
                    bestTarget = hub;
                    bestDot = dot;
                    found = true;
                }
            }

            return found;
        }
    }
}