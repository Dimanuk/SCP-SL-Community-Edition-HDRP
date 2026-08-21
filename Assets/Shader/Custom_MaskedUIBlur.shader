Shader "Custom/MaskedUIBlur"
{
	Properties
	{
		_Size ("Blur", Range(0, 30)) = 1
		[HideInInspector] _MainTex ("Masking Texture", 2D) = "white" {}
		_AdditiveColor ("Additive Tint color", Color) = (0,0,0,0)
		_MultiplyColor ("Multiply Tint color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "IgnoreProjector"="True" "Queue"="Transparent" "RenderType"="Opaque" }

		// ---- Pass 1: horizontal blur of the grabbed screen ----
		GrabPass { "_HBlur" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 grabUV : TEXCOORD0;
			};

			sampler2D _HBlur;
			float4 _HBlur_TexelSize;
			float _Size;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.grabUV = ComputeGrabScreenPos(o.pos);
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				float2 uv = i.grabUV.xy / i.grabUV.w;
				float texel = _HBlur_TexelSize.x * _Size;

				half4 sum = half4(0, 0, 0, 0);
				sum += tex2D(_HBlur, uv + float2(-4.0 * texel, 0.0)) * 0.05;
				sum += tex2D(_HBlur, uv + float2(-3.0 * texel, 0.0)) * 0.09;
				sum += tex2D(_HBlur, uv + float2(-2.0 * texel, 0.0)) * 0.12;
				sum += tex2D(_HBlur, uv + float2(-1.0 * texel, 0.0)) * 0.15;
				sum += tex2D(_HBlur, uv) * 0.18;
				sum += tex2D(_HBlur, uv + float2( 1.0 * texel, 0.0)) * 0.15;
				sum += tex2D(_HBlur, uv + float2( 2.0 * texel, 0.0)) * 0.12;
				sum += tex2D(_HBlur, uv + float2( 3.0 * texel, 0.0)) * 0.09;
				sum += tex2D(_HBlur, uv + float2( 4.0 * texel, 0.0)) * 0.05;
				return sum;
			}
			ENDCG
		}

		// ---- Pass 2: vertical blur + mask + tint ----
		GrabPass { "_VBlur" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 grabUV : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};

			sampler2D _VBlur;
			float4 _VBlur_TexelSize;
			sampler2D _MainTex;
			float _Size;
			fixed4 _AdditiveColor;
			fixed4 _MultiplyColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.grabUV = ComputeGrabScreenPos(o.pos);
				o.uv = v.uv;
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				float2 uv = i.grabUV.xy / i.grabUV.w;
				float texel = _VBlur_TexelSize.y * _Size;

				half4 sum = half4(0, 0, 0, 0);
				sum += tex2D(_VBlur, uv + float2(0.0, -4.0 * texel)) * 0.05;
				sum += tex2D(_VBlur, uv + float2(0.0, -3.0 * texel)) * 0.09;
				sum += tex2D(_VBlur, uv + float2(0.0, -2.0 * texel)) * 0.12;
				sum += tex2D(_VBlur, uv + float2(0.0, -1.0 * texel)) * 0.15;
				sum += tex2D(_VBlur, uv) * 0.18;
				sum += tex2D(_VBlur, uv + float2(0.0,  1.0 * texel)) * 0.15;
				sum += tex2D(_VBlur, uv + float2(0.0,  2.0 * texel)) * 0.12;
				sum += tex2D(_VBlur, uv + float2(0.0,  3.0 * texel)) * 0.09;
				sum += tex2D(_VBlur, uv + float2(0.0,  4.0 * texel)) * 0.05;

				half4 col = sum * _MultiplyColor + _AdditiveColor;
				col.a = tex2D(_MainTex, i.uv).a;
				return col;
			}
			ENDCG
		}
	}
	Fallback Off
}
