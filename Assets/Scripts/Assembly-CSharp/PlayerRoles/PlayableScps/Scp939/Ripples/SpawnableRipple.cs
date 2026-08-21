using System;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Ripples
{
	public class SpawnableRipple : MonoBehaviour
	{
		[CompilerGenerated]
		private static Action<SpawnableRipple> m_OnSpawned;

		[field: SerializeField]
		public float Range { get; private set; }

		public static event Action<SpawnableRipple> OnSpawned;

		private void OnEnabled()
		{
			SpawnableRipple.OnSpawned?.Invoke(this);
		}
	}
}
