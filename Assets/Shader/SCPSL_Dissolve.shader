Shader "SCPSL/Dissolve"
{
    Properties
    {
        _Cutoff ("Mask Clip Value", Float) = 0.5
        _MainTex ("MainTex", 2D) = "white" {}
        _BumpMap ("BumpMap", 2D) = "bump" {}
        [NoScaleOffset] _MetallicGlossMap ("MetallicGlossMap", 2D) = "white" {}
        _DisolveGuide ("Disolve Guide", 2D) = "white" {}
        [NoScaleOffset] _EmissionMap ("EmissionMap", 2D) = "white" {}
        _BurnRamp ("Burn Ramp", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Disintegrate ("Disintegrate", Range(0, 1)) = 0
        _Metallic ("Metallic", Range(0, 1)) = 0
        [HDR] _EmissionColor ("EmissionColor", Color) = (0,0,0,1)
        _Glossiness ("Glossiness", Range(0, 1)) = 0.5
        _GlossMapScale ("GlossMapScale", Range(0, 1)) = 0.5
        [Toggle(_USEMETALLICTEXTURE3_ON)] _UseMetallicTexture3 ("Use Metallic Texture", Float) = 0
        _BumpScale ("BumpScale", Float) = 0
        [Toggle(_USEEMISSION1_ON)] _UseEmission1 ("Use Emission", Float) = 0
        [HideInInspector] _texcoord ("", 2D) = "white" {}
        [HideInInspector] __dirty ("", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest+0" "IsEmissive"="true" }
        LOD 300
        Cull Off

        // Surface shader so Unity generates deferred/forward-add/shadow passes.
        // The project renders in Deferred and facility interiors have no
        // directional light; a ForwardBase-only pass renders pure black here.
        CGPROGRAM
        #pragma target 3.0
        #pragma shader_feature _USEMETALLICTEXTURE3_ON
        #pragma shader_feature _USEEMISSION1_ON
        #pragma surface surf Standard keepalpha addshadow fullforwardshadows

        #include "UnityStandardUtils.cginc"

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _MetallicGlossMap;
        sampler2D _DisolveGuide;
        sampler2D _EmissionMap;
        sampler2D _BurnRamp;

        fixed4 _Color;
        fixed4 _EmissionColor;
        half _Cutoff;
        half _Disintegrate;
        half _Metallic;
        half _Glossiness;
        half _GlossMapScale;
        half _BumpScale;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_DisolveGuide;
        };

        void surf (Input i, inout SurfaceOutputStandard o)
        {
            o.Normal = UnpackScaleNormal(tex2D(_BumpMap, i.uv_BumpMap), _BumpScale);

            fixed4 mainColor = tex2D(_MainTex, i.uv_MainTex) * _Color;

            // Dissolve
            half dissolveValue = tex2D(_DisolveGuide, i.uv_DisolveGuide).r;
            half dissolveThreshold = (1.0 - _Disintegrate) * 1.2 - 0.6;
            half dissolveEdge = saturate((dissolveValue + dissolveThreshold) * 8.0 - 4.0);
            clip(dissolveValue + dissolveThreshold - _Cutoff);

            // Burn
            half3 burnColor = tex2D(_BurnRamp, float2(1.0 - dissolveEdge, 0.0)).rgb;
            o.Albedo = mainColor.rgb + burnColor * (1.0 - dissolveEdge);

            // Metallic/Gloss
            half metallic = _Metallic;
            half smoothness = _Glossiness;
            #ifdef _USEMETALLICTEXTURE3_ON
                fixed4 mg = tex2D(_MetallicGlossMap, i.uv_MainTex);
                metallic = mg.r * _Metallic;
                smoothness = mg.a * _GlossMapScale;
            #endif
            o.Metallic = metallic;
            o.Smoothness = smoothness;

            // Emission
            half3 emission = burnColor * (1.0 - dissolveEdge);
            #ifdef _USEEMISSION1_ON
                emission += tex2D(_EmissionMap, i.uv_MainTex).rgb * _EmissionColor.rgb;
            #endif
            o.Emission = emission;

            o.Occlusion = 1.0;
            o.Alpha = mainColor.a;
        }
        ENDCG
    }

    FallBack "Standard"
}
