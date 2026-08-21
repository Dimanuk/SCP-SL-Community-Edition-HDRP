using System;
using RemoteAdmin.Generic;

namespace RemoteAdmin.Settings
{
	[Serializable]
	public class ToggleItemOrderSetting : ToggleableSetting
	{
		public override string Path { get; } = "ra_invmanager_inverted";
	}
}
