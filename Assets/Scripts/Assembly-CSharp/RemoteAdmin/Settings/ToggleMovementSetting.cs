using System;
using RemoteAdmin.Generic;

namespace RemoteAdmin.Settings
{
	[Serializable]
	public class ToggleMovementSetting : ToggleableSetting
	{
		public override string Path { get; } = "ra_togglemove";

    }
}
