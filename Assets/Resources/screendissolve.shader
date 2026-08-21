Shader "Hidden/Custom Effects/ScreenDissolve"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_BlendTex, sampler_BlendTex);
    TEXTURE2D_SAMPLER2D(_OverlayTex, sampler_OverlayTex);

    float _DissolveAmount;

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float2 uv = i.texcoordStereo;

        // Порог dissolve (точная формула из декомпиляции)
        float threshold = (1.0 - _DissolveAmount) * 0.9 - 0.45;

        // Маска dissolve
        float blendValue = SAMPLE_TEXTURE2D(_BlendTex, sampler_BlendTex, uv).r;
        float dissolveValue = blendValue + threshold;

        // Если <= 0.5 — показываем overlay, иначе оригинал
        if (dissolveValue <= 0.5)
        {
            float3 overlay = SAMPLE_TEXTURE2D(_OverlayTex, sampler_OverlayTex, uv).rgb;
            return float4(overlay, 1.0);
        }
        else
        {
            return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        }
    }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag
            ENDHLSL
        }
    }
}