namespace UnityStandardAssets.Utility
{
	public class DynamicShadowSettings : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.Light sunLight;

		public float minHeight = 10f;

		public float minShadowDistance = 80f;

		public float minShadowBias = 1f;

		public float maxHeight = 1000f;

		public float maxShadowDistance = 10000f;

		public float maxShadowBias = 0.1f;

		public float adaptTime = 1f;

		private float m_SmoothHeight;

		private float m_ChangeSpeed;

		private float m_OriginalStrength = 1f;

		private void Start()
		{
			m_OriginalStrength = sunLight.shadowStrength;
		}

		private void Update()
		{
			global::UnityEngine.Ray ray = new global::UnityEngine.Ray(global::UnityEngine.Camera.main.transform.position, -global::UnityEngine.Vector3.up);
			float num = base.transform.position.y;
			if (global::UnityEngine.Physics.Raycast(ray, out var hitInfo))
			{
				num = hitInfo.distance;
			}
			if (global::UnityEngine.Mathf.Abs(num - m_SmoothHeight) > 1f)
			{
				m_SmoothHeight = global::UnityEngine.Mathf.SmoothDamp(m_SmoothHeight, num, ref m_ChangeSpeed, adaptTime);
			}
			float num2 = global::UnityEngine.Mathf.InverseLerp(minHeight, maxHeight, m_SmoothHeight);
			global::UnityEngine.QualitySettings.shadowDistance = global::UnityEngine.Mathf.Lerp(minShadowDistance, maxShadowDistance, num2);
			sunLight.shadowBias = global::UnityEngine.Mathf.Lerp(minShadowBias, maxShadowBias, 1f - (1f - num2) * (1f - num2));
			sunLight.shadowStrength = global::UnityEngine.Mathf.Lerp(m_OriginalStrength, 0f, num2);
		}
	}
}
