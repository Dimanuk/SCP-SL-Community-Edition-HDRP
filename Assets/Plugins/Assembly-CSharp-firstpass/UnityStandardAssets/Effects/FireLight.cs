namespace UnityStandardAssets.Effects
{
	public class FireLight : global::UnityEngine.MonoBehaviour
	{
		private float m_Rnd;

		private bool m_Burning = true;

		private global::UnityEngine.Light m_Light;

		private void Start()
		{
			m_Rnd = global::UnityEngine.Random.value * 100f;
			m_Light = GetComponent<global::UnityEngine.Light>();
		}

		private void Update()
		{
			if (m_Burning)
			{
				m_Light.intensity = 2f * global::UnityEngine.Mathf.PerlinNoise(m_Rnd + global::UnityEngine.Time.time, m_Rnd + 1f + global::UnityEngine.Time.time * 1f);
				float x = global::UnityEngine.Mathf.PerlinNoise(m_Rnd + 0f + global::UnityEngine.Time.time * 2f, m_Rnd + 1f + global::UnityEngine.Time.time * 2f) - 0.5f;
				float y = global::UnityEngine.Mathf.PerlinNoise(m_Rnd + 2f + global::UnityEngine.Time.time * 2f, m_Rnd + 3f + global::UnityEngine.Time.time * 2f) - 0.5f;
				float z = global::UnityEngine.Mathf.PerlinNoise(m_Rnd + 4f + global::UnityEngine.Time.time * 2f, m_Rnd + 5f + global::UnityEngine.Time.time * 2f) - 0.5f;
				base.transform.localPosition = global::UnityEngine.Vector3.up + new global::UnityEngine.Vector3(x, y, z) * 1f;
			}
		}

		public void Extinguish()
		{
			m_Burning = false;
			m_Light.enabled = false;
		}
	}
}
