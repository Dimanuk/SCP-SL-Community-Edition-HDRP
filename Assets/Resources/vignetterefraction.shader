Shader "Hidden/Custom Effects/VignetteRefraction"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_RefractionTex, sampler_RefractionTex);

    float4 _Values;    // x=Intensity, y=RefractionPower, z=Frosting, w=Fade
    float4 _Color1;    // ColorStart
    float4 _Color2;    // ColorEnd

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float2 uv = i.texcoordStereo;

        // --- Расстояние от центра экрана ---
        float2 centerOffset = uv - 0.5;
        float dist = length(centerOffset);

        // --- Текстура преломления ---
        float4 refrTex = SAMPLE_TEXTURE2D(_RefractionTex, sampler_RefractionTex, uv);

        // --- Базовая кривая виньетки ---
        // Смесь расстояния (×1.31415927) и R канала refraction по RefractionPower
        float distScaled = dist * 1.31415927;
        float vignetteRaw = lerp(distScaled, refrTex.x, _Values.y);

        // --- Фактор для цвета виньетки ---
        float vColorFactor = (vignetteRaw - (1.0 - _Values.x)) / (1.0 - _Values.z);
        vColorFactor = saturate(vColorFactor);

        // --- Цвет виньетки (Color1 → Color2) ---
        float3 vColor = lerp(_Color1.rgb, _Color2.rgb, vColorFactor);

        // --- UV искажение (точная формула из декомпиляции) ---
        float2 distortedUV = uv + refrTex.xy * (uv * (refrTex.xy - 1.0));

        // --- Экран с преломлением ---
        float4 screen = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);

        // --- Добавляем цвет виньетки к экрану ---
        float3 tinted = screen.rgb + vColor;

        // --- Факторы смешивания ---
        float factorX = (vignetteRaw + _Values.z * _Values.x - 1.0) / _Values.w;
        factorX = saturate(factorX);

        float factorY = (vignetteRaw + _Values.x - 1.0) / _Values.w;
        factorY = saturate(factorY);

        // --- Осветление к белому по factorX ---
        float3 blended = lerp(tinted, float3(1.0, 1.0, 1.0), factorX);

        // --- Оригинальный экран ---
        float4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

        // --- Финальное смешивание с оригиналом по factorY ---
        float3 final = lerp(original.rgb, blended, factorY);

        return float4(final, original.a);
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