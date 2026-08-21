Shader "Hidden/Custom Effects/Artefact"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/Colors.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    float _Fade;
    float _Colorisation;
    float _Parasite;
    float _Noise;

    // Простой pseudo-random
    float N21(float2 p)
    {
        float3 p3 = frac(float3(p.xyx) * float3(0.1031, 0.1030, 0.0973));
        p3 += dot(p3, p3.yzx + 33.33);
        return frac((p3.x + p3.y) * p3.z);
    }

    // 2D noise
    float N2(float2 p)
    {
        float2 i = floor(p);
        float2 f = frac(p);
        float a = N21(i);
        float b = N21(i + float2(1.0, 0.0));
        float c = N21(i + float2(0.0, 1.0));
        float d = N21(i + float2(1.0, 1.0));
        float2 u = f * f * (3.0 - 2.0 * f);
        return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
    }

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float2 uv = i.texcoord;
        float2 originalUV = uv;

        // --- Parasite (полосы/артефакты) ---
        float parasiteIntensity = _Parasite * 0.1;
        
        // Горизонтальные полосы с разной частотой
        float lineNoise = N21(float2(0.0, floor(uv.y * 120.0))) * 2.0 - 1.0;
        float lineMask = step(0.92, N21(float2(_Time.y * 0.5, floor(uv.y * 60.0))));
        
        // Случайные блоки артефактов
        float blockSize = 16.0 + _Parasite * 4.0;
        float2 blockUV = floor(uv * blockSize) / blockSize;
        float blockNoise = N21(blockUV + frac(_Time.y * 0.3) * 10.0);
        float blockMask = step(0.85 - parasiteIntensity * 0.05, blockNoise);

        // Смещение UV для артефактов
        float2 shiftedUV = uv;
        shiftedUV.x += (lineNoise * lineMask + blockMask * (N21(float2(_Time.y, uv.y)) - 0.5)) * parasiteIntensity * 0.05;
        
        // --- Colorisation (хроматическая аберрация + цветовой сдвиг) ---
        float colorShift = _Colorisation * 0.02;
        
        // RGB split с искажением от Parasite
        float2 redOffset = float2(colorShift * (1.0 + parasiteIntensity * 0.3), 0.0);
        float2 greenOffset = float2(0.0, colorShift * 0.5);
        float2 blueOffset = float2(-colorShift * (1.0 + parasiteIntensity * 0.2), colorShift * 0.3);

        // Добавляем искажение от полос к цветовым каналам
        float lineShift = lineMask * parasiteIntensity * 0.03;
        redOffset.x += lineShift;
        blueOffset.x -= lineShift;

        float4 colorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftedUV + redOffset);
        float4 colorG = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftedUV + greenOffset);
        float4 colorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftedUV + blueOffset);

        float4 col = float4(colorR.r, colorG.g, colorB.b, 1.0);

        // --- Noise (зернистость) ---
        float noiseVal = N2(uv * 500.0 + _Time.y * 10.0) * 2.0 - 1.0;
        float noiseIntensity = _Noise * 0.01;
        col.rgb += noiseVal * noiseIntensity;

        // Дополнительный шум от Parasite в поврежденных блоках
        float blockNoiseVal = N2(uv * 200.0 + blockUV * 50.0) * blockMask;
        col.rgb += blockNoiseVal * parasiteIntensity * 0.2;

        // --- Fade (интенсивность эффекта) ---
        // Смешиваем с оригиналом на основе _Fade
        float4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, originalUV);
        
        // Дополнительные артефакты при высоком Fade
        float flicker = 1.0 - (N21(float2(_Time.y * 10.0, 0.0)) * _Fade * 0.1);
        col.rgb *= flicker;

        // Итоговое смешивание
        float4 result = lerp(original, col, saturate(_Fade));

        // Насыщение цвета при высокой Colorisation
        float luminance = Luminance(result.rgb);
        result.rgb = lerp(float3(luminance, luminance, luminance), result.rgb, 0.5 + _Colorisation * 0.5);

        return result;
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