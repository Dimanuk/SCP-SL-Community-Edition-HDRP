using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    [PostProcess(typeof(VignetteRefractionRenderer), PostProcessEvent.AfterStack, "Custom/VignetteRefraction")]
    public sealed class VignetteRefraction : PostProcessEffectSettings
    {
        public ColorParameter ColorStart;
        public ColorParameter ColorEnd;
        public TextureParameter RefractionTexture;
        public FloatParameter RefractionPower;
        public FloatParameter Intensity;
        public FloatParameter Frosting;
        public FloatParameter Fade;

        public VignetteRefraction()
        {
            ColorStart = new ColorParameter { value = Color.white };
            ColorEnd = new ColorParameter { value = Color.white };
            RefractionTexture = new TextureParameter();
            RefractionPower = new FloatParameter { value = 0.5f };
            Intensity = new FloatParameter { value = 0f };
            Frosting = new FloatParameter { value = 0f };
            Fade = new FloatParameter { value = 0.05f };
        }

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (RefractionTexture.value == null)
                return false;

            if (Intensity.value <= 0f)
                return false;

            return enabled.value;
        }
    }
}