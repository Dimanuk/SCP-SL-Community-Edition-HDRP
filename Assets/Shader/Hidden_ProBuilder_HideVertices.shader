Shader "Hidden/ProBuilder/HideVertices"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "IGNOREPROJECTOR" = "true" "RenderType" = "Geometry" }
		Pass
		{
			Tags { "IGNOREPROJECTOR" = "true" "RenderType" = "Geometry" }
			GpuProgramID 52387

			HLSLPROGRAM

			// https://docs.unity3d.com/Manual/SL-PragmaDirectives.html
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0


			static float4 gl_Position;
			static float4 vertex_input_0;

			struct Vertex_Stage_Input
			{
				float4 vertex_input_0 : POSITION; // POSITION
			};

			struct Vertex_Stage_Output
			{
				float4 gl_Position : SV_Position;
			};

			void vert_main()
			{
				gl_Position.x = 0.0f;
				gl_Position.y = 0.0f;
				gl_Position.z = 0.0f;
				gl_Position.w = 0.0f;
			}

			Vertex_Stage_Output vert(Vertex_Stage_Input stage_input)
			{
				vertex_input_0 = stage_input.vertex_input_0;
				vert_main();
				Vertex_Stage_Output stage_output;
				stage_output.gl_Position = gl_Position;
				return stage_output;
			}


			static float4 fragment_output_0;

			struct Fragment_Stage_Output
			{
				float4 fragment_output_0 : SV_Target0;
			};

			void frag_main()
			{
				fragment_output_0 = 0.0f.xxxx;
			}

			Fragment_Stage_Output frag()
			{
				frag_main();
				Fragment_Stage_Output stage_output;
				stage_output.fragment_output_0 = fragment_output_0;
				return stage_output;
			}



			ENDHLSL
		}
	}
}
