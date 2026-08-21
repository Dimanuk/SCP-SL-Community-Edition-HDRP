using System;
using RemoteAdmin.Generic;

namespace RemoteAdmin.Settings
{
	[Serializable]
	public class ToggleSuggestionsSetting : ToggleableSetting
	{
		public override bool DefaultValue { get; } = true;

		public override string Path { get; } = "ra_commandsuggestions";
	}
}
