Shader "Hidden/ProBuilder/EdgePicker"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "ALWAYS" "ProBuilderPicker" = "EdgePass" }
		Pass
		{
			Name "Edges"
			Tags { "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "ALWAYS" "ProBuilderPicker" = "EdgePass" }
			Cull Off
			GpuProgramID 58041

			HLSLPROGRAM

			// https://docs.unity3d.com/Manual/SL-PragmaDirectives.html
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0


			float4 unity_OrthoParams;
			float4x4 unity_ObjectToWorld;
			float4x4 glstate_matrix_projection;
			float4x4 unity_MatrixV;

			static float4 vertex_uniform_buffer_0[9];
			static float4 vertex_uniform_buffer_1[4];
			static float4 vertex_uniform_buffer_2[13];
			static float4 gl_Position;
			static float4 vertex_input_0;
			static float4 vertex_input_1;
			static float4 vertex_output_1;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
				float4 vertex_input_1 : COLOR; // COLOR
			};

			struct Vertex_Stage_Output
			{
				float4 vertex_output_1 : COLOR; // COLOR
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				precise float vertex_unnamed_41 = vertex_input_0.y * vertex_uniform_buffer_1[1u].x;
				precise float vertex_unnamed_42 = vertex_input_0.y * vertex_uniform_buffer_1[1u].y;
				precise float vertex_unnamed_43 = vertex_input_0.y * vertex_uniform_buffer_1[1u].z;
				precise float vertex_unnamed_44 = vertex_input_0.y * vertex_uniform_buffer_1[1u].w;
				precise float vertex_unnamed_78 = mad(vertex_uniform_buffer_1[2u].x, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].x, vertex_input_0.x, vertex_unnamed_41)) + vertex_uniform_buffer_1[3u].x;
				precise float vertex_unnamed_79 = mad(vertex_uniform_buffer_1[2u].y, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].y, vertex_input_0.x, vertex_unnamed_42)) + vertex_uniform_buffer_1[3u].y;
				precise float vertex_unnamed_80 = mad(vertex_uniform_buffer_1[2u].z, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].z, vertex_input_0.x, vertex_unnamed_43)) + vertex_uniform_buffer_1[3u].z;
				precise float vertex_unnamed_81 = mad(vertex_uniform_buffer_1[2u].w, vertex_input_0.z, mad(vertex_uniform_buffer_1[0u].w, vertex_input_0.x, vertex_unnamed_44)) + vertex_uniform_buffer_1[3u].w;
				precise float vertex_unnamed_88 = vertex_unnamed_79 * vertex_uniform_buffer_2[10u].x;
				precise float vertex_unnamed_89 = vertex_unnamed_79 * vertex_uniform_buffer_2[10u].y;
				precise float vertex_unnamed_90 = vertex_unnamed_79 * vertex_uniform_buffer_2[10u].z;
				float vertex_unnamed_121 = mad(vertex_uniform_buffer_0[8u].w, 0.0099999904632568359375f, 0.9900000095367431640625f);
				precise float vertex_unnamed_127 = mad(vertex_uniform_buffer_2[12u].x, vertex_unnamed_81, mad(vertex_uniform_buffer_2[11u].x, vertex_unnamed_80, mad(vertex_uniform_buffer_2[9u].x, vertex_unnamed_78, vertex_unnamed_88))) * vertex_unnamed_121;
				precise float vertex_unnamed_128 = mad(vertex_uniform_buffer_2[12u].y, vertex_unnamed_81, mad(vertex_uniform_buffer_2[11u].y, vertex_unnamed_80, mad(vertex_uniform_buffer_2[9u].y, vertex_unnamed_78, vertex_unnamed_89))) * vertex_unnamed_121;
				precise float vertex_unnamed_129 = mad(vertex_uniform_buffer_2[12u].z, vertex_unnamed_81, mad(vertex_uniform_buffer_2[11u].z, vertex_unnamed_80, mad(vertex_uniform_buffer_2[9u].z, vertex_unnamed_78, vertex_unnamed_90))) * vertex_unnamed_121;
				precise float vertex_unnamed_130 = mad(vertex_uniform_buffer_0[8u].w, -0.050000011920928955078125f, 1.0f) * vertex_unnamed_129;
				precise float vertex_unnamed_138 = vertex_unnamed_128 * vertex_uniform_buffer_2[6u].x;
				precise float vertex_unnamed_139 = vertex_unnamed_128 * vertex_uniform_buffer_2[6u].y;
				precise float vertex_unnamed_140 = vertex_unnamed_128 * vertex_uniform_buffer_2[6u].z;
				precise float vertex_unnamed_141 = vertex_unnamed_128 * vertex_uniform_buffer_2[6u].w;
				precise float vertex_unnamed_170 = mad(vertex_uniform_buffer_2[7u].x, vertex_unnamed_130, mad(vertex_uniform_buffer_2[5u].x, vertex_unnamed_127, vertex_unnamed_138)) + vertex_uniform_buffer_2[8u].x;
				precise float vertex_unnamed_171 = mad(vertex_uniform_buffer_2[7u].y, vertex_unnamed_130, mad(vertex_uniform_buffer_2[5u].y, vertex_unnamed_127, vertex_unnamed_139)) + vertex_uniform_buffer_2[8u].y;
				precise float vertex_unnamed_172 = mad(vertex_uniform_buffer_2[7u].z, vertex_unnamed_130, mad(vertex_uniform_buffer_2[5u].z, vertex_unnamed_127, vertex_unnamed_140)) + vertex_uniform_buffer_2[8u].z;
				precise float vertex_unnamed_173 = mad(vertex_uniform_buffer_2[7u].w, vertex_unnamed_130, mad(vertex_uniform_buffer_2[5u].w, vertex_unnamed_127, vertex_unnamed_141)) + vertex_uniform_buffer_2[8u].w;
				gl_Position.x = vertex_unnamed_170;
				gl_Position.y = vertex_unnamed_171;
				gl_Position.z = vertex_unnamed_172;
				gl_Position.w = vertex_unnamed_173;
				vertex_output_1.x = vertex_input_1.x;
				vertex_output_1.y = vertex_input_1.y;
				vertex_output_1.z = vertex_input_1.z;
				vertex_output_1.w = vertex_input_1.w;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[8] = float4(unity_OrthoParams[0], unity_OrthoParams[1], unity_OrthoParams[2], unity_OrthoParams[3]);

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
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				return stage_output;
			}

			static float4 fragment_input_1;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float4 fragment_input_1 : COLOR; // COLOR
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				fragment_output_0.x = fragment_input_1.x;
				fragment_output_0.y = fragment_input_1.y;
				fragment_output_0.z = fragment_input_1.z;
				fragment_output_0.w = fragment_input_1.w;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_input_1 = stage_input.fragment_input_1;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}


			ENDHLSL
		}
	}
}
