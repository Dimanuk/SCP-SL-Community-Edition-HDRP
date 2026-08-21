namespace UnityStandardAssets.Utility
{
	public class CameraRefocus
	{
		public global::UnityEngine.Camera Camera;

		public global::UnityEngine.Vector3 Lookatpoint;

		public global::UnityEngine.Transform Parent;

		private global::UnityEngine.Vector3 m_OrigCameraPos;

		private bool m_Refocus;

		public CameraRefocus(global::UnityEngine.Camera camera, global::UnityEngine.Transform parent, global::UnityEngine.Vector3 origCameraPos)
		{
			m_OrigCameraPos = origCameraPos;
			Camera = camera;
			Parent = parent;
		}

		public void ChangeCamera(global::UnityEngine.Camera camera)
		{
			Camera = camera;
		}

		public void ChangeParent(global::UnityEngine.Transform parent)
		{
			Parent = parent;
		}

		public void GetFocusPoint()
		{
			if (global::UnityEngine.Physics.Raycast(Parent.transform.position + m_OrigCameraPos, Parent.transform.forward, out var hitInfo, 100f))
			{
				Lookatpoint = hitInfo.point;
				m_Refocus = true;
			}
			else
			{
				m_Refocus = false;
			}
		}

		public void SetFocusPoint()
		{
			if (m_Refocus)
			{
				Camera.transform.LookAt(Lookatpoint);
			}
		}
	}
}
