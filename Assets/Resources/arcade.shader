Shader "Hidden/Custom Effects/Arcade"
{
    Properties
    {
        _Fade ("Fade", Float) = 0.0
    }

    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float _Fade;

    struct Attributes
    {
        uint vertexID : SV_VertexID;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        
        // Генерация fullscreen-треугольника (id: 0,1,2)
        float2 uv = float2(
            (input.vertexID == 1) ? 2.0 : 0.0,
            (input.vertexID == 2) ? 2.0 : 0.0
        );
        
        output.positionCS = float4(uv * 2.0 - 1.0, 0.0, 1.0);
        
        #if UNITY_UV_STARTS_AT_TOP
        uv.y = 1.0 - uv.y;
        #endif
        
        output.texcoord = uv;
        return output;
    }

    float4 Frag(Varyings input) : SV_Target
    {
        float2 uv = input.texcoord;
        float2 screenSize = _ScreenParams.xy;
        
        // 1. Пикселизация (блочное разрешение как на старых аркадах)
        float pixelSize = 1.0; // чем больше, тем крупнее "пиксели"
        float2 pixelatedUV = floor(uv * screenSize / pixelSize) * pixelSize / screenSize;
        
        // 2. Хроматическая аберрация по краям экрана (сдвиг R и B каналов)
        float2 center = uv - 0.5;
        float dist = length(center);
        float2 dir = normalize(center + 0.0001);
        float aberration = dist * dist * 0.03;
        
        float4 color;
        color.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pixelatedUV + dir * aberration).r;
        color.g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pixelatedUV).g;
        color.b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pixelatedUV - dir * aberration).b;
        color.a = 1.0;
        
        // 3. Scanlines (горизонтальные линии)
        float scanline = sin(uv.y * screenSize.y * 0.7) * 0.5 + 0.5;
        scanline = lerp(0.82, 1.0, scanline); // интенсивность линий
        color.rgb *= scanline;
        
        // 4. Виньетка (затемнение по краям)
        float vignette = 1.0 - dist * 1.6;
        vignette = saturate(vignette);
        color.rgb *= vignette;
        
        // 5. Ретро-цветокоррекция (контраст + насыщенность)
        color.rgb = saturate(color.rgb * 1.15);
        color.rgb = pow(color.rgb, 0.95);
        
        // Смешиваем с оригиналом через параметр Fade из C#
        float4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        return lerp(original, color, _Fade);
    }

    ENDHLSL

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}