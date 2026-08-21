Shader "Hidden/UnlitShaderReplacement"
{
    // Замена-шейдер для прицела (ScopeShaderReplacement: camera.SetReplacementShader(.., "RenderType")).
    // Рендерит геометрию сцены без освещения тремя вариантами по RenderType (Opaque/Cutout/Transparent).
    // Чистая реконструкция вместо рипа (4209 строк с дублирующимися программ-блоками не компилировались).
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Mask Clip Value", Float) = 0.5
        _EmissionMap ("Emission", 2D) = "white" {}
        _EmissionColor ("Color", Color) = (0,0,0,1)
    }

    CGINCLUDE
    #pragma vertex vert
    #pragma fragment frag
    #pragma target 3.0
    #pragma multi_compile _ _DARKMODE
    #pragma multi_compile _ _EMISSION
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_ST;
    fixed4 _Color;
    float _Cutoff;
    sampler2D _EmissionMap;
    fixed4 _EmissionColor;

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
    };

    v2f vert (appdata v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        return o;
    }

    fixed4 frag (v2f i) : SV_Target
    {
        fixed4 tex = tex2D(_MainTex, i.uv);

        #ifdef _DARKMODE
            // Тёмный режим прицела: геометрия чёрная, светится только эмиссия.
            float a = tex.a * (1.0 + _Color.a);
            clip(a - _Cutoff);
            fixed3 rgb = fixed3(0.0, 0.0, 0.0);
            #ifdef _EMISSION
                rgb = tex2D(_EmissionMap, i.uv).rgb * _EmissionColor.rgb;
            #endif
            return fixed4(rgb, 1.0);
        #else
            // Оригинал: (col * _Color) + col == col * (1 + _Color).
            fixed4 c = tex * (1.0 + _Color);
            #ifdef _EMISSION
                c += tex2D(_EmissionMap, i.uv) * _EmissionColor;
            #endif
            clip(c.a - _Cutoff);
            return fixed4(c.rgb, 1.0);
        #endif
    }
    ENDCG

    // RenderType = Opaque
    SubShader
    {
        LOD 100
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            LOD 100
            Tags { "RenderType" = "Opaque" }
            CGPROGRAM
            ENDCG
        }
    }

    // RenderType = TransparentCutout
    SubShader
    {
        LOD 100
        Tags { "Queue" = "AlphaTest+0" "RenderType" = "TransparentCutout" }
        Pass
        {
            LOD 100
            Tags { "Queue" = "AlphaTest+0" "RenderType" = "TransparentCutout" }
            CGPROGRAM
            ENDCG
        }
    }

    // RenderType = Transparent
    SubShader
    {
        LOD 100
        Tags { "IgnoreProjector" = "True" "Queue" = "Transparent" "RenderType" = "Transparent" }
        Pass
        {
            LOD 100
            Tags { "IgnoreProjector" = "True" "Queue" = "Transparent" "RenderType" = "Transparent" }
            CGPROGRAM
            ENDCG
        }
    }

    FallBack "VertexLit"
}
