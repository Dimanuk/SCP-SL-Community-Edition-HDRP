using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class VignetteRefractionRenderer : PostProcessEffectRenderer<VignetteRefraction>
    {
        private Shader _shader;

        private readonly int _refractionTexId;
        private readonly int _valueId;
        private readonly int _color1Id;
        private readonly int _color2Id;

        public VignetteRefractionRenderer()
        {
            _refractionTexId = Shader.PropertyToID("_RefractionTex");
            _valueId = Shader.PropertyToID("_Values");
            _color1Id = Shader.PropertyToID("_Color1");
            _color2Id = Shader.PropertyToID("_Color2");
        }

        public override void Init()
        {
            _shader = Shader.Find("Hidden/Custom Effects/VignetteRefraction");
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(_shader);
            var properties = sheet.properties;

            var values = new Vector4(
                settings.Intensity.value,
                settings.RefractionPower.value,
                settings.Frosting.value,
                settings.Fade.value
            );
            properties.SetVector(_valueId, values);
            properties.SetTexture(_refractionTexId, settings.RefractionTexture.value);

            properties.SetColor(_color1Id, settings.ColorStart.value);
            properties.SetColor(_color2Id, settings.ColorEnd.value);

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