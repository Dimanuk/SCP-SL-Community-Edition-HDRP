using Interactables;
using Interactables.Interobjects;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerStatsSystem;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp096
{
    public class Scp096PrygateAbility : ScpStandardSubroutine<Scp096Role>
    {
        private PryableDoor _syncDoor;
        private Scp096HitHandler _hitHandler;
        private Scp096AudioPlayer _audioPlayer;

        private const float DoorKillerHeight = 1.5f;
        private const float DoorKillerRadius = 0.2f;
        private const float MaxDisSqr = 8.12f;
        private const float HumanDamage = 200f;

        public void ClientTryPry(PryableDoor door)
        {
            _syncDoor = door;
            ClientSendCmd();
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            _hitHandler = new Scp096HitHandler(
                base.ScpRole, 
                Scp096DamageHandler.AttackType.GateKill, 
                0f, 
                0f, 
                HumanDamage, 
                HumanDamage);
        }

        public override void ResetObject()
        {
            base.ResetObject();
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            NetworkWriterExtensions.WriteNetworkBehaviour(writer, _syncDoor);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);
            

            if (base.ScpRole.StateController.RageState != Scp096RageState.Enraged 
                || base.ScpRole.StateController.AbilityState == Scp096AbilityState.PryingGate)
            {
                return;
            }

            _syncDoor = NetworkReaderExtensions.ReadNetworkBehaviour<PryableDoor>(reader);
            
            if (_syncDoor == null 
                || _syncDoor.TargetState 
                || _syncDoor.GetExactState() > 0f 
                || (_syncDoor.transform.position - base.ScpRole.FpcModule.Position).sqrMagnitude > MaxDisSqr)
            {
                return;
            }

            base.Role.TryGetOwner(out var hub);
            
            if (_syncDoor.TryPryGate(hub))
            {
                _hitHandler.Clear();
                ServerSendRpc(toAll: true);
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            NetworkWriterExtensions.WriteNetworkBehaviour(writer, _syncDoor);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            _syncDoor = NetworkReaderExtensions.ReadNetworkBehaviour<PryableDoor>(reader);
            
            if (NetworkServer.active || base.Owner.isLocalPlayer)
            {
                (base.ScpRole.FpcModule as Scp096MovementModule).SetTargetGate(_syncDoor);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _audioPlayer);
        }

        private void Update()
        {
            if (!NetworkServer.active 
                || !base.ScpRole.IsAbilityState(Scp096AbilityState.PryingGate) 
                || _syncDoor == null)
            {
                return;
            }

            Vector3 position = _syncDoor.transform.position + Vector3.up * DoorKillerHeight;
            Scp096HitResult scp096HitResult = _hitHandler.DamageSphere(position, DoorKillerRadius);
            
            if (scp096HitResult != Scp096HitResult.None)
            {
                _audioPlayer.ServerPlayAttack(scp096HitResult);
                Hitmarker.SendHitmarker(base.Owner, 1f);
            }
        }

        private void OnInteracted(InteractableCollider col)
        {
            if (!base.Role.IsLocalPlayer)
            {
                return;
            }

            if (col.Target is PryableDoor pryableDoor)
            {
                _syncDoor = pryableDoor;
                ClientSendCmd();
            }
        }
    }
}