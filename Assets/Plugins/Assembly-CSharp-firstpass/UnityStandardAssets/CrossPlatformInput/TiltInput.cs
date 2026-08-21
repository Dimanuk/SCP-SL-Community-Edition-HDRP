namespace UnityStandardAssets.CrossPlatformInput
{
	public class TiltInput : global::UnityEngine.MonoBehaviour
	{
		public enum AxisOptions
		{
			ForwardAxis = 0,
			SidewaysAxis = 1
		}

		[global::System.Serializable]
		public class AxisMapping
		{
			public enum MappingType
			{
				NamedAxis = 0,
				MousePositionX = 1,
				MousePositionY = 2,
				MousePositionZ = 3
			}

			public global::UnityStandardAssets.CrossPlatformInput.TiltInput.AxisMapping.MappingType type;

			public string axisName;
		}

		public global::UnityStandardAssets.CrossPlatformInput.TiltInput.AxisMapping mapping;

		public global::UnityStandardAssets.CrossPlatformInput.TiltInput.AxisOptions tiltAroundAxis;

		public float fullTiltAngle = 25f;

		public float centreAngleOffset;

		private global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualAxis m_SteerAxis;

		private void OnEnable()
		{
			if (mapping.type == global::UnityStandardAssets.CrossPlatformInput.TiltInput.AxisMapping.MappingType.NamedAxis)
			{
				m_SteerAxis = new global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualAxis(mapping.axisName);
				global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.RegisterVirtualAxis(m_SteerAxis);
			}
		}

		private void Update()
		{
			float value = 0f;
			if (global::UnityEngine.Input.acceleration != global::UnityEngine.Vector3.zero)
			{
				switch (tiltAroundAxis)
				{
				case global::UnityStandardAssets.CrossPlatformInput.TiltInput.AxisOptions.ForwardAxis:
					value = global::UnityEngine.Mathf.Atan2(global::UnityEngine.Input.acceleration.x, 0f - global::UnityEngine.Input.acceleration.y) * 57.29578f + centreAngleOffset;
					break;
				case global::UnityStandardAssets.CrossPlatformInput.TiltInput.AxisOptions.SidewaysAxis:
					value = global::UnityEngine.Mathf.Atan2(global::UnityEngine.Input.acceleration.z, 0f - global::UnityEngine.Input.acceleration.y) * 57.29578f + centreAngleOffset;
					break;
				}
			}
			float num = global::UnityEngine.Mathf.InverseLerp(0f - fullTiltAngle, fullTiltAngle, value) * 2f - 1f;
			switch (mapping.type)
			{
			case global::UnityStandardAssets.CrossPlatformInput.TiltInput.AxisMapping.MappingType.NamedAxis:
				m_SteerAxis.Update(num);
				break;
			case global::UnityStandardAssets.CrossPlatformInput.TiltInput.AxisMapping.MappingType.MousePositionX:
				global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SetVirtualMousePositionX(num * (float)global::UnityEngine.Screen.width);
				break;
			case global::UnityStandardAssets.CrossPlatformInput.TiltInput.AxisMapping.MappingType.MousePositionY:
				global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SetVirtualMousePositionY(num * (float)global::UnityEngine.Screen.width);
				break;
			case global::UnityStandardAssets.CrossPlatformInput.TiltInput.AxisMapping.MappingType.MousePositionZ:
				global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SetVirtualMousePositionZ(num * (float)global::UnityEngine.Screen.width);
				break;
			}
		}

		private void OnDisable()
		{
			m_SteerAxis.Remove();
		}
	}
}
