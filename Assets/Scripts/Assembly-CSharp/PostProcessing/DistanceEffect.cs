using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    public abstract class DistanceEffect : PostProcessEffectSettings
    {
        [Space]
        public FloatParameter maxIntensity;
        public FloatParameter fogStartDistance;
        public FloatParameter fogFadeDistance;
        public FloatParameter fogDistanceOffset;
        public FloatParameter skyboxInfluence;

        public DistanceEffect()
        {
            maxIntensity = new FloatParameter { value = 1.0f };
            fogStartDistance = new FloatParameter { value = 1.6f };
            fogFadeDistance = new FloatParameter { value = 0.5f };
            fogDistanceOffset = new FloatParameter { value = 0f };
            skyboxInfluence = new FloatParameter { value = 1.0f };
        }

        public void SetDistanceProperties(PostProcessRenderContext context, PropertySheet sheet)
        {
            if (context == null || sheet == null)
                return;

            var cmd = context.command;
            var camera = context.camera;

            // Neutralize the projection matrix's z-terms so that, once inverted, it reconstructs
            // a per-vertex world-space ray whose length (when scaled by LinearEyeDepth) gives the
            // true world position of the fragment along that ray.
            var gpuProjectionMatrix = GL.GetGPUProjectionMatrix(camera.projectionMatrix, false);
            gpuProjectionMatrix[3, 2] = 0f;
            gpuProjectionMatrix[2, 3] = 0f;
            gpuProjectionMatrix[3, 3] = 1f;

            var clipToWorld = Matrix4x4.Inverse(gpuProjectionMatrix * camera.worldToCameraMatrix)
                * Matrix4x4.TRS(new Vector3(0f, 0f, -gpuProjectionMatrix[2, 2]), Quaternion.identity, Vector3.one);

            sheet.properties.SetMatrix("clipToWorld", clipToWorld);

            float start = fogStartDistance.value + fogDistanceOffset.value;
            float fade = fogFadeDistance.value;
            float rampEnd = start + fade + fade;
            float rampStart = start - fade;
            float rampWidth = rampEnd - rampStart;
            float invRampWidth = Mathf.Abs(rampWidth) > 0.0001f ? 1f / rampWidth : 0f;

            cmd.SetGlobalVector("_SceneFogParams", new Vector4(
                -fogStartDistance.value,
                maxIntensity.value,
                -invRampWidth,
                rampEnd * invRampWidth
            ));
            cmd.SetGlobalFloat("_SkyboxInfluence", skyboxInfluence.value);
        }
    }
}