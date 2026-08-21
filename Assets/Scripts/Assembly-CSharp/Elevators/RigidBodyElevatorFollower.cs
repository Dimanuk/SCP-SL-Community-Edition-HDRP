using UnityEngine;

namespace Elevators
{
    public class RigidBodyElevatorFollower : ElevatorFollowerBase
    {
        public Rigidbody Rigidbody;

        private bool _unlinked;

        protected override void Awake()
        {
            base.Awake();
            LastPosition = Rigidbody.position;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (!_unlinked && !Rigidbody.IsSleeping())
            {
                LastPosition = Rigidbody.position;
            }
        }

        private void Reset()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        public void Unlink()
        {
            _unlinked = true;
        }
    }
}
