namespace AmplifyBloom
{
	public class AmplifyUtils
	{
		public static int MaskTextureId;

		public static int BlurRadiusId;

		public static string HighPrecisionKeyword = "AB_HIGH_PRECISION";

		public static string ShaderModeTag = "Mode";

		public static string ShaderModeValue = "Full";

		public static string DebugStr = "[AmplifyBloom] ";

		public static int UpscaleContributionId;

		public static int SourceContributionId;

		public static int LensStarburstRTId;

		public static int LensDirtRTId;

		public static int LensFlareRTId;

		public static int LensGlareRTId;

		public static int[] MipResultsRTS;

		public static int[] AnamorphicRTS;

		public static int[] AnamorphicGlareWeightsMatStr;

		public static int[] AnamorphicGlareOffsetsMatStr;

		public static int[] AnamorphicGlareWeightsStr;

		public static int[] UpscaleWeightsStr;

		public static int[] LensDirtWeightsStr;

		public static int[] LensStarburstWeightsStr;

		public static int BloomRangeId;

		public static int LensDirtStrengthId;

		public static int BloomParamsId;

		public static int TempFilterValueId;

		public static int LensFlareStarMatrixId;

		public static int LensFlareStarburstStrengthId;

		public static int LensFlareGhostsParamsId;

		public static int LensFlareLUTId;

		public static int LensFlareHaloParamsId;

		public static int LensFlareGhostChrDistortionId;

		public static int LensFlareHaloChrDistortionId;

		public static int BokehParamsId = -1;

		public static global::UnityEngine.RenderTextureFormat CurrentRTFormat = global::UnityEngine.RenderTextureFormat.DefaultHDR;

		public static global::UnityEngine.FilterMode CurrentFilterMode = global::UnityEngine.FilterMode.Bilinear;

		public static global::UnityEngine.TextureWrapMode CurrentWrapMode = global::UnityEngine.TextureWrapMode.Clamp;

		public static global::UnityEngine.RenderTextureReadWrite CurrentReadWriteMode = global::UnityEngine.RenderTextureReadWrite.sRGB;

		public static bool IsInitialized = false;

		private static global::System.Collections.Generic.List<global::UnityEngine.RenderTexture> _allocatedRT = new global::System.Collections.Generic.List<global::UnityEngine.RenderTexture>();

