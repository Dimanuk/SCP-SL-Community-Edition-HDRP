Shader "Hidden/Custom Effects/CameraShake"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    float4 _ScanLineJitter;
    float4 _VerticalJump;
    float _HorizontalShake;
    float4 _ColorDrift;

    // Pseudo-random
    float N21(float2 p)
    {
        float3 p3 = frac(float3(p.xyx) * float3(0.1031, 0.1030, 0.0973));
        p3 += dot(p3, p3.yzx + 33.33);
        return frac((p3.x + p3.y) * p3.z);
    }

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float2 uv = i.texcoord;

        // --- Scan Line Jitter ---
        float jitterAmount = _ScanLineJitter.x;
        float jitterThreshold = _ScanLineJitter.y;
        
        // Случайное смещение строк по X
        float lineJitter = N21(float2(0.0, floor(uv.y * 240.0) + _Time.y)) * 2.0 - 1.0;
        float lineMask = step(jitterThreshold, N21(float2(_Time.y * 0.5, floor(uv.y * 240.0))));
        uv.x += lineJitter * lineMask * jitterAmount * 0.5;

        // --- Vertical Jump ---
        float jumpTime = _VerticalJump.x;
        float jumpAmount = _VerticalJump.y;
        
        // Скачок изображения по Y (периодический)
        float jumpOffset = frac(jumpTime) * jumpAmount * 0.25;
        uv.y = frac(uv.y + jumpOffset);

        // --- Horizontal Shake ---
        float shake = _HorizontalShake;
        
        // Дрожание всего изображения по X
        float shakeOffset = (N21(float2(_Time.y * 20.0, 0.0)) * 2.0 - 1.0) * shake;
        uv.x += shakeOffset;

        // --- Color Drift (RGB Split) ---
        float driftTime = _ColorDrift.x;
        float driftAmount = _ColorDrift.y;

        // Смещение каналов по синусу от времени
        float driftOffset = sin(driftTime) * driftAmount;

        float4 colorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(driftOffset, 0.0));
        float4 colorG = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        float4 colorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - float2(driftOffset, 0.0));

        float4 col = float4(colorR.r, colorG.g, colorB.b, 1.0);

        // Дополнительный scanline эффект (тонкие полосы яркости)
        float scanline = sin(uv.y * 800.0 + _Time.y * 10.0) * 0.04;
        col.rgb += scanline * jitterAmount;

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