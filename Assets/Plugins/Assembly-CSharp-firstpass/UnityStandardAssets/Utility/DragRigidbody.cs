namespace UnityStandardAssets.Utility
{
	public class DragRigidbody : global::UnityEngine.MonoBehaviour
	{
		private const float k_Spring = 50f;

		private const float k_Damper = 5f;

		private const float k_Drag = 10f;

		private const float k_AngularDrag = 5f;

		private const float k_Distance = 0.2f;

		private const bool k_AttachToCenterOfMass = false;

		private global::UnityEngine.SpringJoint m_SpringJoint;

		private void Update()
		{
			if (!global::UnityEngine.Input.GetMouseButtonDown(0))
			{
				return;
			}
			global::UnityEngine.Camera camera = FindCamera();
			global::UnityEngine.RaycastHit hitInfo = default(global::UnityEngine.RaycastHit);
			if (global::UnityEngine.Physics.Raycast(camera.ScreenPointToRay(global::UnityEngine.Input.mousePosition).origin, camera.ScreenPointToRay(global::UnityEngine.Input.mousePosition).direction, out hitInfo, 100f, -5) && (bool)hitInfo.rigidbody && !hitInfo.rigidbody.isKinematic)
			{
				if (!m_SpringJoint)
				{
					global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Rigidbody dragger");
					global::UnityEngine.Rigidbody rigidbody = gameObject.AddComponent<global::UnityEngine.Rigidbody>();
					m_SpringJoint = gameObject.AddComponent<global::UnityEngine.SpringJoint>();
					rigidbody.isKinematic = true;
				}
				m_SpringJoint.transform.position = hitInfo.point;
				m_SpringJoint.anchor = global::UnityEngine.Vector3.zero;
				m_SpringJoint.spring = 50f;
				m_SpringJoint.damper = 5f;
				m_SpringJoint.maxDistance = 0.2f;
				m_SpringJoint.connectedBody = hitInfo.rigidbody;
				StartCoroutine("DragObject", hitInfo.distance);
			}
		}

		private global::System.Collections.IEnumerator DragObject(float distance)
		{
			float oldDrag = m_SpringJoint.connectedBody.linearDamping;
			float oldAngularDrag = m_SpringJoint.connectedBody.angularDamping;
			m_SpringJoint.connectedBody.linearDamping = 10f;
			m_SpringJoint.connectedBody.angularDamping = 5f;
			global::UnityEngine.Camera mainCamera = FindCamera();
			while (global::UnityEngine.Input.GetMouseButton(0))
			{
				global::UnityEngine.Ray ray = mainCamera.ScreenPointToRay(global::UnityEngine.Input.mousePosition);
				m_SpringJoint.transform.position = ray.GetPoint(distance);
				yield return null;
			}
			if ((bool)m_SpringJoint.connectedBody)
			{
				m_SpringJoint.connectedBody.linearDamping = oldDrag;
				m_SpringJoint.connectedBody.angularDamping = oldAngularDrag;
				m_SpringJoint.connectedBody = null;
			}
		}

		private global::UnityEngine.Camera FindCamera()
		{
			if ((bool)GetComponent<global::UnityEngine.Camera>())
			{
				return GetComponent<global::UnityEngine.Camera>();
			}
			return global::UnityEngine.Camera.main;
		}
	}
}
