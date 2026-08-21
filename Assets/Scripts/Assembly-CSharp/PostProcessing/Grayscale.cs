using System;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    [PostProcess(typeof(GrayscaleRenderer), PostProcessEvent.AfterStack, "Custom/Grayscale")]
    public sealed class Grayscale : DistanceEffect
    {
        public FloatParameter grayScaleIntensity;

        public Grayscale()
        {
            grayScaleIntensity = new FloatParameter { value = 0f };
        }

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value;
        }
    }
}