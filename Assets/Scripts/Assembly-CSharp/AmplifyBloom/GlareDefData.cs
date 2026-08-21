namespace AmplifyBloom
{
	[global::System.Serializable]
	public class GlareDefData
	{
		public bool FoldoutValue = true;

		[global::UnityEngine.SerializeField]
		private global::AmplifyBloom.StarLibType m_starType;

		[global::UnityEngine.SerializeField]
		private float m_starInclination;

		[global::UnityEngine.SerializeField]
		private float m_chromaticAberration;

		[global::UnityEngine.SerializeField]
		private global::AmplifyBloom.StarDefData m_customStarData;

		public global::AmplifyBloom.StarLibType StarType
		{
			get
			{
				return m_starType;
			}
			set
			{
				m_starType = value;
			}
		}

		public float StarInclination
		{
			get
			{
				return m_starInclination;
			}
			set
			{
				m_starInclination = value;
			}
		}

		public float StarInclinationDeg
		{
			get
			{
				return m_starInclination * 57.29578f;
			}
			set
			{
				m_starInclination = value * ((float)global::System.Math.PI / 180f);
			}
		}

		public float ChromaticAberration
		{
			get
			{
				return m_chromaticAberration;
			}
			set
			{
				m_chromaticAberration = value;
			}
		}

		public global::AmplifyBloom.StarDefData CustomStarData
		{
			get
			{
				return m_customStarData;
			}
			set
			{
				m_customStarData = value;
			}
		}

		public GlareDefData()
		{
			m_customStarData = new global::AmplifyBloom.StarDefData();
		}

		public GlareDefData(global::AmplifyBloom.StarLibType starType, float starInclination, float chromaticAberration)
		{
			m_starType = starType;
			m_starInclination = starInclination;
			m_chromaticAberration = chromaticAberration;
		}
	}
}
