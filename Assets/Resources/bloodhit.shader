Shader "Hidden/Custom Effects/BloodHit"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_MainTex2, sampler_MainTex2);

    float _LightReflect;
    float _HitLeft;
    float _HitUp;
    float _HitRight;
    float _HitDown;
    float _BloodHitLeft;
    float _BloodHitUp;
    float _BloodHitRight;
    float _BloodHitDown;
    float _HitFull;
    float _BloodHitFull1;
    float _BloodHitFull2;
    float _BloodHitFull3;

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float2 uv = i.texcoordStereo;

        // --- Атлас _MainTex2: 4 квадранта с масками ударов ---
        float2 uvTL = uv * 0.5 + float2(0.0, 0.5); // верх-лево  [Left / BloodLeft / Full]
        float2 uvTR = uv * 0.5 + 0.5;              // верх-право [Up   / BloodUp   / BloodFull1]
        float2 uvBL = uv * 0.5;                    // низ-лево   [Right/ BloodRight/ BloodFull2]
        float2 uvBR = uv * 0.5 + float2(0.5, 0.0); // низ-право  [Down / BloodDown / BloodFull3]

        float4 texTL = SAMPLE_TEXTURE2D(_MainTex2, sampler_MainTex2, uvTL);
        float4 texTR = SAMPLE_TEXTURE2D(_MainTex2, sampler_MainTex2, uvTR);
        float4 texBL = SAMPLE_TEXTURE2D(_MainTex2, sampler_MainTex2, uvBL);
        float4 texBR = SAMPLE_TEXTURE2D(_MainTex2, sampler_MainTex2, uvBR);

        // --- Суммарная интенсивность hit (R=Hit, G=BloodHit, B=Full) ---
        float hit = 0.0;

        // R канал — удары с направлений
        hit += texTL.x * _HitLeft;
        hit += texTR.x * _HitUp;
        hit += texBL.x * _HitRight;
        hit += texBR.x * _HitDown;

        // G канал — кровавые удары с направлений
        hit += texTL.y * _BloodHitLeft;
        hit += texTR.y * _BloodHitUp;
        hit += texBL.y * _BloodHitRight;
        hit += texBR.y * _BloodHitDown;

        // B канал — полноэкранные удары
        hit += texTL.z * _HitFull;
        hit += texTR.z * _BloodHitFull1;
        hit += texBL.z * _BloodHitFull2;
        hit += texBR.z * _BloodHitFull3;

        hit *= _LightReflect;

        // --- Искажение UV экрана (1/16) ---
        float2 distortedUV = uv + hit * 0.0625;

        // --- Семпл экрана со смещением ---
        float3 screen = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV).rgb;

        // --- Кровавое наложение (коэффициенты из декомпиляции) ---
        float bloodR = hit + hit * 0.00390625; // hit * (1 + 1/256) — усиление красного
        float bloodGB = hit * 0.00390625;      // hit / 256 — слабое добавление к GB

        float3 result;
        result.r = screen.r + bloodR;
        result.g = screen.g + bloodGB;
        result.b = screen.b + bloodGB;

        return float4(saturate(result), 1.0);
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