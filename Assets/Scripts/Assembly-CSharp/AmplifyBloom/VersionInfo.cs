namespace AmplifyBloom
{
	[global::System.Serializable]
	public class VersionInfo
	{
		public const byte Major = 1;

		public const byte Minor = 2;

		public const byte Release = 0;

		private static string StageSuffix = "_dev001";

		[global::UnityEngine.SerializeField]
		private int m_major;

		[global::UnityEngine.SerializeField]
		private int m_minor;

		[global::UnityEngine.SerializeField]
		private int m_release;

		public int Number => m_major * 100 + m_minor * 10 + m_release;

		public static string StaticToString()
		{
			return $"{(byte)1}.{(byte)2}.{(byte)0}" + StageSuffix;
		}

		public override string ToString()
		{
			return $"{m_major}.{m_minor}.{m_release}" + StageSuffix;
		}

		private VersionInfo()
		{
			m_major = 1;
			m_minor = 2;
			m_release = 0;
		}

		private VersionInfo(byte major, byte minor, byte release)
		{
			m_major = major;
			m_minor = minor;
			m_release = release;
		}

		public static global::AmplifyBloom.VersionInfo Current()
		{
			return new global::AmplifyBloom.VersionInfo(1, 2, 0);
		}

		public static bool Matches(global::AmplifyBloom.VersionInfo version)
		{
			if (1 == version.m_major && 2 == version.m_minor)
			{
				return version.m_release == 0;
			}
			return false;
		}
	}
}
