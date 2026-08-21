namespace UnityStandardAssets.Utility
{
	[global::System.Serializable]
	public class FOVKick
	{
		public global::UnityEngine.Camera Camera;

		[global::UnityEngine.HideInInspector]
		public float originalFov;

		public float FOVIncrease = 3f;

		public float TimeToIncrease = 1f;

		public float TimeToDecrease = 1f;

		public global::UnityEngine.AnimationCurve IncreaseCurve;

		public void Setup(global::UnityEngine.Camera camera)
		{
			CheckStatus(camera);
			Camera = camera;
			originalFov = camera.fieldOfView;
		}

		private void CheckStatus(global::UnityEngine.Camera camera)
		{
			if (camera == null)
			{
				throw new global::System.Exception("FOVKick camera is null, please supply the camera to the constructor");
			}
			if (IncreaseCurve == null)
			{
				throw new global::System.Exception("FOVKick Increase curve is null, please define the curve for the field of view kicks");
			}
		}

		public void ChangeCamera(global::UnityEngine.Camera camera)
		{
			Camera = camera;
		}

		public global::System.Collections.IEnumerator FOVKickUp()
		{
			float t = global::UnityEngine.Mathf.Abs((Camera.fieldOfView - originalFov) / FOVIncrease);
			while (t < TimeToIncrease)
			{
				Camera.fieldOfView = originalFov + IncreaseCurve.Evaluate(t / TimeToIncrease) * FOVIncrease;
				t += global::UnityEngine.Time.deltaTime;
				yield return new global::UnityEngine.WaitForEndOfFrame();
			}
		}

		public global::System.Collections.IEnumerator FOVKickDown()
		{
			float t = global::UnityEngine.Mathf.Abs((Camera.fieldOfView - originalFov) / FOVIncrease);
			while (t > 0f)
			{
				Camera.fieldOfView = originalFov + IncreaseCurve.Evaluate(t / TimeToDecrease) * FOVIncrease;
				t -= global::UnityEngine.Time.deltaTime;
				yield return new global::UnityEngine.WaitForEndOfFrame();
			}
			Camera.fieldOfView = originalFov;
		}
	}
}
