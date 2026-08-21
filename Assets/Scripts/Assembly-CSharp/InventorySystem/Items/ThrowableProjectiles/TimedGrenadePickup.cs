
using Footprinting;
using InventorySystem.Items.Pickups;
using UnityEngine;

namespace InventorySystem.Items.ThrowableProjectiles
{
	public class TimedGrenadePickup : CollisionDetectionPickup, IExplosionTrigger
	{
		private bool _replaceNextFrame;

		private Footprint _attacker;

		private const float ActivationRange = 0.4f;

        private void Update()
        {
            if (_replaceNextFrame && global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(Info.ItemId, out var value) && value is global::InventorySystem.Items.ThrowableProjectiles.ThrowableItem throwableItem)
            {
                global::InventorySystem.Items.ThrowableProjectiles.ThrownProjectile thrownProjectile = global::UnityEngine.Object.Instantiate(throwableItem.Projectile);
                if (thrownProjectile.TryGetComponent<global::UnityEngine.Rigidbody>(out var component))
                {
                    component.position = Rb.position;
                    component.rotation = Rb.rotation;
                    component.linearVelocity = Rb.linearVelocity;
                    component.angularVelocity = Rb.angularVelocity;
                }
                Info.Locked = true;
                thrownProjectile.Info = Info;
                thrownProjectile.PreviousOwner = _attacker;
                global::Mirror.NetworkServer.Spawn(thrownProjectile.gameObject);
                thrownProjectile.InfoReceived(default(global::InventorySystem.Items.Pickups.PickupSyncInfo), Info);
                thrownProjectile.ServerActivate();
                DestroySelf();
                _replaceNextFrame = false;
            }
        }

        public void OnExplosionDetected(global::Footprinting.Footprint attacker, global::UnityEngine.Vector3 source, float range)
        {
            if (!(global::UnityEngine.Vector3.Distance(base.transform.position, source) / range > 0.4f) && !global::UnityEngine.Physics.Linecast(base.gameObject.transform.position, source, global::InventorySystem.Items.MicroHID.MicroHIDItem.WallMask))
            {
                _replaceNextFrame = true;
                _attacker = attacker;
            }
        }
	}
}
