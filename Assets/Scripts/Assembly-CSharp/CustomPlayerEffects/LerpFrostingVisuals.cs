using PostProcessing;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CustomPlayerEffects
{
    public class LerpFrostingVisuals : LerpPostProcessVisuals
    {
        [SerializeField]
        private float _maxFrosting = 0.99f;

        private VignetteRefraction _refraction;

        protected override void Awake()
        {
            base.Awake();
            if (ProcessVolume != null && ProcessVolume.profile != null)
                _refraction = ProcessVolume.profile.GetSetting<VignetteRefraction>();
        }

        protected override void OnWeightChanged(float weight)
        {
            if (_refraction != null)
                _refraction.Frosting.value = Mathf.Min(weight, _maxFrosting);
        }
    }
}
