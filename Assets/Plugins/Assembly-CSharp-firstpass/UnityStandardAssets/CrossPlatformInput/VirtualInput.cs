namespace UnityStandardAssets.CrossPlatformInput
{
	public abstract class VirtualInput
	{
		protected global::System.Collections.Generic.Dictionary<string, global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualAxis> m_VirtualAxes = new global::System.Collections.Generic.Dictionary<string, global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualAxis>();

		protected global::System.Collections.Generic.Dictionary<string, global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualButton> m_VirtualButtons = new global::System.Collections.Generic.Dictionary<string, global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualButton>();

		protected global::System.Collections.Generic.List<string> m_AlwaysUseVirtual = new global::System.Collections.Generic.List<string>();

		public global::UnityEngine.Vector3 virtualMousePosition { get; private set; }

		public bool AxisExists(string name)
		{
			return m_VirtualAxes.ContainsKey(name);
		}

		public bool ButtonExists(string name)
		{
			return m_VirtualButtons.ContainsKey(name);
		}

		public void RegisterVirtualAxis(global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualAxis axis)
		{
			if (m_VirtualAxes.ContainsKey(axis.name))
			{
				global::UnityEngine.Debug.LogError("There is already a virtual axis named " + axis.name + " registered.");
				return;
			}
			m_VirtualAxes.Add(axis.name, axis);
			if (!axis.matchWithInputManager)
			{
				m_AlwaysUseVirtual.Add(axis.name);
			}
		}

		public void RegisterVirtualButton(global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualButton button)
		{
			if (m_VirtualButtons.ContainsKey(button.name))
			{
				global::UnityEngine.Debug.LogError("There is already a virtual button named " + button.name + " registered.");
				return;
			}
			m_VirtualButtons.Add(button.name, button);
			if (!button.matchWithInputManager)
			{
				m_AlwaysUseVirtual.Add(button.name);
			}
		}

		public void UnRegisterVirtualAxis(string name)
		{
			if (m_VirtualAxes.ContainsKey(name))
			{
				m_VirtualAxes.Remove(name);
			}
		}

		public void UnRegisterVirtualButton(string name)
		{
			if (m_VirtualButtons.ContainsKey(name))
			{
				m_VirtualButtons.Remove(name);
			}
		}

		public global::UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
		{
			if (!m_VirtualAxes.ContainsKey(name))
			{
				return null;
			}
			return m_VirtualAxes[name];
		}

		public void SetVirtualMousePositionX(float f)
		{
			virtualMousePosition = new global::UnityEngine.Vector3(f, virtualMousePosition.y, virtualMousePosition.z);
		}

		public void SetVirtualMousePositionY(float f)
		{
			virtualMousePosition = new global::UnityEngine.Vector3(virtualMousePosition.x, f, virtualMousePosition.z);
		}

		public void SetVirtualMousePositionZ(float f)
		{
			virtualMousePosition = new global::UnityEngine.Vector3(virtualMousePosition.x, virtualMousePosition.y, f);
		}

		public abstract float GetAxis(string name, bool raw);

		public abstract bool GetButton(string name);

		public abstract bool GetButtonDown(string name);

		public abstract bool GetButtonUp(string name);

		public abstract void SetButtonDown(string name);

		public abstract void SetButtonUp(string name);

		public abstract void SetAxisPositive(string name);

		public abstract void SetAxisNegative(string name);

		public abstract void SetAxisZero(string name);

		public abstract void SetAxis(string name, float value);

		public abstract global::UnityEngine.Vector3 MousePosition();
	}
}
