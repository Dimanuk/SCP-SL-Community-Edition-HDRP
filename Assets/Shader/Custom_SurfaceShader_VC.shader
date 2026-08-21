Shader "Custom/SurfaceShader_VC"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Normal ("Normap Map", 2D) = "bump" {}
	}
	SubShader
	{
		LOD 200
		Tags { "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass
		{
			Name "FORWARD"
			LOD 200
			Tags { "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			ZWrite Off
			GpuProgramID 8192

			HLSLPROGRAM

			// https://docs.unity3d.com/Manual/SL-PragmaDirectives.html
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			#pragma shader_feature DIRECTIONAL
			#pragma multi_compile _ FOG_LINEAR
			#pragma shader_feature LIGHTPROBE_SH
			#pragma shader_feature VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifndef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[6];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float4 vertex_output_2;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_output_5 : COLOR; // COLOR
				float4 vertex_output_6 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_7 : TEXCOORD8; // TEXCOORD_8
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_57 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_58 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_59 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_83 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_57));
				float vertex_unnamed_84 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_58));
				float vertex_unnamed_85 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_59));
				precise float vertex_unnamed_94 = vertex_unnamed_83 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_95 = vertex_unnamed_84 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_96 = vertex_unnamed_85 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_97 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_60)) + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_115 = vertex_unnamed_95 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_116 = vertex_unnamed_95 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_117 = vertex_unnamed_95 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_118 = vertex_unnamed_95 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_97, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_96, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_94, vertex_unnamed_115)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_97, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_96, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_94, vertex_unnamed_116)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_97, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_96, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_94, vertex_unnamed_117)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_97, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_96, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_94, vertex_unnamed_118)));
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[5u].x, vertex_uniform_buffer_0[5u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[5u].y, vertex_uniform_buffer_0[5u].w);
				vertex_output_2.w = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_83);
				float vertex_unnamed_187 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_201 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_215 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_221 = rsqrt(dot(float3(vertex_unnamed_215, vertex_unnamed_187, vertex_unnamed_201), float3(vertex_unnamed_215, vertex_unnamed_187, vertex_unnamed_201)));
				precise float vertex_unnamed_222 = vertex_unnamed_221 * vertex_unnamed_215;
				precise float vertex_unnamed_223 = vertex_unnamed_221 * vertex_unnamed_187;
				precise float vertex_unnamed_224 = vertex_unnamed_221 * vertex_unnamed_201;
				precise float vertex_unnamed_232 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_233 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_234 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_252 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_232));
				float vertex_unnamed_253 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_233));
				float vertex_unnamed_254 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_234));
				float vertex_unnamed_258 = rsqrt(dot(float3(vertex_unnamed_252, vertex_unnamed_253, vertex_unnamed_254), float3(vertex_unnamed_252, vertex_unnamed_253, vertex_unnamed_254)));
				precise float vertex_unnamed_259 = vertex_unnamed_258 * vertex_unnamed_252;
				precise float vertex_unnamed_260 = vertex_unnamed_258 * vertex_unnamed_253;
				precise float vertex_unnamed_261 = vertex_unnamed_258 * vertex_unnamed_254;
				precise float vertex_unnamed_262 = vertex_unnamed_222 * vertex_unnamed_259;
				precise float vertex_unnamed_263 = vertex_unnamed_223 * vertex_unnamed_260;
				precise float vertex_unnamed_264 = vertex_unnamed_224 * vertex_unnamed_261;
				precise float vertex_unnamed_265 = (-0.0f) - vertex_unnamed_262;
				precise float vertex_unnamed_267 = (-0.0f) - vertex_unnamed_263;
				precise float vertex_unnamed_268 = (-0.0f) - vertex_unnamed_264;
				precise float vertex_unnamed_278 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_279 = vertex_unnamed_278 * mad(vertex_unnamed_224, vertex_unnamed_260, vertex_unnamed_265);
				precise float vertex_unnamed_280 = vertex_unnamed_278 * mad(vertex_unnamed_222, vertex_unnamed_261, vertex_unnamed_267);
				precise float vertex_unnamed_281 = vertex_unnamed_278 * mad(vertex_unnamed_223, vertex_unnamed_259, vertex_unnamed_268);
				vertex_output_2.y = vertex_unnamed_279;
				vertex_output_2.x = vertex_unnamed_261;
				vertex_output_2.z = vertex_unnamed_223;
				vertex_output_3.x = vertex_unnamed_259;
				vertex_output_4.x = vertex_unnamed_260;
				vertex_output_3.z = vertex_unnamed_224;
				vertex_output_4.z = vertex_unnamed_222;
				vertex_output_3.w = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_84);
				vertex_output_4.w = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_85);
				vertex_output_3.y = vertex_unnamed_280;
				vertex_output_4.y = vertex_unnamed_281;
				vertex_output_5.x = vertex_input_7.x;
				vertex_output_5.y = vertex_input_7.y;
				vertex_output_5.z = vertex_input_7.z;
				vertex_output_5.w = vertex_input_7.w;
				vertex_output_6.x = 0.0f;
				vertex_output_6.y = 0.0f;
				vertex_output_6.z = 0.0f;
				vertex_output_6.w = 0.0f;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[5] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // !FOG_LINEAR
			#endif // !LIGHTPROBE_SH
			#endif // !VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifndef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float4 vertex_output_1;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float4 vertex_output_2;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_input_4;
			static float4 vertex_output_5;
			static float4 vertex_output_6;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float4 vertex_output_1 : TEXCOORD1; // vs_TEXCOORD1
				float4 vertex_output_2 : TEXCOORD2; // vs_TEXCOORD2
				float4 vertex_output_3 : TEXCOORD3; // vs_TEXCOORD3
				float4 vertex_output_4 : UNKNOWN4;
				float4 vertex_output_5 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_6 : TEXCOORD8; // vs_TEXCOORD8
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_44;
			static float4 vertex_unnamed_62;
			static float3 vertex_unnamed_206;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_44 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				float3 vertex_unnamed_59 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_59.x, vertex_unnamed_59.y, vertex_unnamed_59.z, vertex_unnamed_9.w);
				vertex_unnamed_62 = vertex_unnamed_44.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_62 = (unity_MatrixVP__array[0] * vertex_unnamed_44.xxxx) + vertex_unnamed_62;
				vertex_unnamed_62 = (unity_MatrixVP__array[2] * vertex_unnamed_44.zzzz) + vertex_unnamed_62;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_44.wwww) + vertex_unnamed_62;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_output_1.w = vertex_unnamed_9.x;
				vertex_unnamed_44.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_44.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_44.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_9.x = dot(vertex_unnamed_44.xyz, vertex_unnamed_44.xyz);
				vertex_unnamed_9.x = rsqrt(vertex_unnamed_9.x);
				float3 vertex_unnamed_154 = vertex_unnamed_9.xxx * vertex_unnamed_44.xyz;
				vertex_unnamed_44 = float4(vertex_unnamed_154.x, vertex_unnamed_154.y, vertex_unnamed_154.z, vertex_unnamed_44.w);
				float3 vertex_unnamed_163 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_62 = float4(vertex_unnamed_163.x, vertex_unnamed_163.y, vertex_unnamed_163.z, vertex_unnamed_62.w);
				float3 vertex_unnamed_174 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_62.xyz;
				vertex_unnamed_62 = float4(vertex_unnamed_174.x, vertex_unnamed_174.y, vertex_unnamed_174.z, vertex_unnamed_62.w);
				float3 vertex_unnamed_185 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_62.xyz;
				vertex_unnamed_62 = float4(vertex_unnamed_185.x, vertex_unnamed_185.y, vertex_unnamed_185.z, vertex_unnamed_62.w);
				vertex_unnamed_9.x = dot(vertex_unnamed_62.xyz, vertex_unnamed_62.xyz);
				vertex_unnamed_9.x = rsqrt(vertex_unnamed_9.x);
				float3 vertex_unnamed_202 = vertex_unnamed_9.xxx * vertex_unnamed_62.xyz;
				vertex_unnamed_62 = float4(vertex_unnamed_202.x, vertex_unnamed_202.y, vertex_unnamed_202.z, vertex_unnamed_62.w);
				vertex_unnamed_206 = vertex_unnamed_44.xyz * vertex_unnamed_62.xyz;
				vertex_unnamed_206 = (vertex_unnamed_44.zxy * vertex_unnamed_62.yzx) + (-vertex_unnamed_206);
				vertex_unnamed_9.x = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_206 = vertex_unnamed_9.xxx * vertex_unnamed_206;
				vertex_output_1.y = vertex_unnamed_206.x;
				vertex_output_1.x = vertex_unnamed_62.z;
				vertex_output_1.z = vertex_unnamed_44.y;
				vertex_output_2.x = vertex_unnamed_62.x;
				vertex_output_3.x = vertex_unnamed_62.y;
				vertex_output_2.z = vertex_unnamed_44.z;
				vertex_output_3.z = vertex_unnamed_44.x;
				vertex_output_2.w = vertex_unnamed_9.y;
				vertex_output_3.w = vertex_unnamed_9.z;
				vertex_output_2.y = vertex_unnamed_206.y;
				vertex_output_3.y = vertex_unnamed_206.z;
				vertex_output_4 = vertex_input_4;
				vertex_output_5 = 0.0f.xxxx;
				vertex_output_6 = 0.0f.xxxx;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;
			float4 unity_SpecCube0_BoxMax;
			float4 unity_SpecCube0_BoxMin;
			float4 unity_SpecCube0_ProbePosition;
			float4 unity_SpecCube0_HDR;
			float4 unity_SpecCube1_BoxMax;
			float4 unity_SpecCube1_BoxMin;
			float4 unity_SpecCube1_ProbePosition;
			float4 unity_SpecCube1_HDR;
			float4 _LightColor0;
			float4 _Color;

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			TextureCube<float4> unity_SpecCube0;
			SamplerState samplerunity_SpecCube0;
			TextureCube<float4> unity_SpecCube1;

			static float4 fragment_input_1;
			static float4 fragment_input_2;
			static float4 fragment_input_3;
			static float2 fragment_input_0;
			static float4 fragment_input_4;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float4 fragment_input_1 : TEXCOORD1; // vs_TEXCOORD1
				float4 fragment_input_2 : TEXCOORD2; // vs_TEXCOORD2
				float4 fragment_input_3 : TEXCOORD3; // vs_TEXCOORD3
				float4 fragment_input_4 : UNKNOWN4;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float3 fragment_unnamed_31;
			static float fragment_unnamed_43;
			static float3 fragment_unnamed_49;
			static float4 fragment_unnamed_55;
			static float3 fragment_unnamed_85;
			static float fragment_unnamed_110;
			static float4 fragment_unnamed_134;
			static bool fragment_unnamed_181;
			static float4 fragment_unnamed_198;
			static float3 fragment_unnamed_206;
			static float3 fragment_unnamed_218;
			static bool3 fragment_unnamed_232;
			static bool fragment_unnamed_347;
			static float fragment_unnamed_362;
			static float3 fragment_unnamed_375;
			static float3 fragment_unnamed_386;
			static bool3 fragment_unnamed_397;
			static float3 fragment_unnamed_572;
			static float fragment_unnamed_584;
			static float3 fragment_unnamed_612;

			void frag_main()
			{
				fragment_unnamed_9.x = fragment_input_1.w;
				fragment_unnamed_9.y = fragment_input_2.w;
				fragment_unnamed_9.z = fragment_input_3.w;
				fragment_unnamed_31 = (-fragment_unnamed_9) + _WorldSpaceCameraPos;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_49 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_55 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_55 *= _Color;
				float3 fragment_unnamed_82 = fragment_unnamed_55.xyz * fragment_input_4.xyz;
				fragment_unnamed_55 = float4(fragment_unnamed_82.x, fragment_unnamed_82.y, fragment_unnamed_82.z, fragment_unnamed_55.w);
				fragment_unnamed_85 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_85.x = fragment_unnamed_85.z * fragment_unnamed_85.x;
				float2 fragment_unnamed_107 = (fragment_unnamed_85.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_85 = float3(fragment_unnamed_107.x, fragment_unnamed_107.y, fragment_unnamed_85.z);
				fragment_unnamed_110 = dot(fragment_unnamed_85.xy, fragment_unnamed_85.xy);
				fragment_unnamed_110 = min(fragment_unnamed_110, 1.0f);
				fragment_unnamed_110 = (-fragment_unnamed_110) + 1.0f;
				fragment_unnamed_85.z = sqrt(fragment_unnamed_110);
				fragment_output_0.w = fragment_unnamed_55.w * fragment_input_4.w;
				fragment_unnamed_134.x = dot(fragment_input_1.xyz, fragment_unnamed_85);
				fragment_unnamed_134.y = dot(fragment_input_2.xyz, fragment_unnamed_85);
				fragment_unnamed_134.z = dot(fragment_input_3.xyz, fragment_unnamed_85);
				fragment_unnamed_110 = dot(fragment_unnamed_134.xyz, fragment_unnamed_134.xyz);
				fragment_unnamed_110 = rsqrt(fragment_unnamed_110);
				fragment_unnamed_85 = fragment_unnamed_110.xxx * fragment_unnamed_134.xyz;
				fragment_unnamed_110 = dot(-fragment_unnamed_49, fragment_unnamed_85);
				fragment_unnamed_110 += fragment_unnamed_110;
				float3 fragment_unnamed_176 = (fragment_unnamed_85 * (-fragment_unnamed_110.xxx)) + (-fragment_unnamed_49);
				fragment_unnamed_134 = float4(fragment_unnamed_176.x, fragment_unnamed_176.y, fragment_unnamed_176.z, fragment_unnamed_134.w);
				fragment_unnamed_181 = 0.0f < unity_SpecCube0_ProbePosition.w;
				if (fragment_unnamed_181)
				{
					fragment_unnamed_110 = dot(fragment_unnamed_134.xyz, fragment_unnamed_134.xyz);
					fragment_unnamed_110 = rsqrt(fragment_unnamed_110);
					float3 fragment_unnamed_203 = fragment_unnamed_110.xxx * fragment_unnamed_134.xyz;
					fragment_unnamed_198 = float4(fragment_unnamed_203.x, fragment_unnamed_203.y, fragment_unnamed_203.z, fragment_unnamed_198.w);
					fragment_unnamed_206 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMax.xyz;
					fragment_unnamed_206 /= fragment_unnamed_198.xyz;
					fragment_unnamed_218 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMin.xyz;
					fragment_unnamed_218 /= fragment_unnamed_198.xyz;
					fragment_unnamed_232 = bool4(0.0f.xxxx.x < fragment_unnamed_198.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_198.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_198.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_198.xyzx.w).xyz;
					float3 fragment_unnamed_240 = fragment_unnamed_206;
					float fragment_unnamed_245;
					if (fragment_unnamed_232.x)
					{
						fragment_unnamed_245 = fragment_unnamed_206.x;
					}
					else
					{
						fragment_unnamed_245 = fragment_unnamed_218.x;
					}
					fragment_unnamed_240.x = fragment_unnamed_245;
					float fragment_unnamed_257;
					if (fragment_unnamed_232.y)
					{
						fragment_unnamed_257 = fragment_unnamed_206.y;
					}
					else
					{
						fragment_unnamed_257 = fragment_unnamed_218.y;
					}
					fragment_unnamed_240.y = fragment_unnamed_257;
					float fragment_unnamed_269;
					if (fragment_unnamed_232.z)
					{
						fragment_unnamed_269 = fragment_unnamed_206.z;
					}
					else
					{
						fragment_unnamed_269 = fragment_unnamed_218.z;
					}
					fragment_unnamed_240.z = fragment_unnamed_269;
					fragment_unnamed_206 = fragment_unnamed_240;
					fragment_unnamed_110 = min(fragment_unnamed_206.y, fragment_unnamed_206.x);
					fragment_unnamed_110 = min(fragment_unnamed_206.z, fragment_unnamed_110);
					fragment_unnamed_206 = fragment_unnamed_9 + (-unity_SpecCube0_ProbePosition.xyz);
					float3 fragment_unnamed_301 = (fragment_unnamed_198.xyz * fragment_unnamed_110.xxx) + fragment_unnamed_206;
					fragment_unnamed_198 = float4(fragment_unnamed_301.x, fragment_unnamed_301.y, fragment_unnamed_301.z, fragment_unnamed_198.w);
				}
				else
				{
					fragment_unnamed_198 = float4(fragment_unnamed_134.xyz.x, fragment_unnamed_134.xyz.y, fragment_unnamed_134.xyz.z, fragment_unnamed_198.w);
				}
				fragment_unnamed_198 = unity_SpecCube0.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_198.xyz, 6.0f);
				fragment_unnamed_110 = fragment_unnamed_198.w + (-1.0f);
				fragment_unnamed_110 = (unity_SpecCube0_HDR.w * fragment_unnamed_110) + 1.0f;
				fragment_unnamed_110 = log2(fragment_unnamed_110);
				fragment_unnamed_110 *= unity_SpecCube0_HDR.y;
				fragment_unnamed_110 = exp2(fragment_unnamed_110);
				fragment_unnamed_110 *= unity_SpecCube0_HDR.x;
				fragment_unnamed_206 = fragment_unnamed_198.xyz * fragment_unnamed_110.xxx;
				fragment_unnamed_347 = unity_SpecCube0_BoxMin.w < 0.999989986419677734375f;
				if (fragment_unnamed_347)
				{
					fragment_unnamed_347 = 0.0f < unity_SpecCube1_ProbePosition.w;
					if (fragment_unnamed_347)
					{
						fragment_unnamed_362 = dot(fragment_unnamed_134.xyz, fragment_unnamed_134.xyz);
						fragment_unnamed_362 = rsqrt(fragment_unnamed_362);
						fragment_unnamed_218 = fragment_unnamed_362.xxx * fragment_unnamed_134.xyz;
						fragment_unnamed_375 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMax.xyz;
						fragment_unnamed_375 /= fragment_unnamed_218;
						fragment_unnamed_386 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMin.xyz;
						fragment_unnamed_386 /= fragment_unnamed_218;
						fragment_unnamed_397 = bool4(0.0f.xxxx.x < fragment_unnamed_218.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_218.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_218.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_218.xyzx.w).xyz;
						float3 fragment_unnamed_402 = fragment_unnamed_375;
						float fragment_unnamed_406;
						if (fragment_unnamed_397.x)
						{
							fragment_unnamed_406 = fragment_unnamed_375.x;
						}
						else
						{
							fragment_unnamed_406 = fragment_unnamed_386.x;
						}
						fragment_unnamed_402.x = fragment_unnamed_406;
						float fragment_unnamed_418;
						if (fragment_unnamed_397.y)
						{
							fragment_unnamed_418 = fragment_unnamed_375.y;
						}
						else
						{
							fragment_unnamed_418 = fragment_unnamed_386.y;
						}
						fragment_unnamed_402.y = fragment_unnamed_418;
						float fragment_unnamed_430;
						if (fragment_unnamed_397.z)
						{
							fragment_unnamed_430 = fragment_unnamed_375.z;
						}
						else
						{
							fragment_unnamed_430 = fragment_unnamed_386.z;
						}
						fragment_unnamed_402.z = fragment_unnamed_430;
						fragment_unnamed_375 = fragment_unnamed_402;
						fragment_unnamed_362 = min(fragment_unnamed_375.y, fragment_unnamed_375.x);
						fragment_unnamed_362 = min(fragment_unnamed_375.z, fragment_unnamed_362);
						fragment_unnamed_9 += (-unity_SpecCube1_ProbePosition.xyz);
						float3 fragment_unnamed_461 = (fragment_unnamed_218 * fragment_unnamed_362.xxx) + fragment_unnamed_9;
						fragment_unnamed_134 = float4(fragment_unnamed_461.x, fragment_unnamed_461.y, fragment_unnamed_461.z, fragment_unnamed_134.w);
					}
					fragment_unnamed_134 = unity_SpecCube1.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_134.xyz, 6.0f);
					fragment_unnamed_9.x = fragment_unnamed_134.w + (-1.0f);
					fragment_unnamed_9.x = (unity_SpecCube1_HDR.w * fragment_unnamed_9.x) + 1.0f;
					fragment_unnamed_9.x = log2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.y;
					fragment_unnamed_9.x = exp2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.x;
					fragment_unnamed_9 = fragment_unnamed_134.xyz * fragment_unnamed_9.xxx;
					float3 fragment_unnamed_515 = (fragment_unnamed_110.xxx * fragment_unnamed_198.xyz) + (-fragment_unnamed_9);
					fragment_unnamed_134 = float4(fragment_unnamed_515.x, fragment_unnamed_515.y, fragment_unnamed_515.z, fragment_unnamed_134.w);
					fragment_unnamed_206 = (unity_SpecCube0_BoxMin.www * fragment_unnamed_134.xyz) + fragment_unnamed_9;
				}
				fragment_unnamed_9 = fragment_unnamed_55.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = max(fragment_unnamed_43, 0.001000000047497451305389404296875f);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_31 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_43 = dot(fragment_unnamed_85, fragment_unnamed_49);
				fragment_unnamed_110 = dot(fragment_unnamed_85, _WorldSpaceLightPos0.xyz);
				fragment_unnamed_110 = clamp(fragment_unnamed_110, 0.0f, 1.0f);
				fragment_unnamed_31.x = dot(_WorldSpaceLightPos0.xyz, fragment_unnamed_31);
				fragment_unnamed_31.x = clamp(fragment_unnamed_31.x, 0.0f, 1.0f);
				fragment_unnamed_572.x = dot(fragment_unnamed_31.xx, fragment_unnamed_31.xx);
				fragment_unnamed_572.x += (-0.5f);
				fragment_unnamed_584 = (-fragment_unnamed_110) + 1.0f;
				fragment_unnamed_49.x = fragment_unnamed_584 * fragment_unnamed_584;
				fragment_unnamed_49.x *= fragment_unnamed_49.x;
				fragment_unnamed_584 *= fragment_unnamed_49.x;
				fragment_unnamed_584 = (fragment_unnamed_572.x * fragment_unnamed_584) + 1.0f;
				fragment_unnamed_49.x = (-abs(fragment_unnamed_43)) + 1.0f;
				fragment_unnamed_612.x = fragment_unnamed_49.x * fragment_unnamed_49.x;
				fragment_unnamed_612.x *= fragment_unnamed_612.x;
				fragment_unnamed_49.x *= fragment_unnamed_612.x;
				fragment_unnamed_572.x = (fragment_unnamed_572.x * fragment_unnamed_49.x) + 1.0f;
				fragment_unnamed_572.x *= fragment_unnamed_584;
				fragment_unnamed_572.x = fragment_unnamed_110 * fragment_unnamed_572.x;
				fragment_unnamed_43 = abs(fragment_unnamed_43) + fragment_unnamed_110;
				fragment_unnamed_43 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_43 = 0.5f / fragment_unnamed_43;
				fragment_unnamed_43 = fragment_unnamed_110 * fragment_unnamed_43;
				fragment_unnamed_43 *= 0.99999988079071044921875f;
				fragment_unnamed_572 = fragment_unnamed_572.xxx * _LightColor0.xyz;
				fragment_unnamed_612 = fragment_unnamed_43.xxx * _LightColor0.xyz;
				fragment_unnamed_43 = (-fragment_unnamed_31.x) + 1.0f;
				fragment_unnamed_31.x = fragment_unnamed_43 * fragment_unnamed_43;
				fragment_unnamed_31.x *= fragment_unnamed_31.x;
				fragment_unnamed_43 *= fragment_unnamed_31.x;
				fragment_unnamed_43 = (fragment_unnamed_43 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				fragment_unnamed_612 = fragment_unnamed_43.xxx * fragment_unnamed_612;
				fragment_unnamed_9 = (fragment_unnamed_9 * fragment_unnamed_572) + fragment_unnamed_612;
				fragment_unnamed_31 = fragment_unnamed_206 * 0.5f.xxx;
				fragment_unnamed_43 = (fragment_unnamed_49.x * 2.2351741790771484375e-08f) + 0.039999999105930328369140625f;
				float3 fragment_unnamed_721 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + fragment_unnamed_9;
				fragment_output_0 = float4(fragment_unnamed_721.x, fragment_unnamed_721.y, fragment_unnamed_721.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_4 = stage_input.fragment_input_4;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // !FOG_LINEAR
			#endif // !LIGHTPROBE_SH
			#endif // !VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifdef LIGHTPROBE_SH
			#ifndef FOG_LINEAR
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4 unity_SHBr;
			float4 unity_SHBg;
			float4 unity_SHBb;
			float4 unity_SHC;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[6];
			static float4 vertex_uniform_buffer_1[46];
			static float4 vertex_uniform_buffer_2[10];
			static float4 vertex_uniform_buffer_3[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float4 vertex_output_2;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_output_6;
			static float3 vertex_output_7;
			static float4 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_output_5 : COLOR; // COLOR
				float4 vertex_output_6 : TEXCOORD4; // TEXCOORD_4
				float3 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float4 vertex_output_8 : TEXCOORD8; // TEXCOORD_8
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_64 = vertex_input_0.y * vertex_uniform_buffer_2[1u].x;
				precise float vertex_unnamed_65 = vertex_input_0.y * vertex_uniform_buffer_2[1u].y;
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_2[1u].z;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_2[1u].w;
				float vertex_unnamed_90 = mad(vertex_uniform_buffer_2[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].x, vertex_input_0.x, vertex_unnamed_64));
				float vertex_unnamed_91 = mad(vertex_uniform_buffer_2[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].y, vertex_input_0.x, vertex_unnamed_65));
				float vertex_unnamed_92 = mad(vertex_uniform_buffer_2[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].z, vertex_input_0.x, vertex_unnamed_66));
				precise float vertex_unnamed_101 = vertex_unnamed_90 + vertex_uniform_buffer_2[3u].x;
				precise float vertex_unnamed_102 = vertex_unnamed_91 + vertex_uniform_buffer_2[3u].y;
				precise float vertex_unnamed_103 = vertex_unnamed_92 + vertex_uniform_buffer_2[3u].z;
				precise float vertex_unnamed_104 = mad(vertex_uniform_buffer_2[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].w, vertex_input_0.x, vertex_unnamed_67)) + vertex_uniform_buffer_2[3u].w;
				precise float vertex_unnamed_122 = vertex_unnamed_102 * vertex_uniform_buffer_3[18u].x;
				precise float vertex_unnamed_123 = vertex_unnamed_102 * vertex_uniform_buffer_3[18u].y;
				precise float vertex_unnamed_124 = vertex_unnamed_102 * vertex_uniform_buffer_3[18u].z;
				precise float vertex_unnamed_125 = vertex_unnamed_102 * vertex_uniform_buffer_3[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_3[20u].x, vertex_unnamed_104, mad(vertex_uniform_buffer_3[19u].x, vertex_unnamed_103, mad(vertex_uniform_buffer_3[17u].x, vertex_unnamed_101, vertex_unnamed_122)));
				gl_Position.y = mad(vertex_uniform_buffer_3[20u].y, vertex_unnamed_104, mad(vertex_uniform_buffer_3[19u].y, vertex_unnamed_103, mad(vertex_uniform_buffer_3[17u].y, vertex_unnamed_101, vertex_unnamed_123)));
				gl_Position.z = mad(vertex_uniform_buffer_3[20u].z, vertex_unnamed_104, mad(vertex_uniform_buffer_3[19u].z, vertex_unnamed_103, mad(vertex_uniform_buffer_3[17u].z, vertex_unnamed_101, vertex_unnamed_124)));
				gl_Position.w = mad(vertex_uniform_buffer_3[20u].w, vertex_unnamed_104, mad(vertex_uniform_buffer_3[19u].w, vertex_unnamed_103, mad(vertex_uniform_buffer_3[17u].w, vertex_unnamed_101, vertex_unnamed_125)));
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[5u].x, vertex_uniform_buffer_0[5u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[5u].y, vertex_uniform_buffer_0[5u].w);
				vertex_output_2.w = mad(vertex_uniform_buffer_2[3u].x, vertex_input_0.w, vertex_unnamed_90);
				precise float vertex_unnamed_189 = vertex_input_1.y * vertex_uniform_buffer_2[1u].y;
				precise float vertex_unnamed_190 = vertex_input_1.y * vertex_uniform_buffer_2[1u].z;
				precise float vertex_unnamed_191 = vertex_input_1.y * vertex_uniform_buffer_2[1u].x;
				float vertex_unnamed_209 = mad(vertex_uniform_buffer_2[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].y, vertex_input_1.x, vertex_unnamed_189));
				float vertex_unnamed_210 = mad(vertex_uniform_buffer_2[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].z, vertex_input_1.x, vertex_unnamed_190));
				float vertex_unnamed_211 = mad(vertex_uniform_buffer_2[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].x, vertex_input_1.x, vertex_unnamed_191));
				float vertex_unnamed_215 = rsqrt(dot(float3(vertex_unnamed_209, vertex_unnamed_210, vertex_unnamed_211), float3(vertex_unnamed_209, vertex_unnamed_210, vertex_unnamed_211)));
				precise float vertex_unnamed_216 = vertex_unnamed_215 * vertex_unnamed_209;
				precise float vertex_unnamed_217 = vertex_unnamed_215 * vertex_unnamed_210;
				precise float vertex_unnamed_218 = vertex_unnamed_215 * vertex_unnamed_211;
				vertex_output_2.x = vertex_unnamed_218;
				float vertex_unnamed_232 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[4u].xyz));
				float vertex_unnamed_246 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[5u].xyz));
				float vertex_unnamed_260 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[6u].xyz));
				float vertex_unnamed_266 = rsqrt(dot(float3(vertex_unnamed_232, vertex_unnamed_246, vertex_unnamed_260), float3(vertex_unnamed_232, vertex_unnamed_246, vertex_unnamed_260)));
				precise float vertex_unnamed_267 = vertex_unnamed_266 * vertex_unnamed_232;
				precise float vertex_unnamed_268 = vertex_unnamed_266 * vertex_unnamed_246;
				precise float vertex_unnamed_269 = vertex_unnamed_266 * vertex_unnamed_260;
				precise float vertex_unnamed_270 = vertex_unnamed_266 * vertex_unnamed_260;
				precise float vertex_unnamed_271 = vertex_unnamed_216 * vertex_unnamed_270;
				precise float vertex_unnamed_272 = vertex_unnamed_217 * vertex_unnamed_267;
				precise float vertex_unnamed_273 = vertex_unnamed_218 * vertex_unnamed_268;
				precise float vertex_unnamed_274 = (-0.0f) - vertex_unnamed_271;
				precise float vertex_unnamed_276 = (-0.0f) - vertex_unnamed_272;
				precise float vertex_unnamed_277 = (-0.0f) - vertex_unnamed_273;
				precise float vertex_unnamed_287 = vertex_input_1.w * vertex_uniform_buffer_2[9u].w;
				precise float vertex_unnamed_288 = vertex_unnamed_287 * mad(vertex_unnamed_268, vertex_unnamed_217, vertex_unnamed_274);
				precise float vertex_unnamed_289 = vertex_unnamed_287 * mad(vertex_unnamed_270, vertex_unnamed_218, vertex_unnamed_276);
				precise float vertex_unnamed_290 = vertex_unnamed_287 * mad(vertex_unnamed_267, vertex_unnamed_216, vertex_unnamed_277);
				vertex_output_2.y = vertex_unnamed_288;
				vertex_output_2.z = vertex_unnamed_267;
				vertex_output_3.x = vertex_unnamed_216;
				vertex_output_4.x = vertex_unnamed_217;
				vertex_output_3.w = mad(vertex_uniform_buffer_2[3u].y, vertex_input_0.w, vertex_unnamed_91);
				vertex_output_4.w = mad(vertex_uniform_buffer_2[3u].z, vertex_input_0.w, vertex_unnamed_92);
				vertex_output_3.y = vertex_unnamed_289;
				vertex_output_4.y = vertex_unnamed_290;
				vertex_output_3.z = vertex_unnamed_268;
				vertex_output_4.z = vertex_unnamed_270;
				vertex_output_5.x = vertex_input_7.x;
				vertex_output_5.y = vertex_input_7.y;
				vertex_output_5.z = vertex_input_7.z;
				vertex_output_5.w = vertex_input_7.w;
				vertex_output_6.x = 0.0f;
				vertex_output_6.y = 0.0f;
				vertex_output_6.z = 0.0f;
				vertex_output_6.w = 0.0f;
				precise float vertex_unnamed_318 = vertex_unnamed_268 * vertex_unnamed_268;
				precise float vertex_unnamed_319 = (-0.0f) - vertex_unnamed_318;
				float vertex_unnamed_320 = mad(vertex_unnamed_267, vertex_unnamed_267, vertex_unnamed_319);
				precise float vertex_unnamed_321 = vertex_unnamed_268 * vertex_unnamed_267;
				precise float vertex_unnamed_322 = vertex_unnamed_270 * vertex_unnamed_268;
				precise float vertex_unnamed_323 = vertex_unnamed_269 * vertex_unnamed_269;
				precise float vertex_unnamed_324 = vertex_unnamed_267 * vertex_unnamed_270;
				vertex_output_7.x = mad(vertex_uniform_buffer_1[45u].x, vertex_unnamed_320, dot(float4(vertex_uniform_buffer_1[42u]), float4(vertex_unnamed_321, vertex_unnamed_322, vertex_unnamed_323, vertex_unnamed_324)));
				vertex_output_7.y = mad(vertex_uniform_buffer_1[45u].y, vertex_unnamed_320, dot(float4(vertex_uniform_buffer_1[43u]), float4(vertex_unnamed_321, vertex_unnamed_322, vertex_unnamed_323, vertex_unnamed_324)));
				vertex_output_7.z = mad(vertex_uniform_buffer_1[45u].z, vertex_unnamed_320, dot(float4(vertex_uniform_buffer_1[44u]), float4(vertex_unnamed_321, vertex_unnamed_322, vertex_unnamed_323, vertex_unnamed_324)));
				vertex_output_8.x = 0.0f;
				vertex_output_8.y = 0.0f;
				vertex_output_8.z = 0.0f;
				vertex_output_8.w = 0.0f;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[5] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[42] = float4(unity_SHBr[0], unity_SHBr[1], unity_SHBr[2], unity_SHBr[3]);

				vertex_uniform_buffer_1[43] = float4(unity_SHBg[0], unity_SHBg[1], unity_SHBg[2], unity_SHBg[3]);

				vertex_uniform_buffer_1[44] = float4(unity_SHBb[0], unity_SHBb[1], unity_SHBb[2], unity_SHBb[3]);

				vertex_uniform_buffer_1[45] = float4(unity_SHC[0], unity_SHC[1], unity_SHC[2], unity_SHC[3]);

				vertex_uniform_buffer_2[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_2[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_2[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_2[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_2[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_2[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_2[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_2[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_3[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_3[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_3[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_3[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // LIGHTPROBE_SH
			#endif // !FOG_LINEAR
			#endif // !VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifdef LIGHTPROBE_SH
			#ifndef FOG_LINEAR
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 unity_SHBr;
			float4 unity_SHBg;
			float4 unity_SHBb;
			float4 unity_SHC;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float4 vertex_output_1;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_output_2;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_input_4;
			static float4 vertex_output_6;
			static float3 vertex_output_5;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float4 vertex_output_1 : TEXCOORD1; // vs_TEXCOORD1
				float4 vertex_output_2 : TEXCOORD2; // vs_TEXCOORD2
				float4 vertex_output_3 : TEXCOORD3; // vs_TEXCOORD3
				float4 vertex_output_4 : UNKNOWN4;
				float3 vertex_output_5 : TEXCOORD5; // vs_TEXCOORD5
				float4 vertex_output_6 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_7 : TEXCOORD8; // vs_TEXCOORD8
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_45;
			static float4 vertex_unnamed_63;
			static float3 vertex_unnamed_210;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_45 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				float3 vertex_unnamed_60 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_60.x, vertex_unnamed_60.y, vertex_unnamed_60.z, vertex_unnamed_9.w);
				vertex_unnamed_63 = vertex_unnamed_45.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_63 = (unity_MatrixVP__array[0] * vertex_unnamed_45.xxxx) + vertex_unnamed_63;
				vertex_unnamed_63 = (unity_MatrixVP__array[2] * vertex_unnamed_45.zzzz) + vertex_unnamed_63;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_45.wwww) + vertex_unnamed_63;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_output_1.w = vertex_unnamed_9.x;
				float3 vertex_unnamed_127 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_45 = float4(vertex_unnamed_127.x, vertex_unnamed_127.y, vertex_unnamed_127.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_138 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_138.x, vertex_unnamed_138.y, vertex_unnamed_138.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_149 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_149.x, vertex_unnamed_149.y, vertex_unnamed_149.z, vertex_unnamed_45.w);
				vertex_unnamed_9.x = dot(vertex_unnamed_45.xyz, vertex_unnamed_45.xyz);
				vertex_unnamed_9.x = rsqrt(vertex_unnamed_9.x);
				float3 vertex_unnamed_166 = vertex_unnamed_9.xxx * vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_166.x, vertex_unnamed_166.y, vertex_unnamed_166.z, vertex_unnamed_45.w);
				vertex_output_1.x = vertex_unnamed_45.z;
				vertex_unnamed_63.x = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_63.y = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_63.z = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_9.x = dot(vertex_unnamed_63.xyz, vertex_unnamed_63.xyz);
				vertex_unnamed_9.x = rsqrt(vertex_unnamed_9.x);
				vertex_unnamed_63 = vertex_unnamed_9.xxxx * vertex_unnamed_63.xyzz;
				vertex_unnamed_210 = vertex_unnamed_45.xyz * vertex_unnamed_63.wxy;
				vertex_unnamed_210 = (vertex_unnamed_63.ywx * vertex_unnamed_45.yzx) + (-vertex_unnamed_210);
				vertex_unnamed_9.x = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_210 = vertex_unnamed_9.xxx * vertex_unnamed_210;
				vertex_output_1.y = vertex_unnamed_210.x;
				vertex_output_1.z = vertex_unnamed_63.x;
				vertex_output_2.x = vertex_unnamed_45.x;
				vertex_output_3.x = vertex_unnamed_45.y;
				vertex_output_2.w = vertex_unnamed_9.y;
				vertex_output_3.w = vertex_unnamed_9.z;
				vertex_output_2.y = vertex_unnamed_210.y;
				vertex_output_3.y = vertex_unnamed_210.z;
				vertex_output_2.z = vertex_unnamed_63.y;
				vertex_output_3.z = vertex_unnamed_63.w;
				vertex_output_4 = vertex_input_4;
				vertex_output_6 = 0.0f.xxxx;
				vertex_unnamed_9.x = vertex_unnamed_63.y * vertex_unnamed_63.y;
				vertex_unnamed_9.x = (vertex_unnamed_63.x * vertex_unnamed_63.x) + (-vertex_unnamed_9.x);
				vertex_unnamed_45 = vertex_unnamed_63.ywzx * vertex_unnamed_63;
				vertex_unnamed_63.x = dot(unity_SHBr, vertex_unnamed_45);
				vertex_unnamed_63.y = dot(unity_SHBg, vertex_unnamed_45);
				vertex_unnamed_63.z = dot(unity_SHBb, vertex_unnamed_45);
				vertex_output_5 = (unity_SHC.xyz * vertex_unnamed_9.xxx) + vertex_unnamed_63.xyz;
				vertex_output_7 = 0.0f.xxxx;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;
			float4 unity_SHAr;
			float4 unity_SHAg;
			float4 unity_SHAb;
			float4 unity_SpecCube0_BoxMax;
			float4 unity_SpecCube0_BoxMin;
			float4 unity_SpecCube0_ProbePosition;
			float4 unity_SpecCube0_HDR;
			float4 unity_SpecCube1_BoxMax;
			float4 unity_SpecCube1_BoxMin;
			float4 unity_SpecCube1_ProbePosition;
			float4 unity_SpecCube1_HDR;
			float4 _LightColor0;
			float4 _Color;

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			TextureCube<float4> unity_SpecCube0;
			SamplerState samplerunity_SpecCube0;
			TextureCube<float4> unity_SpecCube1;

			static float4 fragment_input_1;
			static float4 fragment_input_2;
			static float4 fragment_input_3;
			static float2 fragment_input_0;
			static float4 fragment_input_4;
			static float4 fragment_output_0;
			static float3 fragment_input_5;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float4 fragment_input_1 : TEXCOORD1; // vs_TEXCOORD1
				float4 fragment_input_2 : TEXCOORD2; // vs_TEXCOORD2
				float4 fragment_input_3 : TEXCOORD3; // vs_TEXCOORD3
				float4 fragment_input_4 : UNKNOWN4;
				float3 fragment_input_5 : TEXCOORD5; // vs_TEXCOORD5
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float3 fragment_unnamed_31;
			static float fragment_unnamed_43;
			static float3 fragment_unnamed_49;
			static float4 fragment_unnamed_55;
			static float4 fragment_unnamed_85;
			static float fragment_unnamed_112;
			static float4 fragment_unnamed_136;
			static float3 fragment_unnamed_189;
			static bool fragment_unnamed_219;
			static float4 fragment_unnamed_235;
			static float3 fragment_unnamed_243;
			static float3 fragment_unnamed_255;
			static bool3 fragment_unnamed_269;
			static bool fragment_unnamed_384;
			static float fragment_unnamed_399;
			static float3 fragment_unnamed_412;
			static float3 fragment_unnamed_423;
			static bool3 fragment_unnamed_434;
			static float3 fragment_unnamed_611;
			static float fragment_unnamed_623;
			static float3 fragment_unnamed_651;

			void frag_main()
			{
				fragment_unnamed_9.x = fragment_input_1.w;
				fragment_unnamed_9.y = fragment_input_2.w;
				fragment_unnamed_9.z = fragment_input_3.w;
				fragment_unnamed_31 = (-fragment_unnamed_9) + _WorldSpaceCameraPos;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_49 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_55 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_55 *= _Color;
				float3 fragment_unnamed_82 = fragment_unnamed_55.xyz * fragment_input_4.xyz;
				fragment_unnamed_55 = float4(fragment_unnamed_82.x, fragment_unnamed_82.y, fragment_unnamed_82.z, fragment_unnamed_55.w);
				float3 fragment_unnamed_93 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_85 = float4(fragment_unnamed_93.x, fragment_unnamed_93.y, fragment_unnamed_93.z, fragment_unnamed_85.w);
				fragment_unnamed_85.x = fragment_unnamed_85.z * fragment_unnamed_85.x;
				float2 fragment_unnamed_109 = (fragment_unnamed_85.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_85 = float4(fragment_unnamed_109.x, fragment_unnamed_109.y, fragment_unnamed_85.z, fragment_unnamed_85.w);
				fragment_unnamed_112 = dot(fragment_unnamed_85.xy, fragment_unnamed_85.xy);
				fragment_unnamed_112 = min(fragment_unnamed_112, 1.0f);
				fragment_unnamed_112 = (-fragment_unnamed_112) + 1.0f;
				fragment_unnamed_85.z = sqrt(fragment_unnamed_112);
				fragment_output_0.w = fragment_unnamed_55.w * fragment_input_4.w;
				fragment_unnamed_136.x = dot(fragment_input_1.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_136.y = dot(fragment_input_2.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_136.z = dot(fragment_input_3.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_112 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
				fragment_unnamed_112 = rsqrt(fragment_unnamed_112);
				float3 fragment_unnamed_166 = fragment_unnamed_112.xxx * fragment_unnamed_136.xyz;
				fragment_unnamed_85 = float4(fragment_unnamed_166.x, fragment_unnamed_166.y, fragment_unnamed_166.z, fragment_unnamed_85.w);
				fragment_unnamed_112 = dot(-fragment_unnamed_49, fragment_unnamed_85.xyz);
				fragment_unnamed_112 += fragment_unnamed_112;
				float3 fragment_unnamed_185 = (fragment_unnamed_85.xyz * (-fragment_unnamed_112.xxx)) + (-fragment_unnamed_49);
				fragment_unnamed_136 = float4(fragment_unnamed_185.x, fragment_unnamed_185.y, fragment_unnamed_185.z, fragment_unnamed_136.w);
				fragment_unnamed_85.w = 1.0f;
				fragment_unnamed_189.x = dot(unity_SHAr, fragment_unnamed_85);
				fragment_unnamed_189.y = dot(unity_SHAg, fragment_unnamed_85);
				fragment_unnamed_189.z = dot(unity_SHAb, fragment_unnamed_85);
				fragment_unnamed_189 += fragment_input_5;
				fragment_unnamed_189 = max(fragment_unnamed_189, 0.0f.xxx);
				fragment_unnamed_219 = 0.0f < unity_SpecCube0_ProbePosition.w;
				if (fragment_unnamed_219)
				{
					fragment_unnamed_112 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
					fragment_unnamed_112 = rsqrt(fragment_unnamed_112);
					float3 fragment_unnamed_240 = fragment_unnamed_112.xxx * fragment_unnamed_136.xyz;
					fragment_unnamed_235 = float4(fragment_unnamed_240.x, fragment_unnamed_240.y, fragment_unnamed_240.z, fragment_unnamed_235.w);
					fragment_unnamed_243 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMax.xyz;
					fragment_unnamed_243 /= fragment_unnamed_235.xyz;
					fragment_unnamed_255 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMin.xyz;
					fragment_unnamed_255 /= fragment_unnamed_235.xyz;
					fragment_unnamed_269 = bool4(0.0f.xxxx.x < fragment_unnamed_235.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_235.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_235.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_235.xyzx.w).xyz;
					float3 fragment_unnamed_277 = fragment_unnamed_243;
					float fragment_unnamed_282;
					if (fragment_unnamed_269.x)
					{
						fragment_unnamed_282 = fragment_unnamed_243.x;
					}
					else
					{
						fragment_unnamed_282 = fragment_unnamed_255.x;
					}
					fragment_unnamed_277.x = fragment_unnamed_282;
					float fragment_unnamed_294;
					if (fragment_unnamed_269.y)
					{
						fragment_unnamed_294 = fragment_unnamed_243.y;
					}
					else
					{
						fragment_unnamed_294 = fragment_unnamed_255.y;
					}
					fragment_unnamed_277.y = fragment_unnamed_294;
					float fragment_unnamed_306;
					if (fragment_unnamed_269.z)
					{
						fragment_unnamed_306 = fragment_unnamed_243.z;
					}
					else
					{
						fragment_unnamed_306 = fragment_unnamed_255.z;
					}
					fragment_unnamed_277.z = fragment_unnamed_306;
					fragment_unnamed_243 = fragment_unnamed_277;
					fragment_unnamed_112 = min(fragment_unnamed_243.y, fragment_unnamed_243.x);
					fragment_unnamed_112 = min(fragment_unnamed_243.z, fragment_unnamed_112);
					fragment_unnamed_243 = fragment_unnamed_9 + (-unity_SpecCube0_ProbePosition.xyz);
					float3 fragment_unnamed_338 = (fragment_unnamed_235.xyz * fragment_unnamed_112.xxx) + fragment_unnamed_243;
					fragment_unnamed_235 = float4(fragment_unnamed_338.x, fragment_unnamed_338.y, fragment_unnamed_338.z, fragment_unnamed_235.w);
				}
				else
				{
					fragment_unnamed_235 = float4(fragment_unnamed_136.xyz.x, fragment_unnamed_136.xyz.y, fragment_unnamed_136.xyz.z, fragment_unnamed_235.w);
				}
				fragment_unnamed_235 = unity_SpecCube0.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_235.xyz, 6.0f);
				fragment_unnamed_112 = fragment_unnamed_235.w + (-1.0f);
				fragment_unnamed_112 = (unity_SpecCube0_HDR.w * fragment_unnamed_112) + 1.0f;
				fragment_unnamed_112 = log2(fragment_unnamed_112);
				fragment_unnamed_112 *= unity_SpecCube0_HDR.y;
				fragment_unnamed_112 = exp2(fragment_unnamed_112);
				fragment_unnamed_112 *= unity_SpecCube0_HDR.x;
				fragment_unnamed_243 = fragment_unnamed_235.xyz * fragment_unnamed_112.xxx;
				fragment_unnamed_384 = unity_SpecCube0_BoxMin.w < 0.999989986419677734375f;
				if (fragment_unnamed_384)
				{
					fragment_unnamed_384 = 0.0f < unity_SpecCube1_ProbePosition.w;
					if (fragment_unnamed_384)
					{
						fragment_unnamed_399 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
						fragment_unnamed_399 = rsqrt(fragment_unnamed_399);
						fragment_unnamed_255 = fragment_unnamed_399.xxx * fragment_unnamed_136.xyz;
						fragment_unnamed_412 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMax.xyz;
						fragment_unnamed_412 /= fragment_unnamed_255;
						fragment_unnamed_423 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMin.xyz;
						fragment_unnamed_423 /= fragment_unnamed_255;
						fragment_unnamed_434 = bool4(0.0f.xxxx.x < fragment_unnamed_255.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_255.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_255.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_255.xyzx.w).xyz;
						float3 fragment_unnamed_439 = fragment_unnamed_412;
						float fragment_unnamed_443;
						if (fragment_unnamed_434.x)
						{
							fragment_unnamed_443 = fragment_unnamed_412.x;
						}
						else
						{
							fragment_unnamed_443 = fragment_unnamed_423.x;
						}
						fragment_unnamed_439.x = fragment_unnamed_443;
						float fragment_unnamed_455;
						if (fragment_unnamed_434.y)
						{
							fragment_unnamed_455 = fragment_unnamed_412.y;
						}
						else
						{
							fragment_unnamed_455 = fragment_unnamed_423.y;
						}
						fragment_unnamed_439.y = fragment_unnamed_455;
						float fragment_unnamed_467;
						if (fragment_unnamed_434.z)
						{
							fragment_unnamed_467 = fragment_unnamed_412.z;
						}
						else
						{
							fragment_unnamed_467 = fragment_unnamed_423.z;
						}
						fragment_unnamed_439.z = fragment_unnamed_467;
						fragment_unnamed_412 = fragment_unnamed_439;
						fragment_unnamed_399 = min(fragment_unnamed_412.y, fragment_unnamed_412.x);
						fragment_unnamed_399 = min(fragment_unnamed_412.z, fragment_unnamed_399);
						fragment_unnamed_9 += (-unity_SpecCube1_ProbePosition.xyz);
						float3 fragment_unnamed_498 = (fragment_unnamed_255 * fragment_unnamed_399.xxx) + fragment_unnamed_9;
						fragment_unnamed_136 = float4(fragment_unnamed_498.x, fragment_unnamed_498.y, fragment_unnamed_498.z, fragment_unnamed_136.w);
					}
					fragment_unnamed_136 = unity_SpecCube1.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_136.xyz, 6.0f);
					fragment_unnamed_9.x = fragment_unnamed_136.w + (-1.0f);
					fragment_unnamed_9.x = (unity_SpecCube1_HDR.w * fragment_unnamed_9.x) + 1.0f;
					fragment_unnamed_9.x = log2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.y;
					fragment_unnamed_9.x = exp2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.x;
					fragment_unnamed_9 = fragment_unnamed_136.xyz * fragment_unnamed_9.xxx;
					float3 fragment_unnamed_552 = (fragment_unnamed_112.xxx * fragment_unnamed_235.xyz) + (-fragment_unnamed_9);
					fragment_unnamed_136 = float4(fragment_unnamed_552.x, fragment_unnamed_552.y, fragment_unnamed_552.z, fragment_unnamed_136.w);
					fragment_unnamed_243 = (unity_SpecCube0_BoxMin.www * fragment_unnamed_136.xyz) + fragment_unnamed_9;
				}
				fragment_unnamed_9 = fragment_unnamed_55.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = max(fragment_unnamed_43, 0.001000000047497451305389404296875f);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_31 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_43 = dot(fragment_unnamed_85.xyz, fragment_unnamed_49);
				fragment_unnamed_112 = dot(fragment_unnamed_85.xyz, _WorldSpaceLightPos0.xyz);
				fragment_unnamed_112 = clamp(fragment_unnamed_112, 0.0f, 1.0f);
				fragment_unnamed_31.x = dot(_WorldSpaceLightPos0.xyz, fragment_unnamed_31);
				fragment_unnamed_31.x = clamp(fragment_unnamed_31.x, 0.0f, 1.0f);
				fragment_unnamed_611.x = dot(fragment_unnamed_31.xx, fragment_unnamed_31.xx);
				fragment_unnamed_611.x += (-0.5f);
				fragment_unnamed_623 = (-fragment_unnamed_112) + 1.0f;
				fragment_unnamed_49.x = fragment_unnamed_623 * fragment_unnamed_623;
				fragment_unnamed_49.x *= fragment_unnamed_49.x;
				fragment_unnamed_623 *= fragment_unnamed_49.x;
				fragment_unnamed_623 = (fragment_unnamed_611.x * fragment_unnamed_623) + 1.0f;
				fragment_unnamed_49.x = (-abs(fragment_unnamed_43)) + 1.0f;
				fragment_unnamed_651.x = fragment_unnamed_49.x * fragment_unnamed_49.x;
				fragment_unnamed_651.x *= fragment_unnamed_651.x;
				fragment_unnamed_49.x *= fragment_unnamed_651.x;
				fragment_unnamed_611.x = (fragment_unnamed_611.x * fragment_unnamed_49.x) + 1.0f;
				fragment_unnamed_611.x *= fragment_unnamed_623;
				fragment_unnamed_611.x = fragment_unnamed_112 * fragment_unnamed_611.x;
				fragment_unnamed_43 = abs(fragment_unnamed_43) + fragment_unnamed_112;
				fragment_unnamed_43 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_43 = 0.5f / fragment_unnamed_43;
				fragment_unnamed_43 = fragment_unnamed_112 * fragment_unnamed_43;
				fragment_unnamed_43 *= 0.99999988079071044921875f;
				fragment_unnamed_611 = (_LightColor0.xyz * fragment_unnamed_611.xxx) + fragment_unnamed_189;
				fragment_unnamed_651 = fragment_unnamed_43.xxx * _LightColor0.xyz;
				fragment_unnamed_43 = (-fragment_unnamed_31.x) + 1.0f;
				fragment_unnamed_31.x = fragment_unnamed_43 * fragment_unnamed_43;
				fragment_unnamed_31.x *= fragment_unnamed_31.x;
				fragment_unnamed_43 *= fragment_unnamed_31.x;
				fragment_unnamed_43 = (fragment_unnamed_43 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				fragment_unnamed_651 = fragment_unnamed_43.xxx * fragment_unnamed_651;
				fragment_unnamed_9 = (fragment_unnamed_9 * fragment_unnamed_611) + fragment_unnamed_651;
				fragment_unnamed_31 = fragment_unnamed_243 * 0.5f.xxx;
				fragment_unnamed_43 = (fragment_unnamed_49.x * 2.2351741790771484375e-08f) + 0.039999999105930328369140625f;
				float3 fragment_unnamed_762 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + fragment_unnamed_9;
				fragment_output_0 = float4(fragment_unnamed_762.x, fragment_unnamed_762.y, fragment_unnamed_762.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // LIGHTPROBE_SH
			#endif // !FOG_LINEAR
			#endif // !VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifdef LIGHTPROBE_SH
			#ifdef VERTEXLIGHT_ON
			#ifndef FOG_LINEAR
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4 unity_4LightPosX0;
			float4 unity_4LightPosY0;
			float4 unity_4LightPosZ0;
			float4 unity_4LightAtten0;
			float4 unity_LightColor[8];
			float4 unity_SHBr;
			float4 unity_SHBg;
			float4 unity_SHBb;
			float4 unity_SHC;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[6];
			static float4 vertex_uniform_buffer_1[46];
			static float4 vertex_uniform_buffer_2[10];
			static float4 vertex_uniform_buffer_3[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float4 vertex_output_2;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_output_6;
			static float3 vertex_output_7;
			static float4 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_output_5 : COLOR; // COLOR
				float4 vertex_output_6 : TEXCOORD4; // TEXCOORD_4
				float3 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float4 vertex_output_8 : TEXCOORD8; // TEXCOORD_8
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_64 = vertex_input_0.y * vertex_uniform_buffer_2[1u].x;
				precise float vertex_unnamed_65 = vertex_input_0.y * vertex_uniform_buffer_2[1u].y;
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_2[1u].z;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_2[1u].w;
				float vertex_unnamed_90 = mad(vertex_uniform_buffer_2[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].x, vertex_input_0.x, vertex_unnamed_64));
				float vertex_unnamed_91 = mad(vertex_uniform_buffer_2[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].y, vertex_input_0.x, vertex_unnamed_65));
				float vertex_unnamed_92 = mad(vertex_uniform_buffer_2[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].z, vertex_input_0.x, vertex_unnamed_66));
				precise float vertex_unnamed_101 = vertex_unnamed_90 + vertex_uniform_buffer_2[3u].x;
				precise float vertex_unnamed_102 = vertex_unnamed_91 + vertex_uniform_buffer_2[3u].y;
				precise float vertex_unnamed_103 = vertex_unnamed_92 + vertex_uniform_buffer_2[3u].z;
				precise float vertex_unnamed_104 = mad(vertex_uniform_buffer_2[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].w, vertex_input_0.x, vertex_unnamed_67)) + vertex_uniform_buffer_2[3u].w;
				float vertex_unnamed_112 = mad(vertex_uniform_buffer_2[3u].x, vertex_input_0.w, vertex_unnamed_90);
				float vertex_unnamed_113 = mad(vertex_uniform_buffer_2[3u].y, vertex_input_0.w, vertex_unnamed_91);
				float vertex_unnamed_114 = mad(vertex_uniform_buffer_2[3u].z, vertex_input_0.w, vertex_unnamed_92);
				precise float vertex_unnamed_122 = vertex_unnamed_102 * vertex_uniform_buffer_3[18u].x;
				precise float vertex_unnamed_123 = vertex_unnamed_102 * vertex_uniform_buffer_3[18u].y;
				precise float vertex_unnamed_124 = vertex_unnamed_102 * vertex_uniform_buffer_3[18u].z;
				precise float vertex_unnamed_125 = vertex_unnamed_102 * vertex_uniform_buffer_3[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_3[20u].x, vertex_unnamed_104, mad(vertex_uniform_buffer_3[19u].x, vertex_unnamed_103, mad(vertex_uniform_buffer_3[17u].x, vertex_unnamed_101, vertex_unnamed_122)));
				gl_Position.y = mad(vertex_uniform_buffer_3[20u].y, vertex_unnamed_104, mad(vertex_uniform_buffer_3[19u].y, vertex_unnamed_103, mad(vertex_uniform_buffer_3[17u].y, vertex_unnamed_101, vertex_unnamed_123)));
				gl_Position.z = mad(vertex_uniform_buffer_3[20u].z, vertex_unnamed_104, mad(vertex_uniform_buffer_3[19u].z, vertex_unnamed_103, mad(vertex_uniform_buffer_3[17u].z, vertex_unnamed_101, vertex_unnamed_124)));
				gl_Position.w = mad(vertex_uniform_buffer_3[20u].w, vertex_unnamed_104, mad(vertex_uniform_buffer_3[19u].w, vertex_unnamed_103, mad(vertex_uniform_buffer_3[17u].w, vertex_unnamed_101, vertex_unnamed_125)));
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[5u].x, vertex_uniform_buffer_0[5u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[5u].y, vertex_uniform_buffer_0[5u].w);
				precise float vertex_unnamed_188 = vertex_input_1.y * vertex_uniform_buffer_2[1u].y;
				precise float vertex_unnamed_189 = vertex_input_1.y * vertex_uniform_buffer_2[1u].z;
				precise float vertex_unnamed_190 = vertex_input_1.y * vertex_uniform_buffer_2[1u].x;
				float vertex_unnamed_208 = mad(vertex_uniform_buffer_2[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].y, vertex_input_1.x, vertex_unnamed_188));
				float vertex_unnamed_209 = mad(vertex_uniform_buffer_2[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].z, vertex_input_1.x, vertex_unnamed_189));
				float vertex_unnamed_210 = mad(vertex_uniform_buffer_2[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].x, vertex_input_1.x, vertex_unnamed_190));
				float vertex_unnamed_214 = rsqrt(dot(float3(vertex_unnamed_208, vertex_unnamed_209, vertex_unnamed_210), float3(vertex_unnamed_208, vertex_unnamed_209, vertex_unnamed_210)));
				precise float vertex_unnamed_215 = vertex_unnamed_214 * vertex_unnamed_208;
				precise float vertex_unnamed_216 = vertex_unnamed_214 * vertex_unnamed_209;
				precise float vertex_unnamed_217 = vertex_unnamed_214 * vertex_unnamed_210;
				vertex_output_2.x = vertex_unnamed_217;
				precise float vertex_unnamed_225 = vertex_input_1.w * vertex_uniform_buffer_2[9u].w;
				float vertex_unnamed_238 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[4u].xyz));
				float vertex_unnamed_252 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[5u].xyz));
				float vertex_unnamed_266 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[6u].xyz));
				float vertex_unnamed_272 = rsqrt(dot(float3(vertex_unnamed_238, vertex_unnamed_252, vertex_unnamed_266), float3(vertex_unnamed_238, vertex_unnamed_252, vertex_unnamed_266)));
				precise float vertex_unnamed_273 = vertex_unnamed_272 * vertex_unnamed_238;
				precise float vertex_unnamed_274 = vertex_unnamed_272 * vertex_unnamed_252;
				precise float vertex_unnamed_275 = vertex_unnamed_272 * vertex_unnamed_266;
				precise float vertex_unnamed_276 = vertex_unnamed_272 * vertex_unnamed_266;
				precise float vertex_unnamed_277 = vertex_unnamed_215 * vertex_unnamed_276;
				precise float vertex_unnamed_278 = vertex_unnamed_216 * vertex_unnamed_273;
				precise float vertex_unnamed_279 = vertex_unnamed_217 * vertex_unnamed_274;
				precise float vertex_unnamed_280 = (-0.0f) - vertex_unnamed_277;
				precise float vertex_unnamed_282 = (-0.0f) - vertex_unnamed_278;
				precise float vertex_unnamed_283 = (-0.0f) - vertex_unnamed_279;
				precise float vertex_unnamed_287 = vertex_unnamed_225 * mad(vertex_unnamed_274, vertex_unnamed_216, vertex_unnamed_280);
				precise float vertex_unnamed_288 = vertex_unnamed_225 * mad(vertex_unnamed_276, vertex_unnamed_217, vertex_unnamed_282);
				precise float vertex_unnamed_289 = vertex_unnamed_225 * mad(vertex_unnamed_273, vertex_unnamed_215, vertex_unnamed_283);
				vertex_output_2.y = vertex_unnamed_287;
				vertex_output_2.w = vertex_unnamed_112;
				vertex_output_2.z = vertex_unnamed_273;
				vertex_output_3.x = vertex_unnamed_215;
				vertex_output_4.x = vertex_unnamed_216;
				vertex_output_3.y = vertex_unnamed_288;
				vertex_output_4.y = vertex_unnamed_289;
				vertex_output_3.w = vertex_unnamed_113;
				vertex_output_3.z = vertex_unnamed_274;
				vertex_output_4.w = vertex_unnamed_114;
				vertex_output_4.z = vertex_unnamed_276;
				vertex_output_5.x = vertex_input_7.x;
				vertex_output_5.y = vertex_input_7.y;
				vertex_output_5.z = vertex_input_7.z;
				vertex_output_5.w = vertex_input_7.w;
				vertex_output_6.x = 0.0f;
				vertex_output_6.y = 0.0f;
				vertex_output_6.z = 0.0f;
				vertex_output_6.w = 0.0f;
				precise float vertex_unnamed_318 = (-0.0f) - vertex_unnamed_112;
				precise float vertex_unnamed_325 = vertex_unnamed_318 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_326 = vertex_unnamed_318 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_327 = vertex_unnamed_318 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_328 = vertex_unnamed_318 + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_329 = (-0.0f) - vertex_unnamed_113;
				precise float vertex_unnamed_336 = vertex_unnamed_329 + vertex_uniform_buffer_1[4u].x;
				precise float vertex_unnamed_337 = vertex_unnamed_329 + vertex_uniform_buffer_1[4u].y;
				precise float vertex_unnamed_338 = vertex_unnamed_329 + vertex_uniform_buffer_1[4u].z;
				precise float vertex_unnamed_339 = vertex_unnamed_329 + vertex_uniform_buffer_1[4u].w;
				precise float vertex_unnamed_340 = (-0.0f) - vertex_unnamed_114;
				precise float vertex_unnamed_347 = vertex_unnamed_340 + vertex_uniform_buffer_1[5u].x;
				precise float vertex_unnamed_348 = vertex_unnamed_340 + vertex_uniform_buffer_1[5u].y;
				precise float vertex_unnamed_349 = vertex_unnamed_340 + vertex_uniform_buffer_1[5u].z;
				precise float vertex_unnamed_350 = vertex_unnamed_340 + vertex_uniform_buffer_1[5u].w;
				precise float vertex_unnamed_351 = vertex_unnamed_274 * vertex_unnamed_336;
				precise float vertex_unnamed_352 = vertex_unnamed_274 * vertex_unnamed_337;
				precise float vertex_unnamed_353 = vertex_unnamed_274 * vertex_unnamed_338;
				precise float vertex_unnamed_354 = vertex_unnamed_274 * vertex_unnamed_339;
				precise float vertex_unnamed_355 = vertex_unnamed_336 * vertex_unnamed_336;
				precise float vertex_unnamed_356 = vertex_unnamed_337 * vertex_unnamed_337;
				precise float vertex_unnamed_357 = vertex_unnamed_338 * vertex_unnamed_338;
				precise float vertex_unnamed_358 = vertex_unnamed_339 * vertex_unnamed_339;
				float vertex_unnamed_375 = max(mad(vertex_unnamed_347, vertex_unnamed_347, mad(vertex_unnamed_325, vertex_unnamed_325, vertex_unnamed_355)), 9.9999999747524270787835121154785e-07f);
				float vertex_unnamed_377 = max(mad(vertex_unnamed_348, vertex_unnamed_348, mad(vertex_unnamed_326, vertex_unnamed_326, vertex_unnamed_356)), 9.9999999747524270787835121154785e-07f);
				float vertex_unnamed_378 = max(mad(vertex_unnamed_349, vertex_unnamed_349, mad(vertex_unnamed_327, vertex_unnamed_327, vertex_unnamed_357)), 9.9999999747524270787835121154785e-07f);
				float vertex_unnamed_379 = max(mad(vertex_unnamed_350, vertex_unnamed_350, mad(vertex_unnamed_328, vertex_unnamed_328, vertex_unnamed_358)), 9.9999999747524270787835121154785e-07f);
				precise float vertex_unnamed_395 = 1.0f / mad(vertex_unnamed_375, vertex_uniform_buffer_1[6u].x, 1.0f);
				precise float vertex_unnamed_396 = 1.0f / mad(vertex_unnamed_377, vertex_uniform_buffer_1[6u].y, 1.0f);
				precise float vertex_unnamed_397 = 1.0f / mad(vertex_unnamed_378, vertex_uniform_buffer_1[6u].z, 1.0f);
				precise float vertex_unnamed_398 = 1.0f / mad(vertex_unnamed_379, vertex_uniform_buffer_1[6u].w, 1.0f);
				precise float vertex_unnamed_399 = mad(vertex_unnamed_347, vertex_unnamed_276, mad(vertex_unnamed_325, vertex_unnamed_273, vertex_unnamed_351)) * rsqrt(vertex_unnamed_375);
				precise float vertex_unnamed_400 = mad(vertex_unnamed_348, vertex_unnamed_276, mad(vertex_unnamed_326, vertex_unnamed_273, vertex_unnamed_352)) * rsqrt(vertex_unnamed_377);
				precise float vertex_unnamed_401 = mad(vertex_unnamed_349, vertex_unnamed_275, mad(vertex_unnamed_327, vertex_unnamed_273, vertex_unnamed_353)) * rsqrt(vertex_unnamed_378);
				precise float vertex_unnamed_402 = mad(vertex_unnamed_350, vertex_unnamed_276, mad(vertex_unnamed_328, vertex_unnamed_273, vertex_unnamed_354)) * rsqrt(vertex_unnamed_379);
				precise float vertex_unnamed_407 = vertex_unnamed_395 * max(vertex_unnamed_399, 0.0f);
				precise float vertex_unnamed_408 = vertex_unnamed_396 * max(vertex_unnamed_400, 0.0f);
				precise float vertex_unnamed_409 = vertex_unnamed_397 * max(vertex_unnamed_401, 0.0f);
				precise float vertex_unnamed_410 = vertex_unnamed_398 * max(vertex_unnamed_402, 0.0f);
				precise float vertex_unnamed_417 = vertex_unnamed_408 * vertex_uniform_buffer_1[8u].x;
				precise float vertex_unnamed_418 = vertex_unnamed_408 * vertex_uniform_buffer_1[8u].y;
				precise float vertex_unnamed_419 = vertex_unnamed_408 * vertex_uniform_buffer_1[8u].z;
				precise float vertex_unnamed_445 = vertex_unnamed_274 * vertex_unnamed_274;
				precise float vertex_unnamed_446 = (-0.0f) - vertex_unnamed_445;
				float vertex_unnamed_447 = mad(vertex_unnamed_273, vertex_unnamed_273, vertex_unnamed_446);
				precise float vertex_unnamed_448 = vertex_unnamed_274 * vertex_unnamed_273;
				precise float vertex_unnamed_449 = vertex_unnamed_276 * vertex_unnamed_274;
				precise float vertex_unnamed_450 = vertex_unnamed_275 * vertex_unnamed_275;
				precise float vertex_unnamed_451 = vertex_unnamed_273 * vertex_unnamed_276;
				precise float vertex_unnamed_491 = mad(vertex_uniform_buffer_1[10u].x, vertex_unnamed_410, mad(vertex_uniform_buffer_1[9u].x, vertex_unnamed_409, mad(vertex_uniform_buffer_1[7u].x, vertex_unnamed_407, vertex_unnamed_417))) + mad(vertex_uniform_buffer_1[45u].x, vertex_unnamed_447, dot(float4(vertex_uniform_buffer_1[42u]), float4(vertex_unnamed_448, vertex_unnamed_449, vertex_unnamed_450, vertex_unnamed_451)));
				precise float vertex_unnamed_492 = mad(vertex_uniform_buffer_1[10u].y, vertex_unnamed_410, mad(vertex_uniform_buffer_1[9u].y, vertex_unnamed_409, mad(vertex_uniform_buffer_1[7u].y, vertex_unnamed_407, vertex_unnamed_418))) + mad(vertex_uniform_buffer_1[45u].y, vertex_unnamed_447, dot(float4(vertex_uniform_buffer_1[43u]), float4(vertex_unnamed_448, vertex_unnamed_449, vertex_unnamed_450, vertex_unnamed_451)));
				precise float vertex_unnamed_493 = mad(vertex_uniform_buffer_1[10u].z, vertex_unnamed_410, mad(vertex_uniform_buffer_1[9u].z, vertex_unnamed_409, mad(vertex_uniform_buffer_1[7u].z, vertex_unnamed_407, vertex_unnamed_419))) + mad(vertex_uniform_buffer_1[45u].z, vertex_unnamed_447, dot(float4(vertex_uniform_buffer_1[44u]), float4(vertex_unnamed_448, vertex_unnamed_449, vertex_unnamed_450, vertex_unnamed_451)));
				vertex_output_7.x = vertex_unnamed_491;
				vertex_output_7.y = vertex_unnamed_492;
				vertex_output_7.z = vertex_unnamed_493;
				vertex_output_8.x = 0.0f;
				vertex_output_8.y = 0.0f;
				vertex_output_8.z = 0.0f;
				vertex_output_8.w = 0.0f;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[5] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[3] = float4(unity_4LightPosX0[0], unity_4LightPosX0[1], unity_4LightPosX0[2], unity_4LightPosX0[3]);

				vertex_uniform_buffer_1[4] = float4(unity_4LightPosY0[0], unity_4LightPosY0[1], unity_4LightPosY0[2], unity_4LightPosY0[3]);

				vertex_uniform_buffer_1[5] = float4(unity_4LightPosZ0[0], unity_4LightPosZ0[1], unity_4LightPosZ0[2], unity_4LightPosZ0[3]);

				vertex_uniform_buffer_1[6] = float4(unity_4LightAtten0[0], unity_4LightAtten0[1], unity_4LightAtten0[2], unity_4LightAtten0[3]);

				vertex_uniform_buffer_1[7] = float4(unity_LightColor[0][0], unity_LightColor[0][1], unity_LightColor[0][2], unity_LightColor[0][3]);
				vertex_uniform_buffer_1[8] = float4(unity_LightColor[1][0], unity_LightColor[1][1], unity_LightColor[1][2], unity_LightColor[1][3]);
				vertex_uniform_buffer_1[9] = float4(unity_LightColor[2][0], unity_LightColor[2][1], unity_LightColor[2][2], unity_LightColor[2][3]);
				vertex_uniform_buffer_1[10] = float4(unity_LightColor[3][0], unity_LightColor[3][1], unity_LightColor[3][2], unity_LightColor[3][3]);
				vertex_uniform_buffer_1[11] = float4(unity_LightColor[4][0], unity_LightColor[4][1], unity_LightColor[4][2], unity_LightColor[4][3]);
				vertex_uniform_buffer_1[12] = float4(unity_LightColor[5][0], unity_LightColor[5][1], unity_LightColor[5][2], unity_LightColor[5][3]);
				vertex_uniform_buffer_1[13] = float4(unity_LightColor[6][0], unity_LightColor[6][1], unity_LightColor[6][2], unity_LightColor[6][3]);
				vertex_uniform_buffer_1[14] = float4(unity_LightColor[7][0], unity_LightColor[7][1], unity_LightColor[7][2], unity_LightColor[7][3]);

				vertex_uniform_buffer_1[42] = float4(unity_SHBr[0], unity_SHBr[1], unity_SHBr[2], unity_SHBr[3]);

				vertex_uniform_buffer_1[43] = float4(unity_SHBg[0], unity_SHBg[1], unity_SHBg[2], unity_SHBg[3]);

				vertex_uniform_buffer_1[44] = float4(unity_SHBb[0], unity_SHBb[1], unity_SHBb[2], unity_SHBb[3]);

				vertex_uniform_buffer_1[45] = float4(unity_SHC[0], unity_SHC[1], unity_SHC[2], unity_SHC[3]);

				vertex_uniform_buffer_2[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_2[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_2[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_2[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_2[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_2[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_2[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_2[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_3[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_3[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_3[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_3[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // LIGHTPROBE_SH
			#endif // VERTEXLIGHT_ON
			#endif // !FOG_LINEAR


			#ifdef DIRECTIONAL
			#ifdef LIGHTPROBE_SH
			#ifdef VERTEXLIGHT_ON
			#ifndef FOG_LINEAR
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 unity_4LightPosX0;
			float4 unity_4LightPosY0;
			float4 unity_4LightPosZ0;
			float4 unity_4LightAtten0;
			float4 unity_LightColor[4];
			float4 unity_SHBr;
			float4 unity_SHBg;
			float4 unity_SHBb;
			float4 unity_SHC;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			cbuffer t3edb6ca0a16c42489407ea45cc3bdd8e
			{
				float4 unity_LightColor[8];
			};

			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float4 vertex_input_1;
			static float4 vertex_output_1;
			static float3 vertex_input_2;
			static float4 vertex_output_2;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_input_4;
			static float4 vertex_output_6;
			static float3 vertex_output_5;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float4 vertex_output_1 : TEXCOORD1; // vs_TEXCOORD1
				float4 vertex_output_2 : TEXCOORD2; // vs_TEXCOORD2
				float4 vertex_output_3 : TEXCOORD3; // vs_TEXCOORD3
				float4 vertex_output_4 : UNKNOWN4;
				float3 vertex_output_5 : TEXCOORD5; // vs_TEXCOORD5
				float4 vertex_output_6 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_7 : TEXCOORD8; // vs_TEXCOORD8
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_47;
			static float4 vertex_unnamed_65;
			static float vertex_unnamed_147;
			static float vertex_unnamed_199;
			static float4 vertex_unnamed_212;
			static float4 vertex_unnamed_297;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_47 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				float3 vertex_unnamed_62 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_62.x, vertex_unnamed_62.y, vertex_unnamed_62.z, vertex_unnamed_9.w);
				vertex_unnamed_65 = vertex_unnamed_47.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_65 = (unity_MatrixVP__array[0] * vertex_unnamed_47.xxxx) + vertex_unnamed_65;
				vertex_unnamed_65 = (unity_MatrixVP__array[2] * vertex_unnamed_47.zzzz) + vertex_unnamed_65;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_47.wwww) + vertex_unnamed_65;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				float3 vertex_unnamed_121 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_47 = float4(vertex_unnamed_121.x, vertex_unnamed_121.y, vertex_unnamed_121.z, vertex_unnamed_47.w);
				float3 vertex_unnamed_132 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_47.xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_132.x, vertex_unnamed_132.y, vertex_unnamed_132.z, vertex_unnamed_47.w);
				float3 vertex_unnamed_143 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_47.xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_143.x, vertex_unnamed_143.y, vertex_unnamed_143.z, vertex_unnamed_47.w);
				vertex_unnamed_147 = dot(vertex_unnamed_47.xyz, vertex_unnamed_47.xyz);
				vertex_unnamed_147 = rsqrt(vertex_unnamed_147);
				float3 vertex_unnamed_159 = vertex_unnamed_147.xxx * vertex_unnamed_47.xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_159.x, vertex_unnamed_159.y, vertex_unnamed_159.z, vertex_unnamed_47.w);
				vertex_output_1.x = vertex_unnamed_47.z;
				vertex_unnamed_147 = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_65.x = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_65.y = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_65.z = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_199 = dot(vertex_unnamed_65.xyz, vertex_unnamed_65.xyz);
				vertex_unnamed_199 = rsqrt(vertex_unnamed_199);
				vertex_unnamed_65 = vertex_unnamed_199.xxxx * vertex_unnamed_65.xyzz;
				float3 vertex_unnamed_217 = vertex_unnamed_47.xyz * vertex_unnamed_65.wxy;
				vertex_unnamed_212 = float4(vertex_unnamed_217.x, vertex_unnamed_217.y, vertex_unnamed_217.z, vertex_unnamed_212.w);
				float3 vertex_unnamed_228 = (vertex_unnamed_65.ywx * vertex_unnamed_47.yzx) + (-vertex_unnamed_212.xyz);
				vertex_unnamed_212 = float4(vertex_unnamed_228.x, vertex_unnamed_228.y, vertex_unnamed_228.z, vertex_unnamed_212.w);
				float3 vertex_unnamed_235 = vertex_unnamed_147.xxx * vertex_unnamed_212.xyz;
				vertex_unnamed_212 = float4(vertex_unnamed_235.x, vertex_unnamed_235.y, vertex_unnamed_235.z, vertex_unnamed_212.w);
				vertex_output_1.y = vertex_unnamed_212.x;
				vertex_output_1.w = vertex_unnamed_9.x;
				vertex_output_1.z = vertex_unnamed_65.x;
				vertex_output_2.x = vertex_unnamed_47.x;
				vertex_output_3.x = vertex_unnamed_47.y;
				vertex_output_2.y = vertex_unnamed_212.y;
				vertex_output_3.y = vertex_unnamed_212.z;
				vertex_output_2.w = vertex_unnamed_9.y;
				vertex_output_2.z = vertex_unnamed_65.y;
				vertex_output_3.w = vertex_unnamed_9.z;
				vertex_output_3.z = vertex_unnamed_65.w;
				vertex_output_4 = vertex_input_4;
				vertex_output_6 = 0.0f.xxxx;
				vertex_unnamed_47 = (-vertex_unnamed_9.xxxx) + unity_4LightPosX0;
				vertex_unnamed_212 = (-vertex_unnamed_9.yyyy) + unity_4LightPosY0;
				vertex_unnamed_9 = (-vertex_unnamed_9.zzzz) + unity_4LightPosZ0;
				vertex_unnamed_297 = vertex_unnamed_65.yyyy * vertex_unnamed_212;
				vertex_unnamed_212 *= vertex_unnamed_212;
				vertex_unnamed_212 = (vertex_unnamed_47 * vertex_unnamed_47) + vertex_unnamed_212;
				vertex_unnamed_47 = (vertex_unnamed_47 * vertex_unnamed_65.xxxx) + vertex_unnamed_297;
				vertex_unnamed_47 = (vertex_unnamed_9 * vertex_unnamed_65.wwzw) + vertex_unnamed_47;
				vertex_unnamed_9 = (vertex_unnamed_9 * vertex_unnamed_9) + vertex_unnamed_212;
				vertex_unnamed_9 = max(vertex_unnamed_9, 9.9999999747524270787835121154785e-07f.xxxx);
				vertex_unnamed_212 = rsqrt(vertex_unnamed_9);
				vertex_unnamed_9 = (vertex_unnamed_9 * unity_4LightAtten0) + 1.0f.xxxx;
				vertex_unnamed_9 = 1.0f.xxxx / vertex_unnamed_9;
				vertex_unnamed_47 *= vertex_unnamed_212;
				vertex_unnamed_47 = max(vertex_unnamed_47, 0.0f.xxxx);
				vertex_unnamed_9 *= vertex_unnamed_47;
				float3 vertex_unnamed_356 = vertex_unnamed_9.yyy * unity_LightColor[1].xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_356.x, vertex_unnamed_356.y, vertex_unnamed_356.z, vertex_unnamed_47.w);
				float3 vertex_unnamed_367 = (unity_LightColor[0].xyz * vertex_unnamed_9.xxx) + vertex_unnamed_47.xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_367.x, vertex_unnamed_367.y, vertex_unnamed_367.z, vertex_unnamed_47.w);
				float3 vertex_unnamed_378 = (unity_LightColor[2].xyz * vertex_unnamed_9.zzz) + vertex_unnamed_47.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_378.x, vertex_unnamed_378.y, vertex_unnamed_378.z, vertex_unnamed_9.w);
				float3 vertex_unnamed_389 = (unity_LightColor[3].xyz * vertex_unnamed_9.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_389.x, vertex_unnamed_389.y, vertex_unnamed_389.z, vertex_unnamed_9.w);
				vertex_unnamed_147 = vertex_unnamed_65.y * vertex_unnamed_65.y;
				vertex_unnamed_147 = (vertex_unnamed_65.x * vertex_unnamed_65.x) + (-vertex_unnamed_147);
				vertex_unnamed_47 = vertex_unnamed_65.ywzx * vertex_unnamed_65;
				vertex_unnamed_65.x = dot(unity_SHBr, vertex_unnamed_47);
				vertex_unnamed_65.y = dot(unity_SHBg, vertex_unnamed_47);
				vertex_unnamed_65.z = dot(unity_SHBb, vertex_unnamed_47);
				float3 vertex_unnamed_436 = (unity_SHC.xyz * vertex_unnamed_147.xxx) + vertex_unnamed_65.xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_436.x, vertex_unnamed_436.y, vertex_unnamed_436.z, vertex_unnamed_47.w);
				vertex_output_5 = vertex_unnamed_9.xyz + vertex_unnamed_47.xyz;
				vertex_output_7 = 0.0f.xxxx;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;
			float4 unity_SHAr;
			float4 unity_SHAg;
			float4 unity_SHAb;
			float4 unity_SpecCube0_BoxMax;
			float4 unity_SpecCube0_BoxMin;
			float4 unity_SpecCube0_ProbePosition;
			float4 unity_SpecCube0_HDR;
			float4 unity_SpecCube1_BoxMax;
			float4 unity_SpecCube1_BoxMin;
			float4 unity_SpecCube1_ProbePosition;
			float4 unity_SpecCube1_HDR;
			float4 _LightColor0;
			float4 _Color;

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			TextureCube<float4> unity_SpecCube0;
			SamplerState samplerunity_SpecCube0;
			TextureCube<float4> unity_SpecCube1;

			static float4 fragment_input_1;
			static float4 fragment_input_2;
			static float4 fragment_input_3;
			static float2 fragment_input_0;
			static float4 fragment_input_4;
			static float4 fragment_output_0;
			static float3 fragment_input_5;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float4 fragment_input_1 : TEXCOORD1; // vs_TEXCOORD1
				float4 fragment_input_2 : TEXCOORD2; // vs_TEXCOORD2
				float4 fragment_input_3 : TEXCOORD3; // vs_TEXCOORD3
				float4 fragment_input_4 : UNKNOWN4;
				float3 fragment_input_5 : TEXCOORD5; // vs_TEXCOORD5
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float3 fragment_unnamed_31;
			static float fragment_unnamed_43;
			static float3 fragment_unnamed_49;
			static float4 fragment_unnamed_55;
			static float4 fragment_unnamed_85;
			static float fragment_unnamed_112;
			static float4 fragment_unnamed_136;
			static float3 fragment_unnamed_189;
			static bool fragment_unnamed_219;
			static float4 fragment_unnamed_235;
			static float3 fragment_unnamed_243;
			static float3 fragment_unnamed_255;
			static bool3 fragment_unnamed_269;
			static bool fragment_unnamed_384;
			static float fragment_unnamed_399;
			static float3 fragment_unnamed_412;
			static float3 fragment_unnamed_423;
			static bool3 fragment_unnamed_434;
			static float3 fragment_unnamed_611;
			static float fragment_unnamed_623;
			static float3 fragment_unnamed_651;

			void frag_main()
			{
				fragment_unnamed_9.x = fragment_input_1.w;
				fragment_unnamed_9.y = fragment_input_2.w;
				fragment_unnamed_9.z = fragment_input_3.w;
				fragment_unnamed_31 = (-fragment_unnamed_9) + _WorldSpaceCameraPos;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_49 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_55 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_55 *= _Color;
				float3 fragment_unnamed_82 = fragment_unnamed_55.xyz * fragment_input_4.xyz;
				fragment_unnamed_55 = float4(fragment_unnamed_82.x, fragment_unnamed_82.y, fragment_unnamed_82.z, fragment_unnamed_55.w);
				float3 fragment_unnamed_93 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_85 = float4(fragment_unnamed_93.x, fragment_unnamed_93.y, fragment_unnamed_93.z, fragment_unnamed_85.w);
				fragment_unnamed_85.x = fragment_unnamed_85.z * fragment_unnamed_85.x;
				float2 fragment_unnamed_109 = (fragment_unnamed_85.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_85 = float4(fragment_unnamed_109.x, fragment_unnamed_109.y, fragment_unnamed_85.z, fragment_unnamed_85.w);
				fragment_unnamed_112 = dot(fragment_unnamed_85.xy, fragment_unnamed_85.xy);
				fragment_unnamed_112 = min(fragment_unnamed_112, 1.0f);
				fragment_unnamed_112 = (-fragment_unnamed_112) + 1.0f;
				fragment_unnamed_85.z = sqrt(fragment_unnamed_112);
				fragment_output_0.w = fragment_unnamed_55.w * fragment_input_4.w;
				fragment_unnamed_136.x = dot(fragment_input_1.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_136.y = dot(fragment_input_2.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_136.z = dot(fragment_input_3.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_112 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
				fragment_unnamed_112 = rsqrt(fragment_unnamed_112);
				float3 fragment_unnamed_166 = fragment_unnamed_112.xxx * fragment_unnamed_136.xyz;
				fragment_unnamed_85 = float4(fragment_unnamed_166.x, fragment_unnamed_166.y, fragment_unnamed_166.z, fragment_unnamed_85.w);
				fragment_unnamed_112 = dot(-fragment_unnamed_49, fragment_unnamed_85.xyz);
				fragment_unnamed_112 += fragment_unnamed_112;
				float3 fragment_unnamed_185 = (fragment_unnamed_85.xyz * (-fragment_unnamed_112.xxx)) + (-fragment_unnamed_49);
				fragment_unnamed_136 = float4(fragment_unnamed_185.x, fragment_unnamed_185.y, fragment_unnamed_185.z, fragment_unnamed_136.w);
				fragment_unnamed_85.w = 1.0f;
				fragment_unnamed_189.x = dot(unity_SHAr, fragment_unnamed_85);
				fragment_unnamed_189.y = dot(unity_SHAg, fragment_unnamed_85);
				fragment_unnamed_189.z = dot(unity_SHAb, fragment_unnamed_85);
				fragment_unnamed_189 += fragment_input_5;
				fragment_unnamed_189 = max(fragment_unnamed_189, 0.0f.xxx);
				fragment_unnamed_219 = 0.0f < unity_SpecCube0_ProbePosition.w;
				if (fragment_unnamed_219)
				{
					fragment_unnamed_112 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
					fragment_unnamed_112 = rsqrt(fragment_unnamed_112);
					float3 fragment_unnamed_240 = fragment_unnamed_112.xxx * fragment_unnamed_136.xyz;
					fragment_unnamed_235 = float4(fragment_unnamed_240.x, fragment_unnamed_240.y, fragment_unnamed_240.z, fragment_unnamed_235.w);
					fragment_unnamed_243 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMax.xyz;
					fragment_unnamed_243 /= fragment_unnamed_235.xyz;
					fragment_unnamed_255 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMin.xyz;
					fragment_unnamed_255 /= fragment_unnamed_235.xyz;
					fragment_unnamed_269 = bool4(0.0f.xxxx.x < fragment_unnamed_235.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_235.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_235.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_235.xyzx.w).xyz;
					float3 fragment_unnamed_277 = fragment_unnamed_243;
					float fragment_unnamed_282;
					if (fragment_unnamed_269.x)
					{
						fragment_unnamed_282 = fragment_unnamed_243.x;
					}
					else
					{
						fragment_unnamed_282 = fragment_unnamed_255.x;
					}
					fragment_unnamed_277.x = fragment_unnamed_282;
					float fragment_unnamed_294;
					if (fragment_unnamed_269.y)
					{
						fragment_unnamed_294 = fragment_unnamed_243.y;
					}
					else
					{
						fragment_unnamed_294 = fragment_unnamed_255.y;
					}
					fragment_unnamed_277.y = fragment_unnamed_294;
					float fragment_unnamed_306;
					if (fragment_unnamed_269.z)
					{
						fragment_unnamed_306 = fragment_unnamed_243.z;
					}
					else
					{
						fragment_unnamed_306 = fragment_unnamed_255.z;
					}
					fragment_unnamed_277.z = fragment_unnamed_306;
					fragment_unnamed_243 = fragment_unnamed_277;
					fragment_unnamed_112 = min(fragment_unnamed_243.y, fragment_unnamed_243.x);
					fragment_unnamed_112 = min(fragment_unnamed_243.z, fragment_unnamed_112);
					fragment_unnamed_243 = fragment_unnamed_9 + (-unity_SpecCube0_ProbePosition.xyz);
					float3 fragment_unnamed_338 = (fragment_unnamed_235.xyz * fragment_unnamed_112.xxx) + fragment_unnamed_243;
					fragment_unnamed_235 = float4(fragment_unnamed_338.x, fragment_unnamed_338.y, fragment_unnamed_338.z, fragment_unnamed_235.w);
				}
				else
				{
					fragment_unnamed_235 = float4(fragment_unnamed_136.xyz.x, fragment_unnamed_136.xyz.y, fragment_unnamed_136.xyz.z, fragment_unnamed_235.w);
				}
				fragment_unnamed_235 = unity_SpecCube0.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_235.xyz, 6.0f);
				fragment_unnamed_112 = fragment_unnamed_235.w + (-1.0f);
				fragment_unnamed_112 = (unity_SpecCube0_HDR.w * fragment_unnamed_112) + 1.0f;
				fragment_unnamed_112 = log2(fragment_unnamed_112);
				fragment_unnamed_112 *= unity_SpecCube0_HDR.y;
				fragment_unnamed_112 = exp2(fragment_unnamed_112);
				fragment_unnamed_112 *= unity_SpecCube0_HDR.x;
				fragment_unnamed_243 = fragment_unnamed_235.xyz * fragment_unnamed_112.xxx;
				fragment_unnamed_384 = unity_SpecCube0_BoxMin.w < 0.999989986419677734375f;
				if (fragment_unnamed_384)
				{
					fragment_unnamed_384 = 0.0f < unity_SpecCube1_ProbePosition.w;
					if (fragment_unnamed_384)
					{
						fragment_unnamed_399 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
						fragment_unnamed_399 = rsqrt(fragment_unnamed_399);
						fragment_unnamed_255 = fragment_unnamed_399.xxx * fragment_unnamed_136.xyz;
						fragment_unnamed_412 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMax.xyz;
						fragment_unnamed_412 /= fragment_unnamed_255;
						fragment_unnamed_423 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMin.xyz;
						fragment_unnamed_423 /= fragment_unnamed_255;
						fragment_unnamed_434 = bool4(0.0f.xxxx.x < fragment_unnamed_255.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_255.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_255.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_255.xyzx.w).xyz;
						float3 fragment_unnamed_439 = fragment_unnamed_412;
						float fragment_unnamed_443;
						if (fragment_unnamed_434.x)
						{
							fragment_unnamed_443 = fragment_unnamed_412.x;
						}
						else
						{
							fragment_unnamed_443 = fragment_unnamed_423.x;
						}
						fragment_unnamed_439.x = fragment_unnamed_443;
						float fragment_unnamed_455;
						if (fragment_unnamed_434.y)
						{
							fragment_unnamed_455 = fragment_unnamed_412.y;
						}
						else
						{
							fragment_unnamed_455 = fragment_unnamed_423.y;
						}
						fragment_unnamed_439.y = fragment_unnamed_455;
						float fragment_unnamed_467;
						if (fragment_unnamed_434.z)
						{
							fragment_unnamed_467 = fragment_unnamed_412.z;
						}
						else
						{
							fragment_unnamed_467 = fragment_unnamed_423.z;
						}
						fragment_unnamed_439.z = fragment_unnamed_467;
						fragment_unnamed_412 = fragment_unnamed_439;
						fragment_unnamed_399 = min(fragment_unnamed_412.y, fragment_unnamed_412.x);
						fragment_unnamed_399 = min(fragment_unnamed_412.z, fragment_unnamed_399);
						fragment_unnamed_9 += (-unity_SpecCube1_ProbePosition.xyz);
						float3 fragment_unnamed_498 = (fragment_unnamed_255 * fragment_unnamed_399.xxx) + fragment_unnamed_9;
						fragment_unnamed_136 = float4(fragment_unnamed_498.x, fragment_unnamed_498.y, fragment_unnamed_498.z, fragment_unnamed_136.w);
					}
					fragment_unnamed_136 = unity_SpecCube1.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_136.xyz, 6.0f);
					fragment_unnamed_9.x = fragment_unnamed_136.w + (-1.0f);
					fragment_unnamed_9.x = (unity_SpecCube1_HDR.w * fragment_unnamed_9.x) + 1.0f;
					fragment_unnamed_9.x = log2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.y;
					fragment_unnamed_9.x = exp2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.x;
					fragment_unnamed_9 = fragment_unnamed_136.xyz * fragment_unnamed_9.xxx;
					float3 fragment_unnamed_552 = (fragment_unnamed_112.xxx * fragment_unnamed_235.xyz) + (-fragment_unnamed_9);
					fragment_unnamed_136 = float4(fragment_unnamed_552.x, fragment_unnamed_552.y, fragment_unnamed_552.z, fragment_unnamed_136.w);
					fragment_unnamed_243 = (unity_SpecCube0_BoxMin.www * fragment_unnamed_136.xyz) + fragment_unnamed_9;
				}
				fragment_unnamed_9 = fragment_unnamed_55.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = max(fragment_unnamed_43, 0.001000000047497451305389404296875f);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_31 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_43 = dot(fragment_unnamed_85.xyz, fragment_unnamed_49);
				fragment_unnamed_112 = dot(fragment_unnamed_85.xyz, _WorldSpaceLightPos0.xyz);
				fragment_unnamed_112 = clamp(fragment_unnamed_112, 0.0f, 1.0f);
				fragment_unnamed_31.x = dot(_WorldSpaceLightPos0.xyz, fragment_unnamed_31);
				fragment_unnamed_31.x = clamp(fragment_unnamed_31.x, 0.0f, 1.0f);
				fragment_unnamed_611.x = dot(fragment_unnamed_31.xx, fragment_unnamed_31.xx);
				fragment_unnamed_611.x += (-0.5f);
				fragment_unnamed_623 = (-fragment_unnamed_112) + 1.0f;
				fragment_unnamed_49.x = fragment_unnamed_623 * fragment_unnamed_623;
				fragment_unnamed_49.x *= fragment_unnamed_49.x;
				fragment_unnamed_623 *= fragment_unnamed_49.x;
				fragment_unnamed_623 = (fragment_unnamed_611.x * fragment_unnamed_623) + 1.0f;
				fragment_unnamed_49.x = (-abs(fragment_unnamed_43)) + 1.0f;
				fragment_unnamed_651.x = fragment_unnamed_49.x * fragment_unnamed_49.x;
				fragment_unnamed_651.x *= fragment_unnamed_651.x;
				fragment_unnamed_49.x *= fragment_unnamed_651.x;
				fragment_unnamed_611.x = (fragment_unnamed_611.x * fragment_unnamed_49.x) + 1.0f;
				fragment_unnamed_611.x *= fragment_unnamed_623;
				fragment_unnamed_611.x = fragment_unnamed_112 * fragment_unnamed_611.x;
				fragment_unnamed_43 = abs(fragment_unnamed_43) + fragment_unnamed_112;
				fragment_unnamed_43 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_43 = 0.5f / fragment_unnamed_43;
				fragment_unnamed_43 = fragment_unnamed_112 * fragment_unnamed_43;
				fragment_unnamed_43 *= 0.99999988079071044921875f;
				fragment_unnamed_611 = (_LightColor0.xyz * fragment_unnamed_611.xxx) + fragment_unnamed_189;
				fragment_unnamed_651 = fragment_unnamed_43.xxx * _LightColor0.xyz;
				fragment_unnamed_43 = (-fragment_unnamed_31.x) + 1.0f;
				fragment_unnamed_31.x = fragment_unnamed_43 * fragment_unnamed_43;
				fragment_unnamed_31.x *= fragment_unnamed_31.x;
				fragment_unnamed_43 *= fragment_unnamed_31.x;
				fragment_unnamed_43 = (fragment_unnamed_43 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				fragment_unnamed_651 = fragment_unnamed_43.xxx * fragment_unnamed_651;
				fragment_unnamed_9 = (fragment_unnamed_9 * fragment_unnamed_611) + fragment_unnamed_651;
				fragment_unnamed_31 = fragment_unnamed_243 * 0.5f.xxx;
				fragment_unnamed_43 = (fragment_unnamed_49.x * 2.2351741790771484375e-08f) + 0.039999999105930328369140625f;
				float3 fragment_unnamed_762 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + fragment_unnamed_9;
				fragment_output_0 = float4(fragment_unnamed_762.x, fragment_unnamed_762.y, fragment_unnamed_762.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // LIGHTPROBE_SH
			#endif // VERTEXLIGHT_ON
			#endif // !FOG_LINEAR


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[6];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float vertex_output_1;
			static float4 vertex_output_2;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float vertex_output_1 : TEXCOORD6; // TEXCOORD_6
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_output_5 : COLOR; // COLOR
				float4 vertex_output_6 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_7 : TEXCOORD8; // TEXCOORD_8
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_59 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_61 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_62 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_85 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_59));
				float vertex_unnamed_86 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_60));
				float vertex_unnamed_87 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_61));
				precise float vertex_unnamed_96 = vertex_unnamed_85 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_97 = vertex_unnamed_86 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_98 = vertex_unnamed_87 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_99 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_62)) + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_117 = vertex_unnamed_97 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_118 = vertex_unnamed_97 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_119 = vertex_unnamed_97 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_120 = vertex_unnamed_97 * vertex_uniform_buffer_2[18u].w;
				float vertex_unnamed_152 = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_99, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_98, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_96, vertex_unnamed_119)));
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_99, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_98, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_96, vertex_unnamed_117)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_99, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_98, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_96, vertex_unnamed_118)));
				gl_Position.z = vertex_unnamed_152;
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_99, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_98, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_96, vertex_unnamed_120)));
				vertex_output_1 = vertex_unnamed_152;
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[5u].x, vertex_uniform_buffer_0[5u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[5u].y, vertex_uniform_buffer_0[5u].w);
				vertex_output_2.w = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_85);
				float vertex_unnamed_188 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_202 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_216 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_222 = rsqrt(dot(float3(vertex_unnamed_216, vertex_unnamed_188, vertex_unnamed_202), float3(vertex_unnamed_216, vertex_unnamed_188, vertex_unnamed_202)));
				precise float vertex_unnamed_223 = vertex_unnamed_222 * vertex_unnamed_216;
				precise float vertex_unnamed_224 = vertex_unnamed_222 * vertex_unnamed_188;
				precise float vertex_unnamed_225 = vertex_unnamed_222 * vertex_unnamed_202;
				precise float vertex_unnamed_233 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_234 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_235 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_253 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_233));
				float vertex_unnamed_254 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_234));
				float vertex_unnamed_255 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_235));
				float vertex_unnamed_259 = rsqrt(dot(float3(vertex_unnamed_253, vertex_unnamed_254, vertex_unnamed_255), float3(vertex_unnamed_253, vertex_unnamed_254, vertex_unnamed_255)));
				precise float vertex_unnamed_260 = vertex_unnamed_259 * vertex_unnamed_253;
				precise float vertex_unnamed_261 = vertex_unnamed_259 * vertex_unnamed_254;
				precise float vertex_unnamed_262 = vertex_unnamed_259 * vertex_unnamed_255;
				precise float vertex_unnamed_263 = vertex_unnamed_223 * vertex_unnamed_260;
				precise float vertex_unnamed_264 = vertex_unnamed_224 * vertex_unnamed_261;
				precise float vertex_unnamed_265 = vertex_unnamed_225 * vertex_unnamed_262;
				precise float vertex_unnamed_266 = (-0.0f) - vertex_unnamed_263;
				precise float vertex_unnamed_268 = (-0.0f) - vertex_unnamed_264;
				precise float vertex_unnamed_269 = (-0.0f) - vertex_unnamed_265;
				precise float vertex_unnamed_279 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_280 = vertex_unnamed_279 * mad(vertex_unnamed_225, vertex_unnamed_261, vertex_unnamed_266);
				precise float vertex_unnamed_281 = vertex_unnamed_279 * mad(vertex_unnamed_223, vertex_unnamed_262, vertex_unnamed_268);
				precise float vertex_unnamed_282 = vertex_unnamed_279 * mad(vertex_unnamed_224, vertex_unnamed_260, vertex_unnamed_269);
				vertex_output_2.y = vertex_unnamed_280;
				vertex_output_2.x = vertex_unnamed_262;
				vertex_output_2.z = vertex_unnamed_224;
				vertex_output_3.x = vertex_unnamed_260;
				vertex_output_4.x = vertex_unnamed_261;
				vertex_output_3.z = vertex_unnamed_225;
				vertex_output_4.z = vertex_unnamed_223;
				vertex_output_3.w = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_86);
				vertex_output_4.w = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_87);
				vertex_output_3.y = vertex_unnamed_281;
				vertex_output_4.y = vertex_unnamed_282;
				vertex_output_5.x = vertex_input_7.x;
				vertex_output_5.y = vertex_input_7.y;
				vertex_output_5.z = vertex_input_7.z;
				vertex_output_5.w = vertex_input_7.w;
				vertex_output_6.x = 0.0f;
				vertex_output_6.y = 0.0f;
				vertex_output_6.z = 0.0f;
				vertex_output_6.w = 0.0f;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[5] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // !LIGHTPROBE_SH
			#endif // !VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float vertex_output_1;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float4 vertex_output_2;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_input_4;
			static float4 vertex_output_6;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float vertex_output_1 : TEXCOORD6; // vs_TEXCOORD6
				float4 vertex_output_2 : TEXCOORD1; // vs_TEXCOORD1
				float4 vertex_output_3 : TEXCOORD2; // vs_TEXCOORD2
				float4 vertex_output_4 : TEXCOORD3; // vs_TEXCOORD3
				float4 vertex_output_5 : UNKNOWN5;
				float4 vertex_output_6 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_7 : TEXCOORD8; // vs_TEXCOORD8
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_44;
			static float4 vertex_unnamed_62;
			static float3 vertex_unnamed_210;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_44 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				float3 vertex_unnamed_59 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_59.x, vertex_unnamed_59.y, vertex_unnamed_59.z, vertex_unnamed_9.w);
				vertex_unnamed_62 = vertex_unnamed_44.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_62 = (unity_MatrixVP__array[0] * vertex_unnamed_44.xxxx) + vertex_unnamed_62;
				vertex_unnamed_62 = (unity_MatrixVP__array[2] * vertex_unnamed_44.zzzz) + vertex_unnamed_62;
				vertex_unnamed_44 = (unity_MatrixVP__array[3] * vertex_unnamed_44.wwww) + vertex_unnamed_62;
				gl_Position = vertex_unnamed_44;
				vertex_output_1 = vertex_unnamed_44.z;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_output_2.w = vertex_unnamed_9.x;
				vertex_unnamed_44.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_44.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_44.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_9.x = dot(vertex_unnamed_44.xyz, vertex_unnamed_44.xyz);
				vertex_unnamed_9.x = rsqrt(vertex_unnamed_9.x);
				float3 vertex_unnamed_158 = vertex_unnamed_9.xxx * vertex_unnamed_44.xyz;
				vertex_unnamed_44 = float4(vertex_unnamed_158.x, vertex_unnamed_158.y, vertex_unnamed_158.z, vertex_unnamed_44.w);
				float3 vertex_unnamed_167 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_62 = float4(vertex_unnamed_167.x, vertex_unnamed_167.y, vertex_unnamed_167.z, vertex_unnamed_62.w);
				float3 vertex_unnamed_178 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_62.xyz;
				vertex_unnamed_62 = float4(vertex_unnamed_178.x, vertex_unnamed_178.y, vertex_unnamed_178.z, vertex_unnamed_62.w);
				float3 vertex_unnamed_189 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_62.xyz;
				vertex_unnamed_62 = float4(vertex_unnamed_189.x, vertex_unnamed_189.y, vertex_unnamed_189.z, vertex_unnamed_62.w);
				vertex_unnamed_9.x = dot(vertex_unnamed_62.xyz, vertex_unnamed_62.xyz);
				vertex_unnamed_9.x = rsqrt(vertex_unnamed_9.x);
				float3 vertex_unnamed_206 = vertex_unnamed_9.xxx * vertex_unnamed_62.xyz;
				vertex_unnamed_62 = float4(vertex_unnamed_206.x, vertex_unnamed_206.y, vertex_unnamed_206.z, vertex_unnamed_62.w);
				vertex_unnamed_210 = vertex_unnamed_44.xyz * vertex_unnamed_62.xyz;
				vertex_unnamed_210 = (vertex_unnamed_44.zxy * vertex_unnamed_62.yzx) + (-vertex_unnamed_210);
				vertex_unnamed_9.x = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_210 = vertex_unnamed_9.xxx * vertex_unnamed_210;
				vertex_output_2.y = vertex_unnamed_210.x;
				vertex_output_2.x = vertex_unnamed_62.z;
				vertex_output_2.z = vertex_unnamed_44.y;
				vertex_output_3.x = vertex_unnamed_62.x;
				vertex_output_4.x = vertex_unnamed_62.y;
				vertex_output_3.z = vertex_unnamed_44.z;
				vertex_output_4.z = vertex_unnamed_44.x;
				vertex_output_3.w = vertex_unnamed_9.y;
				vertex_output_4.w = vertex_unnamed_9.z;
				vertex_output_3.y = vertex_unnamed_210.y;
				vertex_output_4.y = vertex_unnamed_210.z;
				vertex_output_5 = vertex_input_4;
				vertex_output_6 = 0.0f.xxxx;
				vertex_output_7 = 0.0f.xxxx;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogColor;
			float4 unity_FogParams;
			float4 unity_SpecCube0_BoxMax;
			float4 unity_SpecCube0_BoxMin;
			float4 unity_SpecCube0_ProbePosition;
			float4 unity_SpecCube0_HDR;
			float4 unity_SpecCube1_BoxMax;
			float4 unity_SpecCube1_BoxMin;
			float4 unity_SpecCube1_ProbePosition;
			float4 unity_SpecCube1_HDR;
			float4 _LightColor0;
			float4 _Color;

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			TextureCube<float4> unity_SpecCube0;
			SamplerState samplerunity_SpecCube0;
			TextureCube<float4> unity_SpecCube1;

			static float4 fragment_input_2;
			static float4 fragment_input_3;
			static float4 fragment_input_4;
			static float2 fragment_input_0;
			static float4 fragment_input_5;
			static float4 fragment_output_0;
			static float fragment_input_1;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float fragment_input_1 : TEXCOORD6; // vs_TEXCOORD6
				float4 fragment_input_2 : TEXCOORD1; // vs_TEXCOORD1
				float4 fragment_input_3 : TEXCOORD2; // vs_TEXCOORD2
				float4 fragment_input_4 : TEXCOORD3; // vs_TEXCOORD3
				float4 fragment_input_5 : UNKNOWN5;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float3 fragment_unnamed_31;
			static float fragment_unnamed_43;
			static float3 fragment_unnamed_49;
			static float4 fragment_unnamed_55;
			static float3 fragment_unnamed_85;
			static float fragment_unnamed_110;
			static float4 fragment_unnamed_134;
			static bool fragment_unnamed_181;
			static float4 fragment_unnamed_198;
			static float3 fragment_unnamed_206;
			static float3 fragment_unnamed_218;
			static bool3 fragment_unnamed_232;
			static bool fragment_unnamed_347;
			static float fragment_unnamed_362;
			static float3 fragment_unnamed_375;
			static float3 fragment_unnamed_386;
			static bool3 fragment_unnamed_397;
			static float3 fragment_unnamed_572;
			static float fragment_unnamed_584;
			static float3 fragment_unnamed_612;

			void frag_main()
			{
				fragment_unnamed_9.x = fragment_input_2.w;
				fragment_unnamed_9.y = fragment_input_3.w;
				fragment_unnamed_9.z = fragment_input_4.w;
				fragment_unnamed_31 = (-fragment_unnamed_9) + _WorldSpaceCameraPos;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_49 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_55 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_55 *= _Color;
				float3 fragment_unnamed_82 = fragment_unnamed_55.xyz * fragment_input_5.xyz;
				fragment_unnamed_55 = float4(fragment_unnamed_82.x, fragment_unnamed_82.y, fragment_unnamed_82.z, fragment_unnamed_55.w);
				fragment_unnamed_85 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_85.x = fragment_unnamed_85.z * fragment_unnamed_85.x;
				float2 fragment_unnamed_107 = (fragment_unnamed_85.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_85 = float3(fragment_unnamed_107.x, fragment_unnamed_107.y, fragment_unnamed_85.z);
				fragment_unnamed_110 = dot(fragment_unnamed_85.xy, fragment_unnamed_85.xy);
				fragment_unnamed_110 = min(fragment_unnamed_110, 1.0f);
				fragment_unnamed_110 = (-fragment_unnamed_110) + 1.0f;
				fragment_unnamed_85.z = sqrt(fragment_unnamed_110);
				fragment_output_0.w = fragment_unnamed_55.w * fragment_input_5.w;
				fragment_unnamed_134.x = dot(fragment_input_2.xyz, fragment_unnamed_85);
				fragment_unnamed_134.y = dot(fragment_input_3.xyz, fragment_unnamed_85);
				fragment_unnamed_134.z = dot(fragment_input_4.xyz, fragment_unnamed_85);
				fragment_unnamed_110 = dot(fragment_unnamed_134.xyz, fragment_unnamed_134.xyz);
				fragment_unnamed_110 = rsqrt(fragment_unnamed_110);
				fragment_unnamed_85 = fragment_unnamed_110.xxx * fragment_unnamed_134.xyz;
				fragment_unnamed_110 = dot(-fragment_unnamed_49, fragment_unnamed_85);
				fragment_unnamed_110 += fragment_unnamed_110;
				float3 fragment_unnamed_176 = (fragment_unnamed_85 * (-fragment_unnamed_110.xxx)) + (-fragment_unnamed_49);
				fragment_unnamed_134 = float4(fragment_unnamed_176.x, fragment_unnamed_176.y, fragment_unnamed_176.z, fragment_unnamed_134.w);
				fragment_unnamed_181 = 0.0f < unity_SpecCube0_ProbePosition.w;
				if (fragment_unnamed_181)
				{
					fragment_unnamed_110 = dot(fragment_unnamed_134.xyz, fragment_unnamed_134.xyz);
					fragment_unnamed_110 = rsqrt(fragment_unnamed_110);
					float3 fragment_unnamed_203 = fragment_unnamed_110.xxx * fragment_unnamed_134.xyz;
					fragment_unnamed_198 = float4(fragment_unnamed_203.x, fragment_unnamed_203.y, fragment_unnamed_203.z, fragment_unnamed_198.w);
					fragment_unnamed_206 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMax.xyz;
					fragment_unnamed_206 /= fragment_unnamed_198.xyz;
					fragment_unnamed_218 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMin.xyz;
					fragment_unnamed_218 /= fragment_unnamed_198.xyz;
					fragment_unnamed_232 = bool4(0.0f.xxxx.x < fragment_unnamed_198.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_198.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_198.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_198.xyzx.w).xyz;
					float3 fragment_unnamed_240 = fragment_unnamed_206;
					float fragment_unnamed_245;
					if (fragment_unnamed_232.x)
					{
						fragment_unnamed_245 = fragment_unnamed_206.x;
					}
					else
					{
						fragment_unnamed_245 = fragment_unnamed_218.x;
					}
					fragment_unnamed_240.x = fragment_unnamed_245;
					float fragment_unnamed_257;
					if (fragment_unnamed_232.y)
					{
						fragment_unnamed_257 = fragment_unnamed_206.y;
					}
					else
					{
						fragment_unnamed_257 = fragment_unnamed_218.y;
					}
					fragment_unnamed_240.y = fragment_unnamed_257;
					float fragment_unnamed_269;
					if (fragment_unnamed_232.z)
					{
						fragment_unnamed_269 = fragment_unnamed_206.z;
					}
					else
					{
						fragment_unnamed_269 = fragment_unnamed_218.z;
					}
					fragment_unnamed_240.z = fragment_unnamed_269;
					fragment_unnamed_206 = fragment_unnamed_240;
					fragment_unnamed_110 = min(fragment_unnamed_206.y, fragment_unnamed_206.x);
					fragment_unnamed_110 = min(fragment_unnamed_206.z, fragment_unnamed_110);
					fragment_unnamed_206 = fragment_unnamed_9 + (-unity_SpecCube0_ProbePosition.xyz);
					float3 fragment_unnamed_301 = (fragment_unnamed_198.xyz * fragment_unnamed_110.xxx) + fragment_unnamed_206;
					fragment_unnamed_198 = float4(fragment_unnamed_301.x, fragment_unnamed_301.y, fragment_unnamed_301.z, fragment_unnamed_198.w);
				}
				else
				{
					fragment_unnamed_198 = float4(fragment_unnamed_134.xyz.x, fragment_unnamed_134.xyz.y, fragment_unnamed_134.xyz.z, fragment_unnamed_198.w);
				}
				fragment_unnamed_198 = unity_SpecCube0.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_198.xyz, 6.0f);
				fragment_unnamed_110 = fragment_unnamed_198.w + (-1.0f);
				fragment_unnamed_110 = (unity_SpecCube0_HDR.w * fragment_unnamed_110) + 1.0f;
				fragment_unnamed_110 = log2(fragment_unnamed_110);
				fragment_unnamed_110 *= unity_SpecCube0_HDR.y;
				fragment_unnamed_110 = exp2(fragment_unnamed_110);
				fragment_unnamed_110 *= unity_SpecCube0_HDR.x;
				fragment_unnamed_206 = fragment_unnamed_198.xyz * fragment_unnamed_110.xxx;
				fragment_unnamed_347 = unity_SpecCube0_BoxMin.w < 0.999989986419677734375f;
				if (fragment_unnamed_347)
				{
					fragment_unnamed_347 = 0.0f < unity_SpecCube1_ProbePosition.w;
					if (fragment_unnamed_347)
					{
						fragment_unnamed_362 = dot(fragment_unnamed_134.xyz, fragment_unnamed_134.xyz);
						fragment_unnamed_362 = rsqrt(fragment_unnamed_362);
						fragment_unnamed_218 = fragment_unnamed_362.xxx * fragment_unnamed_134.xyz;
						fragment_unnamed_375 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMax.xyz;
						fragment_unnamed_375 /= fragment_unnamed_218;
						fragment_unnamed_386 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMin.xyz;
						fragment_unnamed_386 /= fragment_unnamed_218;
						fragment_unnamed_397 = bool4(0.0f.xxxx.x < fragment_unnamed_218.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_218.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_218.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_218.xyzx.w).xyz;
						float3 fragment_unnamed_402 = fragment_unnamed_375;
						float fragment_unnamed_406;
						if (fragment_unnamed_397.x)
						{
							fragment_unnamed_406 = fragment_unnamed_375.x;
						}
						else
						{
							fragment_unnamed_406 = fragment_unnamed_386.x;
						}
						fragment_unnamed_402.x = fragment_unnamed_406;
						float fragment_unnamed_418;
						if (fragment_unnamed_397.y)
						{
							fragment_unnamed_418 = fragment_unnamed_375.y;
						}
						else
						{
							fragment_unnamed_418 = fragment_unnamed_386.y;
						}
						fragment_unnamed_402.y = fragment_unnamed_418;
						float fragment_unnamed_430;
						if (fragment_unnamed_397.z)
						{
							fragment_unnamed_430 = fragment_unnamed_375.z;
						}
						else
						{
							fragment_unnamed_430 = fragment_unnamed_386.z;
						}
						fragment_unnamed_402.z = fragment_unnamed_430;
						fragment_unnamed_375 = fragment_unnamed_402;
						fragment_unnamed_362 = min(fragment_unnamed_375.y, fragment_unnamed_375.x);
						fragment_unnamed_362 = min(fragment_unnamed_375.z, fragment_unnamed_362);
						fragment_unnamed_9 += (-unity_SpecCube1_ProbePosition.xyz);
						float3 fragment_unnamed_461 = (fragment_unnamed_218 * fragment_unnamed_362.xxx) + fragment_unnamed_9;
						fragment_unnamed_134 = float4(fragment_unnamed_461.x, fragment_unnamed_461.y, fragment_unnamed_461.z, fragment_unnamed_134.w);
					}
					fragment_unnamed_134 = unity_SpecCube1.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_134.xyz, 6.0f);
					fragment_unnamed_9.x = fragment_unnamed_134.w + (-1.0f);
					fragment_unnamed_9.x = (unity_SpecCube1_HDR.w * fragment_unnamed_9.x) + 1.0f;
					fragment_unnamed_9.x = log2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.y;
					fragment_unnamed_9.x = exp2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.x;
					fragment_unnamed_9 = fragment_unnamed_134.xyz * fragment_unnamed_9.xxx;
					float3 fragment_unnamed_515 = (fragment_unnamed_110.xxx * fragment_unnamed_198.xyz) + (-fragment_unnamed_9);
					fragment_unnamed_134 = float4(fragment_unnamed_515.x, fragment_unnamed_515.y, fragment_unnamed_515.z, fragment_unnamed_134.w);
					fragment_unnamed_206 = (unity_SpecCube0_BoxMin.www * fragment_unnamed_134.xyz) + fragment_unnamed_9;
				}
				fragment_unnamed_9 = fragment_unnamed_55.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = max(fragment_unnamed_43, 0.001000000047497451305389404296875f);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_31 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_43 = dot(fragment_unnamed_85, fragment_unnamed_49);
				fragment_unnamed_110 = dot(fragment_unnamed_85, _WorldSpaceLightPos0.xyz);
				fragment_unnamed_110 = clamp(fragment_unnamed_110, 0.0f, 1.0f);
				fragment_unnamed_31.x = dot(_WorldSpaceLightPos0.xyz, fragment_unnamed_31);
				fragment_unnamed_31.x = clamp(fragment_unnamed_31.x, 0.0f, 1.0f);
				fragment_unnamed_572.x = dot(fragment_unnamed_31.xx, fragment_unnamed_31.xx);
				fragment_unnamed_572.x += (-0.5f);
				fragment_unnamed_584 = (-fragment_unnamed_110) + 1.0f;
				fragment_unnamed_49.x = fragment_unnamed_584 * fragment_unnamed_584;
				fragment_unnamed_49.x *= fragment_unnamed_49.x;
				fragment_unnamed_584 *= fragment_unnamed_49.x;
				fragment_unnamed_584 = (fragment_unnamed_572.x * fragment_unnamed_584) + 1.0f;
				fragment_unnamed_49.x = (-abs(fragment_unnamed_43)) + 1.0f;
				fragment_unnamed_612.x = fragment_unnamed_49.x * fragment_unnamed_49.x;
				fragment_unnamed_612.x *= fragment_unnamed_612.x;
				fragment_unnamed_49.x *= fragment_unnamed_612.x;
				fragment_unnamed_572.x = (fragment_unnamed_572.x * fragment_unnamed_49.x) + 1.0f;
				fragment_unnamed_572.x *= fragment_unnamed_584;
				fragment_unnamed_572.x = fragment_unnamed_110 * fragment_unnamed_572.x;
				fragment_unnamed_43 = abs(fragment_unnamed_43) + fragment_unnamed_110;
				fragment_unnamed_43 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_43 = 0.5f / fragment_unnamed_43;
				fragment_unnamed_43 = fragment_unnamed_110 * fragment_unnamed_43;
				fragment_unnamed_43 *= 0.99999988079071044921875f;
				fragment_unnamed_572 = fragment_unnamed_572.xxx * _LightColor0.xyz;
				fragment_unnamed_612 = fragment_unnamed_43.xxx * _LightColor0.xyz;
				fragment_unnamed_43 = (-fragment_unnamed_31.x) + 1.0f;
				fragment_unnamed_31.x = fragment_unnamed_43 * fragment_unnamed_43;
				fragment_unnamed_31.x *= fragment_unnamed_31.x;
				fragment_unnamed_43 *= fragment_unnamed_31.x;
				fragment_unnamed_43 = (fragment_unnamed_43 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				fragment_unnamed_612 = fragment_unnamed_43.xxx * fragment_unnamed_612;
				fragment_unnamed_9 = (fragment_unnamed_9 * fragment_unnamed_572) + fragment_unnamed_612;
				fragment_unnamed_31 = fragment_unnamed_206 * 0.5f.xxx;
				fragment_unnamed_43 = (fragment_unnamed_49.x * 2.2351741790771484375e-08f) + 0.039999999105930328369140625f;
				fragment_unnamed_9 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + fragment_unnamed_9;
				fragment_unnamed_43 = fragment_input_1 / _ProjectionParams.y;
				fragment_unnamed_43 = (-fragment_unnamed_43) + 1.0f;
				fragment_unnamed_43 *= _ProjectionParams.z;
				fragment_unnamed_43 = max(fragment_unnamed_43, 0.0f);
				fragment_unnamed_43 = (fragment_unnamed_43 * unity_FogParams.z) + unity_FogParams.w;
				fragment_unnamed_43 = clamp(fragment_unnamed_43, 0.0f, 1.0f);
				fragment_unnamed_9 += (-unity_FogColor.xyz);
				float3 fragment_unnamed_761 = (fragment_unnamed_43.xxx * fragment_unnamed_9) + unity_FogColor.xyz;
				fragment_output_0 = float4(fragment_unnamed_761.x, fragment_unnamed_761.y, fragment_unnamed_761.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_1 = stage_input.fragment_input_1;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // !LIGHTPROBE_SH
			#endif // !VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifdef LIGHTPROBE_SH
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4 unity_SHBr;
			float4 unity_SHBg;
			float4 unity_SHBb;
			float4 unity_SHC;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[6];
			static float4 vertex_uniform_buffer_1[46];
			static float4 vertex_uniform_buffer_2[10];
			static float4 vertex_uniform_buffer_3[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float vertex_output_1;
			static float4 vertex_output_2;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_output_6;
			static float3 vertex_output_7;
			static float4 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float vertex_output_1 : TEXCOORD6; // TEXCOORD_6
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_output_5 : COLOR; // COLOR
				float4 vertex_output_6 : TEXCOORD4; // TEXCOORD_4
				float3 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float4 vertex_output_8 : TEXCOORD8; // TEXCOORD_8
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_2[1u].x;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_2[1u].y;
				precise float vertex_unnamed_68 = vertex_input_0.y * vertex_uniform_buffer_2[1u].z;
				precise float vertex_unnamed_69 = vertex_input_0.y * vertex_uniform_buffer_2[1u].w;
				float vertex_unnamed_92 = mad(vertex_uniform_buffer_2[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].x, vertex_input_0.x, vertex_unnamed_66));
				float vertex_unnamed_93 = mad(vertex_uniform_buffer_2[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].y, vertex_input_0.x, vertex_unnamed_67));
				float vertex_unnamed_94 = mad(vertex_uniform_buffer_2[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].z, vertex_input_0.x, vertex_unnamed_68));
				precise float vertex_unnamed_103 = vertex_unnamed_92 + vertex_uniform_buffer_2[3u].x;
				precise float vertex_unnamed_104 = vertex_unnamed_93 + vertex_uniform_buffer_2[3u].y;
				precise float vertex_unnamed_105 = vertex_unnamed_94 + vertex_uniform_buffer_2[3u].z;
				precise float vertex_unnamed_106 = mad(vertex_uniform_buffer_2[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].w, vertex_input_0.x, vertex_unnamed_69)) + vertex_uniform_buffer_2[3u].w;
				precise float vertex_unnamed_124 = vertex_unnamed_104 * vertex_uniform_buffer_3[18u].x;
				precise float vertex_unnamed_125 = vertex_unnamed_104 * vertex_uniform_buffer_3[18u].y;
				precise float vertex_unnamed_126 = vertex_unnamed_104 * vertex_uniform_buffer_3[18u].z;
				precise float vertex_unnamed_127 = vertex_unnamed_104 * vertex_uniform_buffer_3[18u].w;
				float vertex_unnamed_159 = mad(vertex_uniform_buffer_3[20u].z, vertex_unnamed_106, mad(vertex_uniform_buffer_3[19u].z, vertex_unnamed_105, mad(vertex_uniform_buffer_3[17u].z, vertex_unnamed_103, vertex_unnamed_126)));
				gl_Position.x = mad(vertex_uniform_buffer_3[20u].x, vertex_unnamed_106, mad(vertex_uniform_buffer_3[19u].x, vertex_unnamed_105, mad(vertex_uniform_buffer_3[17u].x, vertex_unnamed_103, vertex_unnamed_124)));
				gl_Position.y = mad(vertex_uniform_buffer_3[20u].y, vertex_unnamed_106, mad(vertex_uniform_buffer_3[19u].y, vertex_unnamed_105, mad(vertex_uniform_buffer_3[17u].y, vertex_unnamed_103, vertex_unnamed_125)));
				gl_Position.z = vertex_unnamed_159;
				gl_Position.w = mad(vertex_uniform_buffer_3[20u].w, vertex_unnamed_106, mad(vertex_uniform_buffer_3[19u].w, vertex_unnamed_105, mad(vertex_uniform_buffer_3[17u].w, vertex_unnamed_103, vertex_unnamed_127)));
				vertex_output_1 = vertex_unnamed_159;
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[5u].x, vertex_uniform_buffer_0[5u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[5u].y, vertex_uniform_buffer_0[5u].w);
				vertex_output_2.w = mad(vertex_uniform_buffer_2[3u].x, vertex_input_0.w, vertex_unnamed_92);
				precise float vertex_unnamed_190 = vertex_input_1.y * vertex_uniform_buffer_2[1u].y;
				precise float vertex_unnamed_191 = vertex_input_1.y * vertex_uniform_buffer_2[1u].z;
				precise float vertex_unnamed_192 = vertex_input_1.y * vertex_uniform_buffer_2[1u].x;
				float vertex_unnamed_210 = mad(vertex_uniform_buffer_2[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].y, vertex_input_1.x, vertex_unnamed_190));
				float vertex_unnamed_211 = mad(vertex_uniform_buffer_2[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].z, vertex_input_1.x, vertex_unnamed_191));
				float vertex_unnamed_212 = mad(vertex_uniform_buffer_2[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].x, vertex_input_1.x, vertex_unnamed_192));
				float vertex_unnamed_216 = rsqrt(dot(float3(vertex_unnamed_210, vertex_unnamed_211, vertex_unnamed_212), float3(vertex_unnamed_210, vertex_unnamed_211, vertex_unnamed_212)));
				precise float vertex_unnamed_217 = vertex_unnamed_216 * vertex_unnamed_210;
				precise float vertex_unnamed_218 = vertex_unnamed_216 * vertex_unnamed_211;
				precise float vertex_unnamed_219 = vertex_unnamed_216 * vertex_unnamed_212;
				vertex_output_2.x = vertex_unnamed_219;
				float vertex_unnamed_233 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[4u].xyz));
				float vertex_unnamed_247 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[5u].xyz));
				float vertex_unnamed_261 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[6u].xyz));
				float vertex_unnamed_267 = rsqrt(dot(float3(vertex_unnamed_233, vertex_unnamed_247, vertex_unnamed_261), float3(vertex_unnamed_233, vertex_unnamed_247, vertex_unnamed_261)));
				precise float vertex_unnamed_268 = vertex_unnamed_267 * vertex_unnamed_233;
				precise float vertex_unnamed_269 = vertex_unnamed_267 * vertex_unnamed_247;
				precise float vertex_unnamed_270 = vertex_unnamed_267 * vertex_unnamed_261;
				precise float vertex_unnamed_271 = vertex_unnamed_267 * vertex_unnamed_261;
				precise float vertex_unnamed_272 = vertex_unnamed_217 * vertex_unnamed_271;
				precise float vertex_unnamed_273 = vertex_unnamed_218 * vertex_unnamed_268;
				precise float vertex_unnamed_274 = vertex_unnamed_219 * vertex_unnamed_269;
				precise float vertex_unnamed_275 = (-0.0f) - vertex_unnamed_272;
				precise float vertex_unnamed_277 = (-0.0f) - vertex_unnamed_273;
				precise float vertex_unnamed_278 = (-0.0f) - vertex_unnamed_274;
				precise float vertex_unnamed_288 = vertex_input_1.w * vertex_uniform_buffer_2[9u].w;
				precise float vertex_unnamed_289 = vertex_unnamed_288 * mad(vertex_unnamed_269, vertex_unnamed_218, vertex_unnamed_275);
				precise float vertex_unnamed_290 = vertex_unnamed_288 * mad(vertex_unnamed_271, vertex_unnamed_219, vertex_unnamed_277);
				precise float vertex_unnamed_291 = vertex_unnamed_288 * mad(vertex_unnamed_268, vertex_unnamed_217, vertex_unnamed_278);
				vertex_output_2.y = vertex_unnamed_289;
				vertex_output_2.z = vertex_unnamed_268;
				vertex_output_3.x = vertex_unnamed_217;
				vertex_output_4.x = vertex_unnamed_218;
				vertex_output_3.w = mad(vertex_uniform_buffer_2[3u].y, vertex_input_0.w, vertex_unnamed_93);
				vertex_output_4.w = mad(vertex_uniform_buffer_2[3u].z, vertex_input_0.w, vertex_unnamed_94);
				vertex_output_3.y = vertex_unnamed_290;
				vertex_output_4.y = vertex_unnamed_291;
				vertex_output_3.z = vertex_unnamed_269;
				vertex_output_4.z = vertex_unnamed_271;
				vertex_output_5.x = vertex_input_7.x;
				vertex_output_5.y = vertex_input_7.y;
				vertex_output_5.z = vertex_input_7.z;
				vertex_output_5.w = vertex_input_7.w;
				vertex_output_6.x = 0.0f;
				vertex_output_6.y = 0.0f;
				vertex_output_6.z = 0.0f;
				vertex_output_6.w = 0.0f;
				precise float vertex_unnamed_319 = vertex_unnamed_269 * vertex_unnamed_269;
				precise float vertex_unnamed_320 = (-0.0f) - vertex_unnamed_319;
				float vertex_unnamed_321 = mad(vertex_unnamed_268, vertex_unnamed_268, vertex_unnamed_320);
				precise float vertex_unnamed_322 = vertex_unnamed_269 * vertex_unnamed_268;
				precise float vertex_unnamed_323 = vertex_unnamed_271 * vertex_unnamed_269;
				precise float vertex_unnamed_324 = vertex_unnamed_270 * vertex_unnamed_270;
				precise float vertex_unnamed_325 = vertex_unnamed_268 * vertex_unnamed_271;
				vertex_output_7.x = mad(vertex_uniform_buffer_1[45u].x, vertex_unnamed_321, dot(float4(vertex_uniform_buffer_1[42u]), float4(vertex_unnamed_322, vertex_unnamed_323, vertex_unnamed_324, vertex_unnamed_325)));
				vertex_output_7.y = mad(vertex_uniform_buffer_1[45u].y, vertex_unnamed_321, dot(float4(vertex_uniform_buffer_1[43u]), float4(vertex_unnamed_322, vertex_unnamed_323, vertex_unnamed_324, vertex_unnamed_325)));
				vertex_output_7.z = mad(vertex_uniform_buffer_1[45u].z, vertex_unnamed_321, dot(float4(vertex_uniform_buffer_1[44u]), float4(vertex_unnamed_322, vertex_unnamed_323, vertex_unnamed_324, vertex_unnamed_325)));
				vertex_output_8.x = 0.0f;
				vertex_output_8.y = 0.0f;
				vertex_output_8.z = 0.0f;
				vertex_output_8.w = 0.0f;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[5] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[42] = float4(unity_SHBr[0], unity_SHBr[1], unity_SHBr[2], unity_SHBr[3]);

				vertex_uniform_buffer_1[43] = float4(unity_SHBg[0], unity_SHBg[1], unity_SHBg[2], unity_SHBg[3]);

				vertex_uniform_buffer_1[44] = float4(unity_SHBb[0], unity_SHBb[1], unity_SHBb[2], unity_SHBb[3]);

				vertex_uniform_buffer_1[45] = float4(unity_SHC[0], unity_SHC[1], unity_SHC[2], unity_SHC[3]);

				vertex_uniform_buffer_2[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_2[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_2[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_2[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_2[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_2[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_2[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_2[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_3[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_3[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_3[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_3[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // LIGHTPROBE_SH
			#endif // !VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifdef LIGHTPROBE_SH
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 unity_SHBr;
			float4 unity_SHBg;
			float4 unity_SHBb;
			float4 unity_SHC;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float vertex_output_1;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float4 vertex_output_2;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_input_4;
			static float4 vertex_output_7;
			static float3 vertex_output_6;
			static float4 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float vertex_output_1 : TEXCOORD6; // vs_TEXCOORD6
				float4 vertex_output_2 : TEXCOORD1; // vs_TEXCOORD1
				float4 vertex_output_3 : TEXCOORD2; // vs_TEXCOORD2
				float4 vertex_output_4 : TEXCOORD3; // vs_TEXCOORD3
				float4 vertex_output_5 : UNKNOWN5;
				float3 vertex_output_6 : TEXCOORD5; // vs_TEXCOORD5
				float4 vertex_output_7 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_8 : TEXCOORD8; // vs_TEXCOORD8
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_45;
			static float4 vertex_unnamed_63;
			static float3 vertex_unnamed_214;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_45 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				float3 vertex_unnamed_60 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_60.x, vertex_unnamed_60.y, vertex_unnamed_60.z, vertex_unnamed_9.w);
				vertex_unnamed_63 = vertex_unnamed_45.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_63 = (unity_MatrixVP__array[0] * vertex_unnamed_45.xxxx) + vertex_unnamed_63;
				vertex_unnamed_63 = (unity_MatrixVP__array[2] * vertex_unnamed_45.zzzz) + vertex_unnamed_63;
				vertex_unnamed_45 = (unity_MatrixVP__array[3] * vertex_unnamed_45.wwww) + vertex_unnamed_63;
				gl_Position = vertex_unnamed_45;
				vertex_output_1 = vertex_unnamed_45.z;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_output_2.w = vertex_unnamed_9.x;
				float3 vertex_unnamed_132 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_45 = float4(vertex_unnamed_132.x, vertex_unnamed_132.y, vertex_unnamed_132.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_143 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_143.x, vertex_unnamed_143.y, vertex_unnamed_143.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_154 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_154.x, vertex_unnamed_154.y, vertex_unnamed_154.z, vertex_unnamed_45.w);
				vertex_unnamed_9.x = dot(vertex_unnamed_45.xyz, vertex_unnamed_45.xyz);
				vertex_unnamed_9.x = rsqrt(vertex_unnamed_9.x);
				float3 vertex_unnamed_171 = vertex_unnamed_9.xxx * vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_171.x, vertex_unnamed_171.y, vertex_unnamed_171.z, vertex_unnamed_45.w);
				vertex_output_2.x = vertex_unnamed_45.z;
				vertex_unnamed_63.x = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_63.y = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_63.z = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_9.x = dot(vertex_unnamed_63.xyz, vertex_unnamed_63.xyz);
				vertex_unnamed_9.x = rsqrt(vertex_unnamed_9.x);
				vertex_unnamed_63 = vertex_unnamed_9.xxxx * vertex_unnamed_63.xyzz;
				vertex_unnamed_214 = vertex_unnamed_45.xyz * vertex_unnamed_63.wxy;
				vertex_unnamed_214 = (vertex_unnamed_63.ywx * vertex_unnamed_45.yzx) + (-vertex_unnamed_214);
				vertex_unnamed_9.x = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_214 = vertex_unnamed_9.xxx * vertex_unnamed_214;
				vertex_output_2.y = vertex_unnamed_214.x;
				vertex_output_2.z = vertex_unnamed_63.x;
				vertex_output_3.x = vertex_unnamed_45.x;
				vertex_output_4.x = vertex_unnamed_45.y;
				vertex_output_3.w = vertex_unnamed_9.y;
				vertex_output_4.w = vertex_unnamed_9.z;
				vertex_output_3.y = vertex_unnamed_214.y;
				vertex_output_4.y = vertex_unnamed_214.z;
				vertex_output_3.z = vertex_unnamed_63.y;
				vertex_output_4.z = vertex_unnamed_63.w;
				vertex_output_5 = vertex_input_4;
				vertex_output_7 = 0.0f.xxxx;
				vertex_unnamed_9.x = vertex_unnamed_63.y * vertex_unnamed_63.y;
				vertex_unnamed_9.x = (vertex_unnamed_63.x * vertex_unnamed_63.x) + (-vertex_unnamed_9.x);
				vertex_unnamed_45 = vertex_unnamed_63.ywzx * vertex_unnamed_63;
				vertex_unnamed_63.x = dot(unity_SHBr, vertex_unnamed_45);
				vertex_unnamed_63.y = dot(unity_SHBg, vertex_unnamed_45);
				vertex_unnamed_63.z = dot(unity_SHBb, vertex_unnamed_45);
				vertex_output_6 = (unity_SHC.xyz * vertex_unnamed_9.xxx) + vertex_unnamed_63.xyz;
				vertex_output_8 = 0.0f.xxxx;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_SHAr;
			float4 unity_SHAg;
			float4 unity_SHAb;
			float4 unity_FogColor;
			float4 unity_FogParams;
			float4 unity_SpecCube0_BoxMax;
			float4 unity_SpecCube0_BoxMin;
			float4 unity_SpecCube0_ProbePosition;
			float4 unity_SpecCube0_HDR;
			float4 unity_SpecCube1_BoxMax;
			float4 unity_SpecCube1_BoxMin;
			float4 unity_SpecCube1_ProbePosition;
			float4 unity_SpecCube1_HDR;
			float4 _LightColor0;
			float4 _Color;

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			TextureCube<float4> unity_SpecCube0;
			SamplerState samplerunity_SpecCube0;
			TextureCube<float4> unity_SpecCube1;

			static float4 fragment_input_2;
			static float4 fragment_input_3;
			static float4 fragment_input_4;
			static float2 fragment_input_0;
			static float4 fragment_input_5;
			static float4 fragment_output_0;
			static float3 fragment_input_6;
			static float fragment_input_1;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float fragment_input_1 : TEXCOORD6; // vs_TEXCOORD6
				float4 fragment_input_2 : TEXCOORD1; // vs_TEXCOORD1
				float4 fragment_input_3 : TEXCOORD2; // vs_TEXCOORD2
				float4 fragment_input_4 : TEXCOORD3; // vs_TEXCOORD3
				float4 fragment_input_5 : UNKNOWN5;
				float3 fragment_input_6 : TEXCOORD5; // vs_TEXCOORD5
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float3 fragment_unnamed_31;
			static float fragment_unnamed_43;
			static float3 fragment_unnamed_49;
			static float4 fragment_unnamed_55;
			static float4 fragment_unnamed_85;
			static float fragment_unnamed_112;
			static float4 fragment_unnamed_136;
			static float3 fragment_unnamed_189;
			static bool fragment_unnamed_219;
			static float4 fragment_unnamed_235;
			static float3 fragment_unnamed_243;
			static float3 fragment_unnamed_255;
			static bool3 fragment_unnamed_269;
			static bool fragment_unnamed_384;
			static float fragment_unnamed_399;
			static float3 fragment_unnamed_412;
			static float3 fragment_unnamed_423;
			static bool3 fragment_unnamed_434;
			static float3 fragment_unnamed_611;
			static float fragment_unnamed_623;
			static float3 fragment_unnamed_651;

			void frag_main()
			{
				fragment_unnamed_9.x = fragment_input_2.w;
				fragment_unnamed_9.y = fragment_input_3.w;
				fragment_unnamed_9.z = fragment_input_4.w;
				fragment_unnamed_31 = (-fragment_unnamed_9) + _WorldSpaceCameraPos;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_49 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_55 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_55 *= _Color;
				float3 fragment_unnamed_82 = fragment_unnamed_55.xyz * fragment_input_5.xyz;
				fragment_unnamed_55 = float4(fragment_unnamed_82.x, fragment_unnamed_82.y, fragment_unnamed_82.z, fragment_unnamed_55.w);
				float3 fragment_unnamed_93 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_85 = float4(fragment_unnamed_93.x, fragment_unnamed_93.y, fragment_unnamed_93.z, fragment_unnamed_85.w);
				fragment_unnamed_85.x = fragment_unnamed_85.z * fragment_unnamed_85.x;
				float2 fragment_unnamed_109 = (fragment_unnamed_85.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_85 = float4(fragment_unnamed_109.x, fragment_unnamed_109.y, fragment_unnamed_85.z, fragment_unnamed_85.w);
				fragment_unnamed_112 = dot(fragment_unnamed_85.xy, fragment_unnamed_85.xy);
				fragment_unnamed_112 = min(fragment_unnamed_112, 1.0f);
				fragment_unnamed_112 = (-fragment_unnamed_112) + 1.0f;
				fragment_unnamed_85.z = sqrt(fragment_unnamed_112);
				fragment_output_0.w = fragment_unnamed_55.w * fragment_input_5.w;
				fragment_unnamed_136.x = dot(fragment_input_2.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_136.y = dot(fragment_input_3.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_136.z = dot(fragment_input_4.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_112 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
				fragment_unnamed_112 = rsqrt(fragment_unnamed_112);
				float3 fragment_unnamed_166 = fragment_unnamed_112.xxx * fragment_unnamed_136.xyz;
				fragment_unnamed_85 = float4(fragment_unnamed_166.x, fragment_unnamed_166.y, fragment_unnamed_166.z, fragment_unnamed_85.w);
				fragment_unnamed_112 = dot(-fragment_unnamed_49, fragment_unnamed_85.xyz);
				fragment_unnamed_112 += fragment_unnamed_112;
				float3 fragment_unnamed_185 = (fragment_unnamed_85.xyz * (-fragment_unnamed_112.xxx)) + (-fragment_unnamed_49);
				fragment_unnamed_136 = float4(fragment_unnamed_185.x, fragment_unnamed_185.y, fragment_unnamed_185.z, fragment_unnamed_136.w);
				fragment_unnamed_85.w = 1.0f;
				fragment_unnamed_189.x = dot(unity_SHAr, fragment_unnamed_85);
				fragment_unnamed_189.y = dot(unity_SHAg, fragment_unnamed_85);
				fragment_unnamed_189.z = dot(unity_SHAb, fragment_unnamed_85);
				fragment_unnamed_189 += fragment_input_6;
				fragment_unnamed_189 = max(fragment_unnamed_189, 0.0f.xxx);
				fragment_unnamed_219 = 0.0f < unity_SpecCube0_ProbePosition.w;
				if (fragment_unnamed_219)
				{
					fragment_unnamed_112 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
					fragment_unnamed_112 = rsqrt(fragment_unnamed_112);
					float3 fragment_unnamed_240 = fragment_unnamed_112.xxx * fragment_unnamed_136.xyz;
					fragment_unnamed_235 = float4(fragment_unnamed_240.x, fragment_unnamed_240.y, fragment_unnamed_240.z, fragment_unnamed_235.w);
					fragment_unnamed_243 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMax.xyz;
					fragment_unnamed_243 /= fragment_unnamed_235.xyz;
					fragment_unnamed_255 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMin.xyz;
					fragment_unnamed_255 /= fragment_unnamed_235.xyz;
					fragment_unnamed_269 = bool4(0.0f.xxxx.x < fragment_unnamed_235.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_235.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_235.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_235.xyzx.w).xyz;
					float3 fragment_unnamed_277 = fragment_unnamed_243;
					float fragment_unnamed_282;
					if (fragment_unnamed_269.x)
					{
						fragment_unnamed_282 = fragment_unnamed_243.x;
					}
					else
					{
						fragment_unnamed_282 = fragment_unnamed_255.x;
					}
					fragment_unnamed_277.x = fragment_unnamed_282;
					float fragment_unnamed_294;
					if (fragment_unnamed_269.y)
					{
						fragment_unnamed_294 = fragment_unnamed_243.y;
					}
					else
					{
						fragment_unnamed_294 = fragment_unnamed_255.y;
					}
					fragment_unnamed_277.y = fragment_unnamed_294;
					float fragment_unnamed_306;
					if (fragment_unnamed_269.z)
					{
						fragment_unnamed_306 = fragment_unnamed_243.z;
					}
					else
					{
						fragment_unnamed_306 = fragment_unnamed_255.z;
					}
					fragment_unnamed_277.z = fragment_unnamed_306;
					fragment_unnamed_243 = fragment_unnamed_277;
					fragment_unnamed_112 = min(fragment_unnamed_243.y, fragment_unnamed_243.x);
					fragment_unnamed_112 = min(fragment_unnamed_243.z, fragment_unnamed_112);
					fragment_unnamed_243 = fragment_unnamed_9 + (-unity_SpecCube0_ProbePosition.xyz);
					float3 fragment_unnamed_338 = (fragment_unnamed_235.xyz * fragment_unnamed_112.xxx) + fragment_unnamed_243;
					fragment_unnamed_235 = float4(fragment_unnamed_338.x, fragment_unnamed_338.y, fragment_unnamed_338.z, fragment_unnamed_235.w);
				}
				else
				{
					fragment_unnamed_235 = float4(fragment_unnamed_136.xyz.x, fragment_unnamed_136.xyz.y, fragment_unnamed_136.xyz.z, fragment_unnamed_235.w);
				}
				fragment_unnamed_235 = unity_SpecCube0.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_235.xyz, 6.0f);
				fragment_unnamed_112 = fragment_unnamed_235.w + (-1.0f);
				fragment_unnamed_112 = (unity_SpecCube0_HDR.w * fragment_unnamed_112) + 1.0f;
				fragment_unnamed_112 = log2(fragment_unnamed_112);
				fragment_unnamed_112 *= unity_SpecCube0_HDR.y;
				fragment_unnamed_112 = exp2(fragment_unnamed_112);
				fragment_unnamed_112 *= unity_SpecCube0_HDR.x;
				fragment_unnamed_243 = fragment_unnamed_235.xyz * fragment_unnamed_112.xxx;
				fragment_unnamed_384 = unity_SpecCube0_BoxMin.w < 0.999989986419677734375f;
				if (fragment_unnamed_384)
				{
					fragment_unnamed_384 = 0.0f < unity_SpecCube1_ProbePosition.w;
					if (fragment_unnamed_384)
					{
						fragment_unnamed_399 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
						fragment_unnamed_399 = rsqrt(fragment_unnamed_399);
						fragment_unnamed_255 = fragment_unnamed_399.xxx * fragment_unnamed_136.xyz;
						fragment_unnamed_412 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMax.xyz;
						fragment_unnamed_412 /= fragment_unnamed_255;
						fragment_unnamed_423 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMin.xyz;
						fragment_unnamed_423 /= fragment_unnamed_255;
						fragment_unnamed_434 = bool4(0.0f.xxxx.x < fragment_unnamed_255.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_255.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_255.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_255.xyzx.w).xyz;
						float3 fragment_unnamed_439 = fragment_unnamed_412;
						float fragment_unnamed_443;
						if (fragment_unnamed_434.x)
						{
							fragment_unnamed_443 = fragment_unnamed_412.x;
						}
						else
						{
							fragment_unnamed_443 = fragment_unnamed_423.x;
						}
						fragment_unnamed_439.x = fragment_unnamed_443;
						float fragment_unnamed_455;
						if (fragment_unnamed_434.y)
						{
							fragment_unnamed_455 = fragment_unnamed_412.y;
						}
						else
						{
							fragment_unnamed_455 = fragment_unnamed_423.y;
						}
						fragment_unnamed_439.y = fragment_unnamed_455;
						float fragment_unnamed_467;
						if (fragment_unnamed_434.z)
						{
							fragment_unnamed_467 = fragment_unnamed_412.z;
						}
						else
						{
							fragment_unnamed_467 = fragment_unnamed_423.z;
						}
						fragment_unnamed_439.z = fragment_unnamed_467;
						fragment_unnamed_412 = fragment_unnamed_439;
						fragment_unnamed_399 = min(fragment_unnamed_412.y, fragment_unnamed_412.x);
						fragment_unnamed_399 = min(fragment_unnamed_412.z, fragment_unnamed_399);
						fragment_unnamed_9 += (-unity_SpecCube1_ProbePosition.xyz);
						float3 fragment_unnamed_498 = (fragment_unnamed_255 * fragment_unnamed_399.xxx) + fragment_unnamed_9;
						fragment_unnamed_136 = float4(fragment_unnamed_498.x, fragment_unnamed_498.y, fragment_unnamed_498.z, fragment_unnamed_136.w);
					}
					fragment_unnamed_136 = unity_SpecCube1.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_136.xyz, 6.0f);
					fragment_unnamed_9.x = fragment_unnamed_136.w + (-1.0f);
					fragment_unnamed_9.x = (unity_SpecCube1_HDR.w * fragment_unnamed_9.x) + 1.0f;
					fragment_unnamed_9.x = log2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.y;
					fragment_unnamed_9.x = exp2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.x;
					fragment_unnamed_9 = fragment_unnamed_136.xyz * fragment_unnamed_9.xxx;
					float3 fragment_unnamed_552 = (fragment_unnamed_112.xxx * fragment_unnamed_235.xyz) + (-fragment_unnamed_9);
					fragment_unnamed_136 = float4(fragment_unnamed_552.x, fragment_unnamed_552.y, fragment_unnamed_552.z, fragment_unnamed_136.w);
					fragment_unnamed_243 = (unity_SpecCube0_BoxMin.www * fragment_unnamed_136.xyz) + fragment_unnamed_9;
				}
				fragment_unnamed_9 = fragment_unnamed_55.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = max(fragment_unnamed_43, 0.001000000047497451305389404296875f);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_31 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_43 = dot(fragment_unnamed_85.xyz, fragment_unnamed_49);
				fragment_unnamed_112 = dot(fragment_unnamed_85.xyz, _WorldSpaceLightPos0.xyz);
				fragment_unnamed_112 = clamp(fragment_unnamed_112, 0.0f, 1.0f);
				fragment_unnamed_31.x = dot(_WorldSpaceLightPos0.xyz, fragment_unnamed_31);
				fragment_unnamed_31.x = clamp(fragment_unnamed_31.x, 0.0f, 1.0f);
				fragment_unnamed_611.x = dot(fragment_unnamed_31.xx, fragment_unnamed_31.xx);
				fragment_unnamed_611.x += (-0.5f);
				fragment_unnamed_623 = (-fragment_unnamed_112) + 1.0f;
				fragment_unnamed_49.x = fragment_unnamed_623 * fragment_unnamed_623;
				fragment_unnamed_49.x *= fragment_unnamed_49.x;
				fragment_unnamed_623 *= fragment_unnamed_49.x;
				fragment_unnamed_623 = (fragment_unnamed_611.x * fragment_unnamed_623) + 1.0f;
				fragment_unnamed_49.x = (-abs(fragment_unnamed_43)) + 1.0f;
				fragment_unnamed_651.x = fragment_unnamed_49.x * fragment_unnamed_49.x;
				fragment_unnamed_651.x *= fragment_unnamed_651.x;
				fragment_unnamed_49.x *= fragment_unnamed_651.x;
				fragment_unnamed_611.x = (fragment_unnamed_611.x * fragment_unnamed_49.x) + 1.0f;
				fragment_unnamed_611.x *= fragment_unnamed_623;
				fragment_unnamed_611.x = fragment_unnamed_112 * fragment_unnamed_611.x;
				fragment_unnamed_43 = abs(fragment_unnamed_43) + fragment_unnamed_112;
				fragment_unnamed_43 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_43 = 0.5f / fragment_unnamed_43;
				fragment_unnamed_43 = fragment_unnamed_112 * fragment_unnamed_43;
				fragment_unnamed_43 *= 0.99999988079071044921875f;
				fragment_unnamed_611 = (_LightColor0.xyz * fragment_unnamed_611.xxx) + fragment_unnamed_189;
				fragment_unnamed_651 = fragment_unnamed_43.xxx * _LightColor0.xyz;
				fragment_unnamed_43 = (-fragment_unnamed_31.x) + 1.0f;
				fragment_unnamed_31.x = fragment_unnamed_43 * fragment_unnamed_43;
				fragment_unnamed_31.x *= fragment_unnamed_31.x;
				fragment_unnamed_43 *= fragment_unnamed_31.x;
				fragment_unnamed_43 = (fragment_unnamed_43 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				fragment_unnamed_651 = fragment_unnamed_43.xxx * fragment_unnamed_651;
				fragment_unnamed_9 = (fragment_unnamed_9 * fragment_unnamed_611) + fragment_unnamed_651;
				fragment_unnamed_31 = fragment_unnamed_243 * 0.5f.xxx;
				fragment_unnamed_43 = (fragment_unnamed_49.x * 2.2351741790771484375e-08f) + 0.039999999105930328369140625f;
				fragment_unnamed_9 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + fragment_unnamed_9;
				fragment_unnamed_43 = fragment_input_1 / _ProjectionParams.y;
				fragment_unnamed_43 = (-fragment_unnamed_43) + 1.0f;
				fragment_unnamed_43 *= _ProjectionParams.z;
				fragment_unnamed_43 = max(fragment_unnamed_43, 0.0f);
				fragment_unnamed_43 = (fragment_unnamed_43 * unity_FogParams.z) + unity_FogParams.w;
				fragment_unnamed_43 = clamp(fragment_unnamed_43, 0.0f, 1.0f);
				fragment_unnamed_9 += (-unity_FogColor.xyz);
				float3 fragment_unnamed_802 = (fragment_unnamed_43.xxx * fragment_unnamed_9) + unity_FogColor.xyz;
				fragment_output_0 = float4(fragment_unnamed_802.x, fragment_unnamed_802.y, fragment_unnamed_802.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_1 = stage_input.fragment_input_1;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // LIGHTPROBE_SH
			#endif // !VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifdef LIGHTPROBE_SH
			#ifdef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4 unity_4LightPosX0;
			float4 unity_4LightPosY0;
			float4 unity_4LightPosZ0;
			float4 unity_4LightAtten0;
			float4 unity_LightColor[8];
			float4 unity_SHBr;
			float4 unity_SHBg;
			float4 unity_SHBb;
			float4 unity_SHC;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[6];
			static float4 vertex_uniform_buffer_1[46];
			static float4 vertex_uniform_buffer_2[10];
			static float4 vertex_uniform_buffer_3[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float vertex_output_1;
			static float4 vertex_output_2;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_output_6;
			static float3 vertex_output_7;
			static float4 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float vertex_output_1 : TEXCOORD6; // TEXCOORD_6
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_output_5 : COLOR; // COLOR
				float4 vertex_output_6 : TEXCOORD4; // TEXCOORD_4
				float3 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float4 vertex_output_8 : TEXCOORD8; // TEXCOORD_8
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_2[1u].x;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_2[1u].y;
				precise float vertex_unnamed_68 = vertex_input_0.y * vertex_uniform_buffer_2[1u].z;
				precise float vertex_unnamed_69 = vertex_input_0.y * vertex_uniform_buffer_2[1u].w;
				float vertex_unnamed_92 = mad(vertex_uniform_buffer_2[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].x, vertex_input_0.x, vertex_unnamed_66));
				float vertex_unnamed_93 = mad(vertex_uniform_buffer_2[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].y, vertex_input_0.x, vertex_unnamed_67));
				float vertex_unnamed_94 = mad(vertex_uniform_buffer_2[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].z, vertex_input_0.x, vertex_unnamed_68));
				precise float vertex_unnamed_103 = vertex_unnamed_92 + vertex_uniform_buffer_2[3u].x;
				precise float vertex_unnamed_104 = vertex_unnamed_93 + vertex_uniform_buffer_2[3u].y;
				precise float vertex_unnamed_105 = vertex_unnamed_94 + vertex_uniform_buffer_2[3u].z;
				precise float vertex_unnamed_106 = mad(vertex_uniform_buffer_2[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_2[0u].w, vertex_input_0.x, vertex_unnamed_69)) + vertex_uniform_buffer_2[3u].w;
				float vertex_unnamed_114 = mad(vertex_uniform_buffer_2[3u].x, vertex_input_0.w, vertex_unnamed_92);
				float vertex_unnamed_115 = mad(vertex_uniform_buffer_2[3u].y, vertex_input_0.w, vertex_unnamed_93);
				float vertex_unnamed_116 = mad(vertex_uniform_buffer_2[3u].z, vertex_input_0.w, vertex_unnamed_94);
				precise float vertex_unnamed_124 = vertex_unnamed_104 * vertex_uniform_buffer_3[18u].x;
				precise float vertex_unnamed_125 = vertex_unnamed_104 * vertex_uniform_buffer_3[18u].y;
				precise float vertex_unnamed_126 = vertex_unnamed_104 * vertex_uniform_buffer_3[18u].z;
				precise float vertex_unnamed_127 = vertex_unnamed_104 * vertex_uniform_buffer_3[18u].w;
				float vertex_unnamed_159 = mad(vertex_uniform_buffer_3[20u].z, vertex_unnamed_106, mad(vertex_uniform_buffer_3[19u].z, vertex_unnamed_105, mad(vertex_uniform_buffer_3[17u].z, vertex_unnamed_103, vertex_unnamed_126)));
				gl_Position.x = mad(vertex_uniform_buffer_3[20u].x, vertex_unnamed_106, mad(vertex_uniform_buffer_3[19u].x, vertex_unnamed_105, mad(vertex_uniform_buffer_3[17u].x, vertex_unnamed_103, vertex_unnamed_124)));
				gl_Position.y = mad(vertex_uniform_buffer_3[20u].y, vertex_unnamed_106, mad(vertex_uniform_buffer_3[19u].y, vertex_unnamed_105, mad(vertex_uniform_buffer_3[17u].y, vertex_unnamed_103, vertex_unnamed_125)));
				gl_Position.z = vertex_unnamed_159;
				gl_Position.w = mad(vertex_uniform_buffer_3[20u].w, vertex_unnamed_106, mad(vertex_uniform_buffer_3[19u].w, vertex_unnamed_105, mad(vertex_uniform_buffer_3[17u].w, vertex_unnamed_103, vertex_unnamed_127)));
				vertex_output_1 = vertex_unnamed_159;
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[5u].x, vertex_uniform_buffer_0[5u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[5u].y, vertex_uniform_buffer_0[5u].w);
				precise float vertex_unnamed_189 = vertex_input_1.y * vertex_uniform_buffer_2[1u].y;
				precise float vertex_unnamed_190 = vertex_input_1.y * vertex_uniform_buffer_2[1u].z;
				precise float vertex_unnamed_191 = vertex_input_1.y * vertex_uniform_buffer_2[1u].x;
				float vertex_unnamed_209 = mad(vertex_uniform_buffer_2[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].y, vertex_input_1.x, vertex_unnamed_189));
				float vertex_unnamed_210 = mad(vertex_uniform_buffer_2[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].z, vertex_input_1.x, vertex_unnamed_190));
				float vertex_unnamed_211 = mad(vertex_uniform_buffer_2[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_2[0u].x, vertex_input_1.x, vertex_unnamed_191));
				float vertex_unnamed_215 = rsqrt(dot(float3(vertex_unnamed_209, vertex_unnamed_210, vertex_unnamed_211), float3(vertex_unnamed_209, vertex_unnamed_210, vertex_unnamed_211)));
				precise float vertex_unnamed_216 = vertex_unnamed_215 * vertex_unnamed_209;
				precise float vertex_unnamed_217 = vertex_unnamed_215 * vertex_unnamed_210;
				precise float vertex_unnamed_218 = vertex_unnamed_215 * vertex_unnamed_211;
				vertex_output_2.x = vertex_unnamed_218;
				precise float vertex_unnamed_226 = vertex_input_1.w * vertex_uniform_buffer_2[9u].w;
				float vertex_unnamed_239 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[4u].xyz));
				float vertex_unnamed_253 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[5u].xyz));
				float vertex_unnamed_267 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_2[6u].xyz));
				float vertex_unnamed_273 = rsqrt(dot(float3(vertex_unnamed_239, vertex_unnamed_253, vertex_unnamed_267), float3(vertex_unnamed_239, vertex_unnamed_253, vertex_unnamed_267)));
				precise float vertex_unnamed_274 = vertex_unnamed_273 * vertex_unnamed_239;
				precise float vertex_unnamed_275 = vertex_unnamed_273 * vertex_unnamed_253;
				precise float vertex_unnamed_276 = vertex_unnamed_273 * vertex_unnamed_267;
				precise float vertex_unnamed_277 = vertex_unnamed_273 * vertex_unnamed_267;
				precise float vertex_unnamed_278 = vertex_unnamed_216 * vertex_unnamed_277;
				precise float vertex_unnamed_279 = vertex_unnamed_217 * vertex_unnamed_274;
				precise float vertex_unnamed_280 = vertex_unnamed_218 * vertex_unnamed_275;
				precise float vertex_unnamed_281 = (-0.0f) - vertex_unnamed_278;
				precise float vertex_unnamed_283 = (-0.0f) - vertex_unnamed_279;
				precise float vertex_unnamed_284 = (-0.0f) - vertex_unnamed_280;
				precise float vertex_unnamed_288 = vertex_unnamed_226 * mad(vertex_unnamed_275, vertex_unnamed_217, vertex_unnamed_281);
				precise float vertex_unnamed_289 = vertex_unnamed_226 * mad(vertex_unnamed_277, vertex_unnamed_218, vertex_unnamed_283);
				precise float vertex_unnamed_290 = vertex_unnamed_226 * mad(vertex_unnamed_274, vertex_unnamed_216, vertex_unnamed_284);
				vertex_output_2.y = vertex_unnamed_288;
				vertex_output_2.w = vertex_unnamed_114;
				vertex_output_2.z = vertex_unnamed_274;
				vertex_output_3.x = vertex_unnamed_216;
				vertex_output_4.x = vertex_unnamed_217;
				vertex_output_3.y = vertex_unnamed_289;
				vertex_output_4.y = vertex_unnamed_290;
				vertex_output_3.w = vertex_unnamed_115;
				vertex_output_3.z = vertex_unnamed_275;
				vertex_output_4.w = vertex_unnamed_116;
				vertex_output_4.z = vertex_unnamed_277;
				vertex_output_5.x = vertex_input_7.x;
				vertex_output_5.y = vertex_input_7.y;
				vertex_output_5.z = vertex_input_7.z;
				vertex_output_5.w = vertex_input_7.w;
				vertex_output_6.x = 0.0f;
				vertex_output_6.y = 0.0f;
				vertex_output_6.z = 0.0f;
				vertex_output_6.w = 0.0f;
				precise float vertex_unnamed_319 = (-0.0f) - vertex_unnamed_114;
				precise float vertex_unnamed_326 = vertex_unnamed_319 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_327 = vertex_unnamed_319 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_328 = vertex_unnamed_319 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_329 = vertex_unnamed_319 + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_330 = (-0.0f) - vertex_unnamed_115;
				precise float vertex_unnamed_337 = vertex_unnamed_330 + vertex_uniform_buffer_1[4u].x;
				precise float vertex_unnamed_338 = vertex_unnamed_330 + vertex_uniform_buffer_1[4u].y;
				precise float vertex_unnamed_339 = vertex_unnamed_330 + vertex_uniform_buffer_1[4u].z;
				precise float vertex_unnamed_340 = vertex_unnamed_330 + vertex_uniform_buffer_1[4u].w;
				precise float vertex_unnamed_341 = (-0.0f) - vertex_unnamed_116;
				precise float vertex_unnamed_348 = vertex_unnamed_341 + vertex_uniform_buffer_1[5u].x;
				precise float vertex_unnamed_349 = vertex_unnamed_341 + vertex_uniform_buffer_1[5u].y;
				precise float vertex_unnamed_350 = vertex_unnamed_341 + vertex_uniform_buffer_1[5u].z;
				precise float vertex_unnamed_351 = vertex_unnamed_341 + vertex_uniform_buffer_1[5u].w;
				precise float vertex_unnamed_352 = vertex_unnamed_275 * vertex_unnamed_337;
				precise float vertex_unnamed_353 = vertex_unnamed_275 * vertex_unnamed_338;
				precise float vertex_unnamed_354 = vertex_unnamed_275 * vertex_unnamed_339;
				precise float vertex_unnamed_355 = vertex_unnamed_275 * vertex_unnamed_340;
				precise float vertex_unnamed_356 = vertex_unnamed_337 * vertex_unnamed_337;
				precise float vertex_unnamed_357 = vertex_unnamed_338 * vertex_unnamed_338;
				precise float vertex_unnamed_358 = vertex_unnamed_339 * vertex_unnamed_339;
				precise float vertex_unnamed_359 = vertex_unnamed_340 * vertex_unnamed_340;
				float vertex_unnamed_376 = max(mad(vertex_unnamed_348, vertex_unnamed_348, mad(vertex_unnamed_326, vertex_unnamed_326, vertex_unnamed_356)), 9.9999999747524270787835121154785e-07f);
				float vertex_unnamed_378 = max(mad(vertex_unnamed_349, vertex_unnamed_349, mad(vertex_unnamed_327, vertex_unnamed_327, vertex_unnamed_357)), 9.9999999747524270787835121154785e-07f);
				float vertex_unnamed_379 = max(mad(vertex_unnamed_350, vertex_unnamed_350, mad(vertex_unnamed_328, vertex_unnamed_328, vertex_unnamed_358)), 9.9999999747524270787835121154785e-07f);
				float vertex_unnamed_380 = max(mad(vertex_unnamed_351, vertex_unnamed_351, mad(vertex_unnamed_329, vertex_unnamed_329, vertex_unnamed_359)), 9.9999999747524270787835121154785e-07f);
				precise float vertex_unnamed_396 = 1.0f / mad(vertex_unnamed_376, vertex_uniform_buffer_1[6u].x, 1.0f);
				precise float vertex_unnamed_397 = 1.0f / mad(vertex_unnamed_378, vertex_uniform_buffer_1[6u].y, 1.0f);
				precise float vertex_unnamed_398 = 1.0f / mad(vertex_unnamed_379, vertex_uniform_buffer_1[6u].z, 1.0f);
				precise float vertex_unnamed_399 = 1.0f / mad(vertex_unnamed_380, vertex_uniform_buffer_1[6u].w, 1.0f);
				precise float vertex_unnamed_400 = mad(vertex_unnamed_348, vertex_unnamed_277, mad(vertex_unnamed_326, vertex_unnamed_274, vertex_unnamed_352)) * rsqrt(vertex_unnamed_376);
				precise float vertex_unnamed_401 = mad(vertex_unnamed_349, vertex_unnamed_277, mad(vertex_unnamed_327, vertex_unnamed_274, vertex_unnamed_353)) * rsqrt(vertex_unnamed_378);
				precise float vertex_unnamed_402 = mad(vertex_unnamed_350, vertex_unnamed_276, mad(vertex_unnamed_328, vertex_unnamed_274, vertex_unnamed_354)) * rsqrt(vertex_unnamed_379);
				precise float vertex_unnamed_403 = mad(vertex_unnamed_351, vertex_unnamed_277, mad(vertex_unnamed_329, vertex_unnamed_274, vertex_unnamed_355)) * rsqrt(vertex_unnamed_380);
				precise float vertex_unnamed_408 = vertex_unnamed_396 * max(vertex_unnamed_400, 0.0f);
				precise float vertex_unnamed_409 = vertex_unnamed_397 * max(vertex_unnamed_401, 0.0f);
				precise float vertex_unnamed_410 = vertex_unnamed_398 * max(vertex_unnamed_402, 0.0f);
				precise float vertex_unnamed_411 = vertex_unnamed_399 * max(vertex_unnamed_403, 0.0f);
				precise float vertex_unnamed_418 = vertex_unnamed_409 * vertex_uniform_buffer_1[8u].x;
				precise float vertex_unnamed_419 = vertex_unnamed_409 * vertex_uniform_buffer_1[8u].y;
				precise float vertex_unnamed_420 = vertex_unnamed_409 * vertex_uniform_buffer_1[8u].z;
				precise float vertex_unnamed_446 = vertex_unnamed_275 * vertex_unnamed_275;
				precise float vertex_unnamed_447 = (-0.0f) - vertex_unnamed_446;
				float vertex_unnamed_448 = mad(vertex_unnamed_274, vertex_unnamed_274, vertex_unnamed_447);
				precise float vertex_unnamed_449 = vertex_unnamed_275 * vertex_unnamed_274;
				precise float vertex_unnamed_450 = vertex_unnamed_277 * vertex_unnamed_275;
				precise float vertex_unnamed_451 = vertex_unnamed_276 * vertex_unnamed_276;
				precise float vertex_unnamed_452 = vertex_unnamed_274 * vertex_unnamed_277;
				precise float vertex_unnamed_492 = mad(vertex_uniform_buffer_1[10u].x, vertex_unnamed_411, mad(vertex_uniform_buffer_1[9u].x, vertex_unnamed_410, mad(vertex_uniform_buffer_1[7u].x, vertex_unnamed_408, vertex_unnamed_418))) + mad(vertex_uniform_buffer_1[45u].x, vertex_unnamed_448, dot(float4(vertex_uniform_buffer_1[42u]), float4(vertex_unnamed_449, vertex_unnamed_450, vertex_unnamed_451, vertex_unnamed_452)));
				precise float vertex_unnamed_493 = mad(vertex_uniform_buffer_1[10u].y, vertex_unnamed_411, mad(vertex_uniform_buffer_1[9u].y, vertex_unnamed_410, mad(vertex_uniform_buffer_1[7u].y, vertex_unnamed_408, vertex_unnamed_419))) + mad(vertex_uniform_buffer_1[45u].y, vertex_unnamed_448, dot(float4(vertex_uniform_buffer_1[43u]), float4(vertex_unnamed_449, vertex_unnamed_450, vertex_unnamed_451, vertex_unnamed_452)));
				precise float vertex_unnamed_494 = mad(vertex_uniform_buffer_1[10u].z, vertex_unnamed_411, mad(vertex_uniform_buffer_1[9u].z, vertex_unnamed_410, mad(vertex_uniform_buffer_1[7u].z, vertex_unnamed_408, vertex_unnamed_420))) + mad(vertex_uniform_buffer_1[45u].z, vertex_unnamed_448, dot(float4(vertex_uniform_buffer_1[44u]), float4(vertex_unnamed_449, vertex_unnamed_450, vertex_unnamed_451, vertex_unnamed_452)));
				vertex_output_7.x = vertex_unnamed_492;
				vertex_output_7.y = vertex_unnamed_493;
				vertex_output_7.z = vertex_unnamed_494;
				vertex_output_8.x = 0.0f;
				vertex_output_8.y = 0.0f;
				vertex_output_8.z = 0.0f;
				vertex_output_8.w = 0.0f;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[5] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[3] = float4(unity_4LightPosX0[0], unity_4LightPosX0[1], unity_4LightPosX0[2], unity_4LightPosX0[3]);

				vertex_uniform_buffer_1[4] = float4(unity_4LightPosY0[0], unity_4LightPosY0[1], unity_4LightPosY0[2], unity_4LightPosY0[3]);

				vertex_uniform_buffer_1[5] = float4(unity_4LightPosZ0[0], unity_4LightPosZ0[1], unity_4LightPosZ0[2], unity_4LightPosZ0[3]);

				vertex_uniform_buffer_1[6] = float4(unity_4LightAtten0[0], unity_4LightAtten0[1], unity_4LightAtten0[2], unity_4LightAtten0[3]);

				vertex_uniform_buffer_1[7] = float4(unity_LightColor[0][0], unity_LightColor[0][1], unity_LightColor[0][2], unity_LightColor[0][3]);
				vertex_uniform_buffer_1[8] = float4(unity_LightColor[1][0], unity_LightColor[1][1], unity_LightColor[1][2], unity_LightColor[1][3]);
				vertex_uniform_buffer_1[9] = float4(unity_LightColor[2][0], unity_LightColor[2][1], unity_LightColor[2][2], unity_LightColor[2][3]);
				vertex_uniform_buffer_1[10] = float4(unity_LightColor[3][0], unity_LightColor[3][1], unity_LightColor[3][2], unity_LightColor[3][3]);
				vertex_uniform_buffer_1[11] = float4(unity_LightColor[4][0], unity_LightColor[4][1], unity_LightColor[4][2], unity_LightColor[4][3]);
				vertex_uniform_buffer_1[12] = float4(unity_LightColor[5][0], unity_LightColor[5][1], unity_LightColor[5][2], unity_LightColor[5][3]);
				vertex_uniform_buffer_1[13] = float4(unity_LightColor[6][0], unity_LightColor[6][1], unity_LightColor[6][2], unity_LightColor[6][3]);
				vertex_uniform_buffer_1[14] = float4(unity_LightColor[7][0], unity_LightColor[7][1], unity_LightColor[7][2], unity_LightColor[7][3]);

				vertex_uniform_buffer_1[42] = float4(unity_SHBr[0], unity_SHBr[1], unity_SHBr[2], unity_SHBr[3]);

				vertex_uniform_buffer_1[43] = float4(unity_SHBg[0], unity_SHBg[1], unity_SHBg[2], unity_SHBg[3]);

				vertex_uniform_buffer_1[44] = float4(unity_SHBb[0], unity_SHBb[1], unity_SHBb[2], unity_SHBb[3]);

				vertex_uniform_buffer_1[45] = float4(unity_SHC[0], unity_SHC[1], unity_SHC[2], unity_SHC[3]);

				vertex_uniform_buffer_2[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_2[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_2[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_2[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_2[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_2[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_2[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_2[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_3[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_3[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_3[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_3[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // LIGHTPROBE_SH
			#endif // VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifdef LIGHTPROBE_SH
			#ifdef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 unity_4LightPosX0;
			float4 unity_4LightPosY0;
			float4 unity_4LightPosZ0;
			float4 unity_4LightAtten0;
			float4 unity_LightColor[4];
			float4 unity_SHBr;
			float4 unity_SHBg;
			float4 unity_SHBb;
			float4 unity_SHC;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			cbuffer t9288a1786c41494888d291e0244b11bc
			{
				float4 unity_LightColor[8];
			};

			static float4 gl_Position;
			static float4 vertex_input_0;
			static float vertex_output_1;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float4 vertex_input_1;
			static float4 vertex_output_2;
			static float3 vertex_input_2;
			static float4 vertex_output_3;
			static float4 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_input_4;
			static float4 vertex_output_7;
			static float3 vertex_output_6;
			static float4 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float vertex_output_1 : TEXCOORD6; // vs_TEXCOORD6
				float4 vertex_output_2 : TEXCOORD1; // vs_TEXCOORD1
				float4 vertex_output_3 : TEXCOORD2; // vs_TEXCOORD2
				float4 vertex_output_4 : TEXCOORD3; // vs_TEXCOORD3
				float4 vertex_output_5 : UNKNOWN5;
				float3 vertex_output_6 : TEXCOORD5; // vs_TEXCOORD5
				float4 vertex_output_7 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_8 : TEXCOORD8; // vs_TEXCOORD8
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_47;
			static float4 vertex_unnamed_65;
			static float vertex_unnamed_153;
			static float vertex_unnamed_203;
			static float4 vertex_unnamed_216;
			static float4 vertex_unnamed_301;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_47 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				float3 vertex_unnamed_62 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_62.x, vertex_unnamed_62.y, vertex_unnamed_62.z, vertex_unnamed_9.w);
				vertex_unnamed_65 = vertex_unnamed_47.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_65 = (unity_MatrixVP__array[0] * vertex_unnamed_47.xxxx) + vertex_unnamed_65;
				vertex_unnamed_65 = (unity_MatrixVP__array[2] * vertex_unnamed_47.zzzz) + vertex_unnamed_65;
				vertex_unnamed_47 = (unity_MatrixVP__array[3] * vertex_unnamed_47.wwww) + vertex_unnamed_65;
				gl_Position = vertex_unnamed_47;
				vertex_output_1 = vertex_unnamed_47.z;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				float3 vertex_unnamed_128 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_47 = float4(vertex_unnamed_128.x, vertex_unnamed_128.y, vertex_unnamed_128.z, vertex_unnamed_47.w);
				float3 vertex_unnamed_139 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_47.xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_139.x, vertex_unnamed_139.y, vertex_unnamed_139.z, vertex_unnamed_47.w);
				float3 vertex_unnamed_150 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_47.xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_150.x, vertex_unnamed_150.y, vertex_unnamed_150.z, vertex_unnamed_47.w);
				vertex_unnamed_153 = dot(vertex_unnamed_47.xyz, vertex_unnamed_47.xyz);
				vertex_unnamed_153 = rsqrt(vertex_unnamed_153);
				float3 vertex_unnamed_165 = vertex_unnamed_153.xxx * vertex_unnamed_47.xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_165.x, vertex_unnamed_165.y, vertex_unnamed_165.z, vertex_unnamed_47.w);
				vertex_output_2.x = vertex_unnamed_47.z;
				vertex_unnamed_153 = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_65.x = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_65.y = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_65.z = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_203 = dot(vertex_unnamed_65.xyz, vertex_unnamed_65.xyz);
				vertex_unnamed_203 = rsqrt(vertex_unnamed_203);
				vertex_unnamed_65 = vertex_unnamed_203.xxxx * vertex_unnamed_65.xyzz;
				float3 vertex_unnamed_221 = vertex_unnamed_47.xyz * vertex_unnamed_65.wxy;
				vertex_unnamed_216 = float4(vertex_unnamed_221.x, vertex_unnamed_221.y, vertex_unnamed_221.z, vertex_unnamed_216.w);
				float3 vertex_unnamed_232 = (vertex_unnamed_65.ywx * vertex_unnamed_47.yzx) + (-vertex_unnamed_216.xyz);
				vertex_unnamed_216 = float4(vertex_unnamed_232.x, vertex_unnamed_232.y, vertex_unnamed_232.z, vertex_unnamed_216.w);
				float3 vertex_unnamed_239 = vertex_unnamed_153.xxx * vertex_unnamed_216.xyz;
				vertex_unnamed_216 = float4(vertex_unnamed_239.x, vertex_unnamed_239.y, vertex_unnamed_239.z, vertex_unnamed_216.w);
				vertex_output_2.y = vertex_unnamed_216.x;
				vertex_output_2.w = vertex_unnamed_9.x;
				vertex_output_2.z = vertex_unnamed_65.x;
				vertex_output_3.x = vertex_unnamed_47.x;
				vertex_output_4.x = vertex_unnamed_47.y;
				vertex_output_3.y = vertex_unnamed_216.y;
				vertex_output_4.y = vertex_unnamed_216.z;
				vertex_output_3.w = vertex_unnamed_9.y;
				vertex_output_3.z = vertex_unnamed_65.y;
				vertex_output_4.w = vertex_unnamed_9.z;
				vertex_output_4.z = vertex_unnamed_65.w;
				vertex_output_5 = vertex_input_4;
				vertex_output_7 = 0.0f.xxxx;
				vertex_unnamed_47 = (-vertex_unnamed_9.xxxx) + unity_4LightPosX0;
				vertex_unnamed_216 = (-vertex_unnamed_9.yyyy) + unity_4LightPosY0;
				vertex_unnamed_9 = (-vertex_unnamed_9.zzzz) + unity_4LightPosZ0;
				vertex_unnamed_301 = vertex_unnamed_65.yyyy * vertex_unnamed_216;
				vertex_unnamed_216 *= vertex_unnamed_216;
				vertex_unnamed_216 = (vertex_unnamed_47 * vertex_unnamed_47) + vertex_unnamed_216;
				vertex_unnamed_47 = (vertex_unnamed_47 * vertex_unnamed_65.xxxx) + vertex_unnamed_301;
				vertex_unnamed_47 = (vertex_unnamed_9 * vertex_unnamed_65.wwzw) + vertex_unnamed_47;
				vertex_unnamed_9 = (vertex_unnamed_9 * vertex_unnamed_9) + vertex_unnamed_216;
				vertex_unnamed_9 = max(vertex_unnamed_9, 9.9999999747524270787835121154785e-07f.xxxx);
				vertex_unnamed_216 = rsqrt(vertex_unnamed_9);
				vertex_unnamed_9 = (vertex_unnamed_9 * unity_4LightAtten0) + 1.0f.xxxx;
				vertex_unnamed_9 = 1.0f.xxxx / vertex_unnamed_9;
				vertex_unnamed_47 *= vertex_unnamed_216;
				vertex_unnamed_47 = max(vertex_unnamed_47, 0.0f.xxxx);
				vertex_unnamed_9 *= vertex_unnamed_47;
				float3 vertex_unnamed_360 = vertex_unnamed_9.yyy * unity_LightColor[1].xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_360.x, vertex_unnamed_360.y, vertex_unnamed_360.z, vertex_unnamed_47.w);
				float3 vertex_unnamed_371 = (unity_LightColor[0].xyz * vertex_unnamed_9.xxx) + vertex_unnamed_47.xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_371.x, vertex_unnamed_371.y, vertex_unnamed_371.z, vertex_unnamed_47.w);
				float3 vertex_unnamed_382 = (unity_LightColor[2].xyz * vertex_unnamed_9.zzz) + vertex_unnamed_47.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_382.x, vertex_unnamed_382.y, vertex_unnamed_382.z, vertex_unnamed_9.w);
				float3 vertex_unnamed_393 = (unity_LightColor[3].xyz * vertex_unnamed_9.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_393.x, vertex_unnamed_393.y, vertex_unnamed_393.z, vertex_unnamed_9.w);
				vertex_unnamed_153 = vertex_unnamed_65.y * vertex_unnamed_65.y;
				vertex_unnamed_153 = (vertex_unnamed_65.x * vertex_unnamed_65.x) + (-vertex_unnamed_153);
				vertex_unnamed_47 = vertex_unnamed_65.ywzx * vertex_unnamed_65;
				vertex_unnamed_65.x = dot(unity_SHBr, vertex_unnamed_47);
				vertex_unnamed_65.y = dot(unity_SHBg, vertex_unnamed_47);
				vertex_unnamed_65.z = dot(unity_SHBb, vertex_unnamed_47);
				float3 vertex_unnamed_440 = (unity_SHC.xyz * vertex_unnamed_153.xxx) + vertex_unnamed_65.xyz;
				vertex_unnamed_47 = float4(vertex_unnamed_440.x, vertex_unnamed_440.y, vertex_unnamed_440.z, vertex_unnamed_47.w);
				vertex_output_6 = vertex_unnamed_9.xyz + vertex_unnamed_47.xyz;
				vertex_output_8 = 0.0f.xxxx;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_SHAr;
			float4 unity_SHAg;
			float4 unity_SHAb;
			float4 unity_FogColor;
			float4 unity_FogParams;
			float4 unity_SpecCube0_BoxMax;
			float4 unity_SpecCube0_BoxMin;
			float4 unity_SpecCube0_ProbePosition;
			float4 unity_SpecCube0_HDR;
			float4 unity_SpecCube1_BoxMax;
			float4 unity_SpecCube1_BoxMin;
			float4 unity_SpecCube1_ProbePosition;
			float4 unity_SpecCube1_HDR;
			float4 _LightColor0;
			float4 _Color;

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			TextureCube<float4> unity_SpecCube0;
			SamplerState samplerunity_SpecCube0;
			TextureCube<float4> unity_SpecCube1;

			static float4 fragment_input_2;
			static float4 fragment_input_3;
			static float4 fragment_input_4;
			static float2 fragment_input_0;
			static float4 fragment_input_5;
			static float4 fragment_output_0;
			static float3 fragment_input_6;
			static float fragment_input_1;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float fragment_input_1 : TEXCOORD6; // vs_TEXCOORD6
				float4 fragment_input_2 : TEXCOORD1; // vs_TEXCOORD1
				float4 fragment_input_3 : TEXCOORD2; // vs_TEXCOORD2
				float4 fragment_input_4 : TEXCOORD3; // vs_TEXCOORD3
				float4 fragment_input_5 : UNKNOWN5;
				float3 fragment_input_6 : TEXCOORD5; // vs_TEXCOORD5
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float3 fragment_unnamed_31;
			static float fragment_unnamed_43;
			static float3 fragment_unnamed_49;
			static float4 fragment_unnamed_55;
			static float4 fragment_unnamed_85;
			static float fragment_unnamed_112;
			static float4 fragment_unnamed_136;
			static float3 fragment_unnamed_189;
			static bool fragment_unnamed_219;
			static float4 fragment_unnamed_235;
			static float3 fragment_unnamed_243;
			static float3 fragment_unnamed_255;
			static bool3 fragment_unnamed_269;
			static bool fragment_unnamed_384;
			static float fragment_unnamed_399;
			static float3 fragment_unnamed_412;
			static float3 fragment_unnamed_423;
			static bool3 fragment_unnamed_434;
			static float3 fragment_unnamed_611;
			static float fragment_unnamed_623;
			static float3 fragment_unnamed_651;

			void frag_main()
			{
				fragment_unnamed_9.x = fragment_input_2.w;
				fragment_unnamed_9.y = fragment_input_3.w;
				fragment_unnamed_9.z = fragment_input_4.w;
				fragment_unnamed_31 = (-fragment_unnamed_9) + _WorldSpaceCameraPos;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_49 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_55 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_55 *= _Color;
				float3 fragment_unnamed_82 = fragment_unnamed_55.xyz * fragment_input_5.xyz;
				fragment_unnamed_55 = float4(fragment_unnamed_82.x, fragment_unnamed_82.y, fragment_unnamed_82.z, fragment_unnamed_55.w);
				float3 fragment_unnamed_93 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_85 = float4(fragment_unnamed_93.x, fragment_unnamed_93.y, fragment_unnamed_93.z, fragment_unnamed_85.w);
				fragment_unnamed_85.x = fragment_unnamed_85.z * fragment_unnamed_85.x;
				float2 fragment_unnamed_109 = (fragment_unnamed_85.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_85 = float4(fragment_unnamed_109.x, fragment_unnamed_109.y, fragment_unnamed_85.z, fragment_unnamed_85.w);
				fragment_unnamed_112 = dot(fragment_unnamed_85.xy, fragment_unnamed_85.xy);
				fragment_unnamed_112 = min(fragment_unnamed_112, 1.0f);
				fragment_unnamed_112 = (-fragment_unnamed_112) + 1.0f;
				fragment_unnamed_85.z = sqrt(fragment_unnamed_112);
				fragment_output_0.w = fragment_unnamed_55.w * fragment_input_5.w;
				fragment_unnamed_136.x = dot(fragment_input_2.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_136.y = dot(fragment_input_3.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_136.z = dot(fragment_input_4.xyz, fragment_unnamed_85.xyz);
				fragment_unnamed_112 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
				fragment_unnamed_112 = rsqrt(fragment_unnamed_112);
				float3 fragment_unnamed_166 = fragment_unnamed_112.xxx * fragment_unnamed_136.xyz;
				fragment_unnamed_85 = float4(fragment_unnamed_166.x, fragment_unnamed_166.y, fragment_unnamed_166.z, fragment_unnamed_85.w);
				fragment_unnamed_112 = dot(-fragment_unnamed_49, fragment_unnamed_85.xyz);
				fragment_unnamed_112 += fragment_unnamed_112;
				float3 fragment_unnamed_185 = (fragment_unnamed_85.xyz * (-fragment_unnamed_112.xxx)) + (-fragment_unnamed_49);
				fragment_unnamed_136 = float4(fragment_unnamed_185.x, fragment_unnamed_185.y, fragment_unnamed_185.z, fragment_unnamed_136.w);
				fragment_unnamed_85.w = 1.0f;
				fragment_unnamed_189.x = dot(unity_SHAr, fragment_unnamed_85);
				fragment_unnamed_189.y = dot(unity_SHAg, fragment_unnamed_85);
				fragment_unnamed_189.z = dot(unity_SHAb, fragment_unnamed_85);
				fragment_unnamed_189 += fragment_input_6;
				fragment_unnamed_189 = max(fragment_unnamed_189, 0.0f.xxx);
				fragment_unnamed_219 = 0.0f < unity_SpecCube0_ProbePosition.w;
				if (fragment_unnamed_219)
				{
					fragment_unnamed_112 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
					fragment_unnamed_112 = rsqrt(fragment_unnamed_112);
					float3 fragment_unnamed_240 = fragment_unnamed_112.xxx * fragment_unnamed_136.xyz;
					fragment_unnamed_235 = float4(fragment_unnamed_240.x, fragment_unnamed_240.y, fragment_unnamed_240.z, fragment_unnamed_235.w);
					fragment_unnamed_243 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMax.xyz;
					fragment_unnamed_243 /= fragment_unnamed_235.xyz;
					fragment_unnamed_255 = (-fragment_unnamed_9) + unity_SpecCube0_BoxMin.xyz;
					fragment_unnamed_255 /= fragment_unnamed_235.xyz;
					fragment_unnamed_269 = bool4(0.0f.xxxx.x < fragment_unnamed_235.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_235.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_235.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_235.xyzx.w).xyz;
					float3 fragment_unnamed_277 = fragment_unnamed_243;
					float fragment_unnamed_282;
					if (fragment_unnamed_269.x)
					{
						fragment_unnamed_282 = fragment_unnamed_243.x;
					}
					else
					{
						fragment_unnamed_282 = fragment_unnamed_255.x;
					}
					fragment_unnamed_277.x = fragment_unnamed_282;
					float fragment_unnamed_294;
					if (fragment_unnamed_269.y)
					{
						fragment_unnamed_294 = fragment_unnamed_243.y;
					}
					else
					{
						fragment_unnamed_294 = fragment_unnamed_255.y;
					}
					fragment_unnamed_277.y = fragment_unnamed_294;
					float fragment_unnamed_306;
					if (fragment_unnamed_269.z)
					{
						fragment_unnamed_306 = fragment_unnamed_243.z;
					}
					else
					{
						fragment_unnamed_306 = fragment_unnamed_255.z;
					}
					fragment_unnamed_277.z = fragment_unnamed_306;
					fragment_unnamed_243 = fragment_unnamed_277;
					fragment_unnamed_112 = min(fragment_unnamed_243.y, fragment_unnamed_243.x);
					fragment_unnamed_112 = min(fragment_unnamed_243.z, fragment_unnamed_112);
					fragment_unnamed_243 = fragment_unnamed_9 + (-unity_SpecCube0_ProbePosition.xyz);
					float3 fragment_unnamed_338 = (fragment_unnamed_235.xyz * fragment_unnamed_112.xxx) + fragment_unnamed_243;
					fragment_unnamed_235 = float4(fragment_unnamed_338.x, fragment_unnamed_338.y, fragment_unnamed_338.z, fragment_unnamed_235.w);
				}
				else
				{
					fragment_unnamed_235 = float4(fragment_unnamed_136.xyz.x, fragment_unnamed_136.xyz.y, fragment_unnamed_136.xyz.z, fragment_unnamed_235.w);
				}
				fragment_unnamed_235 = unity_SpecCube0.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_235.xyz, 6.0f);
				fragment_unnamed_112 = fragment_unnamed_235.w + (-1.0f);
				fragment_unnamed_112 = (unity_SpecCube0_HDR.w * fragment_unnamed_112) + 1.0f;
				fragment_unnamed_112 = log2(fragment_unnamed_112);
				fragment_unnamed_112 *= unity_SpecCube0_HDR.y;
				fragment_unnamed_112 = exp2(fragment_unnamed_112);
				fragment_unnamed_112 *= unity_SpecCube0_HDR.x;
				fragment_unnamed_243 = fragment_unnamed_235.xyz * fragment_unnamed_112.xxx;
				fragment_unnamed_384 = unity_SpecCube0_BoxMin.w < 0.999989986419677734375f;
				if (fragment_unnamed_384)
				{
					fragment_unnamed_384 = 0.0f < unity_SpecCube1_ProbePosition.w;
					if (fragment_unnamed_384)
					{
						fragment_unnamed_399 = dot(fragment_unnamed_136.xyz, fragment_unnamed_136.xyz);
						fragment_unnamed_399 = rsqrt(fragment_unnamed_399);
						fragment_unnamed_255 = fragment_unnamed_399.xxx * fragment_unnamed_136.xyz;
						fragment_unnamed_412 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMax.xyz;
						fragment_unnamed_412 /= fragment_unnamed_255;
						fragment_unnamed_423 = (-fragment_unnamed_9) + unity_SpecCube1_BoxMin.xyz;
						fragment_unnamed_423 /= fragment_unnamed_255;
						fragment_unnamed_434 = bool4(0.0f.xxxx.x < fragment_unnamed_255.xyzx.x, 0.0f.xxxx.y < fragment_unnamed_255.xyzx.y, 0.0f.xxxx.z < fragment_unnamed_255.xyzx.z, 0.0f.xxxx.w < fragment_unnamed_255.xyzx.w).xyz;
						float3 fragment_unnamed_439 = fragment_unnamed_412;
						float fragment_unnamed_443;
						if (fragment_unnamed_434.x)
						{
							fragment_unnamed_443 = fragment_unnamed_412.x;
						}
						else
						{
							fragment_unnamed_443 = fragment_unnamed_423.x;
						}
						fragment_unnamed_439.x = fragment_unnamed_443;
						float fragment_unnamed_455;
						if (fragment_unnamed_434.y)
						{
							fragment_unnamed_455 = fragment_unnamed_412.y;
						}
						else
						{
							fragment_unnamed_455 = fragment_unnamed_423.y;
						}
						fragment_unnamed_439.y = fragment_unnamed_455;
						float fragment_unnamed_467;
						if (fragment_unnamed_434.z)
						{
							fragment_unnamed_467 = fragment_unnamed_412.z;
						}
						else
						{
							fragment_unnamed_467 = fragment_unnamed_423.z;
						}
						fragment_unnamed_439.z = fragment_unnamed_467;
						fragment_unnamed_412 = fragment_unnamed_439;
						fragment_unnamed_399 = min(fragment_unnamed_412.y, fragment_unnamed_412.x);
						fragment_unnamed_399 = min(fragment_unnamed_412.z, fragment_unnamed_399);
						fragment_unnamed_9 += (-unity_SpecCube1_ProbePosition.xyz);
						float3 fragment_unnamed_498 = (fragment_unnamed_255 * fragment_unnamed_399.xxx) + fragment_unnamed_9;
						fragment_unnamed_136 = float4(fragment_unnamed_498.x, fragment_unnamed_498.y, fragment_unnamed_498.z, fragment_unnamed_136.w);
					}
					fragment_unnamed_136 = unity_SpecCube1.SampleLevel(samplerunity_SpecCube0, fragment_unnamed_136.xyz, 6.0f);
					fragment_unnamed_9.x = fragment_unnamed_136.w + (-1.0f);
					fragment_unnamed_9.x = (unity_SpecCube1_HDR.w * fragment_unnamed_9.x) + 1.0f;
					fragment_unnamed_9.x = log2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.y;
					fragment_unnamed_9.x = exp2(fragment_unnamed_9.x);
					fragment_unnamed_9.x *= unity_SpecCube1_HDR.x;
					fragment_unnamed_9 = fragment_unnamed_136.xyz * fragment_unnamed_9.xxx;
					float3 fragment_unnamed_552 = (fragment_unnamed_112.xxx * fragment_unnamed_235.xyz) + (-fragment_unnamed_9);
					fragment_unnamed_136 = float4(fragment_unnamed_552.x, fragment_unnamed_552.y, fragment_unnamed_552.z, fragment_unnamed_136.w);
					fragment_unnamed_243 = (unity_SpecCube0_BoxMin.www * fragment_unnamed_136.xyz) + fragment_unnamed_9;
				}
				fragment_unnamed_9 = fragment_unnamed_55.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_43 = dot(fragment_unnamed_31, fragment_unnamed_31);
				fragment_unnamed_43 = max(fragment_unnamed_43, 0.001000000047497451305389404296875f);
				fragment_unnamed_43 = rsqrt(fragment_unnamed_43);
				fragment_unnamed_31 = fragment_unnamed_43.xxx * fragment_unnamed_31;
				fragment_unnamed_43 = dot(fragment_unnamed_85.xyz, fragment_unnamed_49);
				fragment_unnamed_112 = dot(fragment_unnamed_85.xyz, _WorldSpaceLightPos0.xyz);
				fragment_unnamed_112 = clamp(fragment_unnamed_112, 0.0f, 1.0f);
				fragment_unnamed_31.x = dot(_WorldSpaceLightPos0.xyz, fragment_unnamed_31);
				fragment_unnamed_31.x = clamp(fragment_unnamed_31.x, 0.0f, 1.0f);
				fragment_unnamed_611.x = dot(fragment_unnamed_31.xx, fragment_unnamed_31.xx);
				fragment_unnamed_611.x += (-0.5f);
				fragment_unnamed_623 = (-fragment_unnamed_112) + 1.0f;
				fragment_unnamed_49.x = fragment_unnamed_623 * fragment_unnamed_623;
				fragment_unnamed_49.x *= fragment_unnamed_49.x;
				fragment_unnamed_623 *= fragment_unnamed_49.x;
				fragment_unnamed_623 = (fragment_unnamed_611.x * fragment_unnamed_623) + 1.0f;
				fragment_unnamed_49.x = (-abs(fragment_unnamed_43)) + 1.0f;
				fragment_unnamed_651.x = fragment_unnamed_49.x * fragment_unnamed_49.x;
				fragment_unnamed_651.x *= fragment_unnamed_651.x;
				fragment_unnamed_49.x *= fragment_unnamed_651.x;
				fragment_unnamed_611.x = (fragment_unnamed_611.x * fragment_unnamed_49.x) + 1.0f;
				fragment_unnamed_611.x *= fragment_unnamed_623;
				fragment_unnamed_611.x = fragment_unnamed_112 * fragment_unnamed_611.x;
				fragment_unnamed_43 = abs(fragment_unnamed_43) + fragment_unnamed_112;
				fragment_unnamed_43 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_43 = 0.5f / fragment_unnamed_43;
				fragment_unnamed_43 = fragment_unnamed_112 * fragment_unnamed_43;
				fragment_unnamed_43 *= 0.99999988079071044921875f;
				fragment_unnamed_611 = (_LightColor0.xyz * fragment_unnamed_611.xxx) + fragment_unnamed_189;
				fragment_unnamed_651 = fragment_unnamed_43.xxx * _LightColor0.xyz;
				fragment_unnamed_43 = (-fragment_unnamed_31.x) + 1.0f;
				fragment_unnamed_31.x = fragment_unnamed_43 * fragment_unnamed_43;
				fragment_unnamed_31.x *= fragment_unnamed_31.x;
				fragment_unnamed_43 *= fragment_unnamed_31.x;
				fragment_unnamed_43 = (fragment_unnamed_43 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				fragment_unnamed_651 = fragment_unnamed_43.xxx * fragment_unnamed_651;
				fragment_unnamed_9 = (fragment_unnamed_9 * fragment_unnamed_611) + fragment_unnamed_651;
				fragment_unnamed_31 = fragment_unnamed_243 * 0.5f.xxx;
				fragment_unnamed_43 = (fragment_unnamed_49.x * 2.2351741790771484375e-08f) + 0.039999999105930328369140625f;
				fragment_unnamed_9 = (fragment_unnamed_31 * fragment_unnamed_43.xxx) + fragment_unnamed_9;
				fragment_unnamed_43 = fragment_input_1 / _ProjectionParams.y;
				fragment_unnamed_43 = (-fragment_unnamed_43) + 1.0f;
				fragment_unnamed_43 *= _ProjectionParams.z;
				fragment_unnamed_43 = max(fragment_unnamed_43, 0.0f);
				fragment_unnamed_43 = (fragment_unnamed_43 * unity_FogParams.z) + unity_FogParams.w;
				fragment_unnamed_43 = clamp(fragment_unnamed_43, 0.0f, 1.0f);
				fragment_unnamed_9 += (-unity_FogColor.xyz);
				float3 fragment_unnamed_802 = (fragment_unnamed_43.xxx * fragment_unnamed_9) + unity_FogColor.xyz;
				fragment_output_0 = float4(fragment_unnamed_802.x, fragment_unnamed_802.y, fragment_unnamed_802.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_1 = stage_input.fragment_input_1;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // LIGHTPROBE_SH
			#endif // VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifndef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;
			float4 unity_SpecCube0_BoxMax;
			float4 unity_SpecCube0_BoxMin;
			float4 unity_SpecCube0_ProbePosition;
			float4 unity_SpecCube0_HDR;
			float4 unity_SpecCube1_BoxMax;
			float4 unity_SpecCube1_BoxMin;
			float4 unity_SpecCube1_ProbePosition;
			float4 unity_SpecCube1_HDR;

			static float4 fragment_uniform_buffer_0[5];
			static float4 fragment_uniform_buffer_1[5];
			static float4 fragment_uniform_buffer_2[1];
			static float4 fragment_uniform_buffer_3[8];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			TextureCube<float4> unity_SpecCube0;
			TextureCube<float4> unity_SpecCube1;
			SamplerState samplerunity_SpecCube0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float4 fragment_input_2;
			static float4 fragment_input_3;
			static float4 fragment_input_4;
			static float4 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float4 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float4 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float4 fragment_input_5 : COLOR; // COLOR
				float4 fragment_input_6 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_7 : TEXCOORD8; // TEXCOORD_8
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_68 = (-0.0f) - fragment_input_2.w;
				precise float fragment_unnamed_70 = (-0.0f) - fragment_input_3.w;
				precise float fragment_unnamed_71 = (-0.0f) - fragment_input_4.w;
				precise float fragment_unnamed_80 = fragment_unnamed_68 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_81 = fragment_unnamed_70 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_82 = fragment_unnamed_71 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_88 = rsqrt(dot(float3(fragment_unnamed_80, fragment_unnamed_81, fragment_unnamed_82), float3(fragment_unnamed_80, fragment_unnamed_81, fragment_unnamed_82)));
				precise float fragment_unnamed_89 = fragment_unnamed_88 * fragment_unnamed_80;
				precise float fragment_unnamed_90 = fragment_unnamed_88 * fragment_unnamed_81;
				precise float fragment_unnamed_91 = fragment_unnamed_88 * fragment_unnamed_82;
				float4 fragment_unnamed_99 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_111 = fragment_unnamed_99.x * fragment_uniform_buffer_0[4u].x;
				precise float fragment_unnamed_112 = fragment_unnamed_99.y * fragment_uniform_buffer_0[4u].y;
				precise float fragment_unnamed_113 = fragment_unnamed_99.z * fragment_uniform_buffer_0[4u].z;
				precise float fragment_unnamed_114 = fragment_unnamed_99.w * fragment_uniform_buffer_0[4u].w;
				precise float fragment_unnamed_122 = fragment_unnamed_111 * fragment_input_5.x;
				precise float fragment_unnamed_123 = fragment_unnamed_112 * fragment_input_5.y;
				precise float fragment_unnamed_124 = fragment_unnamed_113 * fragment_input_5.z;
				float4 fragment_unnamed_130 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_135 = fragment_unnamed_130.w * fragment_unnamed_130.x;
				float fragment_unnamed_136 = mad(fragment_unnamed_135, 2.0f, -1.0f);
				float fragment_unnamed_139 = mad(fragment_unnamed_130.y, 2.0f, -1.0f);
				precise float fragment_unnamed_145 = (-0.0f) - min(dot(float2(fragment_unnamed_136, fragment_unnamed_139), float2(fragment_unnamed_136, fragment_unnamed_139)), 1.0f);
				precise float fragment_unnamed_146 = fragment_unnamed_145 + 1.0f;
				float fragment_unnamed_147 = sqrt(fragment_unnamed_146);
				precise float fragment_unnamed_150 = fragment_unnamed_114 * fragment_input_5.w;
				fragment_output_0.w = fragment_unnamed_150;
				float fragment_unnamed_159 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_136, fragment_unnamed_139, fragment_unnamed_147));
				float fragment_unnamed_168 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_136, fragment_unnamed_139, fragment_unnamed_147));
				float fragment_unnamed_177 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_136, fragment_unnamed_139, fragment_unnamed_147));
				float fragment_unnamed_183 = rsqrt(dot(float3(fragment_unnamed_159, fragment_unnamed_168, fragment_unnamed_177), float3(fragment_unnamed_159, fragment_unnamed_168, fragment_unnamed_177)));
				precise float fragment_unnamed_184 = fragment_unnamed_183 * fragment_unnamed_159;
				precise float fragment_unnamed_185 = fragment_unnamed_183 * fragment_unnamed_168;
				precise float fragment_unnamed_186 = fragment_unnamed_183 * fragment_unnamed_177;
				precise float fragment_unnamed_187 = (-0.0f) - fragment_unnamed_89;
				precise float fragment_unnamed_188 = (-0.0f) - fragment_unnamed_90;
				precise float fragment_unnamed_189 = (-0.0f) - fragment_unnamed_91;
				float fragment_unnamed_190 = dot(float3(fragment_unnamed_187, fragment_unnamed_188, fragment_unnamed_189), float3(fragment_unnamed_184, fragment_unnamed_185, fragment_unnamed_186));
				precise float fragment_unnamed_193 = fragment_unnamed_190 + fragment_unnamed_190;
				precise float fragment_unnamed_194 = (-0.0f) - fragment_unnamed_193;
				precise float fragment_unnamed_195 = (-0.0f) - fragment_unnamed_89;
				precise float fragment_unnamed_196 = (-0.0f) - fragment_unnamed_90;
				precise float fragment_unnamed_197 = (-0.0f) - fragment_unnamed_91;
				float fragment_unnamed_198 = mad(fragment_unnamed_184, fragment_unnamed_194, fragment_unnamed_195);
				float fragment_unnamed_199 = mad(fragment_unnamed_185, fragment_unnamed_194, fragment_unnamed_196);
				float fragment_unnamed_200 = mad(fragment_unnamed_186, fragment_unnamed_194, fragment_unnamed_197);
				float fragment_unnamed_272;
				float fragment_unnamed_273;
				float fragment_unnamed_274;
				if (0.0f < fragment_uniform_buffer_3[2u].w)
				{
					float fragment_unnamed_209 = rsqrt(dot(float3(fragment_unnamed_198, fragment_unnamed_199, fragment_unnamed_200), float3(fragment_unnamed_198, fragment_unnamed_199, fragment_unnamed_200)));
					precise float fragment_unnamed_210 = fragment_unnamed_209 * fragment_unnamed_198;
					precise float fragment_unnamed_211 = fragment_unnamed_209 * fragment_unnamed_199;
					precise float fragment_unnamed_212 = fragment_unnamed_209 * fragment_unnamed_200;
					precise float fragment_unnamed_213 = (-0.0f) - fragment_input_2.w;
					precise float fragment_unnamed_214 = (-0.0f) - fragment_input_3.w;
					precise float fragment_unnamed_215 = (-0.0f) - fragment_input_4.w;
					precise float fragment_unnamed_221 = fragment_unnamed_213 + fragment_uniform_buffer_3[0u].x;
					precise float fragment_unnamed_222 = fragment_unnamed_214 + fragment_uniform_buffer_3[0u].y;
					precise float fragment_unnamed_223 = fragment_unnamed_215 + fragment_uniform_buffer_3[0u].z;
					precise float fragment_unnamed_224 = fragment_unnamed_221 / fragment_unnamed_210;
					precise float fragment_unnamed_225 = fragment_unnamed_222 / fragment_unnamed_211;
					precise float fragment_unnamed_226 = fragment_unnamed_223 / fragment_unnamed_212;
					precise float fragment_unnamed_227 = (-0.0f) - fragment_input_2.w;
					precise float fragment_unnamed_228 = (-0.0f) - fragment_input_3.w;
					precise float fragment_unnamed_229 = (-0.0f) - fragment_input_4.w;
					precise float fragment_unnamed_235 = fragment_unnamed_227 + fragment_uniform_buffer_3[1u].x;
					precise float fragment_unnamed_236 = fragment_unnamed_228 + fragment_uniform_buffer_3[1u].y;
					precise float fragment_unnamed_237 = fragment_unnamed_229 + fragment_uniform_buffer_3[1u].z;
					precise float fragment_unnamed_238 = fragment_unnamed_235 / fragment_unnamed_210;
					precise float fragment_unnamed_239 = fragment_unnamed_236 / fragment_unnamed_211;
					precise float fragment_unnamed_240 = fragment_unnamed_237 / fragment_unnamed_212;
					float fragment_unnamed_257 = min(asfloat((0.0f < fragment_unnamed_212) ? asuint(fragment_unnamed_226) : asuint(fragment_unnamed_240)), min(asfloat((0.0f < fragment_unnamed_211) ? asuint(fragment_unnamed_225) : asuint(fragment_unnamed_239)), asfloat((0.0f < fragment_unnamed_210) ? asuint(fragment_unnamed_224) : asuint(fragment_unnamed_238))));
					precise float fragment_unnamed_261 = (-0.0f) - fragment_uniform_buffer_3[2u].x;
					precise float fragment_unnamed_263 = (-0.0f) - fragment_uniform_buffer_3[2u].y;
					precise float fragment_unnamed_265 = (-0.0f) - fragment_uniform_buffer_3[2u].z;
					precise float fragment_unnamed_266 = fragment_input_2.w + fragment_unnamed_261;
					precise float fragment_unnamed_267 = fragment_input_3.w + fragment_unnamed_263;
					precise float fragment_unnamed_268 = fragment_input_4.w + fragment_unnamed_265;
					fragment_unnamed_272 = mad(fragment_unnamed_210, fragment_unnamed_257, fragment_unnamed_266);
					fragment_unnamed_273 = mad(fragment_unnamed_211, fragment_unnamed_257, fragment_unnamed_267);
					fragment_unnamed_274 = mad(fragment_unnamed_212, fragment_unnamed_257, fragment_unnamed_268);
				}
				else
				{
					fragment_unnamed_272 = fragment_unnamed_198;
					fragment_unnamed_273 = fragment_unnamed_199;
					fragment_unnamed_274 = fragment_unnamed_200;
				}
				float4 fragment_unnamed_278 = unity_SpecCube0.SampleLevel(samplerunity_SpecCube0, float3(fragment_unnamed_272, fragment_unnamed_273, fragment_unnamed_274), 6.0f);
				float fragment_unnamed_280 = fragment_unnamed_278.x;
				float fragment_unnamed_281 = fragment_unnamed_278.y;
				float fragment_unnamed_282 = fragment_unnamed_278.z;
				precise float fragment_unnamed_284 = fragment_unnamed_278.w + (-1.0f);
				precise float fragment_unnamed_293 = log2(mad(fragment_uniform_buffer_3[3u].w, fragment_unnamed_284, 1.0f)) * fragment_uniform_buffer_3[3u].y;
				precise float fragment_unnamed_298 = exp2(fragment_unnamed_293) * fragment_uniform_buffer_3[3u].x;
				precise float fragment_unnamed_299 = fragment_unnamed_280 * fragment_unnamed_298;
				precise float fragment_unnamed_300 = fragment_unnamed_281 * fragment_unnamed_298;
				precise float fragment_unnamed_301 = fragment_unnamed_282 * fragment_unnamed_298;
				float fragment_unnamed_312;
				float fragment_unnamed_314;
				float fragment_unnamed_316;
				if (fragment_uniform_buffer_3[1u].w < 0.999989986419677734375f)
				{
					float fragment_unnamed_495;
					float fragment_unnamed_496;
					float fragment_unnamed_497;
					if (0.0f < fragment_uniform_buffer_3[6u].w)
					{
						float fragment_unnamed_432 = rsqrt(dot(float3(fragment_unnamed_198, fragment_unnamed_199, fragment_unnamed_200), float3(fragment_unnamed_198, fragment_unnamed_199, fragment_unnamed_200)));
						precise float fragment_unnamed_433 = fragment_unnamed_432 * fragment_unnamed_198;
						precise float fragment_unnamed_434 = fragment_unnamed_432 * fragment_unnamed_199;
						precise float fragment_unnamed_435 = fragment_unnamed_432 * fragment_unnamed_200;
						precise float fragment_unnamed_436 = (-0.0f) - fragment_input_2.w;
						precise float fragment_unnamed_437 = (-0.0f) - fragment_input_3.w;
						precise float fragment_unnamed_438 = (-0.0f) - fragment_input_4.w;
						precise float fragment_unnamed_444 = fragment_unnamed_436 + fragment_uniform_buffer_3[4u].x;
						precise float fragment_unnamed_445 = fragment_unnamed_437 + fragment_uniform_buffer_3[4u].y;
						precise float fragment_unnamed_446 = fragment_unnamed_438 + fragment_uniform_buffer_3[4u].z;
						precise float fragment_unnamed_447 = fragment_unnamed_444 / fragment_unnamed_433;
						precise float fragment_unnamed_448 = fragment_unnamed_445 / fragment_unnamed_434;
						precise float fragment_unnamed_449 = fragment_unnamed_446 / fragment_unnamed_435;
						precise float fragment_unnamed_450 = (-0.0f) - fragment_input_2.w;
						precise float fragment_unnamed_451 = (-0.0f) - fragment_input_3.w;
						precise float fragment_unnamed_452 = (-0.0f) - fragment_input_4.w;
						precise float fragment_unnamed_458 = fragment_unnamed_450 + fragment_uniform_buffer_3[5u].x;
						precise float fragment_unnamed_459 = fragment_unnamed_451 + fragment_uniform_buffer_3[5u].y;
						precise float fragment_unnamed_460 = fragment_unnamed_452 + fragment_uniform_buffer_3[5u].z;
						precise float fragment_unnamed_461 = fragment_unnamed_458 / fragment_unnamed_433;
						precise float fragment_unnamed_462 = fragment_unnamed_459 / fragment_unnamed_434;
						precise float fragment_unnamed_463 = fragment_unnamed_460 / fragment_unnamed_435;
						float fragment_unnamed_480 = min(asfloat((0.0f < fragment_unnamed_435) ? asuint(fragment_unnamed_449) : asuint(fragment_unnamed_463)), min(asfloat((0.0f < fragment_unnamed_434) ? asuint(fragment_unnamed_448) : asuint(fragment_unnamed_462)), asfloat((0.0f < fragment_unnamed_433) ? asuint(fragment_unnamed_447) : asuint(fragment_unnamed_461))));
						precise float fragment_unnamed_484 = (-0.0f) - fragment_uniform_buffer_3[6u].x;
						precise float fragment_unnamed_486 = (-0.0f) - fragment_uniform_buffer_3[6u].y;
						precise float fragment_unnamed_488 = (-0.0f) - fragment_uniform_buffer_3[6u].z;
						precise float fragment_unnamed_489 = fragment_input_2.w + fragment_unnamed_484;
						precise float fragment_unnamed_490 = fragment_input_3.w + fragment_unnamed_486;
						precise float fragment_unnamed_491 = fragment_input_4.w + fragment_unnamed_488;
						fragment_unnamed_495 = mad(fragment_unnamed_433, fragment_unnamed_480, fragment_unnamed_489);
						fragment_unnamed_496 = mad(fragment_unnamed_434, fragment_unnamed_480, fragment_unnamed_490);
						fragment_unnamed_497 = mad(fragment_unnamed_435, fragment_unnamed_480, fragment_unnamed_491);
					}
					else
					{
						fragment_unnamed_495 = fragment_unnamed_198;
						fragment_unnamed_496 = fragment_unnamed_199;
						fragment_unnamed_497 = fragment_unnamed_200;
					}
					float4 fragment_unnamed_499 = unity_SpecCube1.SampleLevel(samplerunity_SpecCube0, float3(fragment_unnamed_495, fragment_unnamed_496, fragment_unnamed_497), 6.0f);
					precise float fragment_unnamed_505 = fragment_unnamed_499.w + (-1.0f);
					precise float fragment_unnamed_515 = log2(mad(fragment_uniform_buffer_3[7u].w, fragment_unnamed_505, 1.0f)) * fragment_uniform_buffer_3[7u].y;
					precise float fragment_unnamed_520 = exp2(fragment_unnamed_515) * fragment_uniform_buffer_3[7u].x;
					precise float fragment_unnamed_521 = fragment_unnamed_499.x * fragment_unnamed_520;
					precise float fragment_unnamed_522 = fragment_unnamed_499.y * fragment_unnamed_520;
					precise float fragment_unnamed_523 = fragment_unnamed_499.z * fragment_unnamed_520;
					precise float fragment_unnamed_524 = (-0.0f) - fragment_unnamed_521;
					precise float fragment_unnamed_525 = (-0.0f) - fragment_unnamed_522;
					precise float fragment_unnamed_526 = (-0.0f) - fragment_unnamed_523;
					fragment_unnamed_312 = mad(fragment_uniform_buffer_3[1u].w, mad(fragment_unnamed_298, fragment_unnamed_280, fragment_unnamed_524), fragment_unnamed_521);
					fragment_unnamed_314 = mad(fragment_uniform_buffer_3[1u].w, mad(fragment_unnamed_298, fragment_unnamed_281, fragment_unnamed_525), fragment_unnamed_522);
					fragment_unnamed_316 = mad(fragment_uniform_buffer_3[1u].w, mad(fragment_unnamed_298, fragment_unnamed_282, fragment_unnamed_526), fragment_unnamed_523);
				}
				else
				{
					fragment_unnamed_312 = fragment_unnamed_299;
					fragment_unnamed_314 = fragment_unnamed_300;
					fragment_unnamed_316 = fragment_unnamed_301;
				}
				precise float fragment_unnamed_318 = fragment_unnamed_122 * 0.959999978542327880859375f;
				precise float fragment_unnamed_320 = fragment_unnamed_123 * 0.959999978542327880859375f;
				precise float fragment_unnamed_321 = fragment_unnamed_124 * 0.959999978542327880859375f;
				float fragment_unnamed_327 = mad(fragment_unnamed_80, fragment_unnamed_88, fragment_uniform_buffer_2[0u].x);
				float fragment_unnamed_328 = mad(fragment_unnamed_81, fragment_unnamed_88, fragment_uniform_buffer_2[0u].y);
				float fragment_unnamed_329 = mad(fragment_unnamed_82, fragment_unnamed_88, fragment_uniform_buffer_2[0u].z);
				float fragment_unnamed_335 = rsqrt(max(dot(float3(fragment_unnamed_327, fragment_unnamed_328, fragment_unnamed_329), float3(fragment_unnamed_327, fragment_unnamed_328, fragment_unnamed_329)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_336 = fragment_unnamed_335 * fragment_unnamed_327;
				precise float fragment_unnamed_337 = fragment_unnamed_335 * fragment_unnamed_328;
				precise float fragment_unnamed_338 = fragment_unnamed_335 * fragment_unnamed_329;
				float fragment_unnamed_339 = dot(float3(fragment_unnamed_184, fragment_unnamed_185, fragment_unnamed_186), float3(fragment_unnamed_89, fragment_unnamed_90, fragment_unnamed_91));
				float fragment_unnamed_350 = clamp(dot(float3(fragment_unnamed_184, fragment_unnamed_185, fragment_unnamed_186), float3(fragment_uniform_buffer_2[0u].xyz)), 0.0f, 1.0f);
				float fragment_unnamed_359 = clamp(dot(float3(fragment_uniform_buffer_2[0u].xyz), float3(fragment_unnamed_336, fragment_unnamed_337, fragment_unnamed_338)), 0.0f, 1.0f);
				precise float fragment_unnamed_363 = dot(fragment_unnamed_359.xx, fragment_unnamed_359.xx) + (-0.5f);
				precise float fragment_unnamed_365 = (-0.0f) - fragment_unnamed_350;
				precise float fragment_unnamed_366 = fragment_unnamed_365 + 1.0f;
				precise float fragment_unnamed_367 = fragment_unnamed_366 * fragment_unnamed_366;
				precise float fragment_unnamed_368 = fragment_unnamed_367 * fragment_unnamed_367;
				precise float fragment_unnamed_369 = fragment_unnamed_366 * fragment_unnamed_368;
				precise float fragment_unnamed_372 = (-0.0f) - abs(fragment_unnamed_339);
				precise float fragment_unnamed_373 = fragment_unnamed_372 + 1.0f;
				precise float fragment_unnamed_374 = fragment_unnamed_373 * fragment_unnamed_373;
				precise float fragment_unnamed_375 = fragment_unnamed_374 * fragment_unnamed_374;
				precise float fragment_unnamed_376 = fragment_unnamed_373 * fragment_unnamed_375;
				precise float fragment_unnamed_378 = mad(fragment_unnamed_363, fragment_unnamed_376, 1.0f) * mad(fragment_unnamed_363, fragment_unnamed_369, 1.0f);
				precise float fragment_unnamed_379 = fragment_unnamed_350 * fragment_unnamed_378;
				precise float fragment_unnamed_381 = abs(fragment_unnamed_339) + fragment_unnamed_350;
				precise float fragment_unnamed_382 = fragment_unnamed_381 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_384 = 0.5f / fragment_unnamed_382;
				precise float fragment_unnamed_386 = fragment_unnamed_384 * 0.99999988079071044921875f;
				precise float fragment_unnamed_388 = fragment_unnamed_350 * fragment_unnamed_386;
				precise float fragment_unnamed_394 = fragment_unnamed_379 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_395 = fragment_unnamed_379 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_396 = fragment_unnamed_379 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_402 = fragment_unnamed_388 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_403 = fragment_unnamed_388 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_404 = fragment_unnamed_388 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_405 = (-0.0f) - fragment_unnamed_359;
				precise float fragment_unnamed_406 = fragment_unnamed_405 + 1.0f;
				precise float fragment_unnamed_407 = fragment_unnamed_406 * fragment_unnamed_406;
				precise float fragment_unnamed_408 = fragment_unnamed_407 * fragment_unnamed_407;
				precise float fragment_unnamed_409 = fragment_unnamed_406 * fragment_unnamed_408;
				float fragment_unnamed_410 = mad(fragment_unnamed_409, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_412 = fragment_unnamed_410 * fragment_unnamed_402;
				precise float fragment_unnamed_413 = fragment_unnamed_410 * fragment_unnamed_403;
				precise float fragment_unnamed_414 = fragment_unnamed_410 * fragment_unnamed_404;
				precise float fragment_unnamed_418 = fragment_unnamed_312 * 0.5f;
				precise float fragment_unnamed_419 = fragment_unnamed_314 * 0.5f;
				precise float fragment_unnamed_420 = fragment_unnamed_316 * 0.5f;
				float fragment_unnamed_421 = mad(fragment_unnamed_376, 2.2351741790771484375e-08f, 0.039999999105930328369140625f);
				fragment_output_0.x = mad(fragment_unnamed_418, fragment_unnamed_421, mad(fragment_unnamed_318, fragment_unnamed_394, fragment_unnamed_412));
				fragment_output_0.y = mad(fragment_unnamed_419, fragment_unnamed_421, mad(fragment_unnamed_320, fragment_unnamed_395, fragment_unnamed_413));
				fragment_output_0.z = mad(fragment_unnamed_420, fragment_unnamed_421, mad(fragment_unnamed_321, fragment_unnamed_396, fragment_unnamed_414));
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_uniform_buffer_3[0] = float4(unity_SpecCube0_BoxMax[0], unity_SpecCube0_BoxMax[1], unity_SpecCube0_BoxMax[2], unity_SpecCube0_BoxMax[3]);

				fragment_uniform_buffer_3[1] = float4(unity_SpecCube0_BoxMin[0], unity_SpecCube0_BoxMin[1], unity_SpecCube0_BoxMin[2], unity_SpecCube0_BoxMin[3]);

				fragment_uniform_buffer_3[2] = float4(unity_SpecCube0_ProbePosition[0], unity_SpecCube0_ProbePosition[1], unity_SpecCube0_ProbePosition[2], unity_SpecCube0_ProbePosition[3]);

				fragment_uniform_buffer_3[3] = float4(unity_SpecCube0_HDR[0], unity_SpecCube0_HDR[1], unity_SpecCube0_HDR[2], unity_SpecCube0_HDR[3]);

				fragment_uniform_buffer_3[4] = float4(unity_SpecCube1_BoxMax[0], unity_SpecCube1_BoxMax[1], unity_SpecCube1_BoxMax[2], unity_SpecCube1_BoxMax[3]);

				fragment_uniform_buffer_3[5] = float4(unity_SpecCube1_BoxMin[0], unity_SpecCube1_BoxMin[1], unity_SpecCube1_BoxMin[2], unity_SpecCube1_BoxMin[3]);

				fragment_uniform_buffer_3[6] = float4(unity_SpecCube1_ProbePosition[0], unity_SpecCube1_ProbePosition[1], unity_SpecCube1_ProbePosition[2], unity_SpecCube1_ProbePosition[3]);

				fragment_uniform_buffer_3[7] = float4(unity_SpecCube1_HDR[0], unity_SpecCube1_HDR[1], unity_SpecCube1_HDR[2], unity_SpecCube1_HDR[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // !FOG_LINEAR
			#endif // !LIGHTPROBE_SH
			#endif // !VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifdef LIGHTPROBE_SH
			#ifndef FOG_LINEAR
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;
			float4 unity_SHAr;
			float4 unity_SHAg;
			float4 unity_SHAb;
			float4 unity_SpecCube0_BoxMax;
			float4 unity_SpecCube0_BoxMin;
			float4 unity_SpecCube0_ProbePosition;
			float4 unity_SpecCube0_HDR;
			float4 unity_SpecCube1_BoxMax;
			float4 unity_SpecCube1_BoxMin;
			float4 unity_SpecCube1_ProbePosition;
			float4 unity_SpecCube1_HDR;

			static float4 fragment_uniform_buffer_0[5];
			static float4 fragment_uniform_buffer_1[5];
			static float4 fragment_uniform_buffer_2[42];
			static float4 fragment_uniform_buffer_3[8];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			TextureCube<float4> unity_SpecCube0;
			TextureCube<float4> unity_SpecCube1;
			SamplerState samplerunity_SpecCube0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float4 fragment_input_2;
			static float4 fragment_input_3;
			static float4 fragment_input_4;
			static float4 fragment_input_5;
			static float4 fragment_input_6;
			static float3 fragment_input_7;
			static float4 fragment_input_8;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float4 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float4 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float4 fragment_input_5 : COLOR; // COLOR
				float4 fragment_input_6 : TEXCOORD4; // TEXCOORD_4
				float3 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
				float4 fragment_input_8 : TEXCOORD8; // TEXCOORD_8
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_71 = (-0.0f) - fragment_input_2.w;
				precise float fragment_unnamed_73 = (-0.0f) - fragment_input_3.w;
				precise float fragment_unnamed_74 = (-0.0f) - fragment_input_4.w;
				precise float fragment_unnamed_83 = fragment_unnamed_71 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_84 = fragment_unnamed_73 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_85 = fragment_unnamed_74 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_90 = rsqrt(dot(float3(fragment_unnamed_83, fragment_unnamed_84, fragment_unnamed_85), float3(fragment_unnamed_83, fragment_unnamed_84, fragment_unnamed_85)));
				precise float fragment_unnamed_91 = fragment_unnamed_90 * fragment_unnamed_83;
				precise float fragment_unnamed_92 = fragment_unnamed_90 * fragment_unnamed_84;
				precise float fragment_unnamed_93 = fragment_unnamed_90 * fragment_unnamed_85;
				float4 fragment_unnamed_102 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_114 = fragment_unnamed_102.x * fragment_uniform_buffer_0[4u].x;
				precise float fragment_unnamed_115 = fragment_unnamed_102.y * fragment_uniform_buffer_0[4u].y;
				precise float fragment_unnamed_116 = fragment_unnamed_102.z * fragment_uniform_buffer_0[4u].z;
				precise float fragment_unnamed_117 = fragment_unnamed_102.w * fragment_uniform_buffer_0[4u].w;
				precise float fragment_unnamed_125 = fragment_unnamed_114 * fragment_input_5.x;
				precise float fragment_unnamed_126 = fragment_unnamed_115 * fragment_input_5.y;
				precise float fragment_unnamed_127 = fragment_unnamed_116 * fragment_input_5.z;
				float4 fragment_unnamed_133 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_138 = fragment_unnamed_133.w * fragment_unnamed_133.x;
				float fragment_unnamed_139 = mad(fragment_unnamed_138, 2.0f, -1.0f);
				float fragment_unnamed_142 = mad(fragment_unnamed_133.y, 2.0f, -1.0f);
				precise float fragment_unnamed_148 = (-0.0f) - min(dot(float2(fragment_unnamed_139, fragment_unnamed_142), float2(fragment_unnamed_139, fragment_unnamed_142)), 1.0f);
				precise float fragment_unnamed_149 = fragment_unnamed_148 + 1.0f;
				float fragment_unnamed_150 = sqrt(fragment_unnamed_149);
				precise float fragment_unnamed_153 = fragment_unnamed_117 * fragment_input_5.w;
				fragment_output_0.w = fragment_unnamed_153;
				float fragment_unnamed_162 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_139, fragment_unnamed_142, fragment_unnamed_150));
				float fragment_unnamed_171 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_139, fragment_unnamed_142, fragment_unnamed_150));
				float fragment_unnamed_180 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_139, fragment_unnamed_142, fragment_unnamed_150));
				float fragment_unnamed_186 = rsqrt(dot(float3(fragment_unnamed_162, fragment_unnamed_171, fragment_unnamed_180), float3(fragment_unnamed_162, fragment_unnamed_171, fragment_unnamed_180)));
				precise float fragment_unnamed_187 = fragment_unnamed_186 * fragment_unnamed_162;
				precise float fragment_unnamed_188 = fragment_unnamed_186 * fragment_unnamed_171;
				precise float fragment_unnamed_189 = fragment_unnamed_186 * fragment_unnamed_180;
				precise float fragment_unnamed_190 = (-0.0f) - fragment_unnamed_91;
				precise float fragment_unnamed_191 = (-0.0f) - fragment_unnamed_92;
				precise float fragment_unnamed_192 = (-0.0f) - fragment_unnamed_93;
				float fragment_unnamed_193 = dot(float3(fragment_unnamed_190, fragment_unnamed_191, fragment_unnamed_192), float3(fragment_unnamed_187, fragment_unnamed_188, fragment_unnamed_189));
				precise float fragment_unnamed_196 = fragment_unnamed_193 + fragment_unnamed_193;
				precise float fragment_unnamed_197 = (-0.0f) - fragment_unnamed_196;
				precise float fragment_unnamed_198 = (-0.0f) - fragment_unnamed_91;
				precise float fragment_unnamed_199 = (-0.0f) - fragment_unnamed_92;
				precise float fragment_unnamed_200 = (-0.0f) - fragment_unnamed_93;
				float fragment_unnamed_201 = mad(fragment_unnamed_187, fragment_unnamed_197, fragment_unnamed_198);
				float fragment_unnamed_202 = mad(fragment_unnamed_188, fragment_unnamed_197, fragment_unnamed_199);
				float fragment_unnamed_203 = mad(fragment_unnamed_189, fragment_unnamed_197, fragment_unnamed_200);
				float fragment_unnamed_204 = asfloat(1065353216u);
				precise float fragment_unnamed_242 = dot(float4(fragment_uniform_buffer_2[39u]), float4(fragment_unnamed_187, fragment_unnamed_188, fragment_unnamed_189, fragment_unnamed_204)) + fragment_input_7.x;
				precise float fragment_unnamed_243 = dot(float4(fragment_uniform_buffer_2[40u]), float4(fragment_unnamed_187, fragment_unnamed_188, fragment_unnamed_189, fragment_unnamed_204)) + fragment_input_7.y;
				precise float fragment_unnamed_244 = dot(float4(fragment_uniform_buffer_2[41u]), float4(fragment_unnamed_187, fragment_unnamed_188, fragment_unnamed_189, fragment_unnamed_204)) + fragment_input_7.z;
				float fragment_unnamed_319;
				float fragment_unnamed_320;
				float fragment_unnamed_321;
				if (0.0f < fragment_uniform_buffer_3[2u].w)
				{
					float fragment_unnamed_256 = rsqrt(dot(float3(fragment_unnamed_201, fragment_unnamed_202, fragment_unnamed_203), float3(fragment_unnamed_201, fragment_unnamed_202, fragment_unnamed_203)));
					precise float fragment_unnamed_257 = fragment_unnamed_256 * fragment_unnamed_201;
					precise float fragment_unnamed_258 = fragment_unnamed_256 * fragment_unnamed_202;
					precise float fragment_unnamed_259 = fragment_unnamed_256 * fragment_unnamed_203;
					precise float fragment_unnamed_260 = (-0.0f) - fragment_input_2.w;
					precise float fragment_unnamed_261 = (-0.0f) - fragment_input_3.w;
					precise float fragment_unnamed_262 = (-0.0f) - fragment_input_4.w;
					precise float fragment_unnamed_268 = fragment_unnamed_260 + fragment_uniform_buffer_3[0u].x;
					precise float fragment_unnamed_269 = fragment_unnamed_261 + fragment_uniform_buffer_3[0u].y;
					precise float fragment_unnamed_270 = fragment_unnamed_262 + fragment_uniform_buffer_3[0u].z;
					precise float fragment_unnamed_271 = fragment_unnamed_268 / fragment_unnamed_257;
					precise float fragment_unnamed_272 = fragment_unnamed_269 / fragment_unnamed_258;
					precise float fragment_unnamed_273 = fragment_unnamed_270 / fragment_unnamed_259;
					precise float fragment_unnamed_274 = (-0.0f) - fragment_input_2.w;
					precise float fragment_unnamed_275 = (-0.0f) - fragment_input_3.w;
					precise float fragment_unnamed_276 = (-0.0f) - fragment_input_4.w;
					precise float fragment_unnamed_282 = fragment_unnamed_274 + fragment_uniform_buffer_3[1u].x;
					precise float fragment_unnamed_283 = fragment_unnamed_275 + fragment_uniform_buffer_3[1u].y;
					precise float fragment_unnamed_284 = fragment_unnamed_276 + fragment_uniform_buffer_3[1u].z;
					precise float fragment_unnamed_285 = fragment_unnamed_282 / fragment_unnamed_257;
					precise float fragment_unnamed_286 = fragment_unnamed_283 / fragment_unnamed_258;
					precise float fragment_unnamed_287 = fragment_unnamed_284 / fragment_unnamed_259;
					float fragment_unnamed_304 = min(asfloat((0.0f < fragment_unnamed_259) ? asuint(fragment_unnamed_273) : asuint(fragment_unnamed_287)), min(asfloat((0.0f < fragment_unnamed_258) ? asuint(fragment_unnamed_272) : asuint(fragment_unnamed_286)), asfloat((0.0f < fragment_unnamed_257) ? asuint(fragment_unnamed_271) : asuint(fragment_unnamed_285))));
					precise float fragment_unnamed_308 = (-0.0f) - fragment_uniform_buffer_3[2u].x;
					precise float fragment_unnamed_310 = (-0.0f) - fragment_uniform_buffer_3[2u].y;
					precise float fragment_unnamed_312 = (-0.0f) - fragment_uniform_buffer_3[2u].z;
					precise float fragment_unnamed_313 = fragment_input_2.w + fragment_unnamed_308;
					precise float fragment_unnamed_314 = fragment_input_3.w + fragment_unnamed_310;
					precise float fragment_unnamed_315 = fragment_input_4.w + fragment_unnamed_312;
					fragment_unnamed_319 = mad(fragment_unnamed_257, fragment_unnamed_304, fragment_unnamed_313);
					fragment_unnamed_320 = mad(fragment_unnamed_258, fragment_unnamed_304, fragment_unnamed_314);
					fragment_unnamed_321 = mad(fragment_unnamed_259, fragment_unnamed_304, fragment_unnamed_315);
				}
				else
				{
					fragment_unnamed_319 = fragment_unnamed_201;
					fragment_unnamed_320 = fragment_unnamed_202;
					fragment_unnamed_321 = fragment_unnamed_203;
				}
				float4 fragment_unnamed_325 = unity_SpecCube0.SampleLevel(samplerunity_SpecCube0, float3(fragment_unnamed_319, fragment_unnamed_320, fragment_unnamed_321), 6.0f);
				float fragment_unnamed_327 = fragment_unnamed_325.x;
				float fragment_unnamed_328 = fragment_unnamed_325.y;
				float fragment_unnamed_329 = fragment_unnamed_325.z;
				precise float fragment_unnamed_331 = fragment_unnamed_325.w + (-1.0f);
				precise float fragment_unnamed_340 = log2(mad(fragment_uniform_buffer_3[3u].w, fragment_unnamed_331, 1.0f)) * fragment_uniform_buffer_3[3u].y;
				precise float fragment_unnamed_345 = exp2(fragment_unnamed_340) * fragment_uniform_buffer_3[3u].x;
				precise float fragment_unnamed_346 = fragment_unnamed_327 * fragment_unnamed_345;
				precise float fragment_unnamed_347 = fragment_unnamed_328 * fragment_unnamed_345;
				precise float fragment_unnamed_348 = fragment_unnamed_329 * fragment_unnamed_345;
				float fragment_unnamed_359;
				float fragment_unnamed_361;
				float fragment_unnamed_363;
				if (fragment_uniform_buffer_3[1u].w < 0.999989986419677734375f)
				{
					float fragment_unnamed_542;
					float fragment_unnamed_543;
					float fragment_unnamed_544;
					if (0.0f < fragment_uniform_buffer_3[6u].w)
					{
						float fragment_unnamed_479 = rsqrt(dot(float3(fragment_unnamed_201, fragment_unnamed_202, fragment_unnamed_203), float3(fragment_unnamed_201, fragment_unnamed_202, fragment_unnamed_203)));
						precise float fragment_unnamed_480 = fragment_unnamed_479 * fragment_unnamed_201;
						precise float fragment_unnamed_481 = fragment_unnamed_479 * fragment_unnamed_202;
						precise float fragment_unnamed_482 = fragment_unnamed_479 * fragment_unnamed_203;
						precise float fragment_unnamed_483 = (-0.0f) - fragment_input_2.w;
						precise float fragment_unnamed_484 = (-0.0f) - fragment_input_3.w;
						precise float fragment_unnamed_485 = (-0.0f) - fragment_input_4.w;
						precise float fragment_unnamed_491 = fragment_unnamed_483 + fragment_uniform_buffer_3[4u].x;
						precise float fragment_unnamed_492 = fragment_unnamed_484 + fragment_uniform_buffer_3[4u].y;
						precise float fragment_unnamed_493 = fragment_unnamed_485 + fragment_uniform_buffer_3[4u].z;
						precise float fragment_unnamed_494 = fragment_unnamed_491 / fragment_unnamed_480;
						precise float fragment_unnamed_495 = fragment_unnamed_492 / fragment_unnamed_481;
						precise float fragment_unnamed_496 = fragment_unnamed_493 / fragment_unnamed_482;
						precise float fragment_unnamed_497 = (-0.0f) - fragment_input_2.w;
						precise float fragment_unnamed_498 = (-0.0f) - fragment_input_3.w;
						precise float fragment_unnamed_499 = (-0.0f) - fragment_input_4.w;
						precise float fragment_unnamed_505 = fragment_unnamed_497 + fragment_uniform_buffer_3[5u].x;
						precise float fragment_unnamed_506 = fragment_unnamed_498 + fragment_uniform_buffer_3[5u].y;
						precise float fragment_unnamed_507 = fragment_unnamed_499 + fragment_uniform_buffer_3[5u].z;
						precise float fragment_unnamed_508 = fragment_unnamed_505 / fragment_unnamed_480;
						precise float fragment_unnamed_509 = fragment_unnamed_506 / fragment_unnamed_481;
						precise float fragment_unnamed_510 = fragment_unnamed_507 / fragment_unnamed_482;
						float fragment_unnamed_527 = min(asfloat((0.0f < fragment_unnamed_482) ? asuint(fragment_unnamed_496) : asuint(fragment_unnamed_510)), min(asfloat((0.0f < fragment_unnamed_481) ? asuint(fragment_unnamed_495) : asuint(fragment_unnamed_509)), asfloat((0.0f < fragment_unnamed_480) ? asuint(fragment_unnamed_494) : asuint(fragment_unnamed_508))));
						precise float fragment_unnamed_531 = (-0.0f) - fragment_uniform_buffer_3[6u].x;
						precise float fragment_unnamed_533 = (-0.0f) - fragment_uniform_buffer_3[6u].y;
						precise float fragment_unnamed_535 = (-0.0f) - fragment_uniform_buffer_3[6u].z;
						precise float fragment_unnamed_536 = fragment_input_2.w + fragment_unnamed_531;
						precise float fragment_unnamed_537 = fragment_input_3.w + fragment_unnamed_533;
						precise float fragment_unnamed_538 = fragment_input_4.w + fragment_unnamed_535;
						fragment_unnamed_542 = mad(fragment_unnamed_480, fragment_unnamed_527, fragment_unnamed_536);
						fragment_unnamed_543 = mad(fragment_unnamed_481, fragment_unnamed_527, fragment_unnamed_537);
						fragment_unnamed_544 = mad(fragment_unnamed_482, fragment_unnamed_527, fragment_unnamed_538);
					}
					else
					{
						fragment_unnamed_542 = fragment_unnamed_201;
						fragment_unnamed_543 = fragment_unnamed_202;
						fragment_unnamed_544 = fragment_unnamed_203;
					}
					float4 fragment_unnamed_546 = unity_SpecCube1.SampleLevel(samplerunity_SpecCube0, float3(fragment_unnamed_542, fragment_unnamed_543, fragment_unnamed_544), 6.0f);
					precise float fragment_unnamed_552 = fragment_unnamed_546.w + (-1.0f);
					precise float fragment_unnamed_562 = log2(mad(fragment_uniform_buffer_3[7u].w, fragment_unnamed_552, 1.0f)) * fragment_uniform_buffer_3[7u].y;
					precise float fragment_unnamed_567 = exp2(fragment_unnamed_562) * fragment_uniform_buffer_3[7u].x;
					precise float fragment_unnamed_568 = fragment_unnamed_546.x * fragment_unnamed_567;
					precise float fragment_unnamed_569 = fragment_unnamed_546.y * fragment_unnamed_567;
					precise float fragment_unnamed_570 = fragment_unnamed_546.z * fragment_unnamed_567;
					precise float fragment_unnamed_571 = (-0.0f) - fragment_unnamed_568;
					precise float fragment_unnamed_572 = (-0.0f) - fragment_unnamed_569;
					precise float fragment_unnamed_573 = (-0.0f) - fragment_unnamed_570;
					fragment_unnamed_359 = mad(fragment_uniform_buffer_3[1u].w, mad(fragment_unnamed_345, fragment_unnamed_327, fragment_unnamed_571), fragment_unnamed_568);
					fragment_unnamed_361 = mad(fragment_uniform_buffer_3[1u].w, mad(fragment_unnamed_345, fragment_unnamed_328, fragment_unnamed_572), fragment_unnamed_569);
					fragment_unnamed_363 = mad(fragment_uniform_buffer_3[1u].w, mad(fragment_unnamed_345, fragment_unnamed_329, fragment_unnamed_573), fragment_unnamed_570);
				}
				else
				{
					fragment_unnamed_359 = fragment_unnamed_346;
					fragment_unnamed_361 = fragment_unnamed_347;
					fragment_unnamed_363 = fragment_unnamed_348;
				}
				precise float fragment_unnamed_365 = fragment_unnamed_125 * 0.959999978542327880859375f;
				precise float fragment_unnamed_367 = fragment_unnamed_126 * 0.959999978542327880859375f;
				precise float fragment_unnamed_368 = fragment_unnamed_127 * 0.959999978542327880859375f;
				float fragment_unnamed_374 = mad(fragment_unnamed_83, fragment_unnamed_90, fragment_uniform_buffer_2[0u].x);
				float fragment_unnamed_375 = mad(fragment_unnamed_84, fragment_unnamed_90, fragment_uniform_buffer_2[0u].y);
				float fragment_unnamed_376 = mad(fragment_unnamed_85, fragment_unnamed_90, fragment_uniform_buffer_2[0u].z);
				float fragment_unnamed_382 = rsqrt(max(dot(float3(fragment_unnamed_374, fragment_unnamed_375, fragment_unnamed_376), float3(fragment_unnamed_374, fragment_unnamed_375, fragment_unnamed_376)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_383 = fragment_unnamed_382 * fragment_unnamed_374;
				precise float fragment_unnamed_384 = fragment_unnamed_382 * fragment_unnamed_375;
				precise float fragment_unnamed_385 = fragment_unnamed_382 * fragment_unnamed_376;
				float fragment_unnamed_386 = dot(float3(fragment_unnamed_187, fragment_unnamed_188, fragment_unnamed_189), float3(fragment_unnamed_91, fragment_unnamed_92, fragment_unnamed_93));
				float fragment_unnamed_397 = clamp(dot(float3(fragment_unnamed_187, fragment_unnamed_188, fragment_unnamed_189), float3(fragment_uniform_buffer_2[0u].xyz)), 0.0f, 1.0f);
				float fragment_unnamed_406 = clamp(dot(float3(fragment_uniform_buffer_2[0u].xyz), float3(fragment_unnamed_383, fragment_unnamed_384, fragment_unnamed_385)), 0.0f, 1.0f);
				precise float fragment_unnamed_410 = dot(fragment_unnamed_406.xx, fragment_unnamed_406.xx) + (-0.5f);
				precise float fragment_unnamed_412 = (-0.0f) - fragment_unnamed_397;
				precise float fragment_unnamed_413 = fragment_unnamed_412 + 1.0f;
				precise float fragment_unnamed_414 = fragment_unnamed_413 * fragment_unnamed_413;
				precise float fragment_unnamed_415 = fragment_unnamed_414 * fragment_unnamed_414;
				precise float fragment_unnamed_416 = fragment_unnamed_413 * fragment_unnamed_415;
				precise float fragment_unnamed_419 = (-0.0f) - abs(fragment_unnamed_386);
				precise float fragment_unnamed_420 = fragment_unnamed_419 + 1.0f;
				precise float fragment_unnamed_421 = fragment_unnamed_420 * fragment_unnamed_420;
				precise float fragment_unnamed_422 = fragment_unnamed_421 * fragment_unnamed_421;
				precise float fragment_unnamed_423 = fragment_unnamed_420 * fragment_unnamed_422;
				precise float fragment_unnamed_425 = mad(fragment_unnamed_410, fragment_unnamed_423, 1.0f) * mad(fragment_unnamed_410, fragment_unnamed_416, 1.0f);
				precise float fragment_unnamed_426 = fragment_unnamed_397 * fragment_unnamed_425;
				precise float fragment_unnamed_428 = abs(fragment_unnamed_386) + fragment_unnamed_397;
				precise float fragment_unnamed_429 = fragment_unnamed_428 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_431 = 0.5f / fragment_unnamed_429;
				precise float fragment_unnamed_433 = fragment_unnamed_431 * 0.99999988079071044921875f;
				precise float fragment_unnamed_435 = fragment_unnamed_397 * fragment_unnamed_433;
				precise float fragment_unnamed_449 = fragment_unnamed_435 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_450 = fragment_unnamed_435 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_451 = fragment_unnamed_435 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_452 = (-0.0f) - fragment_unnamed_406;
				precise float fragment_unnamed_453 = fragment_unnamed_452 + 1.0f;
				precise float fragment_unnamed_454 = fragment_unnamed_453 * fragment_unnamed_453;
				precise float fragment_unnamed_455 = fragment_unnamed_454 * fragment_unnamed_454;
				precise float fragment_unnamed_456 = fragment_unnamed_453 * fragment_unnamed_455;
				float fragment_unnamed_457 = mad(fragment_unnamed_456, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_459 = fragment_unnamed_457 * fragment_unnamed_449;
				precise float fragment_unnamed_460 = fragment_unnamed_457 * fragment_unnamed_450;
				precise float fragment_unnamed_461 = fragment_unnamed_457 * fragment_unnamed_451;
				precise float fragment_unnamed_465 = fragment_unnamed_359 * 0.5f;
				precise float fragment_unnamed_466 = fragment_unnamed_361 * 0.5f;
				precise float fragment_unnamed_467 = fragment_unnamed_363 * 0.5f;
				float fragment_unnamed_468 = mad(fragment_unnamed_423, 2.2351741790771484375e-08f, 0.039999999105930328369140625f);
				fragment_output_0.x = mad(fragment_unnamed_465, fragment_unnamed_468, mad(fragment_unnamed_365, mad(fragment_uniform_buffer_0[2u].x, fragment_unnamed_426, max(fragment_unnamed_242, 0.0f)), fragment_unnamed_459));
				fragment_output_0.y = mad(fragment_unnamed_466, fragment_unnamed_468, mad(fragment_unnamed_367, mad(fragment_uniform_buffer_0[2u].y, fragment_unnamed_426, max(fragment_unnamed_243, 0.0f)), fragment_unnamed_460));
				fragment_output_0.z = mad(fragment_unnamed_467, fragment_unnamed_468, mad(fragment_unnamed_368, mad(fragment_uniform_buffer_0[2u].z, fragment_unnamed_426, max(fragment_unnamed_244, 0.0f)), fragment_unnamed_461));
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_uniform_buffer_2[39] = float4(unity_SHAr[0], unity_SHAr[1], unity_SHAr[2], unity_SHAr[3]);

				fragment_uniform_buffer_2[40] = float4(unity_SHAg[0], unity_SHAg[1], unity_SHAg[2], unity_SHAg[3]);

				fragment_uniform_buffer_2[41] = float4(unity_SHAb[0], unity_SHAb[1], unity_SHAb[2], unity_SHAb[3]);

				fragment_uniform_buffer_3[0] = float4(unity_SpecCube0_BoxMax[0], unity_SpecCube0_BoxMax[1], unity_SpecCube0_BoxMax[2], unity_SpecCube0_BoxMax[3]);

				fragment_uniform_buffer_3[1] = float4(unity_SpecCube0_BoxMin[0], unity_SpecCube0_BoxMin[1], unity_SpecCube0_BoxMin[2], unity_SpecCube0_BoxMin[3]);

				fragment_uniform_buffer_3[2] = float4(unity_SpecCube0_ProbePosition[0], unity_SpecCube0_ProbePosition[1], unity_SpecCube0_ProbePosition[2], unity_SpecCube0_ProbePosition[3]);

				fragment_uniform_buffer_3[3] = float4(unity_SpecCube0_HDR[0], unity_SpecCube0_HDR[1], unity_SpecCube0_HDR[2], unity_SpecCube0_HDR[3]);

				fragment_uniform_buffer_3[4] = float4(unity_SpecCube1_BoxMax[0], unity_SpecCube1_BoxMax[1], unity_SpecCube1_BoxMax[2], unity_SpecCube1_BoxMax[3]);

				fragment_uniform_buffer_3[5] = float4(unity_SpecCube1_BoxMin[0], unity_SpecCube1_BoxMin[1], unity_SpecCube1_BoxMin[2], unity_SpecCube1_BoxMin[3]);

				fragment_uniform_buffer_3[6] = float4(unity_SpecCube1_ProbePosition[0], unity_SpecCube1_ProbePosition[1], unity_SpecCube1_ProbePosition[2], unity_SpecCube1_ProbePosition[3]);

				fragment_uniform_buffer_3[7] = float4(unity_SpecCube1_HDR[0], unity_SpecCube1_HDR[1], unity_SpecCube1_HDR[2], unity_SpecCube1_HDR[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				fragment_input_8 = stage_input.fragment_input_8;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // LIGHTPROBE_SH
			#endif // !FOG_LINEAR
			#endif // !VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogColor;
			float4 unity_FogParams;
			float4 unity_SpecCube0_BoxMax;
			float4 unity_SpecCube0_BoxMin;
			float4 unity_SpecCube0_ProbePosition;
			float4 unity_SpecCube0_HDR;
			float4 unity_SpecCube1_BoxMax;
			float4 unity_SpecCube1_BoxMin;
			float4 unity_SpecCube1_ProbePosition;
			float4 unity_SpecCube1_HDR;

			static float4 fragment_uniform_buffer_0[5];
			static float4 fragment_uniform_buffer_1[6];
			static float4 fragment_uniform_buffer_2[1];
			static float4 fragment_uniform_buffer_3[2];
			static float4 fragment_uniform_buffer_4[8];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			TextureCube<float4> unity_SpecCube0;
			TextureCube<float4> unity_SpecCube1;
			SamplerState samplerunity_SpecCube0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float fragment_input_1;
			static float4 fragment_input_2;
			static float4 fragment_input_3;
			static float4 fragment_input_4;
			static float4 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float fragment_input_1 : TEXCOORD6; // TEXCOORD_6
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float4 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float4 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float4 fragment_input_5 : COLOR; // COLOR
				float4 fragment_input_6 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_7 : TEXCOORD8; // TEXCOORD_8
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_75 = (-0.0f) - fragment_input_2.w;
				precise float fragment_unnamed_77 = (-0.0f) - fragment_input_3.w;
				precise float fragment_unnamed_78 = (-0.0f) - fragment_input_4.w;
				precise float fragment_unnamed_87 = fragment_unnamed_75 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_88 = fragment_unnamed_77 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_89 = fragment_unnamed_78 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_95 = rsqrt(dot(float3(fragment_unnamed_87, fragment_unnamed_88, fragment_unnamed_89), float3(fragment_unnamed_87, fragment_unnamed_88, fragment_unnamed_89)));
				precise float fragment_unnamed_96 = fragment_unnamed_95 * fragment_unnamed_87;
				precise float fragment_unnamed_97 = fragment_unnamed_95 * fragment_unnamed_88;
				precise float fragment_unnamed_98 = fragment_unnamed_95 * fragment_unnamed_89;
				float4 fragment_unnamed_106 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_118 = fragment_unnamed_106.x * fragment_uniform_buffer_0[4u].x;
				precise float fragment_unnamed_119 = fragment_unnamed_106.y * fragment_uniform_buffer_0[4u].y;
				precise float fragment_unnamed_120 = fragment_unnamed_106.z * fragment_uniform_buffer_0[4u].z;
				precise float fragment_unnamed_121 = fragment_unnamed_106.w * fragment_uniform_buffer_0[4u].w;
				precise float fragment_unnamed_128 = fragment_unnamed_118 * fragment_input_5.x;
				precise float fragment_unnamed_129 = fragment_unnamed_119 * fragment_input_5.y;
				precise float fragment_unnamed_130 = fragment_unnamed_120 * fragment_input_5.z;
				float4 fragment_unnamed_136 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_141 = fragment_unnamed_136.w * fragment_unnamed_136.x;
				float fragment_unnamed_142 = mad(fragment_unnamed_141, 2.0f, -1.0f);
				float fragment_unnamed_145 = mad(fragment_unnamed_136.y, 2.0f, -1.0f);
				precise float fragment_unnamed_151 = (-0.0f) - min(dot(float2(fragment_unnamed_142, fragment_unnamed_145), float2(fragment_unnamed_142, fragment_unnamed_145)), 1.0f);
				precise float fragment_unnamed_152 = fragment_unnamed_151 + 1.0f;
				float fragment_unnamed_153 = sqrt(fragment_unnamed_152);
				precise float fragment_unnamed_156 = fragment_unnamed_121 * fragment_input_5.w;
				fragment_output_0.w = fragment_unnamed_156;
				float fragment_unnamed_165 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_142, fragment_unnamed_145, fragment_unnamed_153));
				float fragment_unnamed_174 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_142, fragment_unnamed_145, fragment_unnamed_153));
				float fragment_unnamed_183 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_142, fragment_unnamed_145, fragment_unnamed_153));
				float fragment_unnamed_189 = rsqrt(dot(float3(fragment_unnamed_165, fragment_unnamed_174, fragment_unnamed_183), float3(fragment_unnamed_165, fragment_unnamed_174, fragment_unnamed_183)));
				precise float fragment_unnamed_190 = fragment_unnamed_189 * fragment_unnamed_165;
				precise float fragment_unnamed_191 = fragment_unnamed_189 * fragment_unnamed_174;
				precise float fragment_unnamed_192 = fragment_unnamed_189 * fragment_unnamed_183;
				precise float fragment_unnamed_193 = (-0.0f) - fragment_unnamed_96;
				precise float fragment_unnamed_194 = (-0.0f) - fragment_unnamed_97;
				precise float fragment_unnamed_195 = (-0.0f) - fragment_unnamed_98;
				float fragment_unnamed_196 = dot(float3(fragment_unnamed_193, fragment_unnamed_194, fragment_unnamed_195), float3(fragment_unnamed_190, fragment_unnamed_191, fragment_unnamed_192));
				precise float fragment_unnamed_199 = fragment_unnamed_196 + fragment_unnamed_196;
				precise float fragment_unnamed_200 = (-0.0f) - fragment_unnamed_199;
				precise float fragment_unnamed_201 = (-0.0f) - fragment_unnamed_96;
				precise float fragment_unnamed_202 = (-0.0f) - fragment_unnamed_97;
				precise float fragment_unnamed_203 = (-0.0f) - fragment_unnamed_98;
				float fragment_unnamed_204 = mad(fragment_unnamed_190, fragment_unnamed_200, fragment_unnamed_201);
				float fragment_unnamed_205 = mad(fragment_unnamed_191, fragment_unnamed_200, fragment_unnamed_202);
				float fragment_unnamed_206 = mad(fragment_unnamed_192, fragment_unnamed_200, fragment_unnamed_203);
				float fragment_unnamed_278;
				float fragment_unnamed_279;
				float fragment_unnamed_280;
				if (0.0f < fragment_uniform_buffer_4[2u].w)
				{
					float fragment_unnamed_215 = rsqrt(dot(float3(fragment_unnamed_204, fragment_unnamed_205, fragment_unnamed_206), float3(fragment_unnamed_204, fragment_unnamed_205, fragment_unnamed_206)));
					precise float fragment_unnamed_216 = fragment_unnamed_215 * fragment_unnamed_204;
					precise float fragment_unnamed_217 = fragment_unnamed_215 * fragment_unnamed_205;
					precise float fragment_unnamed_218 = fragment_unnamed_215 * fragment_unnamed_206;
					precise float fragment_unnamed_219 = (-0.0f) - fragment_input_2.w;
					precise float fragment_unnamed_220 = (-0.0f) - fragment_input_3.w;
					precise float fragment_unnamed_221 = (-0.0f) - fragment_input_4.w;
					precise float fragment_unnamed_227 = fragment_unnamed_219 + fragment_uniform_buffer_4[0u].x;
					precise float fragment_unnamed_228 = fragment_unnamed_220 + fragment_uniform_buffer_4[0u].y;
					precise float fragment_unnamed_229 = fragment_unnamed_221 + fragment_uniform_buffer_4[0u].z;
					precise float fragment_unnamed_230 = fragment_unnamed_227 / fragment_unnamed_216;
					precise float fragment_unnamed_231 = fragment_unnamed_228 / fragment_unnamed_217;
					precise float fragment_unnamed_232 = fragment_unnamed_229 / fragment_unnamed_218;
					precise float fragment_unnamed_233 = (-0.0f) - fragment_input_2.w;
					precise float fragment_unnamed_234 = (-0.0f) - fragment_input_3.w;
					precise float fragment_unnamed_235 = (-0.0f) - fragment_input_4.w;
					precise float fragment_unnamed_241 = fragment_unnamed_233 + fragment_uniform_buffer_4[1u].x;
					precise float fragment_unnamed_242 = fragment_unnamed_234 + fragment_uniform_buffer_4[1u].y;
					precise float fragment_unnamed_243 = fragment_unnamed_235 + fragment_uniform_buffer_4[1u].z;
					precise float fragment_unnamed_244 = fragment_unnamed_241 / fragment_unnamed_216;
					precise float fragment_unnamed_245 = fragment_unnamed_242 / fragment_unnamed_217;
					precise float fragment_unnamed_246 = fragment_unnamed_243 / fragment_unnamed_218;
					float fragment_unnamed_263 = min(asfloat((0.0f < fragment_unnamed_218) ? asuint(fragment_unnamed_232) : asuint(fragment_unnamed_246)), min(asfloat((0.0f < fragment_unnamed_217) ? asuint(fragment_unnamed_231) : asuint(fragment_unnamed_245)), asfloat((0.0f < fragment_unnamed_216) ? asuint(fragment_unnamed_230) : asuint(fragment_unnamed_244))));
					precise float fragment_unnamed_267 = (-0.0f) - fragment_uniform_buffer_4[2u].x;
					precise float fragment_unnamed_269 = (-0.0f) - fragment_uniform_buffer_4[2u].y;
					precise float fragment_unnamed_271 = (-0.0f) - fragment_uniform_buffer_4[2u].z;
					precise float fragment_unnamed_272 = fragment_input_2.w + fragment_unnamed_267;
					precise float fragment_unnamed_273 = fragment_input_3.w + fragment_unnamed_269;
					precise float fragment_unnamed_274 = fragment_input_4.w + fragment_unnamed_271;
					fragment_unnamed_278 = mad(fragment_unnamed_216, fragment_unnamed_263, fragment_unnamed_272);
					fragment_unnamed_279 = mad(fragment_unnamed_217, fragment_unnamed_263, fragment_unnamed_273);
					fragment_unnamed_280 = mad(fragment_unnamed_218, fragment_unnamed_263, fragment_unnamed_274);
				}
				else
				{
					fragment_unnamed_278 = fragment_unnamed_204;
					fragment_unnamed_279 = fragment_unnamed_205;
					fragment_unnamed_280 = fragment_unnamed_206;
				}
				float4 fragment_unnamed_284 = unity_SpecCube0.SampleLevel(samplerunity_SpecCube0, float3(fragment_unnamed_278, fragment_unnamed_279, fragment_unnamed_280), 6.0f);
				float fragment_unnamed_286 = fragment_unnamed_284.x;
				float fragment_unnamed_287 = fragment_unnamed_284.y;
				float fragment_unnamed_288 = fragment_unnamed_284.z;
				precise float fragment_unnamed_290 = fragment_unnamed_284.w + (-1.0f);
				precise float fragment_unnamed_299 = log2(mad(fragment_uniform_buffer_4[3u].w, fragment_unnamed_290, 1.0f)) * fragment_uniform_buffer_4[3u].y;
				precise float fragment_unnamed_304 = exp2(fragment_unnamed_299) * fragment_uniform_buffer_4[3u].x;
				precise float fragment_unnamed_305 = fragment_unnamed_286 * fragment_unnamed_304;
				precise float fragment_unnamed_306 = fragment_unnamed_287 * fragment_unnamed_304;
				precise float fragment_unnamed_307 = fragment_unnamed_288 * fragment_unnamed_304;
				float fragment_unnamed_317;
				float fragment_unnamed_319;
				float fragment_unnamed_321;
				if (fragment_uniform_buffer_4[1u].w < 0.999989986419677734375f)
				{
					float fragment_unnamed_539;
					float fragment_unnamed_540;
					float fragment_unnamed_541;
					if (0.0f < fragment_uniform_buffer_4[6u].w)
					{
						float fragment_unnamed_476 = rsqrt(dot(float3(fragment_unnamed_204, fragment_unnamed_205, fragment_unnamed_206), float3(fragment_unnamed_204, fragment_unnamed_205, fragment_unnamed_206)));
						precise float fragment_unnamed_477 = fragment_unnamed_476 * fragment_unnamed_204;
						precise float fragment_unnamed_478 = fragment_unnamed_476 * fragment_unnamed_205;
						precise float fragment_unnamed_479 = fragment_unnamed_476 * fragment_unnamed_206;
						precise float fragment_unnamed_480 = (-0.0f) - fragment_input_2.w;
						precise float fragment_unnamed_481 = (-0.0f) - fragment_input_3.w;
						precise float fragment_unnamed_482 = (-0.0f) - fragment_input_4.w;
						precise float fragment_unnamed_488 = fragment_unnamed_480 + fragment_uniform_buffer_4[4u].x;
						precise float fragment_unnamed_489 = fragment_unnamed_481 + fragment_uniform_buffer_4[4u].y;
						precise float fragment_unnamed_490 = fragment_unnamed_482 + fragment_uniform_buffer_4[4u].z;
						precise float fragment_unnamed_491 = fragment_unnamed_488 / fragment_unnamed_477;
						precise float fragment_unnamed_492 = fragment_unnamed_489 / fragment_unnamed_478;
						precise float fragment_unnamed_493 = fragment_unnamed_490 / fragment_unnamed_479;
						precise float fragment_unnamed_494 = (-0.0f) - fragment_input_2.w;
						precise float fragment_unnamed_495 = (-0.0f) - fragment_input_3.w;
						precise float fragment_unnamed_496 = (-0.0f) - fragment_input_4.w;
						precise float fragment_unnamed_502 = fragment_unnamed_494 + fragment_uniform_buffer_4[5u].x;
						precise float fragment_unnamed_503 = fragment_unnamed_495 + fragment_uniform_buffer_4[5u].y;
						precise float fragment_unnamed_504 = fragment_unnamed_496 + fragment_uniform_buffer_4[5u].z;
						precise float fragment_unnamed_505 = fragment_unnamed_502 / fragment_unnamed_477;
						precise float fragment_unnamed_506 = fragment_unnamed_503 / fragment_unnamed_478;
						precise float fragment_unnamed_507 = fragment_unnamed_504 / fragment_unnamed_479;
						float fragment_unnamed_524 = min(asfloat((0.0f < fragment_unnamed_479) ? asuint(fragment_unnamed_493) : asuint(fragment_unnamed_507)), min(asfloat((0.0f < fragment_unnamed_478) ? asuint(fragment_unnamed_492) : asuint(fragment_unnamed_506)), asfloat((0.0f < fragment_unnamed_477) ? asuint(fragment_unnamed_491) : asuint(fragment_unnamed_505))));
						precise float fragment_unnamed_528 = (-0.0f) - fragment_uniform_buffer_4[6u].x;
						precise float fragment_unnamed_530 = (-0.0f) - fragment_uniform_buffer_4[6u].y;
						precise float fragment_unnamed_532 = (-0.0f) - fragment_uniform_buffer_4[6u].z;
						precise float fragment_unnamed_533 = fragment_input_2.w + fragment_unnamed_528;
						precise float fragment_unnamed_534 = fragment_input_3.w + fragment_unnamed_530;
						precise float fragment_unnamed_535 = fragment_input_4.w + fragment_unnamed_532;
						fragment_unnamed_539 = mad(fragment_unnamed_477, fragment_unnamed_524, fragment_unnamed_533);
						fragment_unnamed_540 = mad(fragment_unnamed_478, fragment_unnamed_524, fragment_unnamed_534);
						fragment_unnamed_541 = mad(fragment_unnamed_479, fragment_unnamed_524, fragment_unnamed_535);
					}
					else
					{
						fragment_unnamed_539 = fragment_unnamed_204;
						fragment_unnamed_540 = fragment_unnamed_205;
						fragment_unnamed_541 = fragment_unnamed_206;
					}
					float4 fragment_unnamed_543 = unity_SpecCube1.SampleLevel(samplerunity_SpecCube0, float3(fragment_unnamed_539, fragment_unnamed_540, fragment_unnamed_541), 6.0f);
					precise float fragment_unnamed_549 = fragment_unnamed_543.w + (-1.0f);
					precise float fragment_unnamed_559 = log2(mad(fragment_uniform_buffer_4[7u].w, fragment_unnamed_549, 1.0f)) * fragment_uniform_buffer_4[7u].y;
					precise float fragment_unnamed_564 = exp2(fragment_unnamed_559) * fragment_uniform_buffer_4[7u].x;
					precise float fragment_unnamed_565 = fragment_unnamed_543.x * fragment_unnamed_564;
					precise float fragment_unnamed_566 = fragment_unnamed_543.y * fragment_unnamed_564;
					precise float fragment_unnamed_567 = fragment_unnamed_543.z * fragment_unnamed_564;
					precise float fragment_unnamed_568 = (-0.0f) - fragment_unnamed_565;
					precise float fragment_unnamed_569 = (-0.0f) - fragment_unnamed_566;
					precise float fragment_unnamed_570 = (-0.0f) - fragment_unnamed_567;
					fragment_unnamed_317 = mad(fragment_uniform_buffer_4[1u].w, mad(fragment_unnamed_304, fragment_unnamed_286, fragment_unnamed_568), fragment_unnamed_565);
					fragment_unnamed_319 = mad(fragment_uniform_buffer_4[1u].w, mad(fragment_unnamed_304, fragment_unnamed_287, fragment_unnamed_569), fragment_unnamed_566);
					fragment_unnamed_321 = mad(fragment_uniform_buffer_4[1u].w, mad(fragment_unnamed_304, fragment_unnamed_288, fragment_unnamed_570), fragment_unnamed_567);
				}
				else
				{
					fragment_unnamed_317 = fragment_unnamed_305;
					fragment_unnamed_319 = fragment_unnamed_306;
					fragment_unnamed_321 = fragment_unnamed_307;
				}
				precise float fragment_unnamed_323 = fragment_unnamed_128 * 0.959999978542327880859375f;
				precise float fragment_unnamed_325 = fragment_unnamed_129 * 0.959999978542327880859375f;
				precise float fragment_unnamed_326 = fragment_unnamed_130 * 0.959999978542327880859375f;
				float fragment_unnamed_332 = mad(fragment_unnamed_87, fragment_unnamed_95, fragment_uniform_buffer_2[0u].x);
				float fragment_unnamed_333 = mad(fragment_unnamed_88, fragment_unnamed_95, fragment_uniform_buffer_2[0u].y);
				float fragment_unnamed_334 = mad(fragment_unnamed_89, fragment_unnamed_95, fragment_uniform_buffer_2[0u].z);
				float fragment_unnamed_340 = rsqrt(max(dot(float3(fragment_unnamed_332, fragment_unnamed_333, fragment_unnamed_334), float3(fragment_unnamed_332, fragment_unnamed_333, fragment_unnamed_334)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_341 = fragment_unnamed_340 * fragment_unnamed_332;
				precise float fragment_unnamed_342 = fragment_unnamed_340 * fragment_unnamed_333;
				precise float fragment_unnamed_343 = fragment_unnamed_340 * fragment_unnamed_334;
				float fragment_unnamed_344 = dot(float3(fragment_unnamed_190, fragment_unnamed_191, fragment_unnamed_192), float3(fragment_unnamed_96, fragment_unnamed_97, fragment_unnamed_98));
				float fragment_unnamed_355 = clamp(dot(float3(fragment_unnamed_190, fragment_unnamed_191, fragment_unnamed_192), float3(fragment_uniform_buffer_2[0u].xyz)), 0.0f, 1.0f);
				float fragment_unnamed_364 = clamp(dot(float3(fragment_uniform_buffer_2[0u].xyz), float3(fragment_unnamed_341, fragment_unnamed_342, fragment_unnamed_343)), 0.0f, 1.0f);
				precise float fragment_unnamed_368 = dot(fragment_unnamed_364.xx, fragment_unnamed_364.xx) + (-0.5f);
				precise float fragment_unnamed_370 = (-0.0f) - fragment_unnamed_355;
				precise float fragment_unnamed_371 = fragment_unnamed_370 + 1.0f;
				precise float fragment_unnamed_372 = fragment_unnamed_371 * fragment_unnamed_371;
				precise float fragment_unnamed_373 = fragment_unnamed_372 * fragment_unnamed_372;
				precise float fragment_unnamed_374 = fragment_unnamed_371 * fragment_unnamed_373;
				precise float fragment_unnamed_377 = (-0.0f) - abs(fragment_unnamed_344);
				precise float fragment_unnamed_378 = fragment_unnamed_377 + 1.0f;
				precise float fragment_unnamed_379 = fragment_unnamed_378 * fragment_unnamed_378;
				precise float fragment_unnamed_380 = fragment_unnamed_379 * fragment_unnamed_379;
				precise float fragment_unnamed_381 = fragment_unnamed_378 * fragment_unnamed_380;
				precise float fragment_unnamed_383 = mad(fragment_unnamed_368, fragment_unnamed_381, 1.0f) * mad(fragment_unnamed_368, fragment_unnamed_374, 1.0f);
				precise float fragment_unnamed_384 = fragment_unnamed_355 * fragment_unnamed_383;
				precise float fragment_unnamed_386 = abs(fragment_unnamed_344) + fragment_unnamed_355;
				precise float fragment_unnamed_387 = fragment_unnamed_386 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_389 = 0.5f / fragment_unnamed_387;
				precise float fragment_unnamed_391 = fragment_unnamed_389 * 0.99999988079071044921875f;
				precise float fragment_unnamed_393 = fragment_unnamed_355 * fragment_unnamed_391;
				precise float fragment_unnamed_399 = fragment_unnamed_384 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_400 = fragment_unnamed_384 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_401 = fragment_unnamed_384 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_407 = fragment_unnamed_393 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_408 = fragment_unnamed_393 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_409 = fragment_unnamed_393 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_410 = (-0.0f) - fragment_unnamed_364;
				precise float fragment_unnamed_411 = fragment_unnamed_410 + 1.0f;
				precise float fragment_unnamed_412 = fragment_unnamed_411 * fragment_unnamed_411;
				precise float fragment_unnamed_413 = fragment_unnamed_412 * fragment_unnamed_412;
				precise float fragment_unnamed_414 = fragment_unnamed_411 * fragment_unnamed_413;
				float fragment_unnamed_415 = mad(fragment_unnamed_414, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_417 = fragment_unnamed_415 * fragment_unnamed_407;
				precise float fragment_unnamed_418 = fragment_unnamed_415 * fragment_unnamed_408;
				precise float fragment_unnamed_419 = fragment_unnamed_415 * fragment_unnamed_409;
				precise float fragment_unnamed_423 = fragment_unnamed_317 * 0.5f;
				precise float fragment_unnamed_424 = fragment_unnamed_319 * 0.5f;
				precise float fragment_unnamed_425 = fragment_unnamed_321 * 0.5f;
				float fragment_unnamed_426 = mad(fragment_unnamed_381, 2.2351741790771484375e-08f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_435 = fragment_input_1 / fragment_uniform_buffer_1[5u].y;
				precise float fragment_unnamed_436 = (-0.0f) - fragment_unnamed_435;
				precise float fragment_unnamed_437 = fragment_unnamed_436 + 1.0f;
				precise float fragment_unnamed_441 = fragment_unnamed_437 * fragment_uniform_buffer_1[5u].z;
				float fragment_unnamed_450 = clamp(mad(max(fragment_unnamed_441, 0.0f), fragment_uniform_buffer_3[1u].z, fragment_uniform_buffer_3[1u].w), 0.0f, 1.0f);
				precise float fragment_unnamed_454 = (-0.0f) - fragment_uniform_buffer_3[0u].x;
				precise float fragment_unnamed_456 = (-0.0f) - fragment_uniform_buffer_3[0u].y;
				precise float fragment_unnamed_458 = (-0.0f) - fragment_uniform_buffer_3[0u].z;
				precise float fragment_unnamed_459 = mad(fragment_unnamed_423, fragment_unnamed_426, mad(fragment_unnamed_323, fragment_unnamed_399, fragment_unnamed_417)) + fragment_unnamed_454;
				precise float fragment_unnamed_460 = mad(fragment_unnamed_424, fragment_unnamed_426, mad(fragment_unnamed_325, fragment_unnamed_400, fragment_unnamed_418)) + fragment_unnamed_456;
				precise float fragment_unnamed_461 = mad(fragment_unnamed_425, fragment_unnamed_426, mad(fragment_unnamed_326, fragment_unnamed_401, fragment_unnamed_419)) + fragment_unnamed_458;
				fragment_output_0.x = mad(fragment_unnamed_450, fragment_unnamed_459, fragment_uniform_buffer_3[0u].x);
				fragment_output_0.y = mad(fragment_unnamed_450, fragment_unnamed_460, fragment_uniform_buffer_3[0u].y);
				fragment_output_0.z = mad(fragment_unnamed_450, fragment_unnamed_461, fragment_uniform_buffer_3[0u].z);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_1[5] = float4(_ProjectionParams[0], _ProjectionParams[1], _ProjectionParams[2], _ProjectionParams[3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_uniform_buffer_3[0] = float4(unity_FogColor[0], unity_FogColor[1], unity_FogColor[2], unity_FogColor[3]);

				fragment_uniform_buffer_3[1] = float4(unity_FogParams[0], unity_FogParams[1], unity_FogParams[2], unity_FogParams[3]);

				fragment_uniform_buffer_4[0] = float4(unity_SpecCube0_BoxMax[0], unity_SpecCube0_BoxMax[1], unity_SpecCube0_BoxMax[2], unity_SpecCube0_BoxMax[3]);

				fragment_uniform_buffer_4[1] = float4(unity_SpecCube0_BoxMin[0], unity_SpecCube0_BoxMin[1], unity_SpecCube0_BoxMin[2], unity_SpecCube0_BoxMin[3]);

				fragment_uniform_buffer_4[2] = float4(unity_SpecCube0_ProbePosition[0], unity_SpecCube0_ProbePosition[1], unity_SpecCube0_ProbePosition[2], unity_SpecCube0_ProbePosition[3]);

				fragment_uniform_buffer_4[3] = float4(unity_SpecCube0_HDR[0], unity_SpecCube0_HDR[1], unity_SpecCube0_HDR[2], unity_SpecCube0_HDR[3]);

				fragment_uniform_buffer_4[4] = float4(unity_SpecCube1_BoxMax[0], unity_SpecCube1_BoxMax[1], unity_SpecCube1_BoxMax[2], unity_SpecCube1_BoxMax[3]);

				fragment_uniform_buffer_4[5] = float4(unity_SpecCube1_BoxMin[0], unity_SpecCube1_BoxMin[1], unity_SpecCube1_BoxMin[2], unity_SpecCube1_BoxMin[3]);

				fragment_uniform_buffer_4[6] = float4(unity_SpecCube1_ProbePosition[0], unity_SpecCube1_ProbePosition[1], unity_SpecCube1_ProbePosition[2], unity_SpecCube1_ProbePosition[3]);

				fragment_uniform_buffer_4[7] = float4(unity_SpecCube1_HDR[0], unity_SpecCube1_HDR[1], unity_SpecCube1_HDR[2], unity_SpecCube1_HDR[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // !LIGHTPROBE_SH
			#endif // !VERTEXLIGHT_ON


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifdef LIGHTPROBE_SH
			#ifndef VERTEXLIGHT_ON
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_SHAr;
			float4 unity_SHAg;
			float4 unity_SHAb;
			float4 unity_FogColor;
			float4 unity_FogParams;
			float4 unity_SpecCube0_BoxMax;
			float4 unity_SpecCube0_BoxMin;
			float4 unity_SpecCube0_ProbePosition;
			float4 unity_SpecCube0_HDR;
			float4 unity_SpecCube1_BoxMax;
			float4 unity_SpecCube1_BoxMin;
			float4 unity_SpecCube1_ProbePosition;
			float4 unity_SpecCube1_HDR;

			static float4 fragment_uniform_buffer_0[5];
			static float4 fragment_uniform_buffer_1[6];
			static float4 fragment_uniform_buffer_2[42];
			static float4 fragment_uniform_buffer_3[2];
			static float4 fragment_uniform_buffer_4[8];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			TextureCube<float4> unity_SpecCube0;
			TextureCube<float4> unity_SpecCube1;
			SamplerState samplerunity_SpecCube0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float fragment_input_1;
			static float4 fragment_input_2;
			static float4 fragment_input_3;
			static float4 fragment_input_4;
			static float4 fragment_input_5;
			static float4 fragment_input_6;
			static float3 fragment_input_7;
			static float4 fragment_input_8;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float fragment_input_1 : TEXCOORD6; // TEXCOORD_6
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float4 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float4 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float4 fragment_input_5 : COLOR; // COLOR
				float4 fragment_input_6 : TEXCOORD4; // TEXCOORD_4
				float3 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
				float4 fragment_input_8 : TEXCOORD8; // TEXCOORD_8
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_78 = (-0.0f) - fragment_input_2.w;
				precise float fragment_unnamed_80 = (-0.0f) - fragment_input_3.w;
				precise float fragment_unnamed_81 = (-0.0f) - fragment_input_4.w;
				precise float fragment_unnamed_90 = fragment_unnamed_78 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_91 = fragment_unnamed_80 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_92 = fragment_unnamed_81 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_97 = rsqrt(dot(float3(fragment_unnamed_90, fragment_unnamed_91, fragment_unnamed_92), float3(fragment_unnamed_90, fragment_unnamed_91, fragment_unnamed_92)));
				precise float fragment_unnamed_98 = fragment_unnamed_97 * fragment_unnamed_90;
				precise float fragment_unnamed_99 = fragment_unnamed_97 * fragment_unnamed_91;
				precise float fragment_unnamed_100 = fragment_unnamed_97 * fragment_unnamed_92;
				float4 fragment_unnamed_109 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_121 = fragment_unnamed_109.x * fragment_uniform_buffer_0[4u].x;
				precise float fragment_unnamed_122 = fragment_unnamed_109.y * fragment_uniform_buffer_0[4u].y;
				precise float fragment_unnamed_123 = fragment_unnamed_109.z * fragment_uniform_buffer_0[4u].z;
				precise float fragment_unnamed_124 = fragment_unnamed_109.w * fragment_uniform_buffer_0[4u].w;
				precise float fragment_unnamed_131 = fragment_unnamed_121 * fragment_input_5.x;
				precise float fragment_unnamed_132 = fragment_unnamed_122 * fragment_input_5.y;
				precise float fragment_unnamed_133 = fragment_unnamed_123 * fragment_input_5.z;
				float4 fragment_unnamed_139 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_144 = fragment_unnamed_139.w * fragment_unnamed_139.x;
				float fragment_unnamed_145 = mad(fragment_unnamed_144, 2.0f, -1.0f);
				float fragment_unnamed_148 = mad(fragment_unnamed_139.y, 2.0f, -1.0f);
				precise float fragment_unnamed_154 = (-0.0f) - min(dot(float2(fragment_unnamed_145, fragment_unnamed_148), float2(fragment_unnamed_145, fragment_unnamed_148)), 1.0f);
				precise float fragment_unnamed_155 = fragment_unnamed_154 + 1.0f;
				float fragment_unnamed_156 = sqrt(fragment_unnamed_155);
				precise float fragment_unnamed_159 = fragment_unnamed_124 * fragment_input_5.w;
				fragment_output_0.w = fragment_unnamed_159;
				float fragment_unnamed_168 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_145, fragment_unnamed_148, fragment_unnamed_156));
				float fragment_unnamed_177 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_145, fragment_unnamed_148, fragment_unnamed_156));
				float fragment_unnamed_186 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_145, fragment_unnamed_148, fragment_unnamed_156));
				float fragment_unnamed_192 = rsqrt(dot(float3(fragment_unnamed_168, fragment_unnamed_177, fragment_unnamed_186), float3(fragment_unnamed_168, fragment_unnamed_177, fragment_unnamed_186)));
				precise float fragment_unnamed_193 = fragment_unnamed_192 * fragment_unnamed_168;
				precise float fragment_unnamed_194 = fragment_unnamed_192 * fragment_unnamed_177;
				precise float fragment_unnamed_195 = fragment_unnamed_192 * fragment_unnamed_186;
				precise float fragment_unnamed_196 = (-0.0f) - fragment_unnamed_98;
				precise float fragment_unnamed_197 = (-0.0f) - fragment_unnamed_99;
				precise float fragment_unnamed_198 = (-0.0f) - fragment_unnamed_100;
				float fragment_unnamed_199 = dot(float3(fragment_unnamed_196, fragment_unnamed_197, fragment_unnamed_198), float3(fragment_unnamed_193, fragment_unnamed_194, fragment_unnamed_195));
				precise float fragment_unnamed_202 = fragment_unnamed_199 + fragment_unnamed_199;
				precise float fragment_unnamed_203 = (-0.0f) - fragment_unnamed_202;
				precise float fragment_unnamed_204 = (-0.0f) - fragment_unnamed_98;
				precise float fragment_unnamed_205 = (-0.0f) - fragment_unnamed_99;
				precise float fragment_unnamed_206 = (-0.0f) - fragment_unnamed_100;
				float fragment_unnamed_207 = mad(fragment_unnamed_193, fragment_unnamed_203, fragment_unnamed_204);
				float fragment_unnamed_208 = mad(fragment_unnamed_194, fragment_unnamed_203, fragment_unnamed_205);
				float fragment_unnamed_209 = mad(fragment_unnamed_195, fragment_unnamed_203, fragment_unnamed_206);
				float fragment_unnamed_210 = asfloat(1065353216u);
				precise float fragment_unnamed_248 = dot(float4(fragment_uniform_buffer_2[39u]), float4(fragment_unnamed_193, fragment_unnamed_194, fragment_unnamed_195, fragment_unnamed_210)) + fragment_input_7.x;
				precise float fragment_unnamed_249 = dot(float4(fragment_uniform_buffer_2[40u]), float4(fragment_unnamed_193, fragment_unnamed_194, fragment_unnamed_195, fragment_unnamed_210)) + fragment_input_7.y;
				precise float fragment_unnamed_250 = dot(float4(fragment_uniform_buffer_2[41u]), float4(fragment_unnamed_193, fragment_unnamed_194, fragment_unnamed_195, fragment_unnamed_210)) + fragment_input_7.z;
				float fragment_unnamed_325;
				float fragment_unnamed_326;
				float fragment_unnamed_327;
				if (0.0f < fragment_uniform_buffer_4[2u].w)
				{
					float fragment_unnamed_262 = rsqrt(dot(float3(fragment_unnamed_207, fragment_unnamed_208, fragment_unnamed_209), float3(fragment_unnamed_207, fragment_unnamed_208, fragment_unnamed_209)));
					precise float fragment_unnamed_263 = fragment_unnamed_262 * fragment_unnamed_207;
					precise float fragment_unnamed_264 = fragment_unnamed_262 * fragment_unnamed_208;
					precise float fragment_unnamed_265 = fragment_unnamed_262 * fragment_unnamed_209;
					precise float fragment_unnamed_266 = (-0.0f) - fragment_input_2.w;
					precise float fragment_unnamed_267 = (-0.0f) - fragment_input_3.w;
					precise float fragment_unnamed_268 = (-0.0f) - fragment_input_4.w;
					precise float fragment_unnamed_274 = fragment_unnamed_266 + fragment_uniform_buffer_4[0u].x;
					precise float fragment_unnamed_275 = fragment_unnamed_267 + fragment_uniform_buffer_4[0u].y;
					precise float fragment_unnamed_276 = fragment_unnamed_268 + fragment_uniform_buffer_4[0u].z;
					precise float fragment_unnamed_277 = fragment_unnamed_274 / fragment_unnamed_263;
					precise float fragment_unnamed_278 = fragment_unnamed_275 / fragment_unnamed_264;
					precise float fragment_unnamed_279 = fragment_unnamed_276 / fragment_unnamed_265;
					precise float fragment_unnamed_280 = (-0.0f) - fragment_input_2.w;
					precise float fragment_unnamed_281 = (-0.0f) - fragment_input_3.w;
					precise float fragment_unnamed_282 = (-0.0f) - fragment_input_4.w;
					precise float fragment_unnamed_288 = fragment_unnamed_280 + fragment_uniform_buffer_4[1u].x;
					precise float fragment_unnamed_289 = fragment_unnamed_281 + fragment_uniform_buffer_4[1u].y;
					precise float fragment_unnamed_290 = fragment_unnamed_282 + fragment_uniform_buffer_4[1u].z;
					precise float fragment_unnamed_291 = fragment_unnamed_288 / fragment_unnamed_263;
					precise float fragment_unnamed_292 = fragment_unnamed_289 / fragment_unnamed_264;
					precise float fragment_unnamed_293 = fragment_unnamed_290 / fragment_unnamed_265;
					float fragment_unnamed_310 = min(asfloat((0.0f < fragment_unnamed_265) ? asuint(fragment_unnamed_279) : asuint(fragment_unnamed_293)), min(asfloat((0.0f < fragment_unnamed_264) ? asuint(fragment_unnamed_278) : asuint(fragment_unnamed_292)), asfloat((0.0f < fragment_unnamed_263) ? asuint(fragment_unnamed_277) : asuint(fragment_unnamed_291))));
					precise float fragment_unnamed_314 = (-0.0f) - fragment_uniform_buffer_4[2u].x;
					precise float fragment_unnamed_316 = (-0.0f) - fragment_uniform_buffer_4[2u].y;
					precise float fragment_unnamed_318 = (-0.0f) - fragment_uniform_buffer_4[2u].z;
					precise float fragment_unnamed_319 = fragment_input_2.w + fragment_unnamed_314;
					precise float fragment_unnamed_320 = fragment_input_3.w + fragment_unnamed_316;
					precise float fragment_unnamed_321 = fragment_input_4.w + fragment_unnamed_318;
					fragment_unnamed_325 = mad(fragment_unnamed_263, fragment_unnamed_310, fragment_unnamed_319);
					fragment_unnamed_326 = mad(fragment_unnamed_264, fragment_unnamed_310, fragment_unnamed_320);
					fragment_unnamed_327 = mad(fragment_unnamed_265, fragment_unnamed_310, fragment_unnamed_321);
				}
				else
				{
					fragment_unnamed_325 = fragment_unnamed_207;
					fragment_unnamed_326 = fragment_unnamed_208;
					fragment_unnamed_327 = fragment_unnamed_209;
				}
				float4 fragment_unnamed_331 = unity_SpecCube0.SampleLevel(samplerunity_SpecCube0, float3(fragment_unnamed_325, fragment_unnamed_326, fragment_unnamed_327), 6.0f);
				float fragment_unnamed_333 = fragment_unnamed_331.x;
				float fragment_unnamed_334 = fragment_unnamed_331.y;
				float fragment_unnamed_335 = fragment_unnamed_331.z;
				precise float fragment_unnamed_337 = fragment_unnamed_331.w + (-1.0f);
				precise float fragment_unnamed_346 = log2(mad(fragment_uniform_buffer_4[3u].w, fragment_unnamed_337, 1.0f)) * fragment_uniform_buffer_4[3u].y;
				precise float fragment_unnamed_351 = exp2(fragment_unnamed_346) * fragment_uniform_buffer_4[3u].x;
				precise float fragment_unnamed_352 = fragment_unnamed_333 * fragment_unnamed_351;
				precise float fragment_unnamed_353 = fragment_unnamed_334 * fragment_unnamed_351;
				precise float fragment_unnamed_354 = fragment_unnamed_335 * fragment_unnamed_351;
				float fragment_unnamed_364;
				float fragment_unnamed_366;
				float fragment_unnamed_368;
				if (fragment_uniform_buffer_4[1u].w < 0.999989986419677734375f)
				{
					float fragment_unnamed_586;
					float fragment_unnamed_587;
					float fragment_unnamed_588;
					if (0.0f < fragment_uniform_buffer_4[6u].w)
					{
						float fragment_unnamed_523 = rsqrt(dot(float3(fragment_unnamed_207, fragment_unnamed_208, fragment_unnamed_209), float3(fragment_unnamed_207, fragment_unnamed_208, fragment_unnamed_209)));
						precise float fragment_unnamed_524 = fragment_unnamed_523 * fragment_unnamed_207;
						precise float fragment_unnamed_525 = fragment_unnamed_523 * fragment_unnamed_208;
						precise float fragment_unnamed_526 = fragment_unnamed_523 * fragment_unnamed_209;
						precise float fragment_unnamed_527 = (-0.0f) - fragment_input_2.w;
						precise float fragment_unnamed_528 = (-0.0f) - fragment_input_3.w;
						precise float fragment_unnamed_529 = (-0.0f) - fragment_input_4.w;
						precise float fragment_unnamed_535 = fragment_unnamed_527 + fragment_uniform_buffer_4[4u].x;
						precise float fragment_unnamed_536 = fragment_unnamed_528 + fragment_uniform_buffer_4[4u].y;
						precise float fragment_unnamed_537 = fragment_unnamed_529 + fragment_uniform_buffer_4[4u].z;
						precise float fragment_unnamed_538 = fragment_unnamed_535 / fragment_unnamed_524;
						precise float fragment_unnamed_539 = fragment_unnamed_536 / fragment_unnamed_525;
						precise float fragment_unnamed_540 = fragment_unnamed_537 / fragment_unnamed_526;
						precise float fragment_unnamed_541 = (-0.0f) - fragment_input_2.w;
						precise float fragment_unnamed_542 = (-0.0f) - fragment_input_3.w;
						precise float fragment_unnamed_543 = (-0.0f) - fragment_input_4.w;
						precise float fragment_unnamed_549 = fragment_unnamed_541 + fragment_uniform_buffer_4[5u].x;
						precise float fragment_unnamed_550 = fragment_unnamed_542 + fragment_uniform_buffer_4[5u].y;
						precise float fragment_unnamed_551 = fragment_unnamed_543 + fragment_uniform_buffer_4[5u].z;
						precise float fragment_unnamed_552 = fragment_unnamed_549 / fragment_unnamed_524;
						precise float fragment_unnamed_553 = fragment_unnamed_550 / fragment_unnamed_525;
						precise float fragment_unnamed_554 = fragment_unnamed_551 / fragment_unnamed_526;
						float fragment_unnamed_571 = min(asfloat((0.0f < fragment_unnamed_526) ? asuint(fragment_unnamed_540) : asuint(fragment_unnamed_554)), min(asfloat((0.0f < fragment_unnamed_525) ? asuint(fragment_unnamed_539) : asuint(fragment_unnamed_553)), asfloat((0.0f < fragment_unnamed_524) ? asuint(fragment_unnamed_538) : asuint(fragment_unnamed_552))));
						precise float fragment_unnamed_575 = (-0.0f) - fragment_uniform_buffer_4[6u].x;
						precise float fragment_unnamed_577 = (-0.0f) - fragment_uniform_buffer_4[6u].y;
						precise float fragment_unnamed_579 = (-0.0f) - fragment_uniform_buffer_4[6u].z;
						precise float fragment_unnamed_580 = fragment_input_2.w + fragment_unnamed_575;
						precise float fragment_unnamed_581 = fragment_input_3.w + fragment_unnamed_577;
						precise float fragment_unnamed_582 = fragment_input_4.w + fragment_unnamed_579;
						fragment_unnamed_586 = mad(fragment_unnamed_524, fragment_unnamed_571, fragment_unnamed_580);
						fragment_unnamed_587 = mad(fragment_unnamed_525, fragment_unnamed_571, fragment_unnamed_581);
						fragment_unnamed_588 = mad(fragment_unnamed_526, fragment_unnamed_571, fragment_unnamed_582);
					}
					else
					{
						fragment_unnamed_586 = fragment_unnamed_207;
						fragment_unnamed_587 = fragment_unnamed_208;
						fragment_unnamed_588 = fragment_unnamed_209;
					}
					float4 fragment_unnamed_590 = unity_SpecCube1.SampleLevel(samplerunity_SpecCube0, float3(fragment_unnamed_586, fragment_unnamed_587, fragment_unnamed_588), 6.0f);
					precise float fragment_unnamed_596 = fragment_unnamed_590.w + (-1.0f);
					precise float fragment_unnamed_606 = log2(mad(fragment_uniform_buffer_4[7u].w, fragment_unnamed_596, 1.0f)) * fragment_uniform_buffer_4[7u].y;
					precise float fragment_unnamed_611 = exp2(fragment_unnamed_606) * fragment_uniform_buffer_4[7u].x;
					precise float fragment_unnamed_612 = fragment_unnamed_590.x * fragment_unnamed_611;
					precise float fragment_unnamed_613 = fragment_unnamed_590.y * fragment_unnamed_611;
					precise float fragment_unnamed_614 = fragment_unnamed_590.z * fragment_unnamed_611;
					precise float fragment_unnamed_615 = (-0.0f) - fragment_unnamed_612;
					precise float fragment_unnamed_616 = (-0.0f) - fragment_unnamed_613;
					precise float fragment_unnamed_617 = (-0.0f) - fragment_unnamed_614;
					fragment_unnamed_364 = mad(fragment_uniform_buffer_4[1u].w, mad(fragment_unnamed_351, fragment_unnamed_333, fragment_unnamed_615), fragment_unnamed_612);
					fragment_unnamed_366 = mad(fragment_uniform_buffer_4[1u].w, mad(fragment_unnamed_351, fragment_unnamed_334, fragment_unnamed_616), fragment_unnamed_613);
					fragment_unnamed_368 = mad(fragment_uniform_buffer_4[1u].w, mad(fragment_unnamed_351, fragment_unnamed_335, fragment_unnamed_617), fragment_unnamed_614);
				}
				else
				{
					fragment_unnamed_364 = fragment_unnamed_352;
					fragment_unnamed_366 = fragment_unnamed_353;
					fragment_unnamed_368 = fragment_unnamed_354;
				}
				precise float fragment_unnamed_370 = fragment_unnamed_131 * 0.959999978542327880859375f;
				precise float fragment_unnamed_372 = fragment_unnamed_132 * 0.959999978542327880859375f;
				precise float fragment_unnamed_373 = fragment_unnamed_133 * 0.959999978542327880859375f;
				float fragment_unnamed_379 = mad(fragment_unnamed_90, fragment_unnamed_97, fragment_uniform_buffer_2[0u].x);
				float fragment_unnamed_380 = mad(fragment_unnamed_91, fragment_unnamed_97, fragment_uniform_buffer_2[0u].y);
				float fragment_unnamed_381 = mad(fragment_unnamed_92, fragment_unnamed_97, fragment_uniform_buffer_2[0u].z);
				float fragment_unnamed_387 = rsqrt(max(dot(float3(fragment_unnamed_379, fragment_unnamed_380, fragment_unnamed_381), float3(fragment_unnamed_379, fragment_unnamed_380, fragment_unnamed_381)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_388 = fragment_unnamed_387 * fragment_unnamed_379;
				precise float fragment_unnamed_389 = fragment_unnamed_387 * fragment_unnamed_380;
				precise float fragment_unnamed_390 = fragment_unnamed_387 * fragment_unnamed_381;
				float fragment_unnamed_391 = dot(float3(fragment_unnamed_193, fragment_unnamed_194, fragment_unnamed_195), float3(fragment_unnamed_98, fragment_unnamed_99, fragment_unnamed_100));
				float fragment_unnamed_402 = clamp(dot(float3(fragment_unnamed_193, fragment_unnamed_194, fragment_unnamed_195), float3(fragment_uniform_buffer_2[0u].xyz)), 0.0f, 1.0f);
				float fragment_unnamed_411 = clamp(dot(float3(fragment_uniform_buffer_2[0u].xyz), float3(fragment_unnamed_388, fragment_unnamed_389, fragment_unnamed_390)), 0.0f, 1.0f);
				precise float fragment_unnamed_415 = dot(fragment_unnamed_411.xx, fragment_unnamed_411.xx) + (-0.5f);
				precise float fragment_unnamed_417 = (-0.0f) - fragment_unnamed_402;
				precise float fragment_unnamed_418 = fragment_unnamed_417 + 1.0f;
				precise float fragment_unnamed_419 = fragment_unnamed_418 * fragment_unnamed_418;
				precise float fragment_unnamed_420 = fragment_unnamed_419 * fragment_unnamed_419;
				precise float fragment_unnamed_421 = fragment_unnamed_418 * fragment_unnamed_420;
				precise float fragment_unnamed_424 = (-0.0f) - abs(fragment_unnamed_391);
				precise float fragment_unnamed_425 = fragment_unnamed_424 + 1.0f;
				precise float fragment_unnamed_426 = fragment_unnamed_425 * fragment_unnamed_425;
				precise float fragment_unnamed_427 = fragment_unnamed_426 * fragment_unnamed_426;
				precise float fragment_unnamed_428 = fragment_unnamed_425 * fragment_unnamed_427;
				precise float fragment_unnamed_430 = mad(fragment_unnamed_415, fragment_unnamed_428, 1.0f) * mad(fragment_unnamed_415, fragment_unnamed_421, 1.0f);
				precise float fragment_unnamed_431 = fragment_unnamed_402 * fragment_unnamed_430;
				precise float fragment_unnamed_433 = abs(fragment_unnamed_391) + fragment_unnamed_402;
				precise float fragment_unnamed_434 = fragment_unnamed_433 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_436 = 0.5f / fragment_unnamed_434;
				precise float fragment_unnamed_438 = fragment_unnamed_436 * 0.99999988079071044921875f;
				precise float fragment_unnamed_440 = fragment_unnamed_402 * fragment_unnamed_438;
				precise float fragment_unnamed_454 = fragment_unnamed_440 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_455 = fragment_unnamed_440 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_456 = fragment_unnamed_440 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_457 = (-0.0f) - fragment_unnamed_411;
				precise float fragment_unnamed_458 = fragment_unnamed_457 + 1.0f;
				precise float fragment_unnamed_459 = fragment_unnamed_458 * fragment_unnamed_458;
				precise float fragment_unnamed_460 = fragment_unnamed_459 * fragment_unnamed_459;
				precise float fragment_unnamed_461 = fragment_unnamed_458 * fragment_unnamed_460;
				float fragment_unnamed_462 = mad(fragment_unnamed_461, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_464 = fragment_unnamed_462 * fragment_unnamed_454;
				precise float fragment_unnamed_465 = fragment_unnamed_462 * fragment_unnamed_455;
				precise float fragment_unnamed_466 = fragment_unnamed_462 * fragment_unnamed_456;
				precise float fragment_unnamed_470 = fragment_unnamed_364 * 0.5f;
				precise float fragment_unnamed_471 = fragment_unnamed_366 * 0.5f;
				precise float fragment_unnamed_472 = fragment_unnamed_368 * 0.5f;
				float fragment_unnamed_473 = mad(fragment_unnamed_428, 2.2351741790771484375e-08f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_482 = fragment_input_1 / fragment_uniform_buffer_1[5u].y;
				precise float fragment_unnamed_483 = (-0.0f) - fragment_unnamed_482;
				precise float fragment_unnamed_484 = fragment_unnamed_483 + 1.0f;
				precise float fragment_unnamed_488 = fragment_unnamed_484 * fragment_uniform_buffer_1[5u].z;
				float fragment_unnamed_497 = clamp(mad(max(fragment_unnamed_488, 0.0f), fragment_uniform_buffer_3[1u].z, fragment_uniform_buffer_3[1u].w), 0.0f, 1.0f);
				precise float fragment_unnamed_501 = (-0.0f) - fragment_uniform_buffer_3[0u].x;
				precise float fragment_unnamed_503 = (-0.0f) - fragment_uniform_buffer_3[0u].y;
				precise float fragment_unnamed_505 = (-0.0f) - fragment_uniform_buffer_3[0u].z;
				precise float fragment_unnamed_506 = mad(fragment_unnamed_470, fragment_unnamed_473, mad(fragment_unnamed_370, mad(fragment_uniform_buffer_0[2u].x, fragment_unnamed_431, max(fragment_unnamed_248, 0.0f)), fragment_unnamed_464)) + fragment_unnamed_501;
				precise float fragment_unnamed_507 = mad(fragment_unnamed_471, fragment_unnamed_473, mad(fragment_unnamed_372, mad(fragment_uniform_buffer_0[2u].y, fragment_unnamed_431, max(fragment_unnamed_249, 0.0f)), fragment_unnamed_465)) + fragment_unnamed_503;
				precise float fragment_unnamed_508 = mad(fragment_unnamed_472, fragment_unnamed_473, mad(fragment_unnamed_373, mad(fragment_uniform_buffer_0[2u].z, fragment_unnamed_431, max(fragment_unnamed_250, 0.0f)), fragment_unnamed_466)) + fragment_unnamed_505;
				fragment_output_0.x = mad(fragment_unnamed_497, fragment_unnamed_506, fragment_uniform_buffer_3[0u].x);
				fragment_output_0.y = mad(fragment_unnamed_497, fragment_unnamed_507, fragment_uniform_buffer_3[0u].y);
				fragment_output_0.z = mad(fragment_unnamed_497, fragment_unnamed_508, fragment_uniform_buffer_3[0u].z);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_1[5] = float4(_ProjectionParams[0], _ProjectionParams[1], _ProjectionParams[2], _ProjectionParams[3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_uniform_buffer_2[39] = float4(unity_SHAr[0], unity_SHAr[1], unity_SHAr[2], unity_SHAr[3]);

				fragment_uniform_buffer_2[40] = float4(unity_SHAg[0], unity_SHAg[1], unity_SHAg[2], unity_SHAg[3]);

				fragment_uniform_buffer_2[41] = float4(unity_SHAb[0], unity_SHAb[1], unity_SHAb[2], unity_SHAb[3]);

				fragment_uniform_buffer_3[0] = float4(unity_FogColor[0], unity_FogColor[1], unity_FogColor[2], unity_FogColor[3]);

				fragment_uniform_buffer_3[1] = float4(unity_FogParams[0], unity_FogParams[1], unity_FogParams[2], unity_FogParams[3]);

				fragment_uniform_buffer_4[0] = float4(unity_SpecCube0_BoxMax[0], unity_SpecCube0_BoxMax[1], unity_SpecCube0_BoxMax[2], unity_SpecCube0_BoxMax[3]);

				fragment_uniform_buffer_4[1] = float4(unity_SpecCube0_BoxMin[0], unity_SpecCube0_BoxMin[1], unity_SpecCube0_BoxMin[2], unity_SpecCube0_BoxMin[3]);

				fragment_uniform_buffer_4[2] = float4(unity_SpecCube0_ProbePosition[0], unity_SpecCube0_ProbePosition[1], unity_SpecCube0_ProbePosition[2], unity_SpecCube0_ProbePosition[3]);

				fragment_uniform_buffer_4[3] = float4(unity_SpecCube0_HDR[0], unity_SpecCube0_HDR[1], unity_SpecCube0_HDR[2], unity_SpecCube0_HDR[3]);

				fragment_uniform_buffer_4[4] = float4(unity_SpecCube1_BoxMax[0], unity_SpecCube1_BoxMax[1], unity_SpecCube1_BoxMax[2], unity_SpecCube1_BoxMax[3]);

				fragment_uniform_buffer_4[5] = float4(unity_SpecCube1_BoxMin[0], unity_SpecCube1_BoxMin[1], unity_SpecCube1_BoxMin[2], unity_SpecCube1_BoxMin[3]);

				fragment_uniform_buffer_4[6] = float4(unity_SpecCube1_ProbePosition[0], unity_SpecCube1_ProbePosition[1], unity_SpecCube1_ProbePosition[2], unity_SpecCube1_ProbePosition[3]);

				fragment_uniform_buffer_4[7] = float4(unity_SpecCube1_HDR[0], unity_SpecCube1_HDR[1], unity_SpecCube1_HDR[2], unity_SpecCube1_HDR[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				fragment_input_8 = stage_input.fragment_input_8;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // LIGHTPROBE_SH
			#endif // !VERTEXLIGHT_ON


			// Fallback Shader Code
			#ifndef ANY_SHADER_VARIANT_ACTIVE

			// https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
			float4x4 unity_MatrixMVP;

			struct Vertex_Stage_Input
			{
				float3 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixMVP, float4(input.pos, 1.0));
				return output;
			}

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				// Output solid grey color (e.g., 50% grey)
				return float4(0.5, 0.5, 0.5, 1.0); // RGBA
			}

			#endif // !ANY_SHADER_VARIANT_ACTIVE


			ENDHLSL
		}
		Pass
		{
			Name "FORWARD"
			LOD 200
			Tags { "LIGHTMODE" = "FORWARDADD" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha One, SrcAlpha One
			ColorMask RGB
			ZWrite Off
			GpuProgramID 66494

			HLSLPROGRAM

			// https://docs.unity3d.com/Manual/SL-PragmaDirectives.html
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			#pragma shader_feature DIRECTIONAL
			#pragma shader_feature DIRECTIONAL_COOKIE
			#pragma multi_compile _ FOG_LINEAR
			#pragma shader_feature POINT
			#pragma shader_feature POINT_COOKIE
			#pragma shader_feature SPOT


			#ifdef POINT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[10];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;
			static float3 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float3 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float3 vertex_output_5 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_6 : COLOR; // COLOR
				float4 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float3 vertex_output_8 : TEXCOORD6; // TEXCOORD_6
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_58 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_59 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_61 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_84 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_58));
				float vertex_unnamed_85 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_59));
				float vertex_unnamed_86 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_60));
				float vertex_unnamed_87 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_61));
				precise float vertex_unnamed_95 = vertex_unnamed_84 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_96 = vertex_unnamed_85 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_97 = vertex_unnamed_86 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_98 = vertex_unnamed_87 + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_106 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_107 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_108 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_109 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_95, vertex_unnamed_106)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_95, vertex_unnamed_107)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_95, vertex_unnamed_108)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_95, vertex_unnamed_109)));
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[9u].x, vertex_uniform_buffer_0[9u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[9u].y, vertex_uniform_buffer_0[9u].w);
				float vertex_unnamed_177 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_192 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_207 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_213 = rsqrt(dot(float3(vertex_unnamed_207, vertex_unnamed_177, vertex_unnamed_192), float3(vertex_unnamed_207, vertex_unnamed_177, vertex_unnamed_192)));
				precise float vertex_unnamed_214 = vertex_unnamed_213 * vertex_unnamed_207;
				precise float vertex_unnamed_215 = vertex_unnamed_213 * vertex_unnamed_177;
				precise float vertex_unnamed_216 = vertex_unnamed_213 * vertex_unnamed_192;
				precise float vertex_unnamed_224 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_225 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_226 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_244 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_224));
				float vertex_unnamed_245 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_225));
				float vertex_unnamed_246 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_226));
				float vertex_unnamed_250 = rsqrt(dot(float3(vertex_unnamed_244, vertex_unnamed_245, vertex_unnamed_246), float3(vertex_unnamed_244, vertex_unnamed_245, vertex_unnamed_246)));
				precise float vertex_unnamed_251 = vertex_unnamed_250 * vertex_unnamed_244;
				precise float vertex_unnamed_252 = vertex_unnamed_250 * vertex_unnamed_245;
				precise float vertex_unnamed_253 = vertex_unnamed_250 * vertex_unnamed_246;
				precise float vertex_unnamed_254 = vertex_unnamed_214 * vertex_unnamed_251;
				precise float vertex_unnamed_255 = vertex_unnamed_215 * vertex_unnamed_252;
				precise float vertex_unnamed_256 = vertex_unnamed_216 * vertex_unnamed_253;
				precise float vertex_unnamed_257 = (-0.0f) - vertex_unnamed_254;
				precise float vertex_unnamed_259 = (-0.0f) - vertex_unnamed_255;
				precise float vertex_unnamed_260 = (-0.0f) - vertex_unnamed_256;
				precise float vertex_unnamed_269 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_270 = vertex_unnamed_269 * mad(vertex_unnamed_216, vertex_unnamed_252, vertex_unnamed_257);
				precise float vertex_unnamed_271 = vertex_unnamed_269 * mad(vertex_unnamed_214, vertex_unnamed_253, vertex_unnamed_259);
				precise float vertex_unnamed_272 = vertex_unnamed_269 * mad(vertex_unnamed_215, vertex_unnamed_251, vertex_unnamed_260);
				vertex_output_2.y = vertex_unnamed_270;
				vertex_output_2.x = vertex_unnamed_253;
				vertex_output_2.z = vertex_unnamed_215;
				vertex_output_3.x = vertex_unnamed_251;
				vertex_output_4.x = vertex_unnamed_252;
				vertex_output_3.z = vertex_unnamed_216;
				vertex_output_4.z = vertex_unnamed_214;
				vertex_output_3.y = vertex_unnamed_271;
				vertex_output_4.y = vertex_unnamed_272;
				vertex_output_5.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_84);
				vertex_output_5.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_85);
				vertex_output_5.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_86);
				float vertex_unnamed_303 = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_84);
				float vertex_unnamed_304 = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_85);
				float vertex_unnamed_305 = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_86);
				float vertex_unnamed_306 = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_87);
				vertex_output_6.x = vertex_input_7.x;
				vertex_output_6.y = vertex_input_7.y;
				vertex_output_6.z = vertex_input_7.z;
				vertex_output_6.w = vertex_input_7.w;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
				precise float vertex_unnamed_329 = vertex_unnamed_304 * vertex_uniform_buffer_0[5u].x;
				precise float vertex_unnamed_330 = vertex_unnamed_304 * vertex_uniform_buffer_0[5u].y;
				precise float vertex_unnamed_331 = vertex_unnamed_304 * vertex_uniform_buffer_0[5u].z;
				vertex_output_8.x = mad(vertex_uniform_buffer_0[7u].x, vertex_unnamed_306, mad(vertex_uniform_buffer_0[6u].x, vertex_unnamed_305, mad(vertex_uniform_buffer_0[4u].x, vertex_unnamed_303, vertex_unnamed_329)));
				vertex_output_8.y = mad(vertex_uniform_buffer_0[7u].y, vertex_unnamed_306, mad(vertex_uniform_buffer_0[6u].y, vertex_unnamed_305, mad(vertex_uniform_buffer_0[4u].y, vertex_unnamed_303, vertex_unnamed_330)));
				vertex_output_8.z = mad(vertex_uniform_buffer_0[7u].z, vertex_unnamed_306, mad(vertex_uniform_buffer_0[6u].z, vertex_unnamed_305, mad(vertex_uniform_buffer_0[4u].z, vertex_unnamed_303, vertex_unnamed_331)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_uniform_buffer_0[9] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			#endif // POINT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef POINT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 unity_WorldToLight__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float3 vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_input_4;
			static float4 vertex_output_6;
			static float3 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 vertex_output_1 : TEXCOORD1; // vs_TEXCOORD1
				float3 vertex_output_2 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_3 : TEXCOORD3; // vs_TEXCOORD3
				float3 vertex_output_4 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_5 : UNKNOWN5;
				float4 vertex_output_6 : TEXCOORD5; // vs_TEXCOORD5
				float3 vertex_output_7 : TEXCOORD6; // vs_TEXCOORD6
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_45;
			static float4 vertex_unnamed_51;
			static float vertex_unnamed_124;
			static float3 vertex_unnamed_185;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_45 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_unnamed_51 = vertex_unnamed_45.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_51 = (unity_MatrixVP__array[0] * vertex_unnamed_45.xxxx) + vertex_unnamed_51;
				vertex_unnamed_51 = (unity_MatrixVP__array[2] * vertex_unnamed_45.zzzz) + vertex_unnamed_51;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_45.wwww) + vertex_unnamed_51;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_45.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_45.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_45.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_124 = dot(vertex_unnamed_45.xyz, vertex_unnamed_45.xyz);
				vertex_unnamed_124 = rsqrt(vertex_unnamed_124);
				float3 vertex_unnamed_136 = vertex_unnamed_124.xxx * vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_136.x, vertex_unnamed_136.y, vertex_unnamed_136.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_145 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_51 = float4(vertex_unnamed_145.x, vertex_unnamed_145.y, vertex_unnamed_145.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_156 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_156.x, vertex_unnamed_156.y, vertex_unnamed_156.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_167 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_167.x, vertex_unnamed_167.y, vertex_unnamed_167.z, vertex_unnamed_51.w);
				vertex_unnamed_124 = dot(vertex_unnamed_51.xyz, vertex_unnamed_51.xyz);
				vertex_unnamed_124 = rsqrt(vertex_unnamed_124);
				float3 vertex_unnamed_181 = vertex_unnamed_124.xxx * vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_181.x, vertex_unnamed_181.y, vertex_unnamed_181.z, vertex_unnamed_51.w);
				vertex_unnamed_185 = vertex_unnamed_45.xyz * vertex_unnamed_51.xyz;
				vertex_unnamed_185 = (vertex_unnamed_45.zxy * vertex_unnamed_51.yzx) + (-vertex_unnamed_185);
				vertex_unnamed_124 = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_185 = vertex_unnamed_124.xxx * vertex_unnamed_185;
				vertex_output_1.y = vertex_unnamed_185.x;
				vertex_output_1.x = vertex_unnamed_51.z;
				vertex_output_1.z = vertex_unnamed_45.y;
				vertex_output_2.x = vertex_unnamed_51.x;
				vertex_output_3.x = vertex_unnamed_51.y;
				vertex_output_2.z = vertex_unnamed_45.z;
				vertex_output_3.z = vertex_unnamed_45.x;
				vertex_output_2.y = vertex_unnamed_185.y;
				vertex_output_3.y = vertex_unnamed_185.z;
				vertex_output_4 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_9;
				vertex_output_5 = vertex_input_4;
				vertex_output_6 = 0.0f.xxxx;
				float3 vertex_unnamed_272 = vertex_unnamed_9.yyy * unity_WorldToLight__array[1].xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_272.x, vertex_unnamed_272.y, vertex_unnamed_272.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_283 = (unity_WorldToLight__array[0].xyz * vertex_unnamed_9.xxx) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_283.x, vertex_unnamed_283.y, vertex_unnamed_283.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_294 = (unity_WorldToLight__array[2].xyz * vertex_unnamed_9.zzz) + vertex_unnamed_45.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_294.x, vertex_unnamed_294.y, vertex_unnamed_294.z, vertex_unnamed_9.w);
				vertex_output_7 = (unity_WorldToLight__array[3].xyz * vertex_unnamed_9.www) + vertex_unnamed_9.xyz;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;
			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;

			static float4 unity_WorldToLight__array[4];
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			Texture2D<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float3 fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float4 fragment_input_5;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 fragment_input_1 : TEXCOORD1; // vs_TEXCOORD1
				float3 fragment_input_2 : TEXCOORD2; // vs_TEXCOORD2
				float3 fragment_input_3 : TEXCOORD3; // vs_TEXCOORD3
				float3 fragment_input_4 : TEXCOORD4; // vs_TEXCOORD4
				float4 fragment_input_5 : UNKNOWN5;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float4 fragment_unnamed_9;
			static float fragment_unnamed_49;
			static float4 fragment_unnamed_64;
			static float fragment_unnamed_137;
			static float3 fragment_unnamed_143;
			static float fragment_unnamed_166;
			static float fragment_unnamed_221;
			static float fragment_unnamed_227;
			static float fragment_unnamed_240;
			static float fragment_unnamed_245;

			void frag_main()
			{
				float3 fragment_unnamed_26 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_26.x, fragment_unnamed_26.y, fragment_unnamed_26.z, fragment_unnamed_9.w);
				fragment_unnamed_9.x = fragment_unnamed_9.z * fragment_unnamed_9.x;
				float2 fragment_unnamed_46 = (fragment_unnamed_9.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_9 = float4(fragment_unnamed_46.x, fragment_unnamed_46.y, fragment_unnamed_9.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xy, fragment_unnamed_9.xy);
				fragment_unnamed_49 = min(fragment_unnamed_49, 1.0f);
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_9.z = sqrt(fragment_unnamed_49);
				fragment_unnamed_64.x = dot(fragment_input_1, fragment_unnamed_9.xyz);
				fragment_unnamed_64.y = dot(fragment_input_2, fragment_unnamed_9.xyz);
				fragment_unnamed_64.z = dot(fragment_input_3, fragment_unnamed_9.xyz);
				fragment_unnamed_9.x = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_9.x = rsqrt(fragment_unnamed_9.x);
				float3 fragment_unnamed_99 = fragment_unnamed_9.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_9 = float4(fragment_unnamed_99.x, fragment_unnamed_99.y, fragment_unnamed_99.z, fragment_unnamed_9.w);
				float3 fragment_unnamed_115 = (-fragment_input_4) + _WorldSpaceCameraPos;
				fragment_unnamed_64 = float4(fragment_unnamed_115.x, fragment_unnamed_115.y, fragment_unnamed_115.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_49 = rsqrt(fragment_unnamed_49);
				float3 fragment_unnamed_129 = fragment_unnamed_49.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_129.x, fragment_unnamed_129.y, fragment_unnamed_129.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_137 = (-abs(fragment_unnamed_49)) + 1.0f;
				fragment_unnamed_143.x = fragment_unnamed_137 * fragment_unnamed_137;
				fragment_unnamed_143.x *= fragment_unnamed_143.x;
				fragment_unnamed_137 *= fragment_unnamed_143.x;
				fragment_unnamed_143 = (-fragment_input_4) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_166 = dot(fragment_unnamed_143, fragment_unnamed_143);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_178 = (fragment_unnamed_143 * fragment_unnamed_166.xxx) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_178.x, fragment_unnamed_178.y, fragment_unnamed_178.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_166.xxx * fragment_unnamed_143;
				fragment_unnamed_166 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_166 = max(fragment_unnamed_166, 0.001000000047497451305389404296875f);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_199 = fragment_unnamed_64.xyz * fragment_unnamed_166.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_199.x, fragment_unnamed_199.y, fragment_unnamed_199.z, fragment_unnamed_64.w);
				fragment_unnamed_64.x = dot(fragment_unnamed_143, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = clamp(fragment_unnamed_64.x, 0.0f, 1.0f);
				fragment_unnamed_9.x = dot(fragment_unnamed_9.xyz, fragment_unnamed_143);
				fragment_unnamed_9.x = clamp(fragment_unnamed_9.x, 0.0f, 1.0f);
				fragment_unnamed_221 = dot(fragment_unnamed_64.xx, fragment_unnamed_64.xx);
				fragment_unnamed_227 = (-fragment_unnamed_64.x) + 1.0f;
				fragment_unnamed_221 += (-0.5f);
				fragment_unnamed_64.x = (fragment_unnamed_221 * fragment_unnamed_137) + 1.0f;
				fragment_unnamed_240 = (-fragment_unnamed_9.x) + 1.0f;
				fragment_unnamed_245 = fragment_unnamed_240 * fragment_unnamed_240;
				fragment_unnamed_245 *= fragment_unnamed_245;
				fragment_unnamed_240 *= fragment_unnamed_245;
				fragment_unnamed_221 = (fragment_unnamed_221 * fragment_unnamed_240) + 1.0f;
				fragment_unnamed_221 = fragment_unnamed_64.x * fragment_unnamed_221;
				fragment_unnamed_221 = fragment_unnamed_9.x * fragment_unnamed_221;
				float3 fragment_unnamed_273 = fragment_input_4.yyy * unity_WorldToLight__array[1].xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_273.x, fragment_unnamed_273.y, fragment_unnamed_273.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_284 = (unity_WorldToLight__array[0].xyz * fragment_input_4.xxx) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_284.x, fragment_unnamed_284.y, fragment_unnamed_284.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_296 = (unity_WorldToLight__array[2].xyz * fragment_input_4.zzz) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_296.x, fragment_unnamed_296.y, fragment_unnamed_296.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_304 = fragment_unnamed_64.xyz + unity_WorldToLight__array[3].xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_304.x, fragment_unnamed_304.y, fragment_unnamed_304.z, fragment_unnamed_64.w);
				fragment_unnamed_64.x = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = _LightTexture0.Sample(sampler_LightTexture0, fragment_unnamed_64.xx).x;
				float3 fragment_unnamed_328 = fragment_unnamed_64.xxx * _LightColor0.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_328.x, fragment_unnamed_328.y, fragment_unnamed_328.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_221.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_221 = abs(fragment_unnamed_49) + fragment_unnamed_9.x;
				fragment_unnamed_221 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_221 = 0.5f / fragment_unnamed_221;
				fragment_unnamed_9.x *= fragment_unnamed_221;
				fragment_unnamed_9.x *= 0.99999988079071044921875f;
				float3 fragment_unnamed_361 = fragment_unnamed_64.xyz * fragment_unnamed_9.xxx;
				fragment_unnamed_9 = float4(fragment_unnamed_361.x, fragment_unnamed_361.y, fragment_unnamed_9.z, fragment_unnamed_361.z);
				fragment_unnamed_64.x = fragment_unnamed_227 * fragment_unnamed_227;
				fragment_unnamed_64.x *= fragment_unnamed_64.x;
				fragment_unnamed_227 *= fragment_unnamed_64.x;
				fragment_unnamed_227 = (fragment_unnamed_227 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				float3 fragment_unnamed_387 = fragment_unnamed_227.xxx * fragment_unnamed_9.xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_387.x, fragment_unnamed_387.y, fragment_unnamed_387.z, fragment_unnamed_9.w);
				fragment_unnamed_64 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_64 *= _Color;
				float3 fragment_unnamed_408 = fragment_unnamed_64.xyz * fragment_input_5.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_408.x, fragment_unnamed_408.y, fragment_unnamed_408.z, fragment_unnamed_64.w);
				fragment_output_0.w = fragment_unnamed_64.w * fragment_input_5.w;
				float3 fragment_unnamed_425 = fragment_unnamed_64.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_425.x, fragment_unnamed_425.y, fragment_unnamed_425.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_434 = (fragment_unnamed_64.xyz * fragment_unnamed_143) + fragment_unnamed_9.xyz;
				fragment_output_0 = float4(fragment_unnamed_434.x, fragment_unnamed_434.y, fragment_unnamed_434.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // POINT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[6];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float3 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float3 vertex_output_5 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_6 : COLOR; // COLOR
				float4 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_58 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_59 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_61 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_84 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_58));
				float vertex_unnamed_85 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_59));
				float vertex_unnamed_86 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_60));
				precise float vertex_unnamed_95 = vertex_unnamed_84 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_96 = vertex_unnamed_85 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_97 = vertex_unnamed_86 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_98 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_61)) + vertex_uniform_buffer_1[3u].w;
				vertex_output_5.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_84);
				vertex_output_5.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_85);
				vertex_output_5.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_86);
				precise float vertex_unnamed_120 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_121 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_122 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_123 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_95, vertex_unnamed_120)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_95, vertex_unnamed_121)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_95, vertex_unnamed_122)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_95, vertex_unnamed_123)));
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[5u].x, vertex_uniform_buffer_0[5u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[5u].y, vertex_uniform_buffer_0[5u].w);
				float vertex_unnamed_190 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_204 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_218 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_224 = rsqrt(dot(float3(vertex_unnamed_218, vertex_unnamed_190, vertex_unnamed_204), float3(vertex_unnamed_218, vertex_unnamed_190, vertex_unnamed_204)));
				precise float vertex_unnamed_225 = vertex_unnamed_224 * vertex_unnamed_218;
				precise float vertex_unnamed_226 = vertex_unnamed_224 * vertex_unnamed_190;
				precise float vertex_unnamed_227 = vertex_unnamed_224 * vertex_unnamed_204;
				precise float vertex_unnamed_235 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_236 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_237 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_255 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_235));
				float vertex_unnamed_256 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_236));
				float vertex_unnamed_257 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_237));
				float vertex_unnamed_261 = rsqrt(dot(float3(vertex_unnamed_255, vertex_unnamed_256, vertex_unnamed_257), float3(vertex_unnamed_255, vertex_unnamed_256, vertex_unnamed_257)));
				precise float vertex_unnamed_262 = vertex_unnamed_261 * vertex_unnamed_255;
				precise float vertex_unnamed_263 = vertex_unnamed_261 * vertex_unnamed_256;
				precise float vertex_unnamed_264 = vertex_unnamed_261 * vertex_unnamed_257;
				precise float vertex_unnamed_265 = vertex_unnamed_225 * vertex_unnamed_262;
				precise float vertex_unnamed_266 = vertex_unnamed_226 * vertex_unnamed_263;
				precise float vertex_unnamed_267 = vertex_unnamed_227 * vertex_unnamed_264;
				precise float vertex_unnamed_268 = (-0.0f) - vertex_unnamed_265;
				precise float vertex_unnamed_270 = (-0.0f) - vertex_unnamed_266;
				precise float vertex_unnamed_271 = (-0.0f) - vertex_unnamed_267;
				precise float vertex_unnamed_281 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_282 = vertex_unnamed_281 * mad(vertex_unnamed_227, vertex_unnamed_263, vertex_unnamed_268);
				precise float vertex_unnamed_283 = vertex_unnamed_281 * mad(vertex_unnamed_225, vertex_unnamed_264, vertex_unnamed_270);
				precise float vertex_unnamed_284 = vertex_unnamed_281 * mad(vertex_unnamed_226, vertex_unnamed_262, vertex_unnamed_271);
				vertex_output_2.y = vertex_unnamed_282;
				vertex_output_2.x = vertex_unnamed_264;
				vertex_output_2.z = vertex_unnamed_226;
				vertex_output_3.x = vertex_unnamed_262;
				vertex_output_4.x = vertex_unnamed_263;
				vertex_output_3.z = vertex_unnamed_227;
				vertex_output_4.z = vertex_unnamed_225;
				vertex_output_3.y = vertex_unnamed_283;
				vertex_output_4.y = vertex_unnamed_284;
				vertex_output_6.x = vertex_input_7.x;
				vertex_output_6.y = vertex_input_7.y;
				vertex_output_6.z = vertex_input_7.z;
				vertex_output_6.w = vertex_input_7.w;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[5] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float3 vertex_output_4;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float3 vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float4 vertex_output_5;
			static float4 vertex_input_4;
			static float4 vertex_output_6;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 vertex_output_1 : TEXCOORD1; // vs_TEXCOORD1
				float3 vertex_output_2 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_3 : TEXCOORD3; // vs_TEXCOORD3
				float3 vertex_output_4 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_5 : UNKNOWN5;
				float4 vertex_output_6 : TEXCOORD5; // vs_TEXCOORD5
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_44;
			static float vertex_unnamed_133;
			static float3 vertex_unnamed_194;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_44 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_output_4 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = vertex_unnamed_44.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_9 = (unity_MatrixVP__array[0] * vertex_unnamed_44.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_MatrixVP__array[2] * vertex_unnamed_44.zzzz) + vertex_unnamed_9;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_44.wwww) + vertex_unnamed_9;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_9.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_9.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_9.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_133 = dot(vertex_unnamed_9.xyz, vertex_unnamed_9.xyz);
				vertex_unnamed_133 = rsqrt(vertex_unnamed_133);
				float3 vertex_unnamed_145 = vertex_unnamed_133.xxx * vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_145.x, vertex_unnamed_145.y, vertex_unnamed_145.z, vertex_unnamed_9.w);
				float3 vertex_unnamed_154 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_44 = float4(vertex_unnamed_154.x, vertex_unnamed_154.y, vertex_unnamed_154.z, vertex_unnamed_44.w);
				float3 vertex_unnamed_165 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_44.xyz;
				vertex_unnamed_44 = float4(vertex_unnamed_165.x, vertex_unnamed_165.y, vertex_unnamed_165.z, vertex_unnamed_44.w);
				float3 vertex_unnamed_176 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_44.xyz;
				vertex_unnamed_44 = float4(vertex_unnamed_176.x, vertex_unnamed_176.y, vertex_unnamed_176.z, vertex_unnamed_44.w);
				vertex_unnamed_133 = dot(vertex_unnamed_44.xyz, vertex_unnamed_44.xyz);
				vertex_unnamed_133 = rsqrt(vertex_unnamed_133);
				float3 vertex_unnamed_190 = vertex_unnamed_133.xxx * vertex_unnamed_44.xyz;
				vertex_unnamed_44 = float4(vertex_unnamed_190.x, vertex_unnamed_190.y, vertex_unnamed_190.z, vertex_unnamed_44.w);
				vertex_unnamed_194 = vertex_unnamed_9.xyz * vertex_unnamed_44.xyz;
				vertex_unnamed_194 = (vertex_unnamed_9.zxy * vertex_unnamed_44.yzx) + (-vertex_unnamed_194);
				vertex_unnamed_133 = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_194 = vertex_unnamed_133.xxx * vertex_unnamed_194;
				vertex_output_1.y = vertex_unnamed_194.x;
				vertex_output_1.x = vertex_unnamed_44.z;
				vertex_output_1.z = vertex_unnamed_9.y;
				vertex_output_2.x = vertex_unnamed_44.x;
				vertex_output_3.x = vertex_unnamed_44.y;
				vertex_output_2.z = vertex_unnamed_9.z;
				vertex_output_3.z = vertex_unnamed_9.x;
				vertex_output_2.y = vertex_unnamed_194.y;
				vertex_output_3.y = vertex_unnamed_194.z;
				vertex_output_5 = vertex_input_4;
				vertex_output_6 = 0.0f.xxxx;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;
			float4 _LightColor0;
			float4 _Color;

			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float3 fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float4 fragment_input_5;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 fragment_input_1 : TEXCOORD1; // vs_TEXCOORD1
				float3 fragment_input_2 : TEXCOORD2; // vs_TEXCOORD2
				float3 fragment_input_3 : TEXCOORD3; // vs_TEXCOORD3
				float3 fragment_input_4 : TEXCOORD4; // vs_TEXCOORD4
				float4 fragment_input_5 : UNKNOWN5;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float4 fragment_unnamed_9;
			static float fragment_unnamed_49;
			static float3 fragment_unnamed_65;
			static float4 fragment_unnamed_117;
			static float fragment_unnamed_151;
			static float fragment_unnamed_156;
			static float fragment_unnamed_232;

			void frag_main()
			{
				float3 fragment_unnamed_26 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_26.x, fragment_unnamed_26.y, fragment_unnamed_26.z, fragment_unnamed_9.w);
				fragment_unnamed_9.x = fragment_unnamed_9.z * fragment_unnamed_9.x;
				float2 fragment_unnamed_46 = (fragment_unnamed_9.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_9 = float4(fragment_unnamed_46.x, fragment_unnamed_46.y, fragment_unnamed_9.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xy, fragment_unnamed_9.xy);
				fragment_unnamed_49 = min(fragment_unnamed_49, 1.0f);
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_9.z = sqrt(fragment_unnamed_49);
				fragment_unnamed_65.x = dot(fragment_input_1, fragment_unnamed_9.xyz);
				fragment_unnamed_65.y = dot(fragment_input_2, fragment_unnamed_9.xyz);
				fragment_unnamed_65.z = dot(fragment_input_3, fragment_unnamed_9.xyz);
				fragment_unnamed_9.x = dot(fragment_unnamed_65, fragment_unnamed_65);
				fragment_unnamed_9.x = rsqrt(fragment_unnamed_9.x);
				float3 fragment_unnamed_97 = fragment_unnamed_9.xxx * fragment_unnamed_65;
				fragment_unnamed_9 = float4(fragment_unnamed_97.x, fragment_unnamed_97.y, fragment_unnamed_97.z, fragment_unnamed_9.w);
				fragment_unnamed_65 = (-fragment_input_4) + _WorldSpaceCameraPos;
				fragment_unnamed_49 = dot(fragment_unnamed_65, fragment_unnamed_65);
				fragment_unnamed_49 = rsqrt(fragment_unnamed_49);
				float3 fragment_unnamed_121 = fragment_unnamed_49.xxx * fragment_unnamed_65;
				fragment_unnamed_117 = float4(fragment_unnamed_121.x, fragment_unnamed_121.y, fragment_unnamed_121.z, fragment_unnamed_117.w);
				fragment_unnamed_65 = (fragment_unnamed_65 * fragment_unnamed_49.xxx) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_49 = dot(fragment_unnamed_9.xyz, fragment_unnamed_117.xyz);
				fragment_unnamed_9.x = dot(fragment_unnamed_9.xyz, _WorldSpaceLightPos0.xyz);
				fragment_unnamed_9.x = clamp(fragment_unnamed_9.x, 0.0f, 1.0f);
				fragment_unnamed_151 = (-abs(fragment_unnamed_49)) + 1.0f;
				fragment_unnamed_156 = abs(fragment_unnamed_49) + fragment_unnamed_9.x;
				fragment_unnamed_156 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_156 = 0.5f / fragment_unnamed_156;
				fragment_unnamed_156 = fragment_unnamed_9.x * fragment_unnamed_156;
				fragment_unnamed_156 *= 0.99999988079071044921875f;
				float3 fragment_unnamed_181 = fragment_unnamed_156.xxx * _LightColor0.xyz;
				fragment_unnamed_117 = float4(fragment_unnamed_181.x, fragment_unnamed_181.y, fragment_unnamed_181.z, fragment_unnamed_117.w);
				fragment_unnamed_156 = fragment_unnamed_151 * fragment_unnamed_151;
				fragment_unnamed_156 *= fragment_unnamed_156;
				fragment_unnamed_151 *= fragment_unnamed_156;
				fragment_unnamed_156 = dot(fragment_unnamed_65, fragment_unnamed_65);
				fragment_unnamed_156 = max(fragment_unnamed_156, 0.001000000047497451305389404296875f);
				fragment_unnamed_156 = rsqrt(fragment_unnamed_156);
				fragment_unnamed_65 = fragment_unnamed_156.xxx * fragment_unnamed_65;
				fragment_unnamed_156 = dot(_WorldSpaceLightPos0.xyz, fragment_unnamed_65);
				fragment_unnamed_156 = clamp(fragment_unnamed_156, 0.0f, 1.0f);
				fragment_unnamed_49 = dot(fragment_unnamed_156.xx, fragment_unnamed_156.xx);
				fragment_unnamed_156 = (-fragment_unnamed_156) + 1.0f;
				fragment_unnamed_49 += (-0.5f);
				fragment_unnamed_151 = (fragment_unnamed_49 * fragment_unnamed_151) + 1.0f;
				fragment_unnamed_65.x = (-fragment_unnamed_9.x) + 1.0f;
				fragment_unnamed_232 = fragment_unnamed_65.x * fragment_unnamed_65.x;
				fragment_unnamed_232 *= fragment_unnamed_232;
				fragment_unnamed_65.x *= fragment_unnamed_232;
				fragment_unnamed_49 = (fragment_unnamed_49 * fragment_unnamed_65.x) + 1.0f;
				fragment_unnamed_151 *= fragment_unnamed_49;
				fragment_unnamed_9.x *= fragment_unnamed_151;
				float3 fragment_unnamed_264 = fragment_unnamed_9.xxx * _LightColor0.xyz;
				fragment_unnamed_9 = float4(fragment_unnamed_264.x, fragment_unnamed_264.y, fragment_unnamed_9.z, fragment_unnamed_264.z);
				fragment_unnamed_65.x = fragment_unnamed_156 * fragment_unnamed_156;
				fragment_unnamed_65.x *= fragment_unnamed_65.x;
				fragment_unnamed_156 *= fragment_unnamed_65.x;
				fragment_unnamed_156 = (fragment_unnamed_156 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				fragment_unnamed_65 = fragment_unnamed_156.xxx * fragment_unnamed_117.xyz;
				fragment_unnamed_117 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_117 *= _Color;
				float3 fragment_unnamed_309 = fragment_unnamed_117.xyz * fragment_input_5.xyz;
				fragment_unnamed_117 = float4(fragment_unnamed_309.x, fragment_unnamed_309.y, fragment_unnamed_309.z, fragment_unnamed_117.w);
				fragment_output_0.w = fragment_unnamed_117.w * fragment_input_5.w;
				float3 fragment_unnamed_326 = fragment_unnamed_117.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_117 = float4(fragment_unnamed_326.x, fragment_unnamed_326.y, fragment_unnamed_326.z, fragment_unnamed_117.w);
				float3 fragment_unnamed_335 = (fragment_unnamed_117.xyz * fragment_unnamed_9.xyw) + fragment_unnamed_65;
				fragment_output_0 = float4(fragment_unnamed_335.x, fragment_unnamed_335.y, fragment_unnamed_335.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef SPOT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef POINT_COOKIE
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[10];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;
			static float4 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float3 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float3 vertex_output_5 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_6 : COLOR; // COLOR
				float4 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float4 vertex_output_8 : TEXCOORD6; // TEXCOORD_6
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_58 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_59 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_61 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_84 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_58));
				float vertex_unnamed_85 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_59));
				float vertex_unnamed_86 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_60));
				float vertex_unnamed_87 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_61));
				precise float vertex_unnamed_95 = vertex_unnamed_84 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_96 = vertex_unnamed_85 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_97 = vertex_unnamed_86 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_98 = vertex_unnamed_87 + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_106 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_107 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_108 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_109 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_95, vertex_unnamed_106)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_95, vertex_unnamed_107)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_95, vertex_unnamed_108)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_95, vertex_unnamed_109)));
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[9u].x, vertex_uniform_buffer_0[9u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[9u].y, vertex_uniform_buffer_0[9u].w);
				float vertex_unnamed_177 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_192 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_207 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_213 = rsqrt(dot(float3(vertex_unnamed_207, vertex_unnamed_177, vertex_unnamed_192), float3(vertex_unnamed_207, vertex_unnamed_177, vertex_unnamed_192)));
				precise float vertex_unnamed_214 = vertex_unnamed_213 * vertex_unnamed_207;
				precise float vertex_unnamed_215 = vertex_unnamed_213 * vertex_unnamed_177;
				precise float vertex_unnamed_216 = vertex_unnamed_213 * vertex_unnamed_192;
				precise float vertex_unnamed_224 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_225 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_226 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_244 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_224));
				float vertex_unnamed_245 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_225));
				float vertex_unnamed_246 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_226));
				float vertex_unnamed_250 = rsqrt(dot(float3(vertex_unnamed_244, vertex_unnamed_245, vertex_unnamed_246), float3(vertex_unnamed_244, vertex_unnamed_245, vertex_unnamed_246)));
				precise float vertex_unnamed_251 = vertex_unnamed_250 * vertex_unnamed_244;
				precise float vertex_unnamed_252 = vertex_unnamed_250 * vertex_unnamed_245;
				precise float vertex_unnamed_253 = vertex_unnamed_250 * vertex_unnamed_246;
				precise float vertex_unnamed_254 = vertex_unnamed_214 * vertex_unnamed_251;
				precise float vertex_unnamed_255 = vertex_unnamed_215 * vertex_unnamed_252;
				precise float vertex_unnamed_256 = vertex_unnamed_216 * vertex_unnamed_253;
				precise float vertex_unnamed_257 = (-0.0f) - vertex_unnamed_254;
				precise float vertex_unnamed_259 = (-0.0f) - vertex_unnamed_255;
				precise float vertex_unnamed_260 = (-0.0f) - vertex_unnamed_256;
				precise float vertex_unnamed_269 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_270 = vertex_unnamed_269 * mad(vertex_unnamed_216, vertex_unnamed_252, vertex_unnamed_257);
				precise float vertex_unnamed_271 = vertex_unnamed_269 * mad(vertex_unnamed_214, vertex_unnamed_253, vertex_unnamed_259);
				precise float vertex_unnamed_272 = vertex_unnamed_269 * mad(vertex_unnamed_215, vertex_unnamed_251, vertex_unnamed_260);
				vertex_output_2.y = vertex_unnamed_270;
				vertex_output_2.x = vertex_unnamed_253;
				vertex_output_2.z = vertex_unnamed_215;
				vertex_output_3.x = vertex_unnamed_251;
				vertex_output_4.x = vertex_unnamed_252;
				vertex_output_3.z = vertex_unnamed_216;
				vertex_output_4.z = vertex_unnamed_214;
				vertex_output_3.y = vertex_unnamed_271;
				vertex_output_4.y = vertex_unnamed_272;
				vertex_output_5.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_84);
				vertex_output_5.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_85);
				vertex_output_5.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_86);
				float vertex_unnamed_303 = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_84);
				float vertex_unnamed_304 = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_85);
				float vertex_unnamed_305 = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_86);
				float vertex_unnamed_306 = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_87);
				vertex_output_6.x = vertex_input_7.x;
				vertex_output_6.y = vertex_input_7.y;
				vertex_output_6.z = vertex_input_7.z;
				vertex_output_6.w = vertex_input_7.w;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
				precise float vertex_unnamed_330 = vertex_unnamed_304 * vertex_uniform_buffer_0[5u].x;
				precise float vertex_unnamed_331 = vertex_unnamed_304 * vertex_uniform_buffer_0[5u].y;
				precise float vertex_unnamed_332 = vertex_unnamed_304 * vertex_uniform_buffer_0[5u].z;
				precise float vertex_unnamed_333 = vertex_unnamed_304 * vertex_uniform_buffer_0[5u].w;
				vertex_output_8.x = mad(vertex_uniform_buffer_0[7u].x, vertex_unnamed_306, mad(vertex_uniform_buffer_0[6u].x, vertex_unnamed_305, mad(vertex_uniform_buffer_0[4u].x, vertex_unnamed_303, vertex_unnamed_330)));
				vertex_output_8.y = mad(vertex_uniform_buffer_0[7u].y, vertex_unnamed_306, mad(vertex_uniform_buffer_0[6u].y, vertex_unnamed_305, mad(vertex_uniform_buffer_0[4u].y, vertex_unnamed_303, vertex_unnamed_331)));
				vertex_output_8.z = mad(vertex_uniform_buffer_0[7u].z, vertex_unnamed_306, mad(vertex_uniform_buffer_0[6u].z, vertex_unnamed_305, mad(vertex_uniform_buffer_0[4u].z, vertex_unnamed_303, vertex_unnamed_332)));
				vertex_output_8.w = mad(vertex_uniform_buffer_0[7u].w, vertex_unnamed_306, mad(vertex_uniform_buffer_0[6u].w, vertex_unnamed_305, mad(vertex_uniform_buffer_0[4u].w, vertex_unnamed_303, vertex_unnamed_333)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_uniform_buffer_0[9] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			#endif // SPOT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !POINT_COOKIE


			#ifdef SPOT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef POINT_COOKIE
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 unity_WorldToLight__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float3 vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_input_4;
			static float4 vertex_output_6;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 vertex_output_1 : TEXCOORD1; // vs_TEXCOORD1
				float3 vertex_output_2 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_3 : TEXCOORD3; // vs_TEXCOORD3
				float3 vertex_output_4 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_5 : UNKNOWN5;
				float4 vertex_output_6 : TEXCOORD5; // vs_TEXCOORD5
				float4 vertex_output_7 : TEXCOORD6; // vs_TEXCOORD6
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_45;
			static float4 vertex_unnamed_51;
			static float vertex_unnamed_124;
			static float3 vertex_unnamed_185;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_45 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_unnamed_51 = vertex_unnamed_45.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_51 = (unity_MatrixVP__array[0] * vertex_unnamed_45.xxxx) + vertex_unnamed_51;
				vertex_unnamed_51 = (unity_MatrixVP__array[2] * vertex_unnamed_45.zzzz) + vertex_unnamed_51;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_45.wwww) + vertex_unnamed_51;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_45.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_45.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_45.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_124 = dot(vertex_unnamed_45.xyz, vertex_unnamed_45.xyz);
				vertex_unnamed_124 = rsqrt(vertex_unnamed_124);
				float3 vertex_unnamed_136 = vertex_unnamed_124.xxx * vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_136.x, vertex_unnamed_136.y, vertex_unnamed_136.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_145 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_51 = float4(vertex_unnamed_145.x, vertex_unnamed_145.y, vertex_unnamed_145.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_156 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_156.x, vertex_unnamed_156.y, vertex_unnamed_156.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_167 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_167.x, vertex_unnamed_167.y, vertex_unnamed_167.z, vertex_unnamed_51.w);
				vertex_unnamed_124 = dot(vertex_unnamed_51.xyz, vertex_unnamed_51.xyz);
				vertex_unnamed_124 = rsqrt(vertex_unnamed_124);
				float3 vertex_unnamed_181 = vertex_unnamed_124.xxx * vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_181.x, vertex_unnamed_181.y, vertex_unnamed_181.z, vertex_unnamed_51.w);
				vertex_unnamed_185 = vertex_unnamed_45.xyz * vertex_unnamed_51.xyz;
				vertex_unnamed_185 = (vertex_unnamed_45.zxy * vertex_unnamed_51.yzx) + (-vertex_unnamed_185);
				vertex_unnamed_124 = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_185 = vertex_unnamed_124.xxx * vertex_unnamed_185;
				vertex_output_1.y = vertex_unnamed_185.x;
				vertex_output_1.x = vertex_unnamed_51.z;
				vertex_output_1.z = vertex_unnamed_45.y;
				vertex_output_2.x = vertex_unnamed_51.x;
				vertex_output_3.x = vertex_unnamed_51.y;
				vertex_output_2.z = vertex_unnamed_45.z;
				vertex_output_3.z = vertex_unnamed_45.x;
				vertex_output_2.y = vertex_unnamed_185.y;
				vertex_output_3.y = vertex_unnamed_185.z;
				vertex_output_4 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_9;
				vertex_output_5 = vertex_input_4;
				vertex_output_6 = 0.0f.xxxx;
				vertex_unnamed_45 = vertex_unnamed_9.yyyy * unity_WorldToLight__array[1];
				vertex_unnamed_45 = (unity_WorldToLight__array[0] * vertex_unnamed_9.xxxx) + vertex_unnamed_45;
				vertex_unnamed_45 = (unity_WorldToLight__array[2] * vertex_unnamed_9.zzzz) + vertex_unnamed_45;
				vertex_output_7 = (unity_WorldToLight__array[3] * vertex_unnamed_9.wwww) + vertex_unnamed_45;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;
			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;

			static float4 unity_WorldToLight__array[4];
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			Texture2D<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			Texture2D<float4> _LightTextureB0;
			SamplerState sampler_LightTextureB0;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float3 fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float4 fragment_input_5;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 fragment_input_1 : TEXCOORD1; // vs_TEXCOORD1
				float3 fragment_input_2 : TEXCOORD2; // vs_TEXCOORD2
				float3 fragment_input_3 : TEXCOORD3; // vs_TEXCOORD3
				float3 fragment_input_4 : TEXCOORD4; // vs_TEXCOORD4
				float4 fragment_input_5 : UNKNOWN5;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float4 fragment_unnamed_9;
			static float fragment_unnamed_49;
			static float4 fragment_unnamed_64;
			static float fragment_unnamed_137;
			static float3 fragment_unnamed_143;
			static float fragment_unnamed_166;
			static float fragment_unnamed_221;
			static float fragment_unnamed_227;
			static float fragment_unnamed_240;
			static float fragment_unnamed_245;
			static bool fragment_unnamed_318;

			void frag_main()
			{
				float3 fragment_unnamed_26 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_26.x, fragment_unnamed_26.y, fragment_unnamed_26.z, fragment_unnamed_9.w);
				fragment_unnamed_9.x = fragment_unnamed_9.z * fragment_unnamed_9.x;
				float2 fragment_unnamed_46 = (fragment_unnamed_9.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_9 = float4(fragment_unnamed_46.x, fragment_unnamed_46.y, fragment_unnamed_9.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xy, fragment_unnamed_9.xy);
				fragment_unnamed_49 = min(fragment_unnamed_49, 1.0f);
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_9.z = sqrt(fragment_unnamed_49);
				fragment_unnamed_64.x = dot(fragment_input_1, fragment_unnamed_9.xyz);
				fragment_unnamed_64.y = dot(fragment_input_2, fragment_unnamed_9.xyz);
				fragment_unnamed_64.z = dot(fragment_input_3, fragment_unnamed_9.xyz);
				fragment_unnamed_9.x = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_9.x = rsqrt(fragment_unnamed_9.x);
				float3 fragment_unnamed_99 = fragment_unnamed_9.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_9 = float4(fragment_unnamed_99.x, fragment_unnamed_99.y, fragment_unnamed_99.z, fragment_unnamed_9.w);
				float3 fragment_unnamed_115 = (-fragment_input_4) + _WorldSpaceCameraPos;
				fragment_unnamed_64 = float4(fragment_unnamed_115.x, fragment_unnamed_115.y, fragment_unnamed_115.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_49 = rsqrt(fragment_unnamed_49);
				float3 fragment_unnamed_129 = fragment_unnamed_49.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_129.x, fragment_unnamed_129.y, fragment_unnamed_129.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_137 = (-abs(fragment_unnamed_49)) + 1.0f;
				fragment_unnamed_143.x = fragment_unnamed_137 * fragment_unnamed_137;
				fragment_unnamed_143.x *= fragment_unnamed_143.x;
				fragment_unnamed_137 *= fragment_unnamed_143.x;
				fragment_unnamed_143 = (-fragment_input_4) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_166 = dot(fragment_unnamed_143, fragment_unnamed_143);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_178 = (fragment_unnamed_143 * fragment_unnamed_166.xxx) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_178.x, fragment_unnamed_178.y, fragment_unnamed_178.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_166.xxx * fragment_unnamed_143;
				fragment_unnamed_166 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_166 = max(fragment_unnamed_166, 0.001000000047497451305389404296875f);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_199 = fragment_unnamed_64.xyz * fragment_unnamed_166.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_199.x, fragment_unnamed_199.y, fragment_unnamed_199.z, fragment_unnamed_64.w);
				fragment_unnamed_64.x = dot(fragment_unnamed_143, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = clamp(fragment_unnamed_64.x, 0.0f, 1.0f);
				fragment_unnamed_9.x = dot(fragment_unnamed_9.xyz, fragment_unnamed_143);
				fragment_unnamed_9.x = clamp(fragment_unnamed_9.x, 0.0f, 1.0f);
				fragment_unnamed_221 = dot(fragment_unnamed_64.xx, fragment_unnamed_64.xx);
				fragment_unnamed_227 = (-fragment_unnamed_64.x) + 1.0f;
				fragment_unnamed_221 += (-0.5f);
				fragment_unnamed_64.x = (fragment_unnamed_221 * fragment_unnamed_137) + 1.0f;
				fragment_unnamed_240 = (-fragment_unnamed_9.x) + 1.0f;
				fragment_unnamed_245 = fragment_unnamed_240 * fragment_unnamed_240;
				fragment_unnamed_245 *= fragment_unnamed_245;
				fragment_unnamed_240 *= fragment_unnamed_245;
				fragment_unnamed_221 = (fragment_unnamed_221 * fragment_unnamed_240) + 1.0f;
				fragment_unnamed_221 = fragment_unnamed_64.x * fragment_unnamed_221;
				fragment_unnamed_221 = fragment_unnamed_9.x * fragment_unnamed_221;
				fragment_unnamed_64 = fragment_input_4.yyyy * unity_WorldToLight__array[1];
				fragment_unnamed_64 = (unity_WorldToLight__array[0] * fragment_input_4.xxxx) + fragment_unnamed_64;
				fragment_unnamed_64 = (unity_WorldToLight__array[2] * fragment_input_4.zzzz) + fragment_unnamed_64;
				fragment_unnamed_64 += unity_WorldToLight__array[3];
				float2 fragment_unnamed_296 = fragment_unnamed_64.xy / fragment_unnamed_64.ww;
				fragment_unnamed_143 = float3(fragment_unnamed_296.x, fragment_unnamed_296.y, fragment_unnamed_143.z);
				float2 fragment_unnamed_303 = fragment_unnamed_143.xy + 0.5f.xx;
				fragment_unnamed_143 = float3(fragment_unnamed_303.x, fragment_unnamed_303.y, fragment_unnamed_143.z);
				fragment_unnamed_137 = _LightTexture0.Sample(sampler_LightTexture0, fragment_unnamed_143.xy).w;
				fragment_unnamed_318 = 0.0f < fragment_unnamed_64.z;
				fragment_unnamed_64.x = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = _LightTextureB0.Sample(sampler_LightTextureB0, fragment_unnamed_64.xx).x;
				fragment_unnamed_240 = float(fragment_unnamed_318);
				fragment_unnamed_240 = fragment_unnamed_137 * fragment_unnamed_240;
				fragment_unnamed_64.x *= fragment_unnamed_240;
				float3 fragment_unnamed_353 = fragment_unnamed_64.xxx * _LightColor0.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_353.x, fragment_unnamed_353.y, fragment_unnamed_353.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_221.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_221 = abs(fragment_unnamed_49) + fragment_unnamed_9.x;
				fragment_unnamed_221 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_221 = 0.5f / fragment_unnamed_221;
				fragment_unnamed_9.x *= fragment_unnamed_221;
				fragment_unnamed_9.x *= 0.99999988079071044921875f;
				float3 fragment_unnamed_385 = fragment_unnamed_64.xyz * fragment_unnamed_9.xxx;
				fragment_unnamed_9 = float4(fragment_unnamed_385.x, fragment_unnamed_385.y, fragment_unnamed_9.z, fragment_unnamed_385.z);
				fragment_unnamed_64.x = fragment_unnamed_227 * fragment_unnamed_227;
				fragment_unnamed_64.x *= fragment_unnamed_64.x;
				fragment_unnamed_227 *= fragment_unnamed_64.x;
				fragment_unnamed_227 = (fragment_unnamed_227 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				float3 fragment_unnamed_411 = fragment_unnamed_227.xxx * fragment_unnamed_9.xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_411.x, fragment_unnamed_411.y, fragment_unnamed_411.z, fragment_unnamed_9.w);
				fragment_unnamed_64 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_64 *= _Color;
				float3 fragment_unnamed_432 = fragment_unnamed_64.xyz * fragment_input_5.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_432.x, fragment_unnamed_432.y, fragment_unnamed_432.z, fragment_unnamed_64.w);
				fragment_output_0.w = fragment_unnamed_64.w * fragment_input_5.w;
				float3 fragment_unnamed_448 = fragment_unnamed_64.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_448.x, fragment_unnamed_448.y, fragment_unnamed_448.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_457 = (fragment_unnamed_64.xyz * fragment_unnamed_143) + fragment_unnamed_9.xyz;
				fragment_output_0 = float4(fragment_unnamed_457.x, fragment_unnamed_457.y, fragment_unnamed_457.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // SPOT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !POINT_COOKIE


			#ifdef POINT_COOKIE
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[10];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;
			static float3 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float3 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float3 vertex_output_5 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_6 : COLOR; // COLOR
				float4 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float3 vertex_output_8 : TEXCOORD6; // TEXCOORD_6
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_58 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_59 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_61 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_84 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_58));
				float vertex_unnamed_85 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_59));
				float vertex_unnamed_86 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_60));
				float vertex_unnamed_87 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_61));
				precise float vertex_unnamed_95 = vertex_unnamed_84 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_96 = vertex_unnamed_85 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_97 = vertex_unnamed_86 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_98 = vertex_unnamed_87 + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_106 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_107 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_108 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_109 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_95, vertex_unnamed_106)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_95, vertex_unnamed_107)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_95, vertex_unnamed_108)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_95, vertex_unnamed_109)));
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[9u].x, vertex_uniform_buffer_0[9u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[9u].y, vertex_uniform_buffer_0[9u].w);
				float vertex_unnamed_177 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_192 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_207 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_213 = rsqrt(dot(float3(vertex_unnamed_207, vertex_unnamed_177, vertex_unnamed_192), float3(vertex_unnamed_207, vertex_unnamed_177, vertex_unnamed_192)));
				precise float vertex_unnamed_214 = vertex_unnamed_213 * vertex_unnamed_207;
				precise float vertex_unnamed_215 = vertex_unnamed_213 * vertex_unnamed_177;
				precise float vertex_unnamed_216 = vertex_unnamed_213 * vertex_unnamed_192;
				precise float vertex_unnamed_224 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_225 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_226 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_244 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_224));
				float vertex_unnamed_245 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_225));
				float vertex_unnamed_246 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_226));
				float vertex_unnamed_250 = rsqrt(dot(float3(vertex_unnamed_244, vertex_unnamed_245, vertex_unnamed_246), float3(vertex_unnamed_244, vertex_unnamed_245, vertex_unnamed_246)));
				precise float vertex_unnamed_251 = vertex_unnamed_250 * vertex_unnamed_244;
				precise float vertex_unnamed_252 = vertex_unnamed_250 * vertex_unnamed_245;
				precise float vertex_unnamed_253 = vertex_unnamed_250 * vertex_unnamed_246;
				precise float vertex_unnamed_254 = vertex_unnamed_214 * vertex_unnamed_251;
				precise float vertex_unnamed_255 = vertex_unnamed_215 * vertex_unnamed_252;
				precise float vertex_unnamed_256 = vertex_unnamed_216 * vertex_unnamed_253;
				precise float vertex_unnamed_257 = (-0.0f) - vertex_unnamed_254;
				precise float vertex_unnamed_259 = (-0.0f) - vertex_unnamed_255;
				precise float vertex_unnamed_260 = (-0.0f) - vertex_unnamed_256;
				precise float vertex_unnamed_269 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_270 = vertex_unnamed_269 * mad(vertex_unnamed_216, vertex_unnamed_252, vertex_unnamed_257);
				precise float vertex_unnamed_271 = vertex_unnamed_269 * mad(vertex_unnamed_214, vertex_unnamed_253, vertex_unnamed_259);
				precise float vertex_unnamed_272 = vertex_unnamed_269 * mad(vertex_unnamed_215, vertex_unnamed_251, vertex_unnamed_260);
				vertex_output_2.y = vertex_unnamed_270;
				vertex_output_2.x = vertex_unnamed_253;
				vertex_output_2.z = vertex_unnamed_215;
				vertex_output_3.x = vertex_unnamed_251;
				vertex_output_4.x = vertex_unnamed_252;
				vertex_output_3.z = vertex_unnamed_216;
				vertex_output_4.z = vertex_unnamed_214;
				vertex_output_3.y = vertex_unnamed_271;
				vertex_output_4.y = vertex_unnamed_272;
				vertex_output_5.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_84);
				vertex_output_5.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_85);
				vertex_output_5.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_86);
				float vertex_unnamed_303 = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_84);
				float vertex_unnamed_304 = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_85);
				float vertex_unnamed_305 = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_86);
				float vertex_unnamed_306 = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_87);
				vertex_output_6.x = vertex_input_7.x;
				vertex_output_6.y = vertex_input_7.y;
				vertex_output_6.z = vertex_input_7.z;
				vertex_output_6.w = vertex_input_7.w;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
				precise float vertex_unnamed_329 = vertex_unnamed_304 * vertex_uniform_buffer_0[5u].x;
				precise float vertex_unnamed_330 = vertex_unnamed_304 * vertex_uniform_buffer_0[5u].y;
				precise float vertex_unnamed_331 = vertex_unnamed_304 * vertex_uniform_buffer_0[5u].z;
				vertex_output_8.x = mad(vertex_uniform_buffer_0[7u].x, vertex_unnamed_306, mad(vertex_uniform_buffer_0[6u].x, vertex_unnamed_305, mad(vertex_uniform_buffer_0[4u].x, vertex_unnamed_303, vertex_unnamed_329)));
				vertex_output_8.y = mad(vertex_uniform_buffer_0[7u].y, vertex_unnamed_306, mad(vertex_uniform_buffer_0[6u].y, vertex_unnamed_305, mad(vertex_uniform_buffer_0[4u].y, vertex_unnamed_303, vertex_unnamed_330)));
				vertex_output_8.z = mad(vertex_uniform_buffer_0[7u].z, vertex_unnamed_306, mad(vertex_uniform_buffer_0[6u].z, vertex_unnamed_305, mad(vertex_uniform_buffer_0[4u].z, vertex_unnamed_303, vertex_unnamed_331)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_uniform_buffer_0[9] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			#endif // POINT_COOKIE
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !SPOT


			#ifdef POINT_COOKIE
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 unity_WorldToLight__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float3 vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float4 vertex_output_5;
			static float4 vertex_input_4;
			static float4 vertex_output_6;
			static float3 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 vertex_output_1 : TEXCOORD1; // vs_TEXCOORD1
				float3 vertex_output_2 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_3 : TEXCOORD3; // vs_TEXCOORD3
				float3 vertex_output_4 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_5 : UNKNOWN5;
				float4 vertex_output_6 : TEXCOORD5; // vs_TEXCOORD5
				float3 vertex_output_7 : TEXCOORD6; // vs_TEXCOORD6
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_45;
			static float4 vertex_unnamed_51;
			static float vertex_unnamed_124;
			static float3 vertex_unnamed_185;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_45 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_unnamed_51 = vertex_unnamed_45.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_51 = (unity_MatrixVP__array[0] * vertex_unnamed_45.xxxx) + vertex_unnamed_51;
				vertex_unnamed_51 = (unity_MatrixVP__array[2] * vertex_unnamed_45.zzzz) + vertex_unnamed_51;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_45.wwww) + vertex_unnamed_51;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_45.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_45.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_45.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_124 = dot(vertex_unnamed_45.xyz, vertex_unnamed_45.xyz);
				vertex_unnamed_124 = rsqrt(vertex_unnamed_124);
				float3 vertex_unnamed_136 = vertex_unnamed_124.xxx * vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_136.x, vertex_unnamed_136.y, vertex_unnamed_136.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_145 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_51 = float4(vertex_unnamed_145.x, vertex_unnamed_145.y, vertex_unnamed_145.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_156 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_156.x, vertex_unnamed_156.y, vertex_unnamed_156.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_167 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_167.x, vertex_unnamed_167.y, vertex_unnamed_167.z, vertex_unnamed_51.w);
				vertex_unnamed_124 = dot(vertex_unnamed_51.xyz, vertex_unnamed_51.xyz);
				vertex_unnamed_124 = rsqrt(vertex_unnamed_124);
				float3 vertex_unnamed_181 = vertex_unnamed_124.xxx * vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_181.x, vertex_unnamed_181.y, vertex_unnamed_181.z, vertex_unnamed_51.w);
				vertex_unnamed_185 = vertex_unnamed_45.xyz * vertex_unnamed_51.xyz;
				vertex_unnamed_185 = (vertex_unnamed_45.zxy * vertex_unnamed_51.yzx) + (-vertex_unnamed_185);
				vertex_unnamed_124 = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_185 = vertex_unnamed_124.xxx * vertex_unnamed_185;
				vertex_output_1.y = vertex_unnamed_185.x;
				vertex_output_1.x = vertex_unnamed_51.z;
				vertex_output_1.z = vertex_unnamed_45.y;
				vertex_output_2.x = vertex_unnamed_51.x;
				vertex_output_3.x = vertex_unnamed_51.y;
				vertex_output_2.z = vertex_unnamed_45.z;
				vertex_output_3.z = vertex_unnamed_45.x;
				vertex_output_2.y = vertex_unnamed_185.y;
				vertex_output_3.y = vertex_unnamed_185.z;
				vertex_output_4 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_9;
				vertex_output_5 = vertex_input_4;
				vertex_output_6 = 0.0f.xxxx;
				float3 vertex_unnamed_272 = vertex_unnamed_9.yyy * unity_WorldToLight__array[1].xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_272.x, vertex_unnamed_272.y, vertex_unnamed_272.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_283 = (unity_WorldToLight__array[0].xyz * vertex_unnamed_9.xxx) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_283.x, vertex_unnamed_283.y, vertex_unnamed_283.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_294 = (unity_WorldToLight__array[2].xyz * vertex_unnamed_9.zzz) + vertex_unnamed_45.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_294.x, vertex_unnamed_294.y, vertex_unnamed_294.z, vertex_unnamed_9.w);
				vertex_output_7 = (unity_WorldToLight__array[3].xyz * vertex_unnamed_9.www) + vertex_unnamed_9.xyz;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;
			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;

			static float4 unity_WorldToLight__array[4];
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			TextureCube<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			Texture2D<float4> _LightTextureB0;
			SamplerState sampler_LightTextureB0;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float3 fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float4 fragment_input_5;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 fragment_input_1 : TEXCOORD1; // vs_TEXCOORD1
				float3 fragment_input_2 : TEXCOORD2; // vs_TEXCOORD2
				float3 fragment_input_3 : TEXCOORD3; // vs_TEXCOORD3
				float3 fragment_input_4 : TEXCOORD4; // vs_TEXCOORD4
				float4 fragment_input_5 : UNKNOWN5;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float4 fragment_unnamed_9;
			static float fragment_unnamed_49;
			static float4 fragment_unnamed_64;
			static float fragment_unnamed_137;
			static float3 fragment_unnamed_143;
			static float fragment_unnamed_166;
			static float fragment_unnamed_221;
			static float fragment_unnamed_227;
			static float fragment_unnamed_240;
			static float fragment_unnamed_245;

			void frag_main()
			{
				float3 fragment_unnamed_26 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_26.x, fragment_unnamed_26.y, fragment_unnamed_26.z, fragment_unnamed_9.w);
				fragment_unnamed_9.x = fragment_unnamed_9.z * fragment_unnamed_9.x;
				float2 fragment_unnamed_46 = (fragment_unnamed_9.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_9 = float4(fragment_unnamed_46.x, fragment_unnamed_46.y, fragment_unnamed_9.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xy, fragment_unnamed_9.xy);
				fragment_unnamed_49 = min(fragment_unnamed_49, 1.0f);
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_9.z = sqrt(fragment_unnamed_49);
				fragment_unnamed_64.x = dot(fragment_input_1, fragment_unnamed_9.xyz);
				fragment_unnamed_64.y = dot(fragment_input_2, fragment_unnamed_9.xyz);
				fragment_unnamed_64.z = dot(fragment_input_3, fragment_unnamed_9.xyz);
				fragment_unnamed_9.x = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_9.x = rsqrt(fragment_unnamed_9.x);
				float3 fragment_unnamed_99 = fragment_unnamed_9.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_9 = float4(fragment_unnamed_99.x, fragment_unnamed_99.y, fragment_unnamed_99.z, fragment_unnamed_9.w);
				float3 fragment_unnamed_115 = (-fragment_input_4) + _WorldSpaceCameraPos;
				fragment_unnamed_64 = float4(fragment_unnamed_115.x, fragment_unnamed_115.y, fragment_unnamed_115.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_49 = rsqrt(fragment_unnamed_49);
				float3 fragment_unnamed_129 = fragment_unnamed_49.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_129.x, fragment_unnamed_129.y, fragment_unnamed_129.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_137 = (-abs(fragment_unnamed_49)) + 1.0f;
				fragment_unnamed_143.x = fragment_unnamed_137 * fragment_unnamed_137;
				fragment_unnamed_143.x *= fragment_unnamed_143.x;
				fragment_unnamed_137 *= fragment_unnamed_143.x;
				fragment_unnamed_143 = (-fragment_input_4) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_166 = dot(fragment_unnamed_143, fragment_unnamed_143);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_178 = (fragment_unnamed_143 * fragment_unnamed_166.xxx) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_178.x, fragment_unnamed_178.y, fragment_unnamed_178.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_166.xxx * fragment_unnamed_143;
				fragment_unnamed_166 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_166 = max(fragment_unnamed_166, 0.001000000047497451305389404296875f);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_199 = fragment_unnamed_64.xyz * fragment_unnamed_166.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_199.x, fragment_unnamed_199.y, fragment_unnamed_199.z, fragment_unnamed_64.w);
				fragment_unnamed_64.x = dot(fragment_unnamed_143, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = clamp(fragment_unnamed_64.x, 0.0f, 1.0f);
				fragment_unnamed_9.x = dot(fragment_unnamed_9.xyz, fragment_unnamed_143);
				fragment_unnamed_9.x = clamp(fragment_unnamed_9.x, 0.0f, 1.0f);
				fragment_unnamed_221 = dot(fragment_unnamed_64.xx, fragment_unnamed_64.xx);
				fragment_unnamed_227 = (-fragment_unnamed_64.x) + 1.0f;
				fragment_unnamed_221 += (-0.5f);
				fragment_unnamed_64.x = (fragment_unnamed_221 * fragment_unnamed_137) + 1.0f;
				fragment_unnamed_240 = (-fragment_unnamed_9.x) + 1.0f;
				fragment_unnamed_245 = fragment_unnamed_240 * fragment_unnamed_240;
				fragment_unnamed_245 *= fragment_unnamed_245;
				fragment_unnamed_240 *= fragment_unnamed_245;
				fragment_unnamed_221 = (fragment_unnamed_221 * fragment_unnamed_240) + 1.0f;
				fragment_unnamed_221 = fragment_unnamed_64.x * fragment_unnamed_221;
				fragment_unnamed_221 = fragment_unnamed_9.x * fragment_unnamed_221;
				float3 fragment_unnamed_273 = fragment_input_4.yyy * unity_WorldToLight__array[1].xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_273.x, fragment_unnamed_273.y, fragment_unnamed_273.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_284 = (unity_WorldToLight__array[0].xyz * fragment_input_4.xxx) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_284.x, fragment_unnamed_284.y, fragment_unnamed_284.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_296 = (unity_WorldToLight__array[2].xyz * fragment_input_4.zzz) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_296.x, fragment_unnamed_296.y, fragment_unnamed_296.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_304 = fragment_unnamed_64.xyz + unity_WorldToLight__array[3].xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_304.x, fragment_unnamed_304.y, fragment_unnamed_304.z, fragment_unnamed_64.w);
				fragment_unnamed_137 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = _LightTexture0.Sample(sampler_LightTexture0, fragment_unnamed_64.xyz).w;
				fragment_unnamed_240 = _LightTextureB0.Sample(sampler_LightTextureB0, fragment_unnamed_137.xx).x;
				fragment_unnamed_64.x *= fragment_unnamed_240;
				float3 fragment_unnamed_345 = fragment_unnamed_64.xxx * _LightColor0.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_345.x, fragment_unnamed_345.y, fragment_unnamed_345.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_221.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_221 = abs(fragment_unnamed_49) + fragment_unnamed_9.x;
				fragment_unnamed_221 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_221 = 0.5f / fragment_unnamed_221;
				fragment_unnamed_9.x *= fragment_unnamed_221;
				fragment_unnamed_9.x *= 0.99999988079071044921875f;
				float3 fragment_unnamed_378 = fragment_unnamed_64.xyz * fragment_unnamed_9.xxx;
				fragment_unnamed_9 = float4(fragment_unnamed_378.x, fragment_unnamed_378.y, fragment_unnamed_9.z, fragment_unnamed_378.z);
				fragment_unnamed_64.x = fragment_unnamed_227 * fragment_unnamed_227;
				fragment_unnamed_64.x *= fragment_unnamed_64.x;
				fragment_unnamed_227 *= fragment_unnamed_64.x;
				fragment_unnamed_227 = (fragment_unnamed_227 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				float3 fragment_unnamed_404 = fragment_unnamed_227.xxx * fragment_unnamed_9.xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_404.x, fragment_unnamed_404.y, fragment_unnamed_404.z, fragment_unnamed_9.w);
				fragment_unnamed_64 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_64 *= _Color;
				float3 fragment_unnamed_425 = fragment_unnamed_64.xyz * fragment_input_5.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_425.x, fragment_unnamed_425.y, fragment_unnamed_425.z, fragment_unnamed_64.w);
				fragment_output_0.w = fragment_unnamed_64.w * fragment_input_5.w;
				float3 fragment_unnamed_441 = fragment_unnamed_64.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_441.x, fragment_unnamed_441.y, fragment_unnamed_441.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_450 = (fragment_unnamed_64.xyz * fragment_unnamed_143) + fragment_unnamed_9.xyz;
				fragment_output_0 = float4(fragment_unnamed_450.x, fragment_unnamed_450.y, fragment_unnamed_450.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // POINT_COOKIE
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !SPOT


			#ifdef DIRECTIONAL_COOKIE
			#ifndef DIRECTIONAL
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[10];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float2 vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float2 vertex_output_1 : TEXCOORD6; // TEXCOORD_6
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float3 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float3 vertex_output_5 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_6 : COLOR; // COLOR
				float4 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_58 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_59 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_61 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_84 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_58));
				float vertex_unnamed_85 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_59));
				float vertex_unnamed_86 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_60));
				float vertex_unnamed_87 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_61));
				precise float vertex_unnamed_95 = vertex_unnamed_84 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_96 = vertex_unnamed_85 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_97 = vertex_unnamed_86 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_98 = vertex_unnamed_87 + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_106 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_107 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_108 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_109 = vertex_unnamed_96 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_95, vertex_unnamed_106)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_95, vertex_unnamed_107)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_95, vertex_unnamed_108)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_98, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_97, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_95, vertex_unnamed_109)));
				float vertex_unnamed_156 = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_84);
				float vertex_unnamed_157 = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_85);
				float vertex_unnamed_158 = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_86);
				float vertex_unnamed_159 = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_87);
				vertex_output_5.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_84);
				vertex_output_5.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_85);
				vertex_output_5.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_86);
				precise float vertex_unnamed_178 = vertex_unnamed_157 * vertex_uniform_buffer_0[5u].x;
				precise float vertex_unnamed_179 = vertex_unnamed_157 * vertex_uniform_buffer_0[5u].y;
				vertex_output_1.x = mad(vertex_uniform_buffer_0[7u].x, vertex_unnamed_159, mad(vertex_uniform_buffer_0[6u].x, vertex_unnamed_158, mad(vertex_uniform_buffer_0[4u].x, vertex_unnamed_156, vertex_unnamed_178)));
				vertex_output_1.y = mad(vertex_uniform_buffer_0[7u].y, vertex_unnamed_159, mad(vertex_uniform_buffer_0[6u].y, vertex_unnamed_158, mad(vertex_uniform_buffer_0[4u].y, vertex_unnamed_156, vertex_unnamed_179)));
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[9u].x, vertex_uniform_buffer_0[9u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[9u].y, vertex_uniform_buffer_0[9u].w);
				float vertex_unnamed_231 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_245 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_259 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_265 = rsqrt(dot(float3(vertex_unnamed_259, vertex_unnamed_231, vertex_unnamed_245), float3(vertex_unnamed_259, vertex_unnamed_231, vertex_unnamed_245)));
				precise float vertex_unnamed_266 = vertex_unnamed_265 * vertex_unnamed_259;
				precise float vertex_unnamed_267 = vertex_unnamed_265 * vertex_unnamed_231;
				precise float vertex_unnamed_268 = vertex_unnamed_265 * vertex_unnamed_245;
				precise float vertex_unnamed_276 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_277 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_278 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_296 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_276));
				float vertex_unnamed_297 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_277));
				float vertex_unnamed_298 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_278));
				float vertex_unnamed_302 = rsqrt(dot(float3(vertex_unnamed_296, vertex_unnamed_297, vertex_unnamed_298), float3(vertex_unnamed_296, vertex_unnamed_297, vertex_unnamed_298)));
				precise float vertex_unnamed_303 = vertex_unnamed_302 * vertex_unnamed_296;
				precise float vertex_unnamed_304 = vertex_unnamed_302 * vertex_unnamed_297;
				precise float vertex_unnamed_305 = vertex_unnamed_302 * vertex_unnamed_298;
				precise float vertex_unnamed_306 = vertex_unnamed_266 * vertex_unnamed_303;
				precise float vertex_unnamed_307 = vertex_unnamed_267 * vertex_unnamed_304;
				precise float vertex_unnamed_308 = vertex_unnamed_268 * vertex_unnamed_305;
				precise float vertex_unnamed_309 = (-0.0f) - vertex_unnamed_306;
				precise float vertex_unnamed_311 = (-0.0f) - vertex_unnamed_307;
				precise float vertex_unnamed_312 = (-0.0f) - vertex_unnamed_308;
				precise float vertex_unnamed_321 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_322 = vertex_unnamed_321 * mad(vertex_unnamed_268, vertex_unnamed_304, vertex_unnamed_309);
				precise float vertex_unnamed_323 = vertex_unnamed_321 * mad(vertex_unnamed_266, vertex_unnamed_305, vertex_unnamed_311);
				precise float vertex_unnamed_324 = vertex_unnamed_321 * mad(vertex_unnamed_267, vertex_unnamed_303, vertex_unnamed_312);
				vertex_output_2.y = vertex_unnamed_322;
				vertex_output_2.x = vertex_unnamed_305;
				vertex_output_2.z = vertex_unnamed_267;
				vertex_output_3.x = vertex_unnamed_303;
				vertex_output_4.x = vertex_unnamed_304;
				vertex_output_3.z = vertex_unnamed_268;
				vertex_output_4.z = vertex_unnamed_266;
				vertex_output_3.y = vertex_unnamed_323;
				vertex_output_4.y = vertex_unnamed_324;
				vertex_output_6.x = vertex_input_7.x;
				vertex_output_6.y = vertex_input_7.y;
				vertex_output_6.z = vertex_input_7.z;
				vertex_output_6.w = vertex_input_7.w;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_uniform_buffer_0[9] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			#endif // DIRECTIONAL_COOKIE
			#endif // !DIRECTIONAL
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef DIRECTIONAL_COOKIE
			#ifndef DIRECTIONAL
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 unity_WorldToLight__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float3 vertex_output_4;
			static float2 vertex_output_6;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float3 vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float4 vertex_output_5;
			static float4 vertex_input_4;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 vertex_output_1 : TEXCOORD1; // vs_TEXCOORD1
				float3 vertex_output_2 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_3 : TEXCOORD3; // vs_TEXCOORD3
				float3 vertex_output_4 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_5 : UNKNOWN5;
				float2 vertex_output_6 : TEXCOORD6; // vs_TEXCOORD6
				float4 vertex_output_7 : TEXCOORD5; // vs_TEXCOORD5
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_45;
			static float4 vertex_unnamed_51;
			static float vertex_unnamed_183;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_45 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_unnamed_51 = vertex_unnamed_45.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_51 = (unity_MatrixVP__array[0] * vertex_unnamed_45.xxxx) + vertex_unnamed_51;
				vertex_unnamed_51 = (unity_MatrixVP__array[2] * vertex_unnamed_45.zzzz) + vertex_unnamed_51;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_45.wwww) + vertex_unnamed_51;
				vertex_unnamed_45 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_9;
				vertex_output_4 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				float2 vertex_unnamed_111 = vertex_unnamed_45.yy * unity_WorldToLight__array[1].xy;
				vertex_unnamed_9 = float4(vertex_unnamed_111.x, vertex_unnamed_111.y, vertex_unnamed_9.z, vertex_unnamed_9.w);
				float2 vertex_unnamed_122 = (unity_WorldToLight__array[0].xy * vertex_unnamed_45.xx) + vertex_unnamed_9.xy;
				vertex_unnamed_9 = float4(vertex_unnamed_122.x, vertex_unnamed_122.y, vertex_unnamed_9.z, vertex_unnamed_9.w);
				float2 vertex_unnamed_133 = (unity_WorldToLight__array[2].xy * vertex_unnamed_45.zz) + vertex_unnamed_9.xy;
				vertex_unnamed_9 = float4(vertex_unnamed_133.x, vertex_unnamed_133.y, vertex_unnamed_9.z, vertex_unnamed_9.w);
				vertex_output_6 = (unity_WorldToLight__array[3].xy * vertex_unnamed_45.ww) + vertex_unnamed_9.xy;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_9.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_9.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_9.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_183 = dot(vertex_unnamed_9.xyz, vertex_unnamed_9.xyz);
				vertex_unnamed_183 = rsqrt(vertex_unnamed_183);
				float3 vertex_unnamed_195 = vertex_unnamed_183.xxx * vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_195.x, vertex_unnamed_195.y, vertex_unnamed_195.z, vertex_unnamed_9.w);
				float3 vertex_unnamed_204 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_45 = float4(vertex_unnamed_204.x, vertex_unnamed_204.y, vertex_unnamed_204.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_215 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_215.x, vertex_unnamed_215.y, vertex_unnamed_215.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_226 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_226.x, vertex_unnamed_226.y, vertex_unnamed_226.z, vertex_unnamed_45.w);
				vertex_unnamed_183 = dot(vertex_unnamed_45.xyz, vertex_unnamed_45.xyz);
				vertex_unnamed_183 = rsqrt(vertex_unnamed_183);
				float3 vertex_unnamed_240 = vertex_unnamed_183.xxx * vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_240.x, vertex_unnamed_240.y, vertex_unnamed_240.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_247 = vertex_unnamed_9.xyz * vertex_unnamed_45.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_247.x, vertex_unnamed_247.y, vertex_unnamed_247.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_258 = (vertex_unnamed_9.zxy * vertex_unnamed_45.yzx) + (-vertex_unnamed_51.xyz);
				vertex_unnamed_51 = float4(vertex_unnamed_258.x, vertex_unnamed_258.y, vertex_unnamed_258.z, vertex_unnamed_51.w);
				vertex_unnamed_183 = vertex_input_1.w * unity_WorldTransformParams.w;
				float3 vertex_unnamed_273 = vertex_unnamed_183.xxx * vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_273.x, vertex_unnamed_273.y, vertex_unnamed_273.z, vertex_unnamed_51.w);
				vertex_output_1.y = vertex_unnamed_51.x;
				vertex_output_1.x = vertex_unnamed_45.z;
				vertex_output_1.z = vertex_unnamed_9.y;
				vertex_output_2.x = vertex_unnamed_45.x;
				vertex_output_3.x = vertex_unnamed_45.y;
				vertex_output_2.z = vertex_unnamed_9.z;
				vertex_output_3.z = vertex_unnamed_9.x;
				vertex_output_2.y = vertex_unnamed_51.y;
				vertex_output_3.y = vertex_unnamed_51.z;
				vertex_output_5 = vertex_input_4;
				vertex_output_7 = 0.0f.xxxx;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;
			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;

			static float4 unity_WorldToLight__array[4];
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			Texture2D<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float3 fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float4 fragment_input_5;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 fragment_input_1 : TEXCOORD1; // vs_TEXCOORD1
				float3 fragment_input_2 : TEXCOORD2; // vs_TEXCOORD2
				float3 fragment_input_3 : TEXCOORD3; // vs_TEXCOORD3
				float3 fragment_input_4 : TEXCOORD4; // vs_TEXCOORD4
				float4 fragment_input_5 : UNKNOWN5;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float fragment_unnamed_47;
			static float4 fragment_unnamed_63;
			static float3 fragment_unnamed_119;
			static float fragment_unnamed_152;
			static float fragment_unnamed_157;
			static float fragment_unnamed_229;
			static float fragment_unnamed_234;

			void frag_main()
			{
				fragment_unnamed_9 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_9.x = fragment_unnamed_9.z * fragment_unnamed_9.x;
				float2 fragment_unnamed_44 = (fragment_unnamed_9.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_9 = float3(fragment_unnamed_44.x, fragment_unnamed_44.y, fragment_unnamed_9.z);
				fragment_unnamed_47 = dot(fragment_unnamed_9.xy, fragment_unnamed_9.xy);
				fragment_unnamed_47 = min(fragment_unnamed_47, 1.0f);
				fragment_unnamed_47 = (-fragment_unnamed_47) + 1.0f;
				fragment_unnamed_9.z = sqrt(fragment_unnamed_47);
				fragment_unnamed_63.x = dot(fragment_input_1, fragment_unnamed_9);
				fragment_unnamed_63.y = dot(fragment_input_2, fragment_unnamed_9);
				fragment_unnamed_63.z = dot(fragment_input_3, fragment_unnamed_9);
				fragment_unnamed_9.x = dot(fragment_unnamed_63.xyz, fragment_unnamed_63.xyz);
				fragment_unnamed_9.x = rsqrt(fragment_unnamed_9.x);
				fragment_unnamed_9 = fragment_unnamed_9.xxx * fragment_unnamed_63.xyz;
				float3 fragment_unnamed_109 = (-fragment_input_4) + _WorldSpaceCameraPos;
				fragment_unnamed_63 = float4(fragment_unnamed_109.x, fragment_unnamed_109.y, fragment_unnamed_109.z, fragment_unnamed_63.w);
				fragment_unnamed_47 = dot(fragment_unnamed_63.xyz, fragment_unnamed_63.xyz);
				fragment_unnamed_47 = rsqrt(fragment_unnamed_47);
				fragment_unnamed_119 = fragment_unnamed_47.xxx * fragment_unnamed_63.xyz;
				float3 fragment_unnamed_135 = (fragment_unnamed_63.xyz * fragment_unnamed_47.xxx) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_63 = float4(fragment_unnamed_135.x, fragment_unnamed_135.y, fragment_unnamed_135.z, fragment_unnamed_63.w);
				fragment_unnamed_47 = dot(fragment_unnamed_9, fragment_unnamed_119);
				fragment_unnamed_9.x = dot(fragment_unnamed_9, _WorldSpaceLightPos0.xyz);
				fragment_unnamed_9.x = clamp(fragment_unnamed_9.x, 0.0f, 1.0f);
				fragment_unnamed_152 = (-abs(fragment_unnamed_47)) + 1.0f;
				fragment_unnamed_157 = abs(fragment_unnamed_47) + fragment_unnamed_9.x;
				fragment_unnamed_157 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_157 = 0.5f / fragment_unnamed_157;
				fragment_unnamed_157 = fragment_unnamed_9.x * fragment_unnamed_157;
				fragment_unnamed_157 *= 0.99999988079071044921875f;
				fragment_unnamed_47 = fragment_unnamed_152 * fragment_unnamed_152;
				fragment_unnamed_47 *= fragment_unnamed_47;
				fragment_unnamed_152 *= fragment_unnamed_47;
				fragment_unnamed_47 = dot(fragment_unnamed_63.xyz, fragment_unnamed_63.xyz);
				fragment_unnamed_47 = max(fragment_unnamed_47, 0.001000000047497451305389404296875f);
				fragment_unnamed_47 = rsqrt(fragment_unnamed_47);
				float3 fragment_unnamed_199 = fragment_unnamed_47.xxx * fragment_unnamed_63.xyz;
				fragment_unnamed_63 = float4(fragment_unnamed_199.x, fragment_unnamed_199.y, fragment_unnamed_199.z, fragment_unnamed_63.w);
				fragment_unnamed_47 = dot(_WorldSpaceLightPos0.xyz, fragment_unnamed_63.xyz);
				fragment_unnamed_47 = clamp(fragment_unnamed_47, 0.0f, 1.0f);
				fragment_unnamed_63.x = dot(fragment_unnamed_47.xx, fragment_unnamed_47.xx);
				fragment_unnamed_47 = (-fragment_unnamed_47) + 1.0f;
				fragment_unnamed_63.x += (-0.5f);
				fragment_unnamed_152 = (fragment_unnamed_63.x * fragment_unnamed_152) + 1.0f;
				fragment_unnamed_229 = (-fragment_unnamed_9.x) + 1.0f;
				fragment_unnamed_234 = fragment_unnamed_229 * fragment_unnamed_229;
				fragment_unnamed_234 *= fragment_unnamed_234;
				fragment_unnamed_229 *= fragment_unnamed_234;
				fragment_unnamed_63.x = (fragment_unnamed_63.x * fragment_unnamed_229) + 1.0f;
				fragment_unnamed_152 *= fragment_unnamed_63.x;
				fragment_unnamed_9.x *= fragment_unnamed_152;
				float2 fragment_unnamed_265 = fragment_input_4.yy * unity_WorldToLight__array[1].xy;
				fragment_unnamed_63 = float4(fragment_unnamed_265.x, fragment_unnamed_265.y, fragment_unnamed_63.z, fragment_unnamed_63.w);
				float2 fragment_unnamed_276 = (unity_WorldToLight__array[0].xy * fragment_input_4.xx) + fragment_unnamed_63.xy;
				fragment_unnamed_63 = float4(fragment_unnamed_276.x, fragment_unnamed_276.y, fragment_unnamed_63.z, fragment_unnamed_63.w);
				float2 fragment_unnamed_288 = (unity_WorldToLight__array[2].xy * fragment_input_4.zz) + fragment_unnamed_63.xy;
				fragment_unnamed_63 = float4(fragment_unnamed_288.x, fragment_unnamed_288.y, fragment_unnamed_63.z, fragment_unnamed_63.w);
				float2 fragment_unnamed_296 = fragment_unnamed_63.xy + unity_WorldToLight__array[3].xy;
				fragment_unnamed_63 = float4(fragment_unnamed_296.x, fragment_unnamed_296.y, fragment_unnamed_63.z, fragment_unnamed_63.w);
				fragment_unnamed_152 = _LightTexture0.Sample(sampler_LightTexture0, fragment_unnamed_63.xy).w;
				float3 fragment_unnamed_314 = fragment_unnamed_152.xxx * _LightColor0.xyz;
				fragment_unnamed_63 = float4(fragment_unnamed_314.x, fragment_unnamed_314.y, fragment_unnamed_314.z, fragment_unnamed_63.w);
				fragment_unnamed_119 = fragment_unnamed_9.xxx * fragment_unnamed_63.xyz;
				fragment_unnamed_9 = fragment_unnamed_157.xxx * fragment_unnamed_63.xyz;
				fragment_unnamed_63.x = fragment_unnamed_47 * fragment_unnamed_47;
				fragment_unnamed_63.x *= fragment_unnamed_63.x;
				fragment_unnamed_47 *= fragment_unnamed_63.x;
				fragment_unnamed_47 = (fragment_unnamed_47 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				fragment_unnamed_9 = fragment_unnamed_47.xxx * fragment_unnamed_9;
				fragment_unnamed_63 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_63 *= _Color;
				float3 fragment_unnamed_368 = fragment_unnamed_63.xyz * fragment_input_5.xyz;
				fragment_unnamed_63 = float4(fragment_unnamed_368.x, fragment_unnamed_368.y, fragment_unnamed_368.z, fragment_unnamed_63.w);
				fragment_output_0.w = fragment_unnamed_63.w * fragment_input_5.w;
				float3 fragment_unnamed_384 = fragment_unnamed_63.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_63 = float4(fragment_unnamed_384.x, fragment_unnamed_384.y, fragment_unnamed_384.z, fragment_unnamed_63.w);
				float3 fragment_unnamed_392 = (fragment_unnamed_63.xyz * fragment_unnamed_119) + fragment_unnamed_9;
				fragment_output_0 = float4(fragment_unnamed_392.x, fragment_unnamed_392.y, fragment_unnamed_392.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL_COOKIE
			#endif // !DIRECTIONAL
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef FOG_LINEAR
			#ifdef POINT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[10];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;
			static float3 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float vertex_output_1 : TEXCOORD7; // TEXCOORD_7
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float3 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float3 vertex_output_5 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_6 : COLOR; // COLOR
				float4 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float3 vertex_output_8 : TEXCOORD6; // TEXCOORD_6
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_61 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_62 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_63 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_86 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_60));
				float vertex_unnamed_87 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_61));
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_62));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_63));
				precise float vertex_unnamed_97 = vertex_unnamed_86 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_98 = vertex_unnamed_87 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_99 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_100 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_108 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_109 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_110 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_111 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].w;
				float vertex_unnamed_143 = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_97, vertex_unnamed_110)));
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_97, vertex_unnamed_108)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_97, vertex_unnamed_109)));
				gl_Position.z = vertex_unnamed_143;
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_97, vertex_unnamed_111)));
				vertex_output_1 = vertex_unnamed_143;
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[9u].x, vertex_uniform_buffer_0[9u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[9u].y, vertex_uniform_buffer_0[9u].w);
				float vertex_unnamed_178 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_193 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_208 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_214 = rsqrt(dot(float3(vertex_unnamed_208, vertex_unnamed_178, vertex_unnamed_193), float3(vertex_unnamed_208, vertex_unnamed_178, vertex_unnamed_193)));
				precise float vertex_unnamed_215 = vertex_unnamed_214 * vertex_unnamed_208;
				precise float vertex_unnamed_216 = vertex_unnamed_214 * vertex_unnamed_178;
				precise float vertex_unnamed_217 = vertex_unnamed_214 * vertex_unnamed_193;
				precise float vertex_unnamed_225 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_226 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_227 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_245 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_225));
				float vertex_unnamed_246 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_226));
				float vertex_unnamed_247 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_227));
				float vertex_unnamed_251 = rsqrt(dot(float3(vertex_unnamed_245, vertex_unnamed_246, vertex_unnamed_247), float3(vertex_unnamed_245, vertex_unnamed_246, vertex_unnamed_247)));
				precise float vertex_unnamed_252 = vertex_unnamed_251 * vertex_unnamed_245;
				precise float vertex_unnamed_253 = vertex_unnamed_251 * vertex_unnamed_246;
				precise float vertex_unnamed_254 = vertex_unnamed_251 * vertex_unnamed_247;
				precise float vertex_unnamed_255 = vertex_unnamed_215 * vertex_unnamed_252;
				precise float vertex_unnamed_256 = vertex_unnamed_216 * vertex_unnamed_253;
				precise float vertex_unnamed_257 = vertex_unnamed_217 * vertex_unnamed_254;
				precise float vertex_unnamed_258 = (-0.0f) - vertex_unnamed_255;
				precise float vertex_unnamed_260 = (-0.0f) - vertex_unnamed_256;
				precise float vertex_unnamed_261 = (-0.0f) - vertex_unnamed_257;
				precise float vertex_unnamed_270 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_271 = vertex_unnamed_270 * mad(vertex_unnamed_217, vertex_unnamed_253, vertex_unnamed_258);
				precise float vertex_unnamed_272 = vertex_unnamed_270 * mad(vertex_unnamed_215, vertex_unnamed_254, vertex_unnamed_260);
				precise float vertex_unnamed_273 = vertex_unnamed_270 * mad(vertex_unnamed_216, vertex_unnamed_252, vertex_unnamed_261);
				vertex_output_2.y = vertex_unnamed_271;
				vertex_output_2.x = vertex_unnamed_254;
				vertex_output_2.z = vertex_unnamed_216;
				vertex_output_3.x = vertex_unnamed_252;
				vertex_output_4.x = vertex_unnamed_253;
				vertex_output_3.z = vertex_unnamed_217;
				vertex_output_4.z = vertex_unnamed_215;
				vertex_output_3.y = vertex_unnamed_272;
				vertex_output_4.y = vertex_unnamed_273;
				vertex_output_5.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_86);
				vertex_output_5.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_87);
				vertex_output_5.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_88);
				float vertex_unnamed_304 = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_86);
				float vertex_unnamed_305 = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_87);
				float vertex_unnamed_306 = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_88);
				float vertex_unnamed_307 = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_6.x = vertex_input_7.x;
				vertex_output_6.y = vertex_input_7.y;
				vertex_output_6.z = vertex_input_7.z;
				vertex_output_6.w = vertex_input_7.w;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
				precise float vertex_unnamed_330 = vertex_unnamed_305 * vertex_uniform_buffer_0[5u].x;
				precise float vertex_unnamed_331 = vertex_unnamed_305 * vertex_uniform_buffer_0[5u].y;
				precise float vertex_unnamed_332 = vertex_unnamed_305 * vertex_uniform_buffer_0[5u].z;
				vertex_output_8.x = mad(vertex_uniform_buffer_0[7u].x, vertex_unnamed_307, mad(vertex_uniform_buffer_0[6u].x, vertex_unnamed_306, mad(vertex_uniform_buffer_0[4u].x, vertex_unnamed_304, vertex_unnamed_330)));
				vertex_output_8.y = mad(vertex_uniform_buffer_0[7u].y, vertex_unnamed_307, mad(vertex_uniform_buffer_0[6u].y, vertex_unnamed_306, mad(vertex_uniform_buffer_0[4u].y, vertex_unnamed_304, vertex_unnamed_331)));
				vertex_output_8.z = mad(vertex_uniform_buffer_0[7u].z, vertex_unnamed_307, mad(vertex_uniform_buffer_0[6u].z, vertex_unnamed_306, mad(vertex_uniform_buffer_0[4u].z, vertex_unnamed_304, vertex_unnamed_332)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_uniform_buffer_0[9] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			#endif // FOG_LINEAR
			#endif // POINT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef FOG_LINEAR
			#ifdef POINT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 unity_WorldToLight__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float vertex_output_1;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_input_4;
			static float4 vertex_output_7;
			static float3 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float vertex_output_1 : TEXCOORD7; // vs_TEXCOORD7
				float3 vertex_output_2 : TEXCOORD1; // vs_TEXCOORD1
				float3 vertex_output_3 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_4 : TEXCOORD3; // vs_TEXCOORD3
				float3 vertex_output_5 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_6 : UNKNOWN6;
				float4 vertex_output_7 : TEXCOORD5; // vs_TEXCOORD5
				float3 vertex_output_8 : TEXCOORD6; // vs_TEXCOORD6
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_45;
			static float4 vertex_unnamed_51;
			static float vertex_unnamed_129;
			static float3 vertex_unnamed_190;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_45 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_unnamed_51 = vertex_unnamed_45.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_51 = (unity_MatrixVP__array[0] * vertex_unnamed_45.xxxx) + vertex_unnamed_51;
				vertex_unnamed_51 = (unity_MatrixVP__array[2] * vertex_unnamed_45.zzzz) + vertex_unnamed_51;
				vertex_unnamed_45 = (unity_MatrixVP__array[3] * vertex_unnamed_45.wwww) + vertex_unnamed_51;
				gl_Position = vertex_unnamed_45;
				vertex_output_1 = vertex_unnamed_45.z;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_45.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_45.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_45.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_129 = dot(vertex_unnamed_45.xyz, vertex_unnamed_45.xyz);
				vertex_unnamed_129 = rsqrt(vertex_unnamed_129);
				float3 vertex_unnamed_141 = vertex_unnamed_129.xxx * vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_141.x, vertex_unnamed_141.y, vertex_unnamed_141.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_150 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_51 = float4(vertex_unnamed_150.x, vertex_unnamed_150.y, vertex_unnamed_150.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_161 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_161.x, vertex_unnamed_161.y, vertex_unnamed_161.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_172 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_172.x, vertex_unnamed_172.y, vertex_unnamed_172.z, vertex_unnamed_51.w);
				vertex_unnamed_129 = dot(vertex_unnamed_51.xyz, vertex_unnamed_51.xyz);
				vertex_unnamed_129 = rsqrt(vertex_unnamed_129);
				float3 vertex_unnamed_186 = vertex_unnamed_129.xxx * vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_186.x, vertex_unnamed_186.y, vertex_unnamed_186.z, vertex_unnamed_51.w);
				vertex_unnamed_190 = vertex_unnamed_45.xyz * vertex_unnamed_51.xyz;
				vertex_unnamed_190 = (vertex_unnamed_45.zxy * vertex_unnamed_51.yzx) + (-vertex_unnamed_190);
				vertex_unnamed_129 = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_190 = vertex_unnamed_129.xxx * vertex_unnamed_190;
				vertex_output_2.y = vertex_unnamed_190.x;
				vertex_output_2.x = vertex_unnamed_51.z;
				vertex_output_2.z = vertex_unnamed_45.y;
				vertex_output_3.x = vertex_unnamed_51.x;
				vertex_output_4.x = vertex_unnamed_51.y;
				vertex_output_3.z = vertex_unnamed_45.z;
				vertex_output_4.z = vertex_unnamed_45.x;
				vertex_output_3.y = vertex_unnamed_190.y;
				vertex_output_4.y = vertex_unnamed_190.z;
				vertex_output_5 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_9;
				vertex_output_6 = vertex_input_4;
				vertex_output_7 = 0.0f.xxxx;
				float3 vertex_unnamed_276 = vertex_unnamed_9.yyy * unity_WorldToLight__array[1].xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_276.x, vertex_unnamed_276.y, vertex_unnamed_276.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_287 = (unity_WorldToLight__array[0].xyz * vertex_unnamed_9.xxx) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_287.x, vertex_unnamed_287.y, vertex_unnamed_287.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_298 = (unity_WorldToLight__array[2].xyz * vertex_unnamed_9.zzz) + vertex_unnamed_45.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_298.x, vertex_unnamed_298.y, vertex_unnamed_298.z, vertex_unnamed_9.w);
				vertex_output_8 = (unity_WorldToLight__array[3].xyz * vertex_unnamed_9.www) + vertex_unnamed_9.xyz;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogParams;
			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;

			static float4 unity_WorldToLight__array[4];
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			Texture2D<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_output_0;
			static float fragment_input_1;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float fragment_input_1 : TEXCOORD7; // vs_TEXCOORD7
				float3 fragment_input_2 : TEXCOORD1; // vs_TEXCOORD1
				float3 fragment_input_3 : TEXCOORD2; // vs_TEXCOORD2
				float3 fragment_input_4 : TEXCOORD3; // vs_TEXCOORD3
				float3 fragment_input_5 : TEXCOORD4; // vs_TEXCOORD4
				float4 fragment_input_6 : UNKNOWN6;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float4 fragment_unnamed_9;
			static float fragment_unnamed_49;
			static float4 fragment_unnamed_64;
			static float fragment_unnamed_137;
			static float3 fragment_unnamed_143;
			static float fragment_unnamed_166;
			static float fragment_unnamed_221;
			static float fragment_unnamed_227;
			static float fragment_unnamed_240;
			static float fragment_unnamed_245;

			void frag_main()
			{
				float3 fragment_unnamed_26 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_26.x, fragment_unnamed_26.y, fragment_unnamed_26.z, fragment_unnamed_9.w);
				fragment_unnamed_9.x = fragment_unnamed_9.z * fragment_unnamed_9.x;
				float2 fragment_unnamed_46 = (fragment_unnamed_9.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_9 = float4(fragment_unnamed_46.x, fragment_unnamed_46.y, fragment_unnamed_9.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xy, fragment_unnamed_9.xy);
				fragment_unnamed_49 = min(fragment_unnamed_49, 1.0f);
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_9.z = sqrt(fragment_unnamed_49);
				fragment_unnamed_64.x = dot(fragment_input_2, fragment_unnamed_9.xyz);
				fragment_unnamed_64.y = dot(fragment_input_3, fragment_unnamed_9.xyz);
				fragment_unnamed_64.z = dot(fragment_input_4, fragment_unnamed_9.xyz);
				fragment_unnamed_9.x = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_9.x = rsqrt(fragment_unnamed_9.x);
				float3 fragment_unnamed_99 = fragment_unnamed_9.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_9 = float4(fragment_unnamed_99.x, fragment_unnamed_99.y, fragment_unnamed_99.z, fragment_unnamed_9.w);
				float3 fragment_unnamed_115 = (-fragment_input_5) + _WorldSpaceCameraPos;
				fragment_unnamed_64 = float4(fragment_unnamed_115.x, fragment_unnamed_115.y, fragment_unnamed_115.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_49 = rsqrt(fragment_unnamed_49);
				float3 fragment_unnamed_129 = fragment_unnamed_49.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_129.x, fragment_unnamed_129.y, fragment_unnamed_129.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_137 = (-abs(fragment_unnamed_49)) + 1.0f;
				fragment_unnamed_143.x = fragment_unnamed_137 * fragment_unnamed_137;
				fragment_unnamed_143.x *= fragment_unnamed_143.x;
				fragment_unnamed_137 *= fragment_unnamed_143.x;
				fragment_unnamed_143 = (-fragment_input_5) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_166 = dot(fragment_unnamed_143, fragment_unnamed_143);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_178 = (fragment_unnamed_143 * fragment_unnamed_166.xxx) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_178.x, fragment_unnamed_178.y, fragment_unnamed_178.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_166.xxx * fragment_unnamed_143;
				fragment_unnamed_166 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_166 = max(fragment_unnamed_166, 0.001000000047497451305389404296875f);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_199 = fragment_unnamed_64.xyz * fragment_unnamed_166.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_199.x, fragment_unnamed_199.y, fragment_unnamed_199.z, fragment_unnamed_64.w);
				fragment_unnamed_64.x = dot(fragment_unnamed_143, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = clamp(fragment_unnamed_64.x, 0.0f, 1.0f);
				fragment_unnamed_9.x = dot(fragment_unnamed_9.xyz, fragment_unnamed_143);
				fragment_unnamed_9.x = clamp(fragment_unnamed_9.x, 0.0f, 1.0f);
				fragment_unnamed_221 = dot(fragment_unnamed_64.xx, fragment_unnamed_64.xx);
				fragment_unnamed_227 = (-fragment_unnamed_64.x) + 1.0f;
				fragment_unnamed_221 += (-0.5f);
				fragment_unnamed_64.x = (fragment_unnamed_221 * fragment_unnamed_137) + 1.0f;
				fragment_unnamed_240 = (-fragment_unnamed_9.x) + 1.0f;
				fragment_unnamed_245 = fragment_unnamed_240 * fragment_unnamed_240;
				fragment_unnamed_245 *= fragment_unnamed_245;
				fragment_unnamed_240 *= fragment_unnamed_245;
				fragment_unnamed_221 = (fragment_unnamed_221 * fragment_unnamed_240) + 1.0f;
				fragment_unnamed_221 = fragment_unnamed_64.x * fragment_unnamed_221;
				fragment_unnamed_221 = fragment_unnamed_9.x * fragment_unnamed_221;
				float3 fragment_unnamed_274 = fragment_input_5.yyy * unity_WorldToLight__array[1].xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_274.x, fragment_unnamed_274.y, fragment_unnamed_274.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_285 = (unity_WorldToLight__array[0].xyz * fragment_input_5.xxx) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_285.x, fragment_unnamed_285.y, fragment_unnamed_285.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_296 = (unity_WorldToLight__array[2].xyz * fragment_input_5.zzz) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_296.x, fragment_unnamed_296.y, fragment_unnamed_296.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_305 = fragment_unnamed_64.xyz + unity_WorldToLight__array[3].xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_305.x, fragment_unnamed_305.y, fragment_unnamed_305.z, fragment_unnamed_64.w);
				fragment_unnamed_64.x = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = _LightTexture0.Sample(sampler_LightTexture0, fragment_unnamed_64.xx).x;
				float3 fragment_unnamed_330 = fragment_unnamed_64.xxx * _LightColor0.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_330.x, fragment_unnamed_330.y, fragment_unnamed_330.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_221.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_221 = abs(fragment_unnamed_49) + fragment_unnamed_9.x;
				fragment_unnamed_221 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_221 = 0.5f / fragment_unnamed_221;
				fragment_unnamed_9.x *= fragment_unnamed_221;
				fragment_unnamed_9.x *= 0.99999988079071044921875f;
				float3 fragment_unnamed_363 = fragment_unnamed_64.xyz * fragment_unnamed_9.xxx;
				fragment_unnamed_9 = float4(fragment_unnamed_363.x, fragment_unnamed_363.y, fragment_unnamed_9.z, fragment_unnamed_363.z);
				fragment_unnamed_64.x = fragment_unnamed_227 * fragment_unnamed_227;
				fragment_unnamed_64.x *= fragment_unnamed_64.x;
				fragment_unnamed_227 *= fragment_unnamed_64.x;
				fragment_unnamed_227 = (fragment_unnamed_227 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				float3 fragment_unnamed_389 = fragment_unnamed_227.xxx * fragment_unnamed_9.xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_389.x, fragment_unnamed_389.y, fragment_unnamed_389.z, fragment_unnamed_9.w);
				fragment_unnamed_64 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_64 *= _Color;
				float3 fragment_unnamed_410 = fragment_unnamed_64.xyz * fragment_input_6.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_410.x, fragment_unnamed_410.y, fragment_unnamed_410.z, fragment_unnamed_64.w);
				fragment_output_0.w = fragment_unnamed_64.w * fragment_input_6.w;
				float3 fragment_unnamed_427 = fragment_unnamed_64.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_427.x, fragment_unnamed_427.y, fragment_unnamed_427.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_436 = (fragment_unnamed_64.xyz * fragment_unnamed_143) + fragment_unnamed_9.xyz;
				fragment_unnamed_9 = float4(fragment_unnamed_436.x, fragment_unnamed_436.y, fragment_unnamed_436.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = fragment_input_1 / _ProjectionParams.y;
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_49 *= _ProjectionParams.z;
				fragment_unnamed_49 = max(fragment_unnamed_49, 0.0f);
				fragment_unnamed_49 = (fragment_unnamed_49 * unity_FogParams.z) + unity_FogParams.w;
				fragment_unnamed_49 = clamp(fragment_unnamed_49, 0.0f, 1.0f);
				float3 fragment_unnamed_467 = fragment_unnamed_9.xyz * fragment_unnamed_49.xxx;
				fragment_output_0 = float4(fragment_unnamed_467.x, fragment_unnamed_467.y, fragment_unnamed_467.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_1 = stage_input.fragment_input_1;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FOG_LINEAR
			#endif // POINT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[6];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float vertex_output_1 : TEXCOORD7; // TEXCOORD_7
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float3 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float3 vertex_output_5 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_6 : COLOR; // COLOR
				float4 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_61 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_62 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_63 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_86 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_60));
				float vertex_unnamed_87 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_61));
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_62));
				precise float vertex_unnamed_97 = vertex_unnamed_86 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_98 = vertex_unnamed_87 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_99 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_100 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_63)) + vertex_uniform_buffer_1[3u].w;
				vertex_output_5.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_86);
				vertex_output_5.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_87);
				vertex_output_5.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_88);
				precise float vertex_unnamed_121 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_122 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_123 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_124 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].w;
				float vertex_unnamed_156 = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_97, vertex_unnamed_123)));
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_97, vertex_unnamed_121)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_97, vertex_unnamed_122)));
				gl_Position.z = vertex_unnamed_156;
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_97, vertex_unnamed_124)));
				vertex_output_1 = vertex_unnamed_156;
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[5u].x, vertex_uniform_buffer_0[5u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[5u].y, vertex_uniform_buffer_0[5u].w);
				float vertex_unnamed_191 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_205 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_219 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_225 = rsqrt(dot(float3(vertex_unnamed_219, vertex_unnamed_191, vertex_unnamed_205), float3(vertex_unnamed_219, vertex_unnamed_191, vertex_unnamed_205)));
				precise float vertex_unnamed_226 = vertex_unnamed_225 * vertex_unnamed_219;
				precise float vertex_unnamed_227 = vertex_unnamed_225 * vertex_unnamed_191;
				precise float vertex_unnamed_228 = vertex_unnamed_225 * vertex_unnamed_205;
				precise float vertex_unnamed_236 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_237 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_238 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_256 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_236));
				float vertex_unnamed_257 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_237));
				float vertex_unnamed_258 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_238));
				float vertex_unnamed_262 = rsqrt(dot(float3(vertex_unnamed_256, vertex_unnamed_257, vertex_unnamed_258), float3(vertex_unnamed_256, vertex_unnamed_257, vertex_unnamed_258)));
				precise float vertex_unnamed_263 = vertex_unnamed_262 * vertex_unnamed_256;
				precise float vertex_unnamed_264 = vertex_unnamed_262 * vertex_unnamed_257;
				precise float vertex_unnamed_265 = vertex_unnamed_262 * vertex_unnamed_258;
				precise float vertex_unnamed_266 = vertex_unnamed_226 * vertex_unnamed_263;
				precise float vertex_unnamed_267 = vertex_unnamed_227 * vertex_unnamed_264;
				precise float vertex_unnamed_268 = vertex_unnamed_228 * vertex_unnamed_265;
				precise float vertex_unnamed_269 = (-0.0f) - vertex_unnamed_266;
				precise float vertex_unnamed_271 = (-0.0f) - vertex_unnamed_267;
				precise float vertex_unnamed_272 = (-0.0f) - vertex_unnamed_268;
				precise float vertex_unnamed_282 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_283 = vertex_unnamed_282 * mad(vertex_unnamed_228, vertex_unnamed_264, vertex_unnamed_269);
				precise float vertex_unnamed_284 = vertex_unnamed_282 * mad(vertex_unnamed_226, vertex_unnamed_265, vertex_unnamed_271);
				precise float vertex_unnamed_285 = vertex_unnamed_282 * mad(vertex_unnamed_227, vertex_unnamed_263, vertex_unnamed_272);
				vertex_output_2.y = vertex_unnamed_283;
				vertex_output_2.x = vertex_unnamed_265;
				vertex_output_2.z = vertex_unnamed_227;
				vertex_output_3.x = vertex_unnamed_263;
				vertex_output_4.x = vertex_unnamed_264;
				vertex_output_3.z = vertex_unnamed_228;
				vertex_output_4.z = vertex_unnamed_226;
				vertex_output_3.y = vertex_unnamed_284;
				vertex_output_4.y = vertex_unnamed_285;
				vertex_output_6.x = vertex_input_7.x;
				vertex_output_6.y = vertex_input_7.y;
				vertex_output_6.z = vertex_input_7.z;
				vertex_output_6.w = vertex_input_7.w;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[5] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float3 vertex_output_5;
			static float vertex_output_1;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float4 vertex_output_6;
			static float4 vertex_input_4;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float vertex_output_1 : TEXCOORD7; // vs_TEXCOORD7
				float3 vertex_output_2 : TEXCOORD1; // vs_TEXCOORD1
				float3 vertex_output_3 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_4 : TEXCOORD3; // vs_TEXCOORD3
				float3 vertex_output_5 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_6 : UNKNOWN6;
				float4 vertex_output_7 : TEXCOORD5; // vs_TEXCOORD5
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_44;
			static float vertex_unnamed_138;
			static float3 vertex_unnamed_199;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_44 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_output_5 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = vertex_unnamed_44.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_9 = (unity_MatrixVP__array[0] * vertex_unnamed_44.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_MatrixVP__array[2] * vertex_unnamed_44.zzzz) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_MatrixVP__array[3] * vertex_unnamed_44.wwww) + vertex_unnamed_9;
				gl_Position = vertex_unnamed_9;
				vertex_output_1 = vertex_unnamed_9.z;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_9.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_9.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_9.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_138 = dot(vertex_unnamed_9.xyz, vertex_unnamed_9.xyz);
				vertex_unnamed_138 = rsqrt(vertex_unnamed_138);
				float3 vertex_unnamed_150 = vertex_unnamed_138.xxx * vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_150.x, vertex_unnamed_150.y, vertex_unnamed_150.z, vertex_unnamed_9.w);
				float3 vertex_unnamed_159 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_44 = float4(vertex_unnamed_159.x, vertex_unnamed_159.y, vertex_unnamed_159.z, vertex_unnamed_44.w);
				float3 vertex_unnamed_170 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_44.xyz;
				vertex_unnamed_44 = float4(vertex_unnamed_170.x, vertex_unnamed_170.y, vertex_unnamed_170.z, vertex_unnamed_44.w);
				float3 vertex_unnamed_181 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_44.xyz;
				vertex_unnamed_44 = float4(vertex_unnamed_181.x, vertex_unnamed_181.y, vertex_unnamed_181.z, vertex_unnamed_44.w);
				vertex_unnamed_138 = dot(vertex_unnamed_44.xyz, vertex_unnamed_44.xyz);
				vertex_unnamed_138 = rsqrt(vertex_unnamed_138);
				float3 vertex_unnamed_195 = vertex_unnamed_138.xxx * vertex_unnamed_44.xyz;
				vertex_unnamed_44 = float4(vertex_unnamed_195.x, vertex_unnamed_195.y, vertex_unnamed_195.z, vertex_unnamed_44.w);
				vertex_unnamed_199 = vertex_unnamed_9.xyz * vertex_unnamed_44.xyz;
				vertex_unnamed_199 = (vertex_unnamed_9.zxy * vertex_unnamed_44.yzx) + (-vertex_unnamed_199);
				vertex_unnamed_138 = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_199 = vertex_unnamed_138.xxx * vertex_unnamed_199;
				vertex_output_2.y = vertex_unnamed_199.x;
				vertex_output_2.x = vertex_unnamed_44.z;
				vertex_output_2.z = vertex_unnamed_9.y;
				vertex_output_3.x = vertex_unnamed_44.x;
				vertex_output_4.x = vertex_unnamed_44.y;
				vertex_output_3.z = vertex_unnamed_9.z;
				vertex_output_4.z = vertex_unnamed_9.x;
				vertex_output_3.y = vertex_unnamed_199.y;
				vertex_output_4.y = vertex_unnamed_199.z;
				vertex_output_6 = vertex_input_4;
				vertex_output_7 = 0.0f.xxxx;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogParams;
			float4 _LightColor0;
			float4 _Color;

			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_output_0;
			static float fragment_input_1;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float fragment_input_1 : TEXCOORD7; // vs_TEXCOORD7
				float3 fragment_input_2 : TEXCOORD1; // vs_TEXCOORD1
				float3 fragment_input_3 : TEXCOORD2; // vs_TEXCOORD2
				float3 fragment_input_4 : TEXCOORD3; // vs_TEXCOORD3
				float3 fragment_input_5 : TEXCOORD4; // vs_TEXCOORD4
				float4 fragment_input_6 : UNKNOWN6;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float4 fragment_unnamed_9;
			static float fragment_unnamed_49;
			static float3 fragment_unnamed_65;
			static float4 fragment_unnamed_117;
			static float fragment_unnamed_151;
			static float fragment_unnamed_156;
			static float fragment_unnamed_232;

			void frag_main()
			{
				float3 fragment_unnamed_26 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_26.x, fragment_unnamed_26.y, fragment_unnamed_26.z, fragment_unnamed_9.w);
				fragment_unnamed_9.x = fragment_unnamed_9.z * fragment_unnamed_9.x;
				float2 fragment_unnamed_46 = (fragment_unnamed_9.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_9 = float4(fragment_unnamed_46.x, fragment_unnamed_46.y, fragment_unnamed_9.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xy, fragment_unnamed_9.xy);
				fragment_unnamed_49 = min(fragment_unnamed_49, 1.0f);
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_9.z = sqrt(fragment_unnamed_49);
				fragment_unnamed_65.x = dot(fragment_input_2, fragment_unnamed_9.xyz);
				fragment_unnamed_65.y = dot(fragment_input_3, fragment_unnamed_9.xyz);
				fragment_unnamed_65.z = dot(fragment_input_4, fragment_unnamed_9.xyz);
				fragment_unnamed_9.x = dot(fragment_unnamed_65, fragment_unnamed_65);
				fragment_unnamed_9.x = rsqrt(fragment_unnamed_9.x);
				float3 fragment_unnamed_97 = fragment_unnamed_9.xxx * fragment_unnamed_65;
				fragment_unnamed_9 = float4(fragment_unnamed_97.x, fragment_unnamed_97.y, fragment_unnamed_97.z, fragment_unnamed_9.w);
				fragment_unnamed_65 = (-fragment_input_5) + _WorldSpaceCameraPos;
				fragment_unnamed_49 = dot(fragment_unnamed_65, fragment_unnamed_65);
				fragment_unnamed_49 = rsqrt(fragment_unnamed_49);
				float3 fragment_unnamed_121 = fragment_unnamed_49.xxx * fragment_unnamed_65;
				fragment_unnamed_117 = float4(fragment_unnamed_121.x, fragment_unnamed_121.y, fragment_unnamed_121.z, fragment_unnamed_117.w);
				fragment_unnamed_65 = (fragment_unnamed_65 * fragment_unnamed_49.xxx) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_49 = dot(fragment_unnamed_9.xyz, fragment_unnamed_117.xyz);
				fragment_unnamed_9.x = dot(fragment_unnamed_9.xyz, _WorldSpaceLightPos0.xyz);
				fragment_unnamed_9.x = clamp(fragment_unnamed_9.x, 0.0f, 1.0f);
				fragment_unnamed_151 = (-abs(fragment_unnamed_49)) + 1.0f;
				fragment_unnamed_156 = abs(fragment_unnamed_49) + fragment_unnamed_9.x;
				fragment_unnamed_156 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_156 = 0.5f / fragment_unnamed_156;
				fragment_unnamed_156 = fragment_unnamed_9.x * fragment_unnamed_156;
				fragment_unnamed_156 *= 0.99999988079071044921875f;
				float3 fragment_unnamed_181 = fragment_unnamed_156.xxx * _LightColor0.xyz;
				fragment_unnamed_117 = float4(fragment_unnamed_181.x, fragment_unnamed_181.y, fragment_unnamed_181.z, fragment_unnamed_117.w);
				fragment_unnamed_156 = fragment_unnamed_151 * fragment_unnamed_151;
				fragment_unnamed_156 *= fragment_unnamed_156;
				fragment_unnamed_151 *= fragment_unnamed_156;
				fragment_unnamed_156 = dot(fragment_unnamed_65, fragment_unnamed_65);
				fragment_unnamed_156 = max(fragment_unnamed_156, 0.001000000047497451305389404296875f);
				fragment_unnamed_156 = rsqrt(fragment_unnamed_156);
				fragment_unnamed_65 = fragment_unnamed_156.xxx * fragment_unnamed_65;
				fragment_unnamed_156 = dot(_WorldSpaceLightPos0.xyz, fragment_unnamed_65);
				fragment_unnamed_156 = clamp(fragment_unnamed_156, 0.0f, 1.0f);
				fragment_unnamed_49 = dot(fragment_unnamed_156.xx, fragment_unnamed_156.xx);
				fragment_unnamed_156 = (-fragment_unnamed_156) + 1.0f;
				fragment_unnamed_49 += (-0.5f);
				fragment_unnamed_151 = (fragment_unnamed_49 * fragment_unnamed_151) + 1.0f;
				fragment_unnamed_65.x = (-fragment_unnamed_9.x) + 1.0f;
				fragment_unnamed_232 = fragment_unnamed_65.x * fragment_unnamed_65.x;
				fragment_unnamed_232 *= fragment_unnamed_232;
				fragment_unnamed_65.x *= fragment_unnamed_232;
				fragment_unnamed_49 = (fragment_unnamed_49 * fragment_unnamed_65.x) + 1.0f;
				fragment_unnamed_151 *= fragment_unnamed_49;
				fragment_unnamed_9.x *= fragment_unnamed_151;
				float3 fragment_unnamed_264 = fragment_unnamed_9.xxx * _LightColor0.xyz;
				fragment_unnamed_9 = float4(fragment_unnamed_264.x, fragment_unnamed_264.y, fragment_unnamed_9.z, fragment_unnamed_264.z);
				fragment_unnamed_65.x = fragment_unnamed_156 * fragment_unnamed_156;
				fragment_unnamed_65.x *= fragment_unnamed_65.x;
				fragment_unnamed_156 *= fragment_unnamed_65.x;
				fragment_unnamed_156 = (fragment_unnamed_156 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				fragment_unnamed_65 = fragment_unnamed_156.xxx * fragment_unnamed_117.xyz;
				fragment_unnamed_117 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_117 *= _Color;
				float3 fragment_unnamed_309 = fragment_unnamed_117.xyz * fragment_input_6.xyz;
				fragment_unnamed_117 = float4(fragment_unnamed_309.x, fragment_unnamed_309.y, fragment_unnamed_309.z, fragment_unnamed_117.w);
				fragment_output_0.w = fragment_unnamed_117.w * fragment_input_6.w;
				float3 fragment_unnamed_326 = fragment_unnamed_117.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_117 = float4(fragment_unnamed_326.x, fragment_unnamed_326.y, fragment_unnamed_326.z, fragment_unnamed_117.w);
				float3 fragment_unnamed_335 = (fragment_unnamed_117.xyz * fragment_unnamed_9.xyw) + fragment_unnamed_65;
				fragment_unnamed_9 = float4(fragment_unnamed_335.x, fragment_unnamed_335.y, fragment_unnamed_335.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = fragment_input_1 / _ProjectionParams.y;
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_49 *= _ProjectionParams.z;
				fragment_unnamed_49 = max(fragment_unnamed_49, 0.0f);
				fragment_unnamed_49 = (fragment_unnamed_49 * unity_FogParams.z) + unity_FogParams.w;
				fragment_unnamed_49 = clamp(fragment_unnamed_49, 0.0f, 1.0f);
				float3 fragment_unnamed_368 = fragment_unnamed_9.xyz * fragment_unnamed_49.xxx;
				fragment_output_0 = float4(fragment_unnamed_368.x, fragment_unnamed_368.y, fragment_unnamed_368.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_1 = stage_input.fragment_input_1;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef FOG_LINEAR
			#ifdef SPOT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT
			#ifndef POINT_COOKIE
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[10];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;
			static float4 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float vertex_output_1 : TEXCOORD7; // TEXCOORD_7
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float3 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float3 vertex_output_5 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_6 : COLOR; // COLOR
				float4 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float4 vertex_output_8 : TEXCOORD6; // TEXCOORD_6
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_61 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_62 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_63 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_86 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_60));
				float vertex_unnamed_87 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_61));
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_62));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_63));
				precise float vertex_unnamed_97 = vertex_unnamed_86 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_98 = vertex_unnamed_87 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_99 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_100 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_108 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_109 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_110 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_111 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].w;
				float vertex_unnamed_143 = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_97, vertex_unnamed_110)));
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_97, vertex_unnamed_108)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_97, vertex_unnamed_109)));
				gl_Position.z = vertex_unnamed_143;
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_97, vertex_unnamed_111)));
				vertex_output_1 = vertex_unnamed_143;
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[9u].x, vertex_uniform_buffer_0[9u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[9u].y, vertex_uniform_buffer_0[9u].w);
				float vertex_unnamed_178 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_193 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_208 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_214 = rsqrt(dot(float3(vertex_unnamed_208, vertex_unnamed_178, vertex_unnamed_193), float3(vertex_unnamed_208, vertex_unnamed_178, vertex_unnamed_193)));
				precise float vertex_unnamed_215 = vertex_unnamed_214 * vertex_unnamed_208;
				precise float vertex_unnamed_216 = vertex_unnamed_214 * vertex_unnamed_178;
				precise float vertex_unnamed_217 = vertex_unnamed_214 * vertex_unnamed_193;
				precise float vertex_unnamed_225 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_226 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_227 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_245 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_225));
				float vertex_unnamed_246 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_226));
				float vertex_unnamed_247 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_227));
				float vertex_unnamed_251 = rsqrt(dot(float3(vertex_unnamed_245, vertex_unnamed_246, vertex_unnamed_247), float3(vertex_unnamed_245, vertex_unnamed_246, vertex_unnamed_247)));
				precise float vertex_unnamed_252 = vertex_unnamed_251 * vertex_unnamed_245;
				precise float vertex_unnamed_253 = vertex_unnamed_251 * vertex_unnamed_246;
				precise float vertex_unnamed_254 = vertex_unnamed_251 * vertex_unnamed_247;
				precise float vertex_unnamed_255 = vertex_unnamed_215 * vertex_unnamed_252;
				precise float vertex_unnamed_256 = vertex_unnamed_216 * vertex_unnamed_253;
				precise float vertex_unnamed_257 = vertex_unnamed_217 * vertex_unnamed_254;
				precise float vertex_unnamed_258 = (-0.0f) - vertex_unnamed_255;
				precise float vertex_unnamed_260 = (-0.0f) - vertex_unnamed_256;
				precise float vertex_unnamed_261 = (-0.0f) - vertex_unnamed_257;
				precise float vertex_unnamed_270 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_271 = vertex_unnamed_270 * mad(vertex_unnamed_217, vertex_unnamed_253, vertex_unnamed_258);
				precise float vertex_unnamed_272 = vertex_unnamed_270 * mad(vertex_unnamed_215, vertex_unnamed_254, vertex_unnamed_260);
				precise float vertex_unnamed_273 = vertex_unnamed_270 * mad(vertex_unnamed_216, vertex_unnamed_252, vertex_unnamed_261);
				vertex_output_2.y = vertex_unnamed_271;
				vertex_output_2.x = vertex_unnamed_254;
				vertex_output_2.z = vertex_unnamed_216;
				vertex_output_3.x = vertex_unnamed_252;
				vertex_output_4.x = vertex_unnamed_253;
				vertex_output_3.z = vertex_unnamed_217;
				vertex_output_4.z = vertex_unnamed_215;
				vertex_output_3.y = vertex_unnamed_272;
				vertex_output_4.y = vertex_unnamed_273;
				vertex_output_5.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_86);
				vertex_output_5.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_87);
				vertex_output_5.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_88);
				float vertex_unnamed_304 = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_86);
				float vertex_unnamed_305 = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_87);
				float vertex_unnamed_306 = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_88);
				float vertex_unnamed_307 = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_6.x = vertex_input_7.x;
				vertex_output_6.y = vertex_input_7.y;
				vertex_output_6.z = vertex_input_7.z;
				vertex_output_6.w = vertex_input_7.w;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
				precise float vertex_unnamed_331 = vertex_unnamed_305 * vertex_uniform_buffer_0[5u].x;
				precise float vertex_unnamed_332 = vertex_unnamed_305 * vertex_uniform_buffer_0[5u].y;
				precise float vertex_unnamed_333 = vertex_unnamed_305 * vertex_uniform_buffer_0[5u].z;
				precise float vertex_unnamed_334 = vertex_unnamed_305 * vertex_uniform_buffer_0[5u].w;
				vertex_output_8.x = mad(vertex_uniform_buffer_0[7u].x, vertex_unnamed_307, mad(vertex_uniform_buffer_0[6u].x, vertex_unnamed_306, mad(vertex_uniform_buffer_0[4u].x, vertex_unnamed_304, vertex_unnamed_331)));
				vertex_output_8.y = mad(vertex_uniform_buffer_0[7u].y, vertex_unnamed_307, mad(vertex_uniform_buffer_0[6u].y, vertex_unnamed_306, mad(vertex_uniform_buffer_0[4u].y, vertex_unnamed_304, vertex_unnamed_332)));
				vertex_output_8.z = mad(vertex_uniform_buffer_0[7u].z, vertex_unnamed_307, mad(vertex_uniform_buffer_0[6u].z, vertex_unnamed_306, mad(vertex_uniform_buffer_0[4u].z, vertex_unnamed_304, vertex_unnamed_333)));
				vertex_output_8.w = mad(vertex_uniform_buffer_0[7u].w, vertex_unnamed_307, mad(vertex_uniform_buffer_0[6u].w, vertex_unnamed_306, mad(vertex_uniform_buffer_0[4u].w, vertex_unnamed_304, vertex_unnamed_334)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_uniform_buffer_0[9] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			#endif // FOG_LINEAR
			#endif // SPOT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT
			#endif // !POINT_COOKIE


			#ifdef FOG_LINEAR
			#ifdef SPOT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT
			#ifndef POINT_COOKIE
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 unity_WorldToLight__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float vertex_output_1;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_input_4;
			static float4 vertex_output_7;
			static float4 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float vertex_output_1 : TEXCOORD7; // vs_TEXCOORD7
				float3 vertex_output_2 : TEXCOORD1; // vs_TEXCOORD1
				float3 vertex_output_3 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_4 : TEXCOORD3; // vs_TEXCOORD3
				float3 vertex_output_5 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_6 : UNKNOWN6;
				float4 vertex_output_7 : TEXCOORD5; // vs_TEXCOORD5
				float4 vertex_output_8 : TEXCOORD6; // vs_TEXCOORD6
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_45;
			static float4 vertex_unnamed_51;
			static float vertex_unnamed_129;
			static float3 vertex_unnamed_190;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_45 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_unnamed_51 = vertex_unnamed_45.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_51 = (unity_MatrixVP__array[0] * vertex_unnamed_45.xxxx) + vertex_unnamed_51;
				vertex_unnamed_51 = (unity_MatrixVP__array[2] * vertex_unnamed_45.zzzz) + vertex_unnamed_51;
				vertex_unnamed_45 = (unity_MatrixVP__array[3] * vertex_unnamed_45.wwww) + vertex_unnamed_51;
				gl_Position = vertex_unnamed_45;
				vertex_output_1 = vertex_unnamed_45.z;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_45.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_45.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_45.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_129 = dot(vertex_unnamed_45.xyz, vertex_unnamed_45.xyz);
				vertex_unnamed_129 = rsqrt(vertex_unnamed_129);
				float3 vertex_unnamed_141 = vertex_unnamed_129.xxx * vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_141.x, vertex_unnamed_141.y, vertex_unnamed_141.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_150 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_51 = float4(vertex_unnamed_150.x, vertex_unnamed_150.y, vertex_unnamed_150.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_161 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_161.x, vertex_unnamed_161.y, vertex_unnamed_161.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_172 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_172.x, vertex_unnamed_172.y, vertex_unnamed_172.z, vertex_unnamed_51.w);
				vertex_unnamed_129 = dot(vertex_unnamed_51.xyz, vertex_unnamed_51.xyz);
				vertex_unnamed_129 = rsqrt(vertex_unnamed_129);
				float3 vertex_unnamed_186 = vertex_unnamed_129.xxx * vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_186.x, vertex_unnamed_186.y, vertex_unnamed_186.z, vertex_unnamed_51.w);
				vertex_unnamed_190 = vertex_unnamed_45.xyz * vertex_unnamed_51.xyz;
				vertex_unnamed_190 = (vertex_unnamed_45.zxy * vertex_unnamed_51.yzx) + (-vertex_unnamed_190);
				vertex_unnamed_129 = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_190 = vertex_unnamed_129.xxx * vertex_unnamed_190;
				vertex_output_2.y = vertex_unnamed_190.x;
				vertex_output_2.x = vertex_unnamed_51.z;
				vertex_output_2.z = vertex_unnamed_45.y;
				vertex_output_3.x = vertex_unnamed_51.x;
				vertex_output_4.x = vertex_unnamed_51.y;
				vertex_output_3.z = vertex_unnamed_45.z;
				vertex_output_4.z = vertex_unnamed_45.x;
				vertex_output_3.y = vertex_unnamed_190.y;
				vertex_output_4.y = vertex_unnamed_190.z;
				vertex_output_5 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_9;
				vertex_output_6 = vertex_input_4;
				vertex_output_7 = 0.0f.xxxx;
				vertex_unnamed_45 = vertex_unnamed_9.yyyy * unity_WorldToLight__array[1];
				vertex_unnamed_45 = (unity_WorldToLight__array[0] * vertex_unnamed_9.xxxx) + vertex_unnamed_45;
				vertex_unnamed_45 = (unity_WorldToLight__array[2] * vertex_unnamed_9.zzzz) + vertex_unnamed_45;
				vertex_output_8 = (unity_WorldToLight__array[3] * vertex_unnamed_9.wwww) + vertex_unnamed_45;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogParams;
			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;

			static float4 unity_WorldToLight__array[4];
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			Texture2D<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			Texture2D<float4> _LightTextureB0;
			SamplerState sampler_LightTextureB0;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_output_0;
			static float fragment_input_1;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float fragment_input_1 : TEXCOORD7; // vs_TEXCOORD7
				float3 fragment_input_2 : TEXCOORD1; // vs_TEXCOORD1
				float3 fragment_input_3 : TEXCOORD2; // vs_TEXCOORD2
				float3 fragment_input_4 : TEXCOORD3; // vs_TEXCOORD3
				float3 fragment_input_5 : TEXCOORD4; // vs_TEXCOORD4
				float4 fragment_input_6 : UNKNOWN6;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float4 fragment_unnamed_9;
			static float fragment_unnamed_49;
			static float4 fragment_unnamed_64;
			static float fragment_unnamed_137;
			static float3 fragment_unnamed_143;
			static float fragment_unnamed_166;
			static float fragment_unnamed_221;
			static float fragment_unnamed_227;
			static float fragment_unnamed_240;
			static float fragment_unnamed_245;
			static bool fragment_unnamed_319;

			void frag_main()
			{
				float3 fragment_unnamed_26 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_26.x, fragment_unnamed_26.y, fragment_unnamed_26.z, fragment_unnamed_9.w);
				fragment_unnamed_9.x = fragment_unnamed_9.z * fragment_unnamed_9.x;
				float2 fragment_unnamed_46 = (fragment_unnamed_9.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_9 = float4(fragment_unnamed_46.x, fragment_unnamed_46.y, fragment_unnamed_9.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xy, fragment_unnamed_9.xy);
				fragment_unnamed_49 = min(fragment_unnamed_49, 1.0f);
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_9.z = sqrt(fragment_unnamed_49);
				fragment_unnamed_64.x = dot(fragment_input_2, fragment_unnamed_9.xyz);
				fragment_unnamed_64.y = dot(fragment_input_3, fragment_unnamed_9.xyz);
				fragment_unnamed_64.z = dot(fragment_input_4, fragment_unnamed_9.xyz);
				fragment_unnamed_9.x = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_9.x = rsqrt(fragment_unnamed_9.x);
				float3 fragment_unnamed_99 = fragment_unnamed_9.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_9 = float4(fragment_unnamed_99.x, fragment_unnamed_99.y, fragment_unnamed_99.z, fragment_unnamed_9.w);
				float3 fragment_unnamed_115 = (-fragment_input_5) + _WorldSpaceCameraPos;
				fragment_unnamed_64 = float4(fragment_unnamed_115.x, fragment_unnamed_115.y, fragment_unnamed_115.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_49 = rsqrt(fragment_unnamed_49);
				float3 fragment_unnamed_129 = fragment_unnamed_49.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_129.x, fragment_unnamed_129.y, fragment_unnamed_129.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_137 = (-abs(fragment_unnamed_49)) + 1.0f;
				fragment_unnamed_143.x = fragment_unnamed_137 * fragment_unnamed_137;
				fragment_unnamed_143.x *= fragment_unnamed_143.x;
				fragment_unnamed_137 *= fragment_unnamed_143.x;
				fragment_unnamed_143 = (-fragment_input_5) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_166 = dot(fragment_unnamed_143, fragment_unnamed_143);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_178 = (fragment_unnamed_143 * fragment_unnamed_166.xxx) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_178.x, fragment_unnamed_178.y, fragment_unnamed_178.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_166.xxx * fragment_unnamed_143;
				fragment_unnamed_166 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_166 = max(fragment_unnamed_166, 0.001000000047497451305389404296875f);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_199 = fragment_unnamed_64.xyz * fragment_unnamed_166.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_199.x, fragment_unnamed_199.y, fragment_unnamed_199.z, fragment_unnamed_64.w);
				fragment_unnamed_64.x = dot(fragment_unnamed_143, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = clamp(fragment_unnamed_64.x, 0.0f, 1.0f);
				fragment_unnamed_9.x = dot(fragment_unnamed_9.xyz, fragment_unnamed_143);
				fragment_unnamed_9.x = clamp(fragment_unnamed_9.x, 0.0f, 1.0f);
				fragment_unnamed_221 = dot(fragment_unnamed_64.xx, fragment_unnamed_64.xx);
				fragment_unnamed_227 = (-fragment_unnamed_64.x) + 1.0f;
				fragment_unnamed_221 += (-0.5f);
				fragment_unnamed_64.x = (fragment_unnamed_221 * fragment_unnamed_137) + 1.0f;
				fragment_unnamed_240 = (-fragment_unnamed_9.x) + 1.0f;
				fragment_unnamed_245 = fragment_unnamed_240 * fragment_unnamed_240;
				fragment_unnamed_245 *= fragment_unnamed_245;
				fragment_unnamed_240 *= fragment_unnamed_245;
				fragment_unnamed_221 = (fragment_unnamed_221 * fragment_unnamed_240) + 1.0f;
				fragment_unnamed_221 = fragment_unnamed_64.x * fragment_unnamed_221;
				fragment_unnamed_221 = fragment_unnamed_9.x * fragment_unnamed_221;
				fragment_unnamed_64 = fragment_input_5.yyyy * unity_WorldToLight__array[1];
				fragment_unnamed_64 = (unity_WorldToLight__array[0] * fragment_input_5.xxxx) + fragment_unnamed_64;
				fragment_unnamed_64 = (unity_WorldToLight__array[2] * fragment_input_5.zzzz) + fragment_unnamed_64;
				fragment_unnamed_64 += unity_WorldToLight__array[3];
				float2 fragment_unnamed_297 = fragment_unnamed_64.xy / fragment_unnamed_64.ww;
				fragment_unnamed_143 = float3(fragment_unnamed_297.x, fragment_unnamed_297.y, fragment_unnamed_143.z);
				float2 fragment_unnamed_304 = fragment_unnamed_143.xy + 0.5f.xx;
				fragment_unnamed_143 = float3(fragment_unnamed_304.x, fragment_unnamed_304.y, fragment_unnamed_143.z);
				fragment_unnamed_137 = _LightTexture0.Sample(sampler_LightTexture0, fragment_unnamed_143.xy).w;
				fragment_unnamed_319 = 0.0f < fragment_unnamed_64.z;
				fragment_unnamed_64.x = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = _LightTextureB0.Sample(sampler_LightTextureB0, fragment_unnamed_64.xx).x;
				fragment_unnamed_240 = float(fragment_unnamed_319);
				fragment_unnamed_240 = fragment_unnamed_137 * fragment_unnamed_240;
				fragment_unnamed_64.x *= fragment_unnamed_240;
				float3 fragment_unnamed_355 = fragment_unnamed_64.xxx * _LightColor0.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_355.x, fragment_unnamed_355.y, fragment_unnamed_355.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_221.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_221 = abs(fragment_unnamed_49) + fragment_unnamed_9.x;
				fragment_unnamed_221 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_221 = 0.5f / fragment_unnamed_221;
				fragment_unnamed_9.x *= fragment_unnamed_221;
				fragment_unnamed_9.x *= 0.99999988079071044921875f;
				float3 fragment_unnamed_387 = fragment_unnamed_64.xyz * fragment_unnamed_9.xxx;
				fragment_unnamed_9 = float4(fragment_unnamed_387.x, fragment_unnamed_387.y, fragment_unnamed_9.z, fragment_unnamed_387.z);
				fragment_unnamed_64.x = fragment_unnamed_227 * fragment_unnamed_227;
				fragment_unnamed_64.x *= fragment_unnamed_64.x;
				fragment_unnamed_227 *= fragment_unnamed_64.x;
				fragment_unnamed_227 = (fragment_unnamed_227 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				float3 fragment_unnamed_413 = fragment_unnamed_227.xxx * fragment_unnamed_9.xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_413.x, fragment_unnamed_413.y, fragment_unnamed_413.z, fragment_unnamed_9.w);
				fragment_unnamed_64 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_64 *= _Color;
				float3 fragment_unnamed_434 = fragment_unnamed_64.xyz * fragment_input_6.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_434.x, fragment_unnamed_434.y, fragment_unnamed_434.z, fragment_unnamed_64.w);
				fragment_output_0.w = fragment_unnamed_64.w * fragment_input_6.w;
				float3 fragment_unnamed_450 = fragment_unnamed_64.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_450.x, fragment_unnamed_450.y, fragment_unnamed_450.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_459 = (fragment_unnamed_64.xyz * fragment_unnamed_143) + fragment_unnamed_9.xyz;
				fragment_unnamed_9 = float4(fragment_unnamed_459.x, fragment_unnamed_459.y, fragment_unnamed_459.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = fragment_input_1 / _ProjectionParams.y;
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_49 *= _ProjectionParams.z;
				fragment_unnamed_49 = max(fragment_unnamed_49, 0.0f);
				fragment_unnamed_49 = (fragment_unnamed_49 * unity_FogParams.z) + unity_FogParams.w;
				fragment_unnamed_49 = clamp(fragment_unnamed_49, 0.0f, 1.0f);
				float3 fragment_unnamed_490 = fragment_unnamed_9.xyz * fragment_unnamed_49.xxx;
				fragment_output_0 = float4(fragment_unnamed_490.x, fragment_unnamed_490.y, fragment_unnamed_490.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_1 = stage_input.fragment_input_1;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FOG_LINEAR
			#endif // SPOT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT
			#endif // !POINT_COOKIE


			#ifdef FOG_LINEAR
			#ifdef POINT_COOKIE
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[10];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float vertex_output_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;
			static float3 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float vertex_output_1 : TEXCOORD7; // TEXCOORD_7
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float3 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float3 vertex_output_5 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_6 : COLOR; // COLOR
				float4 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float3 vertex_output_8 : TEXCOORD6; // TEXCOORD_6
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_61 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_62 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_63 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_86 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_60));
				float vertex_unnamed_87 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_61));
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_62));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_63));
				precise float vertex_unnamed_97 = vertex_unnamed_86 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_98 = vertex_unnamed_87 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_99 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_100 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_108 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_109 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_110 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_111 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].w;
				float vertex_unnamed_143 = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_97, vertex_unnamed_110)));
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_97, vertex_unnamed_108)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_97, vertex_unnamed_109)));
				gl_Position.z = vertex_unnamed_143;
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_97, vertex_unnamed_111)));
				vertex_output_1 = vertex_unnamed_143;
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[9u].x, vertex_uniform_buffer_0[9u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[9u].y, vertex_uniform_buffer_0[9u].w);
				float vertex_unnamed_178 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_193 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_208 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_214 = rsqrt(dot(float3(vertex_unnamed_208, vertex_unnamed_178, vertex_unnamed_193), float3(vertex_unnamed_208, vertex_unnamed_178, vertex_unnamed_193)));
				precise float vertex_unnamed_215 = vertex_unnamed_214 * vertex_unnamed_208;
				precise float vertex_unnamed_216 = vertex_unnamed_214 * vertex_unnamed_178;
				precise float vertex_unnamed_217 = vertex_unnamed_214 * vertex_unnamed_193;
				precise float vertex_unnamed_225 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_226 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_227 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_245 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_225));
				float vertex_unnamed_246 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_226));
				float vertex_unnamed_247 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_227));
				float vertex_unnamed_251 = rsqrt(dot(float3(vertex_unnamed_245, vertex_unnamed_246, vertex_unnamed_247), float3(vertex_unnamed_245, vertex_unnamed_246, vertex_unnamed_247)));
				precise float vertex_unnamed_252 = vertex_unnamed_251 * vertex_unnamed_245;
				precise float vertex_unnamed_253 = vertex_unnamed_251 * vertex_unnamed_246;
				precise float vertex_unnamed_254 = vertex_unnamed_251 * vertex_unnamed_247;
				precise float vertex_unnamed_255 = vertex_unnamed_215 * vertex_unnamed_252;
				precise float vertex_unnamed_256 = vertex_unnamed_216 * vertex_unnamed_253;
				precise float vertex_unnamed_257 = vertex_unnamed_217 * vertex_unnamed_254;
				precise float vertex_unnamed_258 = (-0.0f) - vertex_unnamed_255;
				precise float vertex_unnamed_260 = (-0.0f) - vertex_unnamed_256;
				precise float vertex_unnamed_261 = (-0.0f) - vertex_unnamed_257;
				precise float vertex_unnamed_270 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_271 = vertex_unnamed_270 * mad(vertex_unnamed_217, vertex_unnamed_253, vertex_unnamed_258);
				precise float vertex_unnamed_272 = vertex_unnamed_270 * mad(vertex_unnamed_215, vertex_unnamed_254, vertex_unnamed_260);
				precise float vertex_unnamed_273 = vertex_unnamed_270 * mad(vertex_unnamed_216, vertex_unnamed_252, vertex_unnamed_261);
				vertex_output_2.y = vertex_unnamed_271;
				vertex_output_2.x = vertex_unnamed_254;
				vertex_output_2.z = vertex_unnamed_216;
				vertex_output_3.x = vertex_unnamed_252;
				vertex_output_4.x = vertex_unnamed_253;
				vertex_output_3.z = vertex_unnamed_217;
				vertex_output_4.z = vertex_unnamed_215;
				vertex_output_3.y = vertex_unnamed_272;
				vertex_output_4.y = vertex_unnamed_273;
				vertex_output_5.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_86);
				vertex_output_5.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_87);
				vertex_output_5.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_88);
				float vertex_unnamed_304 = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_86);
				float vertex_unnamed_305 = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_87);
				float vertex_unnamed_306 = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_88);
				float vertex_unnamed_307 = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_6.x = vertex_input_7.x;
				vertex_output_6.y = vertex_input_7.y;
				vertex_output_6.z = vertex_input_7.z;
				vertex_output_6.w = vertex_input_7.w;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
				precise float vertex_unnamed_330 = vertex_unnamed_305 * vertex_uniform_buffer_0[5u].x;
				precise float vertex_unnamed_331 = vertex_unnamed_305 * vertex_uniform_buffer_0[5u].y;
				precise float vertex_unnamed_332 = vertex_unnamed_305 * vertex_uniform_buffer_0[5u].z;
				vertex_output_8.x = mad(vertex_uniform_buffer_0[7u].x, vertex_unnamed_307, mad(vertex_uniform_buffer_0[6u].x, vertex_unnamed_306, mad(vertex_uniform_buffer_0[4u].x, vertex_unnamed_304, vertex_unnamed_330)));
				vertex_output_8.y = mad(vertex_uniform_buffer_0[7u].y, vertex_unnamed_307, mad(vertex_uniform_buffer_0[6u].y, vertex_unnamed_306, mad(vertex_uniform_buffer_0[4u].y, vertex_unnamed_304, vertex_unnamed_331)));
				vertex_output_8.z = mad(vertex_uniform_buffer_0[7u].z, vertex_unnamed_307, mad(vertex_uniform_buffer_0[6u].z, vertex_unnamed_306, mad(vertex_uniform_buffer_0[4u].z, vertex_unnamed_304, vertex_unnamed_332)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_uniform_buffer_0[9] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			#endif // FOG_LINEAR
			#endif // POINT_COOKIE
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT
			#endif // !SPOT


			#ifdef FOG_LINEAR
			#ifdef POINT_COOKIE
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 unity_WorldToLight__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float vertex_output_1;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float3 vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_input_4;
			static float4 vertex_output_7;
			static float3 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float vertex_output_1 : TEXCOORD7; // vs_TEXCOORD7
				float3 vertex_output_2 : TEXCOORD1; // vs_TEXCOORD1
				float3 vertex_output_3 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_4 : TEXCOORD3; // vs_TEXCOORD3
				float3 vertex_output_5 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_6 : UNKNOWN6;
				float4 vertex_output_7 : TEXCOORD5; // vs_TEXCOORD5
				float3 vertex_output_8 : TEXCOORD6; // vs_TEXCOORD6
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_45;
			static float4 vertex_unnamed_51;
			static float vertex_unnamed_129;
			static float3 vertex_unnamed_190;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_45 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_unnamed_51 = vertex_unnamed_45.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_51 = (unity_MatrixVP__array[0] * vertex_unnamed_45.xxxx) + vertex_unnamed_51;
				vertex_unnamed_51 = (unity_MatrixVP__array[2] * vertex_unnamed_45.zzzz) + vertex_unnamed_51;
				vertex_unnamed_45 = (unity_MatrixVP__array[3] * vertex_unnamed_45.wwww) + vertex_unnamed_51;
				gl_Position = vertex_unnamed_45;
				vertex_output_1 = vertex_unnamed_45.z;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_45.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_45.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_45.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_129 = dot(vertex_unnamed_45.xyz, vertex_unnamed_45.xyz);
				vertex_unnamed_129 = rsqrt(vertex_unnamed_129);
				float3 vertex_unnamed_141 = vertex_unnamed_129.xxx * vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_141.x, vertex_unnamed_141.y, vertex_unnamed_141.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_150 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_51 = float4(vertex_unnamed_150.x, vertex_unnamed_150.y, vertex_unnamed_150.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_161 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_161.x, vertex_unnamed_161.y, vertex_unnamed_161.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_172 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_172.x, vertex_unnamed_172.y, vertex_unnamed_172.z, vertex_unnamed_51.w);
				vertex_unnamed_129 = dot(vertex_unnamed_51.xyz, vertex_unnamed_51.xyz);
				vertex_unnamed_129 = rsqrt(vertex_unnamed_129);
				float3 vertex_unnamed_186 = vertex_unnamed_129.xxx * vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_186.x, vertex_unnamed_186.y, vertex_unnamed_186.z, vertex_unnamed_51.w);
				vertex_unnamed_190 = vertex_unnamed_45.xyz * vertex_unnamed_51.xyz;
				vertex_unnamed_190 = (vertex_unnamed_45.zxy * vertex_unnamed_51.yzx) + (-vertex_unnamed_190);
				vertex_unnamed_129 = vertex_input_1.w * unity_WorldTransformParams.w;
				vertex_unnamed_190 = vertex_unnamed_129.xxx * vertex_unnamed_190;
				vertex_output_2.y = vertex_unnamed_190.x;
				vertex_output_2.x = vertex_unnamed_51.z;
				vertex_output_2.z = vertex_unnamed_45.y;
				vertex_output_3.x = vertex_unnamed_51.x;
				vertex_output_4.x = vertex_unnamed_51.y;
				vertex_output_3.z = vertex_unnamed_45.z;
				vertex_output_4.z = vertex_unnamed_45.x;
				vertex_output_3.y = vertex_unnamed_190.y;
				vertex_output_4.y = vertex_unnamed_190.z;
				vertex_output_5 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_9;
				vertex_output_6 = vertex_input_4;
				vertex_output_7 = 0.0f.xxxx;
				float3 vertex_unnamed_276 = vertex_unnamed_9.yyy * unity_WorldToLight__array[1].xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_276.x, vertex_unnamed_276.y, vertex_unnamed_276.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_287 = (unity_WorldToLight__array[0].xyz * vertex_unnamed_9.xxx) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_287.x, vertex_unnamed_287.y, vertex_unnamed_287.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_298 = (unity_WorldToLight__array[2].xyz * vertex_unnamed_9.zzz) + vertex_unnamed_45.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_298.x, vertex_unnamed_298.y, vertex_unnamed_298.z, vertex_unnamed_9.w);
				vertex_output_8 = (unity_WorldToLight__array[3].xyz * vertex_unnamed_9.www) + vertex_unnamed_9.xyz;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogParams;
			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;

			static float4 unity_WorldToLight__array[4];
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			TextureCube<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			Texture2D<float4> _LightTextureB0;
			SamplerState sampler_LightTextureB0;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_output_0;
			static float fragment_input_1;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float fragment_input_1 : TEXCOORD7; // vs_TEXCOORD7
				float3 fragment_input_2 : TEXCOORD1; // vs_TEXCOORD1
				float3 fragment_input_3 : TEXCOORD2; // vs_TEXCOORD2
				float3 fragment_input_4 : TEXCOORD3; // vs_TEXCOORD3
				float3 fragment_input_5 : TEXCOORD4; // vs_TEXCOORD4
				float4 fragment_input_6 : UNKNOWN6;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float4 fragment_unnamed_9;
			static float fragment_unnamed_49;
			static float4 fragment_unnamed_64;
			static float fragment_unnamed_137;
			static float3 fragment_unnamed_143;
			static float fragment_unnamed_166;
			static float fragment_unnamed_221;
			static float fragment_unnamed_227;
			static float fragment_unnamed_240;
			static float fragment_unnamed_245;

			void frag_main()
			{
				float3 fragment_unnamed_26 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_26.x, fragment_unnamed_26.y, fragment_unnamed_26.z, fragment_unnamed_9.w);
				fragment_unnamed_9.x = fragment_unnamed_9.z * fragment_unnamed_9.x;
				float2 fragment_unnamed_46 = (fragment_unnamed_9.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_9 = float4(fragment_unnamed_46.x, fragment_unnamed_46.y, fragment_unnamed_9.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xy, fragment_unnamed_9.xy);
				fragment_unnamed_49 = min(fragment_unnamed_49, 1.0f);
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_9.z = sqrt(fragment_unnamed_49);
				fragment_unnamed_64.x = dot(fragment_input_2, fragment_unnamed_9.xyz);
				fragment_unnamed_64.y = dot(fragment_input_3, fragment_unnamed_9.xyz);
				fragment_unnamed_64.z = dot(fragment_input_4, fragment_unnamed_9.xyz);
				fragment_unnamed_9.x = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_9.x = rsqrt(fragment_unnamed_9.x);
				float3 fragment_unnamed_99 = fragment_unnamed_9.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_9 = float4(fragment_unnamed_99.x, fragment_unnamed_99.y, fragment_unnamed_99.z, fragment_unnamed_9.w);
				float3 fragment_unnamed_115 = (-fragment_input_5) + _WorldSpaceCameraPos;
				fragment_unnamed_64 = float4(fragment_unnamed_115.x, fragment_unnamed_115.y, fragment_unnamed_115.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_49 = rsqrt(fragment_unnamed_49);
				float3 fragment_unnamed_129 = fragment_unnamed_49.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_129.x, fragment_unnamed_129.y, fragment_unnamed_129.z, fragment_unnamed_64.w);
				fragment_unnamed_49 = dot(fragment_unnamed_9.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_137 = (-abs(fragment_unnamed_49)) + 1.0f;
				fragment_unnamed_143.x = fragment_unnamed_137 * fragment_unnamed_137;
				fragment_unnamed_143.x *= fragment_unnamed_143.x;
				fragment_unnamed_137 *= fragment_unnamed_143.x;
				fragment_unnamed_143 = (-fragment_input_5) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_166 = dot(fragment_unnamed_143, fragment_unnamed_143);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_178 = (fragment_unnamed_143 * fragment_unnamed_166.xxx) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_178.x, fragment_unnamed_178.y, fragment_unnamed_178.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_166.xxx * fragment_unnamed_143;
				fragment_unnamed_166 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_166 = max(fragment_unnamed_166, 0.001000000047497451305389404296875f);
				fragment_unnamed_166 = rsqrt(fragment_unnamed_166);
				float3 fragment_unnamed_199 = fragment_unnamed_64.xyz * fragment_unnamed_166.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_199.x, fragment_unnamed_199.y, fragment_unnamed_199.z, fragment_unnamed_64.w);
				fragment_unnamed_64.x = dot(fragment_unnamed_143, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = clamp(fragment_unnamed_64.x, 0.0f, 1.0f);
				fragment_unnamed_9.x = dot(fragment_unnamed_9.xyz, fragment_unnamed_143);
				fragment_unnamed_9.x = clamp(fragment_unnamed_9.x, 0.0f, 1.0f);
				fragment_unnamed_221 = dot(fragment_unnamed_64.xx, fragment_unnamed_64.xx);
				fragment_unnamed_227 = (-fragment_unnamed_64.x) + 1.0f;
				fragment_unnamed_221 += (-0.5f);
				fragment_unnamed_64.x = (fragment_unnamed_221 * fragment_unnamed_137) + 1.0f;
				fragment_unnamed_240 = (-fragment_unnamed_9.x) + 1.0f;
				fragment_unnamed_245 = fragment_unnamed_240 * fragment_unnamed_240;
				fragment_unnamed_245 *= fragment_unnamed_245;
				fragment_unnamed_240 *= fragment_unnamed_245;
				fragment_unnamed_221 = (fragment_unnamed_221 * fragment_unnamed_240) + 1.0f;
				fragment_unnamed_221 = fragment_unnamed_64.x * fragment_unnamed_221;
				fragment_unnamed_221 = fragment_unnamed_9.x * fragment_unnamed_221;
				float3 fragment_unnamed_274 = fragment_input_5.yyy * unity_WorldToLight__array[1].xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_274.x, fragment_unnamed_274.y, fragment_unnamed_274.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_285 = (unity_WorldToLight__array[0].xyz * fragment_input_5.xxx) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_285.x, fragment_unnamed_285.y, fragment_unnamed_285.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_296 = (unity_WorldToLight__array[2].xyz * fragment_input_5.zzz) + fragment_unnamed_64.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_296.x, fragment_unnamed_296.y, fragment_unnamed_296.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_305 = fragment_unnamed_64.xyz + unity_WorldToLight__array[3].xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_305.x, fragment_unnamed_305.y, fragment_unnamed_305.z, fragment_unnamed_64.w);
				fragment_unnamed_137 = dot(fragment_unnamed_64.xyz, fragment_unnamed_64.xyz);
				fragment_unnamed_64.x = _LightTexture0.Sample(sampler_LightTexture0, fragment_unnamed_64.xyz).w;
				fragment_unnamed_240 = _LightTextureB0.Sample(sampler_LightTextureB0, fragment_unnamed_137.xx).x;
				fragment_unnamed_64.x *= fragment_unnamed_240;
				float3 fragment_unnamed_347 = fragment_unnamed_64.xxx * _LightColor0.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_347.x, fragment_unnamed_347.y, fragment_unnamed_347.z, fragment_unnamed_64.w);
				fragment_unnamed_143 = fragment_unnamed_221.xxx * fragment_unnamed_64.xyz;
				fragment_unnamed_221 = abs(fragment_unnamed_49) + fragment_unnamed_9.x;
				fragment_unnamed_221 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_221 = 0.5f / fragment_unnamed_221;
				fragment_unnamed_9.x *= fragment_unnamed_221;
				fragment_unnamed_9.x *= 0.99999988079071044921875f;
				float3 fragment_unnamed_380 = fragment_unnamed_64.xyz * fragment_unnamed_9.xxx;
				fragment_unnamed_9 = float4(fragment_unnamed_380.x, fragment_unnamed_380.y, fragment_unnamed_9.z, fragment_unnamed_380.z);
				fragment_unnamed_64.x = fragment_unnamed_227 * fragment_unnamed_227;
				fragment_unnamed_64.x *= fragment_unnamed_64.x;
				fragment_unnamed_227 *= fragment_unnamed_64.x;
				fragment_unnamed_227 = (fragment_unnamed_227 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				float3 fragment_unnamed_406 = fragment_unnamed_227.xxx * fragment_unnamed_9.xyw;
				fragment_unnamed_9 = float4(fragment_unnamed_406.x, fragment_unnamed_406.y, fragment_unnamed_406.z, fragment_unnamed_9.w);
				fragment_unnamed_64 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_64 *= _Color;
				float3 fragment_unnamed_427 = fragment_unnamed_64.xyz * fragment_input_6.xyz;
				fragment_unnamed_64 = float4(fragment_unnamed_427.x, fragment_unnamed_427.y, fragment_unnamed_427.z, fragment_unnamed_64.w);
				fragment_output_0.w = fragment_unnamed_64.w * fragment_input_6.w;
				float3 fragment_unnamed_443 = fragment_unnamed_64.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_64 = float4(fragment_unnamed_443.x, fragment_unnamed_443.y, fragment_unnamed_443.z, fragment_unnamed_64.w);
				float3 fragment_unnamed_452 = (fragment_unnamed_64.xyz * fragment_unnamed_143) + fragment_unnamed_9.xyz;
				fragment_unnamed_9 = float4(fragment_unnamed_452.x, fragment_unnamed_452.y, fragment_unnamed_452.z, fragment_unnamed_9.w);
				fragment_unnamed_49 = fragment_input_1 / _ProjectionParams.y;
				fragment_unnamed_49 = (-fragment_unnamed_49) + 1.0f;
				fragment_unnamed_49 *= _ProjectionParams.z;
				fragment_unnamed_49 = max(fragment_unnamed_49, 0.0f);
				fragment_unnamed_49 = (fragment_unnamed_49 * unity_FogParams.z) + unity_FogParams.w;
				fragment_unnamed_49 = clamp(fragment_unnamed_49, 0.0f, 1.0f);
				float3 fragment_unnamed_483 = fragment_unnamed_9.xyz * fragment_unnamed_49.xxx;
				fragment_output_0 = float4(fragment_unnamed_483.x, fragment_unnamed_483.y, fragment_unnamed_483.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_1 = stage_input.fragment_input_1;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FOG_LINEAR
			#endif // POINT_COOKIE
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT
			#endif // !SPOT


			#ifdef DIRECTIONAL_COOKIE
			#ifdef FOG_LINEAR
			#ifndef DIRECTIONAL
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[10];
			static float4 vertex_uniform_buffer_1[10];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float2 vertex_output_1;
			static float2 vertex_output_1;
			static float3 vertex_output_2;
			static float vertex_output_2;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float3 vertex_output_5;
			static float4 vertex_output_6;
			static float4 vertex_output_7;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : TANGENT; // TANGENT
				float3 vertex_input_2 : NORMAL; // NORMAL
				float4 vertex_input_3 : TEXCOORD; // TEXCOORD
				float4 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
				float4 vertex_input_5 : TEXCOORD2; // TEXCOORD_2
				float4 vertex_input_6 : TEXCOORD3; // TEXCOORD_3
				float4 vertex_input_7 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float2 vertex_output_1 : TEXCOORD6; // TEXCOORD_6
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float vertex_output_2 : TEXCOORD7; // TEXCOORD_7
				float3 vertex_output_3 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_4 : TEXCOORD3; // TEXCOORD_3
				float3 vertex_output_5 : TEXCOORD4; // TEXCOORD_4
				float4 vertex_output_6 : COLOR; // COLOR
				float4 vertex_output_7 : TEXCOORD5; // TEXCOORD_5
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_60 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_61 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_62 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_63 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_86 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_60));
				float vertex_unnamed_87 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_61));
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_62));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_63));
				precise float vertex_unnamed_97 = vertex_unnamed_86 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_98 = vertex_unnamed_87 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_99 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_100 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_108 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_109 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_110 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_111 = vertex_unnamed_98 * vertex_uniform_buffer_2[18u].w;
				float vertex_unnamed_143 = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_97, vertex_unnamed_110)));
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_97, vertex_unnamed_108)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_97, vertex_unnamed_109)));
				gl_Position.z = vertex_unnamed_143;
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_99, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_97, vertex_unnamed_111)));
				vertex_output_2 = vertex_unnamed_143;
				float vertex_unnamed_157 = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_86);
				float vertex_unnamed_158 = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_87);
				float vertex_unnamed_159 = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_88);
				float vertex_unnamed_160 = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_5.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_86);
				vertex_output_5.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_87);
				vertex_output_5.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_88);
				precise float vertex_unnamed_179 = vertex_unnamed_158 * vertex_uniform_buffer_0[5u].x;
				precise float vertex_unnamed_180 = vertex_unnamed_158 * vertex_uniform_buffer_0[5u].y;
				vertex_output_1.x = mad(vertex_uniform_buffer_0[7u].x, vertex_unnamed_160, mad(vertex_uniform_buffer_0[6u].x, vertex_unnamed_159, mad(vertex_uniform_buffer_0[4u].x, vertex_unnamed_157, vertex_unnamed_179)));
				vertex_output_1.y = mad(vertex_uniform_buffer_0[7u].y, vertex_unnamed_160, mad(vertex_uniform_buffer_0[6u].y, vertex_unnamed_159, mad(vertex_uniform_buffer_0[4u].y, vertex_unnamed_157, vertex_unnamed_180)));
				vertex_output_1.x = mad(vertex_input_3.x, vertex_uniform_buffer_0[9u].x, vertex_uniform_buffer_0[9u].z);
				vertex_output_1.y = mad(vertex_input_3.y, vertex_uniform_buffer_0[9u].y, vertex_uniform_buffer_0[9u].w);
				float vertex_unnamed_232 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[4u].xyz));
				float vertex_unnamed_246 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[5u].xyz));
				float vertex_unnamed_260 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_1[6u].xyz));
				float vertex_unnamed_266 = rsqrt(dot(float3(vertex_unnamed_260, vertex_unnamed_232, vertex_unnamed_246), float3(vertex_unnamed_260, vertex_unnamed_232, vertex_unnamed_246)));
				precise float vertex_unnamed_267 = vertex_unnamed_266 * vertex_unnamed_260;
				precise float vertex_unnamed_268 = vertex_unnamed_266 * vertex_unnamed_232;
				precise float vertex_unnamed_269 = vertex_unnamed_266 * vertex_unnamed_246;
				precise float vertex_unnamed_277 = vertex_input_1.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_278 = vertex_input_1.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_279 = vertex_input_1.y * vertex_uniform_buffer_1[1u].x;
				float vertex_unnamed_297 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_1.x, vertex_unnamed_277));
				float vertex_unnamed_298 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_1.x, vertex_unnamed_278));
				float vertex_unnamed_299 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_1.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_1.x, vertex_unnamed_279));
				float vertex_unnamed_303 = rsqrt(dot(float3(vertex_unnamed_297, vertex_unnamed_298, vertex_unnamed_299), float3(vertex_unnamed_297, vertex_unnamed_298, vertex_unnamed_299)));
				precise float vertex_unnamed_304 = vertex_unnamed_303 * vertex_unnamed_297;
				precise float vertex_unnamed_305 = vertex_unnamed_303 * vertex_unnamed_298;
				precise float vertex_unnamed_306 = vertex_unnamed_303 * vertex_unnamed_299;
				precise float vertex_unnamed_307 = vertex_unnamed_267 * vertex_unnamed_304;
				precise float vertex_unnamed_308 = vertex_unnamed_268 * vertex_unnamed_305;
				precise float vertex_unnamed_309 = vertex_unnamed_269 * vertex_unnamed_306;
				precise float vertex_unnamed_310 = (-0.0f) - vertex_unnamed_307;
				precise float vertex_unnamed_312 = (-0.0f) - vertex_unnamed_308;
				precise float vertex_unnamed_313 = (-0.0f) - vertex_unnamed_309;
				precise float vertex_unnamed_322 = vertex_input_1.w * vertex_uniform_buffer_1[9u].w;
				precise float vertex_unnamed_323 = vertex_unnamed_322 * mad(vertex_unnamed_269, vertex_unnamed_305, vertex_unnamed_310);
				precise float vertex_unnamed_324 = vertex_unnamed_322 * mad(vertex_unnamed_267, vertex_unnamed_306, vertex_unnamed_312);
				precise float vertex_unnamed_325 = vertex_unnamed_322 * mad(vertex_unnamed_268, vertex_unnamed_304, vertex_unnamed_313);
				vertex_output_2.y = vertex_unnamed_323;
				vertex_output_2.x = vertex_unnamed_306;
				vertex_output_2.z = vertex_unnamed_268;
				vertex_output_3.x = vertex_unnamed_304;
				vertex_output_4.x = vertex_unnamed_305;
				vertex_output_3.z = vertex_unnamed_269;
				vertex_output_4.z = vertex_unnamed_267;
				vertex_output_3.y = vertex_unnamed_324;
				vertex_output_4.y = vertex_unnamed_325;
				vertex_output_6.x = vertex_input_7.x;
				vertex_output_6.y = vertex_input_7.y;
				vertex_output_6.z = vertex_input_7.z;
				vertex_output_6.w = vertex_input_7.w;
				vertex_output_7.x = 0.0f;
				vertex_output_7.y = 0.0f;
				vertex_output_7.z = 0.0f;
				vertex_output_7.w = 0.0f;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_uniform_buffer_0[9] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_1[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_1[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_1[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[9] = float4(unity_WorldTransformParams[0], unity_WorldTransformParams[1], unity_WorldTransformParams[2], unity_WorldTransformParams[3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vertex_input_5 = stage_input.vertex_input_5;
				vertex_input_6 = stage_input.vertex_input_6;
				vertex_input_7 = stage_input.vertex_input_7;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_7 = vertex_output_7;
				return stage_output;
			}

			#endif // DIRECTIONAL_COOKIE
			#endif // FOG_LINEAR
			#endif // !DIRECTIONAL
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef DIRECTIONAL_COOKIE
			#ifdef FOG_LINEAR
			#ifndef DIRECTIONAL
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4 unity_WorldTransformParams;
			float4x4 unity_MatrixVP;
			float4x4 unity_WorldToLight;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 unity_WorldToLight__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float vertex_output_2;
			static float3 vertex_output_5;
			static float2 vertex_output_7;
			static float2 vertex_output_0;
			static float4 vertex_input_3;
			static float3 vertex_input_2;
			static float4 vertex_input_1;
			static float3 vertex_output_1;
			static float3 vertex_output_3;
			static float3 vertex_output_4;
			static float4 vertex_output_6;
			static float4 vertex_input_4;
			static float4 vertex_output_8;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float4 vertex_input_1 : TANGENT;
				float3 vertex_input_2 : NORMAL;
				float4 vertex_input_3 : TEXCOORD0;
				float4 vertex_input_4 : COLOR;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 vertex_output_1 : TEXCOORD1; // vs_TEXCOORD1
				float vertex_output_2 : TEXCOORD7; // vs_TEXCOORD7
				float3 vertex_output_3 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_4 : TEXCOORD3; // vs_TEXCOORD3
				float3 vertex_output_5 : TEXCOORD4; // vs_TEXCOORD4
				float4 vertex_output_6 : UNKNOWN6;
				float2 vertex_output_7 : TEXCOORD6; // vs_TEXCOORD6
				float4 vertex_output_8 : TEXCOORD5; // vs_TEXCOORD5
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_45;
			static float4 vertex_unnamed_51;
			static float vertex_unnamed_188;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_45 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_unnamed_51 = vertex_unnamed_45.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_51 = (unity_MatrixVP__array[0] * vertex_unnamed_45.xxxx) + vertex_unnamed_51;
				vertex_unnamed_51 = (unity_MatrixVP__array[2] * vertex_unnamed_45.zzzz) + vertex_unnamed_51;
				vertex_unnamed_45 = (unity_MatrixVP__array[3] * vertex_unnamed_45.wwww) + vertex_unnamed_51;
				gl_Position = vertex_unnamed_45;
				vertex_output_2 = vertex_unnamed_45.z;
				vertex_unnamed_45 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_9;
				vertex_output_5 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				float2 vertex_unnamed_118 = vertex_unnamed_45.yy * unity_WorldToLight__array[1].xy;
				vertex_unnamed_9 = float4(vertex_unnamed_118.x, vertex_unnamed_118.y, vertex_unnamed_9.z, vertex_unnamed_9.w);
				float2 vertex_unnamed_129 = (unity_WorldToLight__array[0].xy * vertex_unnamed_45.xx) + vertex_unnamed_9.xy;
				vertex_unnamed_9 = float4(vertex_unnamed_129.x, vertex_unnamed_129.y, vertex_unnamed_9.z, vertex_unnamed_9.w);
				float2 vertex_unnamed_140 = (unity_WorldToLight__array[2].xy * vertex_unnamed_45.zz) + vertex_unnamed_9.xy;
				vertex_unnamed_9 = float4(vertex_unnamed_140.x, vertex_unnamed_140.y, vertex_unnamed_9.z, vertex_unnamed_9.w);
				vertex_output_7 = (unity_WorldToLight__array[3].xy * vertex_unnamed_45.ww) + vertex_unnamed_9.xy;
				vertex_output_0 = (vertex_input_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_9.y = dot(vertex_input_2, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_9.z = dot(vertex_input_2, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_9.x = dot(vertex_input_2, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_188 = dot(vertex_unnamed_9.xyz, vertex_unnamed_9.xyz);
				vertex_unnamed_188 = rsqrt(vertex_unnamed_188);
				float3 vertex_unnamed_200 = vertex_unnamed_188.xxx * vertex_unnamed_9.xyz;
				vertex_unnamed_9 = float4(vertex_unnamed_200.x, vertex_unnamed_200.y, vertex_unnamed_200.z, vertex_unnamed_9.w);
				float3 vertex_unnamed_209 = vertex_input_1.yyy * unity_ObjectToWorld__array[1].yzx;
				vertex_unnamed_45 = float4(vertex_unnamed_209.x, vertex_unnamed_209.y, vertex_unnamed_209.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_220 = (unity_ObjectToWorld__array[0].yzx * vertex_input_1.xxx) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_220.x, vertex_unnamed_220.y, vertex_unnamed_220.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_231 = (unity_ObjectToWorld__array[2].yzx * vertex_input_1.zzz) + vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_231.x, vertex_unnamed_231.y, vertex_unnamed_231.z, vertex_unnamed_45.w);
				vertex_unnamed_188 = dot(vertex_unnamed_45.xyz, vertex_unnamed_45.xyz);
				vertex_unnamed_188 = rsqrt(vertex_unnamed_188);
				float3 vertex_unnamed_245 = vertex_unnamed_188.xxx * vertex_unnamed_45.xyz;
				vertex_unnamed_45 = float4(vertex_unnamed_245.x, vertex_unnamed_245.y, vertex_unnamed_245.z, vertex_unnamed_45.w);
				float3 vertex_unnamed_252 = vertex_unnamed_9.xyz * vertex_unnamed_45.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_252.x, vertex_unnamed_252.y, vertex_unnamed_252.z, vertex_unnamed_51.w);
				float3 vertex_unnamed_263 = (vertex_unnamed_9.zxy * vertex_unnamed_45.yzx) + (-vertex_unnamed_51.xyz);
				vertex_unnamed_51 = float4(vertex_unnamed_263.x, vertex_unnamed_263.y, vertex_unnamed_263.z, vertex_unnamed_51.w);
				vertex_unnamed_188 = vertex_input_1.w * unity_WorldTransformParams.w;
				float3 vertex_unnamed_278 = vertex_unnamed_188.xxx * vertex_unnamed_51.xyz;
				vertex_unnamed_51 = float4(vertex_unnamed_278.x, vertex_unnamed_278.y, vertex_unnamed_278.z, vertex_unnamed_51.w);
				vertex_output_1.y = vertex_unnamed_51.x;
				vertex_output_1.x = vertex_unnamed_45.z;
				vertex_output_1.z = vertex_unnamed_9.y;
				vertex_output_3.x = vertex_unnamed_45.x;
				vertex_output_4.x = vertex_unnamed_45.y;
				vertex_output_3.z = vertex_unnamed_9.z;
				vertex_output_4.z = vertex_unnamed_9.x;
				vertex_output_3.y = vertex_unnamed_51.y;
				vertex_output_4.y = vertex_unnamed_51.z;
				vertex_output_6 = vertex_input_4;
				vertex_output_8 = 0.0f.xxxx;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_WorldToObject__array[0] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				unity_WorldToObject__array[1] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				unity_WorldToObject__array[2] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				unity_WorldToObject__array[3] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_5 = vertex_output_5;
				stage_output.vertex_output_7 = vertex_output_7;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_3 = vertex_output_3;
				stage_output.vertex_output_4 = vertex_output_4;
				stage_output.vertex_output_6 = vertex_output_6;
				stage_output.vertex_output_8 = vertex_output_8;
				return stage_output;
			}

			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogParams;
			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;

			static float4 unity_WorldToLight__array[4];
			Texture2D<float4> _Normal;
			SamplerState sampler_Normal;
			Texture2D<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float3 fragment_input_1;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_output_0;
			static float fragment_input_2;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 fragment_input_1 : TEXCOORD1; // vs_TEXCOORD1
				float fragment_input_2 : TEXCOORD7; // vs_TEXCOORD7
				float3 fragment_input_3 : TEXCOORD2; // vs_TEXCOORD2
				float3 fragment_input_4 : TEXCOORD3; // vs_TEXCOORD3
				float3 fragment_input_5 : TEXCOORD4; // vs_TEXCOORD4
				float4 fragment_input_6 : UNKNOWN6;
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float fragment_unnamed_47;
			static float4 fragment_unnamed_63;
			static float3 fragment_unnamed_119;
			static float fragment_unnamed_152;
			static float fragment_unnamed_157;
			static float fragment_unnamed_229;
			static float fragment_unnamed_234;

			void frag_main()
			{
				fragment_unnamed_9 = _Normal.Sample(sampler_Normal, fragment_input_0).xyw;
				fragment_unnamed_9.x = fragment_unnamed_9.z * fragment_unnamed_9.x;
				float2 fragment_unnamed_44 = (fragment_unnamed_9.xy * 2.0f.xx) + (-1.0f).xx;
				fragment_unnamed_9 = float3(fragment_unnamed_44.x, fragment_unnamed_44.y, fragment_unnamed_9.z);
				fragment_unnamed_47 = dot(fragment_unnamed_9.xy, fragment_unnamed_9.xy);
				fragment_unnamed_47 = min(fragment_unnamed_47, 1.0f);
				fragment_unnamed_47 = (-fragment_unnamed_47) + 1.0f;
				fragment_unnamed_9.z = sqrt(fragment_unnamed_47);
				fragment_unnamed_63.x = dot(fragment_input_1, fragment_unnamed_9);
				fragment_unnamed_63.y = dot(fragment_input_3, fragment_unnamed_9);
				fragment_unnamed_63.z = dot(fragment_input_4, fragment_unnamed_9);
				fragment_unnamed_9.x = dot(fragment_unnamed_63.xyz, fragment_unnamed_63.xyz);
				fragment_unnamed_9.x = rsqrt(fragment_unnamed_9.x);
				fragment_unnamed_9 = fragment_unnamed_9.xxx * fragment_unnamed_63.xyz;
				float3 fragment_unnamed_109 = (-fragment_input_5) + _WorldSpaceCameraPos;
				fragment_unnamed_63 = float4(fragment_unnamed_109.x, fragment_unnamed_109.y, fragment_unnamed_109.z, fragment_unnamed_63.w);
				fragment_unnamed_47 = dot(fragment_unnamed_63.xyz, fragment_unnamed_63.xyz);
				fragment_unnamed_47 = rsqrt(fragment_unnamed_47);
				fragment_unnamed_119 = fragment_unnamed_47.xxx * fragment_unnamed_63.xyz;
				float3 fragment_unnamed_135 = (fragment_unnamed_63.xyz * fragment_unnamed_47.xxx) + _WorldSpaceLightPos0.xyz;
				fragment_unnamed_63 = float4(fragment_unnamed_135.x, fragment_unnamed_135.y, fragment_unnamed_135.z, fragment_unnamed_63.w);
				fragment_unnamed_47 = dot(fragment_unnamed_9, fragment_unnamed_119);
				fragment_unnamed_9.x = dot(fragment_unnamed_9, _WorldSpaceLightPos0.xyz);
				fragment_unnamed_9.x = clamp(fragment_unnamed_9.x, 0.0f, 1.0f);
				fragment_unnamed_152 = (-abs(fragment_unnamed_47)) + 1.0f;
				fragment_unnamed_157 = abs(fragment_unnamed_47) + fragment_unnamed_9.x;
				fragment_unnamed_157 += 9.9999997473787516355514526367188e-06f;
				fragment_unnamed_157 = 0.5f / fragment_unnamed_157;
				fragment_unnamed_157 = fragment_unnamed_9.x * fragment_unnamed_157;
				fragment_unnamed_157 *= 0.99999988079071044921875f;
				fragment_unnamed_47 = fragment_unnamed_152 * fragment_unnamed_152;
				fragment_unnamed_47 *= fragment_unnamed_47;
				fragment_unnamed_152 *= fragment_unnamed_47;
				fragment_unnamed_47 = dot(fragment_unnamed_63.xyz, fragment_unnamed_63.xyz);
				fragment_unnamed_47 = max(fragment_unnamed_47, 0.001000000047497451305389404296875f);
				fragment_unnamed_47 = rsqrt(fragment_unnamed_47);
				float3 fragment_unnamed_199 = fragment_unnamed_47.xxx * fragment_unnamed_63.xyz;
				fragment_unnamed_63 = float4(fragment_unnamed_199.x, fragment_unnamed_199.y, fragment_unnamed_199.z, fragment_unnamed_63.w);
				fragment_unnamed_47 = dot(_WorldSpaceLightPos0.xyz, fragment_unnamed_63.xyz);
				fragment_unnamed_47 = clamp(fragment_unnamed_47, 0.0f, 1.0f);
				fragment_unnamed_63.x = dot(fragment_unnamed_47.xx, fragment_unnamed_47.xx);
				fragment_unnamed_47 = (-fragment_unnamed_47) + 1.0f;
				fragment_unnamed_63.x += (-0.5f);
				fragment_unnamed_152 = (fragment_unnamed_63.x * fragment_unnamed_152) + 1.0f;
				fragment_unnamed_229 = (-fragment_unnamed_9.x) + 1.0f;
				fragment_unnamed_234 = fragment_unnamed_229 * fragment_unnamed_229;
				fragment_unnamed_234 *= fragment_unnamed_234;
				fragment_unnamed_229 *= fragment_unnamed_234;
				fragment_unnamed_63.x = (fragment_unnamed_63.x * fragment_unnamed_229) + 1.0f;
				fragment_unnamed_152 *= fragment_unnamed_63.x;
				fragment_unnamed_9.x *= fragment_unnamed_152;
				float2 fragment_unnamed_266 = fragment_input_5.yy * unity_WorldToLight__array[1].xy;
				fragment_unnamed_63 = float4(fragment_unnamed_266.x, fragment_unnamed_266.y, fragment_unnamed_63.z, fragment_unnamed_63.w);
				float2 fragment_unnamed_277 = (unity_WorldToLight__array[0].xy * fragment_input_5.xx) + fragment_unnamed_63.xy;
				fragment_unnamed_63 = float4(fragment_unnamed_277.x, fragment_unnamed_277.y, fragment_unnamed_63.z, fragment_unnamed_63.w);
				float2 fragment_unnamed_288 = (unity_WorldToLight__array[2].xy * fragment_input_5.zz) + fragment_unnamed_63.xy;
				fragment_unnamed_63 = float4(fragment_unnamed_288.x, fragment_unnamed_288.y, fragment_unnamed_63.z, fragment_unnamed_63.w);
				float2 fragment_unnamed_297 = fragment_unnamed_63.xy + unity_WorldToLight__array[3].xy;
				fragment_unnamed_63 = float4(fragment_unnamed_297.x, fragment_unnamed_297.y, fragment_unnamed_63.z, fragment_unnamed_63.w);
				fragment_unnamed_152 = _LightTexture0.Sample(sampler_LightTexture0, fragment_unnamed_63.xy).w;
				float3 fragment_unnamed_316 = fragment_unnamed_152.xxx * _LightColor0.xyz;
				fragment_unnamed_63 = float4(fragment_unnamed_316.x, fragment_unnamed_316.y, fragment_unnamed_316.z, fragment_unnamed_63.w);
				fragment_unnamed_119 = fragment_unnamed_9.xxx * fragment_unnamed_63.xyz;
				fragment_unnamed_9 = fragment_unnamed_157.xxx * fragment_unnamed_63.xyz;
				fragment_unnamed_63.x = fragment_unnamed_47 * fragment_unnamed_47;
				fragment_unnamed_63.x *= fragment_unnamed_63.x;
				fragment_unnamed_47 *= fragment_unnamed_63.x;
				fragment_unnamed_47 = (fragment_unnamed_47 * 0.959999978542327880859375f) + 0.039999999105930328369140625f;
				fragment_unnamed_9 = fragment_unnamed_47.xxx * fragment_unnamed_9;
				fragment_unnamed_63 = _MainTex.Sample(sampler_MainTex, fragment_input_0);
				fragment_unnamed_63 *= _Color;
				float3 fragment_unnamed_370 = fragment_unnamed_63.xyz * fragment_input_6.xyz;
				fragment_unnamed_63 = float4(fragment_unnamed_370.x, fragment_unnamed_370.y, fragment_unnamed_370.z, fragment_unnamed_63.w);
				fragment_output_0.w = fragment_unnamed_63.w * fragment_input_6.w;
				float3 fragment_unnamed_386 = fragment_unnamed_63.xyz * 0.959999978542327880859375f.xxx;
				fragment_unnamed_63 = float4(fragment_unnamed_386.x, fragment_unnamed_386.y, fragment_unnamed_386.z, fragment_unnamed_63.w);
				fragment_unnamed_9 = (fragment_unnamed_63.xyz * fragment_unnamed_119) + fragment_unnamed_9;
				fragment_unnamed_47 = fragment_input_2 / _ProjectionParams.y;
				fragment_unnamed_47 = (-fragment_unnamed_47) + 1.0f;
				fragment_unnamed_47 *= _ProjectionParams.z;
				fragment_unnamed_47 = max(fragment_unnamed_47, 0.0f);
				fragment_unnamed_47 = (fragment_unnamed_47 * unity_FogParams.z) + unity_FogParams.w;
				fragment_unnamed_47 = clamp(fragment_unnamed_47, 0.0f, 1.0f);
				float3 fragment_unnamed_422 = fragment_unnamed_9 * fragment_unnamed_47.xxx;
				fragment_output_0 = float4(fragment_unnamed_422.x, fragment_unnamed_422.y, fragment_unnamed_422.z, fragment_output_0.w);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				unity_WorldToLight__array[0] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				unity_WorldToLight__array[1] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				unity_WorldToLight__array[2] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				unity_WorldToLight__array[3] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL_COOKIE
			#endif // FOG_LINEAR
			#endif // !DIRECTIONAL
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef POINT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;

			static float4 fragment_uniform_buffer_0[9];
			static float4 fragment_uniform_buffer_1[5];
			static float4 fragment_uniform_buffer_2[1];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			Texture2D<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float3 fragment_input_8;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float3 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float3 fragment_input_5 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_6 : COLOR; // COLOR
				float4 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
				float3 fragment_input_8 : TEXCOORD6; // TEXCOORD_6
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float4 fragment_unnamed_64 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_69 = fragment_unnamed_64.w * fragment_unnamed_64.x;
				float fragment_unnamed_71 = mad(fragment_unnamed_69, 2.0f, -1.0f);
				float fragment_unnamed_74 = mad(fragment_unnamed_64.y, 2.0f, -1.0f);
				precise float fragment_unnamed_80 = (-0.0f) - min(dot(float2(fragment_unnamed_71, fragment_unnamed_74), float2(fragment_unnamed_71, fragment_unnamed_74)), 1.0f);
				precise float fragment_unnamed_82 = fragment_unnamed_80 + 1.0f;
				float fragment_unnamed_83 = sqrt(fragment_unnamed_82);
				float fragment_unnamed_91 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_71, fragment_unnamed_74, fragment_unnamed_83));
				float fragment_unnamed_100 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_71, fragment_unnamed_74, fragment_unnamed_83));
				float fragment_unnamed_109 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_71, fragment_unnamed_74, fragment_unnamed_83));
				float fragment_unnamed_115 = rsqrt(dot(float3(fragment_unnamed_91, fragment_unnamed_100, fragment_unnamed_109), float3(fragment_unnamed_91, fragment_unnamed_100, fragment_unnamed_109)));
				precise float fragment_unnamed_116 = fragment_unnamed_115 * fragment_unnamed_91;
				precise float fragment_unnamed_117 = fragment_unnamed_115 * fragment_unnamed_100;
				precise float fragment_unnamed_118 = fragment_unnamed_115 * fragment_unnamed_109;
				precise float fragment_unnamed_121 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_124 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_127 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_135 = fragment_unnamed_121 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_136 = fragment_unnamed_124 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_137 = fragment_unnamed_127 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_141 = rsqrt(dot(float3(fragment_unnamed_135, fragment_unnamed_136, fragment_unnamed_137), float3(fragment_unnamed_135, fragment_unnamed_136, fragment_unnamed_137)));
				precise float fragment_unnamed_142 = fragment_unnamed_141 * fragment_unnamed_135;
				precise float fragment_unnamed_143 = fragment_unnamed_141 * fragment_unnamed_136;
				precise float fragment_unnamed_144 = fragment_unnamed_141 * fragment_unnamed_137;
				float fragment_unnamed_145 = dot(float3(fragment_unnamed_116, fragment_unnamed_117, fragment_unnamed_118), float3(fragment_unnamed_142, fragment_unnamed_143, fragment_unnamed_144));
				precise float fragment_unnamed_149 = (-0.0f) - abs(fragment_unnamed_145);
				precise float fragment_unnamed_150 = fragment_unnamed_149 + 1.0f;
				precise float fragment_unnamed_151 = fragment_unnamed_150 * fragment_unnamed_150;
				precise float fragment_unnamed_152 = fragment_unnamed_151 * fragment_unnamed_151;
				precise float fragment_unnamed_153 = fragment_unnamed_150 * fragment_unnamed_152;
				precise float fragment_unnamed_156 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_159 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_162 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_168 = fragment_unnamed_156 + fragment_uniform_buffer_2[0u].x;
				precise float fragment_unnamed_169 = fragment_unnamed_159 + fragment_uniform_buffer_2[0u].y;
				precise float fragment_unnamed_170 = fragment_unnamed_162 + fragment_uniform_buffer_2[0u].z;
				float fragment_unnamed_174 = rsqrt(dot(float3(fragment_unnamed_168, fragment_unnamed_169, fragment_unnamed_170), float3(fragment_unnamed_168, fragment_unnamed_169, fragment_unnamed_170)));
				float fragment_unnamed_175 = mad(fragment_unnamed_168, fragment_unnamed_174, fragment_unnamed_142);
				float fragment_unnamed_176 = mad(fragment_unnamed_169, fragment_unnamed_174, fragment_unnamed_143);
				float fragment_unnamed_177 = mad(fragment_unnamed_170, fragment_unnamed_174, fragment_unnamed_144);
				precise float fragment_unnamed_178 = fragment_unnamed_174 * fragment_unnamed_168;
				precise float fragment_unnamed_179 = fragment_unnamed_174 * fragment_unnamed_169;
				precise float fragment_unnamed_180 = fragment_unnamed_174 * fragment_unnamed_170;
				float fragment_unnamed_186 = rsqrt(max(dot(float3(fragment_unnamed_175, fragment_unnamed_176, fragment_unnamed_177), float3(fragment_unnamed_175, fragment_unnamed_176, fragment_unnamed_177)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_187 = fragment_unnamed_175 * fragment_unnamed_186;
				precise float fragment_unnamed_188 = fragment_unnamed_176 * fragment_unnamed_186;
				precise float fragment_unnamed_189 = fragment_unnamed_177 * fragment_unnamed_186;
				float fragment_unnamed_193 = clamp(dot(float3(fragment_unnamed_178, fragment_unnamed_179, fragment_unnamed_180), float3(fragment_unnamed_187, fragment_unnamed_188, fragment_unnamed_189)), 0.0f, 1.0f);
				float fragment_unnamed_197 = clamp(dot(float3(fragment_unnamed_116, fragment_unnamed_117, fragment_unnamed_118), float3(fragment_unnamed_178, fragment_unnamed_179, fragment_unnamed_180)), 0.0f, 1.0f);
				precise float fragment_unnamed_201 = (-0.0f) - fragment_unnamed_193;
				precise float fragment_unnamed_202 = fragment_unnamed_201 + 1.0f;
				precise float fragment_unnamed_203 = dot(fragment_unnamed_193.xx, fragment_unnamed_193.xx) + (-0.5f);
				precise float fragment_unnamed_206 = (-0.0f) - fragment_unnamed_197;
				precise float fragment_unnamed_207 = fragment_unnamed_206 + 1.0f;
				precise float fragment_unnamed_208 = fragment_unnamed_207 * fragment_unnamed_207;
				precise float fragment_unnamed_209 = fragment_unnamed_208 * fragment_unnamed_208;
				precise float fragment_unnamed_210 = fragment_unnamed_207 * fragment_unnamed_209;
				precise float fragment_unnamed_212 = mad(fragment_unnamed_203, fragment_unnamed_153, 1.0f) * mad(fragment_unnamed_203, fragment_unnamed_210, 1.0f);
				precise float fragment_unnamed_213 = fragment_unnamed_197 * fragment_unnamed_212;
				precise float fragment_unnamed_221 = fragment_input_5.y * fragment_uniform_buffer_0[5u].x;
				precise float fragment_unnamed_222 = fragment_input_5.y * fragment_uniform_buffer_0[5u].y;
				precise float fragment_unnamed_223 = fragment_input_5.y * fragment_uniform_buffer_0[5u].z;
				precise float fragment_unnamed_251 = mad(fragment_uniform_buffer_0[6u].x, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].x, fragment_input_5.x, fragment_unnamed_221)) + fragment_uniform_buffer_0[7u].x;
				precise float fragment_unnamed_252 = mad(fragment_uniform_buffer_0[6u].y, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].y, fragment_input_5.x, fragment_unnamed_222)) + fragment_uniform_buffer_0[7u].y;
				precise float fragment_unnamed_253 = mad(fragment_uniform_buffer_0[6u].z, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].z, fragment_input_5.x, fragment_unnamed_223)) + fragment_uniform_buffer_0[7u].z;
				float4 fragment_unnamed_258 = _LightTexture0.Sample(sampler_LightTexture0, dot(float3(fragment_unnamed_251, fragment_unnamed_252, fragment_unnamed_253), float3(fragment_unnamed_251, fragment_unnamed_252, fragment_unnamed_253)).xx);
				float fragment_unnamed_260 = fragment_unnamed_258.x;
				precise float fragment_unnamed_266 = fragment_unnamed_260 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_267 = fragment_unnamed_260 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_268 = fragment_unnamed_260 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_269 = fragment_unnamed_213 * fragment_unnamed_266;
				precise float fragment_unnamed_270 = fragment_unnamed_213 * fragment_unnamed_267;
				precise float fragment_unnamed_271 = fragment_unnamed_213 * fragment_unnamed_268;
				precise float fragment_unnamed_273 = abs(fragment_unnamed_145) + fragment_unnamed_197;
				precise float fragment_unnamed_274 = fragment_unnamed_273 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_276 = 0.5f / fragment_unnamed_274;
				precise float fragment_unnamed_278 = fragment_unnamed_276 * 0.99999988079071044921875f;
				precise float fragment_unnamed_280 = fragment_unnamed_197 * fragment_unnamed_278;
				precise float fragment_unnamed_281 = fragment_unnamed_266 * fragment_unnamed_280;
				precise float fragment_unnamed_282 = fragment_unnamed_267 * fragment_unnamed_280;
				precise float fragment_unnamed_283 = fragment_unnamed_268 * fragment_unnamed_280;
				precise float fragment_unnamed_284 = fragment_unnamed_202 * fragment_unnamed_202;
				precise float fragment_unnamed_285 = fragment_unnamed_284 * fragment_unnamed_284;
				precise float fragment_unnamed_286 = fragment_unnamed_202 * fragment_unnamed_285;
				float fragment_unnamed_287 = mad(fragment_unnamed_286, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_290 = fragment_unnamed_287 * fragment_unnamed_281;
				precise float fragment_unnamed_291 = fragment_unnamed_287 * fragment_unnamed_282;
				precise float fragment_unnamed_292 = fragment_unnamed_287 * fragment_unnamed_283;
				float4 fragment_unnamed_298 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_311 = fragment_unnamed_298.x * fragment_uniform_buffer_0[8u].x;
				precise float fragment_unnamed_312 = fragment_unnamed_298.y * fragment_uniform_buffer_0[8u].y;
				precise float fragment_unnamed_313 = fragment_unnamed_298.z * fragment_uniform_buffer_0[8u].z;
				precise float fragment_unnamed_314 = fragment_unnamed_298.w * fragment_uniform_buffer_0[8u].w;
				precise float fragment_unnamed_321 = fragment_unnamed_311 * fragment_input_6.x;
				precise float fragment_unnamed_322 = fragment_unnamed_312 * fragment_input_6.y;
				precise float fragment_unnamed_323 = fragment_unnamed_313 * fragment_input_6.z;
				precise float fragment_unnamed_327 = fragment_unnamed_314 * fragment_input_6.w;
				fragment_output_0.w = fragment_unnamed_327;
				precise float fragment_unnamed_330 = fragment_unnamed_321 * 0.959999978542327880859375f;
				precise float fragment_unnamed_331 = fragment_unnamed_322 * 0.959999978542327880859375f;
				precise float fragment_unnamed_332 = fragment_unnamed_323 * 0.959999978542327880859375f;
				fragment_output_0.x = mad(fragment_unnamed_330, fragment_unnamed_269, fragment_unnamed_290);
				fragment_output_0.y = mad(fragment_unnamed_331, fragment_unnamed_270, fragment_unnamed_291);
				fragment_output_0.z = mad(fragment_unnamed_332, fragment_unnamed_271, fragment_unnamed_292);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				fragment_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				fragment_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				fragment_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_uniform_buffer_0[8] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				fragment_input_8 = stage_input.fragment_input_8;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // POINT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;

			static float4 fragment_uniform_buffer_0[5];
			static float4 fragment_uniform_buffer_1[5];
			static float4 fragment_uniform_buffer_2[1];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float3 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float3 fragment_input_5 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_6 : COLOR; // COLOR
				float4 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float4 fragment_unnamed_58 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_63 = fragment_unnamed_58.w * fragment_unnamed_58.x;
				float fragment_unnamed_65 = mad(fragment_unnamed_63, 2.0f, -1.0f);
				float fragment_unnamed_68 = mad(fragment_unnamed_58.y, 2.0f, -1.0f);
				precise float fragment_unnamed_74 = (-0.0f) - min(dot(float2(fragment_unnamed_65, fragment_unnamed_68), float2(fragment_unnamed_65, fragment_unnamed_68)), 1.0f);
				precise float fragment_unnamed_76 = fragment_unnamed_74 + 1.0f;
				float fragment_unnamed_77 = sqrt(fragment_unnamed_76);
				float fragment_unnamed_85 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_65, fragment_unnamed_68, fragment_unnamed_77));
				float fragment_unnamed_94 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_65, fragment_unnamed_68, fragment_unnamed_77));
				float fragment_unnamed_103 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_65, fragment_unnamed_68, fragment_unnamed_77));
				float fragment_unnamed_109 = rsqrt(dot(float3(fragment_unnamed_85, fragment_unnamed_94, fragment_unnamed_103), float3(fragment_unnamed_85, fragment_unnamed_94, fragment_unnamed_103)));
				precise float fragment_unnamed_110 = fragment_unnamed_109 * fragment_unnamed_85;
				precise float fragment_unnamed_111 = fragment_unnamed_109 * fragment_unnamed_94;
				precise float fragment_unnamed_112 = fragment_unnamed_109 * fragment_unnamed_103;
				precise float fragment_unnamed_115 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_118 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_121 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_129 = fragment_unnamed_115 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_130 = fragment_unnamed_118 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_131 = fragment_unnamed_121 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_135 = rsqrt(dot(float3(fragment_unnamed_129, fragment_unnamed_130, fragment_unnamed_131), float3(fragment_unnamed_129, fragment_unnamed_130, fragment_unnamed_131)));
				precise float fragment_unnamed_136 = fragment_unnamed_135 * fragment_unnamed_129;
				precise float fragment_unnamed_137 = fragment_unnamed_135 * fragment_unnamed_130;
				precise float fragment_unnamed_138 = fragment_unnamed_135 * fragment_unnamed_131;
				float fragment_unnamed_144 = mad(fragment_unnamed_129, fragment_unnamed_135, fragment_uniform_buffer_2[0u].x);
				float fragment_unnamed_145 = mad(fragment_unnamed_130, fragment_unnamed_135, fragment_uniform_buffer_2[0u].y);
				float fragment_unnamed_146 = mad(fragment_unnamed_131, fragment_unnamed_135, fragment_uniform_buffer_2[0u].z);
				float fragment_unnamed_147 = dot(float3(fragment_unnamed_110, fragment_unnamed_111, fragment_unnamed_112), float3(fragment_unnamed_136, fragment_unnamed_137, fragment_unnamed_138));
				float fragment_unnamed_158 = clamp(dot(float3(fragment_unnamed_110, fragment_unnamed_111, fragment_unnamed_112), float3(fragment_uniform_buffer_2[0u].xyz)), 0.0f, 1.0f);
				precise float fragment_unnamed_160 = (-0.0f) - abs(fragment_unnamed_147);
				precise float fragment_unnamed_161 = fragment_unnamed_160 + 1.0f;
				precise float fragment_unnamed_163 = abs(fragment_unnamed_147) + fragment_unnamed_158;
				precise float fragment_unnamed_164 = fragment_unnamed_163 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_166 = 0.5f / fragment_unnamed_164;
				precise float fragment_unnamed_168 = fragment_unnamed_166 * 0.99999988079071044921875f;
				precise float fragment_unnamed_170 = fragment_unnamed_158 * fragment_unnamed_168;
				precise float fragment_unnamed_176 = fragment_unnamed_170 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_177 = fragment_unnamed_170 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_178 = fragment_unnamed_170 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_179 = fragment_unnamed_161 * fragment_unnamed_161;
				precise float fragment_unnamed_180 = fragment_unnamed_179 * fragment_unnamed_179;
				precise float fragment_unnamed_181 = fragment_unnamed_161 * fragment_unnamed_180;
				float fragment_unnamed_187 = rsqrt(max(dot(float3(fragment_unnamed_144, fragment_unnamed_145, fragment_unnamed_146), float3(fragment_unnamed_144, fragment_unnamed_145, fragment_unnamed_146)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_188 = fragment_unnamed_187 * fragment_unnamed_144;
				precise float fragment_unnamed_189 = fragment_unnamed_187 * fragment_unnamed_145;
				precise float fragment_unnamed_190 = fragment_unnamed_187 * fragment_unnamed_146;
				float fragment_unnamed_199 = clamp(dot(float3(fragment_uniform_buffer_2[0u].xyz), float3(fragment_unnamed_188, fragment_unnamed_189, fragment_unnamed_190)), 0.0f, 1.0f);
				precise float fragment_unnamed_203 = (-0.0f) - fragment_unnamed_199;
				precise float fragment_unnamed_204 = fragment_unnamed_203 + 1.0f;
				precise float fragment_unnamed_205 = dot(fragment_unnamed_199.xx, fragment_unnamed_199.xx) + (-0.5f);
				precise float fragment_unnamed_208 = (-0.0f) - fragment_unnamed_158;
				precise float fragment_unnamed_209 = fragment_unnamed_208 + 1.0f;
				precise float fragment_unnamed_210 = fragment_unnamed_209 * fragment_unnamed_209;
				precise float fragment_unnamed_211 = fragment_unnamed_210 * fragment_unnamed_210;
				precise float fragment_unnamed_212 = fragment_unnamed_209 * fragment_unnamed_211;
				precise float fragment_unnamed_214 = mad(fragment_unnamed_205, fragment_unnamed_181, 1.0f) * mad(fragment_unnamed_205, fragment_unnamed_212, 1.0f);
				precise float fragment_unnamed_215 = fragment_unnamed_158 * fragment_unnamed_214;
				precise float fragment_unnamed_221 = fragment_unnamed_215 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_222 = fragment_unnamed_215 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_223 = fragment_unnamed_215 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_224 = fragment_unnamed_204 * fragment_unnamed_204;
				precise float fragment_unnamed_225 = fragment_unnamed_224 * fragment_unnamed_224;
				precise float fragment_unnamed_226 = fragment_unnamed_204 * fragment_unnamed_225;
				float fragment_unnamed_227 = mad(fragment_unnamed_226, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_230 = fragment_unnamed_227 * fragment_unnamed_176;
				precise float fragment_unnamed_231 = fragment_unnamed_227 * fragment_unnamed_177;
				precise float fragment_unnamed_232 = fragment_unnamed_227 * fragment_unnamed_178;
				float4 fragment_unnamed_238 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_250 = fragment_unnamed_238.x * fragment_uniform_buffer_0[4u].x;
				precise float fragment_unnamed_251 = fragment_unnamed_238.y * fragment_uniform_buffer_0[4u].y;
				precise float fragment_unnamed_252 = fragment_unnamed_238.z * fragment_uniform_buffer_0[4u].z;
				precise float fragment_unnamed_253 = fragment_unnamed_238.w * fragment_uniform_buffer_0[4u].w;
				precise float fragment_unnamed_260 = fragment_unnamed_250 * fragment_input_6.x;
				precise float fragment_unnamed_261 = fragment_unnamed_251 * fragment_input_6.y;
				precise float fragment_unnamed_262 = fragment_unnamed_252 * fragment_input_6.z;
				precise float fragment_unnamed_266 = fragment_unnamed_253 * fragment_input_6.w;
				fragment_output_0.w = fragment_unnamed_266;
				precise float fragment_unnamed_269 = fragment_unnamed_260 * 0.959999978542327880859375f;
				precise float fragment_unnamed_270 = fragment_unnamed_261 * 0.959999978542327880859375f;
				precise float fragment_unnamed_271 = fragment_unnamed_262 * 0.959999978542327880859375f;
				fragment_output_0.x = mad(fragment_unnamed_269, fragment_unnamed_221, fragment_unnamed_230);
				fragment_output_0.y = mad(fragment_unnamed_270, fragment_unnamed_222, fragment_unnamed_231);
				fragment_output_0.z = mad(fragment_unnamed_271, fragment_unnamed_223, fragment_unnamed_232);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef SPOT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef POINT_COOKIE
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;

			static float4 fragment_uniform_buffer_0[9];
			static float4 fragment_uniform_buffer_1[5];
			static float4 fragment_uniform_buffer_2[1];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			Texture2D<float4> _LightTexture0;
			Texture2D<float4> _LightTextureB0;
			SamplerState sampler_LightTexture0;
			SamplerState sampler_LightTextureB0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float4 fragment_input_8;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float3 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float3 fragment_input_5 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_6 : COLOR; // COLOR
				float4 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
				float4 fragment_input_8 : TEXCOORD6; // TEXCOORD_6
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float4 fragment_unnamed_68 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_73 = fragment_unnamed_68.w * fragment_unnamed_68.x;
				float fragment_unnamed_75 = mad(fragment_unnamed_73, 2.0f, -1.0f);
				float fragment_unnamed_78 = mad(fragment_unnamed_68.y, 2.0f, -1.0f);
				precise float fragment_unnamed_84 = (-0.0f) - min(dot(float2(fragment_unnamed_75, fragment_unnamed_78), float2(fragment_unnamed_75, fragment_unnamed_78)), 1.0f);
				precise float fragment_unnamed_86 = fragment_unnamed_84 + 1.0f;
				float fragment_unnamed_87 = sqrt(fragment_unnamed_86);
				float fragment_unnamed_95 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_75, fragment_unnamed_78, fragment_unnamed_87));
				float fragment_unnamed_104 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_75, fragment_unnamed_78, fragment_unnamed_87));
				float fragment_unnamed_113 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_75, fragment_unnamed_78, fragment_unnamed_87));
				float fragment_unnamed_119 = rsqrt(dot(float3(fragment_unnamed_95, fragment_unnamed_104, fragment_unnamed_113), float3(fragment_unnamed_95, fragment_unnamed_104, fragment_unnamed_113)));
				precise float fragment_unnamed_120 = fragment_unnamed_119 * fragment_unnamed_95;
				precise float fragment_unnamed_121 = fragment_unnamed_119 * fragment_unnamed_104;
				precise float fragment_unnamed_122 = fragment_unnamed_119 * fragment_unnamed_113;
				precise float fragment_unnamed_125 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_128 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_131 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_139 = fragment_unnamed_125 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_140 = fragment_unnamed_128 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_141 = fragment_unnamed_131 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_145 = rsqrt(dot(float3(fragment_unnamed_139, fragment_unnamed_140, fragment_unnamed_141), float3(fragment_unnamed_139, fragment_unnamed_140, fragment_unnamed_141)));
				precise float fragment_unnamed_146 = fragment_unnamed_145 * fragment_unnamed_139;
				precise float fragment_unnamed_147 = fragment_unnamed_145 * fragment_unnamed_140;
				precise float fragment_unnamed_148 = fragment_unnamed_145 * fragment_unnamed_141;
				float fragment_unnamed_149 = dot(float3(fragment_unnamed_120, fragment_unnamed_121, fragment_unnamed_122), float3(fragment_unnamed_146, fragment_unnamed_147, fragment_unnamed_148));
				precise float fragment_unnamed_153 = (-0.0f) - abs(fragment_unnamed_149);
				precise float fragment_unnamed_154 = fragment_unnamed_153 + 1.0f;
				precise float fragment_unnamed_155 = fragment_unnamed_154 * fragment_unnamed_154;
				precise float fragment_unnamed_156 = fragment_unnamed_155 * fragment_unnamed_155;
				precise float fragment_unnamed_157 = fragment_unnamed_154 * fragment_unnamed_156;
				precise float fragment_unnamed_160 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_163 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_166 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_172 = fragment_unnamed_160 + fragment_uniform_buffer_2[0u].x;
				precise float fragment_unnamed_173 = fragment_unnamed_163 + fragment_uniform_buffer_2[0u].y;
				precise float fragment_unnamed_174 = fragment_unnamed_166 + fragment_uniform_buffer_2[0u].z;
				float fragment_unnamed_178 = rsqrt(dot(float3(fragment_unnamed_172, fragment_unnamed_173, fragment_unnamed_174), float3(fragment_unnamed_172, fragment_unnamed_173, fragment_unnamed_174)));
				float fragment_unnamed_179 = mad(fragment_unnamed_172, fragment_unnamed_178, fragment_unnamed_146);
				float fragment_unnamed_180 = mad(fragment_unnamed_173, fragment_unnamed_178, fragment_unnamed_147);
				float fragment_unnamed_181 = mad(fragment_unnamed_174, fragment_unnamed_178, fragment_unnamed_148);
				precise float fragment_unnamed_182 = fragment_unnamed_178 * fragment_unnamed_172;
				precise float fragment_unnamed_183 = fragment_unnamed_178 * fragment_unnamed_173;
				precise float fragment_unnamed_184 = fragment_unnamed_178 * fragment_unnamed_174;
				float fragment_unnamed_190 = rsqrt(max(dot(float3(fragment_unnamed_179, fragment_unnamed_180, fragment_unnamed_181), float3(fragment_unnamed_179, fragment_unnamed_180, fragment_unnamed_181)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_191 = fragment_unnamed_179 * fragment_unnamed_190;
				precise float fragment_unnamed_192 = fragment_unnamed_180 * fragment_unnamed_190;
				precise float fragment_unnamed_193 = fragment_unnamed_181 * fragment_unnamed_190;
				float fragment_unnamed_197 = clamp(dot(float3(fragment_unnamed_182, fragment_unnamed_183, fragment_unnamed_184), float3(fragment_unnamed_191, fragment_unnamed_192, fragment_unnamed_193)), 0.0f, 1.0f);
				float fragment_unnamed_201 = clamp(dot(float3(fragment_unnamed_120, fragment_unnamed_121, fragment_unnamed_122), float3(fragment_unnamed_182, fragment_unnamed_183, fragment_unnamed_184)), 0.0f, 1.0f);
				precise float fragment_unnamed_205 = (-0.0f) - fragment_unnamed_197;
				precise float fragment_unnamed_206 = fragment_unnamed_205 + 1.0f;
				precise float fragment_unnamed_207 = dot(fragment_unnamed_197.xx, fragment_unnamed_197.xx) + (-0.5f);
				precise float fragment_unnamed_210 = (-0.0f) - fragment_unnamed_201;
				precise float fragment_unnamed_211 = fragment_unnamed_210 + 1.0f;
				precise float fragment_unnamed_212 = fragment_unnamed_211 * fragment_unnamed_211;
				precise float fragment_unnamed_213 = fragment_unnamed_212 * fragment_unnamed_212;
				precise float fragment_unnamed_214 = fragment_unnamed_211 * fragment_unnamed_213;
				precise float fragment_unnamed_216 = mad(fragment_unnamed_207, fragment_unnamed_157, 1.0f) * mad(fragment_unnamed_207, fragment_unnamed_214, 1.0f);
				precise float fragment_unnamed_217 = fragment_unnamed_201 * fragment_unnamed_216;
				precise float fragment_unnamed_226 = fragment_input_5.y * fragment_uniform_buffer_0[5u].x;
				precise float fragment_unnamed_227 = fragment_input_5.y * fragment_uniform_buffer_0[5u].y;
				precise float fragment_unnamed_228 = fragment_input_5.y * fragment_uniform_buffer_0[5u].z;
				precise float fragment_unnamed_229 = fragment_input_5.y * fragment_uniform_buffer_0[5u].w;
				precise float fragment_unnamed_262 = mad(fragment_uniform_buffer_0[6u].x, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].x, fragment_input_5.x, fragment_unnamed_226)) + fragment_uniform_buffer_0[7u].x;
				precise float fragment_unnamed_263 = mad(fragment_uniform_buffer_0[6u].y, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].y, fragment_input_5.x, fragment_unnamed_227)) + fragment_uniform_buffer_0[7u].y;
				precise float fragment_unnamed_264 = mad(fragment_uniform_buffer_0[6u].z, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].z, fragment_input_5.x, fragment_unnamed_228)) + fragment_uniform_buffer_0[7u].z;
				precise float fragment_unnamed_265 = mad(fragment_uniform_buffer_0[6u].w, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].w, fragment_input_5.x, fragment_unnamed_229)) + fragment_uniform_buffer_0[7u].w;
				precise float fragment_unnamed_266 = fragment_unnamed_262 / fragment_unnamed_265;
				precise float fragment_unnamed_267 = fragment_unnamed_263 / fragment_unnamed_265;
				precise float fragment_unnamed_268 = fragment_unnamed_266 + 0.5f;
				precise float fragment_unnamed_270 = fragment_unnamed_267 + 0.5f;
				precise float fragment_unnamed_289 = _LightTexture0.Sample(sampler_LightTexture0, float2(fragment_unnamed_268, fragment_unnamed_270)).w * asfloat(((0.0f < fragment_unnamed_264) ? 4294967295u : 0u) & 1065353216u);
				precise float fragment_unnamed_290 = _LightTextureB0.Sample(sampler_LightTextureB0, dot(float3(fragment_unnamed_262, fragment_unnamed_263, fragment_unnamed_264), float3(fragment_unnamed_262, fragment_unnamed_263, fragment_unnamed_264)).xx).x * fragment_unnamed_289;
				precise float fragment_unnamed_296 = fragment_unnamed_290 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_297 = fragment_unnamed_290 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_298 = fragment_unnamed_290 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_299 = fragment_unnamed_217 * fragment_unnamed_296;
				precise float fragment_unnamed_300 = fragment_unnamed_217 * fragment_unnamed_297;
				precise float fragment_unnamed_301 = fragment_unnamed_217 * fragment_unnamed_298;
				precise float fragment_unnamed_303 = abs(fragment_unnamed_149) + fragment_unnamed_201;
				precise float fragment_unnamed_304 = fragment_unnamed_303 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_306 = 0.5f / fragment_unnamed_304;
				precise float fragment_unnamed_307 = fragment_unnamed_306 * 0.99999988079071044921875f;
				precise float fragment_unnamed_309 = fragment_unnamed_201 * fragment_unnamed_307;
				precise float fragment_unnamed_310 = fragment_unnamed_296 * fragment_unnamed_309;
				precise float fragment_unnamed_311 = fragment_unnamed_297 * fragment_unnamed_309;
				precise float fragment_unnamed_312 = fragment_unnamed_298 * fragment_unnamed_309;
				precise float fragment_unnamed_313 = fragment_unnamed_206 * fragment_unnamed_206;
				precise float fragment_unnamed_314 = fragment_unnamed_313 * fragment_unnamed_313;
				precise float fragment_unnamed_315 = fragment_unnamed_206 * fragment_unnamed_314;
				float fragment_unnamed_316 = mad(fragment_unnamed_315, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_319 = fragment_unnamed_316 * fragment_unnamed_310;
				precise float fragment_unnamed_320 = fragment_unnamed_316 * fragment_unnamed_311;
				precise float fragment_unnamed_321 = fragment_unnamed_316 * fragment_unnamed_312;
				float4 fragment_unnamed_327 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_340 = fragment_unnamed_327.x * fragment_uniform_buffer_0[8u].x;
				precise float fragment_unnamed_341 = fragment_unnamed_327.y * fragment_uniform_buffer_0[8u].y;
				precise float fragment_unnamed_342 = fragment_unnamed_327.z * fragment_uniform_buffer_0[8u].z;
				precise float fragment_unnamed_343 = fragment_unnamed_327.w * fragment_uniform_buffer_0[8u].w;
				precise float fragment_unnamed_350 = fragment_unnamed_340 * fragment_input_6.x;
				precise float fragment_unnamed_351 = fragment_unnamed_341 * fragment_input_6.y;
				precise float fragment_unnamed_352 = fragment_unnamed_342 * fragment_input_6.z;
				precise float fragment_unnamed_356 = fragment_unnamed_343 * fragment_input_6.w;
				fragment_output_0.w = fragment_unnamed_356;
				precise float fragment_unnamed_359 = fragment_unnamed_350 * 0.959999978542327880859375f;
				precise float fragment_unnamed_360 = fragment_unnamed_351 * 0.959999978542327880859375f;
				precise float fragment_unnamed_361 = fragment_unnamed_352 * 0.959999978542327880859375f;
				fragment_output_0.x = mad(fragment_unnamed_359, fragment_unnamed_299, fragment_unnamed_319);
				fragment_output_0.y = mad(fragment_unnamed_360, fragment_unnamed_300, fragment_unnamed_320);
				fragment_output_0.z = mad(fragment_unnamed_361, fragment_unnamed_301, fragment_unnamed_321);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				fragment_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				fragment_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				fragment_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_uniform_buffer_0[8] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				fragment_input_8 = stage_input.fragment_input_8;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // SPOT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !POINT_COOKIE


			#ifdef POINT_COOKIE
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;

			static float4 fragment_uniform_buffer_0[9];
			static float4 fragment_uniform_buffer_1[5];
			static float4 fragment_uniform_buffer_2[1];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			Texture2D<float4> _LightTextureB0;
			TextureCube<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			SamplerState sampler_LightTextureB0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float3 fragment_input_8;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float3 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float3 fragment_input_5 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_6 : COLOR; // COLOR
				float4 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
				float3 fragment_input_8 : TEXCOORD6; // TEXCOORD_6
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float4 fragment_unnamed_70 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_75 = fragment_unnamed_70.w * fragment_unnamed_70.x;
				float fragment_unnamed_77 = mad(fragment_unnamed_75, 2.0f, -1.0f);
				float fragment_unnamed_80 = mad(fragment_unnamed_70.y, 2.0f, -1.0f);
				precise float fragment_unnamed_86 = (-0.0f) - min(dot(float2(fragment_unnamed_77, fragment_unnamed_80), float2(fragment_unnamed_77, fragment_unnamed_80)), 1.0f);
				precise float fragment_unnamed_88 = fragment_unnamed_86 + 1.0f;
				float fragment_unnamed_89 = sqrt(fragment_unnamed_88);
				float fragment_unnamed_97 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_77, fragment_unnamed_80, fragment_unnamed_89));
				float fragment_unnamed_106 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_77, fragment_unnamed_80, fragment_unnamed_89));
				float fragment_unnamed_115 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_77, fragment_unnamed_80, fragment_unnamed_89));
				float fragment_unnamed_121 = rsqrt(dot(float3(fragment_unnamed_97, fragment_unnamed_106, fragment_unnamed_115), float3(fragment_unnamed_97, fragment_unnamed_106, fragment_unnamed_115)));
				precise float fragment_unnamed_122 = fragment_unnamed_121 * fragment_unnamed_97;
				precise float fragment_unnamed_123 = fragment_unnamed_121 * fragment_unnamed_106;
				precise float fragment_unnamed_124 = fragment_unnamed_121 * fragment_unnamed_115;
				precise float fragment_unnamed_127 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_130 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_133 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_141 = fragment_unnamed_127 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_142 = fragment_unnamed_130 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_143 = fragment_unnamed_133 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_147 = rsqrt(dot(float3(fragment_unnamed_141, fragment_unnamed_142, fragment_unnamed_143), float3(fragment_unnamed_141, fragment_unnamed_142, fragment_unnamed_143)));
				precise float fragment_unnamed_148 = fragment_unnamed_147 * fragment_unnamed_141;
				precise float fragment_unnamed_149 = fragment_unnamed_147 * fragment_unnamed_142;
				precise float fragment_unnamed_150 = fragment_unnamed_147 * fragment_unnamed_143;
				float fragment_unnamed_151 = dot(float3(fragment_unnamed_122, fragment_unnamed_123, fragment_unnamed_124), float3(fragment_unnamed_148, fragment_unnamed_149, fragment_unnamed_150));
				precise float fragment_unnamed_155 = (-0.0f) - abs(fragment_unnamed_151);
				precise float fragment_unnamed_156 = fragment_unnamed_155 + 1.0f;
				precise float fragment_unnamed_157 = fragment_unnamed_156 * fragment_unnamed_156;
				precise float fragment_unnamed_158 = fragment_unnamed_157 * fragment_unnamed_157;
				precise float fragment_unnamed_159 = fragment_unnamed_156 * fragment_unnamed_158;
				precise float fragment_unnamed_162 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_165 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_168 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_174 = fragment_unnamed_162 + fragment_uniform_buffer_2[0u].x;
				precise float fragment_unnamed_175 = fragment_unnamed_165 + fragment_uniform_buffer_2[0u].y;
				precise float fragment_unnamed_176 = fragment_unnamed_168 + fragment_uniform_buffer_2[0u].z;
				float fragment_unnamed_180 = rsqrt(dot(float3(fragment_unnamed_174, fragment_unnamed_175, fragment_unnamed_176), float3(fragment_unnamed_174, fragment_unnamed_175, fragment_unnamed_176)));
				float fragment_unnamed_181 = mad(fragment_unnamed_174, fragment_unnamed_180, fragment_unnamed_148);
				float fragment_unnamed_182 = mad(fragment_unnamed_175, fragment_unnamed_180, fragment_unnamed_149);
				float fragment_unnamed_183 = mad(fragment_unnamed_176, fragment_unnamed_180, fragment_unnamed_150);
				precise float fragment_unnamed_184 = fragment_unnamed_180 * fragment_unnamed_174;
				precise float fragment_unnamed_185 = fragment_unnamed_180 * fragment_unnamed_175;
				precise float fragment_unnamed_186 = fragment_unnamed_180 * fragment_unnamed_176;
				float fragment_unnamed_192 = rsqrt(max(dot(float3(fragment_unnamed_181, fragment_unnamed_182, fragment_unnamed_183), float3(fragment_unnamed_181, fragment_unnamed_182, fragment_unnamed_183)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_193 = fragment_unnamed_181 * fragment_unnamed_192;
				precise float fragment_unnamed_194 = fragment_unnamed_182 * fragment_unnamed_192;
				precise float fragment_unnamed_195 = fragment_unnamed_183 * fragment_unnamed_192;
				float fragment_unnamed_199 = clamp(dot(float3(fragment_unnamed_184, fragment_unnamed_185, fragment_unnamed_186), float3(fragment_unnamed_193, fragment_unnamed_194, fragment_unnamed_195)), 0.0f, 1.0f);
				float fragment_unnamed_203 = clamp(dot(float3(fragment_unnamed_122, fragment_unnamed_123, fragment_unnamed_124), float3(fragment_unnamed_184, fragment_unnamed_185, fragment_unnamed_186)), 0.0f, 1.0f);
				precise float fragment_unnamed_207 = (-0.0f) - fragment_unnamed_199;
				precise float fragment_unnamed_208 = fragment_unnamed_207 + 1.0f;
				precise float fragment_unnamed_209 = dot(fragment_unnamed_199.xx, fragment_unnamed_199.xx) + (-0.5f);
				precise float fragment_unnamed_212 = (-0.0f) - fragment_unnamed_203;
				precise float fragment_unnamed_213 = fragment_unnamed_212 + 1.0f;
				precise float fragment_unnamed_214 = fragment_unnamed_213 * fragment_unnamed_213;
				precise float fragment_unnamed_215 = fragment_unnamed_214 * fragment_unnamed_214;
				precise float fragment_unnamed_216 = fragment_unnamed_213 * fragment_unnamed_215;
				precise float fragment_unnamed_218 = mad(fragment_unnamed_209, fragment_unnamed_159, 1.0f) * mad(fragment_unnamed_209, fragment_unnamed_216, 1.0f);
				precise float fragment_unnamed_219 = fragment_unnamed_203 * fragment_unnamed_218;
				precise float fragment_unnamed_227 = fragment_input_5.y * fragment_uniform_buffer_0[5u].x;
				precise float fragment_unnamed_228 = fragment_input_5.y * fragment_uniform_buffer_0[5u].y;
				precise float fragment_unnamed_229 = fragment_input_5.y * fragment_uniform_buffer_0[5u].z;
				precise float fragment_unnamed_257 = mad(fragment_uniform_buffer_0[6u].x, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].x, fragment_input_5.x, fragment_unnamed_227)) + fragment_uniform_buffer_0[7u].x;
				precise float fragment_unnamed_258 = mad(fragment_uniform_buffer_0[6u].y, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].y, fragment_input_5.x, fragment_unnamed_228)) + fragment_uniform_buffer_0[7u].y;
				precise float fragment_unnamed_259 = mad(fragment_uniform_buffer_0[6u].z, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].z, fragment_input_5.x, fragment_unnamed_229)) + fragment_uniform_buffer_0[7u].z;
				precise float fragment_unnamed_272 = _LightTexture0.Sample(sampler_LightTexture0, float3(fragment_unnamed_257, fragment_unnamed_258, fragment_unnamed_259)).w * _LightTextureB0.Sample(sampler_LightTextureB0, dot(float3(fragment_unnamed_257, fragment_unnamed_258, fragment_unnamed_259), float3(fragment_unnamed_257, fragment_unnamed_258, fragment_unnamed_259)).xx).x;
				precise float fragment_unnamed_278 = fragment_unnamed_272 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_279 = fragment_unnamed_272 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_280 = fragment_unnamed_272 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_281 = fragment_unnamed_219 * fragment_unnamed_278;
				precise float fragment_unnamed_282 = fragment_unnamed_219 * fragment_unnamed_279;
				precise float fragment_unnamed_283 = fragment_unnamed_219 * fragment_unnamed_280;
				precise float fragment_unnamed_285 = abs(fragment_unnamed_151) + fragment_unnamed_203;
				precise float fragment_unnamed_286 = fragment_unnamed_285 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_288 = 0.5f / fragment_unnamed_286;
				precise float fragment_unnamed_290 = fragment_unnamed_288 * 0.99999988079071044921875f;
				precise float fragment_unnamed_292 = fragment_unnamed_203 * fragment_unnamed_290;
				precise float fragment_unnamed_293 = fragment_unnamed_278 * fragment_unnamed_292;
				precise float fragment_unnamed_294 = fragment_unnamed_279 * fragment_unnamed_292;
				precise float fragment_unnamed_295 = fragment_unnamed_280 * fragment_unnamed_292;
				precise float fragment_unnamed_296 = fragment_unnamed_208 * fragment_unnamed_208;
				precise float fragment_unnamed_297 = fragment_unnamed_296 * fragment_unnamed_296;
				precise float fragment_unnamed_298 = fragment_unnamed_208 * fragment_unnamed_297;
				float fragment_unnamed_299 = mad(fragment_unnamed_298, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_302 = fragment_unnamed_299 * fragment_unnamed_293;
				precise float fragment_unnamed_303 = fragment_unnamed_299 * fragment_unnamed_294;
				precise float fragment_unnamed_304 = fragment_unnamed_299 * fragment_unnamed_295;
				float4 fragment_unnamed_310 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_323 = fragment_unnamed_310.x * fragment_uniform_buffer_0[8u].x;
				precise float fragment_unnamed_324 = fragment_unnamed_310.y * fragment_uniform_buffer_0[8u].y;
				precise float fragment_unnamed_325 = fragment_unnamed_310.z * fragment_uniform_buffer_0[8u].z;
				precise float fragment_unnamed_326 = fragment_unnamed_310.w * fragment_uniform_buffer_0[8u].w;
				precise float fragment_unnamed_333 = fragment_unnamed_323 * fragment_input_6.x;
				precise float fragment_unnamed_334 = fragment_unnamed_324 * fragment_input_6.y;
				precise float fragment_unnamed_335 = fragment_unnamed_325 * fragment_input_6.z;
				precise float fragment_unnamed_339 = fragment_unnamed_326 * fragment_input_6.w;
				fragment_output_0.w = fragment_unnamed_339;
				precise float fragment_unnamed_342 = fragment_unnamed_333 * 0.959999978542327880859375f;
				precise float fragment_unnamed_343 = fragment_unnamed_334 * 0.959999978542327880859375f;
				precise float fragment_unnamed_344 = fragment_unnamed_335 * 0.959999978542327880859375f;
				fragment_output_0.x = mad(fragment_unnamed_342, fragment_unnamed_281, fragment_unnamed_302);
				fragment_output_0.y = mad(fragment_unnamed_343, fragment_unnamed_282, fragment_unnamed_303);
				fragment_output_0.z = mad(fragment_unnamed_344, fragment_unnamed_283, fragment_unnamed_304);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				fragment_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				fragment_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				fragment_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_uniform_buffer_0[8] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				fragment_input_8 = stage_input.fragment_input_8;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // POINT_COOKIE
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !SPOT


			#ifdef DIRECTIONAL_COOKIE
			#ifndef DIRECTIONAL
			#ifndef FOG_LINEAR
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _WorldSpaceLightPos0;

			static float4 fragment_uniform_buffer_0[9];
			static float4 fragment_uniform_buffer_1[5];
			static float4 fragment_uniform_buffer_2[1];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			Texture2D<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float2 fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float2 fragment_input_1 : TEXCOORD6; // TEXCOORD_6
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float3 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float3 fragment_input_5 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_6 : COLOR; // COLOR
				float4 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float4 fragment_unnamed_64 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_69 = fragment_unnamed_64.w * fragment_unnamed_64.x;
				float fragment_unnamed_71 = mad(fragment_unnamed_69, 2.0f, -1.0f);
				float fragment_unnamed_74 = mad(fragment_unnamed_64.y, 2.0f, -1.0f);
				precise float fragment_unnamed_80 = (-0.0f) - min(dot(float2(fragment_unnamed_71, fragment_unnamed_74), float2(fragment_unnamed_71, fragment_unnamed_74)), 1.0f);
				precise float fragment_unnamed_82 = fragment_unnamed_80 + 1.0f;
				float fragment_unnamed_83 = sqrt(fragment_unnamed_82);
				float fragment_unnamed_91 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_71, fragment_unnamed_74, fragment_unnamed_83));
				float fragment_unnamed_100 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_71, fragment_unnamed_74, fragment_unnamed_83));
				float fragment_unnamed_109 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_71, fragment_unnamed_74, fragment_unnamed_83));
				float fragment_unnamed_115 = rsqrt(dot(float3(fragment_unnamed_91, fragment_unnamed_100, fragment_unnamed_109), float3(fragment_unnamed_91, fragment_unnamed_100, fragment_unnamed_109)));
				precise float fragment_unnamed_116 = fragment_unnamed_115 * fragment_unnamed_91;
				precise float fragment_unnamed_117 = fragment_unnamed_115 * fragment_unnamed_100;
				precise float fragment_unnamed_118 = fragment_unnamed_115 * fragment_unnamed_109;
				precise float fragment_unnamed_121 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_124 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_127 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_135 = fragment_unnamed_121 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_136 = fragment_unnamed_124 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_137 = fragment_unnamed_127 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_141 = rsqrt(dot(float3(fragment_unnamed_135, fragment_unnamed_136, fragment_unnamed_137), float3(fragment_unnamed_135, fragment_unnamed_136, fragment_unnamed_137)));
				precise float fragment_unnamed_142 = fragment_unnamed_141 * fragment_unnamed_135;
				precise float fragment_unnamed_143 = fragment_unnamed_141 * fragment_unnamed_136;
				precise float fragment_unnamed_144 = fragment_unnamed_141 * fragment_unnamed_137;
				float fragment_unnamed_150 = mad(fragment_unnamed_135, fragment_unnamed_141, fragment_uniform_buffer_2[0u].x);
				float fragment_unnamed_151 = mad(fragment_unnamed_136, fragment_unnamed_141, fragment_uniform_buffer_2[0u].y);
				float fragment_unnamed_152 = mad(fragment_unnamed_137, fragment_unnamed_141, fragment_uniform_buffer_2[0u].z);
				float fragment_unnamed_153 = dot(float3(fragment_unnamed_116, fragment_unnamed_117, fragment_unnamed_118), float3(fragment_unnamed_142, fragment_unnamed_143, fragment_unnamed_144));
				float fragment_unnamed_164 = clamp(dot(float3(fragment_unnamed_116, fragment_unnamed_117, fragment_unnamed_118), float3(fragment_uniform_buffer_2[0u].xyz)), 0.0f, 1.0f);
				precise float fragment_unnamed_166 = (-0.0f) - abs(fragment_unnamed_153);
				precise float fragment_unnamed_167 = fragment_unnamed_166 + 1.0f;
				precise float fragment_unnamed_169 = abs(fragment_unnamed_153) + fragment_unnamed_164;
				precise float fragment_unnamed_170 = fragment_unnamed_169 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_172 = 0.5f / fragment_unnamed_170;
				precise float fragment_unnamed_174 = fragment_unnamed_172 * 0.99999988079071044921875f;
				precise float fragment_unnamed_176 = fragment_unnamed_164 * fragment_unnamed_174;
				precise float fragment_unnamed_177 = fragment_unnamed_167 * fragment_unnamed_167;
				precise float fragment_unnamed_178 = fragment_unnamed_177 * fragment_unnamed_177;
				precise float fragment_unnamed_179 = fragment_unnamed_167 * fragment_unnamed_178;
				float fragment_unnamed_185 = rsqrt(max(dot(float3(fragment_unnamed_150, fragment_unnamed_151, fragment_unnamed_152), float3(fragment_unnamed_150, fragment_unnamed_151, fragment_unnamed_152)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_186 = fragment_unnamed_185 * fragment_unnamed_150;
				precise float fragment_unnamed_187 = fragment_unnamed_185 * fragment_unnamed_151;
				precise float fragment_unnamed_188 = fragment_unnamed_185 * fragment_unnamed_152;
				float fragment_unnamed_197 = clamp(dot(float3(fragment_uniform_buffer_2[0u].xyz), float3(fragment_unnamed_186, fragment_unnamed_187, fragment_unnamed_188)), 0.0f, 1.0f);
				precise float fragment_unnamed_201 = (-0.0f) - fragment_unnamed_197;
				precise float fragment_unnamed_202 = fragment_unnamed_201 + 1.0f;
				precise float fragment_unnamed_203 = dot(fragment_unnamed_197.xx, fragment_unnamed_197.xx) + (-0.5f);
				precise float fragment_unnamed_206 = (-0.0f) - fragment_unnamed_164;
				precise float fragment_unnamed_207 = fragment_unnamed_206 + 1.0f;
				precise float fragment_unnamed_208 = fragment_unnamed_207 * fragment_unnamed_207;
				precise float fragment_unnamed_209 = fragment_unnamed_208 * fragment_unnamed_208;
				precise float fragment_unnamed_210 = fragment_unnamed_207 * fragment_unnamed_209;
				precise float fragment_unnamed_212 = mad(fragment_unnamed_203, fragment_unnamed_179, 1.0f) * mad(fragment_unnamed_203, fragment_unnamed_210, 1.0f);
				precise float fragment_unnamed_213 = fragment_unnamed_164 * fragment_unnamed_212;
				precise float fragment_unnamed_220 = fragment_input_5.y * fragment_uniform_buffer_0[5u].x;
				precise float fragment_unnamed_221 = fragment_input_5.y * fragment_uniform_buffer_0[5u].y;
				precise float fragment_unnamed_244 = mad(fragment_uniform_buffer_0[6u].x, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].x, fragment_input_5.x, fragment_unnamed_220)) + fragment_uniform_buffer_0[7u].x;
				precise float fragment_unnamed_245 = mad(fragment_uniform_buffer_0[6u].y, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].y, fragment_input_5.x, fragment_unnamed_221)) + fragment_uniform_buffer_0[7u].y;
				float4 fragment_unnamed_247 = _LightTexture0.Sample(sampler_LightTexture0, float2(fragment_unnamed_244, fragment_unnamed_245));
				float fragment_unnamed_249 = fragment_unnamed_247.w;
				precise float fragment_unnamed_255 = fragment_unnamed_249 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_256 = fragment_unnamed_249 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_257 = fragment_unnamed_249 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_258 = fragment_unnamed_213 * fragment_unnamed_255;
				precise float fragment_unnamed_259 = fragment_unnamed_213 * fragment_unnamed_256;
				precise float fragment_unnamed_260 = fragment_unnamed_213 * fragment_unnamed_257;
				precise float fragment_unnamed_261 = fragment_unnamed_176 * fragment_unnamed_255;
				precise float fragment_unnamed_262 = fragment_unnamed_176 * fragment_unnamed_256;
				precise float fragment_unnamed_263 = fragment_unnamed_176 * fragment_unnamed_257;
				precise float fragment_unnamed_264 = fragment_unnamed_202 * fragment_unnamed_202;
				precise float fragment_unnamed_265 = fragment_unnamed_264 * fragment_unnamed_264;
				precise float fragment_unnamed_266 = fragment_unnamed_202 * fragment_unnamed_265;
				float fragment_unnamed_267 = mad(fragment_unnamed_266, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_270 = fragment_unnamed_267 * fragment_unnamed_261;
				precise float fragment_unnamed_271 = fragment_unnamed_267 * fragment_unnamed_262;
				precise float fragment_unnamed_272 = fragment_unnamed_267 * fragment_unnamed_263;
				float4 fragment_unnamed_278 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_291 = fragment_unnamed_278.x * fragment_uniform_buffer_0[8u].x;
				precise float fragment_unnamed_292 = fragment_unnamed_278.y * fragment_uniform_buffer_0[8u].y;
				precise float fragment_unnamed_293 = fragment_unnamed_278.z * fragment_uniform_buffer_0[8u].z;
				precise float fragment_unnamed_294 = fragment_unnamed_278.w * fragment_uniform_buffer_0[8u].w;
				precise float fragment_unnamed_301 = fragment_unnamed_291 * fragment_input_6.x;
				precise float fragment_unnamed_302 = fragment_unnamed_292 * fragment_input_6.y;
				precise float fragment_unnamed_303 = fragment_unnamed_293 * fragment_input_6.z;
				precise float fragment_unnamed_307 = fragment_unnamed_294 * fragment_input_6.w;
				fragment_output_0.w = fragment_unnamed_307;
				precise float fragment_unnamed_310 = fragment_unnamed_301 * 0.959999978542327880859375f;
				precise float fragment_unnamed_311 = fragment_unnamed_302 * 0.959999978542327880859375f;
				precise float fragment_unnamed_312 = fragment_unnamed_303 * 0.959999978542327880859375f;
				fragment_output_0.x = mad(fragment_unnamed_310, fragment_unnamed_258, fragment_unnamed_270);
				fragment_output_0.y = mad(fragment_unnamed_311, fragment_unnamed_259, fragment_unnamed_271);
				fragment_output_0.z = mad(fragment_unnamed_312, fragment_unnamed_260, fragment_unnamed_272);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				fragment_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				fragment_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				fragment_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_uniform_buffer_0[8] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL_COOKIE
			#endif // !DIRECTIONAL
			#endif // !FOG_LINEAR
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef FOG_LINEAR
			#ifdef POINT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogParams;

			static float4 fragment_uniform_buffer_0[9];
			static float4 fragment_uniform_buffer_1[6];
			static float4 fragment_uniform_buffer_2[1];
			static float4 fragment_uniform_buffer_3[2];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			Texture2D<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float3 fragment_input_8;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float fragment_input_1 : TEXCOORD7; // TEXCOORD_7
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float3 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float3 fragment_input_5 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_6 : COLOR; // COLOR
				float4 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
				float3 fragment_input_8 : TEXCOORD6; // TEXCOORD_6
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float4 fragment_unnamed_70 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_75 = fragment_unnamed_70.w * fragment_unnamed_70.x;
				float fragment_unnamed_77 = mad(fragment_unnamed_75, 2.0f, -1.0f);
				float fragment_unnamed_80 = mad(fragment_unnamed_70.y, 2.0f, -1.0f);
				precise float fragment_unnamed_86 = (-0.0f) - min(dot(float2(fragment_unnamed_77, fragment_unnamed_80), float2(fragment_unnamed_77, fragment_unnamed_80)), 1.0f);
				precise float fragment_unnamed_88 = fragment_unnamed_86 + 1.0f;
				float fragment_unnamed_89 = sqrt(fragment_unnamed_88);
				float fragment_unnamed_96 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_77, fragment_unnamed_80, fragment_unnamed_89));
				float fragment_unnamed_105 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_77, fragment_unnamed_80, fragment_unnamed_89));
				float fragment_unnamed_114 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_77, fragment_unnamed_80, fragment_unnamed_89));
				float fragment_unnamed_120 = rsqrt(dot(float3(fragment_unnamed_96, fragment_unnamed_105, fragment_unnamed_114), float3(fragment_unnamed_96, fragment_unnamed_105, fragment_unnamed_114)));
				precise float fragment_unnamed_121 = fragment_unnamed_120 * fragment_unnamed_96;
				precise float fragment_unnamed_122 = fragment_unnamed_120 * fragment_unnamed_105;
				precise float fragment_unnamed_123 = fragment_unnamed_120 * fragment_unnamed_114;
				precise float fragment_unnamed_126 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_129 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_132 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_140 = fragment_unnamed_126 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_141 = fragment_unnamed_129 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_142 = fragment_unnamed_132 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_146 = rsqrt(dot(float3(fragment_unnamed_140, fragment_unnamed_141, fragment_unnamed_142), float3(fragment_unnamed_140, fragment_unnamed_141, fragment_unnamed_142)));
				precise float fragment_unnamed_147 = fragment_unnamed_146 * fragment_unnamed_140;
				precise float fragment_unnamed_148 = fragment_unnamed_146 * fragment_unnamed_141;
				precise float fragment_unnamed_149 = fragment_unnamed_146 * fragment_unnamed_142;
				float fragment_unnamed_150 = dot(float3(fragment_unnamed_121, fragment_unnamed_122, fragment_unnamed_123), float3(fragment_unnamed_147, fragment_unnamed_148, fragment_unnamed_149));
				precise float fragment_unnamed_154 = (-0.0f) - abs(fragment_unnamed_150);
				precise float fragment_unnamed_155 = fragment_unnamed_154 + 1.0f;
				precise float fragment_unnamed_156 = fragment_unnamed_155 * fragment_unnamed_155;
				precise float fragment_unnamed_157 = fragment_unnamed_156 * fragment_unnamed_156;
				precise float fragment_unnamed_158 = fragment_unnamed_155 * fragment_unnamed_157;
				precise float fragment_unnamed_161 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_164 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_167 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_173 = fragment_unnamed_161 + fragment_uniform_buffer_2[0u].x;
				precise float fragment_unnamed_174 = fragment_unnamed_164 + fragment_uniform_buffer_2[0u].y;
				precise float fragment_unnamed_175 = fragment_unnamed_167 + fragment_uniform_buffer_2[0u].z;
				float fragment_unnamed_179 = rsqrt(dot(float3(fragment_unnamed_173, fragment_unnamed_174, fragment_unnamed_175), float3(fragment_unnamed_173, fragment_unnamed_174, fragment_unnamed_175)));
				float fragment_unnamed_180 = mad(fragment_unnamed_173, fragment_unnamed_179, fragment_unnamed_147);
				float fragment_unnamed_181 = mad(fragment_unnamed_174, fragment_unnamed_179, fragment_unnamed_148);
				float fragment_unnamed_182 = mad(fragment_unnamed_175, fragment_unnamed_179, fragment_unnamed_149);
				precise float fragment_unnamed_183 = fragment_unnamed_179 * fragment_unnamed_173;
				precise float fragment_unnamed_184 = fragment_unnamed_179 * fragment_unnamed_174;
				precise float fragment_unnamed_185 = fragment_unnamed_179 * fragment_unnamed_175;
				float fragment_unnamed_191 = rsqrt(max(dot(float3(fragment_unnamed_180, fragment_unnamed_181, fragment_unnamed_182), float3(fragment_unnamed_180, fragment_unnamed_181, fragment_unnamed_182)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_192 = fragment_unnamed_180 * fragment_unnamed_191;
				precise float fragment_unnamed_193 = fragment_unnamed_181 * fragment_unnamed_191;
				precise float fragment_unnamed_194 = fragment_unnamed_182 * fragment_unnamed_191;
				float fragment_unnamed_198 = clamp(dot(float3(fragment_unnamed_183, fragment_unnamed_184, fragment_unnamed_185), float3(fragment_unnamed_192, fragment_unnamed_193, fragment_unnamed_194)), 0.0f, 1.0f);
				float fragment_unnamed_202 = clamp(dot(float3(fragment_unnamed_121, fragment_unnamed_122, fragment_unnamed_123), float3(fragment_unnamed_183, fragment_unnamed_184, fragment_unnamed_185)), 0.0f, 1.0f);
				precise float fragment_unnamed_206 = (-0.0f) - fragment_unnamed_198;
				precise float fragment_unnamed_207 = fragment_unnamed_206 + 1.0f;
				precise float fragment_unnamed_208 = dot(fragment_unnamed_198.xx, fragment_unnamed_198.xx) + (-0.5f);
				precise float fragment_unnamed_211 = (-0.0f) - fragment_unnamed_202;
				precise float fragment_unnamed_212 = fragment_unnamed_211 + 1.0f;
				precise float fragment_unnamed_213 = fragment_unnamed_212 * fragment_unnamed_212;
				precise float fragment_unnamed_214 = fragment_unnamed_213 * fragment_unnamed_213;
				precise float fragment_unnamed_215 = fragment_unnamed_212 * fragment_unnamed_214;
				precise float fragment_unnamed_217 = mad(fragment_unnamed_208, fragment_unnamed_158, 1.0f) * mad(fragment_unnamed_208, fragment_unnamed_215, 1.0f);
				precise float fragment_unnamed_218 = fragment_unnamed_202 * fragment_unnamed_217;
				precise float fragment_unnamed_227 = fragment_input_5.y * fragment_uniform_buffer_0[5u].x;
				precise float fragment_unnamed_228 = fragment_input_5.y * fragment_uniform_buffer_0[5u].y;
				precise float fragment_unnamed_229 = fragment_input_5.y * fragment_uniform_buffer_0[5u].z;
				precise float fragment_unnamed_256 = mad(fragment_uniform_buffer_0[6u].x, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].x, fragment_input_5.x, fragment_unnamed_227)) + fragment_uniform_buffer_0[7u].x;
				precise float fragment_unnamed_257 = mad(fragment_uniform_buffer_0[6u].y, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].y, fragment_input_5.x, fragment_unnamed_228)) + fragment_uniform_buffer_0[7u].y;
				precise float fragment_unnamed_258 = mad(fragment_uniform_buffer_0[6u].z, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].z, fragment_input_5.x, fragment_unnamed_229)) + fragment_uniform_buffer_0[7u].z;
				float4 fragment_unnamed_263 = _LightTexture0.Sample(sampler_LightTexture0, dot(float3(fragment_unnamed_256, fragment_unnamed_257, fragment_unnamed_258), float3(fragment_unnamed_256, fragment_unnamed_257, fragment_unnamed_258)).xx);
				float fragment_unnamed_265 = fragment_unnamed_263.x;
				precise float fragment_unnamed_271 = fragment_unnamed_265 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_272 = fragment_unnamed_265 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_273 = fragment_unnamed_265 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_274 = fragment_unnamed_218 * fragment_unnamed_271;
				precise float fragment_unnamed_275 = fragment_unnamed_218 * fragment_unnamed_272;
				precise float fragment_unnamed_276 = fragment_unnamed_218 * fragment_unnamed_273;
				precise float fragment_unnamed_278 = abs(fragment_unnamed_150) + fragment_unnamed_202;
				precise float fragment_unnamed_279 = fragment_unnamed_278 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_281 = 0.5f / fragment_unnamed_279;
				precise float fragment_unnamed_283 = fragment_unnamed_281 * 0.99999988079071044921875f;
				precise float fragment_unnamed_285 = fragment_unnamed_202 * fragment_unnamed_283;
				precise float fragment_unnamed_286 = fragment_unnamed_271 * fragment_unnamed_285;
				precise float fragment_unnamed_287 = fragment_unnamed_272 * fragment_unnamed_285;
				precise float fragment_unnamed_288 = fragment_unnamed_273 * fragment_unnamed_285;
				precise float fragment_unnamed_289 = fragment_unnamed_207 * fragment_unnamed_207;
				precise float fragment_unnamed_290 = fragment_unnamed_289 * fragment_unnamed_289;
				precise float fragment_unnamed_291 = fragment_unnamed_207 * fragment_unnamed_290;
				float fragment_unnamed_292 = mad(fragment_unnamed_291, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_295 = fragment_unnamed_292 * fragment_unnamed_286;
				precise float fragment_unnamed_296 = fragment_unnamed_292 * fragment_unnamed_287;
				precise float fragment_unnamed_297 = fragment_unnamed_292 * fragment_unnamed_288;
				float4 fragment_unnamed_303 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_316 = fragment_unnamed_303.x * fragment_uniform_buffer_0[8u].x;
				precise float fragment_unnamed_317 = fragment_unnamed_303.y * fragment_uniform_buffer_0[8u].y;
				precise float fragment_unnamed_318 = fragment_unnamed_303.z * fragment_uniform_buffer_0[8u].z;
				precise float fragment_unnamed_319 = fragment_unnamed_303.w * fragment_uniform_buffer_0[8u].w;
				precise float fragment_unnamed_326 = fragment_unnamed_316 * fragment_input_6.x;
				precise float fragment_unnamed_327 = fragment_unnamed_317 * fragment_input_6.y;
				precise float fragment_unnamed_328 = fragment_unnamed_318 * fragment_input_6.z;
				precise float fragment_unnamed_332 = fragment_unnamed_319 * fragment_input_6.w;
				fragment_output_0.w = fragment_unnamed_332;
				precise float fragment_unnamed_335 = fragment_unnamed_326 * 0.959999978542327880859375f;
				precise float fragment_unnamed_336 = fragment_unnamed_327 * 0.959999978542327880859375f;
				precise float fragment_unnamed_337 = fragment_unnamed_328 * 0.959999978542327880859375f;
				precise float fragment_unnamed_345 = fragment_input_1 / fragment_uniform_buffer_1[5u].y;
				precise float fragment_unnamed_346 = (-0.0f) - fragment_unnamed_345;
				precise float fragment_unnamed_347 = fragment_unnamed_346 + 1.0f;
				precise float fragment_unnamed_351 = fragment_unnamed_347 * fragment_uniform_buffer_1[5u].z;
				float fragment_unnamed_360 = clamp(mad(max(fragment_unnamed_351, 0.0f), fragment_uniform_buffer_3[1u].z, fragment_uniform_buffer_3[1u].w), 0.0f, 1.0f);
				precise float fragment_unnamed_361 = mad(fragment_unnamed_335, fragment_unnamed_274, fragment_unnamed_295) * fragment_unnamed_360;
				precise float fragment_unnamed_362 = mad(fragment_unnamed_336, fragment_unnamed_275, fragment_unnamed_296) * fragment_unnamed_360;
				precise float fragment_unnamed_363 = mad(fragment_unnamed_337, fragment_unnamed_276, fragment_unnamed_297) * fragment_unnamed_360;
				fragment_output_0.x = fragment_unnamed_361;
				fragment_output_0.y = fragment_unnamed_362;
				fragment_output_0.z = fragment_unnamed_363;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				fragment_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				fragment_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				fragment_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_uniform_buffer_0[8] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_1[5] = float4(_ProjectionParams[0], _ProjectionParams[1], _ProjectionParams[2], _ProjectionParams[3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_uniform_buffer_3[1] = float4(unity_FogParams[0], unity_FogParams[1], unity_FogParams[2], unity_FogParams[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				fragment_input_8 = stage_input.fragment_input_8;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FOG_LINEAR
			#endif // POINT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogParams;

			static float4 fragment_uniform_buffer_0[5];
			static float4 fragment_uniform_buffer_1[6];
			static float4 fragment_uniform_buffer_2[1];
			static float4 fragment_uniform_buffer_3[2];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float fragment_input_1 : TEXCOORD7; // TEXCOORD_7
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float3 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float3 fragment_input_5 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_6 : COLOR; // COLOR
				float4 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float4 fragment_unnamed_65 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_70 = fragment_unnamed_65.w * fragment_unnamed_65.x;
				float fragment_unnamed_72 = mad(fragment_unnamed_70, 2.0f, -1.0f);
				float fragment_unnamed_75 = mad(fragment_unnamed_65.y, 2.0f, -1.0f);
				precise float fragment_unnamed_81 = (-0.0f) - min(dot(float2(fragment_unnamed_72, fragment_unnamed_75), float2(fragment_unnamed_72, fragment_unnamed_75)), 1.0f);
				precise float fragment_unnamed_83 = fragment_unnamed_81 + 1.0f;
				float fragment_unnamed_84 = sqrt(fragment_unnamed_83);
				float fragment_unnamed_91 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_72, fragment_unnamed_75, fragment_unnamed_84));
				float fragment_unnamed_100 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_72, fragment_unnamed_75, fragment_unnamed_84));
				float fragment_unnamed_109 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_72, fragment_unnamed_75, fragment_unnamed_84));
				float fragment_unnamed_115 = rsqrt(dot(float3(fragment_unnamed_91, fragment_unnamed_100, fragment_unnamed_109), float3(fragment_unnamed_91, fragment_unnamed_100, fragment_unnamed_109)));
				precise float fragment_unnamed_116 = fragment_unnamed_115 * fragment_unnamed_91;
				precise float fragment_unnamed_117 = fragment_unnamed_115 * fragment_unnamed_100;
				precise float fragment_unnamed_118 = fragment_unnamed_115 * fragment_unnamed_109;
				precise float fragment_unnamed_121 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_124 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_127 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_135 = fragment_unnamed_121 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_136 = fragment_unnamed_124 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_137 = fragment_unnamed_127 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_141 = rsqrt(dot(float3(fragment_unnamed_135, fragment_unnamed_136, fragment_unnamed_137), float3(fragment_unnamed_135, fragment_unnamed_136, fragment_unnamed_137)));
				precise float fragment_unnamed_142 = fragment_unnamed_141 * fragment_unnamed_135;
				precise float fragment_unnamed_143 = fragment_unnamed_141 * fragment_unnamed_136;
				precise float fragment_unnamed_144 = fragment_unnamed_141 * fragment_unnamed_137;
				float fragment_unnamed_150 = mad(fragment_unnamed_135, fragment_unnamed_141, fragment_uniform_buffer_2[0u].x);
				float fragment_unnamed_151 = mad(fragment_unnamed_136, fragment_unnamed_141, fragment_uniform_buffer_2[0u].y);
				float fragment_unnamed_152 = mad(fragment_unnamed_137, fragment_unnamed_141, fragment_uniform_buffer_2[0u].z);
				float fragment_unnamed_153 = dot(float3(fragment_unnamed_116, fragment_unnamed_117, fragment_unnamed_118), float3(fragment_unnamed_142, fragment_unnamed_143, fragment_unnamed_144));
				float fragment_unnamed_164 = clamp(dot(float3(fragment_unnamed_116, fragment_unnamed_117, fragment_unnamed_118), float3(fragment_uniform_buffer_2[0u].xyz)), 0.0f, 1.0f);
				precise float fragment_unnamed_166 = (-0.0f) - abs(fragment_unnamed_153);
				precise float fragment_unnamed_167 = fragment_unnamed_166 + 1.0f;
				precise float fragment_unnamed_169 = abs(fragment_unnamed_153) + fragment_unnamed_164;
				precise float fragment_unnamed_170 = fragment_unnamed_169 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_172 = 0.5f / fragment_unnamed_170;
				precise float fragment_unnamed_174 = fragment_unnamed_172 * 0.99999988079071044921875f;
				precise float fragment_unnamed_176 = fragment_unnamed_164 * fragment_unnamed_174;
				precise float fragment_unnamed_182 = fragment_unnamed_176 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_183 = fragment_unnamed_176 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_184 = fragment_unnamed_176 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_185 = fragment_unnamed_167 * fragment_unnamed_167;
				precise float fragment_unnamed_186 = fragment_unnamed_185 * fragment_unnamed_185;
				precise float fragment_unnamed_187 = fragment_unnamed_167 * fragment_unnamed_186;
				float fragment_unnamed_193 = rsqrt(max(dot(float3(fragment_unnamed_150, fragment_unnamed_151, fragment_unnamed_152), float3(fragment_unnamed_150, fragment_unnamed_151, fragment_unnamed_152)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_194 = fragment_unnamed_193 * fragment_unnamed_150;
				precise float fragment_unnamed_195 = fragment_unnamed_193 * fragment_unnamed_151;
				precise float fragment_unnamed_196 = fragment_unnamed_193 * fragment_unnamed_152;
				float fragment_unnamed_205 = clamp(dot(float3(fragment_uniform_buffer_2[0u].xyz), float3(fragment_unnamed_194, fragment_unnamed_195, fragment_unnamed_196)), 0.0f, 1.0f);
				precise float fragment_unnamed_209 = (-0.0f) - fragment_unnamed_205;
				precise float fragment_unnamed_210 = fragment_unnamed_209 + 1.0f;
				precise float fragment_unnamed_211 = dot(fragment_unnamed_205.xx, fragment_unnamed_205.xx) + (-0.5f);
				precise float fragment_unnamed_214 = (-0.0f) - fragment_unnamed_164;
				precise float fragment_unnamed_215 = fragment_unnamed_214 + 1.0f;
				precise float fragment_unnamed_216 = fragment_unnamed_215 * fragment_unnamed_215;
				precise float fragment_unnamed_217 = fragment_unnamed_216 * fragment_unnamed_216;
				precise float fragment_unnamed_218 = fragment_unnamed_215 * fragment_unnamed_217;
				precise float fragment_unnamed_220 = mad(fragment_unnamed_211, fragment_unnamed_187, 1.0f) * mad(fragment_unnamed_211, fragment_unnamed_218, 1.0f);
				precise float fragment_unnamed_221 = fragment_unnamed_164 * fragment_unnamed_220;
				precise float fragment_unnamed_227 = fragment_unnamed_221 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_228 = fragment_unnamed_221 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_229 = fragment_unnamed_221 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_230 = fragment_unnamed_210 * fragment_unnamed_210;
				precise float fragment_unnamed_231 = fragment_unnamed_230 * fragment_unnamed_230;
				precise float fragment_unnamed_232 = fragment_unnamed_210 * fragment_unnamed_231;
				float fragment_unnamed_233 = mad(fragment_unnamed_232, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_236 = fragment_unnamed_233 * fragment_unnamed_182;
				precise float fragment_unnamed_237 = fragment_unnamed_233 * fragment_unnamed_183;
				precise float fragment_unnamed_238 = fragment_unnamed_233 * fragment_unnamed_184;
				float4 fragment_unnamed_244 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_256 = fragment_unnamed_244.x * fragment_uniform_buffer_0[4u].x;
				precise float fragment_unnamed_257 = fragment_unnamed_244.y * fragment_uniform_buffer_0[4u].y;
				precise float fragment_unnamed_258 = fragment_unnamed_244.z * fragment_uniform_buffer_0[4u].z;
				precise float fragment_unnamed_259 = fragment_unnamed_244.w * fragment_uniform_buffer_0[4u].w;
				precise float fragment_unnamed_266 = fragment_unnamed_256 * fragment_input_6.x;
				precise float fragment_unnamed_267 = fragment_unnamed_257 * fragment_input_6.y;
				precise float fragment_unnamed_268 = fragment_unnamed_258 * fragment_input_6.z;
				precise float fragment_unnamed_272 = fragment_unnamed_259 * fragment_input_6.w;
				fragment_output_0.w = fragment_unnamed_272;
				precise float fragment_unnamed_275 = fragment_unnamed_266 * 0.959999978542327880859375f;
				precise float fragment_unnamed_276 = fragment_unnamed_267 * 0.959999978542327880859375f;
				precise float fragment_unnamed_277 = fragment_unnamed_268 * 0.959999978542327880859375f;
				precise float fragment_unnamed_285 = fragment_input_1 / fragment_uniform_buffer_1[5u].y;
				precise float fragment_unnamed_286 = (-0.0f) - fragment_unnamed_285;
				precise float fragment_unnamed_287 = fragment_unnamed_286 + 1.0f;
				precise float fragment_unnamed_291 = fragment_unnamed_287 * fragment_uniform_buffer_1[5u].z;
				float fragment_unnamed_300 = clamp(mad(max(fragment_unnamed_291, 0.0f), fragment_uniform_buffer_3[1u].z, fragment_uniform_buffer_3[1u].w), 0.0f, 1.0f);
				precise float fragment_unnamed_301 = mad(fragment_unnamed_275, fragment_unnamed_227, fragment_unnamed_236) * fragment_unnamed_300;
				precise float fragment_unnamed_302 = mad(fragment_unnamed_276, fragment_unnamed_228, fragment_unnamed_237) * fragment_unnamed_300;
				precise float fragment_unnamed_303 = mad(fragment_unnamed_277, fragment_unnamed_229, fragment_unnamed_238) * fragment_unnamed_300;
				fragment_output_0.x = fragment_unnamed_301;
				fragment_output_0.y = fragment_unnamed_302;
				fragment_output_0.z = fragment_unnamed_303;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_1[5] = float4(_ProjectionParams[0], _ProjectionParams[1], _ProjectionParams[2], _ProjectionParams[3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_uniform_buffer_3[1] = float4(unity_FogParams[0], unity_FogParams[1], unity_FogParams[2], unity_FogParams[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			#ifdef FOG_LINEAR
			#ifdef SPOT
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT
			#ifndef POINT_COOKIE
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogParams;

			static float4 fragment_uniform_buffer_0[9];
			static float4 fragment_uniform_buffer_1[6];
			static float4 fragment_uniform_buffer_2[1];
			static float4 fragment_uniform_buffer_3[2];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			Texture2D<float4> _LightTexture0;
			Texture2D<float4> _LightTextureB0;
			SamplerState sampler_LightTexture0;
			SamplerState sampler_LightTextureB0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float4 fragment_input_8;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float fragment_input_1 : TEXCOORD7; // TEXCOORD_7
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float3 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float3 fragment_input_5 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_6 : COLOR; // COLOR
				float4 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
				float4 fragment_input_8 : TEXCOORD6; // TEXCOORD_6
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float4 fragment_unnamed_74 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_79 = fragment_unnamed_74.w * fragment_unnamed_74.x;
				float fragment_unnamed_81 = mad(fragment_unnamed_79, 2.0f, -1.0f);
				float fragment_unnamed_84 = mad(fragment_unnamed_74.y, 2.0f, -1.0f);
				precise float fragment_unnamed_90 = (-0.0f) - min(dot(float2(fragment_unnamed_81, fragment_unnamed_84), float2(fragment_unnamed_81, fragment_unnamed_84)), 1.0f);
				precise float fragment_unnamed_92 = fragment_unnamed_90 + 1.0f;
				float fragment_unnamed_93 = sqrt(fragment_unnamed_92);
				float fragment_unnamed_100 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_81, fragment_unnamed_84, fragment_unnamed_93));
				float fragment_unnamed_109 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_81, fragment_unnamed_84, fragment_unnamed_93));
				float fragment_unnamed_118 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_81, fragment_unnamed_84, fragment_unnamed_93));
				float fragment_unnamed_124 = rsqrt(dot(float3(fragment_unnamed_100, fragment_unnamed_109, fragment_unnamed_118), float3(fragment_unnamed_100, fragment_unnamed_109, fragment_unnamed_118)));
				precise float fragment_unnamed_125 = fragment_unnamed_124 * fragment_unnamed_100;
				precise float fragment_unnamed_126 = fragment_unnamed_124 * fragment_unnamed_109;
				precise float fragment_unnamed_127 = fragment_unnamed_124 * fragment_unnamed_118;
				precise float fragment_unnamed_130 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_133 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_136 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_144 = fragment_unnamed_130 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_145 = fragment_unnamed_133 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_146 = fragment_unnamed_136 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_150 = rsqrt(dot(float3(fragment_unnamed_144, fragment_unnamed_145, fragment_unnamed_146), float3(fragment_unnamed_144, fragment_unnamed_145, fragment_unnamed_146)));
				precise float fragment_unnamed_151 = fragment_unnamed_150 * fragment_unnamed_144;
				precise float fragment_unnamed_152 = fragment_unnamed_150 * fragment_unnamed_145;
				precise float fragment_unnamed_153 = fragment_unnamed_150 * fragment_unnamed_146;
				float fragment_unnamed_154 = dot(float3(fragment_unnamed_125, fragment_unnamed_126, fragment_unnamed_127), float3(fragment_unnamed_151, fragment_unnamed_152, fragment_unnamed_153));
				precise float fragment_unnamed_158 = (-0.0f) - abs(fragment_unnamed_154);
				precise float fragment_unnamed_159 = fragment_unnamed_158 + 1.0f;
				precise float fragment_unnamed_160 = fragment_unnamed_159 * fragment_unnamed_159;
				precise float fragment_unnamed_161 = fragment_unnamed_160 * fragment_unnamed_160;
				precise float fragment_unnamed_162 = fragment_unnamed_159 * fragment_unnamed_161;
				precise float fragment_unnamed_165 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_168 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_171 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_177 = fragment_unnamed_165 + fragment_uniform_buffer_2[0u].x;
				precise float fragment_unnamed_178 = fragment_unnamed_168 + fragment_uniform_buffer_2[0u].y;
				precise float fragment_unnamed_179 = fragment_unnamed_171 + fragment_uniform_buffer_2[0u].z;
				float fragment_unnamed_183 = rsqrt(dot(float3(fragment_unnamed_177, fragment_unnamed_178, fragment_unnamed_179), float3(fragment_unnamed_177, fragment_unnamed_178, fragment_unnamed_179)));
				float fragment_unnamed_184 = mad(fragment_unnamed_177, fragment_unnamed_183, fragment_unnamed_151);
				float fragment_unnamed_185 = mad(fragment_unnamed_178, fragment_unnamed_183, fragment_unnamed_152);
				float fragment_unnamed_186 = mad(fragment_unnamed_179, fragment_unnamed_183, fragment_unnamed_153);
				precise float fragment_unnamed_187 = fragment_unnamed_183 * fragment_unnamed_177;
				precise float fragment_unnamed_188 = fragment_unnamed_183 * fragment_unnamed_178;
				precise float fragment_unnamed_189 = fragment_unnamed_183 * fragment_unnamed_179;
				float fragment_unnamed_195 = rsqrt(max(dot(float3(fragment_unnamed_184, fragment_unnamed_185, fragment_unnamed_186), float3(fragment_unnamed_184, fragment_unnamed_185, fragment_unnamed_186)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_196 = fragment_unnamed_184 * fragment_unnamed_195;
				precise float fragment_unnamed_197 = fragment_unnamed_185 * fragment_unnamed_195;
				precise float fragment_unnamed_198 = fragment_unnamed_186 * fragment_unnamed_195;
				float fragment_unnamed_202 = clamp(dot(float3(fragment_unnamed_187, fragment_unnamed_188, fragment_unnamed_189), float3(fragment_unnamed_196, fragment_unnamed_197, fragment_unnamed_198)), 0.0f, 1.0f);
				float fragment_unnamed_206 = clamp(dot(float3(fragment_unnamed_125, fragment_unnamed_126, fragment_unnamed_127), float3(fragment_unnamed_187, fragment_unnamed_188, fragment_unnamed_189)), 0.0f, 1.0f);
				precise float fragment_unnamed_210 = (-0.0f) - fragment_unnamed_202;
				precise float fragment_unnamed_211 = fragment_unnamed_210 + 1.0f;
				precise float fragment_unnamed_212 = dot(fragment_unnamed_202.xx, fragment_unnamed_202.xx) + (-0.5f);
				precise float fragment_unnamed_215 = (-0.0f) - fragment_unnamed_206;
				precise float fragment_unnamed_216 = fragment_unnamed_215 + 1.0f;
				precise float fragment_unnamed_217 = fragment_unnamed_216 * fragment_unnamed_216;
				precise float fragment_unnamed_218 = fragment_unnamed_217 * fragment_unnamed_217;
				precise float fragment_unnamed_219 = fragment_unnamed_216 * fragment_unnamed_218;
				precise float fragment_unnamed_221 = mad(fragment_unnamed_212, fragment_unnamed_162, 1.0f) * mad(fragment_unnamed_212, fragment_unnamed_219, 1.0f);
				precise float fragment_unnamed_222 = fragment_unnamed_206 * fragment_unnamed_221;
				precise float fragment_unnamed_232 = fragment_input_5.y * fragment_uniform_buffer_0[5u].x;
				precise float fragment_unnamed_233 = fragment_input_5.y * fragment_uniform_buffer_0[5u].y;
				precise float fragment_unnamed_234 = fragment_input_5.y * fragment_uniform_buffer_0[5u].z;
				precise float fragment_unnamed_235 = fragment_input_5.y * fragment_uniform_buffer_0[5u].w;
				precise float fragment_unnamed_267 = mad(fragment_uniform_buffer_0[6u].x, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].x, fragment_input_5.x, fragment_unnamed_232)) + fragment_uniform_buffer_0[7u].x;
				precise float fragment_unnamed_268 = mad(fragment_uniform_buffer_0[6u].y, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].y, fragment_input_5.x, fragment_unnamed_233)) + fragment_uniform_buffer_0[7u].y;
				precise float fragment_unnamed_269 = mad(fragment_uniform_buffer_0[6u].z, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].z, fragment_input_5.x, fragment_unnamed_234)) + fragment_uniform_buffer_0[7u].z;
				precise float fragment_unnamed_270 = mad(fragment_uniform_buffer_0[6u].w, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].w, fragment_input_5.x, fragment_unnamed_235)) + fragment_uniform_buffer_0[7u].w;
				precise float fragment_unnamed_271 = fragment_unnamed_267 / fragment_unnamed_270;
				precise float fragment_unnamed_272 = fragment_unnamed_268 / fragment_unnamed_270;
				precise float fragment_unnamed_273 = fragment_unnamed_271 + 0.5f;
				precise float fragment_unnamed_275 = fragment_unnamed_272 + 0.5f;
				precise float fragment_unnamed_294 = _LightTexture0.Sample(sampler_LightTexture0, float2(fragment_unnamed_273, fragment_unnamed_275)).w * asfloat(((0.0f < fragment_unnamed_269) ? 4294967295u : 0u) & 1065353216u);
				precise float fragment_unnamed_295 = _LightTextureB0.Sample(sampler_LightTextureB0, dot(float3(fragment_unnamed_267, fragment_unnamed_268, fragment_unnamed_269), float3(fragment_unnamed_267, fragment_unnamed_268, fragment_unnamed_269)).xx).x * fragment_unnamed_294;
				precise float fragment_unnamed_301 = fragment_unnamed_295 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_302 = fragment_unnamed_295 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_303 = fragment_unnamed_295 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_304 = fragment_unnamed_222 * fragment_unnamed_301;
				precise float fragment_unnamed_305 = fragment_unnamed_222 * fragment_unnamed_302;
				precise float fragment_unnamed_306 = fragment_unnamed_222 * fragment_unnamed_303;
				precise float fragment_unnamed_308 = abs(fragment_unnamed_154) + fragment_unnamed_206;
				precise float fragment_unnamed_309 = fragment_unnamed_308 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_311 = 0.5f / fragment_unnamed_309;
				precise float fragment_unnamed_312 = fragment_unnamed_311 * 0.99999988079071044921875f;
				precise float fragment_unnamed_314 = fragment_unnamed_206 * fragment_unnamed_312;
				precise float fragment_unnamed_315 = fragment_unnamed_301 * fragment_unnamed_314;
				precise float fragment_unnamed_316 = fragment_unnamed_302 * fragment_unnamed_314;
				precise float fragment_unnamed_317 = fragment_unnamed_303 * fragment_unnamed_314;
				precise float fragment_unnamed_318 = fragment_unnamed_211 * fragment_unnamed_211;
				precise float fragment_unnamed_319 = fragment_unnamed_318 * fragment_unnamed_318;
				precise float fragment_unnamed_320 = fragment_unnamed_211 * fragment_unnamed_319;
				float fragment_unnamed_321 = mad(fragment_unnamed_320, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_324 = fragment_unnamed_321 * fragment_unnamed_315;
				precise float fragment_unnamed_325 = fragment_unnamed_321 * fragment_unnamed_316;
				precise float fragment_unnamed_326 = fragment_unnamed_321 * fragment_unnamed_317;
				float4 fragment_unnamed_332 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_345 = fragment_unnamed_332.x * fragment_uniform_buffer_0[8u].x;
				precise float fragment_unnamed_346 = fragment_unnamed_332.y * fragment_uniform_buffer_0[8u].y;
				precise float fragment_unnamed_347 = fragment_unnamed_332.z * fragment_uniform_buffer_0[8u].z;
				precise float fragment_unnamed_348 = fragment_unnamed_332.w * fragment_uniform_buffer_0[8u].w;
				precise float fragment_unnamed_355 = fragment_unnamed_345 * fragment_input_6.x;
				precise float fragment_unnamed_356 = fragment_unnamed_346 * fragment_input_6.y;
				precise float fragment_unnamed_357 = fragment_unnamed_347 * fragment_input_6.z;
				precise float fragment_unnamed_361 = fragment_unnamed_348 * fragment_input_6.w;
				fragment_output_0.w = fragment_unnamed_361;
				precise float fragment_unnamed_364 = fragment_unnamed_355 * 0.959999978542327880859375f;
				precise float fragment_unnamed_365 = fragment_unnamed_356 * 0.959999978542327880859375f;
				precise float fragment_unnamed_366 = fragment_unnamed_357 * 0.959999978542327880859375f;
				precise float fragment_unnamed_374 = fragment_input_1 / fragment_uniform_buffer_1[5u].y;
				precise float fragment_unnamed_375 = (-0.0f) - fragment_unnamed_374;
				precise float fragment_unnamed_376 = fragment_unnamed_375 + 1.0f;
				precise float fragment_unnamed_380 = fragment_unnamed_376 * fragment_uniform_buffer_1[5u].z;
				float fragment_unnamed_389 = clamp(mad(max(fragment_unnamed_380, 0.0f), fragment_uniform_buffer_3[1u].z, fragment_uniform_buffer_3[1u].w), 0.0f, 1.0f);
				precise float fragment_unnamed_390 = mad(fragment_unnamed_364, fragment_unnamed_304, fragment_unnamed_324) * fragment_unnamed_389;
				precise float fragment_unnamed_391 = mad(fragment_unnamed_365, fragment_unnamed_305, fragment_unnamed_325) * fragment_unnamed_389;
				precise float fragment_unnamed_392 = mad(fragment_unnamed_366, fragment_unnamed_306, fragment_unnamed_326) * fragment_unnamed_389;
				fragment_output_0.x = fragment_unnamed_390;
				fragment_output_0.y = fragment_unnamed_391;
				fragment_output_0.z = fragment_unnamed_392;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				fragment_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				fragment_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				fragment_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_uniform_buffer_0[8] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_1[5] = float4(_ProjectionParams[0], _ProjectionParams[1], _ProjectionParams[2], _ProjectionParams[3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_uniform_buffer_3[1] = float4(unity_FogParams[0], unity_FogParams[1], unity_FogParams[2], unity_FogParams[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				fragment_input_8 = stage_input.fragment_input_8;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FOG_LINEAR
			#endif // SPOT
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT
			#endif // !POINT_COOKIE


			#ifdef FOG_LINEAR
			#ifdef POINT_COOKIE
			#ifndef DIRECTIONAL
			#ifndef DIRECTIONAL_COOKIE
			#ifndef POINT
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogParams;

			static float4 fragment_uniform_buffer_0[9];
			static float4 fragment_uniform_buffer_1[6];
			static float4 fragment_uniform_buffer_2[1];
			static float4 fragment_uniform_buffer_3[2];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			Texture2D<float4> _LightTextureB0;
			TextureCube<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			SamplerState sampler_LightTextureB0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float fragment_input_1;
			static float3 fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float3 fragment_input_8;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float fragment_input_1 : TEXCOORD7; // TEXCOORD_7
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float3 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float3 fragment_input_5 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_6 : COLOR; // COLOR
				float4 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
				float3 fragment_input_8 : TEXCOORD6; // TEXCOORD_6
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float4 fragment_unnamed_76 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_81 = fragment_unnamed_76.w * fragment_unnamed_76.x;
				float fragment_unnamed_83 = mad(fragment_unnamed_81, 2.0f, -1.0f);
				float fragment_unnamed_86 = mad(fragment_unnamed_76.y, 2.0f, -1.0f);
				precise float fragment_unnamed_92 = (-0.0f) - min(dot(float2(fragment_unnamed_83, fragment_unnamed_86), float2(fragment_unnamed_83, fragment_unnamed_86)), 1.0f);
				precise float fragment_unnamed_94 = fragment_unnamed_92 + 1.0f;
				float fragment_unnamed_95 = sqrt(fragment_unnamed_94);
				float fragment_unnamed_102 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_83, fragment_unnamed_86, fragment_unnamed_95));
				float fragment_unnamed_111 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_83, fragment_unnamed_86, fragment_unnamed_95));
				float fragment_unnamed_120 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_83, fragment_unnamed_86, fragment_unnamed_95));
				float fragment_unnamed_126 = rsqrt(dot(float3(fragment_unnamed_102, fragment_unnamed_111, fragment_unnamed_120), float3(fragment_unnamed_102, fragment_unnamed_111, fragment_unnamed_120)));
				precise float fragment_unnamed_127 = fragment_unnamed_126 * fragment_unnamed_102;
				precise float fragment_unnamed_128 = fragment_unnamed_126 * fragment_unnamed_111;
				precise float fragment_unnamed_129 = fragment_unnamed_126 * fragment_unnamed_120;
				precise float fragment_unnamed_132 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_135 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_138 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_146 = fragment_unnamed_132 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_147 = fragment_unnamed_135 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_148 = fragment_unnamed_138 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_152 = rsqrt(dot(float3(fragment_unnamed_146, fragment_unnamed_147, fragment_unnamed_148), float3(fragment_unnamed_146, fragment_unnamed_147, fragment_unnamed_148)));
				precise float fragment_unnamed_153 = fragment_unnamed_152 * fragment_unnamed_146;
				precise float fragment_unnamed_154 = fragment_unnamed_152 * fragment_unnamed_147;
				precise float fragment_unnamed_155 = fragment_unnamed_152 * fragment_unnamed_148;
				float fragment_unnamed_156 = dot(float3(fragment_unnamed_127, fragment_unnamed_128, fragment_unnamed_129), float3(fragment_unnamed_153, fragment_unnamed_154, fragment_unnamed_155));
				precise float fragment_unnamed_160 = (-0.0f) - abs(fragment_unnamed_156);
				precise float fragment_unnamed_161 = fragment_unnamed_160 + 1.0f;
				precise float fragment_unnamed_162 = fragment_unnamed_161 * fragment_unnamed_161;
				precise float fragment_unnamed_163 = fragment_unnamed_162 * fragment_unnamed_162;
				precise float fragment_unnamed_164 = fragment_unnamed_161 * fragment_unnamed_163;
				precise float fragment_unnamed_167 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_170 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_173 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_179 = fragment_unnamed_167 + fragment_uniform_buffer_2[0u].x;
				precise float fragment_unnamed_180 = fragment_unnamed_170 + fragment_uniform_buffer_2[0u].y;
				precise float fragment_unnamed_181 = fragment_unnamed_173 + fragment_uniform_buffer_2[0u].z;
				float fragment_unnamed_185 = rsqrt(dot(float3(fragment_unnamed_179, fragment_unnamed_180, fragment_unnamed_181), float3(fragment_unnamed_179, fragment_unnamed_180, fragment_unnamed_181)));
				float fragment_unnamed_186 = mad(fragment_unnamed_179, fragment_unnamed_185, fragment_unnamed_153);
				float fragment_unnamed_187 = mad(fragment_unnamed_180, fragment_unnamed_185, fragment_unnamed_154);
				float fragment_unnamed_188 = mad(fragment_unnamed_181, fragment_unnamed_185, fragment_unnamed_155);
				precise float fragment_unnamed_189 = fragment_unnamed_185 * fragment_unnamed_179;
				precise float fragment_unnamed_190 = fragment_unnamed_185 * fragment_unnamed_180;
				precise float fragment_unnamed_191 = fragment_unnamed_185 * fragment_unnamed_181;
				float fragment_unnamed_197 = rsqrt(max(dot(float3(fragment_unnamed_186, fragment_unnamed_187, fragment_unnamed_188), float3(fragment_unnamed_186, fragment_unnamed_187, fragment_unnamed_188)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_198 = fragment_unnamed_186 * fragment_unnamed_197;
				precise float fragment_unnamed_199 = fragment_unnamed_187 * fragment_unnamed_197;
				precise float fragment_unnamed_200 = fragment_unnamed_188 * fragment_unnamed_197;
				float fragment_unnamed_204 = clamp(dot(float3(fragment_unnamed_189, fragment_unnamed_190, fragment_unnamed_191), float3(fragment_unnamed_198, fragment_unnamed_199, fragment_unnamed_200)), 0.0f, 1.0f);
				float fragment_unnamed_208 = clamp(dot(float3(fragment_unnamed_127, fragment_unnamed_128, fragment_unnamed_129), float3(fragment_unnamed_189, fragment_unnamed_190, fragment_unnamed_191)), 0.0f, 1.0f);
				precise float fragment_unnamed_212 = (-0.0f) - fragment_unnamed_204;
				precise float fragment_unnamed_213 = fragment_unnamed_212 + 1.0f;
				precise float fragment_unnamed_214 = dot(fragment_unnamed_204.xx, fragment_unnamed_204.xx) + (-0.5f);
				precise float fragment_unnamed_217 = (-0.0f) - fragment_unnamed_208;
				precise float fragment_unnamed_218 = fragment_unnamed_217 + 1.0f;
				precise float fragment_unnamed_219 = fragment_unnamed_218 * fragment_unnamed_218;
				precise float fragment_unnamed_220 = fragment_unnamed_219 * fragment_unnamed_219;
				precise float fragment_unnamed_221 = fragment_unnamed_218 * fragment_unnamed_220;
				precise float fragment_unnamed_223 = mad(fragment_unnamed_214, fragment_unnamed_164, 1.0f) * mad(fragment_unnamed_214, fragment_unnamed_221, 1.0f);
				precise float fragment_unnamed_224 = fragment_unnamed_208 * fragment_unnamed_223;
				precise float fragment_unnamed_233 = fragment_input_5.y * fragment_uniform_buffer_0[5u].x;
				precise float fragment_unnamed_234 = fragment_input_5.y * fragment_uniform_buffer_0[5u].y;
				precise float fragment_unnamed_235 = fragment_input_5.y * fragment_uniform_buffer_0[5u].z;
				precise float fragment_unnamed_262 = mad(fragment_uniform_buffer_0[6u].x, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].x, fragment_input_5.x, fragment_unnamed_233)) + fragment_uniform_buffer_0[7u].x;
				precise float fragment_unnamed_263 = mad(fragment_uniform_buffer_0[6u].y, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].y, fragment_input_5.x, fragment_unnamed_234)) + fragment_uniform_buffer_0[7u].y;
				precise float fragment_unnamed_264 = mad(fragment_uniform_buffer_0[6u].z, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].z, fragment_input_5.x, fragment_unnamed_235)) + fragment_uniform_buffer_0[7u].z;
				precise float fragment_unnamed_277 = _LightTexture0.Sample(sampler_LightTexture0, float3(fragment_unnamed_262, fragment_unnamed_263, fragment_unnamed_264)).w * _LightTextureB0.Sample(sampler_LightTextureB0, dot(float3(fragment_unnamed_262, fragment_unnamed_263, fragment_unnamed_264), float3(fragment_unnamed_262, fragment_unnamed_263, fragment_unnamed_264)).xx).x;
				precise float fragment_unnamed_283 = fragment_unnamed_277 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_284 = fragment_unnamed_277 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_285 = fragment_unnamed_277 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_286 = fragment_unnamed_224 * fragment_unnamed_283;
				precise float fragment_unnamed_287 = fragment_unnamed_224 * fragment_unnamed_284;
				precise float fragment_unnamed_288 = fragment_unnamed_224 * fragment_unnamed_285;
				precise float fragment_unnamed_290 = abs(fragment_unnamed_156) + fragment_unnamed_208;
				precise float fragment_unnamed_291 = fragment_unnamed_290 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_293 = 0.5f / fragment_unnamed_291;
				precise float fragment_unnamed_295 = fragment_unnamed_293 * 0.99999988079071044921875f;
				precise float fragment_unnamed_297 = fragment_unnamed_208 * fragment_unnamed_295;
				precise float fragment_unnamed_298 = fragment_unnamed_283 * fragment_unnamed_297;
				precise float fragment_unnamed_299 = fragment_unnamed_284 * fragment_unnamed_297;
				precise float fragment_unnamed_300 = fragment_unnamed_285 * fragment_unnamed_297;
				precise float fragment_unnamed_301 = fragment_unnamed_213 * fragment_unnamed_213;
				precise float fragment_unnamed_302 = fragment_unnamed_301 * fragment_unnamed_301;
				precise float fragment_unnamed_303 = fragment_unnamed_213 * fragment_unnamed_302;
				float fragment_unnamed_304 = mad(fragment_unnamed_303, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_307 = fragment_unnamed_304 * fragment_unnamed_298;
				precise float fragment_unnamed_308 = fragment_unnamed_304 * fragment_unnamed_299;
				precise float fragment_unnamed_309 = fragment_unnamed_304 * fragment_unnamed_300;
				float4 fragment_unnamed_315 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_328 = fragment_unnamed_315.x * fragment_uniform_buffer_0[8u].x;
				precise float fragment_unnamed_329 = fragment_unnamed_315.y * fragment_uniform_buffer_0[8u].y;
				precise float fragment_unnamed_330 = fragment_unnamed_315.z * fragment_uniform_buffer_0[8u].z;
				precise float fragment_unnamed_331 = fragment_unnamed_315.w * fragment_uniform_buffer_0[8u].w;
				precise float fragment_unnamed_338 = fragment_unnamed_328 * fragment_input_6.x;
				precise float fragment_unnamed_339 = fragment_unnamed_329 * fragment_input_6.y;
				precise float fragment_unnamed_340 = fragment_unnamed_330 * fragment_input_6.z;
				precise float fragment_unnamed_344 = fragment_unnamed_331 * fragment_input_6.w;
				fragment_output_0.w = fragment_unnamed_344;
				precise float fragment_unnamed_347 = fragment_unnamed_338 * 0.959999978542327880859375f;
				precise float fragment_unnamed_348 = fragment_unnamed_339 * 0.959999978542327880859375f;
				precise float fragment_unnamed_349 = fragment_unnamed_340 * 0.959999978542327880859375f;
				precise float fragment_unnamed_357 = fragment_input_1 / fragment_uniform_buffer_1[5u].y;
				precise float fragment_unnamed_358 = (-0.0f) - fragment_unnamed_357;
				precise float fragment_unnamed_359 = fragment_unnamed_358 + 1.0f;
				precise float fragment_unnamed_363 = fragment_unnamed_359 * fragment_uniform_buffer_1[5u].z;
				float fragment_unnamed_372 = clamp(mad(max(fragment_unnamed_363, 0.0f), fragment_uniform_buffer_3[1u].z, fragment_uniform_buffer_3[1u].w), 0.0f, 1.0f);
				precise float fragment_unnamed_373 = mad(fragment_unnamed_347, fragment_unnamed_286, fragment_unnamed_307) * fragment_unnamed_372;
				precise float fragment_unnamed_374 = mad(fragment_unnamed_348, fragment_unnamed_287, fragment_unnamed_308) * fragment_unnamed_372;
				precise float fragment_unnamed_375 = mad(fragment_unnamed_349, fragment_unnamed_288, fragment_unnamed_309) * fragment_unnamed_372;
				fragment_output_0.x = fragment_unnamed_373;
				fragment_output_0.y = fragment_unnamed_374;
				fragment_output_0.z = fragment_unnamed_375;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				fragment_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				fragment_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				fragment_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_uniform_buffer_0[8] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_1[5] = float4(_ProjectionParams[0], _ProjectionParams[1], _ProjectionParams[2], _ProjectionParams[3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_uniform_buffer_3[1] = float4(unity_FogParams[0], unity_FogParams[1], unity_FogParams[2], unity_FogParams[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				fragment_input_8 = stage_input.fragment_input_8;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FOG_LINEAR
			#endif // POINT_COOKIE
			#endif // !DIRECTIONAL
			#endif // !DIRECTIONAL_COOKIE
			#endif // !POINT
			#endif // !SPOT


			#ifdef DIRECTIONAL_COOKIE
			#ifdef FOG_LINEAR
			#ifndef DIRECTIONAL
			#ifndef POINT
			#ifndef POINT_COOKIE
			#ifndef SPOT
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _LightColor0;
			float4x4 unity_WorldToLight;
			float4 _Color;
			float3 _WorldSpaceCameraPos;
			float4 _ProjectionParams;
			float4 _WorldSpaceLightPos0;
			float4 unity_FogParams;

			static float4 fragment_uniform_buffer_0[9];
			static float4 fragment_uniform_buffer_1[6];
			static float4 fragment_uniform_buffer_2[1];
			static float4 fragment_uniform_buffer_3[2];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _Normal;
			Texture2D<float4> _LightTexture0;
			SamplerState sampler_LightTexture0;
			SamplerState sampler_MainTex;
			SamplerState sampler_Normal;

			static float2 fragment_input_1;
			static float2 fragment_input_1;
			static float3 fragment_input_2;
			static float fragment_input_2;
			static float3 fragment_input_3;
			static float3 fragment_input_4;
			static float3 fragment_input_5;
			static float4 fragment_input_6;
			static float4 fragment_input_7;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float2 fragment_input_1 : TEXCOORD6; // TEXCOORD_6
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
				float fragment_input_2 : TEXCOORD7; // TEXCOORD_7
				float3 fragment_input_3 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_4 : TEXCOORD3; // TEXCOORD_3
				float3 fragment_input_5 : TEXCOORD4; // TEXCOORD_4
				float4 fragment_input_6 : COLOR; // COLOR
				float4 fragment_input_7 : TEXCOORD5; // TEXCOORD_5
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float4 fragment_unnamed_70 = _Normal.Sample(sampler_Normal, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_75 = fragment_unnamed_70.w * fragment_unnamed_70.x;
				float fragment_unnamed_77 = mad(fragment_unnamed_75, 2.0f, -1.0f);
				float fragment_unnamed_80 = mad(fragment_unnamed_70.y, 2.0f, -1.0f);
				precise float fragment_unnamed_86 = (-0.0f) - min(dot(float2(fragment_unnamed_77, fragment_unnamed_80), float2(fragment_unnamed_77, fragment_unnamed_80)), 1.0f);
				precise float fragment_unnamed_88 = fragment_unnamed_86 + 1.0f;
				float fragment_unnamed_89 = sqrt(fragment_unnamed_88);
				float fragment_unnamed_96 = dot(float3(fragment_input_2.x, fragment_input_2.y, fragment_input_2.z), float3(fragment_unnamed_77, fragment_unnamed_80, fragment_unnamed_89));
				float fragment_unnamed_105 = dot(float3(fragment_input_3.x, fragment_input_3.y, fragment_input_3.z), float3(fragment_unnamed_77, fragment_unnamed_80, fragment_unnamed_89));
				float fragment_unnamed_114 = dot(float3(fragment_input_4.x, fragment_input_4.y, fragment_input_4.z), float3(fragment_unnamed_77, fragment_unnamed_80, fragment_unnamed_89));
				float fragment_unnamed_120 = rsqrt(dot(float3(fragment_unnamed_96, fragment_unnamed_105, fragment_unnamed_114), float3(fragment_unnamed_96, fragment_unnamed_105, fragment_unnamed_114)));
				precise float fragment_unnamed_121 = fragment_unnamed_120 * fragment_unnamed_96;
				precise float fragment_unnamed_122 = fragment_unnamed_120 * fragment_unnamed_105;
				precise float fragment_unnamed_123 = fragment_unnamed_120 * fragment_unnamed_114;
				precise float fragment_unnamed_126 = (-0.0f) - fragment_input_5.x;
				precise float fragment_unnamed_129 = (-0.0f) - fragment_input_5.y;
				precise float fragment_unnamed_132 = (-0.0f) - fragment_input_5.z;
				precise float fragment_unnamed_140 = fragment_unnamed_126 + fragment_uniform_buffer_1[4u].x;
				precise float fragment_unnamed_141 = fragment_unnamed_129 + fragment_uniform_buffer_1[4u].y;
				precise float fragment_unnamed_142 = fragment_unnamed_132 + fragment_uniform_buffer_1[4u].z;
				float fragment_unnamed_146 = rsqrt(dot(float3(fragment_unnamed_140, fragment_unnamed_141, fragment_unnamed_142), float3(fragment_unnamed_140, fragment_unnamed_141, fragment_unnamed_142)));
				precise float fragment_unnamed_147 = fragment_unnamed_146 * fragment_unnamed_140;
				precise float fragment_unnamed_148 = fragment_unnamed_146 * fragment_unnamed_141;
				precise float fragment_unnamed_149 = fragment_unnamed_146 * fragment_unnamed_142;
				float fragment_unnamed_155 = mad(fragment_unnamed_140, fragment_unnamed_146, fragment_uniform_buffer_2[0u].x);
				float fragment_unnamed_156 = mad(fragment_unnamed_141, fragment_unnamed_146, fragment_uniform_buffer_2[0u].y);
				float fragment_unnamed_157 = mad(fragment_unnamed_142, fragment_unnamed_146, fragment_uniform_buffer_2[0u].z);
				float fragment_unnamed_158 = dot(float3(fragment_unnamed_121, fragment_unnamed_122, fragment_unnamed_123), float3(fragment_unnamed_147, fragment_unnamed_148, fragment_unnamed_149));
				float fragment_unnamed_169 = clamp(dot(float3(fragment_unnamed_121, fragment_unnamed_122, fragment_unnamed_123), float3(fragment_uniform_buffer_2[0u].xyz)), 0.0f, 1.0f);
				precise float fragment_unnamed_171 = (-0.0f) - abs(fragment_unnamed_158);
				precise float fragment_unnamed_172 = fragment_unnamed_171 + 1.0f;
				precise float fragment_unnamed_174 = abs(fragment_unnamed_158) + fragment_unnamed_169;
				precise float fragment_unnamed_175 = fragment_unnamed_174 + 9.9999997473787516355514526367188e-06f;
				precise float fragment_unnamed_177 = 0.5f / fragment_unnamed_175;
				precise float fragment_unnamed_179 = fragment_unnamed_177 * 0.99999988079071044921875f;
				precise float fragment_unnamed_181 = fragment_unnamed_169 * fragment_unnamed_179;
				precise float fragment_unnamed_182 = fragment_unnamed_172 * fragment_unnamed_172;
				precise float fragment_unnamed_183 = fragment_unnamed_182 * fragment_unnamed_182;
				precise float fragment_unnamed_184 = fragment_unnamed_172 * fragment_unnamed_183;
				float fragment_unnamed_190 = rsqrt(max(dot(float3(fragment_unnamed_155, fragment_unnamed_156, fragment_unnamed_157), float3(fragment_unnamed_155, fragment_unnamed_156, fragment_unnamed_157)), 0.001000000047497451305389404296875f));
				precise float fragment_unnamed_191 = fragment_unnamed_190 * fragment_unnamed_155;
				precise float fragment_unnamed_192 = fragment_unnamed_190 * fragment_unnamed_156;
				precise float fragment_unnamed_193 = fragment_unnamed_190 * fragment_unnamed_157;
				float fragment_unnamed_202 = clamp(dot(float3(fragment_uniform_buffer_2[0u].xyz), float3(fragment_unnamed_191, fragment_unnamed_192, fragment_unnamed_193)), 0.0f, 1.0f);
				precise float fragment_unnamed_206 = (-0.0f) - fragment_unnamed_202;
				precise float fragment_unnamed_207 = fragment_unnamed_206 + 1.0f;
				precise float fragment_unnamed_208 = dot(fragment_unnamed_202.xx, fragment_unnamed_202.xx) + (-0.5f);
				precise float fragment_unnamed_211 = (-0.0f) - fragment_unnamed_169;
				precise float fragment_unnamed_212 = fragment_unnamed_211 + 1.0f;
				precise float fragment_unnamed_213 = fragment_unnamed_212 * fragment_unnamed_212;
				precise float fragment_unnamed_214 = fragment_unnamed_213 * fragment_unnamed_213;
				precise float fragment_unnamed_215 = fragment_unnamed_212 * fragment_unnamed_214;
				precise float fragment_unnamed_217 = mad(fragment_unnamed_208, fragment_unnamed_184, 1.0f) * mad(fragment_unnamed_208, fragment_unnamed_215, 1.0f);
				precise float fragment_unnamed_218 = fragment_unnamed_169 * fragment_unnamed_217;
				precise float fragment_unnamed_226 = fragment_input_5.y * fragment_uniform_buffer_0[5u].x;
				precise float fragment_unnamed_227 = fragment_input_5.y * fragment_uniform_buffer_0[5u].y;
				precise float fragment_unnamed_249 = mad(fragment_uniform_buffer_0[6u].x, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].x, fragment_input_5.x, fragment_unnamed_226)) + fragment_uniform_buffer_0[7u].x;
				precise float fragment_unnamed_250 = mad(fragment_uniform_buffer_0[6u].y, fragment_input_5.z, mad(fragment_uniform_buffer_0[4u].y, fragment_input_5.x, fragment_unnamed_227)) + fragment_uniform_buffer_0[7u].y;
				float4 fragment_unnamed_252 = _LightTexture0.Sample(sampler_LightTexture0, float2(fragment_unnamed_249, fragment_unnamed_250));
				float fragment_unnamed_254 = fragment_unnamed_252.w;
				precise float fragment_unnamed_260 = fragment_unnamed_254 * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_261 = fragment_unnamed_254 * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_262 = fragment_unnamed_254 * fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_263 = fragment_unnamed_218 * fragment_unnamed_260;
				precise float fragment_unnamed_264 = fragment_unnamed_218 * fragment_unnamed_261;
				precise float fragment_unnamed_265 = fragment_unnamed_218 * fragment_unnamed_262;
				precise float fragment_unnamed_266 = fragment_unnamed_181 * fragment_unnamed_260;
				precise float fragment_unnamed_267 = fragment_unnamed_181 * fragment_unnamed_261;
				precise float fragment_unnamed_268 = fragment_unnamed_181 * fragment_unnamed_262;
				precise float fragment_unnamed_269 = fragment_unnamed_207 * fragment_unnamed_207;
				precise float fragment_unnamed_270 = fragment_unnamed_269 * fragment_unnamed_269;
				precise float fragment_unnamed_271 = fragment_unnamed_207 * fragment_unnamed_270;
				float fragment_unnamed_272 = mad(fragment_unnamed_271, 0.959999978542327880859375f, 0.039999999105930328369140625f);
				precise float fragment_unnamed_275 = fragment_unnamed_272 * fragment_unnamed_266;
				precise float fragment_unnamed_276 = fragment_unnamed_272 * fragment_unnamed_267;
				precise float fragment_unnamed_277 = fragment_unnamed_272 * fragment_unnamed_268;
				float4 fragment_unnamed_283 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				precise float fragment_unnamed_296 = fragment_unnamed_283.x * fragment_uniform_buffer_0[8u].x;
				precise float fragment_unnamed_297 = fragment_unnamed_283.y * fragment_uniform_buffer_0[8u].y;
				precise float fragment_unnamed_298 = fragment_unnamed_283.z * fragment_uniform_buffer_0[8u].z;
				precise float fragment_unnamed_299 = fragment_unnamed_283.w * fragment_uniform_buffer_0[8u].w;
				precise float fragment_unnamed_306 = fragment_unnamed_296 * fragment_input_6.x;
				precise float fragment_unnamed_307 = fragment_unnamed_297 * fragment_input_6.y;
				precise float fragment_unnamed_308 = fragment_unnamed_298 * fragment_input_6.z;
				precise float fragment_unnamed_312 = fragment_unnamed_299 * fragment_input_6.w;
				fragment_output_0.w = fragment_unnamed_312;
				precise float fragment_unnamed_315 = fragment_unnamed_306 * 0.959999978542327880859375f;
				precise float fragment_unnamed_316 = fragment_unnamed_307 * 0.959999978542327880859375f;
				precise float fragment_unnamed_317 = fragment_unnamed_308 * 0.959999978542327880859375f;
				precise float fragment_unnamed_325 = fragment_input_2 / fragment_uniform_buffer_1[5u].y;
				precise float fragment_unnamed_326 = (-0.0f) - fragment_unnamed_325;
				precise float fragment_unnamed_327 = fragment_unnamed_326 + 1.0f;
				precise float fragment_unnamed_331 = fragment_unnamed_327 * fragment_uniform_buffer_1[5u].z;
				float fragment_unnamed_340 = clamp(mad(max(fragment_unnamed_331, 0.0f), fragment_uniform_buffer_3[1u].z, fragment_uniform_buffer_3[1u].w), 0.0f, 1.0f);
				precise float fragment_unnamed_341 = mad(fragment_unnamed_315, fragment_unnamed_263, fragment_unnamed_275) * fragment_unnamed_340;
				precise float fragment_unnamed_342 = mad(fragment_unnamed_316, fragment_unnamed_264, fragment_unnamed_276) * fragment_unnamed_340;
				precise float fragment_unnamed_343 = mad(fragment_unnamed_317, fragment_unnamed_265, fragment_unnamed_277) * fragment_unnamed_340;
				fragment_output_0.x = fragment_unnamed_341;
				fragment_output_0.y = fragment_unnamed_342;
				fragment_output_0.z = fragment_unnamed_343;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_LightColor0[0], _LightColor0[1], _LightColor0[2], _LightColor0[3]);

				fragment_uniform_buffer_0[4] = float4(unity_WorldToLight[0][0], unity_WorldToLight[1][0], unity_WorldToLight[2][0], unity_WorldToLight[3][0]);
				fragment_uniform_buffer_0[5] = float4(unity_WorldToLight[0][1], unity_WorldToLight[1][1], unity_WorldToLight[2][1], unity_WorldToLight[3][1]);
				fragment_uniform_buffer_0[6] = float4(unity_WorldToLight[0][2], unity_WorldToLight[1][2], unity_WorldToLight[2][2], unity_WorldToLight[3][2]);
				fragment_uniform_buffer_0[7] = float4(unity_WorldToLight[0][3], unity_WorldToLight[1][3], unity_WorldToLight[2][3], unity_WorldToLight[3][3]);

				fragment_uniform_buffer_0[8] = float4(_Color[0], _Color[1], _Color[2], _Color[3]);

				fragment_uniform_buffer_1[4] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_1[4][3]);

				fragment_uniform_buffer_1[5] = float4(_ProjectionParams[0], _ProjectionParams[1], _ProjectionParams[2], _ProjectionParams[3]);

				fragment_uniform_buffer_2[0] = float4(_WorldSpaceLightPos0[0], _WorldSpaceLightPos0[1], _WorldSpaceLightPos0[2], _WorldSpaceLightPos0[3]);

				fragment_uniform_buffer_3[1] = float4(unity_FogParams[0], unity_FogParams[1], unity_FogParams[2], unity_FogParams[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_2 = stage_input.fragment_input_2;
				fragment_input_3 = stage_input.fragment_input_3;
				fragment_input_4 = stage_input.fragment_input_4;
				fragment_input_5 = stage_input.fragment_input_5;
				fragment_input_6 = stage_input.fragment_input_6;
				fragment_input_7 = stage_input.fragment_input_7;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL_COOKIE
			#endif // FOG_LINEAR
			#endif // !DIRECTIONAL
			#endif // !POINT
			#endif // !POINT_COOKIE
			#endif // !SPOT


			// Fallback Shader Code
			#ifndef ANY_SHADER_VARIANT_ACTIVE

			// https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
			float4x4 unity_MatrixMVP;

			struct Vertex_Stage_Input
			{
				float3 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixMVP, float4(input.pos, 1.0));
				return output;
			}

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				// Output solid grey color (e.g., 50% grey)
				return float4(0.5, 0.5, 0.5, 1.0); // RGBA
			}

			#endif // !ANY_SHADER_VARIANT_ACTIVE


			ENDHLSL
		}
	}
	FallBack "Diffuse"
}
