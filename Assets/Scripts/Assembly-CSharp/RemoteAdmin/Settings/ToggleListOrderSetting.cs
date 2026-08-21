using System;
using RemoteAdmin.Generic;

namespace RemoteAdmin.Settings
{
	[Serializable]
	public class ToggleListOrderSetting : ToggleableSetting
	{
		public override string Path { get; } = "ra_playerlist_inverted";
	}
}
