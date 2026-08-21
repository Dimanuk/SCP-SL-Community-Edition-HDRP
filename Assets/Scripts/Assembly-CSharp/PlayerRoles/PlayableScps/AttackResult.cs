using System;

namespace PlayerRoles.PlayableScps
{
	[Flags]
	public enum AttackResult
	{
		None = 0,
		AttackedObject = 1,
		AttackedHuman = 2,
		KilledHuman = 6
	}
}
