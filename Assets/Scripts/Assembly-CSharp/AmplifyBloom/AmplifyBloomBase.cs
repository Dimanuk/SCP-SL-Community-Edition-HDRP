namespace AmplifyBloom
{
	[global::System.Serializable]
	[global::UnityEngine.AddComponentMenu("")]
	public class AmplifyBloomBase : global::UnityEngine.MonoBehaviour
	{
		public const int MaxGhosts = 5;

		public const int MinDownscales = 1;

		public const int MaxDownscales = 6;

		public const int MaxGaussian = 8;

		private const float MaxDirtIntensity = 1f;

		private const float MaxStarburstIntensity = 1f;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Texture m_maskTexture;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.RenderTexture m_targetTexture;

		[global::UnityEngine.SerializeField]
		private bool m_showDebugMessages = true;

		[global::UnityEngine.SerializeField]
		private int m_softMaxdownscales = 6;

		[global::UnityEngine.SerializeField]
		private global::AmplifyBloom.DebugToScreenEnum m_debugToScreen;

		[global::UnityEngine.SerializeField]
		private bool m_highPrecision;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Vector4 m_bloomRange = new global::UnityEngine.Vector4(500f, 1f, 0f, 0f);

		[global::UnityEngine.SerializeField]
		private float m_overallThreshold = 0.53f;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Vector4 m_bloomParams = new global::UnityEngine.Vector4(0.8f, 1f, 1f, 1f);

		[global::UnityEngine.SerializeField]
		private bool m_temporalFilteringActive;

		[global::UnityEngine.SerializeField]
		private float m_temporalFilteringValue = 0.05f;

		[global::UnityEngine.SerializeField]
		private int m_bloomDownsampleCount = 6;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.AnimationCurve m_temporalFilteringCurve;

		[global::UnityEngine.SerializeField]
		private bool m_separateFeaturesThreshold;

		[global::UnityEngine.SerializeField]
		private float m_featuresThreshold = 0.05f;

		[global::UnityEngine.SerializeField]
		private global::AmplifyBloom.AmplifyLensFlare m_lensFlare = new global::AmplifyBloom.AmplifyLensFlare();

		[global::UnityEngine.SerializeField]
		private bool m_applyLensDirt = true;

		[global::UnityEngine.SerializeField]
		private float m_lensDirtStrength = 2f;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Texture m_lensDirtTexture;

		[global::UnityEngine.SerializeField]
		private bool m_applyLensStardurst = true;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Texture m_lensStardurstTex;

		[global::UnityEngine.SerializeField]
		private float m_lensStarburstStrength = 2f;

		[global::UnityEngine.SerializeField]
		private global::AmplifyBloom.AmplifyGlare m_anamorphicGlare = new global::AmplifyBloom.AmplifyGlare();

		[global::UnityEngine.SerializeField]
		private global::AmplifyBloom.AmplifyBokeh m_bokehFilter = new global::AmplifyBloom.AmplifyBokeh();

		[global::UnityEngine.SerializeField]
		private float[] m_upscaleWeights = new float[6] { 0.0842f, 0.1282f, 0.1648f, 0.2197f, 0.2197f, 0.1831f };

		[global::UnityEngine.SerializeField]
		private float[] m_gaussianRadius = new float[6] { 1f, 1f, 1f, 1f, 1f, 1f };

		[global::UnityEngine.SerializeField]
		private int[] m_gaussianSteps = new int[6] { 1, 1, 1, 1, 1, 1 };

		[global::UnityEngine.SerializeField]
		private float[] m_lensDirtWeights = new float[6] { 0.067f, 0.102f, 0.1311f, 0.1749f, 0.2332f, 0.3f };

		[global::UnityEngine.SerializeField]
		private float[] m_lensStarburstWeights = new float[6] { 0.067f, 0.102f, 0.1311f, 0.1749f, 0.2332f, 0.3f };

		[global::UnityEngine.SerializeField]
		private bool[] m_downscaleSettingsFoldout = new bool[6];

		[global::UnityEngine.SerializeField]
		private int m_featuresSourceId;

		[global::UnityEngine.SerializeField]
		private global::AmplifyBloom.UpscaleQualityEnum m_upscaleQuality;

		[global::UnityEngine.SerializeField]
		private global::AmplifyBloom.MainThresholdSizeEnum m_mainThresholdSize;

		private global::UnityEngine.Transform m_cameraTransform;

		private global::UnityEngine.Matrix4x4 m_starburstMat;

		private global::UnityEngine.Shader m_bloomShader;

		private global::UnityEngine.Material m_bloomMaterial;

		private global::UnityEngine.Shader m_finalCompositionShader;

		private global::UnityEngine.Material m_finalCompositionMaterial;

		private global::UnityEngine.RenderTexture m_tempFilterBuffer;

		private global::UnityEngine.Camera m_camera;

		private global::UnityEngine.RenderTexture[] m_tempUpscaleRTs = new global::UnityEngine.RenderTexture[6];

		private global::UnityEngine.RenderTexture[] m_tempAuxDownsampleRTs = new global::UnityEngine.RenderTexture[6];

		private global::UnityEngine.Vector2[] m_tempDownsamplesSizes = new global::UnityEngine.Vector2[6];

		private bool silentError;

		public global::AmplifyBloom.AmplifyGlare LensGlareInstance => m_anamorphicGlare;

		public global::AmplifyBloom.AmplifyBokeh BokehFilterInstance => m_bokehFilter;

		public global::AmplifyBloom.AmplifyLensFlare LensFlareInstance => m_lensFlare;

		public bool ApplyLensDirt
		{
			get
			{
				return m_applyLensDirt;
			}
			set
			{
				m_applyLensDirt = value;
			}
		}

		public float LensDirtStrength
		{
			get
			{
				return m_lensDirtStrength;
			}
			set
			{
				m_lensDirtStrength = ((value < 0f) ? 0f : value);
			}
		}

		public global::UnityEngine.Texture LensDirtTexture
		{
			get
			{
				return m_lensDirtTexture;
			}
			set
			{
				m_lensDirtTexture = value;
			}
		}

		public bool ApplyLensStardurst
		{
			get
			{
				return m_applyLensStardurst;
			}
			set
			{
				m_applyLensStardurst = value;
			}
		}

		public global::UnityEngine.Texture LensStardurstTex
		{
			get
			{
				return m_lensStardurstTex;
			}
			set
			{
				m_lensStardurstTex = value;
			}
		}

		public float LensStarburstStrength
		{
			get
			{
				return m_lensStarburstStrength;
			}
			set
			{
				m_lensStarburstStrength = ((value < 0f) ? 0f : value);
			}
		}

		public global::AmplifyBloom.PrecisionModes CurrentPrecisionMode
		{
			get
			{
				if (m_highPrecision)
				{
					return global::AmplifyBloom.PrecisionModes.High;
				}
				return global::AmplifyBloom.PrecisionModes.Low;
			}
			set
			{
				HighPrecision = value == global::AmplifyBloom.PrecisionModes.High;
			}
		}

		public bool HighPrecision
		{
			get
			{
				return m_highPrecision;
			}
			set
			{
				if (m_highPrecision != value)
				{
					m_highPrecision = value;
					CleanTempFilterRT();
				}
			}
		}

		public float BloomRange
		{
			get
			{
				return m_bloomRange.x;
			}
			set
			{
				m_bloomRange.x = ((value < 0f) ? 0f : value);
			}
		}

		public float OverallThreshold
		{
			get
			{
				return m_overallThreshold;
			}
			set
			{
				m_overallThreshold = ((value < 0f) ? 0f : value);
			}
		}

		public global::UnityEngine.Vector4 BloomParams
		{
			get
			{
				return m_bloomParams;
			}
			set
			{
				m_bloomParams = value;
			}
		}

		public float OverallIntensity
		{
			get
			{
				return m_bloomParams.x;
			}
			set
			{
				m_bloomParams.x = ((value < 0f) ? 0f : value);
			}
		}

		public float BloomScale
		{
			get
			{
				return m_bloomParams.w;
			}
			set
			{
				m_bloomParams.w = ((value < 0f) ? 0f : value);
			}
		}

		public float UpscaleBlurRadius
		{
			get
			{
				return m_bloomParams.z;
			}
			set
			{
				m_bloomParams.z = value;
			}
		}

		public bool TemporalFilteringActive
		{
			get
			{
				return m_temporalFilteringActive;
			}
			set
			{
				if (m_temporalFilteringActive != value)
				{
					CleanTempFilterRT();
				}
				m_temporalFilteringActive = value;
			}
		}

		public float TemporalFilteringValue
		{
			get
			{
				return m_temporalFilteringValue;
			}
			set
			{
				m_temporalFilteringValue = value;
			}
		}

		public int SoftMaxdownscales => m_softMaxdownscales;

		public int BloomDownsampleCount
		{
			get
			{
				return m_bloomDownsampleCount;
			}
			set
			{
				m_bloomDownsampleCount = global::UnityEngine.Mathf.Clamp(value, 1, m_softMaxdownscales);
			}
		}

		public int FeaturesSourceId
		{
			get
			{
				return m_featuresSourceId;
			}
			set
			{
				m_featuresSourceId = global::UnityEngine.Mathf.Clamp(value, 0, m_bloomDownsampleCount - 1);
			}
		}

		public bool[] DownscaleSettingsFoldout => m_downscaleSettingsFoldout;

		public float[] UpscaleWeights => m_upscaleWeights;

		public float[] LensDirtWeights => m_lensDirtWeights;

		public float[] LensStarburstWeights => m_lensStarburstWeights;

		public float[] GaussianRadius => m_gaussianRadius;

		public int[] GaussianSteps => m_gaussianSteps;

		public global::UnityEngine.AnimationCurve TemporalFilteringCurve
		{
			get
			{
				return m_temporalFilteringCurve;
			}
			set
			{
				m_temporalFilteringCurve = value;
			}
		}

		public bool SeparateFeaturesThreshold
		{
			get
			{
				return m_separateFeaturesThreshold;
			}
			set
			{
				m_separateFeaturesThreshold = value;
			}
		}

		public float FeaturesThreshold
		{
			get
			{
				return m_featuresThreshold;
			}
			set
			{
				m_featuresThreshold = ((value < 0f) ? 0f : value);
			}
		}

		public global::AmplifyBloom.DebugToScreenEnum DebugToScreen
		{
			get
			{
				return m_debugToScreen;
			}
			set
			{
				m_debugToScreen = value;
			}
		}

		public global::AmplifyBloom.UpscaleQualityEnum UpscaleQuality
		{
			get
			{
				return m_upscaleQuality;
			}
			set
			{
				m_upscaleQuality = value;
			}
		}

		public bool ShowDebugMessages
		{
			get
			{
				return m_showDebugMessages;
			}
			set
			{
				m_showDebugMessages = value;
			}
		}

		public global::AmplifyBloom.MainThresholdSizeEnum MainThresholdSize
		{
			get
			{
				return m_mainThresholdSize;
			}
			set
			{
				m_mainThresholdSize = value;
			}
		}

		public global::UnityEngine.RenderTexture TargetTexture
		{
			get
			{
				return m_targetTexture;
			}
			set
			{
				m_targetTexture = value;
			}
		}

		public global::UnityEngine.Texture MaskTexture
		{
			get
			{
				return m_maskTexture;
			}
			set
			{
				m_maskTexture = value;
			}
		}

		public bool ApplyBokehFilter
		{
			get
			{
				return m_bokehFilter.ApplyBokeh;
			}
			set
			{
				m_bokehFilter.ApplyBokeh = value;
			}
		}

		public bool ApplyLensFlare
		{
			get
			{
				return m_lensFlare.ApplyLensFlare;
			}
			set
			{
				m_lensFlare.ApplyLensFlare = value;
			}
		}

		public bool ApplyLensGlare
		{
			get
			{
				return m_anamorphicGlare.ApplyLensGlare;
			}
			set
			{
				m_anamorphicGlare.ApplyLensGlare = value;
			}
		}

		private void Awake()
		{
			if (global::UnityEngine.SystemInfo.graphicsDeviceType == global::UnityEngine.Rendering.GraphicsDeviceType.Null)
			{
				global::AmplifyBloom.AmplifyUtils.DebugLog("Null graphics device detected. Skipping effect silently.", global::AmplifyBloom.LogType.Error);
				silentError = true;
				return;
			}
			if (!global::AmplifyBloom.AmplifyUtils.IsInitialized)
			{
				global::AmplifyBloom.AmplifyUtils.InitializeIds();
			}
			m_anamorphicGlare.Init();
			m_lensFlare.Init();
			for (int i = 0; i < 6; i++)
			{
				m_tempDownsamplesSizes[i] = new global::UnityEngine.Vector2(0f, 0f);
			}
			m_cameraTransform = base.transform;
			m_tempFilterBuffer = null;
			m_starburstMat = global::UnityEngine.Matrix4x4.identity;
			if (m_temporalFilteringCurve == null)
			{
				m_temporalFilteringCurve = new global::UnityEngine.AnimationCurve(new global::UnityEngine.Keyframe(0f, 0f), new global::UnityEngine.Keyframe(1f, 0.999f));
			}
			m_bloomShader = global::UnityEngine.Shader.Find("Hidden/AmplifyBloom");
			if (m_bloomShader != null)
			{
				m_bloomMaterial = new global::UnityEngine.Material(m_bloomShader);
				m_bloomMaterial.hideFlags = global::UnityEngine.HideFlags.DontSave;
			}
			else
			{
				global::AmplifyBloom.AmplifyUtils.DebugLog("Main Bloom shader not found", global::AmplifyBloom.LogType.Error);
				base.gameObject.SetActive(value: false);
			}
			m_finalCompositionShader = global::UnityEngine.Shader.Find("Hidden/BloomFinal");
			if (m_finalCompositionShader != null)
			{
				m_finalCompositionMaterial = new global::UnityEngine.Material(m_finalCompositionShader);
				if (!m_finalCompositionMaterial.GetTag(global::AmplifyBloom.AmplifyUtils.ShaderModeTag, searchFallbacks: false).Equals(global::AmplifyBloom.AmplifyUtils.ShaderModeValue))
				{
					if (m_showDebugMessages)
					{
						global::AmplifyBloom.AmplifyUtils.DebugLog("Amplify Bloom is running on a limited hardware and may lead to a decrease on its visual quality.", global::AmplifyBloom.LogType.Warning);
					}
				}
				else
				{
					m_softMaxdownscales = 6;
				}
				m_finalCompositionMaterial.hideFlags = global::UnityEngine.HideFlags.DontSave;
				if (m_lensDirtTexture == null)
				{
					m_lensDirtTexture = m_finalCompositionMaterial.GetTexture(global::AmplifyBloom.AmplifyUtils.LensDirtRTId);
				}
				if (m_lensStardurstTex == null)
				{
					m_lensStardurstTex = m_finalCompositionMaterial.GetTexture(global::AmplifyBloom.AmplifyUtils.LensStarburstRTId);
				}
			}
			else
			{
				global::AmplifyBloom.AmplifyUtils.DebugLog("Bloom Composition shader not found", global::AmplifyBloom.LogType.Error);
				base.gameObject.SetActive(value: false);
			}
			m_camera = GetComponent<global::UnityEngine.Camera>();
			m_camera.depthTextureMode |= global::UnityEngine.DepthTextureMode.Depth;
			m_lensFlare.CreateLUTexture();
		}

		private void OnDestroy()
		{
			if (m_bokehFilter != null)
			{
				m_bokehFilter.Destroy();
				m_bokehFilter = null;
			}
			if (m_anamorphicGlare != null)
			{
				m_anamorphicGlare.Destroy();
				m_anamorphicGlare = null;
			}
			if (m_lensFlare != null)
			{
				m_lensFlare.Destroy();
				m_lensFlare = null;
			}
		}

		private void ApplyGaussianBlur(global::UnityEngine.RenderTexture renderTexture, int amount, float radius = 1f, bool applyTemporal = false)
		{
			if (amount == 0)
			{
				return;
			}
			m_bloomMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.BlurRadiusId, radius);
			global::UnityEngine.RenderTexture tempRenderTarget = global::AmplifyBloom.AmplifyUtils.GetTempRenderTarget(renderTexture.width, renderTexture.height);
			for (int i = 0; i < amount; i++)
			{
				tempRenderTarget.DiscardContents();
				global::UnityEngine.Graphics.Blit(renderTexture, tempRenderTarget, m_bloomMaterial, 14);
				if (m_temporalFilteringActive && applyTemporal && i == amount - 1)
				{
					if (m_tempFilterBuffer != null && m_temporalFilteringActive)
					{
						float value = m_temporalFilteringCurve.Evaluate(m_temporalFilteringValue);
						m_bloomMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.TempFilterValueId, value);
						m_bloomMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.AnamorphicRTS[0], m_tempFilterBuffer);
						renderTexture.DiscardContents();
						global::UnityEngine.Graphics.Blit(tempRenderTarget, renderTexture, m_bloomMaterial, 16);
					}
					else
					{
						renderTexture.DiscardContents();
						global::UnityEngine.Graphics.Blit(tempRenderTarget, renderTexture, m_bloomMaterial, 15);
					}
					bool flag = false;
					if (m_tempFilterBuffer != null)
					{
						if (m_tempFilterBuffer.format != renderTexture.format || m_tempFilterBuffer.width != renderTexture.width || m_tempFilterBuffer.height != renderTexture.height)
						{
							CleanTempFilterRT();
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						CreateTempFilterRT(renderTexture);
					}
					m_tempFilterBuffer.DiscardContents();
					global::UnityEngine.Graphics.Blit(renderTexture, m_tempFilterBuffer);
				}
				else
				{
					renderTexture.DiscardContents();
					global::UnityEngine.Graphics.Blit(tempRenderTarget, renderTexture, m_bloomMaterial, 15);
				}
			}
			global::AmplifyBloom.AmplifyUtils.ReleaseTempRenderTarget(tempRenderTarget);
		}

		private void CreateTempFilterRT(global::UnityEngine.RenderTexture source)
		{
			if (m_tempFilterBuffer != null)
			{
				CleanTempFilterRT();
			}
			m_tempFilterBuffer = new global::UnityEngine.RenderTexture(source.width, source.height, 0, source.format, global::AmplifyBloom.AmplifyUtils.CurrentReadWriteMode);
			m_tempFilterBuffer.filterMode = global::AmplifyBloom.AmplifyUtils.CurrentFilterMode;
			m_tempFilterBuffer.wrapMode = global::AmplifyBloom.AmplifyUtils.CurrentWrapMode;
			m_tempFilterBuffer.Create();
		}

		private void CleanTempFilterRT()
		{
			if (m_tempFilterBuffer != null)
			{
				global::UnityEngine.RenderTexture.active = null;
				m_tempFilterBuffer.Release();
				global::UnityEngine.Object.DestroyImmediate(m_tempFilterBuffer);
				m_tempFilterBuffer = null;
			}
		}

		private void OnRenderImage(global::UnityEngine.RenderTexture src, global::UnityEngine.RenderTexture dest)
		{
			if (silentError)
			{
				return;
			}
			if (!global::AmplifyBloom.AmplifyUtils.IsInitialized)
			{
				global::AmplifyBloom.AmplifyUtils.InitializeIds();
			}
			if (m_highPrecision)
			{
				global::AmplifyBloom.AmplifyUtils.EnsureKeywordEnabled(m_bloomMaterial, global::AmplifyBloom.AmplifyUtils.HighPrecisionKeyword, state: true);
				global::AmplifyBloom.AmplifyUtils.EnsureKeywordEnabled(m_finalCompositionMaterial, global::AmplifyBloom.AmplifyUtils.HighPrecisionKeyword, state: true);
				global::AmplifyBloom.AmplifyUtils.CurrentRTFormat = global::UnityEngine.RenderTextureFormat.DefaultHDR;
			}
			else
			{
				global::AmplifyBloom.AmplifyUtils.EnsureKeywordEnabled(m_bloomMaterial, global::AmplifyBloom.AmplifyUtils.HighPrecisionKeyword, state: false);
				global::AmplifyBloom.AmplifyUtils.EnsureKeywordEnabled(m_finalCompositionMaterial, global::AmplifyBloom.AmplifyUtils.HighPrecisionKeyword, state: false);
				global::AmplifyBloom.AmplifyUtils.CurrentRTFormat = global::UnityEngine.RenderTextureFormat.Default;
			}
			float num = global::UnityEngine.Mathf.Acos(global::UnityEngine.Vector3.Dot(m_cameraTransform.right, global::UnityEngine.Vector3.right));
			if (global::UnityEngine.Vector3.Cross(m_cameraTransform.right, global::UnityEngine.Vector3.right).y > 0f)
			{
				num = 0f - num;
			}
			global::UnityEngine.RenderTexture renderTexture = null;
			global::UnityEngine.RenderTexture renderTexture2 = null;
			if (!m_highPrecision)
			{
				m_bloomRange.y = 1f / m_bloomRange.x;
				m_bloomMaterial.SetVector(global::AmplifyBloom.AmplifyUtils.BloomRangeId, m_bloomRange);
				m_finalCompositionMaterial.SetVector(global::AmplifyBloom.AmplifyUtils.BloomRangeId, m_bloomRange);
			}
			m_bloomParams.y = m_overallThreshold;
			m_bloomMaterial.SetVector(global::AmplifyBloom.AmplifyUtils.BloomParamsId, m_bloomParams);
			m_finalCompositionMaterial.SetVector(global::AmplifyBloom.AmplifyUtils.BloomParamsId, m_bloomParams);
			int num2 = 1;
			switch (m_mainThresholdSize)
			{
			case global::AmplifyBloom.MainThresholdSizeEnum.Half:
				num2 = 2;
				break;
			case global::AmplifyBloom.MainThresholdSizeEnum.Quarter:
				num2 = 4;
				break;
			}
			global::UnityEngine.RenderTexture tempRenderTarget = global::AmplifyBloom.AmplifyUtils.GetTempRenderTarget(src.width / num2, src.height / num2);
			if (m_maskTexture != null)
			{
				m_bloomMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.MaskTextureId, m_maskTexture);
				global::UnityEngine.Graphics.Blit(src, tempRenderTarget, m_bloomMaterial, 1);
			}
			else
			{
				global::UnityEngine.Graphics.Blit(src, tempRenderTarget, m_bloomMaterial, 0);
			}
			if (m_debugToScreen == global::AmplifyBloom.DebugToScreenEnum.MainThreshold)
			{
				global::UnityEngine.Graphics.Blit(tempRenderTarget, dest, m_bloomMaterial, 33);
				global::AmplifyBloom.AmplifyUtils.ReleaseAllRT();
				return;
			}
			bool flag = true;
			global::UnityEngine.RenderTexture renderTexture3 = tempRenderTarget;
			if (m_bloomDownsampleCount > 0)
			{
				flag = false;
				int num3 = tempRenderTarget.width;
				int num4 = tempRenderTarget.height;
				for (int i = 0; i < m_bloomDownsampleCount; i++)
				{
					m_tempDownsamplesSizes[i].x = num3;
					m_tempDownsamplesSizes[i].y = num4;
					num3 = num3 + 1 >> 1;
					num4 = num4 + 1 >> 1;
					m_tempAuxDownsampleRTs[i] = global::AmplifyBloom.AmplifyUtils.GetTempRenderTarget(num3, num4);
					if (i == 0)
					{
						if (!m_temporalFilteringActive || m_gaussianSteps[i] != 0)
						{
							if (m_upscaleQuality == global::AmplifyBloom.UpscaleQualityEnum.Realistic)
							{
								global::UnityEngine.Graphics.Blit(renderTexture3, m_tempAuxDownsampleRTs[i], m_bloomMaterial, 10);
							}
							else
							{
								global::UnityEngine.Graphics.Blit(renderTexture3, m_tempAuxDownsampleRTs[i], m_bloomMaterial, 11);
							}
						}
						else
						{
							if (m_tempFilterBuffer != null && m_temporalFilteringActive)
							{
								float value = m_temporalFilteringCurve.Evaluate(m_temporalFilteringValue);
								m_bloomMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.TempFilterValueId, value);
								m_bloomMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.AnamorphicRTS[0], m_tempFilterBuffer);
								if (m_upscaleQuality == global::AmplifyBloom.UpscaleQualityEnum.Realistic)
								{
									global::UnityEngine.Graphics.Blit(renderTexture3, m_tempAuxDownsampleRTs[i], m_bloomMaterial, 12);
								}
								else
								{
									global::UnityEngine.Graphics.Blit(renderTexture3, m_tempAuxDownsampleRTs[i], m_bloomMaterial, 13);
								}
							}
							else if (m_upscaleQuality == global::AmplifyBloom.UpscaleQualityEnum.Realistic)
							{
								global::UnityEngine.Graphics.Blit(renderTexture3, m_tempAuxDownsampleRTs[i], m_bloomMaterial, 10);
							}
							else
							{
								global::UnityEngine.Graphics.Blit(renderTexture3, m_tempAuxDownsampleRTs[i], m_bloomMaterial, 11);
							}
							bool flag2 = false;
							if (m_tempFilterBuffer != null)
							{
								if (m_tempFilterBuffer.format != m_tempAuxDownsampleRTs[i].format || m_tempFilterBuffer.width != m_tempAuxDownsampleRTs[i].width || m_tempFilterBuffer.height != m_tempAuxDownsampleRTs[i].height)
								{
									CleanTempFilterRT();
									flag2 = true;
								}
							}
							else
							{
								flag2 = true;
							}
							if (flag2)
							{
								CreateTempFilterRT(m_tempAuxDownsampleRTs[i]);
							}
							m_tempFilterBuffer.DiscardContents();
							global::UnityEngine.Graphics.Blit(m_tempAuxDownsampleRTs[i], m_tempFilterBuffer);
							if (m_debugToScreen == global::AmplifyBloom.DebugToScreenEnum.TemporalFilter)
							{
								global::UnityEngine.Graphics.Blit(m_tempAuxDownsampleRTs[i], dest);
								global::AmplifyBloom.AmplifyUtils.ReleaseAllRT();
								return;
							}
						}
					}
					else
					{
						global::UnityEngine.Graphics.Blit(m_tempAuxDownsampleRTs[i - 1], m_tempAuxDownsampleRTs[i], m_bloomMaterial, 9);
					}
					if (m_gaussianSteps[i] > 0)
					{
						ApplyGaussianBlur(m_tempAuxDownsampleRTs[i], m_gaussianSteps[i], m_gaussianRadius[i], i == 0);
						if (m_temporalFilteringActive && m_debugToScreen == global::AmplifyBloom.DebugToScreenEnum.TemporalFilter)
						{
							global::UnityEngine.Graphics.Blit(m_tempAuxDownsampleRTs[i], dest);
							global::AmplifyBloom.AmplifyUtils.ReleaseAllRT();
							return;
						}
					}
				}
				renderTexture3 = m_tempAuxDownsampleRTs[m_featuresSourceId];
				global::AmplifyBloom.AmplifyUtils.ReleaseTempRenderTarget(tempRenderTarget);
			}
			if (m_bokehFilter.ApplyBokeh && m_bokehFilter.ApplyOnBloomSource)
			{
				m_bokehFilter.ApplyBokehFilter(renderTexture3, m_bloomMaterial);
				if (m_debugToScreen == global::AmplifyBloom.DebugToScreenEnum.BokehFilter)
				{
					global::UnityEngine.Graphics.Blit(renderTexture3, dest);
					global::AmplifyBloom.AmplifyUtils.ReleaseAllRT();
					return;
				}
			}
			global::UnityEngine.RenderTexture renderTexture4 = null;
			bool flag3 = false;
			if (m_separateFeaturesThreshold)
			{
				m_bloomParams.y = m_featuresThreshold;
				m_bloomMaterial.SetVector(global::AmplifyBloom.AmplifyUtils.BloomParamsId, m_bloomParams);
				m_finalCompositionMaterial.SetVector(global::AmplifyBloom.AmplifyUtils.BloomParamsId, m_bloomParams);
				renderTexture4 = global::AmplifyBloom.AmplifyUtils.GetTempRenderTarget(renderTexture3.width, renderTexture3.height);
				flag3 = true;
				global::UnityEngine.Graphics.Blit(renderTexture3, renderTexture4, m_bloomMaterial, 0);
				if (m_debugToScreen == global::AmplifyBloom.DebugToScreenEnum.FeaturesThreshold)
				{
					global::UnityEngine.Graphics.Blit(renderTexture4, dest);
					global::AmplifyBloom.AmplifyUtils.ReleaseAllRT();
					return;
				}
			}
			else
			{
				renderTexture4 = renderTexture3;
			}
			if (m_bokehFilter.ApplyBokeh && !m_bokehFilter.ApplyOnBloomSource)
			{
				if (!flag3)
				{
					flag3 = true;
					renderTexture4 = global::AmplifyBloom.AmplifyUtils.GetTempRenderTarget(renderTexture3.width, renderTexture3.height);
					global::UnityEngine.Graphics.Blit(renderTexture3, renderTexture4);
				}
				m_bokehFilter.ApplyBokehFilter(renderTexture4, m_bloomMaterial);
				if (m_debugToScreen == global::AmplifyBloom.DebugToScreenEnum.BokehFilter)
				{
					global::UnityEngine.Graphics.Blit(renderTexture4, dest);
					global::AmplifyBloom.AmplifyUtils.ReleaseAllRT();
					return;
				}
			}
			if (m_lensFlare.ApplyLensFlare && m_debugToScreen != global::AmplifyBloom.DebugToScreenEnum.Bloom)
			{
				renderTexture = m_lensFlare.ApplyFlare(m_bloomMaterial, renderTexture4);
				ApplyGaussianBlur(renderTexture, m_lensFlare.LensFlareGaussianBlurAmount);
				if (m_debugToScreen == global::AmplifyBloom.DebugToScreenEnum.LensFlare)
				{
					global::UnityEngine.Graphics.Blit(renderTexture, dest);
					global::AmplifyBloom.AmplifyUtils.ReleaseAllRT();
					return;
				}
			}
			if (m_anamorphicGlare.ApplyLensGlare && m_debugToScreen != global::AmplifyBloom.DebugToScreenEnum.Bloom)
			{
				renderTexture2 = global::AmplifyBloom.AmplifyUtils.GetTempRenderTarget(renderTexture3.width, renderTexture3.height);
				m_anamorphicGlare.OnRenderImage(m_bloomMaterial, renderTexture4, renderTexture2, num);
				if (m_debugToScreen == global::AmplifyBloom.DebugToScreenEnum.LensGlare)
				{
					global::UnityEngine.Graphics.Blit(renderTexture2, dest);
					global::AmplifyBloom.AmplifyUtils.ReleaseAllRT();
					return;
				}
			}
			if (flag3)
			{
				global::AmplifyBloom.AmplifyUtils.ReleaseTempRenderTarget(renderTexture4);
			}
			if (flag)
			{
				ApplyGaussianBlur(renderTexture3, m_gaussianSteps[0], m_gaussianRadius[0]);
			}
			if (m_bloomDownsampleCount > 0)
			{
				if (m_bloomDownsampleCount == 1)
				{
					if (m_upscaleQuality == global::AmplifyBloom.UpscaleQualityEnum.Realistic)
					{
						ApplyUpscale();
						m_finalCompositionMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.MipResultsRTS[0], m_tempUpscaleRTs[0]);
					}
					else
					{
						m_finalCompositionMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.MipResultsRTS[0], m_tempAuxDownsampleRTs[0]);
					}
					m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.UpscaleWeightsStr[0], m_upscaleWeights[0]);
				}
				else if (m_upscaleQuality == global::AmplifyBloom.UpscaleQualityEnum.Realistic)
				{
					ApplyUpscale();
					for (int j = 0; j < m_bloomDownsampleCount; j++)
					{
						int num5 = m_bloomDownsampleCount - j - 1;
						m_finalCompositionMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.MipResultsRTS[num5], m_tempUpscaleRTs[j]);
						m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.UpscaleWeightsStr[num5], m_upscaleWeights[j]);
					}
				}
				else
				{
					for (int k = 0; k < m_bloomDownsampleCount; k++)
					{
						int num6 = m_bloomDownsampleCount - 1 - k;
						m_finalCompositionMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.MipResultsRTS[num6], m_tempAuxDownsampleRTs[num6]);
						m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.UpscaleWeightsStr[num6], m_upscaleWeights[k]);
					}
				}
			}
			else
			{
				m_finalCompositionMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.MipResultsRTS[0], renderTexture3);
				m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.UpscaleWeightsStr[0], 1f);
			}
			if (m_debugToScreen == global::AmplifyBloom.DebugToScreenEnum.Bloom)
			{
				m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.SourceContributionId, 0f);
				FinalComposition(0f, 1f, src, dest, 0);
				return;
			}
			if (m_bloomDownsampleCount > 1)
			{
				for (int l = 0; l < m_bloomDownsampleCount; l++)
				{
					m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.LensDirtWeightsStr[m_bloomDownsampleCount - l - 1], m_lensDirtWeights[l]);
					m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.LensStarburstWeightsStr[m_bloomDownsampleCount - l - 1], m_lensStarburstWeights[l]);
				}
			}
			else
			{
				m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.LensDirtWeightsStr[0], m_lensDirtWeights[0]);
				m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.LensStarburstWeightsStr[0], m_lensStarburstWeights[0]);
			}
			if (m_lensFlare.ApplyLensFlare)
			{
				m_finalCompositionMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.LensFlareRTId, renderTexture);
			}
			if (m_anamorphicGlare.ApplyLensGlare)
			{
				m_finalCompositionMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.LensGlareRTId, renderTexture2);
			}
			if (m_applyLensDirt)
			{
				m_finalCompositionMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.LensDirtRTId, m_lensDirtTexture);
				m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.LensDirtStrengthId, m_lensDirtStrength * 1f);
				if (m_debugToScreen == global::AmplifyBloom.DebugToScreenEnum.LensDirt)
				{
					FinalComposition(0f, 0f, src, dest, 2);
					return;
				}
			}
			if (m_applyLensStardurst)
			{
				m_starburstMat[0, 0] = global::UnityEngine.Mathf.Cos(num);
				m_starburstMat[0, 1] = 0f - global::UnityEngine.Mathf.Sin(num);
				m_starburstMat[1, 0] = global::UnityEngine.Mathf.Sin(num);
				m_starburstMat[1, 1] = global::UnityEngine.Mathf.Cos(num);
				m_finalCompositionMaterial.SetMatrix(global::AmplifyBloom.AmplifyUtils.LensFlareStarMatrixId, m_starburstMat);
				m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.LensFlareStarburstStrengthId, m_lensStarburstStrength * 1f);
				m_finalCompositionMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.LensStarburstRTId, m_lensStardurstTex);
				if (m_debugToScreen == global::AmplifyBloom.DebugToScreenEnum.LensStarburst)
				{
					FinalComposition(0f, 0f, src, dest, 1);
					return;
				}
			}
			if (m_targetTexture != null)
			{
				m_targetTexture.DiscardContents();
				FinalComposition(0f, 1f, src, m_targetTexture, -1);
				global::UnityEngine.Graphics.Blit(src, dest);
			}
			else
			{
				FinalComposition(1f, 1f, src, dest, -1);
			}
		}

		private void FinalComposition(float srcContribution, float upscaleContribution, global::UnityEngine.RenderTexture src, global::UnityEngine.RenderTexture dest, int forcePassId)
		{
			m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.SourceContributionId, srcContribution);
			m_finalCompositionMaterial.SetFloat(global::AmplifyBloom.AmplifyUtils.UpscaleContributionId, upscaleContribution);
			int num = 0;
			if (forcePassId > -1)
			{
				num = forcePassId;
			}
			else
			{
				if (LensFlareInstance.ApplyLensFlare)
				{
					num |= 8;
				}
				if (LensGlareInstance.ApplyLensGlare)
				{
					num |= 4;
				}
				if (m_applyLensDirt)
				{
					num |= 2;
				}
				if (m_applyLensStardurst)
				{
					num |= 1;
				}
			}
			num += (m_bloomDownsampleCount - 1) * 16;
			global::UnityEngine.Graphics.Blit(src, dest, m_finalCompositionMaterial, num);
			global::AmplifyBloom.AmplifyUtils.ReleaseAllRT();
		}

		private void ApplyUpscale()
		{
			int num = m_bloomDownsampleCount - 1;
			int num2 = 0;
			for (int num3 = num; num3 > -1; num3--)
			{
				m_tempUpscaleRTs[num2] = global::AmplifyBloom.AmplifyUtils.GetTempRenderTarget((int)m_tempDownsamplesSizes[num3].x, (int)m_tempDownsamplesSizes[num3].y);
				if (num3 == num)
				{
					global::UnityEngine.Graphics.Blit(m_tempAuxDownsampleRTs[num], m_tempUpscaleRTs[num2], m_bloomMaterial, 17);
				}
				else
				{
					m_bloomMaterial.SetTexture(global::AmplifyBloom.AmplifyUtils.AnamorphicRTS[0], m_tempUpscaleRTs[num2 - 1]);
					global::UnityEngine.Graphics.Blit(m_tempAuxDownsampleRTs[num3], m_tempUpscaleRTs[num2], m_bloomMaterial, 18);
				}
				num2++;
			}
		}
	}
}
