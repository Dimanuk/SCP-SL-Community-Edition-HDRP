using System;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
   [Serializable]
    [PostProcess(typeof(DarkenRenderer), PostProcessEvent.AfterStack, "Custom/Darken")]
    public sealed class Darken : PostProcessEffectSettings
    {
        public FloatParameter intensity = new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (!enabled.value)
                return false;

            return intensity.value != 0f;
        }
    }
}