namespace UnityStandardAssets.Utility
{
	public class ParticleSystemDestroyer : global::UnityEngine.MonoBehaviour
	{
		public float minDuration = 8f;

		public float maxDuration = 10f;

		private float m_MaxLifetime;

		private bool m_EarlyStop;

		private global::System.Collections.IEnumerator Start()
		{
			global::UnityEngine.ParticleSystem[] systems = GetComponentsInChildren<global::UnityEngine.ParticleSystem>();
			global::UnityEngine.ParticleSystem[] array = systems;
			foreach (global::UnityEngine.ParticleSystem particleSystem in array)
			{
				m_MaxLifetime = global::UnityEngine.Mathf.Max(particleSystem.main.startLifetime.constant, m_MaxLifetime);
			}
			float stopTime = global::UnityEngine.Time.time + global::UnityEngine.Random.Range(minDuration, maxDuration);
			while (global::UnityEngine.Time.time < stopTime || m_EarlyStop)
			{
				yield return null;
			}
			global::UnityEngine.Debug.Log("stopping " + base.name);
			array = systems;
			for (int i = 0; i < array.Length; i++)
			{
				global::UnityEngine.ParticleSystem.EmissionModule emission = array[i].emission;
				emission.enabled = false;
			}
			BroadcastMessage("Extinguish", global::UnityEngine.SendMessageOptions.DontRequireReceiver);
			yield return new global::UnityEngine.WaitForSeconds(m_MaxLifetime);
			global::UnityEngine.Object.Destroy(base.gameObject);
		}

		public void Stop()
		{
			m_EarlyStop = true;
		}
	}
}
