Shader "Hidden/Custom Effects/Lighten"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    float _Opacity;
    float _Brightness;

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoordStereo);

        // Восстановлено из декомпиляции:
        // result = original + opacity * (brightness - original)
        // что эквивалентно lerp(original, brightness, opacity)
        float3 target = _Brightness.xxx;
        col.rgb = col.rgb + _Opacity * (target - col.rgb);

        return col;
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