namespace UnityStandardAssets.Utility
{
	public class FollowTarget : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.Transform target;

		public global::UnityEngine.Vector3 offset = new global::UnityEngine.Vector3(0f, 7.5f, 0f);

		private void LateUpdate()
		{
			base.transform.position = target.position + offset;
		}
	}
}
