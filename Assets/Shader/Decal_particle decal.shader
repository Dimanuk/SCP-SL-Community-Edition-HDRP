Shader "Decal/particle decal"
{
    Properties
    {
        [HDR] _TintColor ("Tint Color", Color) = (1,1,1,1)
        _Texture ("Texture", 2D) = "white" {}
        _scale ("Scale", Float) = 1
        _range ("Range", Range(0, 50)) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Blend Mode Destination", Float) = 10
    }

    SubShader
    {
        LOD 100
        Tags { "Queue" = "Transparent-1" "RenderType" = "Transparent" }

        Pass
        {
            LOD 100
            Tags { "Queue" = "Transparent-1" "RenderType" = "Transparent" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0

            #include "UnityCG.cginc"

            float4 _TintColor;
            sampler2D _Texture;
            float4 _Texture_ST;
            float _scale;
            float _range;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 color : COLOR;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 viewRay : TEXCOORD2;
                float4 particleColor : TEXCOORD3;
                float4 scaleRot : TEXCOORD4;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                o.worldPos = v.texcoord0.xyz;
                o.viewRay = float3(0,0,1);
                o.particleColor = v.texcoord1;
                o.scaleRot = v.texcoord2;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // ПРОСТО: покажем текстуру на UV = screenPos
                float2 uv = i.screenPos.xy / i.screenPos.w;
                float4 tex = tex2D(_Texture, uv);
                return tex * i.particleColor * _TintColor;
            }

            ENDHLSL
        }
    }
}