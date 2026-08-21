using PlayerRoles.PlayableScps.Scp106;
using UnityEngine;

namespace DeathAnimations
{
    public class Scp106Animation : FirstpersonDeathAnimation
    {
        private const float DownRaycastDistance = 20f;
        private const float UpRaycastDistance = 5f;
        private const float DisableDelay = 1.5f;
        private const int RaycastMask = 1;

        public float delay = 6f;

        public Transform portalAnchor;

        private bool _disable;

        private float _fadeTime;

        private void Update()
        {
            _fadeTime += Time.deltaTime;

            if (!_disable && _fadeTime >= delay)
            {
                Scp106Hud.SetDissolveAnimation(_fadeTime - delay);
            }

            if (_fadeTime >= delay + DisableDelay)
            {
                foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                {
                    renderer.gameObject.SetActive(false);
                }
                enabled = false;
            }
        }

        protected override void OnAnimationStarted()
        {
            base.OnAnimationStarted();
            enabled = true;

            Vector3 position = portalAnchor.position;
            if (Physics.Raycast(new Ray(position, Vector3.down), out RaycastHit hit, DownRaycastDistance, RaycastMask)
                || Physics.Raycast(new Ray(position, Vector3.up), out hit, UpRaycastDistance, RaycastMask))
            {
                portalAnchor.position = hit.point + Vector3.up;
                portalAnchor.up = hit.normal;
            }

            if (!IsFirstperson)
            {
                _disable = true;
            }
        }

        protected override void OnAnimationEnded()
        {
            EventAssigned = false;

            if (!_disable)
            {
                Scp106Hud.SetDissolveAnimation(0f);
                _disable = true;
            }
        }
    }
}
