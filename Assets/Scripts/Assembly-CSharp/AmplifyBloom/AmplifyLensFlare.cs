namespace AmplifyBloom
{
	[global::System.Serializable]
	public class AmplifyLensFlare : global::AmplifyBloom.IAmplifyItem
	{
		private const int LUTTextureWidth = 256;

		[global::UnityEngine.SerializeField]
		private float m_overallIntensity = 1f;

		[global::UnityEngine.SerializeField]
		private float m_normalizedGhostIntensity = 0.8f;

		[global::UnityEngine.SerializeField]
		private float m_normalizedHaloIntensity = 0.1f;

		[global::UnityEngine.SerializeField]
		private bool m_applyLensFlare = true;

		[global::UnityEngine.SerializeField]
		private int m_lensFlareGhostAmount = 3;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Vector4 m_lensFlareGhostsParams = new global::UnityEngine.Vector4(0.8f, 0.228f, 1f, 4f);

		[global::UnityEngine.SerializeField]
		private float m_lensFlareGhostChrDistortion = 2f;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Gradient m_lensGradient;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Texture2D m_lensFlareGradTexture;

		private global::UnityEngine.Color[] m_lensFlareGradColor = new global::UnityEngine.Color[256];

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Vector4 m_lensFlareHaloParams = new global::UnityEngine.Vector4(0.1f, 0.573f, 1f, 128f);

		[global::UnityEngine.SerializeField]
		private float m_lensFlareHaloChrDistortion = 1.51f;

		[global::UnityEngine.SerializeField]
		private int m_lensFlareGaussianBlurAmount = 1;

		public bool ApplyLensFlare
		{
			get
			{
				return m_applyLensFlare;
			}
			set
			{
				m_applyLensFlare = value;
			}
		}

		public float OverallIntensity
		{
			get
			{
				return m_overallIntensity;
			}
			set
			{
				m_overallIntensity = ((value < 0f) ? 0f : value);
				m_lensFlareGhostsParams.x = value * m_normalizedGhostIntensity;
				m_lensFlareHaloParams.x = value * m_normalizedHaloIntensity;
			}
		}

		public int LensFlareGhostAmount
		{
			get
			{
				return m_lensFlareGhostAmount;
			}
			set
			{
				m_lensFlareGhostAmount = value;
			}
		}

		public global::UnityEngine.Vector4 LensFlareGhostsParams
		{
			get
			{
				return m_lensFlareGhostsParams;
			}
			set
			{
				m_lensFlareGhostsParams = value;
			}
		}

		public float LensFlareNormalizedGhostsIntensity
		{
			get
			{
				return m_normalizedGhostIntensity;
			}
			set
			{
				m_normalizedGhostIntensity = ((value < 0f) ? 0f : value);
				m_lensFlareGhostsParams.x = m_overallIntensity * m_normalizedGhostIntensity;
			}
		}

		public float LensFlareGhostsIntensity
		{
			get
			{
				return m_lensFlareGhostsParams.x;
			}
			set
			{
				m_lensFlareGhostsParams.x = ((value < 0f) ? 0f : value);
			}
		}

		public float LensFlareGhostsDispersal
		{
			get
			{
				return m_lensFlareGhostsParams.y;
			}
			set
			{
				m_lensFlareGhostsParams.y = value;
			}
		}

		public float LensFlareGhostsPowerFactor
		{
			get
			{
				return m_lensFlareGhostsParams.z;
			}
			set
			{
				m_lensFlareGhostsParams.z = value;
			}
		}

		public float LensFlareGhostsPowerFalloff
		{
			get
			{
				return m_lensFlareGhostsParams.w;
			}
			set
			{
				m_lensFlareGhostsParams.w = value;
			}
		}

		public global::UnityEngine.Gradient LensFlareGradient
		{
			get
			{
				return m_lensGradient;
			}
			set
			{
				m_lensGradient = value;
			}
		}

		public global::UnityEngine.Vector4 LensFlareHaloParams
		{
			get
			{
				return m_lensFlareHaloParams;
			}
			set
			{
				m_lensFlareHaloParams = value;
			}
		}

		public float LensFlareNormalizedHaloIntensity
		{
			get
			{
				return m_normalizedHaloIntensity;
			}
			set
			{
				m_normalizedHaloIntensity = ((value < 0f) ? 0f : value);
				m_lensFlareHaloParams.x = m_overallIntensity * m_normalizedHaloIntensity;
			}
		}

		public float LensFlareHaloIntensity
		{
			get
			{
				return m_lensFlareHaloParams.x;
			}
			set
			{
				m_lensFlareHaloParams.x = ((value < 0f) ? 0f : value);
			}
		}

		public float LensFlareHaloWidth
		{
			get
			{
				return m_lensFlareHaloParams.y;
			}
			set
			{
				m_lensFlareHaloParams.y = value;
			}
		}

		public float LensFlareHaloPowerFactor
		{
			get
			{
				return m_lensFlareHaloParams.z;
			}
			set
			{
				m_lensFlareHaloParams.z = value;
			}
		}

		public float LensFlareHaloPowerFalloff
		{
			get
			{
				return m_lensFlareHaloParams.w;
			}
			set
			{
				m_lensFlareHaloParams.w = value;
			}
		}

		public float LensFlareGhostChrDistortion
		{
			get
			{
				return m_lensFlareGhostChrDistortion;
			}
			set
			{
				m_lensFlareGhostChrDistortion = value;
			}
		}

		public float LensFlareHaloChrDistortion
		{
			get
			{
				return m_lensFlareHaloChrDistortion;
			}
			set
			{
				m_lensFlareHaloChrDistortion = value;
			}
		}

		public int LensFlareGaussianBlurAmount
		{
			get
			{
				return m_lensFlareGaussianBlurAmount;
			}
			set
			{
				m_lensFlareGaussianBlurAmount = value;
			}
		}

		public AmplifyLensFlare()
		{
			m_lensGradient = new global::UnityEngine.Gradient();
		}

		public void Init()
		{
			if (m_lensGradient.alphaKeys.Length == 0 && m_lensGradient.colorKeys.Length == 0)
			{
				global::UnityEngine.GradientColorKey[] colorKeys = new global::UnityEngine.GradientColorKey[5]
				{
					new global::UnityEngine.GradientColorKey(global::UnityEngine.Color.white, 0f),
					new global::UnityEngine.GradientColorKey(global::UnityEngine.Color.blue, 0.25f),
					new global::UnityEngine.GradientColorKey(global::UnityEngine.Color.green, 0.5f),
					new global::UnityEngine.GradientColorKey(global::UnityEngine.Color.yellow, 0.75f),
					new global::UnityEngine.GradientColorKey(global::UnityEngine.Color.red, 1f)
				};
				global::UnityEngine.GradientAlphaKey[] alphaKeys = new global::UnityEngine.GradientAlphaKey[5]
				{
					new global::UnityEngine.GradientAlphaKey(1f, 0f),
					new global::UnityEngine.GradientAlphaKey(1f, 0.25f),
					new global::UnityEngine.GradientAlphaKey(1f, 0.5f),
					new global::UnityEngine.GradientAlphaKey(1f, 0.75f),
					new global::UnityEngine.GradientAlphaKey(1f, 1f)
				};
				m_lensGradient.SetKeys(colorKeys, alphaKeys);
			}
		}

		public void Destroy()
		{
			if (m_lensFlareGradTexture != null)
			{
				global::UnityEngine.Object.DestroyImmediate(m_lensFlareGradTexture);
				m_lensFlareGradTexture = null;
			}
		}

		public void CreateLUTexture()
		{
			m_lensFlareGradTexture = new global::UnityEngine.Texture2D(256, 1, global::UnityEngine.TextureFormat.ARGB32, mipChain: false);
			m_lensFlareGradTexture.filterMode = global::UnityEngine.FilterMode.Bilinear;
			TextureFromGradient();
		}

		public global::UnityEngine.RenderTexture ApplyFlare(global::UnityEngine.Material material, global::UnityEngine.RenderTexture source)
		{
			global::UnityEngine.RenderTexture tempRenderTarget = global::AmplifyBloom.AmplifyUtils.GetTempRenderTarget(source.width, source.height);
			material.SetVector(global::AmplifyBloom.AmplifyUtils.LensFlareGhostsParamsId, m_lensFlareGhostsParams);
			material.SetTexture(global::AmplifyBloom.AmplifyUtils.LensFlareLUTId, m_lensFlareGradTexture);
			material.SetVector(global::AmplifyBloom.AmplifyUtils.LensFlareHaloParamsId, m_lensFlareHaloParams);
			material.SetFloat(global::AmplifyBloom.AmplifyUtils.LensFlareGhostChrDistortionId, m_lensFlareGhostChrDistortion);
			material.SetFloat(global::AmplifyBloom.AmplifyUtils.LensFlareHaloChrDistortionId, m_lensFlareHaloChrDistortion);
			global::UnityEngine.Graphics.Blit(source, tempRenderTarget, material, 3 + m_lensFlareGhostAmount);
			return tempRenderTarget;
		}

		public void TextureFromGradient()
		{
			for (int i = 0; i < 256; i++)
			{
				m_lensFlareGradColor[i] = m_lensGradient.Evaluate((float)i / 255f);
			}
			m_lensFlareGradTexture.SetPixels(m_lensFlareGradColor);
			m_lensFlareGradTexture.Apply();
		}
	}
}
