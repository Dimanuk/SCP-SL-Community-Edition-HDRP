using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class BlurEffectRenderer : PostProcessEffectRenderer<BlurEffect>
    {
        private Shader shader;
        private int screenCopyID;

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/BlurEffect");
            screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
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
            var cmd = context.command;

            cmd.GetTemporaryRT(
                screenCopyID,
                context.width,
                context.height,
                0,
                FilterMode.Bilinear,
                context.sourceFormat
            );

            cmd.Blit(context.source, screenCopyID);

            int temp1ID = Shader.PropertyToID("_Temp1");
            int temp2ID = Shader.PropertyToID("_Temp2");

            int downscale = settings.downscaling.value;
            int w = context.screenWidth / downscale;
            int h = context.screenHeight / downscale;

            cmd.GetTemporaryRT(temp1ID, w, h, 0, FilterMode.Bilinear, context.sourceFormat);
            cmd.GetTemporaryRT(temp2ID, w, h, 0, FilterMode.Bilinear, context.sourceFormat);

            cmd.Blit(screenCopyID, temp1ID);

            // Original bails out entirely on absurd iteration counts (safety clamp).
            if (settings.iterations.value > 12)
            {
                cmd.ReleaseTemporaryRT(screenCopyID);
                cmd.ReleaseTemporaryRT(temp1ID);
                cmd.ReleaseTemporaryRT(temp2ID);
                return;
            }

            // Pass 1 = separable gaussian; highQuality doubles the horizontal/vertical pair.
            for (int i = 0; i < settings.iterations.value; i++)
            {
                float offsetX = settings.amount.value / context.screenWidth;
                cmd.SetGlobalVector("_BlurOffsets", new Vector4(offsetX, 0f, 0f, 0f));
                RuntimeUtilities.BlitFullscreenTriangle(cmd, temp1ID, temp2ID, sheet, 1, false);

                float offsetY = settings.amount.value / context.screenHeight;
                cmd.SetGlobalVector("_BlurOffsets", new Vector4(0f, offsetY, 0f, 0f));
                RuntimeUtilities.BlitFullscreenTriangle(cmd, temp2ID, temp1ID, sheet, 1, false);

                if (settings.highQuality.value)
                {
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(offsetX, 0f, 0f, 0f));
                    RuntimeUtilities.BlitFullscreenTriangle(cmd, temp1ID, temp2ID, sheet, 1, false);

                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(0f, offsetY, 0f, 0f));
                    RuntimeUtilities.BlitFullscreenTriangle(cmd, temp2ID, temp1ID, sheet, 1, false);
                }
            }

            cmd.SetGlobalTexture("_BlurredTex", temp1ID);
            settings.SetDistanceProperties(context, sheet);

            // Pass 0 = distance-masked composite of _BlurredTex over the scene.
            RuntimeUtilities.BlitFullscreenTriangle(
                cmd,
                context.source,
                context.destination,
                sheet,
                0,
                false
            );

            cmd.ReleaseTemporaryRT(screenCopyID);
            cmd.ReleaseTemporaryRT(temp1ID);
            cmd.ReleaseTemporaryRT(temp2ID);
        }
    }
}
