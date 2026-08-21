using System;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    [PostProcess(typeof(FogEffectRenderer), PostProcessEvent.AfterStack, "Custom/FogEffect")]
    public sealed class FogEffect : DistanceEffect
    {
        public FloatParameter fogBrightness;

        public FogEffect()
        {
            fogBrightness = new FloatParameter { value = 0f };
        }

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value;
        }
    }
}