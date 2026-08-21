using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class DarkenRenderer : PostProcessEffectRenderer<Darken>
    {
        private Shader shader;
        private readonly int intensityId= Shader.PropertyToID("_DarknessIntensity");

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/Darken");
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat(intensityId, settings.intensity.value);

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