namespace UnityStandardAssets.CrossPlatformInput
{
	public class TouchPad : global::UnityEngine.MonoBehaviour, global::UnityEngine.EventSystems.IPointerDownHandler, global::UnityEngine.EventSystems.IEventSystemHandler, global::UnityEngine.EventSystems.IPointerUpHandler
	{
		public enum AxisOption
		{
			Both = 0,
			OnlyHorizontal = 1,
			OnlyVertical = 2
		}

		public enum ControlStyle
		{
			Absolute = 0,
			Relative = 1,
			Swipe = 2
		}

		public global::UnityStandardAssets.CrossPlatformInput.TouchPad.AxisOption axesToUse;

		public global::UnityStandardAssets.CrossPlatformInput.TouchPad.ControlStyle controlStyle;

		public string horizontalAxisName;

		public string verticalAxisName;

		public float Xsensitivity;

		public float Ysensitivity;

		private global::UnityEngine.Vector3 m_StartPos;

		private global::UnityEngine.Vector2 m_PreviousDelta;

		private global::UnityEngine.Vector3 m_JoytickOutput;

		private bool m_UseX;

		private bool m_UseY;

		private global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;

		private global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

		private bool m_Dragging;

		private int m_Id;

		private global::UnityEngine.Vector2 m_PreviousTouchPos;

		private global::UnityEngine.Vector3 m_Center;

		private global::UnityEngine.UI.Image m_Image;

		private void OnEnable()
		{
		}

		private void Start()
		{
		}

		private void CreateVirtualAxes()
		{
		}

		private void UpdateVirtualAxes(global::UnityEngine.Vector3 value)
		{
		}

		public void OnPointerDown(global::UnityEngine.EventSystems.PointerEventData data)
		{
		}

		private void Update()
		{
		}

		public void OnPointerUp(global::UnityEngine.EventSystems.PointerEventData data)
		{
		}

		private void OnDisable()
		{
		}
	}
}
