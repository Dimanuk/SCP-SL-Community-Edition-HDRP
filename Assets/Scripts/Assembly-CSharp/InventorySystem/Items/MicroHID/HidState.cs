namespace InventorySystem.Items.MicroHID
{
	public enum HidState : byte
	{
		Idle = 0,
		PoweringUp = 1,
		PoweringDown = 2,
		Primed = 3,
		Firing = 4,
		StopSound = 5
	}
}
