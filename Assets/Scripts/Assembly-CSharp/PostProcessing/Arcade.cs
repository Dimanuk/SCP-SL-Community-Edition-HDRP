using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    [PostProcess(typeof(ArcadeRenderer), PostProcessEvent.AfterStack, "Custom/Arcade")]
    public sealed class Arcade : PostProcessEffectSettings
    {
        [Range(0f, 1f)]
        public FloatParameter Fade= new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (!enabled.value)
                return false;

            return Fade.value > 0f;
        }
    }
}