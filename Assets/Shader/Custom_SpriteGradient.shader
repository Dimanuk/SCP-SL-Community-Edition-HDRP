Shader "Custom/SpriteGradient"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "black" {}
        _Color  ("Left Color",  Color) = (1,1,1,1)
        _Color2 ("Right Color", Color) = (1,1,1,1)
        _Start  ("Start",       Float) = 1
    }

    SubShader
    {
        LOD 100
        Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" }

        Pass
        {
            LOD 100
            Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0

            // Unity built-ins
            float4x4 unity_ObjectToWorld;
            float4x4 unity_MatrixVP;

            // Material properties
            float4 _Color;
            float4 _Color2;
            float  _Start;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 color : COLOR;
                float4 pos   : SV_Position;
            };

            v2f vert(appdata v)
            {
                v2f o;

                // Object → World → Clip
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(unity_MatrixVP, worldPos);

                // Gradient: lerp between premultiplied _Color and _Color2
                // using UV.x * _Start as the blend factor
                float  t  = v.uv.x * _Start;
                float4 c1 = _Color  * _Color.a;
                float4 c2 = _Color2 * _Color2.a;
                o.color = lerp(c1, c2, t);

                return o;
            }

            float4 frag(v2f i) : SV_Target0
            {
                return i.color;
            }

            ENDHLSL
        }
    }
}