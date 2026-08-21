
using UnityEngine;

namespace InventorySystem.Items.Pickups
{
	public class SimplifiedPhysicsPickup : IPickupPhysicsModule
	{
		private const float RefreshRate = 4f;

		private const byte PositionSmoothing = 2;

		private const byte AngularSmoothing = 3;

		private const byte LerpCutoffValue = 10;

		private readonly ItemPickupBase _targetPickup;

		private readonly byte _timerResetMark;

		private Rigidbody _rb;

		private byte _timer;

		private PickupSyncInfo _receivedInfo;

        public SimplifiedPhysicsPickup(global::InventorySystem.Items.Pickups.ItemPickupBase targetPickup)
        {
            _targetPickup = targetPickup;
            _timerResetMark = (byte)global::UnityEngine.Mathf.RoundToInt(1f / global::UnityEngine.Time.fixedDeltaTime / 4f);
            _rb = _targetPickup.GetComponent<global::UnityEngine.Rigidbody>();
            if (!global::Mirror.NetworkServer.active)
            {
                _rb.isKinematic = true;
            }
        }

        public void DestroyModule()
		{
			
		}

        public void UpdateInfo(global::InventorySystem.Items.Pickups.PickupSyncInfo info)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                _receivedInfo = info;
            }
        }

        public void UpdatePhysics()
        {
            if (global::Mirror.NetworkServer.active)
            {
                _timer++;
                if (_timer >= _timerResetMark)
                {
                    _timer = 0;
                    _targetPickup.RefreshPositionAndRotation();
                }
            }
            else if (global::UnityEngine.Vector3.Distance(_receivedInfo.Position, _rb.position) < 10f)
            {
                _rb.position = global::UnityEngine.Vector3.Lerp(_rb.position, _receivedInfo.Position, 2f * global::UnityEngine.Time.fixedDeltaTime);
                _rb.rotation = global::UnityEngine.Quaternion.Lerp(_rb.rotation, _receivedInfo.Rotation, 3f * global::UnityEngine.Time.fixedDeltaTime);
            }
            else
            {
                _rb.angularVelocity = global::UnityEngine.Vector3.zero;
                _rb.position = _receivedInfo.Position;
            }
        }
    }
}
