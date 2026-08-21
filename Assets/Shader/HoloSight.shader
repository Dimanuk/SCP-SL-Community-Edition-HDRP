Shader "HoloSight"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGBA)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Emission ("Emission Multiplier", Range(0, 5)) = 1
        
        [Space]
        _RedDotColor ("Red Dot Color (RGB) Brightness (A)", Color) = (1,1,1,1)
        _RedDotTex ("Red Dot Texture (A)", 2D) = "white" {}
        _RedDotSize ("Red Dot Size", Range(0, 30)) = 0
        [Toggle(FIXED_SIZE)] _FixedSize ("Use Fixed Size", Float) = 0
        _RedDotDist ("Red Dot Offset Distance", Range(0, 50)) = 2
        _OffsetX ("Side Offset", Float) = 0
        _OffsetY ("Height Offset", Float) = 0
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
        LOD 200
        
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            
            #pragma shader_feature DIRECTIONAL
            #pragma multi_compile _ FOG_LINEAR
            #pragma shader_feature LIGHTPROBE_SH
            #pragma shader_feature VERTEXLIGHT_ON
            #pragma shader_feature FIXED_SIZE
            
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "UnityStandardUtils.cginc"
            #include "AutoLight.cginc"
            
            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Glossiness;
            float _Metallic;
            float _Emission;
            
            float4 _RedDotColor;
            sampler2D _RedDotTex;
            float _RedDotSize;
            float _RedDotDist;
            float _OffsetX;
            float _OffsetY;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                #if defined(LIGHTPROBE_SH) || defined(VERTEXLIGHT_ON)
                float4 texcoord1 : TEXCOORD1;
                #endif
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 redDotUV : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                #ifdef FOG_LINEAR
                float fogCoord : TEXCOORD4;
                #endif
                #ifdef LIGHTPROBE_SH
                float3 ambient : TEXCOORD5;
                #endif
                #if defined(LIGHTPROBE_SH) && defined(VERTEXLIGHT_ON)
                float3 vertexLights : TEXCOORD6;
                #endif
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                
                float3 worldViewDir = _WorldSpaceCameraPos - o.worldPos;
                float3 objectViewDir = mul(unity_WorldToObject, float4(worldViewDir, 0.0)).xyz;
                
                float2 offsetVertex = v.vertex.xy - float2(_OffsetX, _OffsetY);
                
                float2 projectedView = -objectViewDir.xy * _RedDotDist;
                float2 redDotPos = projectedView + offsetVertex;
                
                float2 redDotUV = redDotPos / _RedDotSize.xx + 0.5;
                o.redDotUV = redDotUV;
                
                #ifdef FOG_LINEAR
                UNITY_TRANSFER_FOG(o, o.pos);
                #endif
                
                #ifdef LIGHTPROBE_SH
                o.ambient = ShadeSH9(float4(o.worldNormal, 1.0));
                
                #ifdef VERTEXLIGHT_ON
                o.vertexLights = Shade4PointLights(
                    unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                    unity_LightColor[0], unity_LightColor[1], 
                    unity_LightColor[2], unity_LightColor[3],
                    unity_4LightAtten0, o.worldPos, o.worldNormal
                );
                #endif
                #endif
                
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                float4 mainTex = tex2D(_MainTex, i.uv);
                float3 albedo = mainTex.rgb * _Color.rgb;
                float alpha = mainTex.a * _Color.a;
                
                float redDotMask = tex2D(_RedDotTex, i.redDotUV).a;
                float4 redDot = redDotMask * _RedDotColor;
                redDot.rgb *= _RedDotColor.a; 

                alpha += redDot.a;
                
                float3 worldNormal = normalize(i.worldNormal);
                float3 worldViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                
                float3 reflectionDir = reflect(-worldViewDir, worldNormal);
                
                float roughness = 1.0 - _Glossiness;
                float perceptualRoughness = roughness * (roughness * 0.7 + 1.7); 
                float mipLevel = perceptualRoughness * 6.0;
                
                float4 envSample0 = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflectionDir);
                float3 envColor = DecodeHDR(envSample0, unity_SpecCube0_HDR);
                
                #if UNITY_SPECCUBE_BLENDING
                float4 envSample1 = UNITY_SAMPLE_TEXCUBE_SAMPLER(unity_SpecCube1, unity_SpecCube0, reflectionDir);
                float3 envColor1 = DecodeHDR(envSample1, unity_SpecCube1_HDR);
                envColor = lerp(envColor1, envColor, unity_SpecCube0_BoxMin.w);
                #endif
                
                float3 specularTint = lerp(0.04, albedo, _Metallic);
                float3 diffuseAlbedo = albedo * (1.0 - _Metallic) * 0.96;
                
                float3 halfDir = normalize(worldLightDir + worldViewDir);
                float NdotL = saturate(dot(worldNormal, worldLightDir));
                float NdotV = abs(dot(worldNormal, worldViewDir));
                float NdotH = saturate(dot(worldNormal, halfDir));
                float LdotH = saturate(dot(worldLightDir, halfDir));
                
                float specularPower = exp2(10 * _Glossiness + 1);
                float specular = pow(NdotH, specularPower) * (_Glossiness * _Glossiness + 0.5);
                
                float fresnel = pow(1.0 - saturate(LdotH), 5);
                float3 fresnelColor = lerp(specularTint, 1.0, fresnel);
                
                float3 directDiffuse = diffuseAlbedo * NdotL * _LightColor0.rgb;
                float3 directSpecular = specular * fresnelColor * _LightColor0.rgb * NdotL;
                
                #ifdef LIGHTPROBE_SH
                float3 ambient = i.ambient;
                #ifdef VERTEXLIGHT_ON
                ambient += i.vertexLights;
                #endif
                #else
                float3 ambient = ShadeSH9(float4(worldNormal, 1.0));
                #endif
                
                float3 indirectDiffuse = diffuseAlbedo * ambient;
                float3 indirectSpecular = envColor * specularTint;
                
                float3 finalColor = directDiffuse + directSpecular + indirectDiffuse + indirectSpecular;
                
                finalColor += redDot.rgb * _Emission;
                
                #ifdef FOG_LINEAR
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                #endif
                
                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
    
    FallBack "Standard"
}