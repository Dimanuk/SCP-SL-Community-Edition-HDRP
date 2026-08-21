using Mirror;
using UnityEngine;

namespace Hints
{
    public class AlphaCurveHintEffect : CurveHintEffect
    {
        private float _oldAlpha;

        public static AlphaCurveHintEffect FromNetwork(NetworkReader reader)
        {
            AlphaCurveHintEffect alphaCurveHintEffect = new AlphaCurveHintEffect();
            alphaCurveHintEffect.Deserialize(reader);
            return alphaCurveHintEffect;
        }

        private AlphaCurveHintEffect()
        {
        }

        public AlphaCurveHintEffect(AnimationCurve curve, float startScalar = 0f, float durationScalar = 1f)
            : base(curve, startScalar, durationScalar)
        {
        }

        protected override void Start()
        {
            base.Start();
            if (base.Hint != null)
            {
                _oldAlpha = base.Hint.alpha;
            }
        }

        protected override void UpdateState(float progress)
        {
            if (base.Curve == null || base.Hint == null)
                return;

            float time = progress * base.IterationScalar;
            float evaluatedAlpha = base.Curve.Evaluate(time);

            base.Hint.alpha = evaluatedAlpha;
        }

        protected override void Destroy()
        {
            base.Destroy();
            if (base.Hint != null)
            {
                base.Hint.alpha = _oldAlpha;
            }
        }
    }
}