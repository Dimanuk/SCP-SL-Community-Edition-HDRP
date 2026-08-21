namespace AmplifyBloom
{
	[global::System.Serializable]
	public sealed class AmplifyGlare : global::AmplifyBloom.IAmplifyItem
	{
		public const int MaxLineSamples = 8;

		public const int MaxTotalSamples = 16;

		public const int MaxStarLines = 4;

		public const int MaxPasses = 4;

		public const int MaxCustomGlare = 32;

		[global::UnityEngine.SerializeField]
		private global::AmplifyBloom.GlareDefData[] m_customGlareDef;

		[global::UnityEngine.SerializeField]
		private int m_customGlareDefIdx;

		[global::UnityEngine.SerializeField]
		private int m_customGlareDefAmount;

		[global::UnityEngine.SerializeField]
		private bool m_applyGlare = true;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Color _overallTint = global::UnityEngine.Color.white;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Gradient m_cromaticAberrationGrad;

		[global::UnityEngine.SerializeField]
		private int m_glareMaxPassCount = 4;

		private global::AmplifyBloom.StarDefData[] m_starDefArr;

		private global::AmplifyBloom.GlareDefData[] m_glareDefArr;

		private global::UnityEngine.Matrix4x4[] m_weigthsMat;

		private global::UnityEngine.Matrix4x4[] m_offsetsMat;

		private global::UnityEngine.Color m_whiteReference;

		private float m_aTanFoV;

		private global::AmplifyBloom.AmplifyGlareCache m_amplifyGlareCache;

		[global::UnityEngine.SerializeField]
		private int m_currentWidth;

		[global::UnityEngine.SerializeField]
		private int m_currentHeight;

		[global::UnityEngine.SerializeField]
		private global::AmplifyBloom.GlareLibType m_currentGlareType;

		[global::UnityEngine.SerializeField]
		private int m_currentGlareIdx;

		[global::UnityEngine.SerializeField]
		private float m_perPassDisplacement = 4f;

		[global::UnityEngine.SerializeField]
		private float m_intensity = 0.17f;

		[global::UnityEngine.SerializeField]
		private float m_overallStreakScale = 1f;

		private bool m_isDirty = true;

		private global::UnityEngine.RenderTexture[] _rtBuffer;

		public global::AmplifyBloom.GlareLibType CurrentGlare
		{
			get
			{
				return m_currentGlareType;
			}
			set
			{
				if (m_currentGlareType != value)
				{
					m_currentGlareType = value;
					m_currentGlareIdx = (int)value;
					m_isDirty = true;
				}
			}
		}

		public int GlareMaxPassCount
		{
			get
			{
				return m_glareMaxPassCount;
			}
			set
			{
				m_glareMaxPassCount = value;
				m_isDirty = true;
			}
		}

		public float PerPassDisplacement
		{
			get
			{
				return m_perPassDisplacement;
			}
			set
			{
				m_perPassDisplacement = value;
				m_isDirty = true;
			}
		}

		public float Intensity
		{
			get
			{
				return m_intensity;
			}
			set
			{
				m_intensity = ((value < 0f) ? 0f : value);
				m_isDirty = true;
			}
		}

		public global::UnityEngine.Color OverallTint
		{
			get
			{
				return _overallTint;
			}
			set
			{
				_overallTint = value;
				m_isDirty = true;
			}
		}

		public bool ApplyLensGlare
		{
			get
			{
				return m_applyGlare;
			}
			set
			{
				m_applyGlare = value;
			}
		}

		public global::UnityEngine.Gradient CromaticColorGradient
		{
			get
			{
				return m_cromaticAberrationGrad;
			}
			set
			{
				m_cromaticAberrationGrad = value;
				m_isDirty = true;
			}
		}

		public float OverallStreakScale
		{
			get
			{
				return m_overallStreakScale;
			}
			set
			{
				m_overallStreakScale = value;
				m_isDirty = true;
			}
		}

		public global::AmplifyBloom.GlareDefData[] CustomGlareDef
		{
			get
			{
				return m_customGlareDef;
			}
			set
			{
				m_customGlareDef = value;
			}
		}

		public int CustomGlareDefIdx
		{
			get
			{
				return m_customGlareDefIdx;
			}
			set
			{
				m_customGlareDefIdx = value;
			}
		}

		public int CustomGlareDefAmount
		{
			get
			{
				return m_customGlareDefAmount;
			}
			set
			{
				if (value == m_customGlareDefAmount)
				{
					return;
				}
				if (value == 0)
				{
					m_customGlareDef = null;
					m_customGlareDefIdx = 0;
					m_customGlareDefAmount = 0;
					return;
				}
				global::AmplifyBloom.GlareDefData[] array = new global::AmplifyBloom.GlareDefData[value];
				for (int i = 0; i < value; i++)
				{
					if (i < m_customGlareDefAmount)
					{
						array[i] = m_customGlareDef[i];
					}
					else
					{
						array[i] = new global::AmplifyBloom.GlareDefData();
					}
				}
				m_customGlareDefIdx = global::UnityEngine.Mathf.Clamp(m_customGlareDefIdx, 0, value - 1);
				m_customGlareDef = array;
				array = null;
				m_customGlareDefAmount = value;
			}
		}

		public AmplifyGlare()
		{
			m_currentGlareIdx = (int)m_currentGlareType;
			m_cromaticAberrationGrad = new global::UnityEngine.Gradient();
			_rtBuffer = new global::UnityEngine.RenderTexture[16];
			m_weigthsMat = new global::UnityEngine.Matrix4x4[4];
			m_offsetsMat = new global::UnityEngine.Matrix4x4[4];
			m_amplifyGlareCache = new global::AmplifyBloom.AmplifyGlareCache();
			m_whiteReference = new global::UnityEngine.Color(0.63f, 0.63f, 0.63f, 0f);
			m_aTanFoV = global::UnityEngine.Mathf.Atan((float)global::System.Math.PI / 8f);
			m_starDefArr = new global::AmplifyBloom.StarDefData[5]
			{
				new global::AmplifyBloom.StarDefData(global::AmplifyBloom.StarLibType.Cross, "Cross", 2, 4, 1f, 0.85f, 0f, 0.5f, -1f, 90f),
				new global::AmplifyBloom.StarDefData(global::AmplifyBloom.StarLibType.Cross_Filter, "CrossFilter", 2, 4, 1f, 0.95f, 0f, 0.5f, -1f, 90f),
				new global::AmplifyBloom.StarDefData(global::AmplifyBloom.StarLibType.Snow_Cross, "snowCross", 3, 4, 1f, 0.96f, 0.349f, 0.5f, -1f),
				new global::AmplifyBloom.StarDefData(global::AmplifyBloom.StarLibType.Vertical, "Vertical", 1, 4, 1f, 0.96f, 0f, 0f, -1f),
				new global::AmplifyBloom.StarDefData(global::AmplifyBloom.StarLibType.Sunny_Cross, "SunnyCross", 4, 4, 1f, 0.88f, 0f, 0f, 0.95f, 45f)
			};
			m_glareDefArr = new global::AmplifyBloom.GlareDefData[9]
			{
				new global::AmplifyBloom.GlareDefData(global::AmplifyBloom.StarLibType.Cross, 0f, 0.5f),
				new global::AmplifyBloom.GlareDefData(global::AmplifyBloom.StarLibType.Cross_Filter, 0.44f, 0.5f),
				new global::AmplifyBloom.GlareDefData(global::AmplifyBloom.StarLibType.Cross_Filter, 1.22f, 1.5f),
				new global::AmplifyBloom.GlareDefData(global::AmplifyBloom.StarLibType.Snow_Cross, 0.17f, 0.5f),
				new global::AmplifyBloom.GlareDefData(global::AmplifyBloom.StarLibType.Snow_Cross, 0.7f, 1.5f),
				new global::AmplifyBloom.GlareDefData(global::AmplifyBloom.StarLibType.Sunny_Cross, 0f, 0.5f),
				new global::AmplifyBloom.GlareDefData(global::AmplifyBloom.StarLibType.Sunny_Cross, 0.79f, 1.5f),
				new global::AmplifyBloom.GlareDefData(global::AmplifyBloom.StarLibType.Vertical, 1.57f, 0.5f),
				new global::AmplifyBloom.GlareDefData(global::AmplifyBloom.StarLibType.Vertical, 0f, 0.5f)
			};
		}

		public void Init()
		{
			if (m_cromaticAberrationGrad.alphaKeys.Length == 0 && m_cromaticAberrationGrad.colorKeys.Length == 0)
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
				m_cromaticAberrationGrad.SetKeys(colorKeys, alphaKeys);
			}
		}

		public void Destroy()
		{
			for (int i = 0; i < m_starDefArr.Length; i++)
			{
				m_starDefArr[i].Destroy();
			}
			m_glareDefArr = null;
			m_weigthsMat = null;
			m_offsetsMat = null;
			for (int j = 0; j < _rtBuffer.Length; j++)
			{
				if (_rtBuffer[j] != null)
				{
					global::AmplifyBloom.AmplifyUtils.ReleaseTempRenderTarget(_rtBuffer[j]);
					_rtBuffer[j] = null;
				}
			}
			_rtBuffer = null;
			m_amplifyGlareCache.Destroy();
			m_amplifyGlareCache = null;
		}

		public void SetDirty()
		{
			m_isDirty = true;
		}

		public void OnRenderFromCache(global::UnityEngine.RenderTexture source, global::UnityEngine.RenderTexture dest, global::UnityEngine.Material material, float glareIntensity, float cameraRotation)
		{
			for (int i = 0; i < m_amplifyGlareCache.TotalRT; i++)
			{
				_rtBuffer[i] = global::AmplifyBloom.AmplifyUtils.GetTempRenderTarget(source.width, source.height);
			}
			int num = 0;
			for (int j = 0; j < m_amplifyGlareCache.StarDef.StarlinesCount; j++)
			{
				for (int k = 0; k < m_amplifyGlareCache.CurrentPassCount; k++)
				{
					UpdateMatrixesForPass(material, m_amplifyGlareCache.Starlines[j].Passes[k].Offsets, m_amplifyGlareCache.Starlines[j].Passes[k].Weights, glareIntensity, cameraRotation * m_amplifyGlareCache.StarDef.CameraRotInfluence);
					if (k == 0)
					{
						global::UnityEngine.Graphics.Blit(source, _rtBuffer[num], material, 2);
					}
					else
					{
						global::UnityEngine.Graphics.Blit(_rtBuffer[num - 1], _rtBuffer[num], material, 2);
					}
					num++;
				}
			}
			for (int l = 0; l < m_amplifyGlareCache.StarDef.StarlinesCount; l++)
			{
				material.SetVector(global::AmplifyBloom.AmplifyUtils.AnamorphicGlareWeightsStr[l], m_amplifyGlareCache.AverageWeight);
				int num2 = (l + 1) * m_amplifyGlareCache.CurrentPassCount - 1;
				material.SetTexture(global::AmplifyBloom.AmplifyUtils.AnamorphicRTS[l], _rtBuffer[num2]);
			}
			int pass = 19 + m_amplifyGlareCache.StarDef.StarlinesCount - 1;
			dest.DiscardContents();
			global::UnityEngine.Graphics.Blit(_rtBuffer[0], dest, material, pass);
			for (num = 0; num < _rtBuffer.Length; num++)
			{
				global::AmplifyBloom.AmplifyUtils.ReleaseTempRenderTarget(_rtBuffer[num]);
				_rtBuffer[num] = null;
			}
		}

		public void UpdateMatrixesForPass(global::UnityEngine.Material material, global::UnityEngine.Vector4[] offsets, global::UnityEngine.Vector4[] weights, float glareIntensity, float rotation)
		{
			float num = global::UnityEngine.Mathf.Cos(rotation);
			float num2 = global::UnityEngine.Mathf.Sin(rotation);
			for (int i = 0; i < 16; i++)
			{
				int num3 = i >> 2;
				int row = i & 3;
				m_offsetsMat[num3][row, 0] = offsets[i].x * num - offsets[i].y * num2;
				m_offsetsMat[num3][row, 1] = offsets[i].x * num2 + offsets[i].y * num;
				m_weigthsMat[num3][row, 0] = glareIntensity * weights[i].x;
				m_weigthsMat[num3][row, 1] = glareIntensity * weights[i].y;
				m_weigthsMat[num3][row, 2] = glareIntensity * weights[i].z;
			}
			for (int j = 0; j < 4; j++)
			{
				material.SetMatrix(global::AmplifyBloom.AmplifyUtils.AnamorphicGlareOffsetsMatStr[j], m_offsetsMat[j]);
				material.SetMatrix(global::AmplifyBloom.AmplifyUtils.AnamorphicGlareWeightsMatStr[j], m_weigthsMat[j]);
			}
		}

		public void OnRenderImage(global::UnityEngine.Material material, global::UnityEngine.RenderTexture source, global::UnityEngine.RenderTexture dest, float cameraRot)
		{
			global::UnityEngine.Graphics.Blit(global::UnityEngine.Texture2D.blackTexture, dest);
			if (m_isDirty || m_currentWidth != source.width || m_currentHeight != source.height)
			{
				m_isDirty = false;
				m_currentWidth = source.width;
				m_currentHeight = source.height;
				global::AmplifyBloom.GlareDefData glareDefData = null;
				bool flag = false;
				if (m_currentGlareType == global::AmplifyBloom.GlareLibType.Custom)
				{
					if (m_customGlareDef != null && m_customGlareDef.Length != 0)
					{
						glareDefData = m_customGlareDef[m_customGlareDefIdx];
						flag = true;
					}
					else
					{
						glareDefData = m_glareDefArr[0];
					}
				}
				else
				{
					glareDefData = m_glareDefArr[m_currentGlareIdx];
				}
				m_amplifyGlareCache.GlareDef = glareDefData;
				float num = source.width;
				float num2 = source.height;
				global::AmplifyBloom.StarDefData starDefData = (flag ? glareDefData.CustomStarData : m_starDefArr[(int)glareDefData.StarType]);
				m_amplifyGlareCache.StarDef = starDefData;
				int num3 = ((m_glareMaxPassCount < starDefData.PassCount) ? m_glareMaxPassCount : starDefData.PassCount);
				m_amplifyGlareCache.CurrentPassCount = num3;
				float num4 = glareDefData.StarInclination + starDefData.Inclination;
				for (int i = 0; i < m_glareMaxPassCount; i++)
				{
					float t = (float)(i + 1) / (float)m_glareMaxPassCount;
					for (int j = 0; j < 8; j++)
					{
						global::UnityEngine.Color b = _overallTint * global::UnityEngine.Color.Lerp(m_cromaticAberrationGrad.Evaluate((float)j / 7f), m_whiteReference, t);
						m_amplifyGlareCache.CromaticAberrationMat[i, j] = global::UnityEngine.Color.Lerp(m_whiteReference, b, glareDefData.ChromaticAberration);
					}
				}
				m_amplifyGlareCache.TotalRT = starDefData.StarlinesCount * num3;
				for (int k = 0; k < m_amplifyGlareCache.TotalRT; k++)
				{
					_rtBuffer[k] = global::AmplifyBloom.AmplifyUtils.GetTempRenderTarget(source.width, source.height);
				}
				int num5 = 0;
				for (int l = 0; l < starDefData.StarlinesCount; l++)
				{
					global::AmplifyBloom.StarLineData starLineData = starDefData.StarLinesArr[l];
					float f = num4 + starLineData.Inclination;
					float num6 = global::UnityEngine.Mathf.Sin(f);
					float num7 = global::UnityEngine.Mathf.Cos(f);
					global::UnityEngine.Vector2 vector = new global::UnityEngine.Vector2
					{
						x = num7 / num * (starLineData.SampleLength * m_overallStreakScale),
						y = num6 / num2 * (starLineData.SampleLength * m_overallStreakScale)
					};
					float num8 = (m_aTanFoV + 0.1f) * 280f / (num + num2) * 1.2f;
					for (int m = 0; m < num3; m++)
					{
						for (int n = 0; n < 8; n++)
						{
							float num9 = global::UnityEngine.Mathf.Pow(starLineData.Attenuation, num8 * (float)n);
							m_amplifyGlareCache.Starlines[l].Passes[m].Weights[n] = m_amplifyGlareCache.CromaticAberrationMat[num3 - 1 - m, n] * num9 * ((float)m + 1f) * 0.5f;
							m_amplifyGlareCache.Starlines[l].Passes[m].Offsets[n].x = vector.x * (float)n;
							m_amplifyGlareCache.Starlines[l].Passes[m].Offsets[n].y = vector.y * (float)n;
							if (global::UnityEngine.Mathf.Abs(m_amplifyGlareCache.Starlines[l].Passes[m].Offsets[n].x) >= 0.9f || global::UnityEngine.Mathf.Abs(m_amplifyGlareCache.Starlines[l].Passes[m].Offsets[n].y) >= 0.9f)
							{
								m_amplifyGlareCache.Starlines[l].Passes[m].Offsets[n].x = 0f;
								m_amplifyGlareCache.Starlines[l].Passes[m].Offsets[n].y = 0f;
								m_amplifyGlareCache.Starlines[l].Passes[m].Weights[n] *= 0f;
							}
						}
						for (int num10 = 8; num10 < 16; num10++)
						{
							m_amplifyGlareCache.Starlines[l].Passes[m].Offsets[num10] = -m_amplifyGlareCache.Starlines[l].Passes[m].Offsets[num10 - 8];
							m_amplifyGlareCache.Starlines[l].Passes[m].Weights[num10] = m_amplifyGlareCache.Starlines[l].Passes[m].Weights[num10 - 8];
						}
						UpdateMatrixesForPass(material, m_amplifyGlareCache.Starlines[l].Passes[m].Offsets, m_amplifyGlareCache.Starlines[l].Passes[m].Weights, m_intensity, starDefData.CameraRotInfluence * cameraRot);
						if (m == 0)
						{
							global::UnityEngine.Graphics.Blit(source, _rtBuffer[num5], material, 2);
						}
						else
						{
							global::UnityEngine.Graphics.Blit(_rtBuffer[num5 - 1], _rtBuffer[num5], material, 2);
						}
						num5++;
						vector *= m_perPassDisplacement;
						num8 *= m_perPassDisplacement;
					}
				}
				m_amplifyGlareCache.AverageWeight = global::UnityEngine.Vector4.one / starDefData.StarlinesCount;
				for (int num11 = 0; num11 < starDefData.StarlinesCount; num11++)
				{
					material.SetVector(global::AmplifyBloom.AmplifyUtils.AnamorphicGlareWeightsStr[num11], m_amplifyGlareCache.AverageWeight);
					int num12 = (num11 + 1) * num3 - 1;
					material.SetTexture(global::AmplifyBloom.AmplifyUtils.AnamorphicRTS[num11], _rtBuffer[num12]);
				}
				int pass = 19 + starDefData.StarlinesCount - 1;
				dest.DiscardContents();
				global::UnityEngine.Graphics.Blit(_rtBuffer[0], dest, material, pass);
				for (num5 = 0; num5 < _rtBuffer.Length; num5++)
				{
					global::AmplifyBloom.AmplifyUtils.ReleaseTempRenderTarget(_rtBuffer[num5]);
					_rtBuffer[num5] = null;
				}
			}
			else
			{
				OnRenderFromCache(source, dest, material, m_intensity, cameraRot);
			}
		}
	}
}
