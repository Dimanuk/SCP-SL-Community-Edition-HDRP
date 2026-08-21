Shader "SCPSL/LiquidHDRP"
{
    Properties
    {
        _Fill ("Fill", Float) = 0
        [HDR] _SideColor ("SideColor", Color) = (0.4, 0.4745098, 0.4, 1)
        _WobbleZ ("WobbleZ", Float) = 0
        _WobbleX ("WobbleX", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="AlphaTest" "RenderPipeline"="HDRenderPipeline" }
        LOD 200

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            // Базовые библиотеки HDRP (без Material/Unlit)
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            float _Fill;
            float _WobbleX;
            float _WobbleZ;
            float4 _SideColor;

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos   : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                // Преобразование объект → мир
                float3 worldPos = TransformObjectToWorld(input.positionOS);
                // Мир → клип (HDRP использует общие функции)
                output.positionCS = TransformWorldToHClip(worldPos);
                output.worldPos = worldPos;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // Преобразование мировых координат в объектные (обратная матрица объекта)
                float3 objectPos = mul(GetWorldToObjectMatrix(), float4(input.worldPos, 1)).xyz;

                // Вычисление волны (полный аналог оригинала)
                float wobbleX = objectPos.x * _WobbleZ;
                float wobbleZ = dot(float2(-0.4480736255645751953125, -0.893996655941009521484375), objectPos.yz);
                wobbleX = (_WobbleX * wobbleZ) + wobbleX;

                // Опорная Y-координата объекта в мировом пространстве (pivotY)
                float3 objectWorldPos = GetObjectToWorldMatrix()._m03_m13_m23;
                float pivotY = objectWorldPos.y;

                // Проверка высоты с учётом колебаний
                float heightCheck = (input.worldPos.y - pivotY) + wobbleX;
                clip(_Fill - heightCheck);

                // Расчёт эмиссии (как в оригинале)
                float peak = max(_SideColor.r, max(_SideColor.g, _SideColor.b));
                float3 col = (_SideColor.rgb / max(peak, 1.0)) * 0.01;

                return float4(col, 1.0);
            }
            ENDHLSL
        }
    }
}