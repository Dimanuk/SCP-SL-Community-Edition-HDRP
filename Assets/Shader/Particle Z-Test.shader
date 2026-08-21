// Restored from the original 12.0.2 build (resources.assets, path_id 3108).
// Binary render state: Blend SrcAlpha One (color+alpha), ColorMask RGB, ZWrite Off,
// Cull Off, zTest = 0 (Disabled) — depth test is explicitly OFF, particles render
// through walls. Verified against shaders with no ZTest line (Standard, Legacy
// Particles/Additive), which all serialize zTest = 4 (LEqual); 0 here is deliberate.
// The compiled shader has NO SOFTPARTICLES_ON variant (keyword absent from the
// binary) — soft-particle depth fade would zero alpha behind walls and defeat
// ZTest Always, so it must not be reintroduced. _InvFade is a dead property.
// Fragment math: 2 * (vertexColor * _TintColor, rgb *= _Glow) * tex.
Shader "Particle Z-Test"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01, 90)) = 1
		_Glow ("Intensity", Range(0, 300)) = 1
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			Blend SrcAlpha One
			ColorMask RGB
			Cull Off
			Lighting Off
			ZWrite Off
			ZTest Always

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			half _Glow;
			float4 _MainTex_ST;

			struct appdata_t
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color * _TintColor;
				o.color.rgb *= _Glow;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = 2.0f * i.color * tex2D(_MainTex, i.texcoord);
				// Additive blend: fog fades toward black, not fog color.
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0, 0, 0, 0));
				return col;
			}
			ENDCG
		}
	}
}
