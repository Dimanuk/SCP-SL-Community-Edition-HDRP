Shader "Hidden/Custom Effects/Glitch"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float _Glitch;
    float _Noise;

    // Псевдослучайная функция
    float rand(float2 co)
    {
        return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
    }

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float2 uv = i.texcoord;
        
        // Нормализуем glitch из диапазона 0..20 в 0..1
        float intensity = _Glitch * 0.05;
        float t = _Time.y;

        // --- 1. Блочный глитч ---
        float2 blockSize = float2(16.0, 8.0);
        float2 blockId = floor(uv * blockSize);
        float blockRand = rand(blockId + floor(t * 8.0));
        float blockThreshold = 1.0 - intensity * 0.4;
        float blockGlitch = blockRand > blockThreshold ? 1.0 : 0.0;

        // --- 2. Горизонтальный сдвиг строк ---
        float lineId = floor(uv.y * 40.0);
        float lineRand = rand(float2(lineId, t * 6.0));
        float lineShift = (lineRand - 0.5) * intensity * blockGlitch * 0.25;

        // --- 3. Джиттер (мелкое дрожание) ---
        float jitter = (rand(float2(t * 20.0, uv.y * 50.0)) - 0.5) * intensity * 0.015;

        float2 shiftedUV = uv;
        shiftedUV.x += lineShift + jitter;

        // --- 4. RGB-split (хроматическая аберрация) ---
        float rgbSplit = intensity * 0.025 * (1.0 + blockGlitch * 2.0);

        float r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftedUV + float2(rgbSplit, 0.0)).r;
        float g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftedUV).g;
        float b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, shiftedUV - float2(rgbSplit, 0.0)).b;

        float4 color = float4(r, g, b, 1.0);

        // --- 5. Цифровой шум ---
        float noiseVal = rand(uv * t * 50.0 + uv.yx * 20.0) * 2.0 - 1.0;
        color.rgb += noiseVal * _Noise * intensity;

        // --- 6. Сканлайны ---
        float scanline = sin(uv.y * 500.0) * 0.015 * intensity;
        color.rgb -= scanline;

        // --- 7. Инверсия цвета на отдельных блоках ---
        float invert = rand(blockId + t * 3.0) > 0.96 ? 1.0 : 0.0;
        color.rgb = lerp(color.rgb, 1.0 - color.rgb, invert * blockGlitch * intensity);

        return saturate(color);
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