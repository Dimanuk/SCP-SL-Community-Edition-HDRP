namespace UnityStandardAssets.CrossPlatformInput
{
	public class AxisTouchButton : global::UnityEngine.MonoBehaviour, global::UnityEngine.EventSystems.IPointerDownHandler, global::UnityEngine.EventSystems.IEventSystemHandler, global::UnityEngine.EventSystems.IPointerUpHandler
	{
		public string axisName;

		public float axisValue;

		public float responseSpeed;

		public float returnToCentreSpeed;

		private global::UnityStandardAssets.CrossPlatformInput.AxisTouchButton m_PairedWith;

		private global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualAxis m_Axis;

		private void OnEnable()
		{
		}

		private void FindPairedButton()
		{
		}

		private void OnDisable()
		{
		}

		public void OnPointerDown(global::UnityEngine.EventSystems.PointerEventData data)
		{
		}

		public void OnPointerUp(global::UnityEngine.EventSystems.PointerEventData data)
		{
		}
	}
}
