using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class StaticRenderer : PostProcessEffectRenderer<Static>
    {
        private Shader shader;
        private Texture2D staticTexture;

        private readonly int fadeId = Shader.PropertyToID("_Fade");
        private readonly int fadeAdditiveId = Shader.PropertyToID("_FadeAdditive");
        private readonly int fadeDistortionId= Shader.PropertyToID("_FadeDistortion");
        private readonly int staticTexId= Shader.PropertyToID("_StaticTex");

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/Static");
            staticTexture = Resources.Load("StaticTexture") as Texture2D;
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat(fadeId, settings.fade.value);
            sheet.properties.SetFloat(fadeAdditiveId, settings.fadeAdditive.value);
            sheet.properties.SetFloat(fadeDistortionId, settings.fadeDistortion.value);
            sheet.properties.SetTexture(staticTexId, staticTexture);

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