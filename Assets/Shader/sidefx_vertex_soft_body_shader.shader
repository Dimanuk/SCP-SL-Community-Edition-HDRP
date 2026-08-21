// Ground truth: decompiled from the original build (resources.assets path_id=3251) via UnityPy.
// The original is the SideFX Houdini VAT (Vertex Animation Textures) soft-body SURFACE shader
// with ONE modification by NW: surf() also writes o.Emission = albedo (verified in the decompiled
// GLSL of every lighting pass - see surf() below). Compiled form has exactly the 4 passes a
// surface shader generates (ForwardBase + ForwardAdd + Deferred + ShadowCaster/addshadow,
// LOD 200, RenderType Opaque, Fallback Diffuse).
// Key VAT math confirmed instruction-by-instruction in the decompiled GLSL:
//   position: v.vertex.xyz += float3(-pos.x, pos.z, pos.y)  (X flip + XZY swizzle, Houdini Z-up)
//   normal (unpacked): n = (nTex.xzy * 2 - 1), then n.x *= -1
//   normal (packed):   f2 = (floor(w*1024/32), remainder) * (4/31.5) - 2, sphere-map decode,
//                      then .xzy swizzle + X flip
// Used by Mat_vertex_lightning (MicroHID electric arc, "Lightning_mesh").
//
// IMPORTANT — Lightning_pos.png MUST stay on Bilinear filtering. The VAT was authored on a
// 732x41 grid (uv2.x steps of 1/1464, 41 frames) but the shipped texture is 512x32, so sample
// coordinates almost never hit texel centers. Bilinear reconstructs the rescaled data (and
// blends adjacent frame rows for smooth animation); Point filtering collapses neighbouring
// points onto shared texels and snaps frames — the arc turns into clumped, stuttering segments.
Shader "sidefx/vertex_soft_body_shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0

        _boundingMax ("Bounding Max", Float) = 1
        _boundingMin ("Bounding Min", Float) = 1
        _numOfFrames ("Number Of Frames", Float) = 240
        _speed ("Speed", Float) = 0.33

        [MaterialToggle] _pack_normal ("Pack Normal", Float) = 0

        _posTex ("Position Map (RGB)", 2D) = "white" {}
        _nTex ("Normal Map (RGB)", 2D) = "grey" {}

        _EmissionLM ("Emission (Lightmapper)", Color) = (1,1,1,1)
        [Toggle] _DynamicEmissionLM ("Dynamic Emission (Lightmapper)", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _posTex;
        sampler2D _nTex;
        uniform float _pack_normal;
        uniform float _boundingMax;
        uniform float _boundingMin;
        uniform float _speed;
        uniform int _numOfFrames;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void vert(inout appdata_full v)
        {
            // Current animation frame, sampled from the VAT strip via UV2.
            float timeInFrames = ((ceil(frac(-_Time.y * _speed) * _numOfFrames)) / _numOfFrames) + (1.0 / _numOfFrames);

            float4 texturePos = tex2Dlod(_posTex, float4(v.texcoord1.x, (timeInFrames + v.texcoord1.y), 0, 0));
            float3 textureN = tex2Dlod(_nTex, float4(v.texcoord1.x, (timeInFrames + v.texcoord1.y), 0, 0));

            // Expand normalised position texture values to object space.
            float expand = _boundingMax - _boundingMin;
            texturePos.xyz *= expand;
            texturePos.xyz += _boundingMin;
            texturePos.x *= -1;             // flipped for Unity's left-handedness
            v.vertex.xyz += texturePos.xzy; // swizzle: VAT textures are exported Z-up

            if (_pack_normal)
            {
                // Decode float to float2 (5-bit pair packed in posTex alpha).
                float alpha = texturePos.w * 1024;
                float2 f2;
                f2.x = floor(alpha / 32.0) / 31.5;
                f2.y = (alpha - (floor(alpha / 32.0) * 32.0)) / 31.5;

                // Decode float2 to float3 (sphere-map style).
                float3 f3;
                f2 *= 4;
                f2 -= 2;
                float f2dot = dot(f2, f2);
                f3.xy = sqrt(1 - (f2dot / 4.0)) * f2;
                f3.z = 1 - (f2dot / 2.0);
                f3 = clamp(f3, -1.0, 1.0);
                f3 = f3.xzy;
                f3.x *= -1;
                v.normal = f3;
            }
            else
            {
                textureN = textureN.xzy;
                textureN *= 2;
                textureN -= 1;
                textureN.x *= -1;
                v.normal = textureN;
            }
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // NOT stock SideFX: the original build's fragment ends with
            //   SV_Target0.xyz = tex(_MainTex) * _Color + <BRDF result>
            // (ForwardBase; Deferred writes the same term into the emission RT).
            // Without this the arc renders as a black silhouette (zero normals, no light).
            o.Emission = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