		public static void InitializeIds()
		{
			IsInitialized = true;
			MaskTextureId = global::UnityEngine.Shader.PropertyToID("_MaskTex");
			MipResultsRTS = new int[6]
			{
				global::UnityEngine.Shader.PropertyToID("_MipResultsRTS0"),
				global::UnityEngine.Shader.PropertyToID("_MipResultsRTS1"),
				global::UnityEngine.Shader.PropertyToID("_MipResultsRTS2"),
				global::UnityEngine.Shader.PropertyToID("_MipResultsRTS3"),
				global::UnityEngine.Shader.PropertyToID("_MipResultsRTS4"),
				global::UnityEngine.Shader.PropertyToID("_MipResultsRTS5")
			};
			AnamorphicRTS = new int[8]
			{
				global::UnityEngine.Shader.PropertyToID("_AnamorphicRTS0"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicRTS1"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicRTS2"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicRTS3"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicRTS4"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicRTS5"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicRTS6"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicRTS7")
			};
			AnamorphicGlareWeightsMatStr = new int[4]
			{
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeightsMat0"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeightsMat1"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeightsMat2"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeightsMat3")
			};
			AnamorphicGlareOffsetsMatStr = new int[4]
			{
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareOffsetsMat0"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareOffsetsMat1"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareOffsetsMat2"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareOffsetsMat3")
			};
			AnamorphicGlareWeightsStr = new int[16]
			{
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights0"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights1"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights2"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights3"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights4"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights5"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights6"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights7"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights8"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights9"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights10"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights11"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights12"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights13"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights14"),
				global::UnityEngine.Shader.PropertyToID("_AnamorphicGlareWeights15")
			};
			UpscaleWeightsStr = new int[8]
			{
				global::UnityEngine.Shader.PropertyToID("_UpscaleWeights0"),
				global::UnityEngine.Shader.PropertyToID("_UpscaleWeights1"),
				global::UnityEngine.Shader.PropertyToID("_UpscaleWeights2"),
				global::UnityEngine.Shader.PropertyToID("_UpscaleWeights3"),
				global::UnityEngine.Shader.PropertyToID("_UpscaleWeights4"),
				global::UnityEngine.Shader.PropertyToID("_UpscaleWeights5"),
				global::UnityEngine.Shader.PropertyToID("_UpscaleWeights6"),
				global::UnityEngine.Shader.PropertyToID("_UpscaleWeights7")
			};
			LensDirtWeightsStr = new int[8]
			{
				global::UnityEngine.Shader.PropertyToID("_LensDirtWeights0"),
				global::UnityEngine.Shader.PropertyToID("_LensDirtWeights1"),
				global::UnityEngine.Shader.PropertyToID("_LensDirtWeights2"),
				global::UnityEngine.Shader.PropertyToID("_LensDirtWeights3"),
				global::UnityEngine.Shader.PropertyToID("_LensDirtWeights4"),
				global::UnityEngine.Shader.PropertyToID("_LensDirtWeights5"),
				global::UnityEngine.Shader.PropertyToID("_LensDirtWeights6"),
				global::UnityEngine.Shader.PropertyToID("_LensDirtWeights7")
			};
			LensStarburstWeightsStr = new int[8]
			{
				global::UnityEngine.Shader.PropertyToID("_LensStarburstWeights0"),
				global::UnityEngine.Shader.PropertyToID("_LensStarburstWeights1"),
				global::UnityEngine.Shader.PropertyToID("_LensStarburstWeights2"),
				global::UnityEngine.Shader.PropertyToID("_LensStarburstWeights3"),
				global::UnityEngine.Shader.PropertyToID("_LensStarburstWeights4"),
				global::UnityEngine.Shader.PropertyToID("_LensStarburstWeights5"),
				global::UnityEngine.Shader.PropertyToID("_LensStarburstWeights6"),
				global::UnityEngine.Shader.PropertyToID("_LensStarburstWeights7")
			};
			BloomRangeId = global::UnityEngine.Shader.PropertyToID("_BloomRange");
			LensDirtStrengthId = global::UnityEngine.Shader.PropertyToID("_LensDirtStrength");
			BloomParamsId = global::UnityEngine.Shader.PropertyToID("_BloomParams");
			TempFilterValueId = global::UnityEngine.Shader.PropertyToID("_TempFilterValue");
			LensFlareStarMatrixId = global::UnityEngine.Shader.PropertyToID("_LensFlareStarMatrix");
			LensFlareStarburstStrengthId = global::UnityEngine.Shader.PropertyToID("_LensFlareStarburstStrength");
			LensFlareGhostsParamsId = global::UnityEngine.Shader.PropertyToID("_LensFlareGhostsParams");
			LensFlareLUTId = global::UnityEngine.Shader.PropertyToID("_LensFlareLUT");
			LensFlareHaloParamsId = global::UnityEngine.Shader.PropertyToID("_LensFlareHaloParams");
			LensFlareGhostChrDistortionId = global::UnityEngine.Shader.PropertyToID("_LensFlareGhostChrDistortion");
			LensFlareHaloChrDistortionId = global::UnityEngine.Shader.PropertyToID("_LensFlareHaloChrDistortion");
			BokehParamsId = global::UnityEngine.Shader.PropertyToID("_BokehParams");
			BlurRadiusId = global::UnityEngine.Shader.PropertyToID("_BlurRadius");
			LensStarburstRTId = global::UnityEngine.Shader.PropertyToID("_LensStarburst");
			LensDirtRTId = global::UnityEngine.Shader.PropertyToID("_LensDirt");
			LensFlareRTId = global::UnityEngine.Shader.PropertyToID("_LensFlare");
			LensGlareRTId = global::UnityEngine.Shader.PropertyToID("_LensGlare");
			SourceContributionId = global::UnityEngine.Shader.PropertyToID("_SourceContribution");
			UpscaleContributionId = global::UnityEngine.Shader.PropertyToID("_UpscaleContribution");
		}

		public static void DebugLog(string value, global::AmplifyBloom.LogType type)
		{
			switch (type)
			{
			case global::AmplifyBloom.LogType.Normal:
				global::UnityEngine.Debug.Log(DebugStr + value);
				break;
			case global::AmplifyBloom.LogType.Warning:
				global::UnityEngine.Debug.LogWarning(DebugStr + value);
				break;
			case global::AmplifyBloom.LogType.Error:
				global::UnityEngine.Debug.LogError(DebugStr + value);
				break;
			}
		}

		public static global::UnityEngine.RenderTexture GetTempRenderTarget(int width, int height)
		{
			global::UnityEngine.RenderTexture temporary = global::UnityEngine.RenderTexture.GetTemporary(width, height, 0, CurrentRTFormat, CurrentReadWriteMode);
			temporary.filterMode = CurrentFilterMode;
			temporary.wrapMode = CurrentWrapMode;
			_allocatedRT.Add(temporary);
			return temporary;
		}

		public static void ReleaseTempRenderTarget(global::UnityEngine.RenderTexture renderTarget)
		{
			if (renderTarget != null && _allocatedRT.Remove(renderTarget))
			{
				renderTarget.DiscardContents();
				global::UnityEngine.RenderTexture.ReleaseTemporary(renderTarget);
			}
		}

		public static void ReleaseAllRT()
		{
			for (int i = 0; i < _allocatedRT.Count; i++)
			{
				_allocatedRT[i].DiscardContents();
				global::UnityEngine.RenderTexture.ReleaseTemporary(_allocatedRT[i]);
			}
			_allocatedRT.Clear();
		}

		public static void EnsureKeywordEnabled(global::UnityEngine.Material mat, string keyword, bool state)
		{
			if (mat != null)
			{
				if (state && !mat.IsKeywordEnabled(keyword))
				{
					mat.EnableKeyword(keyword);
				}
				else if (!state && mat.IsKeywordEnabled(keyword))
				{
					mat.DisableKeyword(keyword);
				}
			}
		}
	}
}
