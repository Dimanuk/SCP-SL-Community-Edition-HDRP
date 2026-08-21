using System;
using System.Runtime.CompilerServices;
using CustomPlayerEffects;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerStatsSystem;
using UnityEngine;
using Utils.Networking;

namespace PlayerRoles.PlayableScps.Scp049
{
    public class Scp049AttackAbility : ScpKeySubroutine<Scp049Role>
    {
        private const float CooldownTime = 1.5f;
        private const float LagBacktrackingCompensation = 0.4f;
        private const float AttackDistance = 1.728f;

        private static int _attackLayerMask;

        [SerializeField]
        private float _statusEffectDuration = 20f;

        private ReferenceHub _target;
        private Scp049ResurrectAbility _resurrect;

        public readonly AbilityCooldown Cooldown = new AbilityCooldown();

        internal static LayerMask AttackMask
        {
            get
            {
                if (_attackLayerMask == 0)
                {
                    _attackLayerMask = LayerMask.GetMask("Hitbox") | (int)FpcStateProcessor.Mask;
                }
                return _attackLayerMask;
            }
        }

        protected override ActionName TargetKey => ActionName.Shoot;

        [CompilerGenerated]
        public event Action<ReferenceHub> OnServerHit;

        public override void ServerProcessCmd(NetworkReader reader)
        {
            if (!Cooldown.IsReady)
                return;

            if (_resurrect != null && _resurrect.IsInProgress)
                return;

            _target = ReferenceHubReaderWriter.ReadReferenceHub(reader);

            if (_target == null)
                return;

            if (!IsTargetValid(_target))
                return;

            Cooldown.Trigger(CooldownTime);

            CardiacArrest effect = _target.playerEffectsController.GetEffect<CardiacArrest>();

            if (effect != null && effect.IsEnabled)
            {
                Scp049DamageHandler damageHandler = new Scp049DamageHandler(base.Owner, -1f, Scp049DamageHandler.AttackType.Instakill);
                _target.playerStats.DealDamage(damageHandler);
            }
            else
            {
                effect.SetAttacker(base.Owner);
                effect.Intensity = 1;
                effect.ServerChangeDuration(_statusEffectDuration);
            }

            OnServerHit?.Invoke(_target);

            ServerSendRpc(toAll: true);

            Hitmarker.SendHitmarker(base.Owner, 1f);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            Cooldown.WriteCooldown(writer);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            Cooldown.ReadCooldown(reader);
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            ReferenceHubReaderWriter.WriteReferenceHub(writer, _target);
        }

        public override void ResetObject()
        {
            base.ResetObject();
            Cooldown.Clear();
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _resurrect);
        }
        protected override void OnKeyDown()
        {
            base.OnKeyDown();

            if (CanFindTarget(base.Owner.PlayerCameraReference, out _target))
            {
                ClientSendCmd();
            }
        }

        private bool IsTargetValid(ReferenceHub target)
        {
            if (!(target.roleManager.CurrentRole is HumanRole humanRole))
                return false;

            if (base.Owner.isLocalPlayer)
                return true;

            Bounds bounds = humanRole.FpcModule.Tracer.GenerateBounds(LagBacktrackingCompensation, ignoreTeleports: true);

            Vector3 attackerPos = base.ScpRole.FpcModule.Position;
            Vector3 closestPoint = bounds.ClosestPoint(attackerPos);

            if (Vector3.Distance(attackerPos, closestPoint) >= AttackDistance)
                return false;

            return !Physics.Linecast(attackerPos, closestPoint, FpcStateProcessor.Mask);
        }

        private bool CanFindTarget(Transform camera, out ReferenceHub target)
        {
            target = null;

            if (!Physics.Raycast(camera.position, camera.forward, out RaycastHit hitInfo, AttackDistance, AttackMask))
                return false;

            if (!hitInfo.collider.TryGetComponent(out HitboxIdentity hitbox))
                return false;

            target = hitbox.TargetHub;
            return true;
        }
    }
}
