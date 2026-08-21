using UnityEngine;

namespace DeathAnimations
{
    public class RigidbodyRagdollHead : RagdollHead
    {
        private Vector3 _localPos;

        private Transform _parent;

        private Transform _t;

        private Vector3 _lastOrigin;

        [SerializeField]
        private float _thickness;

        private static int Mask => PlayerRoles.FirstPersonControl.FpcStateProcessor.Mask;

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
            PlayerRoles.Spectating.SpectatorTargetTracker.SetTrackedTransform(SpectatorCameraAnchor.transform);
            SpectatorCameraAnchor.SetActive(true);
            _t = SpectatorCameraAnchor.transform;
            _parent = _t.parent;
            _localPos = _t.localPosition;
            _lastOrigin = _parent.position;
        }

        private void Update()
        {
            Vector3 origin = _parent.position;
            Vector3 direction = origin - _lastOrigin;
            if (Physics.Raycast(origin, direction, out RaycastHit hit, direction.magnitude, Mask))
            {
                _t.position = hit.point - direction.normalized * _thickness;
            }
            else
            {
                _t.localPosition = _localPos;
            }
            _lastOrigin = origin;
        }
    }
}
