Shader "Hidden/Custom Effects/Darken"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    float _DarknessIntensity;

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoordStereo);

        // Логика из декомпилированного шейдера:
        // result = color + (-color) * intensity
        // что эквивалентно: color * (1.0 - intensity)
        col.rgb = col.rgb * (1.0 - saturate(_DarknessIntensity));

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