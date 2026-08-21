Shader "IES/IESToCubemap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		LOD 100
		Tags { "RenderType" = "Opaque" }
		Pass
		{
			LOD 100
			Tags { "RenderType" = "Opaque" }
			GpuProgramID 14899

			HLSLPROGRAM

			// https://docs.unity3d.com/Manual/SL-PragmaDirectives.html
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			#pragma shader_feature BOTTOM_VERTICAL
			#pragma shader_feature FULL_HORIZONTAL
			#pragma shader_feature FULL_VERTICAL
			#pragma shader_feature HALF_HORIZONTAL
			#pragma shader_feature QUAD_HORIZONTAL
			#pragma shader_feature TOP_VERTICAL


			#ifdef BOTTOM_VERTICAL
			#ifdef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[3];
			static float4 vertex_uniform_buffer_1[4];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;
			static float4 vertex_output_2;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = mad(vertex_input_1.x, vertex_uniform_buffer_0[2u].x, vertex_uniform_buffer_0[2u].z);
				vertex_output_0.y = mad(vertex_input_1.y, vertex_uniform_buffer_0[2u].y, vertex_uniform_buffer_0[2u].w);
				precise float vertex_unnamed_64 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_65 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_64));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_65));
				float vertex_unnamed_90 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_66));
				float vertex_unnamed_91 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_67));
				precise float vertex_unnamed_98 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_99 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_100 = vertex_unnamed_90 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_101 = vertex_unnamed_91 + vertex_uniform_buffer_1[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_88);
				vertex_output_2.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_2.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_90);
				vertex_output_2.w = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_91);
				precise float vertex_unnamed_125 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_126 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_127 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_128 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_98, vertex_unnamed_125)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_98, vertex_unnamed_126)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_98, vertex_unnamed_127)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_98, vertex_unnamed_128)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[2] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				return stage_output;
			}

			#endif // BOTTOM_VERTICAL
			#endif // FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef BOTTOM_VERTICAL
			#ifdef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_1;
			static float2 vertex_input_1;
			static float4 vertex_input_0;
			static float4 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float4 vertex_output_0 : TEXCOORD1; // vs_TEXCOORD1
				float2 vertex_output_1 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_33;
			static float4 vertex_unnamed_57;

			void vert_main()
			{
				vertex_output_1 = (vertex_input_1 * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_33 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_33 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_33;
				vertex_unnamed_57 = vertex_unnamed_33 + unity_ObjectToWorld__array[3];
				vertex_output_0 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_33;
				vertex_unnamed_33 = vertex_unnamed_57.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_33 = (unity_MatrixVP__array[0] * vertex_unnamed_57.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_MatrixVP__array[2] * vertex_unnamed_57.zzzz) + vertex_unnamed_33;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_57.wwww) + vertex_unnamed_33;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_0 = stage_input.vertex_input_0;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float4 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float4 fragment_input_0 : TEXCOORD1; // vs_TEXCOORD1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float fragment_unnamed_31;
			static float fragment_unnamed_49;
			static bool fragment_unnamed_81;
			static bool fragment_unnamed_100;
			static bool fragment_unnamed_129;
			static float fragment_unnamed_168;
			static float fragment_unnamed_177;
			static float fragment_unnamed_189;
			static bool fragment_unnamed_211;

			void frag_main()
			{
				fragment_unnamed_9.x = max(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x = 1.0f / fragment_unnamed_9.x;
				fragment_unnamed_31 = min(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x *= fragment_unnamed_31;
				fragment_unnamed_31 = fragment_unnamed_9.x * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_31 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.1801410019397735595703125f;
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + (-0.33029949665069580078125f);
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.999866008758544921875f;
				fragment_unnamed_49 = fragment_unnamed_31 * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_49 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_81 = abs(fragment_input_0.x) < abs(fragment_input_0.z);
				fragment_unnamed_49 = fragment_unnamed_81 ? fragment_unnamed_49 : 0.0f;
				fragment_unnamed_9.x = (fragment_unnamed_9.x * fragment_unnamed_31) + fragment_unnamed_49;
				fragment_unnamed_100 = fragment_input_0.x < (-fragment_input_0.x);
				fragment_unnamed_31 = fragment_unnamed_100 ? (-3.1415927410125732421875f) : 0.0f;
				fragment_unnamed_9.x = fragment_unnamed_31 + fragment_unnamed_9.x;
				fragment_unnamed_31 = min(fragment_input_0.x, fragment_input_0.z);
				fragment_unnamed_100 = fragment_unnamed_31 < (-fragment_unnamed_31);
				fragment_unnamed_49 = max(fragment_input_0.x, fragment_input_0.z);
				fragment_unnamed_129 = fragment_unnamed_49 >= (-fragment_unnamed_49);
				fragment_unnamed_100 = fragment_unnamed_129 && fragment_unnamed_100;
				float fragment_unnamed_139;
				if (fragment_unnamed_100)
				{
					fragment_unnamed_139 = -fragment_unnamed_9.x;
				}
				else
				{
					fragment_unnamed_139 = fragment_unnamed_9.x;
				}
				fragment_unnamed_9.x = fragment_unnamed_139;
				fragment_unnamed_9.x += 3.141590118408203125f;
				fragment_unnamed_9.x *= 0.159155070781707763671875f;
				fragment_unnamed_49 = dot(fragment_input_0.xz, fragment_input_0.xz);
				fragment_unnamed_49 = sqrt(fragment_unnamed_49);
				fragment_unnamed_168 = max(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_168 = 1.0f / fragment_unnamed_168;
				fragment_unnamed_177 = min(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_168 *= fragment_unnamed_177;
				fragment_unnamed_177 = fragment_unnamed_168 * fragment_unnamed_168;
				fragment_unnamed_189 = (fragment_unnamed_177 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_189 = (fragment_unnamed_177 * fragment_unnamed_189) + 0.1801410019397735595703125f;
				fragment_unnamed_189 = (fragment_unnamed_177 * fragment_unnamed_189) + (-0.33029949665069580078125f);
				fragment_unnamed_177 = (fragment_unnamed_177 * fragment_unnamed_189) + 0.999866008758544921875f;
				fragment_unnamed_189 = fragment_unnamed_168 * fragment_unnamed_177;
				fragment_unnamed_189 = (fragment_unnamed_189 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_211 = fragment_unnamed_49 < abs(fragment_input_0.y);
				fragment_unnamed_49 = min(fragment_unnamed_49, fragment_input_0.y);
				fragment_unnamed_129 = fragment_unnamed_49 < (-fragment_unnamed_49);
				fragment_unnamed_189 = fragment_unnamed_211 ? fragment_unnamed_189 : 0.0f;
				fragment_unnamed_168 = (fragment_unnamed_168 * fragment_unnamed_177) + fragment_unnamed_189;
				float fragment_unnamed_234;
				if (fragment_unnamed_129)
				{
					fragment_unnamed_234 = -fragment_unnamed_168;
				}
				else
				{
					fragment_unnamed_234 = fragment_unnamed_168;
				}
				fragment_unnamed_49 = fragment_unnamed_234;
				fragment_unnamed_9.y = ((-fragment_unnamed_49) * (-0.6366202831268310546875f)) + 1.0f;
				fragment_unnamed_9 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_9.xy).xyz;
				fragment_unnamed_81 = 0.0f >= fragment_input_0.y;
				fragment_unnamed_168 = float(fragment_unnamed_81);
				fragment_output_0 = fragment_unnamed_168.xxxx * fragment_unnamed_9.xyzx;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // BOTTOM_VERTICAL
			#endif // FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef BOTTOM_VERTICAL
			#ifdef HALF_HORIZONTAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[3];
			static float4 vertex_uniform_buffer_1[4];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;
			static float4 vertex_output_2;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = mad(vertex_input_1.x, vertex_uniform_buffer_0[2u].x, vertex_uniform_buffer_0[2u].z);
				vertex_output_0.y = mad(vertex_input_1.y, vertex_uniform_buffer_0[2u].y, vertex_uniform_buffer_0[2u].w);
				precise float vertex_unnamed_64 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_65 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_64));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_65));
				float vertex_unnamed_90 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_66));
				float vertex_unnamed_91 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_67));
				precise float vertex_unnamed_98 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_99 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_100 = vertex_unnamed_90 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_101 = vertex_unnamed_91 + vertex_uniform_buffer_1[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_88);
				vertex_output_2.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_2.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_90);
				vertex_output_2.w = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_91);
				precise float vertex_unnamed_125 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_126 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_127 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_128 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_98, vertex_unnamed_125)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_98, vertex_unnamed_126)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_98, vertex_unnamed_127)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_98, vertex_unnamed_128)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[2] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				return stage_output;
			}

			#endif // BOTTOM_VERTICAL
			#endif // HALF_HORIZONTAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef BOTTOM_VERTICAL
			#ifdef HALF_HORIZONTAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_1;
			static float2 vertex_input_1;
			static float4 vertex_input_0;
			static float4 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float4 vertex_output_0 : TEXCOORD1; // vs_TEXCOORD1
				float2 vertex_output_1 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_33;
			static float4 vertex_unnamed_57;

			void vert_main()
			{
				vertex_output_1 = (vertex_input_1 * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_33 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_33 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_33;
				vertex_unnamed_57 = vertex_unnamed_33 + unity_ObjectToWorld__array[3];
				vertex_output_0 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_33;
				vertex_unnamed_33 = vertex_unnamed_57.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_33 = (unity_MatrixVP__array[0] * vertex_unnamed_57.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_MatrixVP__array[2] * vertex_unnamed_57.zzzz) + vertex_unnamed_33;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_57.wwww) + vertex_unnamed_33;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_0 = stage_input.vertex_input_0;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float4 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float4 fragment_input_0 : TEXCOORD1; // vs_TEXCOORD1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float fragment_unnamed_31;
			static float fragment_unnamed_49;
			static bool fragment_unnamed_81;
			static bool fragment_unnamed_100;
			static float fragment_unnamed_151;
			static float fragment_unnamed_160;
			static float fragment_unnamed_172;
			static bool fragment_unnamed_194;
			static bool fragment_unnamed_204;

			void frag_main()
			{
				fragment_unnamed_9.x = max(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x = 1.0f / fragment_unnamed_9.x;
				fragment_unnamed_31 = min(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x *= fragment_unnamed_31;
				fragment_unnamed_31 = fragment_unnamed_9.x * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_31 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.1801410019397735595703125f;
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + (-0.33029949665069580078125f);
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.999866008758544921875f;
				fragment_unnamed_49 = fragment_unnamed_31 * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_49 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_81 = abs(fragment_input_0.x) < abs(fragment_input_0.z);
				fragment_unnamed_49 = fragment_unnamed_81 ? fragment_unnamed_49 : 0.0f;
				fragment_unnamed_9.x = (fragment_unnamed_9.x * fragment_unnamed_31) + fragment_unnamed_49;
				fragment_unnamed_100 = fragment_input_0.x < (-fragment_input_0.x);
				fragment_unnamed_31 = fragment_unnamed_100 ? (-3.1415927410125732421875f) : 0.0f;
				fragment_unnamed_9.x = fragment_unnamed_31 + fragment_unnamed_9.x;
				fragment_unnamed_31 = min(fragment_input_0.x, abs(fragment_input_0.z));
				fragment_unnamed_100 = fragment_unnamed_31 < (-fragment_unnamed_31);
				float fragment_unnamed_127;
				if (fragment_unnamed_100)
				{
					fragment_unnamed_127 = -fragment_unnamed_9.x;
				}
				else
				{
					fragment_unnamed_127 = fragment_unnamed_9.x;
				}
				fragment_unnamed_9.x = fragment_unnamed_127;
				fragment_unnamed_9.x *= 0.31831014156341552734375f;
				fragment_unnamed_49 = dot(fragment_input_0.xz, fragment_input_0.xz);
				fragment_unnamed_49 = sqrt(fragment_unnamed_49);
				fragment_unnamed_151 = max(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_151 = 1.0f / fragment_unnamed_151;
				fragment_unnamed_160 = min(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_151 *= fragment_unnamed_160;
				fragment_unnamed_160 = fragment_unnamed_151 * fragment_unnamed_151;
				fragment_unnamed_172 = (fragment_unnamed_160 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_172 = (fragment_unnamed_160 * fragment_unnamed_172) + 0.1801410019397735595703125f;
				fragment_unnamed_172 = (fragment_unnamed_160 * fragment_unnamed_172) + (-0.33029949665069580078125f);
				fragment_unnamed_160 = (fragment_unnamed_160 * fragment_unnamed_172) + 0.999866008758544921875f;
				fragment_unnamed_172 = fragment_unnamed_151 * fragment_unnamed_160;
				fragment_unnamed_172 = (fragment_unnamed_172 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_194 = fragment_unnamed_49 < abs(fragment_input_0.y);
				fragment_unnamed_49 = min(fragment_unnamed_49, fragment_input_0.y);
				fragment_unnamed_204 = fragment_unnamed_49 < (-fragment_unnamed_49);
				fragment_unnamed_172 = fragment_unnamed_194 ? fragment_unnamed_172 : 0.0f;
				fragment_unnamed_151 = (fragment_unnamed_151 * fragment_unnamed_160) + fragment_unnamed_172;
				float fragment_unnamed_218;
				if (fragment_unnamed_204)
				{
					fragment_unnamed_218 = -fragment_unnamed_151;
				}
				else
				{
					fragment_unnamed_218 = fragment_unnamed_151;
				}
				fragment_unnamed_49 = fragment_unnamed_218;
				fragment_unnamed_9.y = ((-fragment_unnamed_49) * (-0.6366202831268310546875f)) + 1.0f;
				fragment_unnamed_9 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_9.xy).xyz;
				fragment_unnamed_81 = 0.0f >= fragment_input_0.y;
				fragment_unnamed_151 = float(fragment_unnamed_81);
				fragment_output_0 = fragment_unnamed_151.xxxx * fragment_unnamed_9.xyzx;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // BOTTOM_VERTICAL
			#endif // HALF_HORIZONTAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef BOTTOM_VERTICAL
			#ifdef QUAD_HORIZONTAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[3];
			static float4 vertex_uniform_buffer_1[4];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;
			static float4 vertex_output_2;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = mad(vertex_input_1.x, vertex_uniform_buffer_0[2u].x, vertex_uniform_buffer_0[2u].z);
				vertex_output_0.y = mad(vertex_input_1.y, vertex_uniform_buffer_0[2u].y, vertex_uniform_buffer_0[2u].w);
				precise float vertex_unnamed_64 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_65 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_64));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_65));
				float vertex_unnamed_90 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_66));
				float vertex_unnamed_91 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_67));
				precise float vertex_unnamed_98 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_99 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_100 = vertex_unnamed_90 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_101 = vertex_unnamed_91 + vertex_uniform_buffer_1[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_88);
				vertex_output_2.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_2.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_90);
				vertex_output_2.w = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_91);
				precise float vertex_unnamed_125 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_126 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_127 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_128 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_98, vertex_unnamed_125)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_98, vertex_unnamed_126)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_98, vertex_unnamed_127)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_98, vertex_unnamed_128)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[2] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				return stage_output;
			}

			#endif // BOTTOM_VERTICAL
			#endif // QUAD_HORIZONTAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef BOTTOM_VERTICAL
			#ifdef QUAD_HORIZONTAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_1;
			static float2 vertex_input_1;
			static float4 vertex_input_0;
			static float4 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float4 vertex_output_0 : TEXCOORD1; // vs_TEXCOORD1
				float2 vertex_output_1 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_33;
			static float4 vertex_unnamed_57;

			void vert_main()
			{
				vertex_output_1 = (vertex_input_1 * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_33 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_33 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_33;
				vertex_unnamed_57 = vertex_unnamed_33 + unity_ObjectToWorld__array[3];
				vertex_output_0 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_33;
				vertex_unnamed_33 = vertex_unnamed_57.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_33 = (unity_MatrixVP__array[0] * vertex_unnamed_57.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_MatrixVP__array[2] * vertex_unnamed_57.zzzz) + vertex_unnamed_33;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_57.wwww) + vertex_unnamed_33;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_0 = stage_input.vertex_input_0;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float4 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float4 fragment_input_0 : TEXCOORD1; // vs_TEXCOORD1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float fragment_unnamed_27;
			static float fragment_unnamed_39;
			static float fragment_unnamed_52;
			static bool fragment_unnamed_83;
			static bool fragment_unnamed_96;
			static float fragment_unnamed_153;
			static bool fragment_unnamed_175;
			static bool fragment_unnamed_209;

			void frag_main()
			{
				fragment_unnamed_9.x = dot(fragment_input_0.xz, fragment_input_0.xz);
				fragment_unnamed_9.x = sqrt(fragment_unnamed_9.x);
				fragment_unnamed_27 = max(fragment_unnamed_9.x, abs(fragment_input_0.y));
				fragment_unnamed_27 = 1.0f / fragment_unnamed_27;
				fragment_unnamed_39 = min(fragment_unnamed_9.x, abs(fragment_input_0.y));
				fragment_unnamed_27 *= fragment_unnamed_39;
				fragment_unnamed_39 = fragment_unnamed_27 * fragment_unnamed_27;
				fragment_unnamed_52 = (fragment_unnamed_39 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_52 = (fragment_unnamed_39 * fragment_unnamed_52) + 0.1801410019397735595703125f;
				fragment_unnamed_52 = (fragment_unnamed_39 * fragment_unnamed_52) + (-0.33029949665069580078125f);
				fragment_unnamed_39 = (fragment_unnamed_39 * fragment_unnamed_52) + 0.999866008758544921875f;
				fragment_unnamed_52 = fragment_unnamed_39 * fragment_unnamed_27;
				fragment_unnamed_52 = (fragment_unnamed_52 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_83 = fragment_unnamed_9.x < abs(fragment_input_0.y);
				fragment_unnamed_9.x = min(fragment_unnamed_9.x, fragment_input_0.y);
				fragment_unnamed_96 = fragment_unnamed_9.x < (-fragment_unnamed_9.x);
				fragment_unnamed_52 = fragment_unnamed_83 ? fragment_unnamed_52 : 0.0f;
				fragment_unnamed_27 = (fragment_unnamed_27 * fragment_unnamed_39) + fragment_unnamed_52;
				float fragment_unnamed_114;
				if (fragment_unnamed_96)
				{
					fragment_unnamed_114 = -fragment_unnamed_27;
				}
				else
				{
					fragment_unnamed_114 = fragment_unnamed_27;
				}
				fragment_unnamed_9.x = fragment_unnamed_114;
				fragment_unnamed_9.y = ((-fragment_unnamed_9.x) * (-0.6366202831268310546875f)) + 1.0f;
				fragment_unnamed_39 = max(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_39 = 1.0f / fragment_unnamed_39;
				fragment_unnamed_52 = min(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_39 *= fragment_unnamed_52;
				fragment_unnamed_52 = fragment_unnamed_39 * fragment_unnamed_39;
				fragment_unnamed_153 = (fragment_unnamed_52 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_153 = (fragment_unnamed_52 * fragment_unnamed_153) + 0.1801410019397735595703125f;
				fragment_unnamed_153 = (fragment_unnamed_52 * fragment_unnamed_153) + (-0.33029949665069580078125f);
				fragment_unnamed_52 = (fragment_unnamed_52 * fragment_unnamed_153) + 0.999866008758544921875f;
				fragment_unnamed_153 = fragment_unnamed_52 * fragment_unnamed_39;
				fragment_unnamed_153 = (fragment_unnamed_153 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_175 = abs(fragment_input_0.x) < abs(fragment_input_0.z);
				fragment_unnamed_153 = fragment_unnamed_175 ? fragment_unnamed_153 : 0.0f;
				fragment_unnamed_39 = (fragment_unnamed_39 * fragment_unnamed_52) + fragment_unnamed_153;
				fragment_unnamed_9.x = fragment_unnamed_39 * 0.6366202831268310546875f;
				fragment_unnamed_9 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_9.xy).xyz;
				fragment_unnamed_209 = 0.0f >= fragment_input_0.y;
				fragment_unnamed_52 = float(fragment_unnamed_209);
				fragment_output_0 = fragment_unnamed_52.xxxx * fragment_unnamed_9.xyzx;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // BOTTOM_VERTICAL
			#endif // QUAD_HORIZONTAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef FULL_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[3];
			static float4 vertex_uniform_buffer_1[4];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;
			static float4 vertex_output_2;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = mad(vertex_input_1.x, vertex_uniform_buffer_0[2u].x, vertex_uniform_buffer_0[2u].z);
				vertex_output_0.y = mad(vertex_input_1.y, vertex_uniform_buffer_0[2u].y, vertex_uniform_buffer_0[2u].w);
				precise float vertex_unnamed_64 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_65 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_64));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_65));
				float vertex_unnamed_90 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_66));
				float vertex_unnamed_91 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_67));
				precise float vertex_unnamed_98 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_99 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_100 = vertex_unnamed_90 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_101 = vertex_unnamed_91 + vertex_uniform_buffer_1[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_88);
				vertex_output_2.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_2.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_90);
				vertex_output_2.w = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_91);
				precise float vertex_unnamed_125 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_126 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_127 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_128 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_98, vertex_unnamed_125)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_98, vertex_unnamed_126)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_98, vertex_unnamed_127)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_98, vertex_unnamed_128)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[2] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				return stage_output;
			}

			#endif // FULL_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL


			#ifdef FULL_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_1;
			static float2 vertex_input_1;
			static float4 vertex_input_0;
			static float4 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float4 vertex_output_0 : TEXCOORD1; // vs_TEXCOORD1
				float2 vertex_output_1 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_33;
			static float4 vertex_unnamed_57;

			void vert_main()
			{
				vertex_output_1 = (vertex_input_1 * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_33 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_33 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_33;
				vertex_unnamed_57 = vertex_unnamed_33 + unity_ObjectToWorld__array[3];
				vertex_output_0 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_33;
				vertex_unnamed_33 = vertex_unnamed_57.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_33 = (unity_MatrixVP__array[0] * vertex_unnamed_57.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_MatrixVP__array[2] * vertex_unnamed_57.zzzz) + vertex_unnamed_33;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_57.wwww) + vertex_unnamed_33;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_0 = stage_input.vertex_input_0;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float4 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float4 fragment_input_0 : TEXCOORD1; // vs_TEXCOORD1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float fragment_unnamed_31;
			static float fragment_unnamed_49;
			static bool fragment_unnamed_81;
			static bool fragment_unnamed_100;
			static bool fragment_unnamed_129;
			static float fragment_unnamed_163;
			static float fragment_unnamed_172;
			static float fragment_unnamed_184;
			static bool fragment_unnamed_206;

			void frag_main()
			{
				fragment_unnamed_9.x = max(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x = 1.0f / fragment_unnamed_9.x;
				fragment_unnamed_31 = min(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x *= fragment_unnamed_31;
				fragment_unnamed_31 = fragment_unnamed_9.x * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_31 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.1801410019397735595703125f;
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + (-0.33029949665069580078125f);
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.999866008758544921875f;
				fragment_unnamed_49 = fragment_unnamed_31 * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_49 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_81 = abs(fragment_input_0.x) < abs(fragment_input_0.z);
				fragment_unnamed_49 = fragment_unnamed_81 ? fragment_unnamed_49 : 0.0f;
				fragment_unnamed_9.x = (fragment_unnamed_9.x * fragment_unnamed_31) + fragment_unnamed_49;
				fragment_unnamed_100 = fragment_input_0.x < (-fragment_input_0.x);
				fragment_unnamed_31 = fragment_unnamed_100 ? (-3.1415927410125732421875f) : 0.0f;
				fragment_unnamed_9.x = fragment_unnamed_31 + fragment_unnamed_9.x;
				fragment_unnamed_31 = min(fragment_input_0.x, fragment_input_0.z);
				fragment_unnamed_100 = fragment_unnamed_31 < (-fragment_unnamed_31);
				fragment_unnamed_49 = max(fragment_input_0.x, fragment_input_0.z);
				fragment_unnamed_129 = fragment_unnamed_49 >= (-fragment_unnamed_49);
				fragment_unnamed_100 = fragment_unnamed_129 && fragment_unnamed_100;
				float fragment_unnamed_139;
				if (fragment_unnamed_100)
				{
					fragment_unnamed_139 = -fragment_unnamed_9.x;
				}
				else
				{
					fragment_unnamed_139 = fragment_unnamed_9.x;
				}
				fragment_unnamed_9.x = fragment_unnamed_139;
				fragment_unnamed_9.x += 3.141590118408203125f;
				fragment_unnamed_49 = dot(fragment_input_0.xz, fragment_input_0.xz);
				fragment_unnamed_49 = sqrt(fragment_unnamed_49);
				fragment_unnamed_163 = max(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_163 = 1.0f / fragment_unnamed_163;
				fragment_unnamed_172 = min(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_163 *= fragment_unnamed_172;
				fragment_unnamed_172 = fragment_unnamed_163 * fragment_unnamed_163;
				fragment_unnamed_184 = (fragment_unnamed_172 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_184 = (fragment_unnamed_172 * fragment_unnamed_184) + 0.1801410019397735595703125f;
				fragment_unnamed_184 = (fragment_unnamed_172 * fragment_unnamed_184) + (-0.33029949665069580078125f);
				fragment_unnamed_172 = (fragment_unnamed_172 * fragment_unnamed_184) + 0.999866008758544921875f;
				fragment_unnamed_184 = fragment_unnamed_163 * fragment_unnamed_172;
				fragment_unnamed_184 = (fragment_unnamed_184 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_206 = fragment_unnamed_49 < abs(fragment_input_0.y);
				fragment_unnamed_49 = min(fragment_unnamed_49, fragment_input_0.y);
				fragment_unnamed_129 = fragment_unnamed_49 < (-fragment_unnamed_49);
				fragment_unnamed_184 = fragment_unnamed_206 ? fragment_unnamed_184 : 0.0f;
				fragment_unnamed_163 = (fragment_unnamed_163 * fragment_unnamed_172) + fragment_unnamed_184;
				float fragment_unnamed_229;
				if (fragment_unnamed_129)
				{
					fragment_unnamed_229 = -fragment_unnamed_163;
				}
				else
				{
					fragment_unnamed_229 = fragment_unnamed_163;
				}
				fragment_unnamed_9.z = fragment_unnamed_229;
				float2 fragment_unnamed_243 = fragment_unnamed_9.xz * float2(0.159155070781707763671875f, 0.6366202831268310546875f);
				fragment_unnamed_9 = float3(fragment_unnamed_243.x, fragment_unnamed_243.y, fragment_unnamed_9.z);
				fragment_unnamed_9 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_9.xy).xyz;
				fragment_unnamed_81 = fragment_input_0.y >= 0.0f;
				fragment_unnamed_163 = float(fragment_unnamed_81);
				fragment_output_0 = fragment_unnamed_163.xxxx * fragment_unnamed_9.xyzx;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FULL_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL


			#ifdef HALF_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[3];
			static float4 vertex_uniform_buffer_1[4];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;
			static float4 vertex_output_2;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = mad(vertex_input_1.x, vertex_uniform_buffer_0[2u].x, vertex_uniform_buffer_0[2u].z);
				vertex_output_0.y = mad(vertex_input_1.y, vertex_uniform_buffer_0[2u].y, vertex_uniform_buffer_0[2u].w);
				precise float vertex_unnamed_64 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_65 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_64));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_65));
				float vertex_unnamed_90 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_66));
				float vertex_unnamed_91 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_67));
				precise float vertex_unnamed_98 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_99 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_100 = vertex_unnamed_90 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_101 = vertex_unnamed_91 + vertex_uniform_buffer_1[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_88);
				vertex_output_2.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_2.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_90);
				vertex_output_2.w = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_91);
				precise float vertex_unnamed_125 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_126 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_127 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_128 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_98, vertex_unnamed_125)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_98, vertex_unnamed_126)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_98, vertex_unnamed_127)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_98, vertex_unnamed_128)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[2] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				return stage_output;
			}

			#endif // HALF_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !QUAD_HORIZONTAL


			#ifdef HALF_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_1;
			static float2 vertex_input_1;
			static float4 vertex_input_0;
			static float4 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float4 vertex_output_0 : TEXCOORD1; // vs_TEXCOORD1
				float2 vertex_output_1 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_33;
			static float4 vertex_unnamed_57;

			void vert_main()
			{
				vertex_output_1 = (vertex_input_1 * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_33 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_33 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_33;
				vertex_unnamed_57 = vertex_unnamed_33 + unity_ObjectToWorld__array[3];
				vertex_output_0 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_33;
				vertex_unnamed_33 = vertex_unnamed_57.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_33 = (unity_MatrixVP__array[0] * vertex_unnamed_57.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_MatrixVP__array[2] * vertex_unnamed_57.zzzz) + vertex_unnamed_33;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_57.wwww) + vertex_unnamed_33;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_0 = stage_input.vertex_input_0;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float4 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float4 fragment_input_0 : TEXCOORD1; // vs_TEXCOORD1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float fragment_unnamed_31;
			static float fragment_unnamed_49;
			static bool fragment_unnamed_81;
			static bool fragment_unnamed_100;
			static float fragment_unnamed_146;
			static float fragment_unnamed_155;
			static float fragment_unnamed_167;
			static bool fragment_unnamed_189;
			static bool fragment_unnamed_199;

			void frag_main()
			{
				fragment_unnamed_9.x = max(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x = 1.0f / fragment_unnamed_9.x;
				fragment_unnamed_31 = min(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x *= fragment_unnamed_31;
				fragment_unnamed_31 = fragment_unnamed_9.x * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_31 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.1801410019397735595703125f;
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + (-0.33029949665069580078125f);
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.999866008758544921875f;
				fragment_unnamed_49 = fragment_unnamed_31 * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_49 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_81 = abs(fragment_input_0.x) < abs(fragment_input_0.z);
				fragment_unnamed_49 = fragment_unnamed_81 ? fragment_unnamed_49 : 0.0f;
				fragment_unnamed_9.x = (fragment_unnamed_9.x * fragment_unnamed_31) + fragment_unnamed_49;
				fragment_unnamed_100 = fragment_input_0.x < (-fragment_input_0.x);
				fragment_unnamed_31 = fragment_unnamed_100 ? (-3.1415927410125732421875f) : 0.0f;
				fragment_unnamed_9.x = fragment_unnamed_31 + fragment_unnamed_9.x;
				fragment_unnamed_31 = min(fragment_input_0.x, abs(fragment_input_0.z));
				fragment_unnamed_100 = fragment_unnamed_31 < (-fragment_unnamed_31);
				float fragment_unnamed_127;
				if (fragment_unnamed_100)
				{
					fragment_unnamed_127 = -fragment_unnamed_9.x;
				}
				else
				{
					fragment_unnamed_127 = fragment_unnamed_9.x;
				}
				fragment_unnamed_9.x = fragment_unnamed_127;
				fragment_unnamed_49 = dot(fragment_input_0.xz, fragment_input_0.xz);
				fragment_unnamed_49 = sqrt(fragment_unnamed_49);
				fragment_unnamed_146 = max(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_146 = 1.0f / fragment_unnamed_146;
				fragment_unnamed_155 = min(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_146 *= fragment_unnamed_155;
				fragment_unnamed_155 = fragment_unnamed_146 * fragment_unnamed_146;
				fragment_unnamed_167 = (fragment_unnamed_155 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_167 = (fragment_unnamed_155 * fragment_unnamed_167) + 0.1801410019397735595703125f;
				fragment_unnamed_167 = (fragment_unnamed_155 * fragment_unnamed_167) + (-0.33029949665069580078125f);
				fragment_unnamed_155 = (fragment_unnamed_155 * fragment_unnamed_167) + 0.999866008758544921875f;
				fragment_unnamed_167 = fragment_unnamed_146 * fragment_unnamed_155;
				fragment_unnamed_167 = (fragment_unnamed_167 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_189 = fragment_unnamed_49 < abs(fragment_input_0.y);
				fragment_unnamed_49 = min(fragment_unnamed_49, fragment_input_0.y);
				fragment_unnamed_199 = fragment_unnamed_49 < (-fragment_unnamed_49);
				fragment_unnamed_167 = fragment_unnamed_189 ? fragment_unnamed_167 : 0.0f;
				fragment_unnamed_146 = (fragment_unnamed_146 * fragment_unnamed_155) + fragment_unnamed_167;
				float fragment_unnamed_213;
				if (fragment_unnamed_199)
				{
					fragment_unnamed_213 = -fragment_unnamed_146;
				}
				else
				{
					fragment_unnamed_213 = fragment_unnamed_146;
				}
				fragment_unnamed_9.z = fragment_unnamed_213;
				float2 fragment_unnamed_227 = fragment_unnamed_9.xz * float2(0.31831014156341552734375f, 0.6366202831268310546875f);
				fragment_unnamed_9 = float3(fragment_unnamed_227.x, fragment_unnamed_227.y, fragment_unnamed_9.z);
				fragment_unnamed_9 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_9.xy).xyz;
				fragment_unnamed_81 = fragment_input_0.y >= 0.0f;
				fragment_unnamed_146 = float(fragment_unnamed_81);
				fragment_output_0 = fragment_unnamed_146.xxxx * fragment_unnamed_9.xyzx;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // HALF_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !QUAD_HORIZONTAL


			#ifdef QUAD_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[3];
			static float4 vertex_uniform_buffer_1[4];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;
			static float4 vertex_output_2;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = mad(vertex_input_1.x, vertex_uniform_buffer_0[2u].x, vertex_uniform_buffer_0[2u].z);
				vertex_output_0.y = mad(vertex_input_1.y, vertex_uniform_buffer_0[2u].y, vertex_uniform_buffer_0[2u].w);
				precise float vertex_unnamed_64 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_65 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_64));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_65));
				float vertex_unnamed_90 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_66));
				float vertex_unnamed_91 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_67));
				precise float vertex_unnamed_98 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_99 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_100 = vertex_unnamed_90 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_101 = vertex_unnamed_91 + vertex_uniform_buffer_1[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_88);
				vertex_output_2.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_2.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_90);
				vertex_output_2.w = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_91);
				precise float vertex_unnamed_125 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_126 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_127 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_128 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_98, vertex_unnamed_125)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_98, vertex_unnamed_126)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_98, vertex_unnamed_127)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_98, vertex_unnamed_128)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[2] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				return stage_output;
			}

			#endif // QUAD_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL


			#ifdef QUAD_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_1;
			static float2 vertex_input_1;
			static float4 vertex_input_0;
			static float4 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float4 vertex_output_0 : TEXCOORD1; // vs_TEXCOORD1
				float2 vertex_output_1 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_33;
			static float4 vertex_unnamed_57;

			void vert_main()
			{
				vertex_output_1 = (vertex_input_1 * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_33 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_33 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_33;
				vertex_unnamed_57 = vertex_unnamed_33 + unity_ObjectToWorld__array[3];
				vertex_output_0 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_33;
				vertex_unnamed_33 = vertex_unnamed_57.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_33 = (unity_MatrixVP__array[0] * vertex_unnamed_57.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_MatrixVP__array[2] * vertex_unnamed_57.zzzz) + vertex_unnamed_33;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_57.wwww) + vertex_unnamed_33;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_0 = stage_input.vertex_input_0;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float4 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float4 fragment_input_0 : TEXCOORD1; // vs_TEXCOORD1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float fragment_unnamed_27;
			static float fragment_unnamed_39;
			static float fragment_unnamed_52;
			static bool fragment_unnamed_83;
			static bool fragment_unnamed_96;
			static float fragment_unnamed_151;
			static bool fragment_unnamed_173;
			static bool fragment_unnamed_206;

			void frag_main()
			{
				fragment_unnamed_9.x = dot(fragment_input_0.xz, fragment_input_0.xz);
				fragment_unnamed_9.x = sqrt(fragment_unnamed_9.x);
				fragment_unnamed_27 = max(fragment_unnamed_9.x, abs(fragment_input_0.y));
				fragment_unnamed_27 = 1.0f / fragment_unnamed_27;
				fragment_unnamed_39 = min(fragment_unnamed_9.x, abs(fragment_input_0.y));
				fragment_unnamed_27 *= fragment_unnamed_39;
				fragment_unnamed_39 = fragment_unnamed_27 * fragment_unnamed_27;
				fragment_unnamed_52 = (fragment_unnamed_39 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_52 = (fragment_unnamed_39 * fragment_unnamed_52) + 0.1801410019397735595703125f;
				fragment_unnamed_52 = (fragment_unnamed_39 * fragment_unnamed_52) + (-0.33029949665069580078125f);
				fragment_unnamed_39 = (fragment_unnamed_39 * fragment_unnamed_52) + 0.999866008758544921875f;
				fragment_unnamed_52 = fragment_unnamed_39 * fragment_unnamed_27;
				fragment_unnamed_52 = (fragment_unnamed_52 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_83 = fragment_unnamed_9.x < abs(fragment_input_0.y);
				fragment_unnamed_9.x = min(fragment_unnamed_9.x, fragment_input_0.y);
				fragment_unnamed_96 = fragment_unnamed_9.x < (-fragment_unnamed_9.x);
				fragment_unnamed_52 = fragment_unnamed_83 ? fragment_unnamed_52 : 0.0f;
				fragment_unnamed_27 = (fragment_unnamed_27 * fragment_unnamed_39) + fragment_unnamed_52;
				float fragment_unnamed_114;
				if (fragment_unnamed_96)
				{
					fragment_unnamed_114 = -fragment_unnamed_27;
				}
				else
				{
					fragment_unnamed_114 = fragment_unnamed_27;
				}
				fragment_unnamed_9.x = fragment_unnamed_114;
				fragment_unnamed_9.y = fragment_unnamed_9.x * 0.6366202831268310546875f;
				fragment_unnamed_39 = max(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_39 = 1.0f / fragment_unnamed_39;
				fragment_unnamed_52 = min(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_39 *= fragment_unnamed_52;
				fragment_unnamed_52 = fragment_unnamed_39 * fragment_unnamed_39;
				fragment_unnamed_151 = (fragment_unnamed_52 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_151 = (fragment_unnamed_52 * fragment_unnamed_151) + 0.1801410019397735595703125f;
				fragment_unnamed_151 = (fragment_unnamed_52 * fragment_unnamed_151) + (-0.33029949665069580078125f);
				fragment_unnamed_52 = (fragment_unnamed_52 * fragment_unnamed_151) + 0.999866008758544921875f;
				fragment_unnamed_151 = fragment_unnamed_52 * fragment_unnamed_39;
				fragment_unnamed_151 = (fragment_unnamed_151 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_173 = abs(fragment_input_0.x) < abs(fragment_input_0.z);
				fragment_unnamed_151 = fragment_unnamed_173 ? fragment_unnamed_151 : 0.0f;
				fragment_unnamed_39 = (fragment_unnamed_39 * fragment_unnamed_52) + fragment_unnamed_151;
				fragment_unnamed_9.x = fragment_unnamed_39 * 0.6366202831268310546875f;
				fragment_unnamed_9 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_9.xy).xyz;
				fragment_unnamed_206 = fragment_input_0.y >= 0.0f;
				fragment_unnamed_52 = float(fragment_unnamed_206);
				fragment_output_0 = fragment_unnamed_52.xxxx * fragment_unnamed_9.xyzx;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // QUAD_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL


			#ifdef FULL_HORIZONTAL
			#ifdef FULL_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[3];
			static float4 vertex_uniform_buffer_1[4];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;
			static float4 vertex_output_2;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = mad(vertex_input_1.x, vertex_uniform_buffer_0[2u].x, vertex_uniform_buffer_0[2u].z);
				vertex_output_0.y = mad(vertex_input_1.y, vertex_uniform_buffer_0[2u].y, vertex_uniform_buffer_0[2u].w);
				precise float vertex_unnamed_64 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_65 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_64));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_65));
				float vertex_unnamed_90 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_66));
				float vertex_unnamed_91 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_67));
				precise float vertex_unnamed_98 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_99 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_100 = vertex_unnamed_90 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_101 = vertex_unnamed_91 + vertex_uniform_buffer_1[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_88);
				vertex_output_2.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_2.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_90);
				vertex_output_2.w = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_91);
				precise float vertex_unnamed_125 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_126 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_127 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_128 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_98, vertex_unnamed_125)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_98, vertex_unnamed_126)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_98, vertex_unnamed_127)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_98, vertex_unnamed_128)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[2] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				return stage_output;
			}

			#endif // FULL_HORIZONTAL
			#endif // FULL_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef FULL_HORIZONTAL
			#ifdef FULL_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_1;
			static float2 vertex_input_1;
			static float4 vertex_input_0;
			static float4 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float4 vertex_output_0 : TEXCOORD1; // vs_TEXCOORD1
				float2 vertex_output_1 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_33;
			static float4 vertex_unnamed_57;

			void vert_main()
			{
				vertex_output_1 = (vertex_input_1 * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_33 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_33 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_33;
				vertex_unnamed_57 = vertex_unnamed_33 + unity_ObjectToWorld__array[3];
				vertex_output_0 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_33;
				vertex_unnamed_33 = vertex_unnamed_57.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_33 = (unity_MatrixVP__array[0] * vertex_unnamed_57.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_MatrixVP__array[2] * vertex_unnamed_57.zzzz) + vertex_unnamed_33;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_57.wwww) + vertex_unnamed_33;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_0 = stage_input.vertex_input_0;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float4 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float4 fragment_input_0 : TEXCOORD1; // vs_TEXCOORD1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float fragment_unnamed_31;
			static float fragment_unnamed_49;
			static bool fragment_unnamed_81;
			static bool fragment_unnamed_100;
			static bool fragment_unnamed_129;
			static float fragment_unnamed_163;
			static float fragment_unnamed_172;
			static float fragment_unnamed_184;
			static bool fragment_unnamed_206;

			void frag_main()
			{
				fragment_unnamed_9.x = max(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x = 1.0f / fragment_unnamed_9.x;
				fragment_unnamed_31 = min(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x *= fragment_unnamed_31;
				fragment_unnamed_31 = fragment_unnamed_9.x * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_31 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.1801410019397735595703125f;
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + (-0.33029949665069580078125f);
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.999866008758544921875f;
				fragment_unnamed_49 = fragment_unnamed_31 * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_49 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_81 = abs(fragment_input_0.x) < abs(fragment_input_0.z);
				fragment_unnamed_49 = fragment_unnamed_81 ? fragment_unnamed_49 : 0.0f;
				fragment_unnamed_9.x = (fragment_unnamed_9.x * fragment_unnamed_31) + fragment_unnamed_49;
				fragment_unnamed_100 = fragment_input_0.x < (-fragment_input_0.x);
				fragment_unnamed_31 = fragment_unnamed_100 ? (-3.1415927410125732421875f) : 0.0f;
				fragment_unnamed_9.x = fragment_unnamed_31 + fragment_unnamed_9.x;
				fragment_unnamed_31 = min(fragment_input_0.x, fragment_input_0.z);
				fragment_unnamed_100 = fragment_unnamed_31 < (-fragment_unnamed_31);
				fragment_unnamed_49 = max(fragment_input_0.x, fragment_input_0.z);
				fragment_unnamed_129 = fragment_unnamed_49 >= (-fragment_unnamed_49);
				fragment_unnamed_100 = fragment_unnamed_129 && fragment_unnamed_100;
				float fragment_unnamed_139;
				if (fragment_unnamed_100)
				{
					fragment_unnamed_139 = -fragment_unnamed_9.x;
				}
				else
				{
					fragment_unnamed_139 = fragment_unnamed_9.x;
				}
				fragment_unnamed_9.x = fragment_unnamed_139;
				fragment_unnamed_9.x += 3.141590118408203125f;
				fragment_unnamed_49 = dot(fragment_input_0.xz, fragment_input_0.xz);
				fragment_unnamed_49 = sqrt(fragment_unnamed_49);
				fragment_unnamed_163 = max(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_163 = 1.0f / fragment_unnamed_163;
				fragment_unnamed_172 = min(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_163 *= fragment_unnamed_172;
				fragment_unnamed_172 = fragment_unnamed_163 * fragment_unnamed_163;
				fragment_unnamed_184 = (fragment_unnamed_172 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_184 = (fragment_unnamed_172 * fragment_unnamed_184) + 0.1801410019397735595703125f;
				fragment_unnamed_184 = (fragment_unnamed_172 * fragment_unnamed_184) + (-0.33029949665069580078125f);
				fragment_unnamed_172 = (fragment_unnamed_172 * fragment_unnamed_184) + 0.999866008758544921875f;
				fragment_unnamed_184 = fragment_unnamed_163 * fragment_unnamed_172;
				fragment_unnamed_184 = (fragment_unnamed_184 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_206 = fragment_unnamed_49 < abs(fragment_input_0.y);
				fragment_unnamed_49 = min(fragment_unnamed_49, fragment_input_0.y);
				fragment_unnamed_129 = fragment_unnamed_49 < (-fragment_unnamed_49);
				fragment_unnamed_184 = fragment_unnamed_206 ? fragment_unnamed_184 : 0.0f;
				fragment_unnamed_163 = (fragment_unnamed_163 * fragment_unnamed_172) + fragment_unnamed_184;
				float fragment_unnamed_229;
				if (fragment_unnamed_129)
				{
					fragment_unnamed_229 = -fragment_unnamed_163;
				}
				else
				{
					fragment_unnamed_229 = fragment_unnamed_163;
				}
				fragment_unnamed_49 = fragment_unnamed_229;
				fragment_unnamed_9.z = fragment_unnamed_49 + 1.5707950592041015625f;
				float2 fragment_unnamed_246 = fragment_unnamed_9.xz * float2(0.159155070781707763671875f, 0.31831014156341552734375f);
				fragment_unnamed_9 = float3(fragment_unnamed_246.x, fragment_unnamed_246.y, fragment_unnamed_9.z);
				fragment_unnamed_9 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_9.xy).xyz;
				fragment_output_0 = fragment_unnamed_9.xyzx;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FULL_HORIZONTAL
			#endif // FULL_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef FULL_VERTICAL
			#ifdef HALF_HORIZONTAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[3];
			static float4 vertex_uniform_buffer_1[4];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;
			static float4 vertex_output_2;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = mad(vertex_input_1.x, vertex_uniform_buffer_0[2u].x, vertex_uniform_buffer_0[2u].z);
				vertex_output_0.y = mad(vertex_input_1.y, vertex_uniform_buffer_0[2u].y, vertex_uniform_buffer_0[2u].w);
				precise float vertex_unnamed_64 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_65 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_64));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_65));
				float vertex_unnamed_90 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_66));
				float vertex_unnamed_91 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_67));
				precise float vertex_unnamed_98 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_99 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_100 = vertex_unnamed_90 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_101 = vertex_unnamed_91 + vertex_uniform_buffer_1[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_88);
				vertex_output_2.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_2.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_90);
				vertex_output_2.w = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_91);
				precise float vertex_unnamed_125 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_126 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_127 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_128 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_98, vertex_unnamed_125)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_98, vertex_unnamed_126)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_98, vertex_unnamed_127)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_98, vertex_unnamed_128)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[2] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				return stage_output;
			}

			#endif // FULL_VERTICAL
			#endif // HALF_HORIZONTAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef FULL_VERTICAL
			#ifdef HALF_HORIZONTAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_1;
			static float2 vertex_input_1;
			static float4 vertex_input_0;
			static float4 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float4 vertex_output_0 : TEXCOORD1; // vs_TEXCOORD1
				float2 vertex_output_1 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_33;
			static float4 vertex_unnamed_57;

			void vert_main()
			{
				vertex_output_1 = (vertex_input_1 * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_33 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_33 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_33;
				vertex_unnamed_57 = vertex_unnamed_33 + unity_ObjectToWorld__array[3];
				vertex_output_0 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_33;
				vertex_unnamed_33 = vertex_unnamed_57.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_33 = (unity_MatrixVP__array[0] * vertex_unnamed_57.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_MatrixVP__array[2] * vertex_unnamed_57.zzzz) + vertex_unnamed_33;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_57.wwww) + vertex_unnamed_33;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_0 = stage_input.vertex_input_0;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float4 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float4 fragment_input_0 : TEXCOORD1; // vs_TEXCOORD1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float fragment_unnamed_31;
			static float fragment_unnamed_49;
			static bool fragment_unnamed_81;
			static bool fragment_unnamed_100;
			static float fragment_unnamed_146;
			static float fragment_unnamed_155;
			static float fragment_unnamed_167;
			static bool fragment_unnamed_189;
			static bool fragment_unnamed_199;

			void frag_main()
			{
				fragment_unnamed_9.x = max(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x = 1.0f / fragment_unnamed_9.x;
				fragment_unnamed_31 = min(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_9.x *= fragment_unnamed_31;
				fragment_unnamed_31 = fragment_unnamed_9.x * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_31 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.1801410019397735595703125f;
				fragment_unnamed_49 = (fragment_unnamed_31 * fragment_unnamed_49) + (-0.33029949665069580078125f);
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_49) + 0.999866008758544921875f;
				fragment_unnamed_49 = fragment_unnamed_31 * fragment_unnamed_9.x;
				fragment_unnamed_49 = (fragment_unnamed_49 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_81 = abs(fragment_input_0.x) < abs(fragment_input_0.z);
				fragment_unnamed_49 = fragment_unnamed_81 ? fragment_unnamed_49 : 0.0f;
				fragment_unnamed_9.x = (fragment_unnamed_9.x * fragment_unnamed_31) + fragment_unnamed_49;
				fragment_unnamed_100 = fragment_input_0.x < (-fragment_input_0.x);
				fragment_unnamed_31 = fragment_unnamed_100 ? (-3.1415927410125732421875f) : 0.0f;
				fragment_unnamed_9.x = fragment_unnamed_31 + fragment_unnamed_9.x;
				fragment_unnamed_31 = min(fragment_input_0.x, abs(fragment_input_0.z));
				fragment_unnamed_100 = fragment_unnamed_31 < (-fragment_unnamed_31);
				float fragment_unnamed_127;
				if (fragment_unnamed_100)
				{
					fragment_unnamed_127 = -fragment_unnamed_9.x;
				}
				else
				{
					fragment_unnamed_127 = fragment_unnamed_9.x;
				}
				fragment_unnamed_9.x = fragment_unnamed_127;
				fragment_unnamed_49 = dot(fragment_input_0.xz, fragment_input_0.xz);
				fragment_unnamed_49 = sqrt(fragment_unnamed_49);
				fragment_unnamed_146 = max(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_146 = 1.0f / fragment_unnamed_146;
				fragment_unnamed_155 = min(fragment_unnamed_49, abs(fragment_input_0.y));
				fragment_unnamed_146 *= fragment_unnamed_155;
				fragment_unnamed_155 = fragment_unnamed_146 * fragment_unnamed_146;
				fragment_unnamed_167 = (fragment_unnamed_155 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_167 = (fragment_unnamed_155 * fragment_unnamed_167) + 0.1801410019397735595703125f;
				fragment_unnamed_167 = (fragment_unnamed_155 * fragment_unnamed_167) + (-0.33029949665069580078125f);
				fragment_unnamed_155 = (fragment_unnamed_155 * fragment_unnamed_167) + 0.999866008758544921875f;
				fragment_unnamed_167 = fragment_unnamed_146 * fragment_unnamed_155;
				fragment_unnamed_167 = (fragment_unnamed_167 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_189 = fragment_unnamed_49 < abs(fragment_input_0.y);
				fragment_unnamed_49 = min(fragment_unnamed_49, fragment_input_0.y);
				fragment_unnamed_199 = fragment_unnamed_49 < (-fragment_unnamed_49);
				fragment_unnamed_167 = fragment_unnamed_189 ? fragment_unnamed_167 : 0.0f;
				fragment_unnamed_146 = (fragment_unnamed_146 * fragment_unnamed_155) + fragment_unnamed_167;
				float fragment_unnamed_213;
				if (fragment_unnamed_199)
				{
					fragment_unnamed_213 = -fragment_unnamed_146;
				}
				else
				{
					fragment_unnamed_213 = fragment_unnamed_146;
				}
				fragment_unnamed_49 = fragment_unnamed_213;
				fragment_unnamed_9.z = fragment_unnamed_49 + 1.5707950592041015625f;
				float2 fragment_unnamed_229 = fragment_unnamed_9.xz * 0.31831014156341552734375f.xx;
				fragment_unnamed_9 = float3(fragment_unnamed_229.x, fragment_unnamed_229.y, fragment_unnamed_9.z);
				fragment_unnamed_9 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_9.xy).xyz;
				fragment_output_0 = fragment_unnamed_9.xyzx;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FULL_VERTICAL
			#endif // HALF_HORIZONTAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef FULL_VERTICAL
			#ifdef QUAD_HORIZONTAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef HALF_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4 _MainTex_ST;
			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[3];
			static float4 vertex_uniform_buffer_1[4];
			static float4 vertex_uniform_buffer_2[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;
			static float4 vertex_output_2;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : TEXCOORD1; // TEXCOORD_1
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = mad(vertex_input_1.x, vertex_uniform_buffer_0[2u].x, vertex_uniform_buffer_0[2u].z);
				vertex_output_0.y = mad(vertex_input_1.y, vertex_uniform_buffer_0[2u].y, vertex_uniform_buffer_0[2u].w);
				precise float vertex_unnamed_64 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_65 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_66 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_67 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_64));
				float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_65));
				float vertex_unnamed_90 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_66));
				float vertex_unnamed_91 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_67));
				precise float vertex_unnamed_98 = vertex_unnamed_88 + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_99 = vertex_unnamed_89 + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_100 = vertex_unnamed_90 + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_101 = vertex_unnamed_91 + vertex_uniform_buffer_1[3u].w;
				vertex_output_2.x = mad(vertex_uniform_buffer_1[3u].x, vertex_input_0.w, vertex_unnamed_88);
				vertex_output_2.y = mad(vertex_uniform_buffer_1[3u].y, vertex_input_0.w, vertex_unnamed_89);
				vertex_output_2.z = mad(vertex_uniform_buffer_1[3u].z, vertex_input_0.w, vertex_unnamed_90);
				vertex_output_2.w = mad(vertex_uniform_buffer_1[3u].w, vertex_input_0.w, vertex_unnamed_91);
				precise float vertex_unnamed_125 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].x;
				precise float vertex_unnamed_126 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].y;
				precise float vertex_unnamed_127 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].z;
				precise float vertex_unnamed_128 = vertex_unnamed_99 * vertex_uniform_buffer_2[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_2[20u].x, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].x, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].x, vertex_unnamed_98, vertex_unnamed_125)));
				gl_Position.y = mad(vertex_uniform_buffer_2[20u].y, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].y, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].y, vertex_unnamed_98, vertex_unnamed_126)));
				gl_Position.z = mad(vertex_uniform_buffer_2[20u].z, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].z, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].z, vertex_unnamed_98, vertex_unnamed_127)));
				gl_Position.w = mad(vertex_uniform_buffer_2[20u].w, vertex_unnamed_101, mad(vertex_uniform_buffer_2[19u].w, vertex_unnamed_100, mad(vertex_uniform_buffer_2[17u].w, vertex_unnamed_98, vertex_unnamed_128)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[2] = float4(_MainTex_ST[0], _MainTex_ST[1], _MainTex_ST[2], _MainTex_ST[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_2[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_2[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_2[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				stage_output.vertex_output_2 = vertex_output_2;
				return stage_output;
			}

			#endif // FULL_VERTICAL
			#endif // QUAD_HORIZONTAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !HALF_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef FULL_VERTICAL
			#ifdef QUAD_HORIZONTAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef HALF_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_1;
			static float2 vertex_input_1;
			static float4 vertex_input_0;
			static float4 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float4 vertex_output_0 : TEXCOORD1; // vs_TEXCOORD1
				float2 vertex_output_1 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_33;
			static float4 vertex_unnamed_57;

			void vert_main()
			{
				vertex_output_1 = (vertex_input_1 * _MainTex_ST.xy) + _MainTex_ST.zw;
				vertex_unnamed_33 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_33 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_33;
				vertex_unnamed_57 = vertex_unnamed_33 + unity_ObjectToWorld__array[3];
				vertex_output_0 = (unity_ObjectToWorld__array[3] * vertex_input_0.wwww) + vertex_unnamed_33;
				vertex_unnamed_33 = vertex_unnamed_57.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_33 = (unity_MatrixVP__array[0] * vertex_unnamed_57.xxxx) + vertex_unnamed_33;
				vertex_unnamed_33 = (unity_MatrixVP__array[2] * vertex_unnamed_57.zzzz) + vertex_unnamed_33;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_57.wwww) + vertex_unnamed_33;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				unity_ObjectToWorld__array[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				unity_ObjectToWorld__array[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				unity_ObjectToWorld__array[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				unity_ObjectToWorld__array[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				unity_MatrixVP__array[0] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				unity_MatrixVP__array[1] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				unity_MatrixVP__array[2] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				unity_MatrixVP__array[3] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_0 = stage_input.vertex_input_0;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float4 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float4 fragment_input_0 : TEXCOORD1; // vs_TEXCOORD1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float3 fragment_unnamed_9;
			static float fragment_unnamed_27;
			static float fragment_unnamed_39;
			static float fragment_unnamed_52;
			static bool fragment_unnamed_83;
			static bool fragment_unnamed_96;
			static float fragment_unnamed_156;
			static bool fragment_unnamed_178;

			void frag_main()
			{
				fragment_unnamed_9.x = dot(fragment_input_0.xz, fragment_input_0.xz);
				fragment_unnamed_9.x = sqrt(fragment_unnamed_9.x);
				fragment_unnamed_27 = max(fragment_unnamed_9.x, abs(fragment_input_0.y));
				fragment_unnamed_27 = 1.0f / fragment_unnamed_27;
				fragment_unnamed_39 = min(fragment_unnamed_9.x, abs(fragment_input_0.y));
				fragment_unnamed_27 *= fragment_unnamed_39;
				fragment_unnamed_39 = fragment_unnamed_27 * fragment_unnamed_27;
				fragment_unnamed_52 = (fragment_unnamed_39 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_52 = (fragment_unnamed_39 * fragment_unnamed_52) + 0.1801410019397735595703125f;
				fragment_unnamed_52 = (fragment_unnamed_39 * fragment_unnamed_52) + (-0.33029949665069580078125f);
				fragment_unnamed_39 = (fragment_unnamed_39 * fragment_unnamed_52) + 0.999866008758544921875f;
				fragment_unnamed_52 = fragment_unnamed_39 * fragment_unnamed_27;
				fragment_unnamed_52 = (fragment_unnamed_52 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_83 = fragment_unnamed_9.x < abs(fragment_input_0.y);
				fragment_unnamed_9.x = min(fragment_unnamed_9.x, fragment_input_0.y);
				fragment_unnamed_96 = fragment_unnamed_9.x < (-fragment_unnamed_9.x);
				fragment_unnamed_52 = fragment_unnamed_83 ? fragment_unnamed_52 : 0.0f;
				fragment_unnamed_27 = (fragment_unnamed_27 * fragment_unnamed_39) + fragment_unnamed_52;
				float fragment_unnamed_114;
				if (fragment_unnamed_96)
				{
					fragment_unnamed_114 = -fragment_unnamed_27;
				}
				else
				{
					fragment_unnamed_114 = fragment_unnamed_27;
				}
				fragment_unnamed_9.x = fragment_unnamed_114;
				fragment_unnamed_9.x += 1.5707950592041015625f;
				fragment_unnamed_9.y = fragment_unnamed_9.x * 0.31831014156341552734375f;
				fragment_unnamed_39 = max(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_39 = 1.0f / fragment_unnamed_39;
				fragment_unnamed_52 = min(abs(fragment_input_0.x), abs(fragment_input_0.z));
				fragment_unnamed_39 *= fragment_unnamed_52;
				fragment_unnamed_52 = fragment_unnamed_39 * fragment_unnamed_39;
				fragment_unnamed_156 = (fragment_unnamed_52 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_156 = (fragment_unnamed_52 * fragment_unnamed_156) + 0.1801410019397735595703125f;
				fragment_unnamed_156 = (fragment_unnamed_52 * fragment_unnamed_156) + (-0.33029949665069580078125f);
				fragment_unnamed_52 = (fragment_unnamed_52 * fragment_unnamed_156) + 0.999866008758544921875f;
				fragment_unnamed_156 = fragment_unnamed_52 * fragment_unnamed_39;
				fragment_unnamed_156 = (fragment_unnamed_156 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_178 = abs(fragment_input_0.x) < abs(fragment_input_0.z);
				fragment_unnamed_156 = fragment_unnamed_178 ? fragment_unnamed_156 : 0.0f;
				fragment_unnamed_39 = (fragment_unnamed_39 * fragment_unnamed_52) + fragment_unnamed_156;
				fragment_unnamed_9.x = fragment_unnamed_39 * 0.6366202831268310546875f;
				fragment_unnamed_9 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_9.xy).xyz;
				fragment_output_0 = fragment_unnamed_9.xyzx;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FULL_VERTICAL
			#endif // QUAD_HORIZONTAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !HALF_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef BOTTOM_VERTICAL
			#ifdef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_35 = 1.0f / max(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_44 = fragment_unnamed_35 * min(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_45 = fragment_unnamed_44 * fragment_unnamed_44;
				float fragment_unnamed_53 = mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_55 = fragment_unnamed_53 * fragment_unnamed_44;
				precise float fragment_unnamed_77 = (-0.0f) - fragment_input_2.x;
				precise float fragment_unnamed_84 = asfloat(((fragment_input_2.x < fragment_unnamed_77) ? 4294967295u : 0u) & 3226013659u) + mad(fragment_unnamed_44, fragment_unnamed_53, asfloat(((abs(fragment_input_2.x) < abs(fragment_input_2.z)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_55, -2.0f, 1.57079637050628662109375f))));
				float fragment_unnamed_89 = min(fragment_input_2.x, fragment_input_2.z);
				precise float fragment_unnamed_90 = (-0.0f) - fragment_unnamed_89;
				float fragment_unnamed_97 = max(fragment_input_2.x, fragment_input_2.z);
				precise float fragment_unnamed_98 = (-0.0f) - fragment_unnamed_97;
				precise float fragment_unnamed_103 = (-0.0f) - fragment_unnamed_84;
				precise float fragment_unnamed_105 = (((((fragment_unnamed_97 >= fragment_unnamed_98) ? 4294967295u : 0u) & ((fragment_unnamed_89 < fragment_unnamed_90) ? 4294967295u : 0u)) != 0u) ? fragment_unnamed_103 : fragment_unnamed_84) + 3.141590118408203125f;
				precise float fragment_unnamed_107 = fragment_unnamed_105 * 0.159155070781707763671875f;
				float fragment_unnamed_120 = sqrt(dot(float2(fragment_input_2.x, fragment_input_2.z), float2(fragment_input_2.x, fragment_input_2.z)));
				precise float fragment_unnamed_126 = 1.0f / max(fragment_unnamed_120, abs(fragment_input_2.y));
				precise float fragment_unnamed_131 = fragment_unnamed_126 * min(fragment_unnamed_120, abs(fragment_input_2.y));
				precise float fragment_unnamed_132 = fragment_unnamed_131 * fragment_unnamed_131;
				float fragment_unnamed_136 = mad(fragment_unnamed_132, mad(fragment_unnamed_132, mad(fragment_unnamed_132, mad(fragment_unnamed_132, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_137 = fragment_unnamed_131 * fragment_unnamed_136;
				float fragment_unnamed_146 = min(fragment_unnamed_120, fragment_input_2.y);
				precise float fragment_unnamed_147 = (-0.0f) - fragment_unnamed_146;
				float fragment_unnamed_152 = mad(fragment_unnamed_131, fragment_unnamed_136, asfloat(((fragment_unnamed_120 < abs(fragment_input_2.y)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_137, -2.0f, 1.57079637050628662109375f))));
				precise float fragment_unnamed_153 = (-0.0f) - fragment_unnamed_152;
				precise float fragment_unnamed_155 = (-0.0f) - ((fragment_unnamed_146 < fragment_unnamed_147) ? fragment_unnamed_153 : fragment_unnamed_152);
				float4 fragment_unnamed_161 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_107, mad(fragment_unnamed_155, -0.6366202831268310546875f, 1.0f)));
				float fragment_unnamed_163 = fragment_unnamed_161.x;
				float fragment_unnamed_172 = asfloat(((0.0f >= fragment_input_2.y) ? 4294967295u : 0u) & 1065353216u);
				precise float fragment_unnamed_173 = fragment_unnamed_172 * fragment_unnamed_163;
				precise float fragment_unnamed_174 = fragment_unnamed_172 * fragment_unnamed_161.y;
				precise float fragment_unnamed_175 = fragment_unnamed_172 * fragment_unnamed_161.z;
				precise float fragment_unnamed_176 = fragment_unnamed_172 * fragment_unnamed_163;
				fragment_output_0.x = fragment_unnamed_173;
				fragment_output_0.y = fragment_unnamed_174;
				fragment_output_0.z = fragment_unnamed_175;
				fragment_output_0.w = fragment_unnamed_176;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // BOTTOM_VERTICAL
			#endif // FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef BOTTOM_VERTICAL
			#ifdef HALF_HORIZONTAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_35 = 1.0f / max(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_44 = fragment_unnamed_35 * min(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_45 = fragment_unnamed_44 * fragment_unnamed_44;
				float fragment_unnamed_53 = mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_55 = fragment_unnamed_53 * fragment_unnamed_44;
				precise float fragment_unnamed_77 = (-0.0f) - fragment_input_2.x;
				precise float fragment_unnamed_84 = asfloat(((fragment_input_2.x < fragment_unnamed_77) ? 4294967295u : 0u) & 3226013659u) + mad(fragment_unnamed_44, fragment_unnamed_53, asfloat(((abs(fragment_input_2.x) < abs(fragment_input_2.z)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_55, -2.0f, 1.57079637050628662109375f))));
				float fragment_unnamed_90 = min(fragment_input_2.x, abs(fragment_input_2.z));
				precise float fragment_unnamed_91 = (-0.0f) - fragment_unnamed_90;
				precise float fragment_unnamed_93 = (-0.0f) - fragment_unnamed_84;
				precise float fragment_unnamed_95 = ((fragment_unnamed_90 < fragment_unnamed_91) ? fragment_unnamed_93 : fragment_unnamed_84) * 0.31831014156341552734375f;
				float fragment_unnamed_108 = sqrt(dot(float2(fragment_input_2.x, fragment_input_2.z), float2(fragment_input_2.x, fragment_input_2.z)));
				precise float fragment_unnamed_114 = 1.0f / max(fragment_unnamed_108, abs(fragment_input_2.y));
				precise float fragment_unnamed_119 = fragment_unnamed_114 * min(fragment_unnamed_108, abs(fragment_input_2.y));
				precise float fragment_unnamed_120 = fragment_unnamed_119 * fragment_unnamed_119;
				float fragment_unnamed_124 = mad(fragment_unnamed_120, mad(fragment_unnamed_120, mad(fragment_unnamed_120, mad(fragment_unnamed_120, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_125 = fragment_unnamed_119 * fragment_unnamed_124;
				float fragment_unnamed_134 = min(fragment_unnamed_108, fragment_input_2.y);
				precise float fragment_unnamed_135 = (-0.0f) - fragment_unnamed_134;
				float fragment_unnamed_140 = mad(fragment_unnamed_119, fragment_unnamed_124, asfloat(((fragment_unnamed_108 < abs(fragment_input_2.y)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_125, -2.0f, 1.57079637050628662109375f))));
				precise float fragment_unnamed_141 = (-0.0f) - fragment_unnamed_140;
				precise float fragment_unnamed_143 = (-0.0f) - ((fragment_unnamed_134 < fragment_unnamed_135) ? fragment_unnamed_141 : fragment_unnamed_140);
				float4 fragment_unnamed_149 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_95, mad(fragment_unnamed_143, -0.6366202831268310546875f, 1.0f)));
				float fragment_unnamed_151 = fragment_unnamed_149.x;
				float fragment_unnamed_160 = asfloat(((0.0f >= fragment_input_2.y) ? 4294967295u : 0u) & 1065353216u);
				precise float fragment_unnamed_161 = fragment_unnamed_160 * fragment_unnamed_151;
				precise float fragment_unnamed_162 = fragment_unnamed_160 * fragment_unnamed_149.y;
				precise float fragment_unnamed_163 = fragment_unnamed_160 * fragment_unnamed_149.z;
				precise float fragment_unnamed_164 = fragment_unnamed_160 * fragment_unnamed_151;
				fragment_output_0.x = fragment_unnamed_161;
				fragment_output_0.y = fragment_unnamed_162;
				fragment_output_0.z = fragment_unnamed_163;
				fragment_output_0.w = fragment_unnamed_164;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // BOTTOM_VERTICAL
			#endif // HALF_HORIZONTAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef BOTTOM_VERTICAL
			#ifdef QUAD_HORIZONTAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float fragment_unnamed_39 = sqrt(dot(float2(fragment_input_2.x, fragment_input_2.z), float2(fragment_input_2.x, fragment_input_2.z)));
				precise float fragment_unnamed_45 = 1.0f / max(fragment_unnamed_39, abs(fragment_input_2.y));
				precise float fragment_unnamed_51 = fragment_unnamed_45 * min(fragment_unnamed_39, abs(fragment_input_2.y));
				precise float fragment_unnamed_52 = fragment_unnamed_51 * fragment_unnamed_51;
				float fragment_unnamed_60 = mad(fragment_unnamed_52, mad(fragment_unnamed_52, mad(fragment_unnamed_52, mad(fragment_unnamed_52, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_62 = fragment_unnamed_60 * fragment_unnamed_51;
				float fragment_unnamed_75 = min(fragment_unnamed_39, fragment_input_2.y);
				precise float fragment_unnamed_76 = (-0.0f) - fragment_unnamed_75;
				float fragment_unnamed_82 = mad(fragment_unnamed_51, fragment_unnamed_60, asfloat(asuint(mad(fragment_unnamed_62, -2.0f, 1.57079637050628662109375f)) & ((fragment_unnamed_39 < abs(fragment_input_2.y)) ? 4294967295u : 0u)));
				precise float fragment_unnamed_83 = (-0.0f) - fragment_unnamed_82;
				precise float fragment_unnamed_85 = (-0.0f) - ((fragment_unnamed_75 < fragment_unnamed_76) ? fragment_unnamed_83 : fragment_unnamed_82);
				precise float fragment_unnamed_95 = 1.0f / max(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_103 = fragment_unnamed_95 * min(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_104 = fragment_unnamed_103 * fragment_unnamed_103;
				float fragment_unnamed_108 = mad(fragment_unnamed_104, mad(fragment_unnamed_104, mad(fragment_unnamed_104, mad(fragment_unnamed_104, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_109 = fragment_unnamed_108 * fragment_unnamed_103;
				precise float fragment_unnamed_123 = mad(fragment_unnamed_103, fragment_unnamed_108, asfloat(((abs(fragment_input_2.x) < abs(fragment_input_2.z)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_109, -2.0f, 1.57079637050628662109375f)))) * 0.6366202831268310546875f;
				float4 fragment_unnamed_128 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_123, mad(fragment_unnamed_85, -0.6366202831268310546875f, 1.0f)));
				float fragment_unnamed_130 = fragment_unnamed_128.x;
				float fragment_unnamed_139 = asfloat(((0.0f >= fragment_input_2.y) ? 4294967295u : 0u) & 1065353216u);
				precise float fragment_unnamed_140 = fragment_unnamed_139 * fragment_unnamed_130;
				precise float fragment_unnamed_141 = fragment_unnamed_139 * fragment_unnamed_128.y;
				precise float fragment_unnamed_142 = fragment_unnamed_139 * fragment_unnamed_128.z;
				precise float fragment_unnamed_143 = fragment_unnamed_139 * fragment_unnamed_130;
				fragment_output_0.x = fragment_unnamed_140;
				fragment_output_0.y = fragment_unnamed_141;
				fragment_output_0.z = fragment_unnamed_142;
				fragment_output_0.w = fragment_unnamed_143;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // BOTTOM_VERTICAL
			#endif // QUAD_HORIZONTAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef FULL_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_35 = 1.0f / max(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_44 = fragment_unnamed_35 * min(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_45 = fragment_unnamed_44 * fragment_unnamed_44;
				float fragment_unnamed_53 = mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_55 = fragment_unnamed_53 * fragment_unnamed_44;
				precise float fragment_unnamed_77 = (-0.0f) - fragment_input_2.x;
				precise float fragment_unnamed_84 = asfloat(((fragment_input_2.x < fragment_unnamed_77) ? 4294967295u : 0u) & 3226013659u) + mad(fragment_unnamed_44, fragment_unnamed_53, asfloat(((abs(fragment_input_2.x) < abs(fragment_input_2.z)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_55, -2.0f, 1.57079637050628662109375f))));
				float fragment_unnamed_89 = min(fragment_input_2.x, fragment_input_2.z);
				precise float fragment_unnamed_90 = (-0.0f) - fragment_unnamed_89;
				float fragment_unnamed_97 = max(fragment_input_2.x, fragment_input_2.z);
				precise float fragment_unnamed_98 = (-0.0f) - fragment_unnamed_97;
				precise float fragment_unnamed_103 = (-0.0f) - fragment_unnamed_84;
				precise float fragment_unnamed_105 = (((((fragment_unnamed_97 >= fragment_unnamed_98) ? 4294967295u : 0u) & ((fragment_unnamed_89 < fragment_unnamed_90) ? 4294967295u : 0u)) != 0u) ? fragment_unnamed_103 : fragment_unnamed_84) + 3.141590118408203125f;
				float fragment_unnamed_118 = sqrt(dot(float2(fragment_input_2.x, fragment_input_2.z), float2(fragment_input_2.x, fragment_input_2.z)));
				precise float fragment_unnamed_124 = 1.0f / max(fragment_unnamed_118, abs(fragment_input_2.y));
				precise float fragment_unnamed_129 = fragment_unnamed_124 * min(fragment_unnamed_118, abs(fragment_input_2.y));
				precise float fragment_unnamed_130 = fragment_unnamed_129 * fragment_unnamed_129;
				float fragment_unnamed_134 = mad(fragment_unnamed_130, mad(fragment_unnamed_130, mad(fragment_unnamed_130, mad(fragment_unnamed_130, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_135 = fragment_unnamed_129 * fragment_unnamed_134;
				float fragment_unnamed_144 = min(fragment_unnamed_118, fragment_input_2.y);
				precise float fragment_unnamed_145 = (-0.0f) - fragment_unnamed_144;
				float fragment_unnamed_150 = mad(fragment_unnamed_129, fragment_unnamed_134, asfloat(((fragment_unnamed_118 < abs(fragment_input_2.y)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_135, -2.0f, 1.57079637050628662109375f))));
				precise float fragment_unnamed_151 = (-0.0f) - fragment_unnamed_150;
				precise float fragment_unnamed_153 = fragment_unnamed_105 * 0.159155070781707763671875f;
				precise float fragment_unnamed_155 = ((fragment_unnamed_144 < fragment_unnamed_145) ? fragment_unnamed_151 : fragment_unnamed_150) * 0.6366202831268310546875f;
				float4 fragment_unnamed_160 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_153, fragment_unnamed_155));
				float fragment_unnamed_162 = fragment_unnamed_160.x;
				float fragment_unnamed_171 = asfloat(((fragment_input_2.y >= 0.0f) ? 4294967295u : 0u) & 1065353216u);
				precise float fragment_unnamed_172 = fragment_unnamed_171 * fragment_unnamed_162;
				precise float fragment_unnamed_173 = fragment_unnamed_171 * fragment_unnamed_160.y;
				precise float fragment_unnamed_174 = fragment_unnamed_171 * fragment_unnamed_160.z;
				precise float fragment_unnamed_175 = fragment_unnamed_171 * fragment_unnamed_162;
				fragment_output_0.x = fragment_unnamed_172;
				fragment_output_0.y = fragment_unnamed_173;
				fragment_output_0.z = fragment_unnamed_174;
				fragment_output_0.w = fragment_unnamed_175;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FULL_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL


			#ifdef HALF_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_35 = 1.0f / max(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_44 = fragment_unnamed_35 * min(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_45 = fragment_unnamed_44 * fragment_unnamed_44;
				float fragment_unnamed_53 = mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_55 = fragment_unnamed_53 * fragment_unnamed_44;
				precise float fragment_unnamed_77 = (-0.0f) - fragment_input_2.x;
				precise float fragment_unnamed_84 = asfloat(((fragment_input_2.x < fragment_unnamed_77) ? 4294967295u : 0u) & 3226013659u) + mad(fragment_unnamed_44, fragment_unnamed_53, asfloat(((abs(fragment_input_2.x) < abs(fragment_input_2.z)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_55, -2.0f, 1.57079637050628662109375f))));
				float fragment_unnamed_90 = min(fragment_input_2.x, abs(fragment_input_2.z));
				precise float fragment_unnamed_91 = (-0.0f) - fragment_unnamed_90;
				precise float fragment_unnamed_93 = (-0.0f) - fragment_unnamed_84;
				float fragment_unnamed_106 = sqrt(dot(float2(fragment_input_2.x, fragment_input_2.z), float2(fragment_input_2.x, fragment_input_2.z)));
				precise float fragment_unnamed_112 = 1.0f / max(fragment_unnamed_106, abs(fragment_input_2.y));
				precise float fragment_unnamed_117 = fragment_unnamed_112 * min(fragment_unnamed_106, abs(fragment_input_2.y));
				precise float fragment_unnamed_118 = fragment_unnamed_117 * fragment_unnamed_117;
				float fragment_unnamed_122 = mad(fragment_unnamed_118, mad(fragment_unnamed_118, mad(fragment_unnamed_118, mad(fragment_unnamed_118, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_123 = fragment_unnamed_117 * fragment_unnamed_122;
				float fragment_unnamed_132 = min(fragment_unnamed_106, fragment_input_2.y);
				precise float fragment_unnamed_133 = (-0.0f) - fragment_unnamed_132;
				float fragment_unnamed_138 = mad(fragment_unnamed_117, fragment_unnamed_122, asfloat(((fragment_unnamed_106 < abs(fragment_input_2.y)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_123, -2.0f, 1.57079637050628662109375f))));
				precise float fragment_unnamed_139 = (-0.0f) - fragment_unnamed_138;
				precise float fragment_unnamed_141 = ((fragment_unnamed_90 < fragment_unnamed_91) ? fragment_unnamed_93 : fragment_unnamed_84) * 0.31831014156341552734375f;
				precise float fragment_unnamed_143 = ((fragment_unnamed_132 < fragment_unnamed_133) ? fragment_unnamed_139 : fragment_unnamed_138) * 0.6366202831268310546875f;
				float4 fragment_unnamed_148 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_141, fragment_unnamed_143));
				float fragment_unnamed_150 = fragment_unnamed_148.x;
				float fragment_unnamed_159 = asfloat(((fragment_input_2.y >= 0.0f) ? 4294967295u : 0u) & 1065353216u);
				precise float fragment_unnamed_160 = fragment_unnamed_159 * fragment_unnamed_150;
				precise float fragment_unnamed_161 = fragment_unnamed_159 * fragment_unnamed_148.y;
				precise float fragment_unnamed_162 = fragment_unnamed_159 * fragment_unnamed_148.z;
				precise float fragment_unnamed_163 = fragment_unnamed_159 * fragment_unnamed_150;
				fragment_output_0.x = fragment_unnamed_160;
				fragment_output_0.y = fragment_unnamed_161;
				fragment_output_0.z = fragment_unnamed_162;
				fragment_output_0.w = fragment_unnamed_163;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // HALF_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !QUAD_HORIZONTAL


			#ifdef QUAD_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef FULL_VERTICAL
			#ifndef HALF_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float fragment_unnamed_39 = sqrt(dot(float2(fragment_input_2.x, fragment_input_2.z), float2(fragment_input_2.x, fragment_input_2.z)));
				precise float fragment_unnamed_45 = 1.0f / max(fragment_unnamed_39, abs(fragment_input_2.y));
				precise float fragment_unnamed_51 = fragment_unnamed_45 * min(fragment_unnamed_39, abs(fragment_input_2.y));
				precise float fragment_unnamed_52 = fragment_unnamed_51 * fragment_unnamed_51;
				float fragment_unnamed_60 = mad(fragment_unnamed_52, mad(fragment_unnamed_52, mad(fragment_unnamed_52, mad(fragment_unnamed_52, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_62 = fragment_unnamed_60 * fragment_unnamed_51;
				float fragment_unnamed_75 = min(fragment_unnamed_39, fragment_input_2.y);
				precise float fragment_unnamed_76 = (-0.0f) - fragment_unnamed_75;
				float fragment_unnamed_82 = mad(fragment_unnamed_51, fragment_unnamed_60, asfloat(asuint(mad(fragment_unnamed_62, -2.0f, 1.57079637050628662109375f)) & ((fragment_unnamed_39 < abs(fragment_input_2.y)) ? 4294967295u : 0u)));
				precise float fragment_unnamed_83 = (-0.0f) - fragment_unnamed_82;
				precise float fragment_unnamed_85 = ((fragment_unnamed_75 < fragment_unnamed_76) ? fragment_unnamed_83 : fragment_unnamed_82) * 0.6366202831268310546875f;
				precise float fragment_unnamed_94 = 1.0f / max(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_102 = fragment_unnamed_94 * min(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_103 = fragment_unnamed_102 * fragment_unnamed_102;
				float fragment_unnamed_107 = mad(fragment_unnamed_103, mad(fragment_unnamed_103, mad(fragment_unnamed_103, mad(fragment_unnamed_103, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_108 = fragment_unnamed_107 * fragment_unnamed_102;
				precise float fragment_unnamed_122 = mad(fragment_unnamed_102, fragment_unnamed_107, asfloat(((abs(fragment_input_2.x) < abs(fragment_input_2.z)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_108, -2.0f, 1.57079637050628662109375f)))) * 0.6366202831268310546875f;
				float4 fragment_unnamed_126 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_122, fragment_unnamed_85));
				float fragment_unnamed_128 = fragment_unnamed_126.x;
				float fragment_unnamed_137 = asfloat(((fragment_input_2.y >= 0.0f) ? 4294967295u : 0u) & 1065353216u);
				precise float fragment_unnamed_138 = fragment_unnamed_137 * fragment_unnamed_128;
				precise float fragment_unnamed_139 = fragment_unnamed_137 * fragment_unnamed_126.y;
				precise float fragment_unnamed_140 = fragment_unnamed_137 * fragment_unnamed_126.z;
				precise float fragment_unnamed_141 = fragment_unnamed_137 * fragment_unnamed_128;
				fragment_output_0.x = fragment_unnamed_138;
				fragment_output_0.y = fragment_unnamed_139;
				fragment_output_0.z = fragment_unnamed_140;
				fragment_output_0.w = fragment_unnamed_141;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // QUAD_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !FULL_VERTICAL
			#endif // !HALF_HORIZONTAL


			#ifdef FULL_HORIZONTAL
			#ifdef FULL_VERTICAL
			#ifndef BOTTOM_VERTICAL
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_35 = 1.0f / max(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_44 = fragment_unnamed_35 * min(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_45 = fragment_unnamed_44 * fragment_unnamed_44;
				float fragment_unnamed_53 = mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_55 = fragment_unnamed_53 * fragment_unnamed_44;
				precise float fragment_unnamed_77 = (-0.0f) - fragment_input_2.x;
				precise float fragment_unnamed_84 = asfloat(((fragment_input_2.x < fragment_unnamed_77) ? 4294967295u : 0u) & 3226013659u) + mad(fragment_unnamed_44, fragment_unnamed_53, asfloat(((abs(fragment_input_2.x) < abs(fragment_input_2.z)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_55, -2.0f, 1.57079637050628662109375f))));
				float fragment_unnamed_89 = min(fragment_input_2.x, fragment_input_2.z);
				precise float fragment_unnamed_90 = (-0.0f) - fragment_unnamed_89;
				float fragment_unnamed_97 = max(fragment_input_2.x, fragment_input_2.z);
				precise float fragment_unnamed_98 = (-0.0f) - fragment_unnamed_97;
				precise float fragment_unnamed_103 = (-0.0f) - fragment_unnamed_84;
				precise float fragment_unnamed_105 = (((((fragment_unnamed_97 >= fragment_unnamed_98) ? 4294967295u : 0u) & ((fragment_unnamed_89 < fragment_unnamed_90) ? 4294967295u : 0u)) != 0u) ? fragment_unnamed_103 : fragment_unnamed_84) + 3.141590118408203125f;
				float fragment_unnamed_118 = sqrt(dot(float2(fragment_input_2.x, fragment_input_2.z), float2(fragment_input_2.x, fragment_input_2.z)));
				precise float fragment_unnamed_124 = 1.0f / max(fragment_unnamed_118, abs(fragment_input_2.y));
				precise float fragment_unnamed_129 = fragment_unnamed_124 * min(fragment_unnamed_118, abs(fragment_input_2.y));
				precise float fragment_unnamed_130 = fragment_unnamed_129 * fragment_unnamed_129;
				float fragment_unnamed_134 = mad(fragment_unnamed_130, mad(fragment_unnamed_130, mad(fragment_unnamed_130, mad(fragment_unnamed_130, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_135 = fragment_unnamed_129 * fragment_unnamed_134;
				float fragment_unnamed_144 = min(fragment_unnamed_118, fragment_input_2.y);
				precise float fragment_unnamed_145 = (-0.0f) - fragment_unnamed_144;
				float fragment_unnamed_150 = mad(fragment_unnamed_129, fragment_unnamed_134, asfloat(((fragment_unnamed_118 < abs(fragment_input_2.y)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_135, -2.0f, 1.57079637050628662109375f))));
				precise float fragment_unnamed_151 = (-0.0f) - fragment_unnamed_150;
				precise float fragment_unnamed_153 = ((fragment_unnamed_144 < fragment_unnamed_145) ? fragment_unnamed_151 : fragment_unnamed_150) + 1.5707950592041015625f;
				precise float fragment_unnamed_155 = fragment_unnamed_105 * 0.159155070781707763671875f;
				precise float fragment_unnamed_157 = fragment_unnamed_153 * 0.31831014156341552734375f;
				float4 fragment_unnamed_162 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_155, fragment_unnamed_157));
				float fragment_unnamed_164 = fragment_unnamed_162.x;
				fragment_output_0.x = fragment_unnamed_164;
				fragment_output_0.y = fragment_unnamed_162.y;
				fragment_output_0.z = fragment_unnamed_162.z;
				fragment_output_0.w = fragment_unnamed_164;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FULL_HORIZONTAL
			#endif // FULL_VERTICAL
			#endif // !BOTTOM_VERTICAL
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef FULL_VERTICAL
			#ifdef HALF_HORIZONTAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_35 = 1.0f / max(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_44 = fragment_unnamed_35 * min(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_45 = fragment_unnamed_44 * fragment_unnamed_44;
				float fragment_unnamed_53 = mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, mad(fragment_unnamed_45, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_55 = fragment_unnamed_53 * fragment_unnamed_44;
				precise float fragment_unnamed_77 = (-0.0f) - fragment_input_2.x;
				precise float fragment_unnamed_84 = asfloat(((fragment_input_2.x < fragment_unnamed_77) ? 4294967295u : 0u) & 3226013659u) + mad(fragment_unnamed_44, fragment_unnamed_53, asfloat(((abs(fragment_input_2.x) < abs(fragment_input_2.z)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_55, -2.0f, 1.57079637050628662109375f))));
				float fragment_unnamed_90 = min(fragment_input_2.x, abs(fragment_input_2.z));
				precise float fragment_unnamed_91 = (-0.0f) - fragment_unnamed_90;
				precise float fragment_unnamed_93 = (-0.0f) - fragment_unnamed_84;
				float fragment_unnamed_106 = sqrt(dot(float2(fragment_input_2.x, fragment_input_2.z), float2(fragment_input_2.x, fragment_input_2.z)));
				precise float fragment_unnamed_112 = 1.0f / max(fragment_unnamed_106, abs(fragment_input_2.y));
				precise float fragment_unnamed_117 = fragment_unnamed_112 * min(fragment_unnamed_106, abs(fragment_input_2.y));
				precise float fragment_unnamed_118 = fragment_unnamed_117 * fragment_unnamed_117;
				float fragment_unnamed_122 = mad(fragment_unnamed_118, mad(fragment_unnamed_118, mad(fragment_unnamed_118, mad(fragment_unnamed_118, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_123 = fragment_unnamed_117 * fragment_unnamed_122;
				float fragment_unnamed_132 = min(fragment_unnamed_106, fragment_input_2.y);
				precise float fragment_unnamed_133 = (-0.0f) - fragment_unnamed_132;
				float fragment_unnamed_138 = mad(fragment_unnamed_117, fragment_unnamed_122, asfloat(((fragment_unnamed_106 < abs(fragment_input_2.y)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_123, -2.0f, 1.57079637050628662109375f))));
				precise float fragment_unnamed_139 = (-0.0f) - fragment_unnamed_138;
				precise float fragment_unnamed_141 = ((fragment_unnamed_132 < fragment_unnamed_133) ? fragment_unnamed_139 : fragment_unnamed_138) + 1.5707950592041015625f;
				precise float fragment_unnamed_143 = ((fragment_unnamed_90 < fragment_unnamed_91) ? fragment_unnamed_93 : fragment_unnamed_84) * 0.31831014156341552734375f;
				precise float fragment_unnamed_145 = fragment_unnamed_141 * 0.31831014156341552734375f;
				float4 fragment_unnamed_149 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_143, fragment_unnamed_145));
				float fragment_unnamed_151 = fragment_unnamed_149.x;
				fragment_output_0.x = fragment_unnamed_151;
				fragment_output_0.y = fragment_unnamed_149.y;
				fragment_output_0.z = fragment_unnamed_149.z;
				fragment_output_0.w = fragment_unnamed_151;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FULL_VERTICAL
			#endif // HALF_HORIZONTAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !QUAD_HORIZONTAL
			#endif // !TOP_VERTICAL


			#ifdef FULL_VERTICAL
			#ifdef QUAD_HORIZONTAL
			#ifndef BOTTOM_VERTICAL
			#ifndef FULL_HORIZONTAL
			#ifndef HALF_HORIZONTAL
			#ifndef TOP_VERTICAL
			#define ANY_SHADER_VARIANT_ACTIVE

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : TEXCOORD1; // TEXCOORD_1
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float fragment_unnamed_39 = sqrt(dot(float2(fragment_input_2.x, fragment_input_2.z), float2(fragment_input_2.x, fragment_input_2.z)));
				precise float fragment_unnamed_45 = 1.0f / max(fragment_unnamed_39, abs(fragment_input_2.y));
				precise float fragment_unnamed_51 = fragment_unnamed_45 * min(fragment_unnamed_39, abs(fragment_input_2.y));
				precise float fragment_unnamed_52 = fragment_unnamed_51 * fragment_unnamed_51;
				float fragment_unnamed_60 = mad(fragment_unnamed_52, mad(fragment_unnamed_52, mad(fragment_unnamed_52, mad(fragment_unnamed_52, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_62 = fragment_unnamed_60 * fragment_unnamed_51;
				float fragment_unnamed_75 = min(fragment_unnamed_39, fragment_input_2.y);
				precise float fragment_unnamed_76 = (-0.0f) - fragment_unnamed_75;
				float fragment_unnamed_82 = mad(fragment_unnamed_51, fragment_unnamed_60, asfloat(asuint(mad(fragment_unnamed_62, -2.0f, 1.57079637050628662109375f)) & ((fragment_unnamed_39 < abs(fragment_input_2.y)) ? 4294967295u : 0u)));
				precise float fragment_unnamed_83 = (-0.0f) - fragment_unnamed_82;
				precise float fragment_unnamed_85 = ((fragment_unnamed_75 < fragment_unnamed_76) ? fragment_unnamed_83 : fragment_unnamed_82) + 1.5707950592041015625f;
				precise float fragment_unnamed_87 = fragment_unnamed_85 * 0.31831014156341552734375f;
				precise float fragment_unnamed_96 = 1.0f / max(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_104 = fragment_unnamed_96 * min(abs(fragment_input_2.x), abs(fragment_input_2.z));
				precise float fragment_unnamed_105 = fragment_unnamed_104 * fragment_unnamed_104;
				float fragment_unnamed_109 = mad(fragment_unnamed_105, mad(fragment_unnamed_105, mad(fragment_unnamed_105, mad(fragment_unnamed_105, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_110 = fragment_unnamed_109 * fragment_unnamed_104;
				precise float fragment_unnamed_124 = mad(fragment_unnamed_104, fragment_unnamed_109, asfloat(((abs(fragment_input_2.x) < abs(fragment_input_2.z)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_110, -2.0f, 1.57079637050628662109375f)))) * 0.6366202831268310546875f;
				float4 fragment_unnamed_129 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_124, fragment_unnamed_87));
				float fragment_unnamed_131 = fragment_unnamed_129.x;
				fragment_output_0.x = fragment_unnamed_131;
				fragment_output_0.y = fragment_unnamed_129.y;
				fragment_output_0.z = fragment_unnamed_129.z;
				fragment_output_0.w = fragment_unnamed_131;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_0 = stage_input.fragment_input_0;
				fragment_input_2 = stage_input.fragment_input_2;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FULL_VERTICAL
			#endif // QUAD_HORIZONTAL
			#endif // !BOTTOM_VERTICAL
			#endif // !FULL_HORIZONTAL
			#endif // !HALF_HORIZONTAL
			#endif // !TOP_VERTICAL


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
}
