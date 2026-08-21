using System;
using Mirror;
using UnityEngine;
using Utils.Networking;

namespace Hints
{
    public abstract class CurveHintEffect : HintEffect
    {
        private AnimationCurve _curve;

        protected AnimationCurve Curve
        {
            get => _curve;
            set
            {
                if (value != null && value.length > 0)
                {
                    float timeRange = value[value.length - 1].time - value[0].time;
                    IterationScalar = timeRange;
                }
                else
                {
                    IterationScalar = 0f;
                }
                _curve = value;
            }
        }

        protected float IterationScalar { get; private set; }

        protected CurveHintEffect(float startScalar = 0f, float durationScalar = 1f)
            : base(durationScalar)
        {
            this.StartScalar = startScalar; 
        }

        protected CurveHintEffect(AnimationCurve curve, float startScalar = 0f, float durationScalar = 1f)
            : base(durationScalar)
        {
            this.StartScalar = startScalar;
            this.Curve = curve ?? throw new ArgumentNullException(nameof(curve));
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            this.StartScalar = reader.ReadFloat();
            this.Curve = AnimationCurveReaderWriter.ReadAnimationCurve(reader);
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.WriteFloat(this.StartScalar);
            AnimationCurveReaderWriter.WriteAnimationCurve(writer, this._curve);
        }

        protected float EvaluateCurve(float progress)
        {
            if (_curve == null) throw new NullReferenceException();
            return _curve.Evaluate(progress * IterationScalar);
        }
    }
}