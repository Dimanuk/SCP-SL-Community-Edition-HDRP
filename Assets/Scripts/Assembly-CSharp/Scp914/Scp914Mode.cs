using System;

namespace Scp914
{
	[Flags]
	public enum Scp914Mode
	{
		Dropped = 1,
		Inventory = 2,
		Held = 6,
		DroppedAndPlayerTeleport = 5,
		DroppedAndInventory = 3,
		DroppedAndHeld = 7
	}
}
