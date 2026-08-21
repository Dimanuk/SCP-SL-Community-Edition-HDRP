namespace UnityStandardAssets.Effects
{
	public class Hose : global::UnityEngine.MonoBehaviour
	{
		public float maxPower = 20f;

		public float minPower = 5f;

		public float changeSpeed = 5f;

		public global::UnityEngine.ParticleSystem[] hoseWaterSystems;

		public global::UnityEngine.Renderer systemRenderer;

		private float m_Power;

		private void Update()
		{
			m_Power = global::UnityEngine.Mathf.Lerp(m_Power, global::UnityEngine.Input.GetMouseButton(0) ? maxPower : minPower, global::UnityEngine.Time.deltaTime * changeSpeed);
			if (global::UnityEngine.Input.GetKeyDown(global::UnityEngine.KeyCode.Alpha1))
			{
				systemRenderer.enabled = !systemRenderer.enabled;
			}
			global::UnityEngine.ParticleSystem[] array = hoseWaterSystems;
			foreach (global::UnityEngine.ParticleSystem obj in array)
			{
				global::UnityEngine.ParticleSystem.MainModule main = obj.main;
				main.startSpeed = m_Power;
				global::UnityEngine.ParticleSystem.EmissionModule emission = obj.emission;
				emission.enabled = m_Power > minPower * 1.1f;
			}
		}
	}
}
