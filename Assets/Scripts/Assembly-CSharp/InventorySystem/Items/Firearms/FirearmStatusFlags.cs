using System;

namespace InventorySystem.Items.Firearms
{
	[Flags]
	public enum FirearmStatusFlags : byte
	{
		None = 0,
		Cocked = 2,
		MagazineInserted = 4,
		FlashlightEnabled = 8,
		Chambered = 0x10
	}
}
