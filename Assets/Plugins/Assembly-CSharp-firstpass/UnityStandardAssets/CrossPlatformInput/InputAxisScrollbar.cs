namespace UnityStandardAssets.CrossPlatformInput
{
	public class InputAxisScrollbar : global::UnityEngine.MonoBehaviour
	{
		public string axis;

		private void Update()
		{
		}

		public void HandleInput(float value)
		{
			global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SetAxis(axis, value * 2f - 1f);
		}
	}
}
