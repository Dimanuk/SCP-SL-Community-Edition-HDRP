using System;
using UnityEngine;
using UnityEngine.Rendering;

#if PPS
using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using Vector2Parameter = UnityEngine.Rendering.PostProcessing.Vector2Parameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;
#endif

namespace SCPE
{
#if PPS
    public sealed class CausticsRenderer : PostProcessEffectRenderer<Caustics>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Caustics);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(shader);
            var cmd = context.command;
            
            var p = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, false);
            p[2, 3] = p[3, 2] = 0.0f;
            p[3, 3] = 1.0f;
            var clipToWorld = Matrix4x4.Inverse(p * context.camera.worldToCameraMatrix) * Matrix4x4.TRS(new Vector3(0, 0, -p[2, 2]), Quaternion.identity, Vector3.one);
            sheet.properties.SetMatrix("clipToWorld", clipToWorld);
    
            if(settings.causticsTexture.value) sheet.properties.SetTexture("_CausticsTex", settings.causticsTexture.value);
            sheet.properties.SetFloat("_LuminanceThreshold", Mathf.GammaToLinearSpace(settings.luminanceThreshold.value));
            sheet.properties.SetVector("_CausticsParams", new Vector4(settings.size, settings.speed, settings.projectFromSun.value ? 1 : 0, settings.intensity));
            sheet.properties.SetVector("_HeightParams", new Vector4(settings.minHeight.value, settings.minHeightFalloff.value, settings.maxHeight.value, settings.maxHeightFalloff.value));
            
            cmd.SetGlobalVector("_FadeParams", new Vector4(settings.startFadeDistance.value, settings.endFadeDistance.value, 0, settings.distanceFade.value ? 1 : 0));

            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }
    }
#endif
}