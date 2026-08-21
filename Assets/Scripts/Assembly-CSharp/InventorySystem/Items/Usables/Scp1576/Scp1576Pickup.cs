using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using InventorySystem.Items.Pickups;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp1576
{
	public class Scp1576Pickup : CollisionDetectionPickup
	{
		private byte _prevSyncHorn;

		[SyncVar]
		private byte _syncHorn;

		[SerializeField]
		private Transform _horn;

		[SerializeField]
		private Vector3 _posZero;

		[SerializeField]
		private Vector3 _posOne;

        public float HornPos
        {
            get
            {
                return (float)(int)_syncHorn / 255f;
            }
            set
            {
                _syncHorn = (byte)global::UnityEngine.Mathf.Clamp(global::UnityEngine.Mathf.RoundToInt(value * 255f), 0, 255);
            }
        }

        public static event Action<ushort, float> OnHornPositionUpdated;

        private void Update()
        {
            if (_prevSyncHorn != _syncHorn)
            {
                float hornPos = HornPos;
                _horn.localPosition = global::UnityEngine.Vector3.Lerp(_posZero, _posOne, hornPos);
                global::InventorySystem.Items.Usables.Scp1576.Scp1576Pickup.OnHornPositionUpdated?.Invoke(Info.Serial, hornPos);
                _prevSyncHorn = _syncHorn;
            }
        }
    }
}
