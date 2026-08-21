using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CustomPlayerEffects
{
    public abstract class PostProcessEffectWave : MonoBehaviour
    {
        public float WaveIntensity = 1f;
        public float speedMultiplier = 1f;
        public float disableTolerance = 0.01f;

        private float _defaultIntensity;
        private float _timer;
        private bool _disabling;

        protected abstract float EffectValue { get; set; }
        protected abstract void SetEffectType(PostProcessProfile profile);

        public void EnableEffect()
        {
            base.enabled = true;
            _disabling = false;
        }

        public void DisableEffect()
        {
            if (base.enabled)
                _disabling = true;
        }

        private void Awake()
        {
            if (TryGetComponent<PostProcessVolume>(out var volume))
            {
                SetEffectType(volume.profile);
                _defaultIntensity = EffectValue;
            }
        }

        private void Update()
        {
            if (_disabling)
            {
                if (EffectValue < disableTolerance)
                {
                    _disabling = false;
                    base.enabled = false;
                }
                else
                {
                    _timer += Time.deltaTime * speedMultiplier;
                    EffectValue = Mathf.Lerp(EffectValue, 0f, _timer);
                }
            }
        }
    }
}
