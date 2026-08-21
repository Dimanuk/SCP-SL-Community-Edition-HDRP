using System;
using System.Runtime.CompilerServices;

using InventorySystem.Items.Pickups;
using UnityEngine;

namespace InventorySystem.Items.ThrowableProjectiles
{
	public class ThrownProjectile : CollisionDetectionPickup
	{
		public float AdditionalGravity;

		[SerializeField]
		private GameObject _renderersRoot;

		public static event Action<ThrownProjectile> OnProjectileSpawned;

        protected override void Start()
        {
            base.Start();
            global::InventorySystem.Items.ThrowableProjectiles.ThrownProjectile.OnProjectileSpawned?.Invoke(this);
        }

        protected virtual void Update()
        {
            Rb.AddForce(global::UnityEngine.Vector3.down * AdditionalGravity, global::UnityEngine.ForceMode.Force);
        }

        public virtual void ToggleRenderers(bool state)
		{
			_renderersRoot.SetActive(state);
		}

		public virtual void ServerActivate()
		{
		}
	}
}
