Shader "HoloSight/HDRP17_3"
{
    Properties
    {
        [MainColor] _Color ("Color", Color) = (1,1,1,1)
        [MainTexture] _MainTex ("Albedo (RGBA)", 2D) = "white" {}

        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0
        [HDR] _Emission ("Emission Multiplier", Range(0, 5)) = 1

        [Space]
        [HDR] _RedDotColor ("Red Dot Color (RGB) Brightness (A)", Color) = (1,1,1,1)
        _RedDotTex ("Red Dot Texture (A)", 2D) = "white" {}
        _RedDotSize ("Red Dot Size", Range(0.001, 30)) = 0.1
        [Toggle(FIXED_SIZE)] _FixedSize ("Use Fixed Size", Float) = 0
        _RedDotDist ("Red Dot Offset Distance", Range(0, 50)) = 2
        _OffsetX ("Side Offset", Float) = 0
        _OffsetY ("Height Offset", Float) = 0

        [HDR] _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularIntensity ("Specular Intensity", Range(0, 8)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "HDRenderPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        LOD 200
        Cull Back
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "Forward" }

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #pragma shader_feature_local_fragment FIXED_SIZE

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariablesFunctions.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_RedDotTex);
            SAMPLER(sampler_RedDotTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _MainTex_ST;

                float _Glossiness;
                float _Metallic;
                float _Emission;

                float4 _RedDotColor;
                float _RedDotSize;
                float _RedDotDist;
                float _OffsetX;
                float _OffsetY;

                float4 _SpecularColor;
                float _SpecularIntensity;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float2 redDotUV   : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float3 normalWS   : TEXCOORD3;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 positionWS = TransformObjectToWorld(input.positionOS);
                float3 normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));

                output.positionWS = positionWS;
                output.normalWS = normalWS;
                output.positionCS = TransformWorldToHClip(positionWS);

                output.uv = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;

                float3 cameraWS = GetCameraPositionWS();
                float3 worldViewDir = cameraWS - positionWS;
                float3 objectViewDir =
                    mul((float3x3)GetWorldToObjectMatrix(), worldViewDir);

                float2 offsetVertex = input.positionOS.xy -
                                      float2(_OffsetX, _OffsetY);

                float2 projectedView = -objectViewDir.xy * _RedDotDist;
                float2 redDotPos = projectedView + offsetVertex;

                #if defined(FIXED_SIZE)
                    float viewDistance = max(length(worldViewDir), 0.001);
                    float fixedScale = max(_RedDotSize * 0.01 * viewDistance, 0.001);
                    output.redDotUV = redDotPos / fixedScale.xx + 0.5;
                #else
                    output.redDotUV = redDotPos / max(_RedDotSize, 0.001).xx + 0.5;
                #endif

                return output;
            }

            float3 FresnelSchlick(float cosTheta, float3 F0)
            {
                return F0 + (1.0 - F0) * pow(1.0 - saturate(cosTheta), 5.0);
            }

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 mainTex = SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex, input.uv);

                float3 albedo = mainTex.rgb * _Color.rgb;
                float alpha = mainTex.a * _Color.a;

                float redDotMask = SAMPLE_TEXTURE2D(
                    _RedDotTex, sampler_RedDotTex, input.redDotUV).a;

                float3 redDotRGB = redDotMask * _RedDotColor.rgb * _RedDotColor.a;
                float redDotAlpha = redDotMask * _RedDotColor.a;

                alpha = saturate(alpha + redDotAlpha);

                float3 N = normalize(input.normalWS);
                float3 V = normalize(GetCameraPositionWS() - input.positionWS);

                float3 L = normalize(V + N * 0.35);
                float3 H = normalize(L + V);

                float NdotL = saturate(dot(N, L));
                float NdotV = saturate(dot(N, V));
                float NdotH = saturate(dot(N, H));

                float roughness = max(1.0 - _Glossiness, 0.045);
                float smoothPower = lerp(8.0, 256.0, _Glossiness * _Glossiness);

                float3 F0 = lerp(
                    float3(0.04, 0.04, 0.04),
                    albedo,
                    saturate(_Metallic));

                F0 *= _SpecularColor.rgb;

                float3 F = FresnelSchlick(NdotV, F0);
                float specular = pow(NdotH, smoothPower) * NdotL;

                float3 diffuse = albedo * lerp(0.35, 0.08, _Metallic) * NdotL;
                float3 spec = F * specular * _SpecularIntensity;

                float3 finalColor = diffuse + spec;

                finalColor += redDotRGB * _Emission;

                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
    }

    CustomEditor "UnityEditor.Rendering.HighDefinition.HDShaderGUI"
}
