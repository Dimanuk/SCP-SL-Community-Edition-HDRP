using UnityEngine;

namespace InventorySystem.Items.Usables.Scp244
{
    public class Scp244Lid : MonoBehaviour
    {
        [SerializeField]
        private Scp244DeployablePickup _pickup;

        [SerializeField]
        private Vector3 _offset;

        [SerializeField]
        private float _upDot;

        [SerializeField]
        private AudioSource _pressureSound;

        private void Update()
        {
            if (_pickup == null)
                return;

            if (_pickup.State != Scp244State.Active)
                return;

            float dot = Vector3.Dot(transform.up, Vector3.up);

            if (dot <= _upDot)
            {
                if (_pressureSound != null)
                    _pressureSound.enabled = true;
            }
            else
            {
                transform.localPosition = transform.localPosition + _offset;
            }

            enabled = false;

            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }
}