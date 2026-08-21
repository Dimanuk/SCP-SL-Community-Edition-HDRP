using System;

[Serializable]
public struct ClutterStruct
{
	public string descriptor;

	public Holidays[] validTimespan;

	public bool invertTimespan;

	public float chanceToSpawn;

	public Clutter clutterComponent;
}
