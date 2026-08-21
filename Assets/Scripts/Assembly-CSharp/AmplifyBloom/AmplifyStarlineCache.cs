namespace AmplifyBloom
{
	[global::System.Serializable]
	public class AmplifyStarlineCache
	{
		[global::UnityEngine.SerializeField]
		internal global::AmplifyBloom.AmplifyPassCache[] Passes;

		public AmplifyStarlineCache()
		{
			Passes = new global::AmplifyBloom.AmplifyPassCache[4];
			for (int i = 0; i < 4; i++)
			{
				Passes[i] = new global::AmplifyBloom.AmplifyPassCache();
			}
		}

		public void Destroy()
		{
			for (int i = 0; i < 4; i++)
			{
				Passes[i].Destroy();
			}
			Passes = null;
		}
	}
}
