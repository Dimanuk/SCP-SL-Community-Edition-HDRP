using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class GlitchRenderer : PostProcessEffectRenderer<Glitch>
    {
        private Shader shader;
        private static readonly int glitchId = Shader.PropertyToID("_Glitch");
        private static readonly int noiseId  = Shader.PropertyToID("_Noise");

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/Glitch");

            if (shader == null)
                Debug.LogError("[Glitch] Shader 'Hidden/Custom Effects/Glitch' not found!");
        }

        public override void Render(PostProcessRenderContext context)
        {
            if (shader == null)
                return;

            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat(glitchId, settings.glitch.value);
            sheet.properties.SetFloat(noiseId,  settings.noise.value);

            RuntimeUtilities.BlitFullscreenTriangle(
                context.command,
                context.source,
                context.destination,
                sheet,
                0,
                false
            );
        }
    }
}