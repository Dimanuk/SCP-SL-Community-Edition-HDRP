using System;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    [PostProcess(typeof(ScreenDissolveRenderer), PostProcessEvent.AfterStack, "Custom/ScreenDissolve")]
    public sealed class ScreenDissolve : PostProcessEffectSettings
    {
        public FloatParameter DissolveAmount;
        public TextureParameter BlendTexture;
        public TextureParameter OverlayTexture;

        public ScreenDissolve()
        {
            DissolveAmount = new FloatParameter { value = 0f };
            BlendTexture = new TextureParameter();
            OverlayTexture = new TextureParameter();
        }

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (!enabled.value)
                return false;

            if (BlendTexture.value == null)
                return false;

            if (OverlayTexture.value == null)
                return false;

            return DissolveAmount.value != 0f;
        }
    }
}