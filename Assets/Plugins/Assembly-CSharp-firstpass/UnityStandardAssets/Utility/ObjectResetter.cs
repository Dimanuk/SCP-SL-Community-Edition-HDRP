namespace UnityStandardAssets.Utility
{
	public class ObjectResetter : global::UnityEngine.MonoBehaviour
	{
		private global::UnityEngine.Vector3 originalPosition;

		private global::UnityEngine.Quaternion originalRotation;

		private global::System.Collections.Generic.List<global::UnityEngine.Transform> originalStructure;

		private global::UnityEngine.Rigidbody Rigidbody;

		private void Start()
		{
			originalStructure = new global::System.Collections.Generic.List<global::UnityEngine.Transform>(GetComponentsInChildren<global::UnityEngine.Transform>());
			originalPosition = base.transform.position;
			originalRotation = base.transform.rotation;
			Rigidbody = GetComponent<global::UnityEngine.Rigidbody>();
		}

		public void DelayedReset(float delay)
		{
			StartCoroutine(ResetCoroutine(delay));
		}

		public global::System.Collections.IEnumerator ResetCoroutine(float delay)
		{
			yield return new global::UnityEngine.WaitForSeconds(delay);
			global::UnityEngine.Transform[] componentsInChildren = GetComponentsInChildren<global::UnityEngine.Transform>();
			foreach (global::UnityEngine.Transform transform in componentsInChildren)
			{
				if (!originalStructure.Contains(transform))
				{
					transform.parent = null;
				}
			}
			base.transform.position = originalPosition;
			base.transform.rotation = originalRotation;
			if ((bool)Rigidbody)
			{
				Rigidbody.linearVelocity = global::UnityEngine.Vector3.zero;
				Rigidbody.angularVelocity = global::UnityEngine.Vector3.zero;
			}
			SendMessage("Reset");
		}
	}
}
