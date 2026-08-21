Shader "Hidden/ProBuilder/VertexPicker"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "ALWAYS" "ProBuilderPicker" = "VertexPass" "RenderType" = "Transparent" }
		Pass
		{
			Name "Vertices"
			Tags { "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "ALWAYS" "ProBuilderPicker" = "VertexPass" "RenderType" = "Transparent" }
			Cull Off
			Offset -1, -1
			GpuProgramID 18139

			HLSLPROGRAM

			// https://docs.unity3d.com/Manual/SL-PragmaDirectives.html
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0


			float4 _ScreenParams;
			float4x4 unity_ObjectToWorld;
			float4x4 glstate_matrix_projection;
			float4x4 unity_MatrixV;

			static float4 vertex_uniform_buffer_0[7];
			static float4 vertex_uniform_buffer_1[4];
			static float4 vertex_uniform_buffer_2[13];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float3 vertex_input_1;
			static float4 vertex_input_2;
			static float2 vertex_input_3;
			static float2 vertex_input_4;
			static float2 vertex_output_1;
			static float4 vertex_output_2;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float3 vertex_input_1 : NORMAL; // NORMAL
				float4 vertex_input_2 : COLOR; // COLOR
				float2 vertex_input_3 : TEXCOORD; // TEXCOORD
				float2 vertex_input_4 : TEXCOORD1; // TEXCOORD_1
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float4 vertex_output_2 : COLOR; // COLOR
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_50 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_51 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_52 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_53 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				precise float vertex_unnamed_87 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_50)) + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_88 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_51)) + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_89 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_52)) + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_90 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_53)) + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_97 = vertex_unnamed_88 * vertex_uniform_buffer_2[10u].x;
				precise float vertex_unnamed_98 = vertex_unnamed_88 * vertex_uniform_buffer_2[10u].y;
				precise float vertex_unnamed_99 = vertex_unnamed_88 * vertex_uniform_buffer_2[10u].z;
				precise float vertex_unnamed_131 = (-0.0f) - vertex_uniform_buffer_2[8u].w;
				precise float vertex_unnamed_133 = vertex_unnamed_131 + 1.0f;
				float vertex_unnamed_135 = mad(vertex_unnamed_133, -0.040000021457672119140625f, 0.9900000095367431640625f);
				precise float vertex_unnamed_138 = mad(vertex_uniform_buffer_2[12u].x, vertex_unnamed_90, mad(vertex_uniform_buffer_2[11u].x, vertex_unnamed_89, mad(vertex_uniform_buffer_2[9u].x, vertex_unnamed_87, vertex_unnamed_97))) * vertex_unnamed_135;
				precise float vertex_unnamed_139 = mad(vertex_uniform_buffer_2[12u].y, vertex_unnamed_90, mad(vertex_uniform_buffer_2[11u].y, vertex_unnamed_89, mad(vertex_uniform_buffer_2[9u].y, vertex_unnamed_87, vertex_unnamed_98))) * vertex_unnamed_135;
				precise float vertex_unnamed_140 = mad(vertex_uniform_buffer_2[12u].z, vertex_unnamed_90, mad(vertex_uniform_buffer_2[11u].z, vertex_unnamed_89, mad(vertex_uniform_buffer_2[9u].z, vertex_unnamed_87, vertex_unnamed_99))) * vertex_unnamed_135;
				precise float vertex_unnamed_148 = vertex_unnamed_139 * vertex_uniform_buffer_2[6u].x;
				precise float vertex_unnamed_149 = vertex_unnamed_139 * vertex_uniform_buffer_2[6u].y;
				precise float vertex_unnamed_150 = vertex_unnamed_139 * vertex_uniform_buffer_2[6u].z;
				precise float vertex_unnamed_151 = vertex_unnamed_139 * vertex_uniform_buffer_2[6u].w;
				precise float vertex_unnamed_179 = mad(vertex_uniform_buffer_2[7u].x, vertex_unnamed_140, mad(vertex_uniform_buffer_2[5u].x, vertex_unnamed_138, vertex_unnamed_148)) + vertex_uniform_buffer_2[8u].x;
				precise float vertex_unnamed_180 = mad(vertex_uniform_buffer_2[7u].y, vertex_unnamed_140, mad(vertex_uniform_buffer_2[5u].y, vertex_unnamed_138, vertex_unnamed_149)) + vertex_uniform_buffer_2[8u].y;
				precise float vertex_unnamed_181 = mad(vertex_uniform_buffer_2[7u].z, vertex_unnamed_140, mad(vertex_uniform_buffer_2[5u].z, vertex_unnamed_138, vertex_unnamed_150)) + vertex_uniform_buffer_2[8u].z;
				precise float vertex_unnamed_182 = mad(vertex_uniform_buffer_2[7u].w, vertex_unnamed_140, mad(vertex_uniform_buffer_2[5u].w, vertex_unnamed_138, vertex_unnamed_151)) + vertex_uniform_buffer_2[8u].w;
				precise float vertex_unnamed_183 = vertex_unnamed_179 / vertex_unnamed_182;
				precise float vertex_unnamed_184 = vertex_unnamed_180 / vertex_unnamed_182;
				precise float vertex_unnamed_192 = vertex_input_4.x * 3.5f;
				precise float vertex_unnamed_194 = vertex_input_4.y * 3.5f;
				precise float vertex_unnamed_205 = mad(mad(vertex_unnamed_183, 0.5f, 0.5f), vertex_uniform_buffer_0[6u].x, vertex_unnamed_192) / vertex_uniform_buffer_0[6u].x;
				precise float vertex_unnamed_206 = mad(mad(vertex_unnamed_184, 0.5f, 0.5f), vertex_uniform_buffer_0[6u].y, vertex_unnamed_194) / vertex_uniform_buffer_0[6u].y;
				precise float vertex_unnamed_207 = vertex_unnamed_205 + (-0.5f);
				precise float vertex_unnamed_209 = vertex_unnamed_206 + (-0.5f);
				precise float vertex_unnamed_210 = vertex_unnamed_182 * vertex_unnamed_207;
				precise float vertex_unnamed_211 = vertex_unnamed_182 * vertex_unnamed_209;
				precise float vertex_unnamed_212 = vertex_unnamed_210 + vertex_unnamed_210;
				precise float vertex_unnamed_213 = vertex_unnamed_211 + vertex_unnamed_211;
				gl_Position.x = vertex_unnamed_212;
				gl_Position.y = vertex_unnamed_213;
				precise float vertex_unnamed_217 = (-0.0f) - vertex_unnamed_133;
				gl_Position.z = mad(vertex_unnamed_217, 9.9999997473787516355514526367188e-05f, vertex_unnamed_181);
				gl_Position.w = vertex_unnamed_182;
				vertex_output_1.x = vertex_input_3.x;
				vertex_output_1.y = vertex_input_3.y;
				vertex_output_2.x = vertex_input_2.x;
				vertex_output_2.y = vertex_input_2.y;
				vertex_output_2.z = vertex_input_2.z;
				vertex_output_2.w = vertex_input_2.w;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[6] = float4(_ScreenParams[0], _ScreenParams[1], _ScreenParams[2], _ScreenParams[3]);

				vertex_uniform_buffer_1[0] = float4(unity_ObjectToWorld[0][0], unity_ObjectToWorld[1][0], unity_ObjectToWorld[2][0], unity_ObjectToWorld[3][0]);
				vertex_uniform_buffer_1[1] = float4(unity_ObjectToWorld[0][1], unity_ObjectToWorld[1][1], unity_ObjectToWorld[2][1], unity_ObjectToWorld[3][1]);
				vertex_uniform_buffer_1[2] = float4(unity_ObjectToWorld[0][2], unity_ObjectToWorld[1][2], unity_ObjectToWorld[2][2], unity_ObjectToWorld[3][2]);
				vertex_uniform_buffer_1[3] = float4(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3], unity_ObjectToWorld[3][3]);

				vertex_uniform_buffer_2[5] = float4(glstate_matrix_projection[0][0], glstate_matrix_projection[1][0], glstate_matrix_projection[2][0], glstate_matrix_projection[3][0]);
				vertex_uniform_buffer_2[6] = float4(glstate_matrix_projection[0][1], glstate_matrix_projection[1][1], glstate_matrix_projection[2][1], glstate_matrix_projection[3][1]);
				vertex_uniform_buffer_2[7] = float4(glstate_matrix_projection[0][2], glstate_matrix_projection[1][2], glstate_matrix_projection[2][2], glstate_matrix_projection[3][2]);
				vertex_uniform_buffer_2[8] = float4(glstate_matrix_projection[0][3], glstate_matrix_projection[1][3], glstate_matrix_projection[2][3], glstate_matrix_projection[3][3]);

				vertex_uniform_buffer_2[9] = float4(unity_MatrixV[0][0], unity_MatrixV[1][0], unity_MatrixV[2][0], unity_MatrixV[3][0]);
				vertex_uniform_buffer_2[10] = float4(unity_MatrixV[0][1], unity_MatrixV[1][1], unity_MatrixV[2][1], unity_MatrixV[3][1]);
				vertex_uniform_buffer_2[11] = float4(unity_MatrixV[0][2], unity_MatrixV[1][2], unity_MatrixV[2][2], unity_MatrixV[3][2]);
				vertex_uniform_buffer_2[12] = float4(unity_MatrixV[0][3], unity_MatrixV[1][3], unity_MatrixV[2][3], unity_MatrixV[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vertex_input_2 = stage_input.vertex_input_2;
				vertex_input_3 = stage_input.vertex_input_3;
				vertex_input_4 = stage_input.vertex_input_4;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				return stage_output;
			}
			static float2 fragment_input_1;
			static float4 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float4 fragment_input_2 : COLOR; // COLOR
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				fragment_output_0.x = fragment_input_2.x;
				fragment_output_0.y = fragment_input_2.y;
				fragment_output_0.z = fragment_input_2.z;
				fragment_output_0.w = fragment_input_2.w;
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


			ENDHLSL
		}
	}
}
