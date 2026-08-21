namespace AmplifyBloom
{
	[global::System.Serializable]
	public class AmplifyPassCache
	{
		[global::UnityEngine.SerializeField]
		internal global::UnityEngine.Vector4[] Offsets;

		[global::UnityEngine.SerializeField]
		internal global::UnityEngine.Vector4[] Weights;

		public AmplifyPassCache()
		{
			Offsets = new global::UnityEngine.Vector4[16];
			Weights = new global::UnityEngine.Vector4[16];
		}

		public void Destroy()
		{
			Offsets = null;
			Weights = null;
		}
	}
}
