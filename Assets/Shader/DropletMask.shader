// Restored from the original 12.0.2 build (resources.assets, path_id 3131).
// Binary structure: GrabPass + one pass with Blend SrcAlpha OneMinusSrcAlpha and
// material-driven states: ZTest [_ZTestMode], ZWrite [_ZWriteMode], Cull [_CullMode]
// (the rip hardcoded "ZWrite Off / Cull Off" and dropped ZTest entirely).
// Fragment math (from the ripped dump, kept verbatim):
//   offset.x = (n.a * n.r * 2 - 1) * _Intensity, offset.y = (n.g * 2 - 1) * _Intensity
//   rgb = grab(grabPos.xy / grabPos.w + offset * vertexColor.xy)
//   a   = alphaMask.a * vertexColor.a * _Fade
Shader "DropletMask"
{
	Properties
	{
		[Enum(Off,0,Front,1,Back,2)] _CullMode ("Culling", Float) = 2
		[Toggle] _ZWriteMode ("Write to Z-buffer", Float) = 1
		[Enum(Less,0,Greater,1,LEqual,2,GEqual,3,Equal,4,NotEqual,5,Always,6)] _ZTestMode ("Depth Test", Float) = 6
		_AlphaMask ("Alpha Mask", 2D) = "white" {}
		_Normal ("Normal", 2D) = "bump" {}
		_Intensity ("Intensity", Float) = 1
		_Fade ("Opacity", Range(0, 1)) = 1
		[HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		GrabPass { }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull [_CullMode]
			ZWrite [_ZWriteMode]
			ZTest [_ZTestMode]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			sampler2D _GrabTexture;
			sampler2D _Normal;
			sampler2D _AlphaMask;
			float4 _Normal_ST;
			float4 _AlphaMask_ST;
			float _Intensity;
			float _Fade;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				float4 grabPos : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				o.grabPos = ComputeGrabScreenPos(o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				half4 n = tex2D(_Normal, TRANSFORM_TEX(i.texcoord, _Normal));

				// Same unpack as the binary: x from a*r (DXT5nm-style), y from g.
				half2 offset;
				offset.x = (n.a * n.r * 2.0h - 1.0h) * _Intensity;
				offset.y = (n.g * 2.0h - 1.0h) * _Intensity;

				// Per-vertex color.xy scales the distortion, grab UV is perspective-divided.
				float2 grabUV = i.grabPos.xy / i.grabPos.w + offset * i.color.xy;
				fixed3 rgb = tex2D(_GrabTexture, grabUV).rgb;

				fixed a = tex2D(_AlphaMask, TRANSFORM_TEX(i.texcoord, _AlphaMask)).a * i.color.a * _Fade;
				return fixed4(rgb, a);
			}
			ENDCG
		}
	}
}
