Shader "Hidden/Custom Effects/FadeEffect"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			Name "Fog Effect"
			ZTest Always
			ZWrite Off
			Cull Off
			GpuProgramID 37711

			HLSLPROGRAM

			// https://docs.unity3d.com/Manual/SL-PragmaDirectives.html
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0


			float3 _WorldSpaceCameraPos;
			float _RenderViewportScaleFactor;
			float4x4 clipToWorld;

			static float4 vertex_uniform_buffer_0[35];
			static float4 gl_Position;
			static float3 vertex_input_0;
			static float2 vertex_input_1;
			static float2 vertex_output_1;
			static float3 vertex_output_2;

			struct Vertex_Stage_Input
			{
				float3 vertex_input_0 : POSITION; // POSITION
				float2 vertex_input_1 : TEXCOORD; // TEXCOORD
			};

			struct Vertex_Stage_Output
			{
				float2 vertex_output_1 : TEXCOORD; // TEXCOORD
				float2 vertex_output_1b : TEXCOORD1; // TEXCOORD_1
				float3 vertex_output_2 : TEXCOORD2; // TEXCOORD_2
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				gl_Position.x = vertex_input_0.x;
				gl_Position.y = vertex_input_0.y;
				gl_Position.z = 0.0f;
				gl_Position.w = 1.0f;
				precise float vertex_unnamed_46 = vertex_input_0.x + 1.0f;
				precise float vertex_unnamed_47 = vertex_input_0.y + 1.0f;
				float vertex_unnamed_49 = mad(vertex_unnamed_46, 0.5f, 0.0f);
				float vertex_unnamed_51 = mad(vertex_unnamed_47, -0.5f, 1.0f);
				precise float vertex_unnamed_58 = vertex_unnamed_49 * vertex_uniform_buffer_0[26u].x;
				precise float vertex_unnamed_59 = vertex_unnamed_51 * vertex_uniform_buffer_0[26u].x;
				vertex_output_1.x = vertex_unnamed_58;
				vertex_output_1.y = vertex_unnamed_59;
				float vertex_unnamed_62 = mad(vertex_unnamed_49, 2.0f, -1.0f);
				float vertex_unnamed_65 = mad(vertex_unnamed_51, 2.0f, -1.0f);
				vertex_output_1.x = mad(vertex_input_0.x, 0.5f, 0.5f);
				vertex_output_1.y = mad(vertex_input_0.y, -0.5f, 0.5f);
				precise float vertex_unnamed_80 = vertex_unnamed_65 * vertex_uniform_buffer_0[32u].x;
				precise float vertex_unnamed_81 = vertex_unnamed_65 * vertex_uniform_buffer_0[32u].y;
				precise float vertex_unnamed_82 = vertex_unnamed_65 * vertex_uniform_buffer_0[32u].z;
				precise float vertex_unnamed_98 = mad(vertex_uniform_buffer_0[31u].x, vertex_unnamed_62, vertex_unnamed_80) + vertex_uniform_buffer_0[34u].x;
				precise float vertex_unnamed_99 = mad(vertex_uniform_buffer_0[31u].y, vertex_unnamed_62, vertex_unnamed_81) + vertex_uniform_buffer_0[34u].y;
				precise float vertex_unnamed_100 = mad(vertex_uniform_buffer_0[31u].z, vertex_unnamed_62, vertex_unnamed_82) + vertex_uniform_buffer_0[34u].z;
				precise float vertex_unnamed_105 = (-0.0f) - vertex_uniform_buffer_0[16u].x;
				precise float vertex_unnamed_108 = (-0.0f) - vertex_uniform_buffer_0[16u].y;
				precise float vertex_unnamed_110 = (-0.0f) - vertex_uniform_buffer_0[16u].z;
				precise float vertex_unnamed_111 = vertex_unnamed_98 + vertex_unnamed_105;
				precise float vertex_unnamed_112 = vertex_unnamed_99 + vertex_unnamed_108;
				precise float vertex_unnamed_113 = vertex_unnamed_100 + vertex_unnamed_110;
				vertex_output_2.x = vertex_unnamed_111;
				vertex_output_2.y = vertex_unnamed_112;
				vertex_output_2.z = vertex_unnamed_113;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_uniform_buffer_0[16] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], vertex_uniform_buffer_0[16][3]);

				vertex_uniform_buffer_0[26] = float4(_RenderViewportScaleFactor, vertex_uniform_buffer_0[26][1], vertex_uniform_buffer_0[26][2], vertex_uniform_buffer_0[26][3]);

				vertex_uniform_buffer_0[31] = float4(clipToWorld[0][0], clipToWorld[1][0], clipToWorld[2][0], clipToWorld[3][0]);
				vertex_uniform_buffer_0[32] = float4(clipToWorld[0][1], clipToWorld[1][1], clipToWorld[2][1], clipToWorld[3][1]);
				vertex_uniform_buffer_0[33] = float4(clipToWorld[0][2], clipToWorld[1][2], clipToWorld[2][2], clipToWorld[3][2]);
				vertex_uniform_buffer_0[34] = float4(clipToWorld[0][3], clipToWorld[1][3], clipToWorld[2][3], clipToWorld[3][3]);

				vertex_input_0 = stage_input.vertex_input_0;
				vertex_input_1 = stage_input.vertex_input_1;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				stage_output.vertex_output_1 = vertex_output_1;
				stage_output.vertex_output_1b = vertex_output_1;
				stage_output.vertex_output_2 = vertex_output_2;
				return stage_output;
			}
			float4 _ProjectionParams;
			float4 unity_OrthoParams;
			float4 _ZBufferParams;
			float4 _SceneFogParams;
			float _SkyboxInfluence;

			static float4 fragment_uniform_buffer_0[31];
			Texture2D<float4> _MainTex;
			Texture2D<float4> _CameraDepthTexture;
			SamplerState sampler_CameraDepthTexture;
			SamplerState sampler_MainTex;

			static float2 fragment_input_1;
			static float2 fragment_input_1b;
			static float3 fragment_input_2;
			static float4 fragment_output_0;

			struct Fragment_Stage_Input
			{
				float2 fragment_input_1 : TEXCOORD; // TEXCOORD
				float2 fragment_input_1b : TEXCOORD1; // TEXCOORD_1
				float3 fragment_input_2 : TEXCOORD2; // TEXCOORD_2
			};

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				float4 fragment_unnamed_46 = _CameraDepthTexture.Sample(sampler_CameraDepthTexture, float2(fragment_input_1.x, fragment_input_1.y));
				float fragment_unnamed_48 = fragment_unnamed_46.x;
				precise float fragment_unnamed_62 = fragment_unnamed_48 * fragment_uniform_buffer_0[21u].x;
				precise float fragment_unnamed_63 = 1.0f / mad(fragment_uniform_buffer_0[21u].z, fragment_unnamed_48, fragment_uniform_buffer_0[21u].w);
				precise float fragment_unnamed_81 = (-0.0f) - mad(fragment_input_2.x, fragment_unnamed_63, fragment_uniform_buffer_0[16u].x);
				precise float fragment_unnamed_83 = (-0.0f) - mad(fragment_input_2.y, fragment_unnamed_63, fragment_uniform_buffer_0[16u].y);
				precise float fragment_unnamed_84 = (-0.0f) - mad(fragment_input_2.z, fragment_unnamed_63, fragment_uniform_buffer_0[16u].z);
				precise float fragment_unnamed_90 = fragment_unnamed_81 + fragment_uniform_buffer_0[16u].x;
				precise float fragment_unnamed_91 = fragment_unnamed_83 + fragment_uniform_buffer_0[16u].y;
				precise float fragment_unnamed_92 = fragment_unnamed_84 + fragment_uniform_buffer_0[16u].z;
				precise float fragment_unnamed_101 = (-0.0f) - fragment_uniform_buffer_0[17u].y;
				precise float fragment_unnamed_102 = sqrt(dot(float3(fragment_unnamed_90, fragment_unnamed_91, fragment_unnamed_92), float3(fragment_unnamed_90, fragment_unnamed_91, fragment_unnamed_92))) + fragment_unnamed_101;
				precise float fragment_unnamed_107 = fragment_unnamed_102 + fragment_uniform_buffer_0[29u].x;
				float fragment_unnamed_116 = clamp(mad(max(fragment_unnamed_107, 0.0f), fragment_uniform_buffer_0[29u].z, fragment_uniform_buffer_0[29u].w), 0.0f, 1.0f);
				precise float fragment_unnamed_117 = fragment_unnamed_116 + (-1.0f);
				precise float fragment_unnamed_128 = (-0.0f) - fragment_uniform_buffer_0[20u].w;
				precise float fragment_unnamed_129 = fragment_unnamed_128 + 1.0f;
				precise float fragment_unnamed_137 = (-0.0f) - fragment_uniform_buffer_0[20u].w;
				precise float fragment_unnamed_139 = mad(fragment_unnamed_137, fragment_unnamed_62, 1.0f) / mad(fragment_unnamed_129, fragment_unnamed_62, fragment_uniform_buffer_0[21u].y);
				precise float fragment_unnamed_147 = (-0.0f) - asfloat((0.9900000095367431640625f < fragment_unnamed_139) ? asuint(mad(fragment_uniform_buffer_0[30u].x, fragment_unnamed_117, 1.0f)) : asuint(fragment_unnamed_116));
				precise float fragment_unnamed_148 = fragment_unnamed_147 + 1.0f;
				float fragment_unnamed_152 = min(fragment_unnamed_148, fragment_uniform_buffer_0[29u].y);
				precise float fragment_unnamed_153 = (-0.0f) - fragment_unnamed_152;
				precise float fragment_unnamed_154 = fragment_unnamed_153 + 1.0f;
				float fragment_unnamed_155 = mad(fragment_unnamed_152, fragment_unnamed_154, fragment_unnamed_152);
				precise float fragment_unnamed_156 = (-0.0f) - fragment_unnamed_155;
				precise float fragment_unnamed_157 = fragment_unnamed_156 + 1.0f;
				float4 fragment_unnamed_164 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_1.x, fragment_input_1.y));
				float fragment_unnamed_169 = fragment_unnamed_164.w;
				precise float fragment_unnamed_170 = (-0.0f) - fragment_unnamed_169;
				fragment_output_0.w = mad(mad(fragment_unnamed_152, fragment_unnamed_157, fragment_unnamed_155), fragment_unnamed_170, fragment_unnamed_169);
				fragment_output_0.x = fragment_unnamed_164.x;
				fragment_output_0.y = fragment_unnamed_164.y;
				fragment_output_0.z = fragment_unnamed_164.z;
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[16] = float4(_WorldSpaceCameraPos[0], _WorldSpaceCameraPos[1], _WorldSpaceCameraPos[2], fragment_uniform_buffer_0[16][3]);

				fragment_uniform_buffer_0[17] = float4(_ProjectionParams[0], _ProjectionParams[1], _ProjectionParams[2], _ProjectionParams[3]);

				fragment_uniform_buffer_0[20] = float4(unity_OrthoParams[0], unity_OrthoParams[1], unity_OrthoParams[2], unity_OrthoParams[3]);

				fragment_uniform_buffer_0[21] = float4(_ZBufferParams[0], _ZBufferParams[1], _ZBufferParams[2], _ZBufferParams[3]);

				fragment_uniform_buffer_0[29] = float4(_SceneFogParams[0], _SceneFogParams[1], _SceneFogParams[2], _SceneFogParams[3]);

				fragment_uniform_buffer_0[30] = float4(_SkyboxInfluence, fragment_uniform_buffer_0[30][1], fragment_uniform_buffer_0[30][2], fragment_uniform_buffer_0[30][3]);

				fragment_input_1 = stage_input.fragment_input_1;
				fragment_input_1b = stage_input.fragment_input_1b;
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
