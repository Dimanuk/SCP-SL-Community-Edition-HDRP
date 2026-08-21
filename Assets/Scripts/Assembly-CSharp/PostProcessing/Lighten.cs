using System;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    [PostProcess(typeof(LightenRenderer), PostProcessEvent.AfterStack, "Custom/Lighten")]
    public sealed class Lighten : PostProcessEffectSettings
    {
        public FloatParameter brightness;
        public FloatParameter opacity;

        public Lighten()
        {
            brightness = new FloatParameter { value = 1.0f };
            opacity = new FloatParameter { value = 0f };
        }

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (!enabled.value)
                return false;

            return opacity.value != 0f;
        }
    }
}