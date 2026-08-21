namespace AmplifyBloom
{
	[global::System.Serializable]
	public class AmplifyBokehData
	{
		internal global::UnityEngine.RenderTexture BokehRenderTexture;

		internal global::UnityEngine.Vector4[] Offsets;

		public AmplifyBokehData(global::UnityEngine.Vector4[] offsets)
		{
			Offsets = offsets;
		}

		public void Destroy()
		{
			if (BokehRenderTexture != null)
			{
				global::AmplifyBloom.AmplifyUtils.ReleaseTempRenderTarget(BokehRenderTexture);
				BokehRenderTexture = null;
			}
			Offsets = null;
		}
	}
}
