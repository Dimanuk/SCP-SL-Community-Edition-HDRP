namespace UnityStandardAssets.CrossPlatformInput
{
	public class Joystick : global::UnityEngine.MonoBehaviour, global::UnityEngine.EventSystems.IPointerDownHandler, global::UnityEngine.EventSystems.IEventSystemHandler, global::UnityEngine.EventSystems.IPointerUpHandler, global::UnityEngine.EventSystems.IDragHandler
	{
		public enum AxisOption
		{
			Both = 0,
			OnlyHorizontal = 1,
			OnlyVertical = 2
		}

		public int MovementRange;

		public global::UnityStandardAssets.CrossPlatformInput.Joystick.AxisOption axesToUse;

		public string horizontalAxisName;

		public string verticalAxisName;

		private global::UnityEngine.Vector3 m_StartPos;

		private bool m_UseX;

		private bool m_UseY;

		private global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;

		private global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

		private void OnEnable()
		{
		}

		private void Start()
		{
		}

		private void UpdateVirtualAxes(global::UnityEngine.Vector3 value)
		{
		}

		private void CreateVirtualAxes()
		{
		}

		public void OnDrag(global::UnityEngine.EventSystems.PointerEventData data)
		{
		}

		public void OnPointerUp(global::UnityEngine.EventSystems.PointerEventData data)
		{
		}

		public void OnPointerDown(global::UnityEngine.EventSystems.PointerEventData data)
		{
		}

		private void OnDisable()
		{
		}
	}
}
