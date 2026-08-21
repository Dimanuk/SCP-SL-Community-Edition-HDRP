using MapGeneration;
using UnityEngine;

namespace CustomCulling
{
    public class DynamicCullableBase : CullableBase
    {
        private const float MovedSqrThreshold = 0.0001f;

        public bool IgnoreRoomCulling = false;

        private Transform _transform;
        private Vector3 _savedPosition;

        public CullableRoom LinkedRoom { get; private set; }

        internal override bool CullEnabled
        {
            get
            {
                if (IgnoreRoomCulling || LinkedRoom == null)
                    return true;

                return LinkedRoom.CurrentRoomState != CullableRoom.RoomState.Disabled;
            }
        }

        public void UpdateRoom(bool updateChildDynamicCullableBases = false)
        {
            if (!CullingManager.Initialized || _transform == null || IgnoreRoomCulling)
                return;

            RoomIdentifier roomId = RoomIdUtils.RoomAtPositionRaycasts(_transform.position, true);
            CullableRoom room = roomId != null ? roomId.GetComponent<CullableRoom>() : null;

            if (room == null)
            {
                UpdateBehaviours();
                return;
            }

            if (LinkedRoom != null && LinkedRoom.gameObject == room.gameObject)
            {
                if (Lights != null)
                {
                    for (int i = Lights.Count - 1; i >= 0; i--)
                    {
                        Light light = Lights[i];
                        if (light == null) { Lights.RemoveAt(i); continue; }
                        LinkedRoom.UpdateNearbyLight(light);
                    }
                }
                return;
            }

            if (!updateChildDynamicCullableBases)
            {
                LinkToRoom(room);
                return;
            }

            foreach (DynamicCullableBase child in GetComponentsInChildren<DynamicCullableBase>())
                if (child != null)
                    child.LinkToRoom(room);
        }

        protected override void OnAwake()
        {
            _transform = GetComponent<Transform>();
        }

        private void Update()
        {
            if (!CullingManager.Initialized || _transform == null || IgnoreRoomCulling)
                return;

            Vector3 currentPos = _transform.position;
            if ((currentPos - _savedPosition).sqrMagnitude > MovedSqrThreshold)
            {
                _savedPosition = currentPos;
                UpdateRoom(false);
            }
        }

        private void OnDestroy()
        {
            if (LinkedRoom != null)
            {
                LinkedRoom.RemoveCullableBase(this);
                LinkedRoom = null;
            }
        }

        private void LinkToRoom(CullableRoom room)
        {
            LinkedRoom?.RemoveCullableBase(this);
            LinkedRoom = room;
            LinkedRoom?.AddCullableBase(this);

            if (StaticObject)
                enabled = false;
        }
    }
}
