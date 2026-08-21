Shader "Hidden/Custom Effects/FogEffect"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

    float4x4 clipToWorld;

    // Packed by DistanceEffect.SetDistanceProperties:
    //   x = -fogStartDistance
    //   y = maxIntensity
    //   z = -1 / (3 * fogFadeDistance)
    //   w = (fogStartDistance + fogDistanceOffset + 2 * fogFadeDistance) * -z
    float4 _SceneFogParams;
    float _SkyboxInfluence;
    float _FogBrightness;

    struct VaryingsFog
    {
        float4 vertex : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        float2 texcoordStereo : TEXCOORD1;
        float3 ray : TEXCOORD2;
    };

    VaryingsFog Vert(AttributesDefault v)
    {
        VaryingsDefault d = VertDefault(v);

        VaryingsFog o;
        o.vertex = d.vertex;
        o.texcoord = d.texcoord;
        o.texcoordStereo = d.texcoordStereo;

        // World-space ray from the camera through this vertex, reconstructed via clipToWorld
        // (built on the CPU with the projection matrix's z-terms neutralized). Scaling this ray
        // by LinearEyeDepth() in the fragment stage yields the fragment's true world position.
        // NB: mul(M, v) — the compiled original combines matrix COLUMNS (x*col0 + y*col1 + col3);
        // indexing rows via clipToWorld[i] here would transpose the matrix and break the ray.
        o.ray = mul(clipToWorld, float4(v.vertex.xy, 0.0, 1.0)).xyz - _WorldSpaceCameraPos;

        return o;
    }

    float4 Frag(VaryingsFog i) : SV_Target
    {
        float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo);
        float linearDepth = LinearEyeDepth(rawDepth);

        // True Euclidean distance from the camera to the fragment's world position, not just
        // Z-depth — matters off-axis, where Z-depth under-measures distance vs. the real ray length.
        float3 worldPos = i.ray * linearDepth + _WorldSpaceCameraPos;
        float dist = length(_WorldSpaceCameraPos - worldPos);

        float d = max(dist - _ProjectionParams.y + _SceneFogParams.x, 0.0);
        float distFactor = saturate(d * _SceneFogParams.z + _SceneFogParams.w);

        // Skybox-adjusted factor: at _SkyboxInfluence = 0 the skybox ignores the fog entirely.
        float skyboxFactor = _SkyboxInfluence * (distFactor - 1.0) + 1.0;

        float depth01 = Linear01Depth(rawDepth);
        float factor = (depth01 > 0.99) ? skyboxFactor : distFactor;

        float fogAmount = min(1.0 - factor, _SceneFogParams.y);

        float4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoordStereo);
        float3 col = lerp(sceneColor.rgb, _FogBrightness.xxx, fogAmount);

        return float4(col, sceneColor.a);
    }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
