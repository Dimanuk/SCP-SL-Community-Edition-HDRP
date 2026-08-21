namespace UnityStandardAssets.Effects
{
	public class ParticleSystemMultiplier : global::UnityEngine.MonoBehaviour
	{
		public float multiplier = 1f;

		private void Start()
		{
			global::UnityEngine.ParticleSystem[] componentsInChildren = GetComponentsInChildren<global::UnityEngine.ParticleSystem>();
			foreach (global::UnityEngine.ParticleSystem obj in componentsInChildren)
			{
				global::UnityEngine.ParticleSystem.MainModule main = obj.main;
				main.startSizeMultiplier = multiplier;
				main.startSpeedMultiplier = multiplier;
				main.startLifetimeMultiplier = global::UnityEngine.Mathf.Lerp(multiplier, 1f, 0.5f);
				obj.Clear();
				obj.Play();
			}
		}
	}
}
