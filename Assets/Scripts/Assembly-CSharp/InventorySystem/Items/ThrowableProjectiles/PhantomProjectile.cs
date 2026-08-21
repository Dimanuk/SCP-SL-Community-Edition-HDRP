
using UnityEngine;

namespace InventorySystem.Items.ThrowableProjectiles
{
	public class PhantomProjectile : MonoBehaviour
	{
		public Rigidbody Rigidbody;

		[SerializeField]
		private float _minimalExistenceTime;

		[SerializeField]
		private float _transitionTime;

		[SerializeField]
		private Vector3 _startScale;

		[SerializeField]
		private Vector3 _targetScale;

		private const float AutoDestroyTime = 0.5f;

		private ushort _projectileSerial;

		private float _scaleFactor;

		private float _transitionFactor;

		private float _replaceTime;

		private float _gravity;

		private bool _hasPickup;

		private ThrownProjectile _pickupToReplace;

        private float CurTime => global::UnityEngine.Time.timeSinceLevelLoad;

        public void Init(ushort serial, global::UnityEngine.Transform cam, global::UnityEngine.Vector3 relativePosition, float gravity)
        {
            global::InventorySystem.Items.ThrowableProjectiles.ThrownProjectile.OnProjectileSpawned += OnSpawned;
            _projectileSerial = serial;
            _replaceTime = CurTime + _minimalExistenceTime;
            _scaleFactor = -1f;
            _transitionFactor = -1f;
            _gravity = gravity;
            global::UnityEngine.Object.Destroy(base.gameObject, _minimalExistenceTime + _transitionTime + 0.5f);
            base.transform.SetParent(null);
            base.transform.position = cam.TransformPoint(relativePosition);
            base.transform.localScale = _startScale;
        }

        private void OnDestroy()
        {
            global::InventorySystem.Items.ThrowableProjectiles.ThrownProjectile.OnProjectileSpawned -= OnSpawned;
        }

        private void Update()
        {
            Rigidbody.AddForce(global::UnityEngine.Vector3.down * _gravity, global::UnityEngine.ForceMode.Force);
            if (_scaleFactor < 1f)
            {
                _scaleFactor = global::UnityEngine.Mathf.Clamp01(_scaleFactor + global::UnityEngine.Time.deltaTime / _minimalExistenceTime);
                base.transform.localScale = global::UnityEngine.Vector3.Lerp(_startScale, _targetScale, _scaleFactor);
            }
            if (_hasPickup && !(CurTime < _replaceTime))
            {
                Replace();
            }
        }

        private void Replace()
        {
            if (_pickupToReplace != null)
            {
                if (_transitionFactor < 1f && _pickupToReplace.TryGetComponent<global::UnityEngine.Rigidbody>(out var component))
                {
                    _transitionFactor = global::UnityEngine.Mathf.Clamp01(_transitionFactor + global::UnityEngine.Time.deltaTime / _transitionTime);
                    Rigidbody.MovePosition(global::UnityEngine.Vector3.Lerp(Rigidbody.position, component.position, _transitionFactor));
                    Rigidbody.linearVelocity = global::UnityEngine.Vector3.Lerp(Rigidbody.linearVelocity, component.linearVelocity, _transitionFactor);
                    Rigidbody.rotation = global::UnityEngine.Quaternion.Lerp(Rigidbody.rotation, component.rotation, _transitionFactor);
                    return;
                }
                _pickupToReplace.ToggleRenderers(state: true);
            }
            global::UnityEngine.Object.Destroy(base.gameObject);
        }

        private void OnSpawned(global::InventorySystem.Items.ThrowableProjectiles.ThrownProjectile projectile)
        {
            if (projectile.Info.Serial == _projectileSerial)
            {
                _hasPickup = true;
                _pickupToReplace = projectile;
                if (!global::Mirror.NetworkServer.active)
                {
                    projectile.ToggleRenderers(state: false);
                }
                else
                {
                    Replace();
                }
            }
        }
    }
}
