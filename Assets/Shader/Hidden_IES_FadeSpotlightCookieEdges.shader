Shader "Hidden/IES/FadeSpotlightCookieEdges"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			ZTest Always
			ZWrite Off
			Cull Off
			GpuProgramID 63358

			HLSLPROGRAM

			// https://docs.unity3d.com/Manual/SL-PragmaDirectives.html
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0


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

			float _HorizontalFadeDistance;
			float _VerticalFadeDistance;
			float _VerticalCenter;

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
				precise float fragment_unnamed_34 = (-0.0f) - fragment_uniform_buffer_0[2u].z;
				precise float fragment_unnamed_36 = fragment_unnamed_34 + 1.0f;
				precise float fragment_unnamed_38 = (-0.0f) - fragment_unnamed_36;
				precise float fragment_unnamed_47 = asfloat(3204448256u) + fragment_input_0.x;
				precise float fragment_unnamed_48 = fragment_unnamed_38 + fragment_input_0.y;
				precise float fragment_unnamed_49 = fragment_unnamed_47 * fragment_unnamed_47;
				precise float fragment_unnamed_50 = fragment_unnamed_48 * fragment_unnamed_48;
				precise float fragment_unnamed_59 = fragment_uniform_buffer_0[2u].x * fragment_uniform_buffer_0[2u].x;
				precise float fragment_unnamed_60 = fragment_uniform_buffer_0[2u].y * fragment_uniform_buffer_0[2u].y;
				precise float fragment_unnamed_61 = fragment_unnamed_49 / fragment_unnamed_59;
				precise float fragment_unnamed_62 = fragment_unnamed_50 / fragment_unnamed_60;
				precise float fragment_unnamed_63 = fragment_unnamed_62 + fragment_unnamed_61;
				float fragment_unnamed_65 = log2(fragment_unnamed_63);
				float4 fragment_unnamed_73 = _MainTex.Sample(sampler_MainTex, float2(fragment_input_0.x, fragment_input_0.y));
				float fragment_unnamed_75 = fragment_unnamed_73.x;
				float fragment_unnamed_76 = fragment_unnamed_73.y;
				float fragment_unnamed_77 = fragment_unnamed_73.z;
				float fragment_unnamed_78 = fragment_unnamed_73.w;
				precise float fragment_unnamed_79 = (-0.0f) - fragment_unnamed_75;
				precise float fragment_unnamed_80 = (-0.0f) - fragment_unnamed_76;
				precise float fragment_unnamed_81 = (-0.0f) - fragment_unnamed_77;
				precise float fragment_unnamed_82 = (-0.0f) - fragment_unnamed_78;
				precise float fragment_unnamed_83 = fragment_unnamed_79 + 1.0f;
				precise float fragment_unnamed_84 = fragment_unnamed_80 + 1.0f;
				precise float fragment_unnamed_85 = fragment_unnamed_81 + 1.0f;
				precise float fragment_unnamed_86 = fragment_unnamed_82 + 1.0f;
				precise float fragment_unnamed_87 = fragment_unnamed_83 * fragment_unnamed_83;
				precise float fragment_unnamed_88 = fragment_unnamed_84 * fragment_unnamed_84;
				precise float fragment_unnamed_89 = fragment_unnamed_85 * fragment_unnamed_85;
				precise float fragment_unnamed_90 = fragment_unnamed_86 * fragment_unnamed_86;
				precise float fragment_unnamed_91 = fragment_unnamed_87 * fragment_unnamed_87;
				precise float fragment_unnamed_92 = fragment_unnamed_88 * fragment_unnamed_88;
				precise float fragment_unnamed_93 = fragment_unnamed_89 * fragment_unnamed_89;
				precise float fragment_unnamed_94 = fragment_unnamed_90 * fragment_unnamed_90;
				precise float fragment_unnamed_95 = fragment_unnamed_87 * fragment_unnamed_91;
				precise float fragment_unnamed_96 = fragment_unnamed_88 * fragment_unnamed_92;
				precise float fragment_unnamed_97 = fragment_unnamed_89 * fragment_unnamed_93;
				precise float fragment_unnamed_98 = fragment_unnamed_90 * fragment_unnamed_94;
				precise float fragment_unnamed_105 = fragment_unnamed_65 * mad(fragment_unnamed_95, 6.0f, 2.0f);
				precise float fragment_unnamed_106 = fragment_unnamed_65 * mad(fragment_unnamed_96, 6.0f, 2.0f);
				precise float fragment_unnamed_107 = fragment_unnamed_65 * mad(fragment_unnamed_97, 6.0f, 2.0f);
				precise float fragment_unnamed_108 = fragment_unnamed_65 * mad(fragment_unnamed_98, 6.0f, 2.0f);
				precise float fragment_unnamed_113 = (-0.0f) - fragment_unnamed_75;
				precise float fragment_unnamed_114 = (-0.0f) - fragment_unnamed_76;
				precise float fragment_unnamed_115 = (-0.0f) - fragment_unnamed_77;
				precise float fragment_unnamed_116 = (-0.0f) - fragment_unnamed_78;
				fragment_output_0.x = mad(exp2(fragment_unnamed_105), fragment_unnamed_113, fragment_unnamed_75);
				fragment_output_0.y = mad(exp2(fragment_unnamed_106), fragment_unnamed_114, fragment_unnamed_76);
				fragment_output_0.z = mad(exp2(fragment_unnamed_107), fragment_unnamed_115, fragment_unnamed_77);
				fragment_output_0.w = mad(exp2(fragment_unnamed_108), fragment_unnamed_116, fragment_unnamed_78);
			}

			Fragment_Stage_Output frag(Fragment_Stage_Input stage_input)
			{
				fragment_uniform_buffer_0[2] = float4(_HorizontalFadeDistance, fragment_uniform_buffer_0[2][1], fragment_uniform_buffer_0[2][2], fragment_uniform_buffer_0[2][3]);

				fragment_uniform_buffer_0[2] = float4(fragment_uniform_buffer_0[2][0], _VerticalFadeDistance, fragment_uniform_buffer_0[2][2], fragment_uniform_buffer_0[2][3]);

				fragment_uniform_buffer_0[2] = float4(fragment_uniform_buffer_0[2][0], fragment_uniform_buffer_0[2][1], _VerticalCenter, fragment_uniform_buffer_0[2][3]);

				fragment_input_0 = stage_input.fragment_input_0;
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}


			ENDHLSL
		}
	}
}
