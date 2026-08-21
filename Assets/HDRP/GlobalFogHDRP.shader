Shader "GlobalFog"
{
    Properties
    {
        _Fog_Start("Fog Start", Float) = 100
        _Fog_End("Fog End", Float) = 500
        _Fog_Color("Fog Color", Color) = (0.3018868, 0.3018868, 0.3018868, 1)
        _Dog_Density("Dog Density", Float) = 1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="HDRenderPipeline"
            // RenderType: <None>
            // Queue: <None>
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="HDFullscreenSubTarget"
        }
        Pass
        {
            Name "DrawProcedural"
        
        // Render State
        Cull Off
        Blend Off
        ZTest Off
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        // #pragma enable_d3d11_debug_symbols
        
        /* WARNING: $splice Could not find named fragment 'DotsInstancingOptions' */
        /* WARNING: $splice Could not find named fragment 'HybridV1InjectedBuiltinProperties' */
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        #define FULLSCREEN_SHADERGRAPH
        
        // Defines
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_VERTEXID
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        
        // Force depth texture because we need it for almost every nodes
        // TODO: dependency system that triggers this define from position or view direction usage
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_NORMAL_TEXTURE
        
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DRAWPROCEDURAL
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_OPAQUE_TEXTURE
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenShaderPass.cs.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassSampling.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Fullscreen/HDFullscreenFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
             uint vertexID : VERTEXID_SEMANTIC;
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        struct VertexDescriptionInputs
        {
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Fog_Start;
        float _Fog_End;
        float4 _Fog_Color;
        float _Dog_Density;
        CBUFFER_END
        
        
        // Object and Global properties
        float _FlipY;
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // Graph Functions
        
        float3 Unity_HDRP_SampleSceneColor_float(float2 uv, float lod, float exposureMultiplier)
        {
            #if defined(REQUIRE_OPAQUE_TEXTURE) && defined(_SURFACE_TYPE_TRANSPARENT) && defined(SHADERPASS) && (SHADERPASS != SHADERPASS_LIGHT_TRANSPORT) && (SHADERPASS != SHADERPASS_PATH_TRACING) && (SHADERPASS != SHADERPASS_RAYTRACING_VISIBILITY) && (SHADERPASS != SHADERPASS_RAYTRACING_FORWARD)
            return SampleCameraColor(uv, lod) * exposureMultiplier;
            #endif
            #if defined(REQUIRE_OPAQUE_TEXTURE) && defined(CUSTOM_PASS_SAMPLING_HLSL) && defined(SHADERPASS) && (SHADERPASS == SHADERPASS_DRAWPROCEDURAL || SHADERPASS == SHADERPASS_BLIT)
            return CustomPassSampleCameraColor(uv, lod) * exposureMultiplier;
            #endif
            return float3(0.0, 0.0, 0.0);
        }
        
        StructuredBuffer<int2> _DepthPyramidMipLevelOffsets;
        float Unity_HDRP_SampleSceneDepth_float(float2 uv, float lod)
        {
            #if defined(REQUIRE_DEPTH_TEXTURE) && defined(SHADERPASS) && (SHADERPASS != SHADERPASS_LIGHT_TRANSPORT)
            int2 coord = int2(uv * _ScreenSize.xy);
            int2 mipCoord  = coord.xy >> int(lod);
            int2 mipOffset = _DepthPyramidMipLevelOffsets[int(lod)];
            return LOAD_TEXTURE2D_X(_CameraDepthTexture, mipOffset + mipCoord).r;
            #endif
            return 0.0;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        // GraphVertex: <None>
        
        // Custom interpolators, pre surface
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreSurface' */
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float3 _HDSceneColor_2c68fd7c8a354f30b04e0f98b596db79_Output_2_Vector3 = Unity_HDRP_SampleSceneColor_float(float4(IN.NDCPosition.xy, 0, 0).xy, float(0), GetInverseCurrentExposureMultiplier());
            float4 _Property_aab32b78c29a4e02984320ff8e306232_Out_0_Vector4 = _Fog_Color;
            float4 _ScreenPosition_62434c8867ef4be79ff8e9141807f0af_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float _HDSceneDepth_1d50ca6aaf0b4d698b952b97c4c5e98c_Output_2_Float = LinearEyeDepth(Unity_HDRP_SampleSceneDepth_float(_ScreenPosition_62434c8867ef4be79ff8e9141807f0af_Out_0_Vector4.xy, float(0)), _ZBufferParams);
            float _Property_ae147f92641549fb8a39af6c8cf98f2d_Out_0_Float = _Fog_Start;
            float _Subtract_7a28263fd4874304a6d57957bd1cafdd_Out_2_Float;
            Unity_Subtract_float(_HDSceneDepth_1d50ca6aaf0b4d698b952b97c4c5e98c_Output_2_Float, _Property_ae147f92641549fb8a39af6c8cf98f2d_Out_0_Float, _Subtract_7a28263fd4874304a6d57957bd1cafdd_Out_2_Float);
            float _Property_0785ae8d40c7414b9b4407bcf4731424_Out_0_Float = _Fog_End;
            float _Subtract_374be8c656c446d6bd29bb2f42c66aeb_Out_2_Float;
            Unity_Subtract_float(_Property_ae147f92641549fb8a39af6c8cf98f2d_Out_0_Float, _Property_0785ae8d40c7414b9b4407bcf4731424_Out_0_Float, _Subtract_374be8c656c446d6bd29bb2f42c66aeb_Out_2_Float);
            float _Divide_199aef5c02dd4c278fcefceeb88527e3_Out_2_Float;
            Unity_Divide_float(_Subtract_7a28263fd4874304a6d57957bd1cafdd_Out_2_Float, _Subtract_374be8c656c446d6bd29bb2f42c66aeb_Out_2_Float, _Divide_199aef5c02dd4c278fcefceeb88527e3_Out_2_Float);
            float _Saturate_a3f0c1b633d940189ed13c18ba316f83_Out_1_Float;
            Unity_Saturate_float(_Divide_199aef5c02dd4c278fcefceeb88527e3_Out_2_Float, _Saturate_a3f0c1b633d940189ed13c18ba316f83_Out_1_Float);
            float3 _Lerp_99b044c8b9844d0e829d4cb21c598206_Out_3_Vector3;
            Unity_Lerp_float3(_HDSceneColor_2c68fd7c8a354f30b04e0f98b596db79_Output_2_Vector3, (_Property_aab32b78c29a4e02984320ff8e306232_Out_0_Vector4.xyz), (_Saturate_a3f0c1b633d940189ed13c18ba316f83_Out_1_Float.xxx), _Lerp_99b044c8b9844d0e829d4cb21c598206_Out_3_Vector3);
            surface.BaseColor = _Lerp_99b044c8b9844d0e829d4cb21c598206_Out_3_Vector3;
            surface.Alpha = float(1);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            float3 normalWS = SHADERGRAPH_SAMPLE_SCENE_NORMAL(input.texCoord0.xy);
            float4 tangentWS = float4(0, 1, 0, 0); // We can't access the tangent in screen space
        
        
        
        
            float3 viewDirWS = normalize(input.texCoord1.xyz);
            float linearDepth = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(input.texCoord0.xy), _ZBufferParams);
            float3 cameraForward = -UNITY_MATRIX_V[2].xyz;
            float camearDistance = linearDepth / dot(viewDirWS, cameraForward);
            float3 positionWS = viewDirWS * camearDistance + GetCameraPositionWS();
        
        
            output.WorldSpacePosition = positionWS;
            output.ScreenPosition = float4(input.texCoord0.xy, 0, 1);
            output.NDCPosition = input.texCoord0.xy;
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenCommon.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenDrawProcedural.hlsl"
        
        ENDHLSL
        }
        Pass
        {
            Name "Blit"
        
        // Render State
        Cull Off
        Blend Off
        ZTest Off
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        // #pragma enable_d3d11_debug_symbols
        
        /* WARNING: $splice Could not find named fragment 'DotsInstancingOptions' */
        /* WARNING: $splice Could not find named fragment 'HybridV1InjectedBuiltinProperties' */
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        #define FULLSCREEN_SHADERGRAPH
        
        // Defines
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_VERTEXID
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        
        // Force depth texture because we need it for almost every nodes
        // TODO: dependency system that triggers this define from position or view direction usage
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_NORMAL_TEXTURE
        
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_BLIT
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_OPAQUE_TEXTURE
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenShaderPass.cs.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassSampling.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Fullscreen/HDFullscreenFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
             uint vertexID : VERTEXID_SEMANTIC;
             float3 positionOS : POSITION;
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        struct VertexDescriptionInputs
        {
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Fog_Start;
        float _Fog_End;
        float4 _Fog_Color;
        float _Dog_Density;
        CBUFFER_END
        
        
        // Object and Global properties
        float _FlipY;
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // Graph Functions
        
        float3 Unity_HDRP_SampleSceneColor_float(float2 uv, float lod, float exposureMultiplier)
        {
            #if defined(REQUIRE_OPAQUE_TEXTURE) && defined(_SURFACE_TYPE_TRANSPARENT) && defined(SHADERPASS) && (SHADERPASS != SHADERPASS_LIGHT_TRANSPORT) && (SHADERPASS != SHADERPASS_PATH_TRACING) && (SHADERPASS != SHADERPASS_RAYTRACING_VISIBILITY) && (SHADERPASS != SHADERPASS_RAYTRACING_FORWARD)
            return SampleCameraColor(uv, lod) * exposureMultiplier;
            #endif
            #if defined(REQUIRE_OPAQUE_TEXTURE) && defined(CUSTOM_PASS_SAMPLING_HLSL) && defined(SHADERPASS) && (SHADERPASS == SHADERPASS_DRAWPROCEDURAL || SHADERPASS == SHADERPASS_BLIT)
            return CustomPassSampleCameraColor(uv, lod) * exposureMultiplier;
            #endif
            return float3(0.0, 0.0, 0.0);
        }
        
        StructuredBuffer<int2> _DepthPyramidMipLevelOffsets;
        float Unity_HDRP_SampleSceneDepth_float(float2 uv, float lod)
        {
            #if defined(REQUIRE_DEPTH_TEXTURE) && defined(SHADERPASS) && (SHADERPASS != SHADERPASS_LIGHT_TRANSPORT)
            int2 coord = int2(uv * _ScreenSize.xy);
            int2 mipCoord  = coord.xy >> int(lod);
            int2 mipOffset = _DepthPyramidMipLevelOffsets[int(lod)];
            return LOAD_TEXTURE2D_X(_CameraDepthTexture, mipOffset + mipCoord).r;
            #endif
            return 0.0;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        // GraphVertex: <None>
        
        // Custom interpolators, pre surface
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreSurface' */
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float3 _HDSceneColor_2c68fd7c8a354f30b04e0f98b596db79_Output_2_Vector3 = Unity_HDRP_SampleSceneColor_float(float4(IN.NDCPosition.xy, 0, 0).xy, float(0), GetInverseCurrentExposureMultiplier());
            float4 _Property_aab32b78c29a4e02984320ff8e306232_Out_0_Vector4 = _Fog_Color;
            float4 _ScreenPosition_62434c8867ef4be79ff8e9141807f0af_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float _HDSceneDepth_1d50ca6aaf0b4d698b952b97c4c5e98c_Output_2_Float = LinearEyeDepth(Unity_HDRP_SampleSceneDepth_float(_ScreenPosition_62434c8867ef4be79ff8e9141807f0af_Out_0_Vector4.xy, float(0)), _ZBufferParams);
            float _Property_ae147f92641549fb8a39af6c8cf98f2d_Out_0_Float = _Fog_Start;
            float _Subtract_7a28263fd4874304a6d57957bd1cafdd_Out_2_Float;
            Unity_Subtract_float(_HDSceneDepth_1d50ca6aaf0b4d698b952b97c4c5e98c_Output_2_Float, _Property_ae147f92641549fb8a39af6c8cf98f2d_Out_0_Float, _Subtract_7a28263fd4874304a6d57957bd1cafdd_Out_2_Float);
            float _Property_0785ae8d40c7414b9b4407bcf4731424_Out_0_Float = _Fog_End;
            float _Subtract_374be8c656c446d6bd29bb2f42c66aeb_Out_2_Float;
            Unity_Subtract_float(_Property_ae147f92641549fb8a39af6c8cf98f2d_Out_0_Float, _Property_0785ae8d40c7414b9b4407bcf4731424_Out_0_Float, _Subtract_374be8c656c446d6bd29bb2f42c66aeb_Out_2_Float);
            float _Divide_199aef5c02dd4c278fcefceeb88527e3_Out_2_Float;
            Unity_Divide_float(_Subtract_7a28263fd4874304a6d57957bd1cafdd_Out_2_Float, _Subtract_374be8c656c446d6bd29bb2f42c66aeb_Out_2_Float, _Divide_199aef5c02dd4c278fcefceeb88527e3_Out_2_Float);
            float _Saturate_a3f0c1b633d940189ed13c18ba316f83_Out_1_Float;
            Unity_Saturate_float(_Divide_199aef5c02dd4c278fcefceeb88527e3_Out_2_Float, _Saturate_a3f0c1b633d940189ed13c18ba316f83_Out_1_Float);
            float3 _Lerp_99b044c8b9844d0e829d4cb21c598206_Out_3_Vector3;
            Unity_Lerp_float3(_HDSceneColor_2c68fd7c8a354f30b04e0f98b596db79_Output_2_Vector3, (_Property_aab32b78c29a4e02984320ff8e306232_Out_0_Vector4.xyz), (_Saturate_a3f0c1b633d940189ed13c18ba316f83_Out_1_Float.xxx), _Lerp_99b044c8b9844d0e829d4cb21c598206_Out_3_Vector3);
            surface.BaseColor = _Lerp_99b044c8b9844d0e829d4cb21c598206_Out_3_Vector3;
            surface.Alpha = float(1);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            float3 normalWS = SHADERGRAPH_SAMPLE_SCENE_NORMAL(input.texCoord0.xy);
            float4 tangentWS = float4(0, 1, 0, 0); // We can't access the tangent in screen space
        
        
        
        
            float3 viewDirWS = normalize(input.texCoord1.xyz);
            float linearDepth = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(input.texCoord0.xy), _ZBufferParams);
            float3 cameraForward = -UNITY_MATRIX_V[2].xyz;
            float camearDistance = linearDepth / dot(viewDirWS, cameraForward);
            float3 positionWS = viewDirWS * camearDistance + GetCameraPositionWS();
        
        
            output.WorldSpacePosition = positionWS;
            output.ScreenPosition = float4(input.texCoord0.xy, 0, 1);
            output.NDCPosition = input.texCoord0.xy;
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenCommon.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenBlit.hlsl"
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.Rendering.Fullscreen.ShaderGraph.FullscreenShaderGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}