using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    [PostProcess(typeof(GlitchRenderer), PostProcessEvent.AfterStack, "Custom/Glitch")]
    public sealed class Glitch : PostProcessEffectSettings
    {
        [Range(0f, 20f)]
        public FloatParameter glitch = new FloatParameter { value = 0f };

        [Range(0f, 1f)]
        public FloatParameter noise = new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (!enabled.value)
                return false;

            return glitch.value > 0f || noise.value > 0f;
        }
    }
}