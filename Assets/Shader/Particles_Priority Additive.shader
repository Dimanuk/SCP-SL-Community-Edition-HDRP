Shader "Particles/Priority Additive"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01, 3)) = 1
    }
    SubShader
    {
        Tags { "IgnoreProjector"="True" "Queue"="Transparent+1" "RenderType"="Transparent" }
        Pass
        {
            Tags { "IgnoreProjector"="True" "Queue"="Transparent+1" "RenderType"="Transparent" }
            Blend SrcAlpha One, SrcAlpha One
            ColorMask RGB
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _ FOG_LINEAR
            #pragma multi_compile _ SOFTPARTICLES_ON

            #include "UnityCG.cginc"

            fixed4 _TintColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _InvFade;

            #ifdef SOFTPARTICLES_ON
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            #endif

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                #ifdef SOFTPARTICLES_ON
                float4 projPos : TEXCOORD1;
                #endif
                #ifdef FOG_LINEAR
                UNITY_FOG_COORDS(2)
                #endif
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _TintColor;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                #ifdef SOFTPARTICLES_ON
                o.projPos = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif

                #ifdef FOG_LINEAR
                UNITY_TRANSFER_FOG(o, o.pos);
                #endif

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = i.color;

                #ifdef SOFTPARTICLES_ON
                float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                float partZ = i.projPos.z;
                float fade = saturate(_InvFade * (sceneZ - partZ));
                col.a *= fade;
                #endif

                col *= tex2D(_MainTex, i.texcoord);
                col *= 2.0;

                #ifdef FOG_LINEAR
                UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));
                #endif

                return col;
            }
            ENDCG
        }
    }
}
