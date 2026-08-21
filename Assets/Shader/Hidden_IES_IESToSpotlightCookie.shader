Shader "Hidden/IES/IESToSpotlightCookie"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SpotHeight ("Spot height", Float) = 0
	}
	SubShader
	{
		Pass
		{
			ZTest Always
			ZWrite Off
			Cull Off
			GpuProgramID 44939

			HLSLPROGRAM

			// https://docs.unity3d.com/Manual/SL-PragmaDirectives.html
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			#pragma shader_feature FULL_HORIZONTAL
			#pragma shader_feature HALF_HORIZONTAL
			#pragma shader_feature QUAD_HORIZONTAL
			#pragma shader_feature TOP_VERTICAL
			#pragma shader_feature VIGNETTE


			#ifdef FULL_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifdef VIGNETTE
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[4];
			static float4 vertex_uniform_buffer_1[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = vertex_input_1.x;
				vertex_output_0.y = vertex_input_1.y;
				precise float vertex_unnamed_46 = vertex_input_0.y * vertex_uniform_buffer_0[1u].x;
				precise float vertex_unnamed_47 = vertex_input_0.y * vertex_uniform_buffer_0[1u].y;
				precise float vertex_unnamed_48 = vertex_input_0.y * vertex_uniform_buffer_0[1u].z;
				precise float vertex_unnamed_49 = vertex_input_0.y * vertex_uniform_buffer_0[1u].w;
				precise float vertex_unnamed_83 = mad(vertex_uniform_buffer_0[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].x, vertex_input_0.x, vertex_unnamed_46)) + vertex_uniform_buffer_0[3u].x;
				precise float vertex_unnamed_84 = mad(vertex_uniform_buffer_0[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].y, vertex_input_0.x, vertex_unnamed_47)) + vertex_uniform_buffer_0[3u].y;
				precise float vertex_unnamed_85 = mad(vertex_uniform_buffer_0[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].z, vertex_input_0.x, vertex_unnamed_48)) + vertex_uniform_buffer_0[3u].z;
				precise float vertex_unnamed_86 = mad(vertex_uniform_buffer_0[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].w, vertex_input_0.x, vertex_unnamed_49)) + vertex_uniform_buffer_0[3u].w;
				precise float vertex_unnamed_94 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].x;
				precise float vertex_unnamed_95 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].y;
				precise float vertex_unnamed_96 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].z;
				precise float vertex_unnamed_97 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_1[20u].x, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].x, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].x, vertex_unnamed_83, vertex_unnamed_94)));
				gl_Position.y = mad(vertex_uniform_buffer_1[20u].y, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].y, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].y, vertex_unnamed_83, vertex_unnamed_95)));
				gl_Position.z = mad(vertex_uniform_buffer_1[20u].z, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].z, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].z, vertex_unnamed_83, vertex_unnamed_96)));
				gl_Position.w = mad(vertex_uniform_buffer_1[20u].w, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].w, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].w, vertex_unnamed_83, vertex_unnamed_97)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_0[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_0[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_0[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_1[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_1[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_1[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			#endif // FULL_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // VIGNETTE
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL


			#ifdef FULL_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifdef VIGNETTE
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_0;
			static float2 vertex_input_1;
			static float4 vertex_input_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_15;
			static float4 vertex_unnamed_54;

			void vert_main()
			{
				vertex_output_0 = vertex_input_1;
				vertex_unnamed_15 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_15 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_15;
				vertex_unnamed_15 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_15;
				vertex_unnamed_15 += unity_ObjectToWorld__array[3];
				vertex_unnamed_54 = vertex_unnamed_15.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_54 = (unity_MatrixVP__array[0] * vertex_unnamed_15.xxxx) + vertex_unnamed_54;
				vertex_unnamed_54 = (unity_MatrixVP__array[2] * vertex_unnamed_15.zzzz) + vertex_unnamed_54;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_15.wwww) + vertex_unnamed_54;
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
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			float _SpotHeight;

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float2 fragment_unnamed_9;
			static float fragment_unnamed_17;
			static float fragment_unnamed_31;
			static float2 fragment_unnamed_45;
			static bool fragment_unnamed_85;
			static bool fragment_unnamed_110;
			static bool fragment_unnamed_146;
			static float3 fragment_unnamed_158;
			static bool fragment_unnamed_234;
			static bool fragment_unnamed_274;

			void frag_main()
			{
				fragment_unnamed_9 = fragment_input_0 + (-0.5f).xx;
				fragment_unnamed_17 = max(abs(fragment_unnamed_9.x), abs(fragment_unnamed_9.y));
				fragment_unnamed_17 = 1.0f / fragment_unnamed_17;
				fragment_unnamed_31 = min(abs(fragment_unnamed_9.x), abs(fragment_unnamed_9.y));
				fragment_unnamed_17 *= fragment_unnamed_31;
				fragment_unnamed_31 = fragment_unnamed_17 * fragment_unnamed_17;
				fragment_unnamed_45.x = (fragment_unnamed_31 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_45.x = (fragment_unnamed_31 * fragment_unnamed_45.x) + 0.1801410019397735595703125f;
				fragment_unnamed_45.x = (fragment_unnamed_31 * fragment_unnamed_45.x) + (-0.33029949665069580078125f);
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_45.x) + 0.999866008758544921875f;
				fragment_unnamed_45.x = fragment_unnamed_31 * fragment_unnamed_17;
				fragment_unnamed_45.x = (fragment_unnamed_45.x * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_85 = abs(fragment_unnamed_9.x) < abs(fragment_unnamed_9.y);
				float fragment_unnamed_95;
				if (fragment_unnamed_85)
				{
					fragment_unnamed_95 = fragment_unnamed_45.x;
				}
				else
				{
					fragment_unnamed_95 = 0.0f;
				}
				fragment_unnamed_45.x = fragment_unnamed_95;
				fragment_unnamed_17 = (fragment_unnamed_17 * fragment_unnamed_31) + fragment_unnamed_45.x;
				fragment_unnamed_110 = fragment_unnamed_9.x < (-fragment_unnamed_9.x);
				fragment_unnamed_31 = fragment_unnamed_110 ? (-3.1415927410125732421875f) : 0.0f;
				fragment_unnamed_17 = fragment_unnamed_31 + fragment_unnamed_17;
				fragment_unnamed_31 = min(fragment_unnamed_9.x, fragment_unnamed_9.y);
				fragment_unnamed_110 = fragment_unnamed_31 < (-fragment_unnamed_31);
				fragment_unnamed_45.x = max(fragment_unnamed_9.x, fragment_unnamed_9.y);
				fragment_unnamed_9.x = dot(fragment_unnamed_9, fragment_unnamed_9);
				fragment_unnamed_9.x = sqrt(fragment_unnamed_9.x);
				fragment_unnamed_146 = fragment_unnamed_45.x >= (-fragment_unnamed_45.x);
				fragment_unnamed_146 = fragment_unnamed_146 && fragment_unnamed_110;
				float fragment_unnamed_160;
				if (fragment_unnamed_146)
				{
					fragment_unnamed_160 = -fragment_unnamed_17;
				}
				else
				{
					fragment_unnamed_160 = fragment_unnamed_17;
				}
				fragment_unnamed_158.x = fragment_unnamed_160;
				fragment_unnamed_158.x += 3.141590118408203125f;
				fragment_unnamed_45.x = fragment_unnamed_158.x * 0.159155070781707763671875f;
				fragment_unnamed_158.x = max(fragment_unnamed_9.x, abs(_SpotHeight));
				fragment_unnamed_158.x = 1.0f / fragment_unnamed_158.x;
				fragment_unnamed_17 = min(fragment_unnamed_9.x, abs(_SpotHeight));
				fragment_unnamed_158.x *= fragment_unnamed_17;
				fragment_unnamed_17 = fragment_unnamed_158.x * fragment_unnamed_158.x;
				fragment_unnamed_31 = (fragment_unnamed_17 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_31 = (fragment_unnamed_17 * fragment_unnamed_31) + 0.1801410019397735595703125f;
				fragment_unnamed_31 = (fragment_unnamed_17 * fragment_unnamed_31) + (-0.33029949665069580078125f);
				fragment_unnamed_17 = (fragment_unnamed_17 * fragment_unnamed_31) + 0.999866008758544921875f;
				fragment_unnamed_31 = fragment_unnamed_17 * fragment_unnamed_158.x;
				fragment_unnamed_31 = (fragment_unnamed_31 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_234 = fragment_unnamed_9.x < abs(_SpotHeight);
				fragment_unnamed_31 = fragment_unnamed_234 ? fragment_unnamed_31 : 0.0f;
				fragment_unnamed_158.x = (fragment_unnamed_158.x * fragment_unnamed_17) + fragment_unnamed_31;
				fragment_unnamed_17 = min(fragment_unnamed_9.x, _SpotHeight);
				fragment_unnamed_9.x += fragment_unnamed_9.x;
				fragment_unnamed_9.x *= fragment_unnamed_9.x;
				fragment_unnamed_9.x *= fragment_unnamed_9.x;
				fragment_unnamed_274 = fragment_unnamed_17 < (-fragment_unnamed_17);
				float fragment_unnamed_280;
				if (fragment_unnamed_274)
				{
					fragment_unnamed_280 = -fragment_unnamed_158.x;
				}
				else
				{
					fragment_unnamed_280 = fragment_unnamed_158.x;
				}
				fragment_unnamed_158.x = fragment_unnamed_280;
				fragment_unnamed_45.y = ((-fragment_unnamed_158.x) * 0.6366202831268310546875f) + 1.0f;
				fragment_unnamed_158 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_45).xyz;
				fragment_output_0 = (fragment_unnamed_9.xxxx * (-fragment_unnamed_158.xyzx)) + fragment_unnamed_158.xyzx;
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
			#endif // VIGNETTE
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL


			#ifdef HALF_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifdef VIGNETTE
			#ifndef FULL_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[4];
			static float4 vertex_uniform_buffer_1[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = vertex_input_1.x;
				vertex_output_0.y = vertex_input_1.y;
				precise float vertex_unnamed_46 = vertex_input_0.y * vertex_uniform_buffer_0[1u].x;
				precise float vertex_unnamed_47 = vertex_input_0.y * vertex_uniform_buffer_0[1u].y;
				precise float vertex_unnamed_48 = vertex_input_0.y * vertex_uniform_buffer_0[1u].z;
				precise float vertex_unnamed_49 = vertex_input_0.y * vertex_uniform_buffer_0[1u].w;
				precise float vertex_unnamed_83 = mad(vertex_uniform_buffer_0[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].x, vertex_input_0.x, vertex_unnamed_46)) + vertex_uniform_buffer_0[3u].x;
				precise float vertex_unnamed_84 = mad(vertex_uniform_buffer_0[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].y, vertex_input_0.x, vertex_unnamed_47)) + vertex_uniform_buffer_0[3u].y;
				precise float vertex_unnamed_85 = mad(vertex_uniform_buffer_0[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].z, vertex_input_0.x, vertex_unnamed_48)) + vertex_uniform_buffer_0[3u].z;
				precise float vertex_unnamed_86 = mad(vertex_uniform_buffer_0[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].w, vertex_input_0.x, vertex_unnamed_49)) + vertex_uniform_buffer_0[3u].w;
				precise float vertex_unnamed_94 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].x;
				precise float vertex_unnamed_95 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].y;
				precise float vertex_unnamed_96 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].z;
				precise float vertex_unnamed_97 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_1[20u].x, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].x, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].x, vertex_unnamed_83, vertex_unnamed_94)));
				gl_Position.y = mad(vertex_uniform_buffer_1[20u].y, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].y, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].y, vertex_unnamed_83, vertex_unnamed_95)));
				gl_Position.z = mad(vertex_uniform_buffer_1[20u].z, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].z, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].z, vertex_unnamed_83, vertex_unnamed_96)));
				gl_Position.w = mad(vertex_uniform_buffer_1[20u].w, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].w, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].w, vertex_unnamed_83, vertex_unnamed_97)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_0[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_0[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_0[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_1[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_1[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_1[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			#endif // HALF_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // VIGNETTE
			#endif // !FULL_HORIZONTAL
			#endif // !QUAD_HORIZONTAL


			#ifdef HALF_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifdef VIGNETTE
			#ifndef FULL_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_0;
			static float2 vertex_input_1;
			static float4 vertex_input_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_15;
			static float4 vertex_unnamed_54;

			void vert_main()
			{
				vertex_output_0 = vertex_input_1;
				vertex_unnamed_15 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_15 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_15;
				vertex_unnamed_15 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_15;
				vertex_unnamed_15 += unity_ObjectToWorld__array[3];
				vertex_unnamed_54 = vertex_unnamed_15.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_54 = (unity_MatrixVP__array[0] * vertex_unnamed_15.xxxx) + vertex_unnamed_54;
				vertex_unnamed_54 = (unity_MatrixVP__array[2] * vertex_unnamed_15.zzzz) + vertex_unnamed_54;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_15.wwww) + vertex_unnamed_54;
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
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			float _SpotHeight;

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float2 fragment_unnamed_9;
			static float fragment_unnamed_17;
			static float fragment_unnamed_31;
			static float2 fragment_unnamed_45;
			static bool fragment_unnamed_85;
			static bool fragment_unnamed_110;
			static bool fragment_unnamed_137;
			static float3 fragment_unnamed_144;
			static bool fragment_unnamed_215;
			static bool fragment_unnamed_255;

			void frag_main()
			{
				fragment_unnamed_9 = fragment_input_0 + (-0.5f).xx;
				fragment_unnamed_17 = max(abs(fragment_unnamed_9.x), abs(fragment_unnamed_9.y));
				fragment_unnamed_17 = 1.0f / fragment_unnamed_17;
				fragment_unnamed_31 = min(abs(fragment_unnamed_9.x), abs(fragment_unnamed_9.y));
				fragment_unnamed_17 *= fragment_unnamed_31;
				fragment_unnamed_31 = fragment_unnamed_17 * fragment_unnamed_17;
				fragment_unnamed_45.x = (fragment_unnamed_31 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_45.x = (fragment_unnamed_31 * fragment_unnamed_45.x) + 0.1801410019397735595703125f;
				fragment_unnamed_45.x = (fragment_unnamed_31 * fragment_unnamed_45.x) + (-0.33029949665069580078125f);
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_45.x) + 0.999866008758544921875f;
				fragment_unnamed_45.x = fragment_unnamed_31 * fragment_unnamed_17;
				fragment_unnamed_45.x = (fragment_unnamed_45.x * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_85 = abs(fragment_unnamed_9.x) < abs(fragment_unnamed_9.y);
				float fragment_unnamed_95;
				if (fragment_unnamed_85)
				{
					fragment_unnamed_95 = fragment_unnamed_45.x;
				}
				else
				{
					fragment_unnamed_95 = 0.0f;
				}
				fragment_unnamed_45.x = fragment_unnamed_95;
				fragment_unnamed_17 = (fragment_unnamed_17 * fragment_unnamed_31) + fragment_unnamed_45.x;
				fragment_unnamed_110 = fragment_unnamed_9.x < (-fragment_unnamed_9.x);
				fragment_unnamed_31 = fragment_unnamed_110 ? (-3.1415927410125732421875f) : 0.0f;
				fragment_unnamed_17 = fragment_unnamed_31 + fragment_unnamed_17;
				fragment_unnamed_31 = min(fragment_unnamed_9.x, abs(fragment_unnamed_9.y));
				fragment_unnamed_9.x = dot(fragment_unnamed_9, fragment_unnamed_9);
				fragment_unnamed_9.x = sqrt(fragment_unnamed_9.x);
				fragment_unnamed_137 = fragment_unnamed_31 < (-fragment_unnamed_31);
				float fragment_unnamed_146;
				if (fragment_unnamed_137)
				{
					fragment_unnamed_146 = -fragment_unnamed_17;
				}
				else
				{
					fragment_unnamed_146 = fragment_unnamed_17;
				}
				fragment_unnamed_144.x = fragment_unnamed_146;
				fragment_unnamed_45.x = fragment_unnamed_144.x * 0.31831014156341552734375f;
				fragment_unnamed_144.x = max(fragment_unnamed_9.x, abs(_SpotHeight));
				fragment_unnamed_144.x = 1.0f / fragment_unnamed_144.x;
				fragment_unnamed_17 = min(fragment_unnamed_9.x, abs(_SpotHeight));
				fragment_unnamed_144.x *= fragment_unnamed_17;
				fragment_unnamed_17 = fragment_unnamed_144.x * fragment_unnamed_144.x;
				fragment_unnamed_31 = (fragment_unnamed_17 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_31 = (fragment_unnamed_17 * fragment_unnamed_31) + 0.1801410019397735595703125f;
				fragment_unnamed_31 = (fragment_unnamed_17 * fragment_unnamed_31) + (-0.33029949665069580078125f);
				fragment_unnamed_17 = (fragment_unnamed_17 * fragment_unnamed_31) + 0.999866008758544921875f;
				fragment_unnamed_31 = fragment_unnamed_17 * fragment_unnamed_144.x;
				fragment_unnamed_31 = (fragment_unnamed_31 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_215 = fragment_unnamed_9.x < abs(_SpotHeight);
				fragment_unnamed_31 = fragment_unnamed_215 ? fragment_unnamed_31 : 0.0f;
				fragment_unnamed_144.x = (fragment_unnamed_144.x * fragment_unnamed_17) + fragment_unnamed_31;
				fragment_unnamed_17 = min(fragment_unnamed_9.x, _SpotHeight);
				fragment_unnamed_9.x += fragment_unnamed_9.x;
				fragment_unnamed_9.x *= fragment_unnamed_9.x;
				fragment_unnamed_9.x *= fragment_unnamed_9.x;
				fragment_unnamed_255 = fragment_unnamed_17 < (-fragment_unnamed_17);
				float fragment_unnamed_261;
				if (fragment_unnamed_255)
				{
					fragment_unnamed_261 = -fragment_unnamed_144.x;
				}
				else
				{
					fragment_unnamed_261 = fragment_unnamed_144.x;
				}
				fragment_unnamed_144.x = fragment_unnamed_261;
				fragment_unnamed_45.y = ((-fragment_unnamed_144.x) * 0.6366202831268310546875f) + 1.0f;
				fragment_unnamed_144 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_45).xyz;
				fragment_output_0 = (fragment_unnamed_9.xxxx * (-fragment_unnamed_144.xyzx)) + fragment_unnamed_144.xyzx;
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
			#endif // VIGNETTE
			#endif // !FULL_HORIZONTAL
			#endif // !QUAD_HORIZONTAL


			#ifdef QUAD_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifdef VIGNETTE
			#ifndef FULL_HORIZONTAL
			#ifndef HALF_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 vertex_uniform_buffer_0[4];
			static float4 vertex_uniform_buffer_1[21];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD0; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD; // TEXCOORD
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				vertex_output_0.x = vertex_input_1.x;
				vertex_output_0.y = vertex_input_1.y;
				precise float vertex_unnamed_46 = vertex_input_0.y * vertex_uniform_buffer_0[1u].x;
				precise float vertex_unnamed_47 = vertex_input_0.y * vertex_uniform_buffer_0[1u].y;
				precise float vertex_unnamed_48 = vertex_input_0.y * vertex_uniform_buffer_0[1u].z;
				precise float vertex_unnamed_49 = vertex_input_0.y * vertex_uniform_buffer_0[1u].w;
				precise float vertex_unnamed_83 = mad(vertex_uniform_buffer_0[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].x, vertex_input_0.x, vertex_unnamed_46)) + vertex_uniform_buffer_0[3u].x;
				precise float vertex_unnamed_84 = mad(vertex_uniform_buffer_0[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].y, vertex_input_0.x, vertex_unnamed_47)) + vertex_uniform_buffer_0[3u].y;
				precise float vertex_unnamed_85 = mad(vertex_uniform_buffer_0[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].z, vertex_input_0.x, vertex_unnamed_48)) + vertex_uniform_buffer_0[3u].z;
				precise float vertex_unnamed_86 = mad(vertex_uniform_buffer_0[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_0[0u].w, vertex_input_0.x, vertex_unnamed_49)) + vertex_uniform_buffer_0[3u].w;
				precise float vertex_unnamed_94 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].x;
				precise float vertex_unnamed_95 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].y;
				precise float vertex_unnamed_96 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].z;
				precise float vertex_unnamed_97 = vertex_unnamed_84 * vertex_uniform_buffer_1[18u].w;
				gl_Position.x = mad(vertex_uniform_buffer_1[20u].x, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].x, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].x, vertex_unnamed_83, vertex_unnamed_94)));
				gl_Position.y = mad(vertex_uniform_buffer_1[20u].y, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].y, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].y, vertex_unnamed_83, vertex_unnamed_95)));
				gl_Position.z = mad(vertex_uniform_buffer_1[20u].z, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].z, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].z, vertex_unnamed_83, vertex_unnamed_96)));
				gl_Position.w = mad(vertex_uniform_buffer_1[20u].w, vertex_unnamed_86, mad(vertex_uniform_buffer_1[19u].w, vertex_unnamed_85, mad(vertex_uniform_buffer_1[17u].w, vertex_unnamed_83, vertex_unnamed_97)));
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_0[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_0[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_0[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_1[17] = float4(unity_MatrixVP[0][0], unity_MatrixVP[1][0], unity_MatrixVP[2][0], unity_MatrixVP[3][0]);
				vertex_uniform_buffer_1[18] = float4(unity_MatrixVP[0][1], unity_MatrixVP[1][1], unity_MatrixVP[2][1], unity_MatrixVP[3][1]);
				vertex_uniform_buffer_1[19] = float4(unity_MatrixVP[0][2], unity_MatrixVP[1][2], unity_MatrixVP[2][2], unity_MatrixVP[3][2]);
				vertex_uniform_buffer_1[20] = float4(unity_MatrixVP[0][3], unity_MatrixVP[1][3], unity_MatrixVP[2][3], unity_MatrixVP[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			#endif // QUAD_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // VIGNETTE
			#endif // !FULL_HORIZONTAL
			#endif // !HALF_HORIZONTAL


			#ifdef QUAD_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifdef VIGNETTE
			#ifndef FULL_HORIZONTAL
			#ifndef HALF_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			static float4 unity_ObjectToWorld__array[4];
			static float4 unity_MatrixVP__array[4];
			static float4 gl_Position;
			static float2 vertex_output_0;
			static float2 vertex_input_1;
			static float4 vertex_input_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION;
				float2 vertex_input_1 : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_0 : TEXCOORD0; // vs_TEXCOORD0
				float4 gl_Position : SV_Position;
			};

			static float4 vertex_unnamed_15;
			static float4 vertex_unnamed_54;

			void vert_main()
			{
				vertex_output_0 = vertex_input_1;
				vertex_unnamed_15 = vertex_input_0.yyyy * unity_ObjectToWorld__array[1];
				vertex_unnamed_15 = (unity_ObjectToWorld__array[0] * vertex_input_0.xxxx) + vertex_unnamed_15;
				vertex_unnamed_15 = (unity_ObjectToWorld__array[2] * vertex_input_0.zzzz) + vertex_unnamed_15;
				vertex_unnamed_15 += unity_ObjectToWorld__array[3];
				vertex_unnamed_54 = vertex_unnamed_15.yyyy * unity_MatrixVP__array[1];
				vertex_unnamed_54 = (unity_MatrixVP__array[0] * vertex_unnamed_15.xxxx) + vertex_unnamed_54;
				vertex_unnamed_54 = (unity_MatrixVP__array[2] * vertex_unnamed_15.zzzz) + vertex_unnamed_54;
				gl_Position = (unity_MatrixVP__array[3] * vertex_unnamed_15.wwww) + vertex_unnamed_54;
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
				stage_output.vertex_output_0 = vertex_output_0;
				return stage_output;
			}

			float _SpotHeight;

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD0; // vs_TEXCOORD0
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			static float2 fragment_unnamed_9;
			static float fragment_unnamed_17;
			static float fragment_unnamed_31;
			static float2 fragment_unnamed_45;
			static bool fragment_unnamed_85;
			static float3 fragment_unnamed_103;
			static bool fragment_unnamed_182;
			static bool fragment_unnamed_222;

			void frag_main()
			{
				fragment_unnamed_9 = fragment_input_0 + (-0.5f).xx;
				fragment_unnamed_17 = max(abs(fragment_unnamed_9.x), abs(fragment_unnamed_9.y));
				fragment_unnamed_17 = 1.0f / fragment_unnamed_17;
				fragment_unnamed_31 = min(abs(fragment_unnamed_9.x), abs(fragment_unnamed_9.y));
				fragment_unnamed_17 *= fragment_unnamed_31;
				fragment_unnamed_31 = fragment_unnamed_17 * fragment_unnamed_17;
				fragment_unnamed_45.x = (fragment_unnamed_31 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_45.x = (fragment_unnamed_31 * fragment_unnamed_45.x) + 0.1801410019397735595703125f;
				fragment_unnamed_45.x = (fragment_unnamed_31 * fragment_unnamed_45.x) + (-0.33029949665069580078125f);
				fragment_unnamed_31 = (fragment_unnamed_31 * fragment_unnamed_45.x) + 0.999866008758544921875f;
				fragment_unnamed_45.x = fragment_unnamed_31 * fragment_unnamed_17;
				fragment_unnamed_45.x = (fragment_unnamed_45.x * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_85 = abs(fragment_unnamed_9.x) < abs(fragment_unnamed_9.y);
				fragment_unnamed_9.x = dot(fragment_unnamed_9, fragment_unnamed_9);
				fragment_unnamed_9.x = sqrt(fragment_unnamed_9.x);
				float fragment_unnamed_106;
				if (fragment_unnamed_85)
				{
					fragment_unnamed_106 = fragment_unnamed_45.x;
				}
				else
				{
					fragment_unnamed_106 = 0.0f;
				}
				fragment_unnamed_103.x = fragment_unnamed_106;
				fragment_unnamed_103.x = (fragment_unnamed_17 * fragment_unnamed_31) + fragment_unnamed_103.x;
				fragment_unnamed_45.x = fragment_unnamed_103.x * 0.6366202831268310546875f;
				fragment_unnamed_103.x = max(fragment_unnamed_9.x, abs(_SpotHeight));
				fragment_unnamed_103.x = 1.0f / fragment_unnamed_103.x;
				fragment_unnamed_17 = min(fragment_unnamed_9.x, abs(_SpotHeight));
				fragment_unnamed_103.x *= fragment_unnamed_17;
				fragment_unnamed_17 = fragment_unnamed_103.x * fragment_unnamed_103.x;
				fragment_unnamed_31 = (fragment_unnamed_17 * 0.02083509974181652069091796875f) + (-0.08513300120830535888671875f);
				fragment_unnamed_31 = (fragment_unnamed_17 * fragment_unnamed_31) + 0.1801410019397735595703125f;
				fragment_unnamed_31 = (fragment_unnamed_17 * fragment_unnamed_31) + (-0.33029949665069580078125f);
				fragment_unnamed_17 = (fragment_unnamed_17 * fragment_unnamed_31) + 0.999866008758544921875f;
				fragment_unnamed_31 = fragment_unnamed_17 * fragment_unnamed_103.x;
				fragment_unnamed_31 = (fragment_unnamed_31 * (-2.0f)) + 1.57079637050628662109375f;
				fragment_unnamed_182 = fragment_unnamed_9.x < abs(_SpotHeight);
				fragment_unnamed_31 = fragment_unnamed_182 ? fragment_unnamed_31 : 0.0f;
				fragment_unnamed_103.x = (fragment_unnamed_103.x * fragment_unnamed_17) + fragment_unnamed_31;
				fragment_unnamed_17 = min(fragment_unnamed_9.x, _SpotHeight);
				fragment_unnamed_9.x += fragment_unnamed_9.x;
				fragment_unnamed_9.x *= fragment_unnamed_9.x;
				fragment_unnamed_9.x *= fragment_unnamed_9.x;
				fragment_unnamed_222 = fragment_unnamed_17 < (-fragment_unnamed_17);
				float fragment_unnamed_228;
				if (fragment_unnamed_222)
				{
					fragment_unnamed_228 = -fragment_unnamed_103.x;
				}
				else
				{
					fragment_unnamed_228 = fragment_unnamed_103.x;
				}
				fragment_unnamed_103.x = fragment_unnamed_228;
				fragment_unnamed_45.y = ((-fragment_unnamed_103.x) * 0.6366202831268310546875f) + 1.0f;
				fragment_unnamed_103 = _MainTex.Sample(sampler_MainTex, fragment_unnamed_45).xyz;
				fragment_output_0 = (fragment_unnamed_9.xxxx * (-fragment_unnamed_103.xyzx)) + fragment_unnamed_103.xyzx;
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
			#endif // VIGNETTE
			#endif // !FULL_HORIZONTAL
			#endif // !HALF_HORIZONTAL


			#ifdef FULL_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifdef VIGNETTE
			#ifndef HALF_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float _SpotHeight;

			static float4 fragment_uniform_buffer_0[3];
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_35 = fragment_input_0.x + (-0.5f);
				precise float fragment_unnamed_37 = fragment_input_0.y + (-0.5f);
				precise float fragment_unnamed_42 = 1.0f / max(abs(fragment_unnamed_35), abs(fragment_unnamed_37));
				precise float fragment_unnamed_47 = fragment_unnamed_42 * min(abs(fragment_unnamed_35), abs(fragment_unnamed_37));
				precise float fragment_unnamed_48 = fragment_unnamed_47 * fragment_unnamed_47;
				float fragment_unnamed_56 = mad(fragment_unnamed_48, mad(fragment_unnamed_48, mad(fragment_unnamed_48, mad(fragment_unnamed_48, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_58 = fragment_unnamed_56 * fragment_unnamed_47;
				precise float fragment_unnamed_72 = (-0.0f) - fragment_unnamed_35;
				precise float fragment_unnamed_79 = asfloat(((fragment_unnamed_35 < fragment_unnamed_72) ? 4294967295u : 0u) & 3226013659u) + mad(fragment_unnamed_47, fragment_unnamed_56, asfloat(((abs(fragment_unnamed_35) < abs(fragment_unnamed_37)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_58, -2.0f, 1.57079637050628662109375f))));
				float fragment_unnamed_80 = min(fragment_unnamed_35, fragment_unnamed_37);
				precise float fragment_unnamed_81 = (-0.0f) - fragment_unnamed_80;
				float fragment_unnamed_84 = max(fragment_unnamed_35, fragment_unnamed_37);
				float fragment_unnamed_88 = sqrt(dot(float2(fragment_unnamed_35, fragment_unnamed_37), float2(fragment_unnamed_35, fragment_unnamed_37)));
				precise float fragment_unnamed_89 = (-0.0f) - fragment_unnamed_84;
				precise float fragment_unnamed_94 = (-0.0f) - fragment_unnamed_79;
				precise float fragment_unnamed_96 = (((((fragment_unnamed_84 >= fragment_unnamed_89) ? 4294967295u : 0u) & ((fragment_unnamed_80 < fragment_unnamed_81) ? 4294967295u : 0u)) != 0u) ? fragment_unnamed_94 : fragment_unnamed_79) + 3.141590118408203125f;
				precise float fragment_unnamed_98 = fragment_unnamed_96 * 0.159155070781707763671875f;
				precise float fragment_unnamed_107 = 1.0f / max(fragment_unnamed_88, abs(fragment_uniform_buffer_0[2u].x));
				precise float fragment_unnamed_113 = fragment_unnamed_107 * min(fragment_unnamed_88, abs(fragment_uniform_buffer_0[2u].x));
				precise float fragment_unnamed_114 = fragment_unnamed_113 * fragment_unnamed_113;
				float fragment_unnamed_118 = mad(fragment_unnamed_114, mad(fragment_unnamed_114, mad(fragment_unnamed_114, mad(fragment_unnamed_114, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_119 = fragment_unnamed_118 * fragment_unnamed_113;
				float fragment_unnamed_130 = mad(fragment_unnamed_113, fragment_unnamed_118, asfloat(asuint(mad(fragment_unnamed_119, -2.0f, 1.57079637050628662109375f)) & ((fragment_unnamed_88 < abs(fragment_uniform_buffer_0[2u].x)) ? 4294967295u : 0u)));
				float fragment_unnamed_134 = min(fragment_unnamed_88, fragment_uniform_buffer_0[2u].x);
				precise float fragment_unnamed_135 = fragment_unnamed_88 + fragment_unnamed_88;
				precise float fragment_unnamed_136 = fragment_unnamed_135 * fragment_unnamed_135;
				precise float fragment_unnamed_137 = fragment_unnamed_136 * fragment_unnamed_136;
				precise float fragment_unnamed_138 = (-0.0f) - fragment_unnamed_134;
				precise float fragment_unnamed_140 = (-0.0f) - fragment_unnamed_130;
				precise float fragment_unnamed_142 = (-0.0f) - ((fragment_unnamed_134 < fragment_unnamed_138) ? fragment_unnamed_140 : fragment_unnamed_130);
				float4 fragment_unnamed_148 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_98, mad(fragment_unnamed_142, 0.6366202831268310546875f, 1.0f)));
				float fragment_unnamed_150 = fragment_unnamed_148.x;
				float fragment_unnamed_151 = fragment_unnamed_148.y;
				float fragment_unnamed_152 = fragment_unnamed_148.z;
				precise float fragment_unnamed_153 = (-0.0f) - fragment_unnamed_150;
				precise float fragment_unnamed_154 = (-0.0f) - fragment_unnamed_151;
				precise float fragment_unnamed_155 = (-0.0f) - fragment_unnamed_152;
				fragment_output_0.x = mad(fragment_unnamed_137, fragment_unnamed_153, fragment_unnamed_150);
				fragment_output_0.y = mad(fragment_unnamed_137, fragment_unnamed_154, fragment_unnamed_151);
				fragment_output_0.z = mad(fragment_unnamed_137, fragment_unnamed_155, fragment_unnamed_152);
				fragment_output_0.w = mad(fragment_unnamed_137, fragment_unnamed_153, fragment_unnamed_150);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_SpotHeight, fragment_uniform_buffer_0[2][1], fragment_uniform_buffer_0[2][2], fragment_uniform_buffer_0[2][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // FULL_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // VIGNETTE
			#endif // !HALF_HORIZONTAL
			#endif // !QUAD_HORIZONTAL


			#ifdef HALF_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifdef VIGNETTE
			#ifndef FULL_HORIZONTAL
			#ifndef QUAD_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float _SpotHeight;

			static float4 fragment_uniform_buffer_0[3];
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_35 = fragment_input_0.x + (-0.5f);
				precise float fragment_unnamed_37 = fragment_input_0.y + (-0.5f);
				precise float fragment_unnamed_42 = 1.0f / max(abs(fragment_unnamed_35), abs(fragment_unnamed_37));
				precise float fragment_unnamed_47 = fragment_unnamed_42 * min(abs(fragment_unnamed_35), abs(fragment_unnamed_37));
				precise float fragment_unnamed_48 = fragment_unnamed_47 * fragment_unnamed_47;
				float fragment_unnamed_56 = mad(fragment_unnamed_48, mad(fragment_unnamed_48, mad(fragment_unnamed_48, mad(fragment_unnamed_48, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_58 = fragment_unnamed_56 * fragment_unnamed_47;
				precise float fragment_unnamed_72 = (-0.0f) - fragment_unnamed_35;
				precise float fragment_unnamed_79 = asfloat(((fragment_unnamed_35 < fragment_unnamed_72) ? 4294967295u : 0u) & 3226013659u) + mad(fragment_unnamed_47, fragment_unnamed_56, asfloat(((abs(fragment_unnamed_35) < abs(fragment_unnamed_37)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_58, -2.0f, 1.57079637050628662109375f))));
				float fragment_unnamed_81 = min(fragment_unnamed_35, abs(fragment_unnamed_37));
				float fragment_unnamed_85 = sqrt(dot(float2(fragment_unnamed_35, fragment_unnamed_37), float2(fragment_unnamed_35, fragment_unnamed_37)));
				precise float fragment_unnamed_86 = (-0.0f) - fragment_unnamed_81;
				precise float fragment_unnamed_88 = (-0.0f) - fragment_unnamed_79;
				precise float fragment_unnamed_90 = ((fragment_unnamed_81 < fragment_unnamed_86) ? fragment_unnamed_88 : fragment_unnamed_79) * 0.31831014156341552734375f;
				precise float fragment_unnamed_99 = 1.0f / max(fragment_unnamed_85, abs(fragment_uniform_buffer_0[2u].x));
				precise float fragment_unnamed_105 = fragment_unnamed_99 * min(fragment_unnamed_85, abs(fragment_uniform_buffer_0[2u].x));
				precise float fragment_unnamed_106 = fragment_unnamed_105 * fragment_unnamed_105;
				float fragment_unnamed_110 = mad(fragment_unnamed_106, mad(fragment_unnamed_106, mad(fragment_unnamed_106, mad(fragment_unnamed_106, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_111 = fragment_unnamed_110 * fragment_unnamed_105;
				float fragment_unnamed_122 = mad(fragment_unnamed_105, fragment_unnamed_110, asfloat(asuint(mad(fragment_unnamed_111, -2.0f, 1.57079637050628662109375f)) & ((fragment_unnamed_85 < abs(fragment_uniform_buffer_0[2u].x)) ? 4294967295u : 0u)));
				float fragment_unnamed_126 = min(fragment_unnamed_85, fragment_uniform_buffer_0[2u].x);
				precise float fragment_unnamed_127 = fragment_unnamed_85 + fragment_unnamed_85;
				precise float fragment_unnamed_128 = fragment_unnamed_127 * fragment_unnamed_127;
				precise float fragment_unnamed_129 = fragment_unnamed_128 * fragment_unnamed_128;
				precise float fragment_unnamed_130 = (-0.0f) - fragment_unnamed_126;
				precise float fragment_unnamed_132 = (-0.0f) - fragment_unnamed_122;
				precise float fragment_unnamed_134 = (-0.0f) - ((fragment_unnamed_126 < fragment_unnamed_130) ? fragment_unnamed_132 : fragment_unnamed_122);
				float4 fragment_unnamed_140 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_90, mad(fragment_unnamed_134, 0.6366202831268310546875f, 1.0f)));
				float fragment_unnamed_142 = fragment_unnamed_140.x;
				float fragment_unnamed_143 = fragment_unnamed_140.y;
				float fragment_unnamed_144 = fragment_unnamed_140.z;
				precise float fragment_unnamed_145 = (-0.0f) - fragment_unnamed_142;
				precise float fragment_unnamed_146 = (-0.0f) - fragment_unnamed_143;
				precise float fragment_unnamed_147 = (-0.0f) - fragment_unnamed_144;
				fragment_output_0.x = mad(fragment_unnamed_129, fragment_unnamed_145, fragment_unnamed_142);
				fragment_output_0.y = mad(fragment_unnamed_129, fragment_unnamed_146, fragment_unnamed_143);
				fragment_output_0.z = mad(fragment_unnamed_129, fragment_unnamed_147, fragment_unnamed_144);
				fragment_output_0.w = mad(fragment_unnamed_129, fragment_unnamed_145, fragment_unnamed_142);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_SpotHeight, fragment_uniform_buffer_0[2][1], fragment_uniform_buffer_0[2][2], fragment_uniform_buffer_0[2][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // HALF_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // VIGNETTE
			#endif // !FULL_HORIZONTAL
			#endif // !QUAD_HORIZONTAL


			#ifdef QUAD_HORIZONTAL
			#ifdef TOP_VERTICAL
			#ifdef VIGNETTE
			#ifndef FULL_HORIZONTAL
			#ifndef HALF_HORIZONTAL
			#define ANY_SHADER_VARIANT_ACTIVE

			float _SpotHeight;

			static float4 fragment_uniform_buffer_0[3];
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			static float2 fragment_input_0;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_0 : TEXCOORD; // TEXCOORD
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				precise float fragment_unnamed_35 = fragment_input_0.x + (-0.5f);
				precise float fragment_unnamed_37 = fragment_input_0.y + (-0.5f);
				precise float fragment_unnamed_42 = 1.0f / max(abs(fragment_unnamed_35), abs(fragment_unnamed_37));
				precise float fragment_unnamed_47 = fragment_unnamed_42 * min(abs(fragment_unnamed_35), abs(fragment_unnamed_37));
				precise float fragment_unnamed_48 = fragment_unnamed_47 * fragment_unnamed_47;
				float fragment_unnamed_56 = mad(fragment_unnamed_48, mad(fragment_unnamed_48, mad(fragment_unnamed_48, mad(fragment_unnamed_48, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_58 = fragment_unnamed_56 * fragment_unnamed_47;
				float fragment_unnamed_71 = sqrt(dot(float2(fragment_unnamed_35, fragment_unnamed_37), float2(fragment_unnamed_35, fragment_unnamed_37)));
				precise float fragment_unnamed_76 = mad(fragment_unnamed_47, fragment_unnamed_56, asfloat(((abs(fragment_unnamed_35) < abs(fragment_unnamed_37)) ? 4294967295u : 0u) & asuint(mad(fragment_unnamed_58, -2.0f, 1.57079637050628662109375f)))) * 0.6366202831268310546875f;
				precise float fragment_unnamed_85 = 1.0f / max(fragment_unnamed_71, abs(fragment_uniform_buffer_0[2u].x));
				precise float fragment_unnamed_91 = fragment_unnamed_85 * min(fragment_unnamed_71, abs(fragment_uniform_buffer_0[2u].x));
				precise float fragment_unnamed_92 = fragment_unnamed_91 * fragment_unnamed_91;
				float fragment_unnamed_96 = mad(fragment_unnamed_92, mad(fragment_unnamed_92, mad(fragment_unnamed_92, mad(fragment_unnamed_92, 0.02083509974181652069091796875f, -0.08513300120830535888671875f), 0.1801410019397735595703125f), -0.33029949665069580078125f), 0.999866008758544921875f);
				precise float fragment_unnamed_97 = fragment_unnamed_96 * fragment_unnamed_91;
				float fragment_unnamed_108 = mad(fragment_unnamed_91, fragment_unnamed_96, asfloat(asuint(mad(fragment_unnamed_97, -2.0f, 1.57079637050628662109375f)) & ((fragment_unnamed_71 < abs(fragment_uniform_buffer_0[2u].x)) ? 4294967295u : 0u)));
				float fragment_unnamed_112 = min(fragment_unnamed_71, fragment_uniform_buffer_0[2u].x);
				precise float fragment_unnamed_113 = fragment_unnamed_71 + fragment_unnamed_71;
				precise float fragment_unnamed_114 = fragment_unnamed_113 * fragment_unnamed_113;
				precise float fragment_unnamed_115 = fragment_unnamed_114 * fragment_unnamed_114;
				precise float fragment_unnamed_116 = (-0.0f) - fragment_unnamed_112;
				precise float fragment_unnamed_119 = (-0.0f) - fragment_unnamed_108;
				precise float fragment_unnamed_121 = (-0.0f) - ((fragment_unnamed_112 < fragment_unnamed_116) ? fragment_unnamed_119 : fragment_unnamed_108);
				float4 fragment_unnamed_126 = _MainTex.Sample(sampler_MainTex, float2(fragment_unnamed_76, mad(fragment_unnamed_121, 0.6366202831268310546875f, 1.0f)));
				float fragment_unnamed_128 = fragment_unnamed_126.x;
				float fragment_unnamed_129 = fragment_unnamed_126.y;
				float fragment_unnamed_130 = fragment_unnamed_126.z;
				precise float fragment_unnamed_131 = (-0.0f) - fragment_unnamed_128;
				precise float fragment_unnamed_132 = (-0.0f) - fragment_unnamed_129;
				precise float fragment_unnamed_133 = (-0.0f) - fragment_unnamed_130;
				fragment_output_0.x = mad(fragment_unnamed_115, fragment_unnamed_131, fragment_unnamed_128);
				fragment_output_0.y = mad(fragment_unnamed_115, fragment_unnamed_132, fragment_unnamed_129);
				fragment_output_0.z = mad(fragment_unnamed_115, fragment_unnamed_133, fragment_unnamed_130);
				fragment_output_0.w = mad(fragment_unnamed_115, fragment_unnamed_131, fragment_unnamed_128);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_SpotHeight, fragment_uniform_buffer_0[2][1], fragment_uniform_buffer_0[2][2], fragment_uniform_buffer_0[2][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}

			#endif // QUAD_HORIZONTAL
			#endif // TOP_VERTICAL
			#endif // VIGNETTE
			#endif // !FULL_HORIZONTAL
			#endif // !HALF_HORIZONTAL


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
