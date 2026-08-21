using UnityEngine;

namespace MapGeneration.Distributors
{
    public abstract class DistributorSpawnpointBase : MonoBehaviour
    {
        private RoomName _roomName;

        private bool _roomSet;

        public RoomName RoomName
        {
            get
            {
                if (!_roomSet)
                {
                    _roomName = RoomIdUtils.RoomAtPosition(base.transform.position).Name;
                    _roomSet = true;
                }
                return _roomName;
            }
        }

        private void Awake()
        {
            base.transform.localScale = Vector3.one;
        }
    }
}
