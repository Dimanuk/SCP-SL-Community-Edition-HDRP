using System.Runtime.InteropServices;

using InventorySystem.Items.Pickups;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.MicroHID
{
	public class MicroHIDPickup : CollisionDetectionPickup
	{
		[SyncVar]
		public float Energy;

		[SerializeField]
		private Transform _needle;

		private float _prevEnergy = -1f;

		private void Update()
		{
			MicroHIDViewmodel.LerpGauge(_needle, Energy, 1f);
			_prevEnergy = Energy;
		}
	}
}
