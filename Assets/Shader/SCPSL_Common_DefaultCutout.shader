Shader "SCPSL/Common/DefaultCutout_Fixed"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _OcclusionMap ("OcclusionMap", 2D) = "white" {}
        [NoScaleOffset] _MetallicGlossMap ("MetallicGlossMap", 2D) = "white" {}
        [NoScaleOffset] [Normal] _BumpMap ("BumpMap", 2D) = "bump" {}
        [NoScaleOffset] _EmissionMap ("EmissionMap", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Mask Clip Value", Float) = 0.5
        [HDR] _EmissionColor ("EmissionColor", Color) = (0,0,0,1)
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Glossiness ("Glossiness", Range(0, 1)) = 0.5
        _GlossMapScale ("GlossMapScale", Range(0, 1)) = 0.5
        _BumpScale ("BumpScale", Float) = 1
        [Toggle(_USEMETALLICTEXTURE_ON)] _UseMetallicTexture ("Use Metallic Texture", Float) = 0
        [Toggle(_USEEMISSION_ON)] _UseEmission ("Use Emission", Float) = 0
        [HideInInspector] _texcoord ("", 2D) = "white" {}
        [HideInInspector] __dirty ("", Float) = 1
    }
    
    SubShader
    {
        Tags { 
            "IGNOREPROJECTOR" = "true" 
            "IsEmissive" = "true" 
            "QUEUE" = "AlphaTest+0" 
            "RenderType" = "Opaque" 
        }
        
        Pass
        {
            Name "FORWARD"
            Tags { 
                "IGNOREPROJECTOR" = "true" 
                "IsEmissive" = "true" 
                "LIGHTMODE" = "FORWARDBASE" 
                "QUEUE" = "AlphaTest+0" 
                "RenderType" = "Opaque" 
                "SHADOWSUPPORT" = "true" 
            }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            
            #pragma shader_feature DIRECTIONAL
            #pragma multi_compile _ FOG_LINEAR
            #pragma multi_compile _ LIGHTPROBE_SH
            #pragma multi_compile _ SHADOWS_SCREEN
            #pragma multi_compile _ VERTEXLIGHT_ON
            #pragma multi_compile _ _USEMETALLICTEXTURE_ON
            
            float4x4 unity_ObjectToWorld;
            float4x4 unity_WorldToObject;
            float4x4 unity_MatrixVP;
            float4x4 unity_MatrixV;
            float4 unity_WorldTransformParams;
            float4 _ProjectionParams;
            
            float3 _WorldSpaceCameraPos;
            float4 _WorldSpaceLightPos0;
            float4 _LightColor0;
            float4 _LightShadowData;
            float4 unity_ShadowFadeCenterAndType;
            
            float4 unity_SHAr;
            float4 unity_SHAg;
            float4 unity_SHAb;
            float4 unity_SHBr;
            float4 unity_SHBg;
            float4 unity_SHBb;
            float4 unity_SHC;
            
            float4 unity_SpecCube0_BoxMax;
            float4 unity_SpecCube0_BoxMin;
            float4 unity_SpecCube0_ProbePosition;
            float4 unity_SpecCube0_HDR;
            float4 unity_SpecCube1_BoxMax;
            float4 unity_SpecCube1_BoxMin;
            float4 unity_SpecCube1_ProbePosition;
            float4 unity_SpecCube1_HDR;
            
            float4 _texcoord_ST;
            float4 _MainTex_ST;
            float4 _OcclusionMap_ST;
            float _BumpScale;
            float4 _Color;
            float _Metallic;
            float _Glossiness;
            float _GlossMapScale;
            float _Cutoff;
            float4 _EmissionColor;
            
            Texture2D<float4> _BumpMap;
            SamplerState sampler_BumpMap;
            Texture2D<float4> _MainTex;
            SamplerState sampler_MainTex;
            Texture2D<float4> _MetallicGlossMap;
            SamplerState sampler_MetallicGlossMap;
            Texture2D<float4> _OcclusionMap;
            SamplerState sampler_OcclusionMap;
            Texture2D<float4> _ShadowMapTexture;
            SamplerState sampler_ShadowMapTexture;
            TextureCube<float4> unity_SpecCube0;
            SamplerState samplerunity_SpecCube0;
            TextureCube<float4> unity_SpecCube1;
            Texture2D<float4> _EmissionMap;
            
            float4 ComputeScreenPos(float4 pos)
            {
                float4 o = pos * 0.5;
                o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
                o.zw = pos.zw;
                return o;
            }
            
            float3 DecodeHDR(float4 data, float4 decodeInstructions)
            {
                float alpha = decodeInstructions.w * (data.a - 1.0) + 1.0;
                return (data.rgb * alpha) * decodeInstructions.x;
            }
            
            float3 UnityObjectToWorldNormal(float3 norm)
            {
                return normalize(mul((float3x3)unity_ObjectToWorld, norm));
            }
            
            float3 UnityObjectToWorldDir(float3 dir)
            {
                return normalize(mul((float3x3)unity_ObjectToWorld, dir));
            }
            
            float3 UnityWorldSpaceViewDir(float3 worldPos)
            {
                return normalize(_WorldSpaceCameraPos - worldPos);
            }
            
            float3 ShadeSH9(float4 normal)
            {
                float3 x1, x2, x3;
                
                x1.r = dot(unity_SHAr, normal);
                x1.g = dot(unity_SHAg, normal);
                x1.b = dot(unity_SHAb, normal);
                
                float4 vB = normal.xyzz * normal.yzzx;
                x2.r = dot(unity_SHBr, vB);
                x2.g = dot(unity_SHBg, vB);
                x2.b = dot(unity_SHBb, vB);

                float vC = normal.x * normal.x - normal.y * normal.y;
                x3 = unity_SHC.rgb * vC;
                
                return x1 + x2 + x3;
            }
            
            float3 DisneyDiffuse(float NdotV, float NdotL, float LdotH, float roughness, float3 baseColor)
            {
                float fd90 = 0.5 + 2.0 * LdotH * LdotH * roughness;
                float lightScatter = 1.0 + (fd90 - 1.0) * pow(1.0 - NdotL, 5);
                float viewScatter = 1.0 + (fd90 - 1.0) * pow(1.0 - NdotV, 5);
                return baseColor * lightScatter * viewScatter;
            }
            
            float SmithJointGGXVisibilityTerm(float NdotL, float NdotV, float roughness)
            {
                float a2 = roughness * roughness;
                float lambdaV = NdotL * (NdotV * (1.0 - a2) + a2);
                float lambdaL = NdotV * (NdotL * (1.0 - a2) + a2);
                return 0.5 / (lambdaV + lambdaL + 1e-5);
            }
            
            float GGXTerm(float NdotH, float roughness)
            {
                float a2 = roughness * roughness;
                float d = (NdotH * a2 - NdotH) * NdotH + 1.0;
                return a2 / (d * d * 3.14159265);
            }
            
            float3 FresnelTerm(float3 F0, float cosA)
            {
                return F0 + (1.0 - F0) * pow(1.0 - cosA, 5);
            }
            
            float3 FresnelLerp(float3 F0, float3 F90, float cosA)
            {
                float t = pow(1.0 - cosA, 5);
                return lerp(F0, F90, t);
            }
            
            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                float4 texcoord3 : TEXCOORD3;
                float4 color : COLOR;
            };
            
            struct VertexOutput
            {
                float2 uv : TEXCOORD0;
                float4 TtoW0 : TEXCOORD1;
                float4 TtoW1 : TEXCOORD2;
                float4 TtoW2 : TEXCOORD3;
                
                #if defined(LIGHTPROBE_SH)
                float3 sh : TEXCOORD4;
                #endif
                
                #if defined(SHADOWS_SCREEN)
                float4 shadowCoord : TEXCOORD6;
                #endif
                
                float4 pos : SV_POSITION;
            };
            
            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(unity_MatrixVP, worldPos);
                o.uv = v.texcoord.xy * _texcoord_ST.xy + _texcoord_ST.zw;
                
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w * unity_WorldTransformParams.w;
                
                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
                
                #if defined(LIGHTPROBE_SH)
                float3 worldNorm = worldNormal;
                o.sh = ShadeSH9(float4(worldNorm, 1.0));
                #endif
                
                #if defined(SHADOWS_SCREEN)
                o.shadowCoord = ComputeScreenPos(o.pos);
                #endif
                
                return o;
            }
            
            float4 frag(VertexOutput i) : SV_Target
            {
                // Sample textures
                float2 uv = i.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                
                float4 mainTex = _MainTex.Sample(sampler_MainTex, uv);
                float3 albedo = mainTex.rgb * _Color.rgb;
                float alpha = mainTex.a;
                
                // Alpha test
                clip(alpha - _Cutoff);
                
                // Normal map
                float3 bump = _BumpMap.Sample(sampler_BumpMap, uv).xyz;
                bump.xy = bump.xy * 2.0 - 1.0;
                bump.xy *= _BumpScale;
                bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
                
                float3 worldNormal;
                worldNormal.x = dot(i.TtoW0.xyz, bump);
                worldNormal.y = dot(i.TtoW1.xyz, bump);
                worldNormal.z = dot(i.TtoW2.xyz, bump);
                worldNormal = normalize(worldNormal);
                
                // Material properties
                float metallic, smoothness;
                #if defined(_USEMETALLICTEXTURE_ON)
                float4 metallicGloss = _MetallicGlossMap.Sample(sampler_MetallicGlossMap, uv);
                metallic = metallicGloss.r;
                smoothness = metallicGloss.a * _GlossMapScale;
                #else
                metallic = _Metallic;
                smoothness = _Glossiness;
                #endif
                
                float occlusion = _OcclusionMap.Sample(sampler_OcclusionMap, i.uv * _OcclusionMap_ST.xy + _OcclusionMap_ST.zw).r;
                
                // PBR calculations
                float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
                float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDir = normalize(lightDir + viewDir);
                
                float NdotL = saturate(dot(worldNormal, lightDir));
                float NdotV = abs(dot(worldNormal, viewDir));
                float NdotH = saturate(dot(worldNormal, halfDir));
                float LdotH = saturate(dot(lightDir, halfDir));
                
                float perceptualRoughness = 1.0 - smoothness;
                float roughness = perceptualRoughness * perceptualRoughness;
                roughness = max(roughness, 0.002);
                
                // Diffuse and Specular colors
                float3 specularColor = lerp(0.04, albedo, metallic);
                float3 diffuseColor = albedo * (1.0 - metallic);
                
                // Direct lighting
                float3 lightColor = _LightColor0.rgb;
                
                #if defined(SHADOWS_SCREEN)
                float2 shadowUV = i.shadowCoord.xy / i.shadowCoord.w;
                float shadowAtten = _ShadowMapTexture.Sample(sampler_ShadowMapTexture, shadowUV).r;
                
                // Shadow fade
                float3 viewZ = mul(unity_MatrixV, float4(worldPos, 1.0)).xyz;
                float shadowFade = saturate(length(viewZ) * _LightShadowData.z + _LightShadowData.w);
                shadowAtten = lerp(shadowAtten, 1.0, shadowFade);
                lightColor *= shadowAtten;
                #endif
                
                // Specular term
                float V = SmithJointGGXVisibilityTerm(NdotL, NdotV, roughness);
                float D = GGXTerm(NdotH, roughness);
                float3 F = FresnelTerm(specularColor, LdotH);
                
                float3 specularTerm = V * D * 3.14159265 * F * NdotL;
                specularTerm = max(specularTerm, 0.0);
                
                // Diffuse term
                float3 diffuseTerm = DisneyDiffuse(NdotV, NdotL, LdotH, roughness, diffuseColor) * NdotL;
                
                // Direct light result
                float3 directLighting = (diffuseTerm + specularTerm) * lightColor;
                
                // Indirect lighting (IBL)
                float3 reflectDir = reflect(-viewDir, worldNormal);
                float3 indirectDiffuse = 0.0;
                float3 indirectSpecular = 0.0;
                
                #if defined(LIGHTPROBE_SH)
                indirectDiffuse = i.sh;
                #endif
                
                // Reflection probe sampling
                float4 envColor0 = unity_SpecCube0.SampleLevel(samplerunity_SpecCube0, reflectDir, perceptualRoughness * 6.0);
                float3 env0 = DecodeHDR(envColor0, unity_SpecCube0_HDR);
                
                #if defined(UNITY_SPECCUBE_BLENDING)
                float4 envColor1 = unity_SpecCube1.SampleLevel(samplerunity_SpecCube0, reflectDir, perceptualRoughness * 6.0);
                float3 env1 = DecodeHDR(envColor1, unity_SpecCube1_HDR);
                env0 = lerp(env1, env0, unity_SpecCube0_BoxMin.w);
                #endif
                
                indirectSpecular = env0 * occlusion;
                
                // Indirect diffuse
                float3 kd = (1.0 - F) * (1.0 - metallic);
                indirectDiffuse = indirectDiffuse * diffuseColor * kd;
                
                // Indirect specular
                float surfaceReduction = 1.0 / (roughness * roughness + 1.0);
                float grazingTerm = saturate(smoothness + (1.0 - metallic));
                indirectSpecular *= surfaceReduction * FresnelLerp(specularColor, grazingTerm, NdotV);
                
                float3 indirectLighting = indirectDiffuse + indirectSpecular;
                
                // Final color
                float3 finalColor = directLighting + indirectLighting;
                
                // Emission
                #if defined(_USEEMISSION_ON)
                float3 emission = _EmissionMap.Sample(sampler_MainTex, uv).rgb * _EmissionColor.rgb;
                finalColor += emission;
                #endif
                
                return float4(finalColor, 1.0);
            }
            
            ENDHLSL
        }
    }
}