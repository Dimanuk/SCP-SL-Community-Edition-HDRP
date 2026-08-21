using InventorySystem.Items.Firearms;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class MultiShotHitreg : SingleBulletHitreg
    {
        private readonly Vector3[] _offsets;

        public MultiShotHitreg(Firearm fa, ReferenceHub hub, FirearmRecoilPattern pattern, Vector3[] offsets)
            : base(fa, hub, pattern)
        {
            _offsets = offsets ?? System.Array.Empty<Vector3>();
        }

        protected override void ServerPerformShot(Ray ray)
        {
            ray = ServerRandomizeRay(ray);
            Quaternion rot = Hub.PlayerCameraReference.rotation;


            for (int i = 0; i < _offsets.Length; i++)
            {
                Fire(ray, rot * _offsets[i]);
            }
        }

        private void Fire(Ray ray, Vector3 offset)
        {
            ray = new Ray(ray.origin + offset, ray.direction);

            bool hit = Physics.Raycast(
                    ray,
                    out RaycastHit hitInfo,
                    Firearm.BaseStats.MaxDistance(),
                    StandardHitregBase.HitregMask);


            if (hit)
            {
                ServerProcessRaycastHit(ray, hitInfo);
            }
        }
    }
}