namespace AmplifyBloom
{
	[global::System.Serializable]
	public sealed class AmplifyBokeh : global::AmplifyBloom.IAmplifyItem, global::UnityEngine.ISerializationCallbackReceiver
	{
		private const int PerPassSampleCount = 8;

		[global::UnityEngine.SerializeField]
		private bool m_isActive;

		[global::UnityEngine.SerializeField]
		private bool m_applyOnBloomSource;

		[global::UnityEngine.SerializeField]
		private float m_bokehSampleRadius = 0.5f;

		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Vector4 m_bokehCameraProperties = new global::UnityEngine.Vector4(0.05f, 0.018f, 1.34f, 0.18f);

		[global::UnityEngine.SerializeField]
		private float m_offsetRotation;

		[global::UnityEngine.SerializeField]
		private global::AmplifyBloom.ApertureShape m_apertureShape = global::AmplifyBloom.ApertureShape.Hexagon;

		private global::System.Collections.Generic.List<global::AmplifyBloom.AmplifyBokehData> m_bokehOffsets;

		public global::AmplifyBloom.ApertureShape ApertureShape
		{
			get
			{
				return m_apertureShape;
			}
			set
			{
				if (m_apertureShape != value)
				{
					m_apertureShape = value;
					CreateBokehOffsets(value);
				}
			}
		}

		public bool ApplyBokeh
		{
			get
			{
				return m_isActive;
			}
			set
			{
				m_isActive = value;
			}
		}

		public bool ApplyOnBloomSource
		{
			get
			{
				return m_applyOnBloomSource;
			}
			set
			{
				m_applyOnBloomSource = value;
			}
		}

		public float BokehSampleRadius
		{
			get
			{
				return m_bokehSampleRadius;
			}
			set
			{
				m_bokehSampleRadius = value;
			}
		}

		public float OffsetRotation
		{
			get
			{
				return m_offsetRotation;
			}
			set
			{
				m_offsetRotation = value;
			}
		}

		public global::UnityEngine.Vector4 BokehCameraProperties
		{
			get
			{
				return m_bokehCameraProperties;
			}
			set
			{
				m_bokehCameraProperties = value;
			}
		}

		public float Aperture
		{
			get
			{
				return m_bokehCameraProperties.x;
			}
			set
			{
				m_bokehCameraProperties.x = value;
			}
		}

		public float FocalLength
		{
			get
			{
				return m_bokehCameraProperties.y;
			}
			set
			{
				m_bokehCameraProperties.y = value;
			}
		}

		public float FocalDistance
		{
			get
			{
				return m_bokehCameraProperties.z;
			}
			set
			{
				m_bokehCameraProperties.z = value;
			}
		}

		public float MaxCoCDiameter
		{
			get
			{
				return m_bokehCameraProperties.w;
			}
			set
			{
				m_bokehCameraProperties.w = value;
			}
		}

		public AmplifyBokeh()
		{
			m_bokehOffsets = new global::System.Collections.Generic.List<global::AmplifyBloom.AmplifyBokehData>();
			CreateBokehOffsets(global::AmplifyBloom.ApertureShape.Hexagon);
		}

		public void Destroy()
		{
			for (int i = 0; i < m_bokehOffsets.Count; i++)
			{
				m_bokehOffsets[i].Destroy();
			}
		}

		private void CreateBokehOffsets(global::AmplifyBloom.ApertureShape shape)
		{
			m_bokehOffsets.Clear();
			switch (shape)
			{
			case global::AmplifyBloom.ApertureShape.Square:
				m_bokehOffsets.Add(new global::AmplifyBloom.AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation)));
				m_bokehOffsets.Add(new global::AmplifyBloom.AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation + 90f)));
				break;
			case global::AmplifyBloom.ApertureShape.Hexagon:
				m_bokehOffsets.Add(new global::AmplifyBloom.AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation)));
				m_bokehOffsets.Add(new global::AmplifyBloom.AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation - 75f)));
				m_bokehOffsets.Add(new global::AmplifyBloom.AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation + 75f)));
				break;
			case global::AmplifyBloom.ApertureShape.Octagon:
				m_bokehOffsets.Add(new global::AmplifyBloom.AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation)));
				m_bokehOffsets.Add(new global::AmplifyBloom.AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation + 65f)));
				m_bokehOffsets.Add(new global::AmplifyBloom.AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation + 90f)));
				m_bokehOffsets.Add(new global::AmplifyBloom.AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation + 115f)));
				break;
			}
		}

		private global::UnityEngine.Vector4[] CalculateBokehSamples(int sampleCount, float angle)
		{
			global::UnityEngine.Vector4[] array = new global::UnityEngine.Vector4[sampleCount];
			float f = (float)global::System.Math.PI / 180f * angle;
			float num = (float)global::UnityEngine.Screen.width / (float)global::UnityEngine.Screen.height;
			global::UnityEngine.Vector4 vector = new global::UnityEngine.Vector4(m_bokehSampleRadius * global::UnityEngine.Mathf.Cos(f), m_bokehSampleRadius * global::UnityEngine.Mathf.Sin(f));
			vector.x /= num;
			for (int i = 0; i < sampleCount; i++)
			{
				float t = (float)i / ((float)sampleCount - 1f);
				array[i] = global::UnityEngine.Vector4.Lerp(-vector, vector, t);
			}
			return array;
		}

		public void ApplyBokehFilter(global::UnityEngine.RenderTexture source, global::UnityEngine.Material material)
		{
			for (int i = 0; i < m_bokehOffsets.Count; i++)
			{
				m_bokehOffsets[i].BokehRenderTexture = global::AmplifyBloom.AmplifyUtils.GetTempRenderTarget(source.width, source.height);
			}
			material.SetVector(global::AmplifyBloom.AmplifyUtils.BokehParamsId, m_bokehCameraProperties);
			for (int j = 0; j < m_bokehOffsets.Count; j++)
			{
				for (int k = 0; k < 8; k++)
				{
					material.SetVector(global::AmplifyBloom.AmplifyUtils.AnamorphicGlareWeightsStr[k], m_bokehOffsets[j].Offsets[k]);
				}
				global::UnityEngine.Graphics.Blit(source, m_bokehOffsets[j].BokehRenderTexture, material, 27);
			}
			for (int l = 0; l < m_bokehOffsets.Count - 1; l++)
			{
				material.SetTexture(global::AmplifyBloom.AmplifyUtils.AnamorphicRTS[l], m_bokehOffsets[l].BokehRenderTexture);
			}
			source.DiscardContents();
			global::UnityEngine.Graphics.Blit(m_bokehOffsets[m_bokehOffsets.Count - 1].BokehRenderTexture, source, material, 28 + (m_bokehOffsets.Count - 2));
			for (int m = 0; m < m_bokehOffsets.Count; m++)
			{
				global::AmplifyBloom.AmplifyUtils.ReleaseTempRenderTarget(m_bokehOffsets[m].BokehRenderTexture);
				m_bokehOffsets[m].BokehRenderTexture = null;
			}
		}

		public void OnAfterDeserialize()
		{
			CreateBokehOffsets(m_apertureShape);
		}

		public void OnBeforeSerialize()
		{
		}
	}
}
