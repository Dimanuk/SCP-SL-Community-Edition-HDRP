using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class LightenRenderer : PostProcessEffectRenderer<Lighten>
    {
        private Shader shader;
        private readonly int opacityId;
        private readonly int brightnessId;

        public LightenRenderer()
        {
            opacityId = Shader.PropertyToID("_Opacity");
            brightnessId = Shader.PropertyToID("_Brightness");
        }

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/Lighten");
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat(opacityId, settings.opacity.value);
            sheet.properties.SetFloat(brightnessId, settings.brightness.value);

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