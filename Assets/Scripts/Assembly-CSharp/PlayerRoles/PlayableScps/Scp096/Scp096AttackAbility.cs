using System;
using System.Collections.Generic;
using GameObjectPools;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using RelativePositioning;
using UnityEngine;
using Utils.Networking;

namespace PlayerRoles.PlayableScps.Scp096
{
    public class Scp096AttackAbility : ScpKeySubroutine<Scp096Role>, IPoolResettable
    {
        public const float DefaultAttackCooldown = 0.5f;

        private const float HumanDamage = 85f;
        private const float DoorDamage = 250f;
        private const int WindowDamage = 500;
        private const float BacktrackingDisSqr = 9f;
        private const byte LeftAttackSyncCode = 64;

        [SerializeField]
        private float _sphereHitboxRadius;

        [SerializeField]
        private float _sphereHitboxOffset;

        private static readonly List<FpcBacktracker> BacktrackedPlayers = new List<FpcBacktracker>();
        private static readonly List<ReferenceHub> PlayersToSend = new List<ReferenceHub>();

        private readonly AbilityCooldown _clientAttackCooldown = new AbilityCooldown();
        private readonly TolerantAbilityCooldown _serverAttackCooldown = new TolerantAbilityCooldown(0.2f);

        private Scp096HitHandler _leftHitHandler;
        private Scp096HitHandler _rightHitHandler;
        private Scp096AudioPlayer _audioPlayer;
        private Scp096HitResult _hitResult;

        private bool AttackPossible
        {
            get
            {
                if (base.ScpRole.IsRageState(Scp096RageState.Enraged))
                {
                    return base.ScpRole.IsAbilityState(Scp096AbilityState.None);
                }
                return false;
            }
        }

        protected override ActionName TargetKey => ActionName.Shoot;

        public bool LeftAttack { get; private set; }

        public event Action<Scp096HitResult> OnHitReceived;
        public event Action OnAttackTriggered;

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            byte b = (byte)_hitResult;
            if (LeftAttack)
            {
                b |= LeftAttackSyncCode;
            }
            writer.WriteByte(b);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            byte b = reader.ReadByte();
            LeftAttack = (b & LeftAttackSyncCode) != 0;
            Scp096HitResult scp096HitResult = (Scp096HitResult)(b & ~LeftAttackSyncCode);
            OnHitReceived?.Invoke(scp096HitResult);
            if (scp096HitResult != Scp096HitResult.None && (base.Owner.isLocalPlayer || SpectatorNetworking.IsLocallySpectated(base.Owner)))
            {
                Hitmarker.PlayHitmarker(1f);
            }
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            RelativePositionSerialization.WriteRelativePosition(writer, new RelativePosition(base.ScpRole.FpcModule.Position));
            NetworkWriterExtensions.WriteQuaternion(writer, base.Owner.PlayerCameraReference.rotation);
            foreach (ReferenceHub item in PlayersToSend)
            {
                Vector3 position = (item.roleManager.CurrentRole as HumanRole).FpcModule.Position;
                ReferenceHubReaderWriter.WriteReferenceHub(writer, item);
                RelativePositionSerialization.WriteRelativePosition(writer, new RelativePosition(position));
            }
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);
            if (!_serverAttackCooldown.TolerantIsReady || !AttackPossible)
            {
                return;
            }
            BacktrackedPlayers.Clear();
            RelativePosition relativePosition = RelativePositionSerialization.ReadRelativePosition(reader);
            BacktrackedPlayers.Add(new FpcBacktracker(base.Owner, relativePosition.Position, NetworkReaderExtensions.ReadQuaternion(reader)));
            while (reader.Position < reader.Capacity)
            {
                bool flag = ReferenceHubReaderWriter.TryReadReferenceHub(reader, out ReferenceHub hub);
                Vector3 position = RelativePositionSerialization.ReadRelativePosition(reader).Position;
                if (flag)
                {
                    BacktrackedPlayers.Add(new FpcBacktracker(hub, position));
                }
            }
            ServerAttack();
            BacktrackedPlayers.ForEach(x => x.RestorePosition());
            _serverAttackCooldown.Trigger(DefaultAttackCooldown);
        }

        private void ServerAttack()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            LeftAttack = !LeftAttack;
            base.ScpRole.StateController.SetAbilityState(Scp096AbilityState.Attacking);
            Transform playerCameraReference = base.Owner.PlayerCameraReference;
            Scp096HitHandler scp096HitHandler = LeftAttack ? _leftHitHandler : _rightHitHandler;
            scp096HitHandler.Clear();
            _hitResult = scp096HitHandler.DamageSphere(playerCameraReference.position + playerCameraReference.forward * _sphereHitboxOffset, _sphereHitboxRadius);
            _audioPlayer.ServerPlayAttack(_hitResult);
            ServerSendRpc(toAll: true);
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _clientAttackCooldown.Clear();
            _serverAttackCooldown.Clear();
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            _leftHitHandler = new Scp096HitHandler(base.ScpRole, Scp096DamageHandler.AttackType.SlapLeft, WindowDamage, DoorDamage, HumanDamage, 0f);
            _rightHitHandler = new Scp096HitHandler(base.ScpRole, Scp096DamageHandler.AttackType.SlapRight, WindowDamage, DoorDamage, HumanDamage, 0f);
        }

        protected override void Update()
        {
            base.Update();
            if (NetworkServer.active && _serverAttackCooldown.IsReady && base.ScpRole.IsAbilityState(Scp096AbilityState.Attacking))
            {
                base.ScpRole.ResetAbilityState();
            }
        }

        protected override void OnKeyDown()
        {
            base.OnKeyDown();
            if (!AttackPossible || !_clientAttackCooldown.IsReady)
            {
                return;
            }
            PlayersToSend.Clear();
            Vector3 position = base.ScpRole.FpcModule.Position;
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.roleManager.CurrentRole is HumanRole humanRole && (humanRole.FpcModule.Position - position).sqrMagnitude <= BacktrackingDisSqr)
                {
                    PlayersToSend.Add(allHub);
                }
            }
            ClientSendCmd();
            _clientAttackCooldown.Trigger(DefaultAttackCooldown);
            OnAttackTriggered?.Invoke();
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine<Scp096AudioPlayer>(out _audioPlayer);
        }
    }
}