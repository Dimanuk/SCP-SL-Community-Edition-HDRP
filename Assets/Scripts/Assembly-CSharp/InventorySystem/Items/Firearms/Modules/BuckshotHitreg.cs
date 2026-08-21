using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class BuckshotHitreg : StandardHitregBase
    {
        [Serializable]
        public struct BuckshotSettings
        {
            public Vector2[] PredefinedPellets;
            public int MaxHits;
            public float Randomness;
            public float OverallScale;
        }

        private struct ShotgunHit
        {
            public readonly float Damage;
            public readonly Ray RcRay;
            public readonly RaycastHit RcResult;

            public ShotgunHit(float damage, Ray ray, RaycastHit hit)
            {
                Damage = damage;
                RcRay = ray;
                RcResult = hit;
            }
        }

        private readonly BuckshotSettings _buckshotSettings;

        protected override Firearm Firearm { get; set; }
        protected override ReferenceHub Hub { get; set; }

        public const float TotalInaccuracyScale = 0.4f;

        private static readonly Dictionary<IDestructible, List<ShotgunHit>> Hits = new Dictionary<IDestructible, List<ShotgunHit>>();

        public float BuckshotScale => _buckshotSettings.OverallScale
            * AttachmentsUtils.AttachmentsValue(Firearm, AttachmentParam.SpreadMultiplier);

        private Vector2 GenerateRandomPelletDirection =>
            ((new Vector2(UnityEngine.Random.value, UnityEngine.Random.value) - Vector2.one / 2f).normalized * UnityEngine.Random.value);

        private float BuckshotRandomness => 1f - Mathf.Clamp01(
            (1f - _buckshotSettings.Randomness)
            * AttachmentsUtils.AttachmentsValue(Firearm, AttachmentParam.SpreadPredictability));

        private int LastFiredAmount => (Firearm.ActionModule as PumpAction)?.LastFiredAmount ?? 1;

        public BuckshotHitreg(Firearm firearm, ReferenceHub hub, BuckshotSettings buckshotSettings)
        {
            Firearm = firearm;
            Hub = hub;
            _buckshotSettings = buckshotSettings;
        }

        protected override void ServerPerformShot(Ray shootRay)
        {
            bool isGrounded;
            float movementSpeed;

            if (Hub.roleManager.CurrentRole is PlayerRoles.FirstPersonControl.IFpcRole fpcRole)
            {
                isGrounded = fpcRole.FpcModule.IsGrounded;
                movementSpeed = fpcRole.FpcModule.Motor.Velocity.magnitude;
            }
            else
            {
                isGrounded = true;
                movementSpeed = 0f;
            }

            float inaccuracy = Firearm.BaseStats.GetInaccuracy(Firearm, Firearm.AdsModule.ServerAds, movementSpeed, isGrounded)
                * TotalInaccuracyScale;


            Vector2 offsetVector = (new Vector2(UnityEngine.Random.value, UnityEngine.Random.value) - Vector2.one / 2f).normalized
                * UnityEngine.Random.value * inaccuracy;

            Hits.Clear();

            for (int i = 0; i < LastFiredAmount; i++)
            {
                foreach (Vector2 pelletSettings in _buckshotSettings.PredefinedPellets)
                {
                    ShootPellet(pelletSettings, shootRay, offsetVector);
                }
            }

            float totalDamage = 0f;

            foreach (var hit in Hits)
            {
                totalDamage += ApplyHits(hit.Key, hit.Value);
            }


            if (totalDamage > 0f)
            {
                Hitmarker.SendHitmarker(base.Conn, totalDamage / 50f + 0.5f);
            }
        }

        private float ApplyHits(IDestructible target, List<ShotgunHit> hits)
        {
            float totalDamageDealt = 0f;

            bool showHitmarker = true;
            if (ReferenceHub.TryGetHubNetID(target.NetworkId, out var hub))
            {
                if (hub.playerEffectsController.GetEffect<CustomPlayerEffects.Invisible>().IsEnabled)
                {
                    showHitmarker = false;
                }
            }

            foreach (ShotgunHit hit in hits)
            {
                float damage = hit.Damage;

                if (target.Damage(damage, new PlayerStatsSystem.FirearmDamageHandler(Firearm, damage, useHumanMutlipliers: false), hit.RcResult.point))
                {
                    PlaceBloodDecal(hit.RcRay, hit.RcResult, target);

                    if (showHitmarker)
                    {
                        totalDamageDealt += damage;
                    }
                }
            }

            ShowHitIndicator(target.NetworkId, totalDamageDealt, Hub.transform.position);
            return totalDamageDealt;
        }

        private bool CanShoot(IDestructible dest)
        {
            if (!Hits.TryGetValue(dest, out var list))
            {
                list = new List<ShotgunHit>();
                Hits.Add(dest, list);
                return true;
            }

            return list.Count < _buckshotSettings.MaxHits * LastFiredAmount;
        }

        private void ShootPellet(Vector2 pelletSettings, Ray originalRay, Vector2 offsetVector)
        {
            Vector2 direction2D = Vector2.Lerp(pelletSettings, GenerateRandomPelletDirection, BuckshotRandomness)
                * BuckshotScale;

            Vector3 direction = originalRay.direction;
            direction = Quaternion.AngleAxis(direction2D.x + offsetVector.x, Hub.PlayerCameraReference.up) * direction;
            direction = Quaternion.AngleAxis(direction2D.y + offsetVector.y, Hub.PlayerCameraReference.right) * direction;

            Ray ray = new(originalRay.origin, direction);

            if (Physics.Raycast(ray, out var hitInfo, Firearm.BaseStats.MaxDistance(), StandardHitregBase.HitregMask))
            {
                if (hitInfo.collider == null || !hitInfo.collider.TryGetComponent<IDestructible>(out var component))
                {
                    PlaceBulletholeDecal(ray, hitInfo);
                }
                else if (CanShoot(component))
                {
                    float damage = Firearm.BaseStats.DamageAtDistance(Firearm, hitInfo.distance)
                        / (float)_buckshotSettings.MaxHits;

                    Hits[component].Add(new ShotgunHit(damage, ray, hitInfo));
                }
            }
        }
    }
}