using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class FogEffectRenderer : PostProcessEffectRenderer<FogEffect>
    {
        private Shader shader;

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/FogEffect");
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

            context.command.SetGlobalFloat("_FogBrightness", settings.fogBrightness.value);

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