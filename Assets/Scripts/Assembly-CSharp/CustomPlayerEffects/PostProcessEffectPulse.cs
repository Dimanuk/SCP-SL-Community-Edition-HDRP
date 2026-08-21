using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CustomPlayerEffects
{
    public abstract class PostProcessEffectPulse : MonoBehaviour
    {
        public float PulseIntensity = 1f;
        public float animationSpeedMultiplier = 1f;

        private float _defaultIntensity;
        private float timer;

        private AnimationCurve pulseAnimation = new();

        protected abstract float EffectValue { get; set; }
        protected abstract void SetEffectType(PostProcessProfile profile);

        public void Pulse()
        {
            base.enabled = true;
        }

        private void Awake()
        {
            var volume = GetComponent<PostProcessVolume>();
            SetEffectType(volume.profile);
            _defaultIntensity = EffectValue;
            pulseAnimation = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));
            timer = 0f;
        }

        private void Update()
        {
            if (timer < 1f)
            {
                timer += Time.deltaTime * animationSpeedMultiplier;
                EffectValue = Mathf.Lerp(_defaultIntensity, PulseIntensity, pulseAnimation.Evaluate(timer));
            }
            else
            {
                Reset();
            }
        }

        private void Reset()
        {
            EffectValue = _defaultIntensity;
            timer = 0f;
            base.enabled = false;
        }
    }
}
