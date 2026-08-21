namespace UnityStandardAssets.Utility
{
	public class SmoothFollow : global::UnityEngine.MonoBehaviour
	{
		[global::UnityEngine.SerializeField]
		private global::UnityEngine.Transform target;

		[global::UnityEngine.SerializeField]
		private float distance = 10f;

		[global::UnityEngine.SerializeField]
		private float height = 5f;

		[global::UnityEngine.SerializeField]
		private float rotationDamping;

		[global::UnityEngine.SerializeField]
		private float heightDamping;

		private void Start()
		{
		}

		private void LateUpdate()
		{
			if ((bool)target)
			{
				float y = target.eulerAngles.y;
				float b = target.position.y + height;
				float y2 = base.transform.eulerAngles.y;
				float y3 = base.transform.position.y;
				y2 = global::UnityEngine.Mathf.LerpAngle(y2, y, rotationDamping * global::UnityEngine.Time.deltaTime);
				y3 = global::UnityEngine.Mathf.Lerp(y3, b, heightDamping * global::UnityEngine.Time.deltaTime);
				global::UnityEngine.Quaternion quaternion = global::UnityEngine.Quaternion.Euler(0f, y2, 0f);
				base.transform.position = target.position;
				base.transform.position -= quaternion * global::UnityEngine.Vector3.forward * distance;
				base.transform.position = new global::UnityEngine.Vector3(base.transform.position.x, y3, base.transform.position.z);
				base.transform.LookAt(target);
			}
		}
	}
}
