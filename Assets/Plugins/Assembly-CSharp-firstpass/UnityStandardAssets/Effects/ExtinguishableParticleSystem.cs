namespace UnityStandardAssets.Effects
{
	public class ExtinguishableParticleSystem : global::UnityEngine.MonoBehaviour
	{
		public float multiplier = 1f;

		private global::UnityEngine.ParticleSystem[] m_Systems;

		private void Start()
		{
			m_Systems = GetComponentsInChildren<global::UnityEngine.ParticleSystem>();
		}

		public void Extinguish()
		{
			global::UnityEngine.ParticleSystem[] systems = m_Systems;
			for (int i = 0; i < systems.Length; i++)
			{
				global::UnityEngine.ParticleSystem.EmissionModule emission = systems[i].emission;
				emission.enabled = false;
			}
		}
	}
}
