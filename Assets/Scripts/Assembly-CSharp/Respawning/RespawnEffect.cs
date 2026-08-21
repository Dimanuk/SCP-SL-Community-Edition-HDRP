using System;
using PlayerRoles;
using UnityEngine;

namespace Respawning
{
	[Serializable]
	public struct RespawnEffect
	{
		public SpawnableTeamType TargetTeam;

		public bool WhitelistEnabled;

		public RoleTypeId[] WhitelistedRoles;

		public Animator AnimatorEffects;

		public AudioSource AudioAnnouncement;
	}
}
