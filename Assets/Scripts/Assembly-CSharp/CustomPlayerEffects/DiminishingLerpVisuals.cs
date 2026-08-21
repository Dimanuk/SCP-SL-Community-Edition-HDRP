using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CustomPlayerEffects
{
    public class DiminishingLerpVisuals : LerpVisualsBase
    {
        [SerializeField]
        private AnimationCurve _intensityCurve;

        [SerializeField]
        private float _diminishingTime = 1f;

        private PostProcessVolume _processVolume;

        public float Intensity
        {
            get => Weight;
            set => Weight = value;
        }

        protected override void Awake()
        {
            base.Awake();
            _processVolume = GetComponent<PostProcessVolume>();
        }

        protected override void Update()
        {
            Weight -= Time.deltaTime / _diminishingTime;
        }

        protected override void OnActivated()
        {
            if (_processVolume != null)
                _processVolume.enabled = true;
        }

        protected override void OnShutdown()
        {
            if (_processVolume != null)
                _processVolume.enabled = false;
        }

        protected override void OnWeightChanged(float weight)
        {
            if (_processVolume != null && _intensityCurve != null)
                _processVolume.weight = _intensityCurve.Evaluate(weight);
        }
    }
}
