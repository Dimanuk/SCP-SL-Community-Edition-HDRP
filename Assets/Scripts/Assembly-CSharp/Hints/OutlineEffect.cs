using Mirror;
using UnityEngine;
using TMPro;

namespace Hints
{
    public class OutlineEffect : HintEffect
    {
        private readonly struct OutlineInfo
        {
            public readonly Color32 Color;
            public readonly float Width;

            public OutlineInfo(Color32 color, float width)
            {
                Color = color;
                Width = width;
            }
        }

        private OutlineInfo _oldOutline;

        protected Color32 OutlineColor { get; private set; }
        protected float OutlineWidth { get; private set; }

        public static OutlineEffect FromNetwork(NetworkReader reader)
        {
            OutlineEffect outlineEffect = new OutlineEffect();
            outlineEffect.Deserialize(reader);
            return outlineEffect;
        }

        private OutlineEffect()
        {
        }

        public OutlineEffect(Color32 outlineColor, float outlineWidth, float startScalar = 0f, float durationScalar = 1f)
            : base(startScalar, durationScalar)
        {
            OutlineColor = outlineColor;
            OutlineWidth = outlineWidth;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            OutlineColor = reader.ReadColor32();
            OutlineWidth = reader.ReadFloat();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.WriteColor32(OutlineColor);
            writer.WriteFloat(OutlineWidth);
        }

        protected override void Start()
        {
            base.Start();

            TextMeshProUGUI text = this.Data.Textbox;

            this._oldOutline = new OutlineInfo(text.outlineColor, text.outlineWidth);
        }

        protected override void Destroy()
        {
            base.Destroy();

            TextMeshProUGUI text = this.Data.Textbox;

            text.outlineColor = this._oldOutline.Color;
            text.outlineWidth = this._oldOutline.Width;
        }

        protected override void UpdateState(float progress)
        {
            TextMeshProUGUI text = this.Data.Textbox;

            text.outlineColor = this.OutlineColor;
            text.outlineWidth = this.OutlineWidth;
        }
    }
}