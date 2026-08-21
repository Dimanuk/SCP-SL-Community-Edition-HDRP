using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class ArcadeRenderer : PostProcessEffectRenderer<Arcade>
    {
        private Shader shader;
         private static readonly int _fadeId= Shader.PropertyToID("_Fade");

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/Arcade");

            if (shader == null)
                Debug.LogError("[Glitch] Shader 'Hidden/Custom Effects/Arcade' not found!");
        }

        public override void Render(PostProcessRenderContext context)
        {
            if (shader == null)
                return;
            var sheet = context.propertySheets.Get(shader);
            sheet.properties.SetFloat(_fadeId, settings.Fade.value);

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