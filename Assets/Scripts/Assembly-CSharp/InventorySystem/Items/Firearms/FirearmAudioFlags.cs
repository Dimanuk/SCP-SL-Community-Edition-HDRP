using System;

namespace InventorySystem.Items.Firearms
{
	[Flags]
	public enum FirearmAudioFlags : byte
	{
		None = 0,
		ScaleDistance = 2,
		IsGunshot = 4,
		SendToPlayers = 8,
		UseDedicatedAudioChannel = 0x10
	}
}
