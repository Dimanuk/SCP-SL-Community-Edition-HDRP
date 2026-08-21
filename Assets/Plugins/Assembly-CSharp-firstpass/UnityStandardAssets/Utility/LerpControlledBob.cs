namespace UnityStandardAssets.Utility
{
	[global::System.Serializable]
	public class LerpControlledBob
	{
		public float BobDuration;

		public float BobAmount;

		private float m_Offset;

		public float Offset()
		{
			return m_Offset;
		}

		public global::System.Collections.IEnumerator DoBobCycle()
		{
			float t = 0f;
			while (t < BobDuration)
			{
				m_Offset = global::UnityEngine.Mathf.Lerp(0f, BobAmount, t / BobDuration);
				t += global::UnityEngine.Time.deltaTime;
				yield return new global::UnityEngine.WaitForFixedUpdate();
			}
			t = 0f;
			while (t < BobDuration)
			{
				m_Offset = global::UnityEngine.Mathf.Lerp(BobAmount, 0f, t / BobDuration);
				t += global::UnityEngine.Time.deltaTime;
				yield return new global::UnityEngine.WaitForFixedUpdate();
			}
			m_Offset = 0f;
		}
	}
}
