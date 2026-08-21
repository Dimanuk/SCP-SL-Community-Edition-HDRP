using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    [PostProcess(typeof(BlurEffectRenderer), PostProcessEvent.AfterStack, "Custom/BlurEffect")]
    public sealed class BlurEffect : DistanceEffect
    {
        [Space]
        public BoolParameter highQuality;
        public FloatParameter amount;
        public IntParameter iterations;
        public IntParameter downscaling;

        public BlurEffect()
        {
            highQuality = new BoolParameter { value = false };
            amount = new FloatParameter { value = 0f };
            iterations = new IntParameter { value = 6 };
            downscaling = new IntParameter { value = 2 };
        }

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value;
        }
    }
}