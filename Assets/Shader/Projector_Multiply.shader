Shader "Projector/Multiply"
{
    Properties
    {
        _ShadowTex ("Cookie", 2D) = "gray" {}
        _FalloffTex ("FallOff", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Pass
        {
            Tags { "Queue"="Transparent" }
            Blend DstColor Zero
            ColorMask RGB
            ZWrite Off
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _ FOG_LINEAR

            #include "UnityCG.cginc"

            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;
            sampler2D _ShadowTex;
            sampler2D _FalloffTex;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uvShadow : TEXCOORD0;
                float4 uvFalloff : TEXCOORD1;
                #ifdef FOG_LINEAR
                UNITY_FOG_COORDS(2)
                #endif
            };

            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.uvShadow = mul(unity_Projector, vertex);
                o.uvFalloff = mul(unity_ProjectorClip, vertex);
                #ifdef FOG_LINEAR
                UNITY_TRANSFER_FOG(o, o.pos);
                #endif
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvShadow = i.uvShadow.xy / i.uvShadow.w;
                float2 uvFalloff = i.uvFalloff.xy / i.uvFalloff.w;

                fixed4 texS = tex2D(_ShadowTex, uvShadow);
                texS = texS * fixed4(1,1,1,-1) + fixed4(-1,-1,-1,1);

                fixed falloff = tex2D(_FalloffTex, uvFalloff).a;

                fixed4 res = falloff * texS + fixed4(1,1,1,0);

                #ifdef FOG_LINEAR
                UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(1,1,1,1));
                #endif

                return res;
            }
            ENDCG
        }
    }
}