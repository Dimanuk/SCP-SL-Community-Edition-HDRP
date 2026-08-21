namespace Security
{
	public enum ServerRateLimit : ushort
	{
		playerInteract = 0,
		modPref = 1,
		commands = 2,
		inventory = 3,
		itemSync = 4,
		footstep = 5,
		movementSync = 6,
		cameraSync = 7,
		medicalItem = 8
	}
}
