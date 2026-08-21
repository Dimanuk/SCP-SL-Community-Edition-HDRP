using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utils.Networking;

namespace InventorySystem.Items.Firearms.Modules
{
    public class DisruptorHitreg : StandardHitregBase
    {
        public struct DisruptorHitMessage : NetworkMessage
        {
            public Vector3 Position;
            public LowPrecisionQuaternion Rotation;
        }

        private readonly ExplosionGrenade _explosionSettings;

        private const float ExplosionThrowback = 0.1f;

        protected override Firearm Firearm { get; set; }
        protected override ReferenceHub Hub { get; set; }

        public DisruptorHitreg(Firearm firearm, ReferenceHub hub, ExplosionGrenade explosionSettings)
        {
            Firearm = firearm;
            Hub = hub;
            _explosionSettings = explosionSettings;
        }

        protected override void ServerPerformShot(Ray ray)
        {
            FirearmBaseStats baseStats = Firearm.BaseStats;

            bool isGrounded;
            Vector3 velocity;

            if (Hub.roleManager.CurrentRole is IFpcRole fpcRole)
            {
                isGrounded = fpcRole.FpcModule.IsGrounded;
                velocity = fpcRole.FpcModule.Motor.Velocity;
            }
            else
            {
                isGrounded = true;
                velocity = Vector3.zero;
            }

            Vector3 randomVec = (new Vector3(Random.value, Random.value, Random.value) - Vector3.one / 2f).normalized * Random.value;
            float inaccuracy = baseStats.GetInaccuracy(Firearm, Firearm.AdsModule.ServerAds, velocity.magnitude, isGrounded);

            ray.direction = Quaternion.Euler(inaccuracy * randomVec) * ray.direction;

            LayerMask layerMask = LayerMask.GetMask("Default", "Hitbox", "CCTV", "Door", "Locker", "Pickup");

            if (!Physics.Raycast(ray, out RaycastHit hitInfo, baseStats.MaxDistance(), layerMask))
            {
                return;
            }

            if (!hitInfo.collider.TryGetComponent<IDestructible>(out var component))
            {
                NetworkUtils.SendToAuthenticated(new DisruptorHitMessage
                {
                    Position = hitInfo.point + hitInfo.normal * ExplosionThrowback,
                    Rotation = new LowPrecisionQuaternion(Quaternion.LookRotation(-hitInfo.normal))
                });

                CreateExplosion(hitInfo.point);
                return;
            }

            float damage = baseStats.DamageAtDistance(Firearm, hitInfo.distance);

            if (component.Damage(damage, new DisruptorDamageHandler(Firearm.Footprint, damage), hitInfo.point))
            {
                if (!ReferenceHub.TryGetHubNetID(component.NetworkId, out var hitHub) ||
                    !hitHub.playerEffectsController.GetEffect<CustomPlayerEffects.Invisible>().IsEnabled)
                {
                    Hitmarker.SendHitmarker(base.Conn, 2f);
                }

                ShowHitIndicator(component.NetworkId, damage, ray.origin);
            }

            CreateExplosion(hitInfo.point);
        }

        private void CreateExplosion(Vector3 hitPoint)
        {
            Vector3 offset = (Firearm.Owner.PlayerCameraReference.position - hitPoint).normalized * ExplosionThrowback;
            ExplosionGrenade.Explode(Firearm.Footprint, hitPoint + offset, _explosionSettings);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += () =>
            {
                NetworkClient.ReplaceHandler<DisruptorHitMessage>(ProcessHitMessage, true);
            };
        }

        private static void ProcessHitMessage(DisruptorHitMessage msg)
        {
            if (InventoryItemLoader.TryGetItem(ItemType.ParticleDisruptor, out ItemBase itemBase) &&
                itemBase is ParticleDisruptor disruptor && disruptor.ExplosionPrefab != null)
            {
                GameObject effectInstance = Object.Instantiate(disruptor.ExplosionPrefab);
                if (effectInstance != null)
                {
                    Transform t = effectInstance.transform;
                    t.SetPositionAndRotation(msg.Position, msg.Rotation.Value);
                    t.localScale = Vector3.one;
                }
            }
        }
    }
}
