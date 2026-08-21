namespace UnityStandardAssets.Utility
{
	[global::System.Serializable]
	public class CurveControlledBob
	{
		public float HorizontalBobRange = 0.33f;

		public float VerticalBobRange = 0.33f;

		public global::UnityEngine.AnimationCurve Bobcurve = new global::UnityEngine.AnimationCurve(new global::UnityEngine.Keyframe(0f, 0f), new global::UnityEngine.Keyframe(0.5f, 1f), new global::UnityEngine.Keyframe(1f, 0f), new global::UnityEngine.Keyframe(1.5f, -1f), new global::UnityEngine.Keyframe(2f, 0f));

		public float VerticaltoHorizontalRatio = 1f;

		private float m_CyclePositionX;

		private float m_CyclePositionY;

		private float m_BobBaseInterval;

		private global::UnityEngine.Vector3 m_OriginalCameraPosition;

		private float m_Time;

		public void Setup(global::UnityEngine.Camera camera, float bobBaseInterval)
		{
			m_BobBaseInterval = bobBaseInterval;
			m_OriginalCameraPosition = camera.transform.localPosition;
			m_Time = Bobcurve[Bobcurve.length - 1].time;
		}

		public global::UnityEngine.Vector3 DoHeadBob(float speed)
		{
			float x = m_OriginalCameraPosition.x + Bobcurve.Evaluate(m_CyclePositionX) * HorizontalBobRange * 0.5f;
			float y = m_OriginalCameraPosition.y + Bobcurve.Evaluate(m_CyclePositionY) * VerticalBobRange / 2f;
			m_CyclePositionX += speed * global::UnityEngine.Time.deltaTime / m_BobBaseInterval;
			m_CyclePositionY += speed * global::UnityEngine.Time.deltaTime / m_BobBaseInterval * VerticaltoHorizontalRatio;
			if (m_CyclePositionX > m_Time)
			{
				m_CyclePositionX -= m_Time;
			}
			if (m_CyclePositionY > m_Time)
			{
				m_CyclePositionY -= m_Time;
			}
			return new global::UnityEngine.Vector3(x, y, 0f);
		}
	}
}
