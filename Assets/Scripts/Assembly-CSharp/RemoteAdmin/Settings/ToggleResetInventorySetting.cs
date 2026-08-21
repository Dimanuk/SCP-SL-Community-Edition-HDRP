using System;
using RemoteAdmin.Generic;

namespace RemoteAdmin.Settings
{
	[Serializable]
	public class ToggleResetInventorySetting : ToggleableSetting
	{
		public override string Path { get; } = "ra_resetinventory";

        public override bool DefaultValue => true;
	}
}
