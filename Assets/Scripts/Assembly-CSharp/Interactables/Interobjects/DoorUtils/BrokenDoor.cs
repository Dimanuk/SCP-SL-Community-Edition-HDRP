namespace Interactables.Interobjects.DoorUtils
{
    public class BrokenDoor : global::UnityEngine.MonoBehaviour
    {
        [global::System.Serializable]
        public struct BrokenDoorPart
        {
            [global::UnityEngine.SerializeField]
            private global::UnityEngine.Rigidbody _rigidbody;

            [global::UnityEngine.SerializeField]
            private float _force;

            [global::UnityEngine.SerializeField]
            private float _randomTorque;

            [global::UnityEngine.SerializeField]
            private global::UnityEngine.Vector3 _direction;

            public void Init()
            {
                _rigidbody.linearVelocity = _rigidbody.transform.TransformDirection(_direction.normalized) * _force;
                _rigidbody.angularVelocity = new global::UnityEngine.Vector3((global::UnityEngine.Random.value - 0.5f) * 2f, (global::UnityEngine.Random.value - 0.5f) * 2f, (global::UnityEngine.Random.value - 0.5f) * 2f) * _randomTorque;
            }

            public void Freeze()
            {
                global::UnityEngine.Collider[] components = _rigidbody.GetComponents<global::UnityEngine.Collider>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].enabled = false;
                }
                global::UnityEngine.Object.Destroy(_rigidbody);
            }
        }

        [global::UnityEngine.SerializeField]
        private global::Interactables.Interobjects.DoorUtils.BrokenDoor.BrokenDoorPart[] _parts;

        [global::UnityEngine.SerializeField]
        private float _timeUntilFreeze = 5f;

        private void Start()
        {
            global::Interactables.Interobjects.DoorUtils.BrokenDoor.BrokenDoorPart[] parts = _parts;
            foreach (global::Interactables.Interobjects.DoorUtils.BrokenDoor.BrokenDoorPart brokenDoorPart in parts)
            {
                brokenDoorPart.Init();
            }
        }

        private void Update()
        {
            _timeUntilFreeze -= global::UnityEngine.Time.deltaTime;
            if (_timeUntilFreeze < 0f)
            {
                global::Interactables.Interobjects.DoorUtils.BrokenDoor.BrokenDoorPart[] parts = _parts;
                foreach (global::Interactables.Interobjects.DoorUtils.BrokenDoor.BrokenDoorPart brokenDoorPart in parts)
                {
                    brokenDoorPart.Freeze();
                }
                base.enabled = false;
            }
        }
    }
}
