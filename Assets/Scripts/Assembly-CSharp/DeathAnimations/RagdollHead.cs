using PlayerRoles.Spectating;
using UnityEngine;

namespace DeathAnimations
{
    public class RagdollHead : FirstpersonDeathAnimation
    {
        public float CameraFixSpeed;

        public GameObject SpectatorCameraAnchor;

        protected override void OnAnimationStarted()
        {
            if (IsFirstperson)
            {
                EventAssigned = true;
            }

            if (!IsFirstperson)
            {
                Destroy(this);
                return;
            }

            enabled = true;
            SpectatorTargetTracker.SetTrackedTransform(SpectatorCameraAnchor.transform);
            SpectatorCameraAnchor.SetActive(true);
        }

        protected override void OnAnimationEnded()
        {
            EventAssigned = false;
            SpectatorCameraAnchor.SetActive(false);
            SpectatorTargetTracker.SetTrackedTransform(null);
            Destroy(this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsPlaying)
                return;

            float magnitude = collision.relativeVelocity.magnitude;
            if (magnitude > 8f && BloodEffectsSystem.LocalPlayerSingleton != null)
            {
                BloodEffectsSystem.LocalPlayerSingleton.AddScrapes(magnitude / 40f);
            }
        }
    }
}
