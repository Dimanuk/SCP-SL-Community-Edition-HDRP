Shader "Hidden/Custom Effects/Grayscale"
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
    float _GrayScaleIntensity;

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

        // World-space camera ray; scaled by LinearEyeDepth in the fragment it yields the
        // fragment's true world position (same reconstruction as the FogEffect shader).
        o.ray = mul(clipToWorld, float4(v.vertex.xy, 0.0, 1.0)).xyz - _WorldSpaceCameraPos;

        return o;
    }

    float4 Frag(VaryingsFog i) : SV_Target
    {
        float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo);
        float linearDepth = LinearEyeDepth(rawDepth);

        float3 worldPos = i.ray * linearDepth + _WorldSpaceCameraPos;
        float dist = length(_WorldSpaceCameraPos - worldPos);

        float d = max(dist - _ProjectionParams.y + _SceneFogParams.x, 0.0);
        float distFactor = saturate(d * _SceneFogParams.z + _SceneFogParams.w);

        float skyboxFactor = _SkyboxInfluence * (distFactor - 1.0) + 1.0;

        float depth01 = Linear01Depth(rawDepth);
        float factor = (depth01 > 0.99) ? skyboxFactor : distFactor;

        float amount = min(1.0 - factor, _SceneFogParams.y);

        float4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoordStereo);
        float lum = dot(sceneColor.rgb, float3(0.2126, 0.7152, 0.0722));
        float3 col = amount * (lum.xxx - sceneColor.rgb) * _GrayScaleIntensity + sceneColor.rgb;

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
