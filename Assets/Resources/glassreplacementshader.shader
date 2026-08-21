Shader "Hidden/GlassReplacementShader"
{
	Properties
	{
		[HideInInspector] __dirty ("", Float) = 1
	}
	SubShader
	{
		Tags { "IsEmissive" = "true" "QUEUE" = "Geometry+0" "RenderType" = "Opaque" }
		Pass
		{
			Name "FORWARD"
			Tags { "IsEmissive" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Geometry+0" "RenderType" = "Opaque" }
			GpuProgramID 28813

			HLSLPROGRAM

			// https://docs.unity3d.com/Manual/SL-PragmaDirectives.html
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			#pragma shader_feature DIRECTIONAL
			#pragma multi_compile _ FOG_LINEAR
			#pragma multi_compile _ LIGHTPROBE_SH


			#ifdef DIRECTIONAL
			#ifndef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[8];
			static float4 vertex_uniform_buffer_1[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float3 vertex_output_1;
			static float3 vertex_output_2;

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
				float3 vertex_output_1 : TEXCOORD; // TEXCOORD
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_46 = vertex_input_0.y * vertex_uniform_buffer_0[1u].x;
				precise float vertex_unnamed_47 = vertex_input_0.y * vertex_uniform_buffer_0[1u].y;
				precise float vertex_unnamed_48 = vertex_input_0.y * vertex_uniform_buffer_0[1u].z;
				precise float vertex_unnamed_49 = vertex_input_0.y * vertex_uniform_buffer_0[1u].w;
				float vertex_unnamed_72 = mad(vertex_uniform_buffer_0[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].x, vertex_input_0.x, vertex_unnamed_46));
				float vertex_unnamed_73 = mad(vertex_uniform_buffer_0[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].y, vertex_input_0.x, vertex_unnamed_47));
				float vertex_unnamed_74 = mad(vertex_uniform_buffer_0[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].z, vertex_input_0.x, vertex_unnamed_48));
				precise float vertex_unnamed_83 = vertex_unnamed_72 + vertex_uniform_buffer_0[3u].x;
				precise float vertex_unnamed_84 = vertex_unnamed_73 + vertex_uniform_buffer_0[3u].y;
				precise float vertex_unnamed_85 = vertex_unnamed_74 + vertex_uniform_buffer_0[3u].z;
				precise float vertex_unnamed_86 = mad(vertex_uniform_buffer_0[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].w, vertex_input_0.x, vertex_unnamed_49)) + vertex_uniform_buffer_0[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_0[3u].x, vertex_input_0.w, vertex_unnamed_72);
				vertex_output_2.y = mad(vertex_uniform_buffer_0[3u].y, vertex_input_0.w, vertex_unnamed_73);
				vertex_output_2.z = mad(vertex_uniform_buffer_0[3u].z, vertex_input_0.w, vertex_unnamed_74);
				precise float vertex_unnamed_108 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].x;
				precise float vertex_unnamed_109 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].y;
				precise float vertex_unnamed_110 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].z;
				precise float vertex_unnamed_111 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_1[20u].x, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].x, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].x, vertex_unnamed_83, vertex_unnamed_108)));
				gl_Position.y = mad(vertex_uniform_buffer_1[20u].y, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].y, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].y, vertex_unnamed_83, vertex_unnamed_109)));
				gl_Position.z = mad(vertex_uniform_buffer_1[20u].z, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].z, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].z, vertex_unnamed_83, vertex_unnamed_110)));
				gl_Position.w = mad(vertex_uniform_buffer_1[20u].w, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].w, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].w, vertex_unnamed_83, vertex_unnamed_111)));
				float vertex_unnamed_161 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[4u].xyz));
				float vertex_unnamed_176 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[5u].xyz));
				float vertex_unnamed_191 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[6u].xyz));
				float vertex_unnamed_197 = rsqrt(dot(float3(vertex_unnamed_161, vertex_unnamed_176, vertex_unnamed_191), float3(vertex_unnamed_161, vertex_unnamed_176, vertex_unnamed_191)));
				precise float vertex_unnamed_198 = vertex_unnamed_197 * vertex_unnamed_161;
				precise float vertex_unnamed_199 = vertex_unnamed_197 * vertex_unnamed_176;
				precise float vertex_unnamed_200 = vertex_unnamed_197 * vertex_unnamed_191;
				vertex_output_1.x = vertex_unnamed_198;
				vertex_output_1.y = vertex_unnamed_199;
				vertex_output_1.z = vertex_unnamed_200;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_0[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_0[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_0[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_0[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_1[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_1[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_1[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

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
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // !FOG_LINEAR
			#endif // !LIGHTPROBE_SH


			#ifdef DIRECTIONAL
			#ifndef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4x4 unity_MatrixVP;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float3 vertex_output_1;
			static float3 vertex_input_1;
			static float3 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float3 vertex_input_1 : NORMAL;
			};

			struct Vertex_Stage_Output
			{
				float3 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 vertex_output_1 : TEXCOORD1; // vs_TEXCOORD1
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_44;
			static float vertex_unnamed_118;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_44 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_output_1 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = vertex_unnamed_44.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_9 = (unity_MatrixVP__array[0] * vertex_unnamed_44.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_MatrixVP__array[2] * vertex_unnamed_44.zzzz) + vertex_unnamed_9;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_44.wwww) + vertex_unnamed_9;
				vertex_unnamed_9.x = dot(vertex_input_1, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_9.y = dot(vertex_input_1, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_9.z = dot(vertex_input_1, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_118 = dot(vertex_unnamed_9.xyz, vertex_unnamed_9.xyz);
				vertex_unnamed_118 = rsqrt(vertex_unnamed_118);
				vertex_output_0 = vertex_unnamed_118.xxx * vertex_unnamed_9.xyz;
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
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			static float4 fragment_output_0;

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				fragment_output_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
			}

			Fragment_Stage_Output frag()
			{
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // !FOG_LINEAR
			#endif // !LIGHTPROBE_SH


			#ifdef DIRECTIONAL
			#ifdef LIGHTPROBE_SH
			#ifndef FOG_LINEAR
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[8];
			static float4 vertex_uniform_buffer_1[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float3 vertex_output_1;
			static float3 vertex_output_2;

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
				float3 vertex_output_1 : TEXCOORD; // TEXCOORD
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_46 = vertex_input_0.y * vertex_uniform_buffer_0[1u].x;
				precise float vertex_unnamed_47 = vertex_input_0.y * vertex_uniform_buffer_0[1u].y;
				precise float vertex_unnamed_48 = vertex_input_0.y * vertex_uniform_buffer_0[1u].z;
				precise float vertex_unnamed_49 = vertex_input_0.y * vertex_uniform_buffer_0[1u].w;
				float vertex_unnamed_72 = mad(vertex_uniform_buffer_0[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].x, vertex_input_0.x, vertex_unnamed_46));
				float vertex_unnamed_73 = mad(vertex_uniform_buffer_0[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].y, vertex_input_0.x, vertex_unnamed_47));
				float vertex_unnamed_74 = mad(vertex_uniform_buffer_0[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].z, vertex_input_0.x, vertex_unnamed_48));
				precise float vertex_unnamed_83 = vertex_unnamed_72 + vertex_uniform_buffer_0[3u].x;
				precise float vertex_unnamed_84 = vertex_unnamed_73 + vertex_uniform_buffer_0[3u].y;
				precise float vertex_unnamed_85 = vertex_unnamed_74 + vertex_uniform_buffer_0[3u].z;
				precise float vertex_unnamed_86 = mad(vertex_uniform_buffer_0[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].w, vertex_input_0.x, vertex_unnamed_49)) + vertex_uniform_buffer_0[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_0[3u].x, vertex_input_0.w, vertex_unnamed_72);
				vertex_output_2.y = mad(vertex_uniform_buffer_0[3u].y, vertex_input_0.w, vertex_unnamed_73);
				vertex_output_2.z = mad(vertex_uniform_buffer_0[3u].z, vertex_input_0.w, vertex_unnamed_74);
				precise float vertex_unnamed_108 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].x;
				precise float vertex_unnamed_109 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].y;
				precise float vertex_unnamed_110 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].z;
				precise float vertex_unnamed_111 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_1[20u].x, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].x, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].x, vertex_unnamed_83, vertex_unnamed_108)));
				gl_Position.y = mad(vertex_uniform_buffer_1[20u].y, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].y, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].y, vertex_unnamed_83, vertex_unnamed_109)));
				gl_Position.z = mad(vertex_uniform_buffer_1[20u].z, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].z, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].z, vertex_unnamed_83, vertex_unnamed_110)));
				gl_Position.w = mad(vertex_uniform_buffer_1[20u].w, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].w, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].w, vertex_unnamed_83, vertex_unnamed_111)));
				float vertex_unnamed_161 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[4u].xyz));
				float vertex_unnamed_176 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[5u].xyz));
				float vertex_unnamed_191 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[6u].xyz));
				float vertex_unnamed_197 = rsqrt(dot(float3(vertex_unnamed_161, vertex_unnamed_176, vertex_unnamed_191), float3(vertex_unnamed_161, vertex_unnamed_176, vertex_unnamed_191)));
				precise float vertex_unnamed_198 = vertex_unnamed_197 * vertex_unnamed_161;
				precise float vertex_unnamed_199 = vertex_unnamed_197 * vertex_unnamed_176;
				precise float vertex_unnamed_200 = vertex_unnamed_197 * vertex_unnamed_191;
				vertex_output_1.x = vertex_unnamed_198;
				vertex_output_1.y = vertex_unnamed_199;
				vertex_output_1.z = vertex_unnamed_200;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_0[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_0[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_0[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_0[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_1[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_1[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_1[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

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
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // LIGHTPROBE_SH
			#endif // !FOG_LINEAR


			#ifdef DIRECTIONAL
			#ifdef LIGHTPROBE_SH
			#ifndef FOG_LINEAR
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4x4 unity_MatrixVP;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float3 vertex_output_1;
			static float3 vertex_input_1;
			static float3 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float3 vertex_input_1 : NORMAL;
			};

			struct Vertex_Stage_Output
			{
				float3 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float3 vertex_output_1 : TEXCOORD1; // vs_TEXCOORD1
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_44;
			static float vertex_unnamed_118;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_44 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_output_1 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = vertex_unnamed_44.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_9 = (unity_MatrixVP__array[0] * vertex_unnamed_44.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_MatrixVP__array[2] * vertex_unnamed_44.zzzz) + vertex_unnamed_9;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_44.wwww) + vertex_unnamed_9;
				vertex_unnamed_9.x = dot(vertex_input_1, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_9.y = dot(vertex_input_1, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_9.z = dot(vertex_input_1, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_118 = dot(vertex_unnamed_9.xyz, vertex_unnamed_9.xyz);
				vertex_unnamed_118 = rsqrt(vertex_unnamed_118);
				vertex_output_0 = vertex_unnamed_118.xxx * vertex_unnamed_9.xyz;
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
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			static float4 fragment_output_0;

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				fragment_output_0 = float4(0.0f, 0.0f, 0.0f, 1.0f);
			}

			Fragment_Stage_Output frag()
			{
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // LIGHTPROBE_SH
			#endif // !FOG_LINEAR


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[8];
			static float4 vertex_uniform_buffer_1[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float3 vertex_output_1;
			static float vertex_output_1;
			static float3 vertex_output_2;

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
				float3 vertex_output_1 : TEXCOORD; // TEXCOORD
				float vertex_output_1 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_48 = vertex_input_0.y * vertex_uniform_buffer_0[1u].x;
				precise float vertex_unnamed_49 = vertex_input_0.y * vertex_uniform_buffer_0[1u].y;
				precise float vertex_unnamed_50 = vertex_input_0.y * vertex_uniform_buffer_0[1u].z;
				precise float vertex_unnamed_51 = vertex_input_0.y * vertex_uniform_buffer_0[1u].w;
				float vertex_unnamed_74 = mad(vertex_uniform_buffer_0[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].x, vertex_input_0.x, vertex_unnamed_48));
				float vertex_unnamed_75 = mad(vertex_uniform_buffer_0[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].y, vertex_input_0.x, vertex_unnamed_49));
				float vertex_unnamed_76 = mad(vertex_uniform_buffer_0[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].z, vertex_input_0.x, vertex_unnamed_50));
				precise float vertex_unnamed_85 = vertex_unnamed_74 + vertex_uniform_buffer_0[3u].x;
				precise float vertex_unnamed_86 = vertex_unnamed_75 + vertex_uniform_buffer_0[3u].y;
				precise float vertex_unnamed_87 = vertex_unnamed_76 + vertex_uniform_buffer_0[3u].z;
				precise float vertex_unnamed_88 = mad(vertex_uniform_buffer_0[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].w, vertex_input_0.x, vertex_unnamed_51)) + vertex_uniform_buffer_0[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_0[3u].x, vertex_input_0.w, vertex_unnamed_74);
				vertex_output_2.y = mad(vertex_uniform_buffer_0[3u].y, vertex_input_0.w, vertex_unnamed_75);
				vertex_output_2.z = mad(vertex_uniform_buffer_0[3u].z, vertex_input_0.w, vertex_unnamed_76);
				precise float vertex_unnamed_109 = vertex_unnamed_86 * vertex_uniform_buffer_1[18u].x;
				precise float vertex_unnamed_110 = vertex_unnamed_86 * vertex_uniform_buffer_1[18u].y;
				precise float vertex_unnamed_111 = vertex_unnamed_86 * vertex_uniform_buffer_1[18u].z;
				precise float vertex_unnamed_112 = vertex_unnamed_86 * vertex_uniform_buffer_1[18u].w;
				float vertex_unnamed_144 = mad(vertex_uniform_buffer_1[20u].z, vertex_unnamed_88, mad(vertex_uniform_buffer_1[19u].z, vertex_unnamed_87, mad(vertex_uniform_buffer_1[17u].z, vertex_unnamed_85, vertex_unnamed_111)));
				gl_Position.x = mad(vertex_uniform_buffer_1[20u].x, vertex_unnamed_88, mad(vertex_uniform_buffer_1[19u].x, vertex_unnamed_87, mad(vertex_uniform_buffer_1[17u].x, vertex_unnamed_85, vertex_unnamed_109)));
				gl_Position.y = mad(vertex_uniform_buffer_1[20u].y, vertex_unnamed_88, mad(vertex_uniform_buffer_1[19u].y, vertex_unnamed_87, mad(vertex_uniform_buffer_1[17u].y, vertex_unnamed_85, vertex_unnamed_110)));
				gl_Position.z = vertex_unnamed_144;
				gl_Position.w = mad(vertex_uniform_buffer_1[20u].w, vertex_unnamed_88, mad(vertex_uniform_buffer_1[19u].w, vertex_unnamed_87, mad(vertex_uniform_buffer_1[17u].w, vertex_unnamed_85, vertex_unnamed_112)));
				vertex_output_1 = vertex_unnamed_144;
				float vertex_unnamed_162 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[4u].xyz));
				float vertex_unnamed_177 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[5u].xyz));
				float vertex_unnamed_192 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[6u].xyz));
				float vertex_unnamed_198 = rsqrt(dot(float3(vertex_unnamed_162, vertex_unnamed_177, vertex_unnamed_192), float3(vertex_unnamed_162, vertex_unnamed_177, vertex_unnamed_192)));
				precise float vertex_unnamed_199 = vertex_unnamed_198 * vertex_unnamed_162;
				precise float vertex_unnamed_200 = vertex_unnamed_198 * vertex_unnamed_177;
				precise float vertex_unnamed_201 = vertex_unnamed_198 * vertex_unnamed_192;
				vertex_output_1.x = vertex_unnamed_199;
				vertex_output_1.y = vertex_unnamed_200;
				vertex_output_1.z = vertex_unnamed_201;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_0[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_0[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_0[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_0[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_1[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_1[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_1[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

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
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // !LIGHTPROBE_SH


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4x4 unity_MatrixVP;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float3 vertex_output_2;
			static float vertex_output_0;
			static float3 vertex_input_1;
			static float3 vertex_output_1;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float3 vertex_input_1 : NORMAL;
			};

			struct Vertex_Stage_Output
			{
				float vertex_output_0 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_1 : TEXCOORD0; // vs_TEXCOORD0
				float3 vertex_output_2 : TEXCOORD1; // vs_TEXCOORD1
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_44;
			static float vertex_unnamed_123;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_44 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_output_2 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = vertex_unnamed_44.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_9 = (unity_MatrixVP__array[0] * vertex_unnamed_44.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_MatrixVP__array[2] * vertex_unnamed_44.zzzz) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_MatrixVP__array[3] * vertex_unnamed_44.wwww) + vertex_unnamed_9;
				gl_Position = vertex_unnamed_9;
				vertex_output_0 = vertex_unnamed_9.z;
				vertex_unnamed_9.x = dot(vertex_input_1, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_9.y = dot(vertex_input_1, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_9.z = dot(vertex_input_1, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_123 = dot(vertex_unnamed_9.xyz, vertex_unnamed_9.xyz);
				vertex_unnamed_123 = rsqrt(vertex_unnamed_123);
				vertex_output_1 = vertex_unnamed_123.xxx * vertex_unnamed_9.xyz;
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
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_1 = vertex_output_1;
				return stage_output;
			}

			float4 _ProjectionParams;
			float4 unity_FogColor;
			float4 unity_FogParams;

			static float fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float fragment_input_0 : TEXCOORD2; // vs_TEXCOORD2
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float fragment_unnamed_8;

			void frag_main()
			{
				fragment_unnamed_8 = fragment_input_0 / _ProjectionParams.y;
				fragment_unnamed_8 = (-fragment_unnamed_8) + 1.0f;
				fragment_unnamed_8 *= _ProjectionParams.z;
				fragment_unnamed_8 = max(fragment_unnamed_8, 0.0f);
				fragment_unnamed_8 = (fragment_unnamed_8 * unity_FogParams.z) + unity_FogParams.w;
				fragment_unnamed_8 = clamp(fragment_unnamed_8, 0.0f, 1.0f);
				float3 fragment_unnamed_62 = (fragment_unnamed_8.xxx * (-unity_FogColor.xyz)) + unity_FogColor.xyz;
				fragment_output_0 = float4(fragment_unnamed_62.x, fragment_unnamed_62.y, fragment_unnamed_62.z, fragment_output_0.w);
				fragment_output_0.w = 1.0f;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // !LIGHTPROBE_SH


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifdef LIGHTPROBE_SH
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[8];
			static float4 vertex_uniform_buffer_1[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float3 vertex_input_2;
			static float4 vertex_input_3;
			static float4 vertex_input_4;
			static float4 vertex_input_5;
			static float4 vertex_input_6;
			static float4 vertex_input_7;
			static float3 vertex_output_1;
			static float vertex_output_1;
			static float3 vertex_output_2;

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
				float3 vertex_output_1 : TEXCOORD; // TEXCOORD
				float vertex_output_1 : TEXCOORD2; // TEXCOORD_2
				float3 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_48 = vertex_input_0.y * vertex_uniform_buffer_0[1u].x;
				precise float vertex_unnamed_49 = vertex_input_0.y * vertex_uniform_buffer_0[1u].y;
				precise float vertex_unnamed_50 = vertex_input_0.y * vertex_uniform_buffer_0[1u].z;
				precise float vertex_unnamed_51 = vertex_input_0.y * vertex_uniform_buffer_0[1u].w;
				float vertex_unnamed_74 = mad(vertex_uniform_buffer_0[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].x, vertex_input_0.x, vertex_unnamed_48));
				float vertex_unnamed_75 = mad(vertex_uniform_buffer_0[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].y, vertex_input_0.x, vertex_unnamed_49));
				float vertex_unnamed_76 = mad(vertex_uniform_buffer_0[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].z, vertex_input_0.x, vertex_unnamed_50));
				precise float vertex_unnamed_85 = vertex_unnamed_74 + vertex_uniform_buffer_0[3u].x;
				precise float vertex_unnamed_86 = vertex_unnamed_75 + vertex_uniform_buffer_0[3u].y;
				precise float vertex_unnamed_87 = vertex_unnamed_76 + vertex_uniform_buffer_0[3u].z;
				precise float vertex_unnamed_88 = mad(vertex_uniform_buffer_0[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].w, vertex_input_0.x, vertex_unnamed_51)) + vertex_uniform_buffer_0[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_0[3u].x, vertex_input_0.w, vertex_unnamed_74);
				vertex_output_2.y = mad(vertex_uniform_buffer_0[3u].y, vertex_input_0.w, vertex_unnamed_75);
				vertex_output_2.z = mad(vertex_uniform_buffer_0[3u].z, vertex_input_0.w, vertex_unnamed_76);
				precise float vertex_unnamed_109 = vertex_unnamed_86 * vertex_uniform_buffer_1[18u].x;
				precise float vertex_unnamed_110 = vertex_unnamed_86 * vertex_uniform_buffer_1[18u].y;
				precise float vertex_unnamed_111 = vertex_unnamed_86 * vertex_uniform_buffer_1[18u].z;
				precise float vertex_unnamed_112 = vertex_unnamed_86 * vertex_uniform_buffer_1[18u].w;
				float vertex_unnamed_144 = mad(vertex_uniform_buffer_1[20u].z, vertex_unnamed_88, mad(vertex_uniform_buffer_1[19u].z, vertex_unnamed_87, mad(vertex_uniform_buffer_1[17u].z, vertex_unnamed_85, vertex_unnamed_111)));
				gl_Position.x = mad(vertex_uniform_buffer_1[20u].x, vertex_unnamed_88, mad(vertex_uniform_buffer_1[19u].x, vertex_unnamed_87, mad(vertex_uniform_buffer_1[17u].x, vertex_unnamed_85, vertex_unnamed_109)));
				gl_Position.y = mad(vertex_uniform_buffer_1[20u].y, vertex_unnamed_88, mad(vertex_uniform_buffer_1[19u].y, vertex_unnamed_87, mad(vertex_uniform_buffer_1[17u].y, vertex_unnamed_85, vertex_unnamed_110)));
				gl_Position.z = vertex_unnamed_144;
				gl_Position.w = mad(vertex_uniform_buffer_1[20u].w, vertex_unnamed_88, mad(vertex_uniform_buffer_1[19u].w, vertex_unnamed_87, mad(vertex_uniform_buffer_1[17u].w, vertex_unnamed_85, vertex_unnamed_112)));
				vertex_output_1 = vertex_unnamed_144;
				float vertex_unnamed_162 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[4u].xyz));
				float vertex_unnamed_177 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[5u].xyz));
				float vertex_unnamed_192 = dot(float3(vertex_input_2.x, vertex_input_2.y, vertex_input_2.z), float3(vertex_uniform_buffer_0[6u].xyz));
				float vertex_unnamed_198 = rsqrt(dot(float3(vertex_unnamed_162, vertex_unnamed_177, vertex_unnamed_192), float3(vertex_unnamed_162, vertex_unnamed_177, vertex_unnamed_192)));
				precise float vertex_unnamed_199 = vertex_unnamed_198 * vertex_unnamed_162;
				precise float vertex_unnamed_200 = vertex_unnamed_198 * vertex_unnamed_177;
				precise float vertex_unnamed_201 = vertex_unnamed_198 * vertex_unnamed_192;
				vertex_output_1.x = vertex_unnamed_199;
				vertex_output_1.y = vertex_unnamed_200;
				vertex_output_1.z = vertex_unnamed_201;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_0[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_0[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_0[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_0[4] = float4(unity_WorldToObject[0][0], unity_WorldToObject[1][0], unity_WorldToObject[2][0], unity_WorldToObject[3][0]);
				vertex_uniform_buffer_0[5] = float4(unity_WorldToObject[0][1], unity_WorldToObject[1][1], unity_WorldToObject[2][1], unity_WorldToObject[3][1]);
				vertex_uniform_buffer_0[6] = float4(unity_WorldToObject[0][2], unity_WorldToObject[1][2], unity_WorldToObject[2][2], unity_WorldToObject[3][2]);
				vertex_uniform_buffer_0[7] = float4(unity_WorldToObject[0][3], unity_WorldToObject[1][3], unity_WorldToObject[2][3], unity_WorldToObject[3][3]);

				vertex_uniform_buffer_1[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_1[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_1[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_1[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

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
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // LIGHTPROBE_SH


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifdef LIGHTPROBE_SH
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_WorldToObject;
			float4x4 unity_MatrixVP;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_WorldToObject__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float3 vertex_output_2;
			static float vertex_output_0;
			static float3 vertex_input_1;
			static float3 vertex_output_1;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float3 vertex_input_1 : NORMAL;
			};

			struct Vertex_Stage_Output
			{
				float vertex_output_0 : TEXCOORD2; // vs_TEXCOORD2
				float3 vertex_output_1 : TEXCOORD0; // vs_TEXCOORD0
				float3 vertex_output_2 : TEXCOORD1; // vs_TEXCOORD1
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_9;
			static float4 vertex_unnamed_44;
			static float vertex_unnamed_123;

			void vert_main()
			{
				vertex_unnamed_9 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_9 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_9;
				vertex_unnamed_44 = vertex_unnamed_9 + unity_ObjectToWorld__array[3];
				vertex_output_2 = (unity_ObjectToWorld__array[3].xyz * vertex_input_0.www) + vertex_unnamed_9.xyz;
				vertex_unnamed_9 = vertex_unnamed_44.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_9 = (unity_MatrixVP__array[0] * vertex_unnamed_44.xxxx) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_MatrixVP__array[2] * vertex_unnamed_44.zzzz) + vertex_unnamed_9;
				vertex_unnamed_9 = (unity_MatrixVP__array[3] * vertex_unnamed_44.wwww) + vertex_unnamed_9;
				gl_Position = vertex_unnamed_9;
				vertex_output_0 = vertex_unnamed_9.z;
				vertex_unnamed_9.x = dot(vertex_input_1, unity_WorldToObject__array[0].xyz);
				vertex_unnamed_9.y = dot(vertex_input_1, unity_WorldToObject__array[1].xyz);
				vertex_unnamed_9.z = dot(vertex_input_1, unity_WorldToObject__array[2].xyz);
				vertex_unnamed_123 = dot(vertex_unnamed_9.xyz, vertex_unnamed_9.xyz);
				vertex_unnamed_123 = rsqrt(vertex_unnamed_123);
				vertex_output_1 = vertex_unnamed_123.xxx * vertex_unnamed_9.xyz;
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
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_2 = vertex_output_2;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_1 = vertex_output_1;
				return stage_output;
			}

			float4 _ProjectionParams;
			float4 unity_FogColor;
			float4 unity_FogParams;

			static float fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float fragment_input_0 : TEXCOORD2; // vs_TEXCOORD2
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float fragment_unnamed_8;

			void frag_main()
			{
				fragment_unnamed_8 = fragment_input_0 / _ProjectionParams.y;
				fragment_unnamed_8 = (-fragment_unnamed_8) + 1.0f;
				fragment_unnamed_8 *= _ProjectionParams.z;
				fragment_unnamed_8 = max(fragment_unnamed_8, 0.0f);
				fragment_unnamed_8 = (fragment_unnamed_8 * unity_FogParams.z) + unity_FogParams.w;
				fragment_unnamed_8 = clamp(fragment_unnamed_8, 0.0f, 1.0f);
				float3 fragment_unnamed_62 = (fragment_unnamed_8.xxx * (-unity_FogColor.xyz)) + unity_FogColor.xyz;
				fragment_output_0 = float4(fragment_unnamed_62.x, fragment_unnamed_62.y, fragment_unnamed_62.z, fragment_output_0.w);
				fragment_output_0.w = 1.0f;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // LIGHTPROBE_SH


			#ifdef DIRECTIONAL
			#ifndef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#define ANY_SHADER_VARIANT_ACTIVE

			static float3 fragment_input_1;
			static float3 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float3 fragment_input_1 : TEXCOORD; // TEXCOORD
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				fragment_output_0.x = 0.0f;
				fragment_output_0.y = 0.0f;
				fragment_output_0.z = 0.0f;
				fragment_output_0.w = 1.0f;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // !FOG_LINEAR
			#endif // !LIGHTPROBE_SH


			#ifdef DIRECTIONAL
			#ifdef LIGHTPROBE_SH
			#ifndef FOG_LINEAR
			#define ANY_SHADER_VARIANT_ACTIVE

			static float3 fragment_input_1;
			static float3 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float3 fragment_input_1 : TEXCOORD; // TEXCOORD
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				fragment_output_0.x = 0.0f;
				fragment_output_0.y = 0.0f;
				fragment_output_0.z = 0.0f;
				fragment_output_0.w = 1.0f;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // LIGHTPROBE_SH
			#endif // !FOG_LINEAR


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifndef LIGHTPROBE_SH
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _ProjectionParams;
			float4 unity_FogColor;
			float4 unity_FogParams;

			static float4 fragment_uniform_buffer_0[6];
			static float4 fragment_uniform_buffer_1[2];
			static float3 fragment_input_1;
			static float fragment_input_1;
			static float3 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float3 fragment_input_1 : TEXCOORD; // TEXCOORD
				float fragment_input_1 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_35 = fragment_input_1 / fragment_uniform_buffer_0[5u].y;
				precise float fragment_unnamed_36 = (-0.0f) - fragment_unnamed_35;
				precise float fragment_unnamed_38 = fragment_unnamed_36 + 1.0f;
				precise float fragment_unnamed_43 = fragment_unnamed_38 * fragment_uniform_buffer_0[5u].z;
				float fragment_unnamed_55 = clamp(mad(max(fragment_unnamed_43, 0.0f), fragment_uniform_buffer_1[1u].z, fragment_uniform_buffer_1[1u].w), 0.0f, 1.0f);
				precise float fragment_unnamed_59 = (-0.0f) - fragment_uniform_buffer_1[0u].x;
				precise float fragment_unnamed_61 = (-0.0f) - fragment_uniform_buffer_1[0u].y;
				precise float fragment_unnamed_63 = (-0.0f) - fragment_uniform_buffer_1[0u].z;
				fragment_output_0.x = mad(fragment_unnamed_55, fragment_unnamed_59, fragment_uniform_buffer_1[0u].x);
				fragment_output_0.y = mad(fragment_unnamed_55, fragment_unnamed_61, fragment_uniform_buffer_1[0u].y);
				fragment_output_0.z = mad(fragment_unnamed_55, fragment_unnamed_63, fragment_uniform_buffer_1[0u].z);
				fragment_output_0.w = 1.0f;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[5] = float4(_ProjectionParams[0], _ProjectionParams[1], _ProjectionParams[2], _ProjectionParams[3]);

				fragment_uniform_buffer_1[0] = float4(unity_FogColor[0], unity_FogColor[1], unity_FogColor[2], unity_FogColor[3]);

				fragment_uniform_buffer_1[1] = float4(unity_FogParams[0], unity_FogParams[1], unity_FogParams[2], unity_FogParams[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // !LIGHTPROBE_SH


			#ifdef DIRECTIONAL
			#ifdef FOG_LINEAR
			#ifdef LIGHTPROBE_SH
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _ProjectionParams;
			float4 unity_FogColor;
			float4 unity_FogParams;

			static float4 fragment_uniform_buffer_0[6];
			static float4 fragment_uniform_buffer_1[2];
			static float3 fragment_input_1;
			static float fragment_input_1;
			static float3 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float3 fragment_input_1 : TEXCOORD; // TEXCOORD
				float fragment_input_1 : TEXCOORD2; // TEXCOORD_2
				float3 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_35 = fragment_input_1 / fragment_uniform_buffer_0[5u].y;
				precise float fragment_unnamed_36 = (-0.0f) - fragment_unnamed_35;
				precise float fragment_unnamed_38 = fragment_unnamed_36 + 1.0f;
				precise float fragment_unnamed_43 = fragment_unnamed_38 * fragment_uniform_buffer_0[5u].z;
				float fragment_unnamed_55 = clamp(mad(max(fragment_unnamed_43, 0.0f), fragment_uniform_buffer_1[1u].z, fragment_uniform_buffer_1[1u].w), 0.0f, 1.0f);
				precise float fragment_unnamed_59 = (-0.0f) - fragment_uniform_buffer_1[0u].x;
				precise float fragment_unnamed_61 = (-0.0f) - fragment_uniform_buffer_1[0u].y;
				precise float fragment_unnamed_63 = (-0.0f) - fragment_uniform_buffer_1[0u].z;
				fragment_output_0.x = mad(fragment_unnamed_55, fragment_unnamed_59, fragment_uniform_buffer_1[0u].x);
				fragment_output_0.y = mad(fragment_unnamed_55, fragment_unnamed_61, fragment_uniform_buffer_1[0u].y);
				fragment_output_0.z = mad(fragment_unnamed_55, fragment_unnamed_63, fragment_uniform_buffer_1[0u].z);
				fragment_output_0.w = 1.0f;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[5] = float4(_ProjectionParams[0], _ProjectionParams[1], _ProjectionParams[2], _ProjectionParams[3]);

				fragment_uniform_buffer_1[0] = float4(unity_FogColor[0], unity_FogColor[1], unity_FogColor[2], unity_FogColor[3]);

				fragment_uniform_buffer_1[1] = float4(unity_FogParams[0], unity_FogParams[1], unity_FogParams[2], unity_FogParams[3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // DIRECTIONAL
			#endif // FOG_LINEAR
			#endif // LIGHTPROBE_SH


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
	CustomEditor "ASEMaterialInspector"
}
