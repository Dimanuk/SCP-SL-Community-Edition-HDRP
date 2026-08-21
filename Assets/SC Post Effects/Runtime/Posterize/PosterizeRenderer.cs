using System;
using UnityEngine;
#if !PPS
using UnityEngine.Rendering.PostProcessing;
#endif

namespace SCPE
{
#if !PPS
    public sealed class PosterizeRenderer : PostProcessEffectRenderer<Posterize>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Posterize);
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetVector("_Params", new Vector4(settings.hue.value, settings.saturation.value, settings.value.value, settings.levels.value));

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.hsvMode.value ? 1 : 0);
        }
    }
#endif
}