namespace UnityStandardAssets.Utility
{
	public class AutoMoveAndRotate : global::UnityEngine.MonoBehaviour
	{
		[global::System.Serializable]
		public class Vector3andSpace
		{
			public global::UnityEngine.Vector3 value;

			public global::UnityEngine.Space space = global::UnityEngine.Space.Self;
		}

		public global::UnityStandardAssets.Utility.AutoMoveAndRotate.Vector3andSpace moveUnitsPerSecond;

		public global::UnityStandardAssets.Utility.AutoMoveAndRotate.Vector3andSpace rotateDegreesPerSecond;

		public bool ignoreTimescale;

		private float m_LastRealTime;

		private void Start()
		{
			m_LastRealTime = global::UnityEngine.Time.realtimeSinceStartup;
		}

		private void Update()
		{
			float num = global::UnityEngine.Time.deltaTime;
			if (ignoreTimescale)
			{
				num = global::UnityEngine.Time.realtimeSinceStartup - m_LastRealTime;
				m_LastRealTime = global::UnityEngine.Time.realtimeSinceStartup;
			}
			base.transform.Translate(moveUnitsPerSecond.value * num, moveUnitsPerSecond.space);
			base.transform.Rotate(rotateDegreesPerSecond.value * num, moveUnitsPerSecond.space);
		}
	}
}
