using Mirror;
using UnityEngine;

namespace Hints
{
    public class AlphaEffect : HintEffect
    {
        protected float Alpha { get; private set; }
        private float _oldAlpha;

        public static AlphaEffect FromNetwork(NetworkReader reader)
        {
            AlphaEffect alphaEffect = new AlphaEffect();
            alphaEffect.Deserialize(reader);
            return alphaEffect;
        }

        private AlphaEffect() { }

        public AlphaEffect(float alpha, float startScalar = 0f, float durationScalar = 1f)
            : base(startScalar, durationScalar)
        {
            Alpha = alpha;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            Alpha = reader.ReadFloat();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.WriteFloat(Alpha);
        }

        protected override void Start()
        {
            if (Data.Textbox == null) return;
            _oldAlpha = Data.Textbox.alpha;
            Data.Textbox.alpha = Alpha;
        }

        protected override void UpdateState(float progress)
        {
            if (Data.Textbox != null)
                Data.Textbox.alpha = Alpha;
        }

        protected override void Destroy()
        {
            if (Data.Textbox != null)
                Data.Textbox.alpha = _oldAlpha;
        }
    }
}