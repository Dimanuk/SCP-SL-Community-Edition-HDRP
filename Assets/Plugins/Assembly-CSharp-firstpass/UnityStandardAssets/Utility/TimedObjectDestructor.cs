namespace UnityStandardAssets.Utility
{
	public class TimedObjectDestructor : global::UnityEngine.MonoBehaviour
	{
		[global::UnityEngine.SerializeField]
		private float m_TimeOut = 1f;

		[global::UnityEngine.SerializeField]
		private bool m_DetachChildren;

		private void Awake()
		{
			Invoke("DestroyNow", m_TimeOut);
		}

		private void DestroyNow()
		{
			if (m_DetachChildren)
			{
				base.transform.DetachChildren();
			}
			global::UnityEngine.Object.DestroyObject(base.gameObject);
		}
	}
}
