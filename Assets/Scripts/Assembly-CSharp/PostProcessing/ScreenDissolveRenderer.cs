using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class ScreenDissolveRenderer : PostProcessEffectRenderer<ScreenDissolve>
    {
        private Shader shader;
        private readonly int _dissolveAmount;
        private readonly int _blendTex2Id;
        private readonly int _overlayTex2Id;

        public ScreenDissolveRenderer()
        {
            _dissolveAmount = Shader.PropertyToID("_DissolveAmount");
            _blendTex2Id = Shader.PropertyToID("_BlendTex");
            _overlayTex2Id = Shader.PropertyToID("_OverlayTex");
        }

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/ScreenDissolve");
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat(_dissolveAmount, settings.DissolveAmount.value);
            sheet.properties.SetTexture(_blendTex2Id, settings.BlendTexture.value);
            sheet.properties.SetTexture(_overlayTex2Id, settings.OverlayTexture.value);

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