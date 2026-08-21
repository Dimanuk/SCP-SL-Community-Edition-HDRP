Shader "Rollthered/Fire"
{
    Properties
    {
        _FresnelPower ("Fresnel Power", Range(0, 5)) = 2
        [HideInInspector] _FresnelScale ("Fresnel Scale", Range(0, 0.3)) = 1.5
        _EdgeLength ("Edge length", Range(2, 50)) = 13.5
        _FresnelBias ("Fresnel Bias", Range(0, 0.2)) = 0.2364706
        [HDR] _Flamecolor2 ("Flame color 2", Color) = (1,0,0,0)
        [HDR] _FlameColor ("Flame Color", Color) = (1,0.8068966,0,0)
        _Y_Mask ("Y_Mask", Range(0, 5)) = 0
        _FlameHeight ("Flame Height", Range(0, 1)) = 0
        _Flamenoise ("Flame noise", 2D) = "white" {}
        _FlameWave ("Flame Wave", 2D) = "white" {}
        _v ("v", Range(-1, 1)) = 0
        _u ("u", Range(-1, 1)) = 0
        _Alpha ("Alpha", Range(0, 1)) = 0
        [HideInInspector] __dirty ("", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "IgnoreProjector" = "true"
            "IsEmissive" = "true"
            "Queue" = "Transparent+100"
            "RenderType" = "Transparent"
        }

        CGINCLUDE
        #include "UnityCG.cginc"

        float _FresnelPower;
        float _FresnelScale;
        float _FresnelBias;
        float _Y_Mask;
        float _Alpha;
        float4 _Flamecolor2;
        float4 _FlameColor;
        float _FlameHeight;
        float _u;
        float _v;
        sampler2D _Flamenoise;
        float4 _Flamenoise_ST;
        sampler2D _FlameWave;

        struct v2f
        {
            float4 pos : SV_POSITION;
            float3 worldNormal : TEXCOORD0;
            float3 worldPos : TEXCOORD1;
        };

        // The compiled build displaces vertices along the object-space world-up
        // by FlameWave*Flamenoise (UVs scrolled by _u/_v over time), masked by
        // the same Y-mask used in the fragment stage.
        v2f RolltheredVert(appdata_full v)
        {
            v2f o;

            float3 worldNormal = UnityObjectToWorldNormal(v.normal);

            float2 uv = v.texcoord.xy * _Flamenoise_ST.xy + _Flamenoise_ST.zw;
            uv += _Time.y * float2(_u, _v);
            float disp = tex2Dlod(_FlameWave, float4(uv, 0, 0)).x * tex2Dlod(_Flamenoise, float4(uv, 0, 0)).x;

            float yMask = 1.0 - saturate(abs(_Y_Mask - worldNormal.y) - _Y_Mask);

            float3 upOS = mul(unity_WorldToObject, float4(0, 1, 0, 0)).xyz;
            v.vertex.xyz += disp * yMask * _FlameHeight * upOS;

            o.pos = UnityObjectToClipPos(v.vertex);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.worldNormal = worldNormal;
            return o;
        }

        // Faithful to the compiled fragment: dot(N,V) is NOT saturated. With
        // Cull Front the visible faces have dot(N,V) < 0, so (1 - dot) runs up
        // to 2 and pow amplifies it - that is what makes the ghost visible.
        float RollthredFresnel(float3 worldNormal, float3 viewDir)
        {
            float ndv = dot(worldNormal, viewDir);
            return _FresnelScale * pow(1.0 - ndv, _FresnelPower) + _FresnelBias;
        }
        ENDCG

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            ZWrite Off
            Cull Front

            CGPROGRAM
            #pragma vertex RolltheredVert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_fwdbase

            #include "UnityLightingCommon.cginc"

            fixed4 frag(v2f i) : SV_Target
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 worldNormal = normalize(i.worldNormal);
                float ndv = dot(worldNormal, viewDir);

                float fresnel = RollthredFresnel(worldNormal, viewDir);

                float3 flameColor = _Flamecolor2.rgb + fresnel * (_FlameColor.rgb - _Flamecolor2.rgb);
                flameColor = clamp(flameColor, 0.0, 5.0);

                float yMask = 1.0 - saturate(abs(_Y_Mask - worldNormal.y) - _Y_Mask);

                float alpha = fresnel * yMask * _Alpha;

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float ndl = saturate(dot(worldNormal, lightDir));

                float3 halfDir = normalize(viewDir + lightDir);
                float ldh = saturate(dot(lightDir, halfDir));

                float vis = 0.5 / (abs(ndv) + ndl + 1e-5);
                float f = 1.0 - ldh;
                float f5 = f * f;
                f5 *= f5;
                f5 *= f;
                float3 spec = ndl * vis * _LightColor0.rgb * (f5 * 0.96 + 0.04);

                float3 reflectDir = reflect(-viewDir, worldNormal);
                float4 envSample = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectDir, 6.0);
                float3 env = DecodeHDR(envSample, unity_SpecCube0_HDR) * 0.5;
                float fnv = 1.0 - abs(ndv);
                float fnv5 = fnv * fnv;
                fnv5 *= fnv5;
                fnv5 *= fnv;
                env *= fnv5 * 2.235e-8 + 0.04;

                return fixed4(flameColor + spec + env, alpha);
            }
            ENDCG
        }

        Pass
        {
            Name "FORWARD_ADD"
            Tags { "LightMode" = "ForwardAdd" }

            Blend SrcAlpha One
            ColorMask RGB
            ZWrite Off
            Cull Front

            CGPROGRAM
            #pragma vertex RolltheredVert
            #pragma fragment fragAdd
            #pragma target 3.0
            #pragma multi_compile_fwdadd

            #include "UnityLightingCommon.cginc"

            fixed4 fragAdd(v2f i) : SV_Target
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 worldNormal = normalize(i.worldNormal);
                float ndv = dot(worldNormal, viewDir);

                float fresnel = RollthredFresnel(worldNormal, viewDir);

                float yMask = 1.0 - saturate(abs(_Y_Mask - worldNormal.y) - _Y_Mask);
                float alpha = fresnel * yMask * _Alpha;

                #if defined(POINT) || defined(SPOT)
                    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                #else
                    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                #endif

                float ndl = saturate(dot(worldNormal, lightDir));

                float3 halfDir = normalize(viewDir + lightDir);
                float ldh = saturate(dot(lightDir, halfDir));

                float vis = 0.5 / (abs(ndv) + ndl + 1e-5);
                float f = 1.0 - ldh;
                float f5 = f * f;
                f5 *= f5;
                f5 *= f;
                float3 spec = ndl * vis * _LightColor0.rgb * (f5 * 0.96 + 0.04);

                return fixed4(spec, alpha);
            }
            ENDCG
        }
    }

    FallBack "Transparent/VertexLit"
}
