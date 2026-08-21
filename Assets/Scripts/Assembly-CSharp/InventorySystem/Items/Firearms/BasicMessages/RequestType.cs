namespace InventorySystem.Items.Firearms.BasicMessages
{
	public enum RequestType : byte
	{
		Unload = 0,
		Reload = 1,
		AdsIn = 2,
		AdsOut = 3,
		Dryfire = 4,
		ToggleFlashlight = 5,
		ReloadStop = 6,
		RequestStatuses = 7,
		Inspect = 8
	}
}
