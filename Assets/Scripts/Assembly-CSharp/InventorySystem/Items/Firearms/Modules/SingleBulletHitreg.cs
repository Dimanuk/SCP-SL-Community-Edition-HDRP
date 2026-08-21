using InventorySystem.Items.Firearms;

namespace InventorySystem.Items.Firearms.Modules
{
    public class SingleBulletHitreg : global::InventorySystem.Items.Firearms.Modules.StandardHitregBase
    {
        private readonly bool _usesRecoilPattern;

        public readonly global::InventorySystem.Items.Firearms.FirearmRecoilPattern RecoilPattern;

        protected override global::InventorySystem.Items.Firearms.Firearm Firearm { get; set; }

        protected override ReferenceHub Hub { get; set; }

        public SingleBulletHitreg(global::InventorySystem.Items.Firearms.Firearm firearm, ReferenceHub hub, global::InventorySystem.Items.Firearms.FirearmRecoilPattern recoilPattern = null)
        {
            Firearm = firearm;
            Hub = hub;
            RecoilPattern = recoilPattern;
            _usesRecoilPattern = RecoilPattern != null;
        }

        protected virtual global::UnityEngine.Ray ServerRandomizeRay(global::UnityEngine.Ray ray)
        {
            global::InventorySystem.Items.Firearms.FirearmBaseStats baseStats = Firearm.BaseStats;
            global::UnityEngine.Vector3 vector = (new global::UnityEngine.Vector3(global::UnityEngine.Random.value, global::UnityEngine.Random.value, global::UnityEngine.Random.value) - global::UnityEngine.Vector3.one / 2f).normalized * global::UnityEngine.Random.value;
            bool isGrounded;
            global::UnityEngine.Vector3 vector2;
            if (Hub.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole)
            {
                isGrounded = fpcRole.FpcModule.IsGrounded;
                vector2 = fpcRole.FpcModule.Motor.Velocity;
            }
            else
            {
                isGrounded = true;
                vector2 = global::UnityEngine.Vector3.zero;
            }
            float num = baseStats.GetInaccuracy(Firearm, Firearm.AdsModule.ServerAds, vector2.magnitude, isGrounded);
            if (_usesRecoilPattern)
            {
                RecoilPattern.ApplyShot(1f / Firearm.ActionModule.CyclicRate);
                num += RecoilPattern.GetInaccuracy();
            }
            ray.direction = global::UnityEngine.Quaternion.Euler(num * vector) * ray.direction;
            return ray;
        }

        protected virtual void ServerProcessRaycastHit(global::UnityEngine.Ray ray, global::UnityEngine.RaycastHit hit)
        {
            if (hit.collider.TryGetComponent<IDestructible>(out var component) && CheckInaccurateFriendlyFire(component))
            {
                float damage = Firearm.BaseStats.DamageAtDistance(Firearm, hit.distance);
                bool didDamage = component.Damage(damage, new global::PlayerStatsSystem.FirearmDamageHandler(Firearm, damage), hit.point);
                if (didDamage)
                {
                    if (!ReferenceHub.TryGetHubNetID(component.NetworkId, out var hub) || !hub.playerEffectsController.GetEffect<global::CustomPlayerEffects.Invisible>().IsEnabled)
                    {
                        Hitmarker.SendHitmarker(base.Conn, 1f);
                    }
                    ShowHitIndicator(component.NetworkId, damage, ray.origin);
                    PlaceBloodDecal(ray, hit, component);
                }
            }
            else
            {
                PlaceBulletholeDecal(ray, hit);
            }
            if (global::InventorySystem.Items.Firearms.Modules.StandardHitregBase.DebugMode)
            {
                SendDebug($"Raycast result: colliderName={hit.collider.name} hitPoint={hit.point} movementSpeed={(global::PlayerRoles.FirstPersonControl.FpcExtensionMethods.GetVelocity(Hub))} ads={Firearm.AdsModule.ServerAds}");
            }
        }

        protected override void ServerPerformShot(global::UnityEngine.Ray ray)
        {
            ray = ServerRandomizeRay(ray);
            global::InventorySystem.Items.Firearms.FirearmBaseStats baseStats = Firearm.BaseStats;
            bool hit = global::UnityEngine.Physics.Raycast(ray, out var hitInfo, baseStats.MaxDistance(), global::InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask);
            if (hit)
            {
                ServerProcessRaycastHit(ray, hitInfo);
            }
        }

        private bool CheckInaccurateFriendlyFire(IDestructible target)
        {
            if (target.NetworkId == base.PrimaryTargetNetId)
            {
                return true;
            }
            if (!ReferenceHub.TryGetHubNetID(target.NetworkId, out var hub))
            {
                return true;
            }
            global::PlayerRoles.Faction faction = global::PlayerRoles.PlayerRolesUtils.GetFaction(hub);
            global::PlayerRoles.Faction faction2 = global::PlayerRoles.PlayerRolesUtils.GetFaction(Hub);
            return faction != faction2;
        }
    }
}
