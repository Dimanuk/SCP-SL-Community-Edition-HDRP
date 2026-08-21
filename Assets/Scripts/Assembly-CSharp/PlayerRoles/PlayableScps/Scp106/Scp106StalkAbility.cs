using GameObjectPools;
using InventorySystem.Items;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106StalkAbility : Scp106VigorAbilityBase, IPoolResettable, IInteractionBlocker
    {
        private const float VigorRegeneration        = 0.03f;
        private const float VigorStalkCostStationary = 0.06f;
        private const float VigorStalkCostMoving     = 0.09f;
        private const float MinVigorToSubmerge       = 0.25f;
        private const float SubmergeTime             = 2.5f;

        private bool _isActive;
        private bool _valueDirty;

        private Scp106SinkholeController _sinkhole;

        protected override ActionName TargetKey => ActionName.Run;

        public override bool IsSubmerged => _isActive;

        public bool IsActive
        {
            get => _isActive;
            private set
            {
                if (_isActive == value)
                    return;

                _isActive = value;
                _valueDirty = true;

                base.Owner.interCoordinator.AddBlocker(this);

                if (value)
                {
                    base.ScpRole.Sinkhole.TargetDuration = SubmergeTime;
                }
            }
        }

        public BlockedInteraction BlockedInteractions => BlockedInteraction.All;

        public bool CanBeCleared => !_isActive;
        protected override void Awake()
        {
            base.Awake();

            _sinkhole = base.ScpRole.Sinkhole;

            PlayerRoleManager.OnRoleChanged += (hub, oldRole, newRole) =>
            {
                if (NetworkServer.active && newRole is SpectatorRole)
                    ServerSendRpc(hub);
            };
        }

        protected override void Update()
        {
            base.Update();

            if (NetworkServer.active)
                UpdateServerside();
        }
        protected override void OnKeyDown()
        {
            base.OnKeyDown();

            if (!_sinkhole.Cooldown.IsReady)
                Scp106Hud.PlayFlash(vigor: false);

            ClientSendCmd();
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (_sinkhole.IsDuringAnimation || !_sinkhole.Cooldown.IsReady || !base.ScpRole.FpcModule.IsGrounded)
                return;

            if (IsActive)
            {
                IsActive = false;
                return;
            }

            if (base.Vigor.VigorAmount < MinVigorToSubmerge)
            {
                if (base.Role.IsLocalPlayer)
                    Scp106Hud.PlayFlash(vigor: true);

                ServerSendRpc(toAll: false);
                return;
            }

            IsActive = true;
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            NetworkWriterExtensions.WriteBool(writer, _isActive);
            _sinkhole.Cooldown.WriteCooldown(writer);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            bool flag = NetworkReaderExtensions.ReadBool(reader);
            _sinkhole.Cooldown.ReadCooldown(reader);

            if (NetworkServer.active)
                return;

            if (flag == _isActive)
                Scp106Hud.PlayFlash(vigor: true);
            else
                IsActive = flag;
        }

        public override void ResetObject()
        {
            base.ResetObject();
            IsActive = false;
        }

        private void UpdateServerside()
        {
            if (_valueDirty)
            {
                _valueDirty = false;
                ServerSendRpc(toAll: true);
            }

            if (_sinkhole.IsDuringAnimation)
                return;

            if (IsActive)
            {
                float cost = base.ScpRole.FpcModule.Motor.MovementDetected
                    ? VigorStalkCostMoving
                    : VigorStalkCostStationary;

                base.Vigor.VigorAmount -= Time.deltaTime * cost;

                if (base.Vigor.VigorAmount <= 0f && base.ScpRole.FpcModule.IsGrounded)
                    IsActive = false;
            }
            else
            {
                if (_sinkhole.Cooldown.IsReady && base.ScpRole.FpcModule.Motor.MovementDetected)
                    base.Vigor.VigorAmount += VigorRegeneration * Time.deltaTime;
            }
        }
    }
}