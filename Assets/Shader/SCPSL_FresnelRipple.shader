Shader "SCPSL/FresnelRipple"
{
    Properties
    {
        _DistortionAmount("Distortion Amount", Range(0, 0.1)) = 0.292
        _DepthFadeDistance("Depth Fade Distance", Float) = 0
        _TextureSample1("Texture Sample 1", 2D) = "bump" {}
        _TimeScale("Time Scale", Float) = 0
        _ForcefieldTint("Forcefield Tint", Color) = (0,0,0,0)
        _IntersectionColor("Intersection Color", Color) = (0.4338235,0.4377282,1,0)
        _FresnelPower("Fresnel Power", Float) = 0
        _FresnelScale("Fresnel Scale", Float) = 0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

        Pass
        {
            Name "First Pass"
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0

            #include "UnityCG.cginc"

            float _DepthFadeDistance;
            float4 _IntersectionColor;
            sampler2D _CameraDepthTexture;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 screenUV = i.screenPos.xy / i.screenPos.w;

                float sceneDepth = LinearEyeDepth(tex2D(_CameraDepthTexture, screenUV).r);
                float fragDepth = LinearEyeDepth(i.screenPos.z / i.screenPos.w);

                float diff = sceneDepth - fragDepth;
                float fade = saturate(abs(diff) / _DepthFadeDistance);

                float alpha = 1.0 - fade;
                return float4(_IntersectionColor.rgb, alpha);
            }
            ENDHLSL
        }

        GrabPass { }

        Pass
        {
            Name "Second Pass"
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0

            #include "UnityCG.cginc"

            float _DistortionAmount;
            float _DepthFadeDistance;
            float _TimeScale;
            float _FresnelPower;
            float _FresnelScale;
            float4 _ForcefieldTint;
            float4 _IntersectionColor;

            sampler2D _TextureSample1;
            sampler2D _GrabTexture;
            sampler2D _CameraDepthTexture;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                o.screenPos = ComputeScreenPos(o.pos);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Rotating UVs based on time
                float2 uvCentered = i.uv - 0.5;
                float angle = _Time.y * _TimeScale;
                float s = sin(angle);
                float c = cos(angle);
                float2 rotatedUV = float2(dot(uvCentered, float2(c, s)),
                                           dot(uvCentered, float2(-s, c))) + 0.5;

                // Sample bump texture, use alpha as mask on red channel
                float4 bumpTex = tex2D(_TextureSample1, rotatedUV);
                float2 bump = float2(bumpTex.r * bumpTex.a, bumpTex.g) * 2.0 - 1.0;
                bump *= _DistortionAmount;

                // Grab texture distortion
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float2 grabUV = screenUV + bump;
                fixed4 grabColor = tex2D(_GrabTexture, grabUV);
                float3 tintedColor = grabColor.rgb * _ForcefieldTint.rgb;

                // Fresnel effect
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float NdotV = dot(i.worldNormal, viewDir);
                float fresnel = pow(saturate(1.0 - NdotV), _FresnelPower) * _FresnelScale;

                // Blend via fresnel²
                float fresnelSq = fresnel * fresnel;
                float3 color = _IntersectionColor.rgb * fresnelSq + tintedColor * (1.0 - fresnel);

                // Depth intersection alpha
                float sceneDepth = LinearEyeDepth(tex2D(_CameraDepthTexture, screenUV).r);
                float fragDepth = LinearEyeDepth(i.screenPos.z / i.screenPos.w);
                float depthDiff = sceneDepth - fragDepth;
                float alpha = saturate(abs(depthDiff) / _DepthFadeDistance);

                return float4(color, alpha);
            }
            ENDHLSL
        }
    }

    CustomEditor "ASEMaterialInspector"
}