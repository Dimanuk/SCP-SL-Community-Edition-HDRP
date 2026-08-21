using Elevators;
using UnityEngine;

namespace PlayerRoles.Ragdolls
{
    public class DynamicRagdoll : BasicRagdoll
    {
        public Rigidbody[] LinkedRigidbodies;

        public Transform[] LinkedRigidbodiesTransforms;

        public HitboxData[] Hitboxes;

        private static readonly HitboxData[] EmptyHitboxes = new HitboxData[0];

        private static readonly Rigidbody[] EmptyRigidbodies = new Rigidbody[0];

        protected override void OnCleanup()
        {
            base.OnCleanup();

            foreach (Rigidbody rb in LinkedRigidbodies)
            {
                if (rb != null)
                    rb.gameObject.SetActive(false);
            }
        }
    }
}