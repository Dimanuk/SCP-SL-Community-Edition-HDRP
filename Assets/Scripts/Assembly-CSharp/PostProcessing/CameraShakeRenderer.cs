using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class CameraShakeRenderer : PostProcessEffectRenderer<CameraShake>
    {
        private Shader shader;
        private float _verticalJumpTime;

        private readonly int _ScanLineJitter = Shader.PropertyToID("_ScanLineJitter");
        private readonly int _VerticalJump = Shader.PropertyToID("_VerticalJump");
        private readonly int _HorizontalShake = Shader.PropertyToID("_HorizontalShake");
        private readonly int _ColorDrift = Shader.PropertyToID("_ColorDrift");

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/CameraShake");
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            _verticalJumpTime += Time.deltaTime * settings.verticalJump.value * 11.3f;

            float jitterAmount = settings.scanLineJitter.value;
            float jitterThreshold = Mathf.Clamp01(1.2f - jitterAmount);
            var jitterVector = new Vector4(jitterAmount, jitterThreshold, 0f, 0f);
            sheet.properties.SetVector(_ScanLineJitter, jitterVector);

            var jumpVector = new Vector4(_verticalJumpTime, settings.verticalJump.value, 0f, 0f);
            sheet.properties.SetVector(_VerticalJump, jumpVector);

            float shake = settings.horizontalShake.value * 0.2f;
            sheet.properties.SetFloat(_HorizontalShake, shake);

            float driftTime = Time.time * 606.11f;
            float driftAmount = settings.colorDrift.value * 0.04f;
            var driftVector = new Vector4(driftTime, driftAmount, 0f, 0f);
            sheet.properties.SetVector(_ColorDrift, driftVector);

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