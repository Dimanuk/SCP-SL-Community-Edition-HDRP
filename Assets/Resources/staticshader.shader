Shader "Hidden/Custom Effects/Static"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_StaticTex, sampler_StaticTex);

    float _Fade;
    float _FadeAdditive;
    float _FadeDistortion;

    float N21(float2 p)
    {
        float3 p3 = frac(float3(p.xyx) * float3(0.1031, 0.1030, 0.0973));
        p3 += dot(p3, p3.yzx + 33.33);
        return frac((p3.x + p3.y) * p3.z);
    }

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float2 uv = i.texcoord;

        // Анимированные UV для текстуры статики (бесконечный скролл шума)
        float2 staticUV = uv * 1.5 + frac(_Time.y * float2(12.0, 8.0));

        // Семплим текстуру шума
        float4 staticColor = SAMPLE_TEXTURE2D(_StaticTex, sampler_StaticTex, staticUV);

        // --- UV Distortion ---
        // Искажаем UV оригинала на основе R/G каналов статики
        float2 distortionOffset = (staticColor.rg - 0.5) * 2.0;
        float2 distortedUV = uv + distortionOffset * _FadeDistortion * 0.1;

        // Семплим оригинальное изображение с искажёнными координатами
        float4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);

        // --- Fade (замещение изображения шумом) ---
        float4 result = lerp(original, staticColor, saturate(_Fade));

        // --- Additive (добавляем шум поверх картинки) ---
        result.rgb += staticColor.rgb * _FadeAdditive;

        // Лёгкий flicker для живости эффекта
        float flicker = 1.0 - N21(float2(_Time.y * 30.0, 0.0)) * saturate(_Fade + _FadeAdditive) * 0.2;
        result.rgb *= flicker;

        return saturate(result);
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