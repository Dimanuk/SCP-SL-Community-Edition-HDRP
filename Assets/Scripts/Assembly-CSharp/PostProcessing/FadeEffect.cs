using System;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
   [Serializable]
    [PostProcess(typeof(FadeEffectRenderer), PostProcessEvent.AfterStack, "Custom/FadeEffect")]
    public sealed class FadeEffect : DistanceEffect
    {
        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value;
        }
    }
}