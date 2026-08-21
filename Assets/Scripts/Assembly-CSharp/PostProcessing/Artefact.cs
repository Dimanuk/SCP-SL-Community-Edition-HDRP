using System;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    [PostProcess(typeof(ArtefactRenderer), PostProcessEvent.AfterStack, "Custom/Artefact")]
    public sealed class Artefact : PostProcessEffectSettings
    {
        public FloatParameter Fade = new FloatParameter { value = 0.01f };
        public FloatParameter Colorization = new FloatParameter { value = 0.9f };
        public FloatParameter Parasite = new FloatParameter { value = 5.3f };
        public FloatParameter Noise = new FloatParameter { value = 10.0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (!enabled.value)
                return false;

            return Fade.value > 0f;
        }
    }
}