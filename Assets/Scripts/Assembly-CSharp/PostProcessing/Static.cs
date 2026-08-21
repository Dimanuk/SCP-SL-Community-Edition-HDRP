using System;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
   [Serializable]
    [PostProcess(typeof(StaticRenderer), PostProcessEvent.AfterStack, "Custom/Static")]
    public sealed class Static : PostProcessEffectSettings
    {
        public FloatParameter fade= new FloatParameter { value = 0f };
        public FloatParameter fadeAdditive = new FloatParameter { value = 0f };
        public FloatParameter fadeDistortion= new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (!enabled.value)
                return false;

            return fade.value != 0f
                || fadeAdditive.value != 0f
                || fadeDistortion.value != 0f;
        }
    }
}