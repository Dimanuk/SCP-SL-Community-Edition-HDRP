using UnityEngine;

namespace CameraShaking
{
    public class TrackerShake : IShakeEffect
    {
        private readonly Transform _tracker;
        private readonly Quaternion _offset;
        private readonly float _intensity;
        private bool _isVisible = true;
        private float _remainingFade = 0.1f;
        private Quaternion _lastKnownRotation;

        public TrackerShake(Transform target, Vector3 offset, float intensity = 1f)
            : this(target, Quaternion.Euler(offset), intensity) { }

        public TrackerShake(Transform target, Quaternion offset, float intensity = 1f)
        {
            _tracker = target;
            _offset = offset;
            _intensity = intensity;
        }

        public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
        {
            if (_tracker == null || !_tracker.gameObject.activeInHierarchy)
                _isVisible = false;

            Quaternion targetRot;
            if (_isVisible)
            {
                targetRot = _tracker.localRotation * _offset;
                if (_intensity != 1f)
                    targetRot = Quaternion.LerpUnclamped(Quaternion.identity, targetRot, _intensity);

                _lastKnownRotation = targetRot;
            }
            else
            {
                _remainingFade -= Time.deltaTime;
                targetRot = Quaternion.Lerp(Quaternion.identity, _lastKnownRotation, _remainingFade / 0.1f);
            }

            Vector3 euler = targetRot.eulerAngles;
            Quaternion finalRot = Quaternion.Euler(-euler.x, euler.z, euler.y);

            shakeValues = new ShakeEffectValues(finalRot, null, null);
            return _isVisible || _remainingFade > 0f;
        }
    }
}