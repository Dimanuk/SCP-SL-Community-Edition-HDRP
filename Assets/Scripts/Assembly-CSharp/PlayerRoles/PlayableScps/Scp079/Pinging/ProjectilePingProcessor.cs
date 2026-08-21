using System;
using InventorySystem.Items.ThrowableProjectiles;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Pinging
{
    public class ProjectilePingProcessor : IPingProcessor
    {
        private const float Radius = 0.9f;
        private const int Detections = 8;
        private static readonly Collider[] Hit = new Collider[Detections];

        public float Range => 20f;
        public LayerMask Mask => VisionInformation.VisionLayerMask;

        public int IconId { get; private set; }

        public bool TryProcess(RaycastHit hit)
        {
            int count = Physics.OverlapSphereNonAlloc(hit.point, Radius, Hit);

            for (int i = 0; i < count; i++)
            {
                Collider collider = Hit[i];
                
                if (collider.TryGetComponent<ThrownProjectile>(out var projectile))
                {
                    Type projectileType = projectile.GetType();

                    if (projectileType == typeof(ExplosionGrenade))
                    {
                        IconId = 4;
                        return true;
                    }
                    
                    if (projectileType == typeof(FlashbangGrenade))
                    {
                        IconId = 5;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
