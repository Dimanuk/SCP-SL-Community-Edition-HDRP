using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class ArtefactRenderer : PostProcessEffectRenderer<Artefact>
    {
        private Shader shader;
        private static readonly int _fadeId = Shader.PropertyToID("_Fade");
        private static readonly int _colorisationId= Shader.PropertyToID("_Colorisation");
        private static readonly int _parasiteId = Shader.PropertyToID("_Parasite");
        private static readonly int _noiseId = Shader.PropertyToID("_Noise");

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/Artefact");
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat(_fadeId, settings.Fade.value);
            sheet.properties.SetFloat(_colorisationId, settings.Colorization.value);
            sheet.properties.SetFloat(_parasiteId, settings.Parasite.value);
            sheet.properties.SetFloat(_noiseId, settings.Noise.value);

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