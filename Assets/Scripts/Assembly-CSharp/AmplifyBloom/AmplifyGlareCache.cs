namespace AmplifyBloom
{
	[global::System.Serializable]
	public class AmplifyGlareCache
	{
		[global::UnityEngine.SerializeField]
		internal global::AmplifyBloom.AmplifyStarlineCache[] Starlines;

		[global::UnityEngine.SerializeField]
		internal global::UnityEngine.Vector4 AverageWeight;

		[global::UnityEngine.SerializeField]
		internal global::UnityEngine.Vector4[,] CromaticAberrationMat;

		[global::UnityEngine.SerializeField]
		internal int TotalRT;

		[global::UnityEngine.SerializeField]
		internal global::AmplifyBloom.GlareDefData GlareDef;

		[global::UnityEngine.SerializeField]
		internal global::AmplifyBloom.StarDefData StarDef;

		[global::UnityEngine.SerializeField]
		internal int CurrentPassCount;

		public AmplifyGlareCache()
		{
			Starlines = new global::AmplifyBloom.AmplifyStarlineCache[4];
			CromaticAberrationMat = new global::UnityEngine.Vector4[4, 8];
			for (int i = 0; i < 4; i++)
			{
				Starlines[i] = new global::AmplifyBloom.AmplifyStarlineCache();
			}
		}

		public void Destroy()
		{
			for (int i = 0; i < 4; i++)
			{
				Starlines[i].Destroy();
			}
			Starlines = null;
			CromaticAberrationMat = null;
		}
	}
}
