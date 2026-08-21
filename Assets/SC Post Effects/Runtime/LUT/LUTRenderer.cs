using UnityEngine;

#if !PPS
using UnityEngine.Rendering.PostProcessing;
#endif

namespace SCPE
{
#if !PPS
    public sealed class LUTRenderer : PostProcessEffectRenderer<LUT>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.LUT);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            if (LUT.Bypass) return;
            
            var sheet = context.propertySheets.Get(shader);

            if (settings.lutNear.value)
            {
                sheet.properties.SetTexture("_LUT_Near", settings.lutNear);
                sheet.properties.SetVector("_LUT_Params", new Vector4(1f / settings.lutNear.value.width, 1f / settings.lutNear.value.height, settings.lutNear.value.height - 1f, settings.intensity));
            }

            if ((int)settings.mode.value == 1)
            {
                context.command.SetGlobalVector("_FadeParams", new Vector4(settings.startFadeDistance.value, settings.endFadeDistance.value, 0, 0));

                if (settings.lutFar.value) sheet.properties.SetTexture("_LUT_Far", settings.lutFar);
            }

            sheet.properties.SetFloat("_Invert", settings.invert);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.mode.value);
        }
    }
#endif
}
