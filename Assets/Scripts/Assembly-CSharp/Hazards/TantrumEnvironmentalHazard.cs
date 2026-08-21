using Knife.DeferredDecals;
using Mirror;
using RelativePositioning;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Hazards
{
    public class TantrumEnvironmentalHazard : global::Hazards.TemporaryHazard
    {
        public static readonly List<TantrumEnvironmentalHazard> AllTantrums = new List<TantrumEnvironmentalHazard>();

        private const float ExplosionHeight = 5f;

        private const float DelayedDestroy = 6f;

        private const string DestroyAnimProperty = "queueDestroy";

        private const string PlaySizzleAnimProperty = "playSizzle";

        [CompilerGenerated]
        [SyncVar]
        private RelativePosition _003CSynchronizedPosition_003Ek__BackingField;

        private readonly float _explodeDistance = 5.25f;

        [SerializeField]
        private Transform _correctPosition;

        [SerializeField]
        private Animator _animator;

        public override global::UnityEngine.Vector3 SourcePosition
        {
            get
            {
                return _correctPosition.position + SourceOffset;
            }
            set
            {
                base.transform.position = value;
            }
        }

        public global::RelativePositioning.RelativePosition SynchronizedPosition
        {
            [global::System.Runtime.CompilerServices.CompilerGenerated]
            get
            {
                return _003CSynchronizedPosition_003Ek__BackingField;
            }
            [global::System.Runtime.CompilerServices.CompilerGenerated]
            set
            {
                _003CSynchronizedPosition_003Ek__BackingField = value;
            }
        }

        public bool PlaySizzle { get; set; }

        protected override float HazardDuration => 180f;

        protected override float DecaySpeed
        {
            get
            {
                float num = 1f;
                foreach (global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup instance in global::InventorySystem.Items.Usables.Scp244.Scp244DeployablePickup.Instances)
                {
                    num += instance.FogPercentForPoint(SourcePosition);
                }
                return num;
            }
        }

        public override void OnEnter(ReferenceHub player)
        {
            if (IsActive && !global::PlayerRoles.PlayerRolesUtils.IsSCP(player))
            {
                base.OnEnter(player);
                player.playerEffectsController.EnableEffect<global::CustomPlayerEffects.Stained>(1f);
            }
        }

        public override void OnStay(ReferenceHub player)
        {
            player.playerEffectsController.EnableEffect<global::CustomPlayerEffects.Stained>(1f);
        }

        public override void OnExit(ReferenceHub player)
        {
            base.OnExit(player);
            if (IsActive && !global::PlayerRoles.PlayerRolesUtils.IsSCP(player))
            {
                player.playerEffectsController.EnableEffect<global::CustomPlayerEffects.Stained>(2f);
            }
        }

        protected override void Start()
        {
            base.Start();
            if (global::Mirror.NetworkServer.active)
            {
                AllTantrums.Add(this);
                global::InventorySystem.Items.ThrowableProjectiles.ExplosionGrenade.OnExploded += CheckExplosion;
            }
        }

        [global::Mirror.Server]
        public override void ServerDestroy()
        {
            if (!global::Mirror.NetworkServer.active)
            {
                global::UnityEngine.Debug.LogWarning("[Server] function 'System.Void Hazards.TantrumEnvironmentalHazard::ServerDestroy()' called when server was not active");
                return;
            }
            base.ServerDestroy();
            RpcDespawn(PlaySizzle);
            ServerDelayedDestroy(6f);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (global::Mirror.NetworkServer.active)
            {
                AllTantrums.Remove(this);
                global::InventorySystem.Items.ThrowableProjectiles.ExplosionGrenade.OnExploded -= CheckExplosion;
            }
        }

        [global::Mirror.ClientRpc]
        private void RpcDespawn(bool playSizzle)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC RpcDespawn called on server.");
                return;
            }

            _animator.SetBool(DestroyAnimProperty, true);

            _animator.SetBool(PlaySizzleAnimProperty, playSizzle);
        }

        private void ServerDelayedDestroy(float waitTime)
        {
            if (global::Mirror.NetworkServer.active)
            {
                global::MEC.Timing.RunCoroutine(DelayedPuddleRemoval(waitTime).CancelWith(base.gameObject));
            }
        }

        private void LateUpdate()
        {
            SourcePosition = SynchronizedPosition.Position;
        }

        private void CheckExplosion(global::Footprinting.Footprint attacker, global::UnityEngine.Vector3 pos, global::InventorySystem.Items.ThrowableProjectiles.ExplosionGrenade grenade)
        {
            global::UnityEngine.Vector3 position = _correctPosition.position;
            if (!(global::UnityEngine.Mathf.Abs(pos.y - position.y) > 5f))
            {
                float num = _explodeDistance * _explodeDistance;
                if (!((position - pos).SqrMagnitudeIgnoreY() > num))
                {
                    PlaySizzle = true;
                    ServerDestroy();
                }
            }
        }

        private global::System.Collections.Generic.IEnumerator<float> DelayedPuddleRemoval(float waitTime)
        {
            yield return global::MEC.Timing.WaitForSeconds(waitTime);
            global::Mirror.NetworkServer.Destroy(base.gameObject);
        }
    }
}
