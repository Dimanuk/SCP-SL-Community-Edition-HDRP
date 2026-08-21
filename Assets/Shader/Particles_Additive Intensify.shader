Shader "Particles/Additive Intensify"
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
        Tags { "IgnoreProjector"="True" "Queue"="Transparent" "RenderType"="Overlay" }
        Pass
        {
            Tags { "IgnoreProjector"="True" "Queue"="Transparent" "RenderType"="Overlay" }
            Blend SrcAlpha One
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
            float _Glow;

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

                // Тинт * вершинный цвет; интенсивность _Glow множит ТОЛЬКО RGB
                // (альфа не усиливается — как в оригинале: color.rgb * _Glow, color.a без изменений).
                fixed4 c = v.color * _TintColor;
                o.color = fixed4(c.rgb * _Glow, c.a);

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

                // Оригинал: 2 * color * tex (удвоение яркости).
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
