using System;
using RemoteAdmin.Generic;

namespace RemoteAdmin.Settings
{
	[Serializable]
	public class ToggleTimestampsSetting : ToggleableSetting
	{
		public override string Path { get; } = "ra_timestamps";
	}
}
