using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class GrayscaleRenderer : PostProcessEffectRenderer<Grayscale>
    {
        private Shader shader;
        private readonly int _graySCaleIntensityId;

        public GrayscaleRenderer()
        {
            _graySCaleIntensityId = Shader.PropertyToID("_GrayScaleIntensity");
        }

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/Grayscale");
        }

        public override void Release()
        {
            base.Release();
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            settings.SetDistanceProperties(context, sheet);
            sheet.properties.SetFloat(_graySCaleIntensityId, settings.grayScaleIntensity.value);

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