Shader "Unlit/SmoothGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseForce ("Noise Force", Range(0, 500)) = 10
        _NoiseScale ("Noise Scale", Vector) = (10,10,0,0)
        _ColorFrom ("Color From", Color) = (1,0,1,1)
        _ColorTo ("Color To", Color) = (1,1,0,1)
        _RadiusMultiplier ("Radius", Range(0, 1000)) = 1
        _Dropoff ("Dropoff", Range(0, 1000)) = 1
        _GradientPivot ("Position", Vector) = (0.5, 0.5, 0, 0) // лучше задать центр по умолчанию
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;

            float _NoiseForce;
            float2 _NoiseScale;
            float4 _ColorFrom;
            float4 _ColorTo;
            float _RadiusMultiplier;
            float _Dropoff;
            float4 _GradientPivot;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Градиент
                float dist = length(i.uv - _GradientPivot.xy);
                float gradient = pow(saturate(dist * _RadiusMultiplier), _Dropoff);

                float4 color = lerp(_ColorFrom, _ColorTo, gradient);

                // Noise
                float2 noiseUV = i.uv * _NoiseScale;
                float3 noise = tex2D(_NoiseTex, noiseUV).rgb / _NoiseForce;

                color.rgb += noise;
                color.a = _ColorFrom.a; // или lerp альфы, если нужно

                return color;
            }
            ENDHLSL
        }
    }
}