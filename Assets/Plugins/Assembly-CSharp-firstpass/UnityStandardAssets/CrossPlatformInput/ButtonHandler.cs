namespace UnityStandardAssets.CrossPlatformInput
{
	public class ButtonHandler : global::UnityEngine.MonoBehaviour
	{
		public string Name;

		private void OnEnable()
		{
		}

		public void SetDownState()
		{
			global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SetButtonDown(Name);
		}

		public void SetUpState()
		{
			global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SetButtonUp(Name);
		}

		public void SetAxisPositiveState()
		{
			global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SetAxisPositive(Name);
		}

		public void SetAxisNeutralState()
		{
			global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SetAxisZero(Name);
		}

		public void SetAxisNegativeState()
		{
			global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SetAxisNegative(Name);
		}

		public void Update()
		{
		}
	}
}
