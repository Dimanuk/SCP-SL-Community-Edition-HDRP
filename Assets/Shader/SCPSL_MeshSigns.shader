Shader "SCPSL/MeshSigns"
{
	Properties
	{
		_MainTex ("MainTex", 2D) = "white" {}
		[NoScaleOffset] _MetallicGlossMap ("MetallicGlossMap", 2D) = "white" {}
		[NoScaleOffset] [Normal] _BumpMap ("BumpMap", 2D) = "bump" {}
		_Cutoff ("Mask Clip Value", Range(0, 1)) = 0.5
		[NoScaleOffset] _EmissionMap ("EmissionMap", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		[HDR] _EmissionColor ("EmissionColor", Color) = (0,0,0,1)
		_Metallic ("Metallic", Range(0, 1)) = 0
		_Glossiness ("Glossiness", Range(0, 1)) = 0.5
		_GlossMapScale ("GlossMapScale", Range(0, 1)) = 0.5
		_BumpScale ("BumpScale", Float) = 1
		[Toggle(_USEMETALLICTEXTURE_ON)] _UseMetallicTexture ("Use Metallic Texture", Float) = 0
		[Toggle(_USEEMISSION_ON)] _UseEmission ("Use Emission", Float) = 0
		_939VisionIntensity ("_939VisionIntensity", Float) = 2
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
		[Header(Forward Rendering Options)] [ToggleOff] _SpecularHighlights ("Specular Highlights", Float) = 1
		[ToggleOff] _GlossyReflections ("Reflections", Float) = 1
	}

	SubShader
	{
		Tags { "IGNOREPROJECTOR" = "true" "IsEmissive" = "true" "QUEUE" = "AlphaTest+0" "RenderType" = "TransparentCutout" }
		LOD 300

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff addshadow
		#pragma shader_feature _USEMETALLICTEXTURE_ON
		#pragma shader_feature _USEEMISSION_ON
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF

		sampler2D _MainTex;
		sampler2D _MetallicGlossMap;
		sampler2D _BumpMap;
		sampler2D _EmissionMap;

		half4 _Color;
		half4 _EmissionColor;
		half _Metallic;
		half _Glossiness;
		half _GlossMapScale;
		half _BumpScale;

		// Set globally (not per-material) by SCP-939's dark-vision effect to brighten signs.
		half _939VisionIntensity;
		half4 _939VisionWhite;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed3 albedo = tex.rgb * _Color.rgb;
			albedo *= 1.0 + _939VisionIntensity * _939VisionWhite.rgb;

			o.Albedo = albedo;
			o.Alpha = tex.a;

			#if defined(_USEMETALLICTEXTURE_ON)
			fixed4 metallicGloss = tex2D(_MetallicGlossMap, IN.uv_MainTex);
			o.Metallic = metallicGloss.r;
			o.Smoothness = metallicGloss.a * _GlossMapScale;
			#else
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			#endif

			o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);

			#if defined(_USEEMISSION_ON)
			o.Emission = tex2D(_EmissionMap, IN.uv_MainTex).rgb * _EmissionColor.rgb;
			#endif
		}
		ENDCG
	}

	FallBack "Standard"
}
